using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NextOnServices.Infrastructure.Models.APIProjects;

public sealed class LucidMarketplaceCallbackSignatureVerificationResult
{
    public bool IsValid { get; init; }

    public string Message { get; init; } = "Lucid Marketplace callback signature verification failed.";

    public int StatusCode { get; init; } = 401;

    public string? SupplierCode { get; init; }

    public string? SubscriptionType { get; init; }

    public int? SubscriptionId { get; init; }

    public string? KeyId { get; init; }

    public long? Timestamp { get; init; }

    public string ReasonCode { get; init; } = "Unknown";

    public string? DiagnosticMessage { get; init; }
}

public static class LucidMarketplaceCallbackSignatureHelper
{
    private static readonly string[] PublicKeyPropertyNames =
    {
        "public_key",
        "publicKey",
        "webhook_public_key",
        "webhookPublicKey",
        "ecdsa_public_key",
        "ecdsaPublicKey",
        "encoded_public_key",
        "encodedPublicKey",
        "verification_key",
        "verificationKey",
        "pem"
    };

    private static readonly string[] KeyIdPropertyNames =
    {
        "key_id",
        "keyId",
        "kid",
        "id",
        "key_identifier",
        "keyIdentifier",
        "signing_key_id",
        "signingKeyId",
        "webhook_key_id",
        "webhookKeyId"
    };

    public static void PopulateWebhookVerificationMetadata(
        LucidMarketplaceSubscriptionDTO subscription,
        LucidMarketplaceSubscriptionDTO? existingSubscription = null)
    {
        ArgumentNullException.ThrowIfNull(subscription);

        var extractedCandidates = ExtractCandidatesFromJson(subscription.ResponsePayloadSnapshot, subscription).ToList();
        var preservedCandidates = existingSubscription == null
            ? new List<LucidMarketplaceWebhookKeyCandidate>()
            : ExtractStructuredKeyCandidates(existingSubscription).ToList();

        var mergedCandidates = extractedCandidates
            .Concat(preservedCandidates)
            .Where(CandidateHasUsableVerificationKey)
            .GroupBy(GetCandidateIdentity)
            .Select(group => group.First())
            .ToList();

        var primaryCandidate = extractedCandidates
            .Concat(preservedCandidates)
            .FirstOrDefault(CandidateHasUsableVerificationKey);

        subscription.WebhookKeyId = primaryCandidate?.KeyId ?? existingSubscription?.WebhookKeyId;
        subscription.WebhookKeyIdFull = primaryCandidate?.KeyIdFull ?? existingSubscription?.WebhookKeyIdFull;
        subscription.WebhookPublicKey = primaryCandidate?.PublicKey ?? existingSubscription?.WebhookPublicKey;
        subscription.WebhookSecuritySnapshot = mergedCandidates.Count > 0
            ? JsonSerializer.Serialize(mergedCandidates.Select(candidate => new
            {
                key_id = candidate.KeyId,
                full_key_id = candidate.KeyIdFull,
                public_key = candidate.PublicKey,
                jwk_x = candidate.JwkX,
                jwk_y = candidate.JwkY
            }))
            : existingSubscription?.WebhookSecuritySnapshot;
    }

    public static LucidMarketplaceCallbackSignatureVerificationResult Verify(
        string? signatureHeader,
        string requestBody,
        IEnumerable<LucidMarketplaceSubscriptionDTO>? subscriptions,
        string subscriptionType,
        string? supplierCode,
        TimeSpan? tolerance = null)
    {
        var normalizedSubscriptionType = NormalizeSubscriptionType(subscriptionType);

        if (string.IsNullOrWhiteSpace(requestBody))
        {
            return Failure(
                reasonCode: "MissingRequestBody",
                message: "Lucid Marketplace callback request body is required.",
                statusCode: 400,
                supplierCode: supplierCode,
                subscriptionType: normalizedSubscriptionType);
        }

        if (!TryParseSignatureHeader(signatureHeader, out var parsedHeader, out var parseReasonCode, out var parseError))
        {
            return Failure(
                reasonCode: parseReasonCode,
                message: parseError ?? "Lucid Marketplace callback signature header is invalid.",
                statusCode: parseReasonCode is "MissingSignatureHeader" or "MalformedHeader" or "MissingTimestamp" or "MissingKeyId" ? 400 : 401,
                supplierCode: supplierCode,
                subscriptionType: normalizedSubscriptionType,
                diagnosticMessage: BuildDiagnosticMessage(
                    requestBody,
                    timestampText: null,
                    parsedTimestamp: null,
                    signatureHeader: signatureHeader,
                    extra: "Signature header parse failed."));
        }

        if (!TryValidateTimestamp(parsedHeader.TimestampText, tolerance ?? TimeSpan.FromMinutes(5), out var parsedTimestamp, out var timestampReasonCode, out var timestampError))
        {
            return Failure(
                reasonCode: timestampReasonCode,
                message: timestampError ?? "Lucid Marketplace callback timestamp is invalid.",
                statusCode: timestampReasonCode == "StaleTimestamp" ? 401 : 400,
                supplierCode: supplierCode,
                subscriptionType: normalizedSubscriptionType,
                timestamp: long.TryParse(parsedHeader.TimestampText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedHeaderTimestamp) ? parsedHeaderTimestamp : null,
                diagnosticMessage: BuildDiagnosticMessage(
                    requestBody,
                    parsedHeader.TimestampText,
                    long.TryParse(parsedHeader.TimestampText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var diagnosticHeaderTimestamp) ? diagnosticHeaderTimestamp : null,
                    parsedHeader.Signatures,
                    signatureHeader: signatureHeader,
                    extra: "Timestamp validation failed."));
        }

        var orderedSubscriptions = (subscriptions ?? Enumerable.Empty<LucidMarketplaceSubscriptionDTO>())
            .Where(subscription =>
                string.Equals(NormalizeSubscriptionType(subscription.SubscriptionType), normalizedSubscriptionType, StringComparison.OrdinalIgnoreCase) &&
                (string.IsNullOrWhiteSpace(supplierCode) ||
                 string.Equals(subscription.SupplierCode, supplierCode, StringComparison.OrdinalIgnoreCase)))
            .OrderByDescending(subscription => subscription.IsActive)
            .ThenByDescending(subscription => subscription.LastValidatedOn ?? subscription.ModifiedDate ?? subscription.CreatedDate)
            .ToList();

        if (orderedSubscriptions.Count == 0)
        {
            return Failure(
                reasonCode: "PublicKeyNotFound",
                message: $"No {normalizedSubscriptionType} subscription record was found for Lucid Marketplace callback verification.",
                statusCode: 401,
                supplierCode: supplierCode,
                subscriptionType: normalizedSubscriptionType,
                timestamp: parsedTimestamp,
                diagnosticMessage: BuildDiagnosticMessage(
                    requestBody,
                    parsedHeader.TimestampText,
                    parsedTimestamp,
                    parsedHeader.Signatures,
                    signatureHeader: signatureHeader,
                    extra: "No matching subscription record was found."));
        }

        var activeSubscriptions = orderedSubscriptions.Where(subscription => subscription.IsActive).ToList();
        var preferredSubscriptions = activeSubscriptions.Count > 0 ? activeSubscriptions : orderedSubscriptions;

        var candidates = preferredSubscriptions
            .SelectMany(ExtractKeyCandidates)
            .Where(CandidateHasUsableVerificationKey)
            .GroupBy(GetCandidateIdentity)
            .Select(group => group.First())
            .ToList();

        if (candidates.Count == 0 && activeSubscriptions.Count > 0 && activeSubscriptions.Count < orderedSubscriptions.Count)
        {
            candidates = orderedSubscriptions
                .SelectMany(ExtractKeyCandidates)
                .Where(CandidateHasUsableVerificationKey)
                .GroupBy(GetCandidateIdentity)
                .Select(group => group.First())
                .ToList();
        }

        if (candidates.Count == 0)
        {
            return Failure(
                reasonCode: "PublicKeyNotFound",
                message: $"Lucid Marketplace webhook public key metadata is missing for the active {normalizedSubscriptionType} subscription. Refresh the subscription before accepting callbacks.",
                statusCode: 401,
                supplierCode: supplierCode,
                subscriptionType: normalizedSubscriptionType,
                timestamp: parsedTimestamp,
                diagnosticMessage: BuildDiagnosticMessage(
                    requestBody,
                    parsedHeader.TimestampText,
                    parsedTimestamp,
                    parsedHeader.Signatures,
                    signatureHeader: signatureHeader,
                    extra: $"MatchedSubscriptions={orderedSubscriptions.Count}; Candidate extraction returned no usable verification keys."));
        }

        var payloadBytes = Encoding.UTF8.GetBytes($"{parsedHeader.TimestampText}.{requestBody}");
        var attemptDiagnostics = new List<LucidMarketplaceVerificationAttemptDiagnostic>();
        string? firstMissingCandidateKeyId = null;
        string? firstVerificationFailureKeyId = null;
        bool hadDecodableSignature = false;
        bool hadMatchingKeyMaterial = false;

        foreach (var signatureEntry in parsedHeader.Signatures)
        {
            var matchingCandidates = candidates
                .Where(candidate => CandidateMatchesKeyId(candidate, signatureEntry.KeyId))
                .ToList();

            if (matchingCandidates.Count == 0)
            {
                firstMissingCandidateKeyId ??= signatureEntry.KeyId;
                attemptDiagnostics.Add(new LucidMarketplaceVerificationAttemptDiagnostic
                {
                    HeaderKeyId = signatureEntry.KeyId,
                    SignatureDecodeSucceeded = false,
                    FailureMessage = "No stored webhook key matched the callback header key id."
                });
                continue;
            }

            hadMatchingKeyMaterial = true;

            if (!TryDecodeSignature(signatureEntry.Signature, out var signatureBytes, out var usedBase64Url, out var signatureDecodeError))
            {
                attemptDiagnostics.Add(new LucidMarketplaceVerificationAttemptDiagnostic
                {
                    HeaderKeyId = signatureEntry.KeyId,
                    SignatureDecodeSucceeded = false,
                    UsedBase64Url = usedBase64Url,
                    FailureMessage = signatureDecodeError
                });
                continue;
            }

            hadDecodableSignature = true;

            foreach (var candidate in matchingCandidates)
            {
                var verificationAttempt = VerifyWithCandidate(candidate, payloadBytes, signatureBytes);
                attemptDiagnostics.Add(new LucidMarketplaceVerificationAttemptDiagnostic
                {
                    HeaderKeyId = signatureEntry.KeyId,
                    CandidateSubscriptionId = candidate.SubscriptionId,
                    CandidateKeyId = candidate.KeyId,
                    CandidateKeyIdFull = candidate.KeyIdFull,
                    SignatureDecodeSucceeded = true,
                    UsedBase64Url = usedBase64Url,
                    KeyImportSucceeded = verificationAttempt.KeyImportSucceeded,
                    SignatureFormat = verificationAttempt.SignatureFormat,
                    VerificationSucceeded = verificationAttempt.IsValid,
                    FailureMessage = verificationAttempt.ErrorMessage
                });

                if (verificationAttempt.IsValid)
                {
                    return Success(
                        normalizedSubscriptionType,
                        candidate,
                        parsedTimestamp,
                        supplierCode,
                        signatureEntry.KeyId,
                        BuildDiagnosticMessage(
                            requestBody,
                            parsedHeader.TimestampText,
                            parsedTimestamp,
                            parsedHeader.Signatures,
                            candidates,
                            attemptDiagnostics,
                            signatureHeader,
                            "ECDSA verification succeeded."));
                }

                firstVerificationFailureKeyId ??= signatureEntry.KeyId ?? candidate.KeyId ?? candidate.KeyIdFull;
            }
        }

        var diagnosticMessage = BuildDiagnosticMessage(
            requestBody,
            parsedHeader.TimestampText,
            parsedTimestamp,
            parsedHeader.Signatures,
            candidates,
            attemptDiagnostics,
            signatureHeader,
            "ECDSA verification did not succeed for any candidate key.");

        if (!hadMatchingKeyMaterial)
        {
            return Failure(
                reasonCode: "PublicKeyNotFound",
                message: !string.IsNullOrWhiteSpace(firstMissingCandidateKeyId)
                    ? $"Lucid Marketplace webhook public key was not found for signing key '{firstMissingCandidateKeyId}'."
                    : "Lucid Marketplace callback signing key could not be matched to the stored webhook metadata.",
                statusCode: 401,
                supplierCode: supplierCode,
                subscriptionType: normalizedSubscriptionType,
                keyId: firstMissingCandidateKeyId,
                timestamp: parsedTimestamp,
                diagnosticMessage: diagnosticMessage);
        }

        if (!hadDecodableSignature)
        {
            return Failure(
                reasonCode: "MalformedHeader",
                message: "Lucid Marketplace callback signature value is malformed or could not be decoded.",
                statusCode: 400,
                supplierCode: supplierCode,
                subscriptionType: normalizedSubscriptionType,
                timestamp: parsedTimestamp,
                diagnosticMessage: diagnosticMessage);
        }

        return Failure(
            reasonCode: "EcdsaVerificationFailed",
            message: !string.IsNullOrWhiteSpace(firstVerificationFailureKeyId)
                ? $"Lucid Marketplace callback ECDSA verification failed for signing key '{firstVerificationFailureKeyId}'."
                : "Lucid Marketplace callback ECDSA verification failed.",
            statusCode: 401,
            supplierCode: supplierCode,
            subscriptionType: normalizedSubscriptionType,
            keyId: firstVerificationFailureKeyId,
            timestamp: parsedTimestamp,
            diagnosticMessage: diagnosticMessage);
    }

    private static LucidMarketplaceCallbackSignatureVerificationResult Success(
        string normalizedSubscriptionType,
        LucidMarketplaceWebhookKeyCandidate candidate,
        long parsedTimestamp,
        string? requestedSupplierCode,
        string? headerKeyId,
        string diagnosticMessage)
    {
        return new LucidMarketplaceCallbackSignatureVerificationResult
        {
            IsValid = true,
            Message = "Lucid Marketplace callback signature validated.",
            StatusCode = 200,
            SupplierCode = string.IsNullOrWhiteSpace(requestedSupplierCode) ? candidate.SupplierCode : requestedSupplierCode,
            SubscriptionType = normalizedSubscriptionType,
            SubscriptionId = candidate.SubscriptionId,
            KeyId = candidate.KeyId ?? headerKeyId ?? candidate.KeyIdFull,
            Timestamp = parsedTimestamp,
            ReasonCode = "Accepted",
            DiagnosticMessage = diagnosticMessage
        };
    }

    private static LucidMarketplaceCallbackSignatureVerificationResult Failure(
        string reasonCode,
        string message,
        int statusCode,
        string? supplierCode = null,
        string? subscriptionType = null,
        int? subscriptionId = null,
        string? keyId = null,
        long? timestamp = null,
        string? diagnosticMessage = null)
    {
        return new LucidMarketplaceCallbackSignatureVerificationResult
        {
            IsValid = false,
            Message = message,
            StatusCode = statusCode,
            SupplierCode = supplierCode,
            SubscriptionType = subscriptionType,
            SubscriptionId = subscriptionId,
            KeyId = keyId,
            Timestamp = timestamp,
            ReasonCode = reasonCode,
            DiagnosticMessage = diagnosticMessage
        };
    }

    private static IEnumerable<LucidMarketplaceWebhookKeyCandidate> ExtractKeyCandidates(LucidMarketplaceSubscriptionDTO subscription)
    {
        foreach (var candidate in ExtractStructuredKeyCandidates(subscription))
        {
            yield return candidate;
        }

        foreach (var candidate in ExtractCandidatesFromJson(subscription.ResponsePayloadSnapshot, subscription))
        {
            yield return candidate;
        }
    }

    private static IEnumerable<LucidMarketplaceWebhookKeyCandidate> ExtractStructuredKeyCandidates(LucidMarketplaceSubscriptionDTO subscription)
    {
        if (!string.IsNullOrWhiteSpace(subscription.WebhookPublicKey) ||
            !string.IsNullOrWhiteSpace(subscription.WebhookKeyId) ||
            !string.IsNullOrWhiteSpace(subscription.WebhookKeyIdFull))
        {
            yield return new LucidMarketplaceWebhookKeyCandidate
            {
                SubscriptionId = subscription.LucidMarketplaceSubscriptionId,
                KeyId = NormalizeKeyId(subscription.WebhookKeyId ?? subscription.WebhookKeyIdFull),
                KeyIdFull = NormalizeTrimmedValue(subscription.WebhookKeyIdFull ?? subscription.WebhookKeyId),
                PublicKey = subscription.WebhookPublicKey,
                SupplierCode = subscription.SupplierCode,
                SubscriptionType = NormalizeSubscriptionType(subscription.SubscriptionType)
            };
        }

        if (string.IsNullOrWhiteSpace(subscription.WebhookSecuritySnapshot))
        {
            yield break;
        }

        foreach (var candidate in ExtractCandidatesFromJson(subscription.WebhookSecuritySnapshot, subscription))
        {
            yield return candidate;
        }
    }

    private static IEnumerable<LucidMarketplaceWebhookKeyCandidate> ExtractCandidatesFromJson(
        string? payloadSnapshot,
        LucidMarketplaceSubscriptionDTO subscription)
    {
        if (string.IsNullOrWhiteSpace(payloadSnapshot))
        {
            yield break;
        }

        JsonDocument? document;
        try
        {
            document = JsonDocument.Parse(payloadSnapshot);
        }
        catch
        {
            yield break;
        }

        using (document)
        {
            foreach (var objectToken in EnumerateObjects(document.RootElement))
            {
                var publicKey = GetJsonString(objectToken, PublicKeyPropertyNames) ??
                                GetHeuristicPublicKey(objectToken) ??
                                GetNestedJsonString(objectToken, PublicKeyPropertyNames);
                var fullKeyId = GetJsonString(objectToken, KeyIdPropertyNames) ??
                                GetHeuristicKeyId(objectToken) ??
                                GetNestedJsonString(objectToken, KeyIdPropertyNames);

                if (!string.IsNullOrWhiteSpace(publicKey))
                {
                    yield return new LucidMarketplaceWebhookKeyCandidate
                    {
                        SubscriptionId = subscription.LucidMarketplaceSubscriptionId,
                        KeyId = NormalizeKeyId(fullKeyId),
                        KeyIdFull = NormalizeTrimmedValue(fullKeyId),
                        PublicKey = publicKey,
                        SupplierCode = subscription.SupplierCode,
                        SubscriptionType = NormalizeSubscriptionType(subscription.SubscriptionType)
                    };
                    continue;
                }

                var jwkX = GetJsonString(objectToken, "jwk_x", "x") ?? GetNestedJsonString(objectToken, "jwk_x", "x");
                var jwkY = GetJsonString(objectToken, "jwk_y", "y") ?? GetNestedJsonString(objectToken, "jwk_y", "y");
                var curve = GetJsonString(objectToken, "crv") ?? GetNestedJsonString(objectToken, "crv");
                var keyType = GetJsonString(objectToken, "kty") ?? GetNestedJsonString(objectToken, "kty");

                if (!string.IsNullOrWhiteSpace(jwkX) &&
                    !string.IsNullOrWhiteSpace(jwkY) &&
                    (string.IsNullOrWhiteSpace(keyType) || string.Equals(keyType, "EC", StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(curve) || string.Equals(curve, "P-256", StringComparison.OrdinalIgnoreCase)))
                {
                    yield return new LucidMarketplaceWebhookKeyCandidate
                    {
                        SubscriptionId = subscription.LucidMarketplaceSubscriptionId,
                        KeyId = NormalizeKeyId(fullKeyId),
                        KeyIdFull = NormalizeTrimmedValue(fullKeyId),
                        JwkX = jwkX,
                        JwkY = jwkY,
                        SupplierCode = subscription.SupplierCode,
                        SubscriptionType = NormalizeSubscriptionType(subscription.SubscriptionType)
                    };
                }
            }
        }
    }

    private static bool TryParseSignatureHeader(
        string? signatureHeader,
        out LucidMarketplaceParsedSignatureHeader parsedHeader,
        out string reasonCode,
        out string? errorMessage)
    {
        parsedHeader = new LucidMarketplaceParsedSignatureHeader();
        reasonCode = "MalformedHeader";
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(signatureHeader))
        {
            reasonCode = "MissingSignatureHeader";
            errorMessage = "Missing X-Lucid-Signature header.";
            return false;
        }

        bool sawMalformedSegment = false;
        bool sawMissingKeyId = false;

        foreach (var part in signatureHeader.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (part.StartsWith("t:", StringComparison.OrdinalIgnoreCase))
            {
                parsedHeader.TimestampText = part[2..].Trim();
                continue;
            }

            var separatorIndex = part.IndexOf(':');
            if (separatorIndex <= 0)
            {
                sawMalformedSegment = true;
                continue;
            }

            var schemaVersion = part[..separatorIndex].Trim();
            if (!schemaVersion.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
                sawMalformedSegment = true;
                continue;
            }

            var components = part.Split(':', 3, StringSplitOptions.TrimEntries);
            if (components.Length < 3 || string.IsNullOrWhiteSpace(components[2]))
            {
                sawMalformedSegment = true;
                continue;
            }

            if (string.IsNullOrWhiteSpace(components[1]))
            {
                sawMissingKeyId = true;
                continue;
            }

            parsedHeader.Signatures.Add(new LucidMarketplaceSignatureEntry
            {
                SchemaVersion = components[0],
                KeyId = components[1].Trim(),
                Signature = components[2].Trim()
            });
        }

        if (string.IsNullOrWhiteSpace(parsedHeader.TimestampText))
        {
            reasonCode = "MissingTimestamp";
            errorMessage = "Missing Lucid Marketplace signature timestamp.";
            return false;
        }

        if (parsedHeader.Signatures.Count == 0)
        {
            reasonCode = sawMissingKeyId ? "MissingKeyId" : "MalformedHeader";
            errorMessage = sawMissingKeyId
                ? "Lucid Marketplace callback signature header is missing a signing key identifier."
                : "Lucid Marketplace callback signature header is malformed.";
            return false;
        }

        parsedHeader.HadMalformedSegments = sawMalformedSegment;
        return true;
    }

    private static bool TryValidateTimestamp(
        string timestampText,
        TimeSpan tolerance,
        out long parsedTimestamp,
        out string reasonCode,
        out string? errorMessage)
    {
        parsedTimestamp = 0;
        reasonCode = "MalformedHeader";
        errorMessage = null;

        if (!long.TryParse(timestampText, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedTimestamp))
        {
            errorMessage = "Lucid Marketplace callback signature timestamp is not a valid Unix timestamp.";
            return false;
        }

        var timestamp = DateTimeOffset.FromUnixTimeSeconds(parsedTimestamp);
        var now = DateTimeOffset.UtcNow;
        if (timestamp < now.Subtract(tolerance) || timestamp > now.Add(tolerance))
        {
            reasonCode = "StaleTimestamp";
            errorMessage = $"Lucid Marketplace callback timestamp is outside the allowed {tolerance.TotalMinutes:0}-minute window.";
            return false;
        }

        return true;
    }

    private static string NormalizeSubscriptionType(string? subscriptionType)
    {
        var normalized = (subscriptionType ?? string.Empty)
            .Trim()
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .ToLowerInvariant();

        return normalized switch
        {
            "respondentoutcomes" or "outcomes" => "RespondentOutcomes",
            _ => "Opportunities"
        };
    }

    private static string? NormalizeKeyId(string? keyId)
    {
        var normalized = NormalizeTrimmedValue(keyId);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return null;
        }

        return normalized.Length > 8 ? normalized[..8] : normalized;
    }

    private static string? NormalizeTrimmedValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static bool CandidateMatchesKeyId(LucidMarketplaceWebhookKeyCandidate candidate, string? headerKeyId)
    {
        if (string.IsNullOrWhiteSpace(headerKeyId))
        {
            return false;
        }

        var normalizedHeaderKeyId = NormalizeTrimmedValue(headerKeyId);
        var shortenedHeaderKeyId = NormalizeKeyId(headerKeyId);

        return KeyIdsMatch(normalizedHeaderKeyId, candidate.KeyId) ||
               KeyIdsMatch(normalizedHeaderKeyId, candidate.KeyIdFull) ||
               KeyIdsMatch(shortenedHeaderKeyId, candidate.KeyId) ||
               KeyIdsMatch(shortenedHeaderKeyId, candidate.KeyIdFull);
    }

    private static bool KeyIdsMatch(string? left, string? right)
    {
        if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
        {
            return false;
        }

        if (string.Equals(left, right, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return right.StartsWith(left, StringComparison.OrdinalIgnoreCase) ||
               left.StartsWith(right, StringComparison.OrdinalIgnoreCase);
    }

    private static bool CandidateHasUsableVerificationKey(LucidMarketplaceWebhookKeyCandidate candidate)
    {
        return !string.IsNullOrWhiteSpace(candidate.PublicKey) ||
               (!string.IsNullOrWhiteSpace(candidate.JwkX) && !string.IsNullOrWhiteSpace(candidate.JwkY));
    }

    private static string GetCandidateIdentity(LucidMarketplaceWebhookKeyCandidate candidate)
    {
        return string.Join("|",
            candidate.SubscriptionId?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            candidate.KeyId ?? string.Empty,
            candidate.KeyIdFull ?? string.Empty,
            candidate.PublicKey ?? string.Empty,
            candidate.JwkX ?? string.Empty,
            candidate.JwkY ?? string.Empty);
    }

    private static LucidMarketplaceCandidateVerificationResult VerifyWithCandidate(
        LucidMarketplaceWebhookKeyCandidate candidate,
        byte[] payloadBytes,
        byte[] signatureBytes)
    {
        var result = new LucidMarketplaceCandidateVerificationResult
        {
            SignatureFormat = nameof(DSASignatureFormat.Rfc3279DerSequence)
        };

        try
        {
            using var ecdsa = ECDsa.Create();
            if (!string.IsNullOrWhiteSpace(candidate.PublicKey))
            {
                result.KeyImportSucceeded = TryImportPublicKey(ecdsa, candidate.PublicKey, out var importError, out var usedBase64Url);
                result.UsedBase64UrlForKeyImport = usedBase64Url;
                result.ErrorMessage = importError;

                if (!result.KeyImportSucceeded)
                {
                    return result;
                }
            }
            else if (!string.IsNullOrWhiteSpace(candidate.JwkX) && !string.IsNullOrWhiteSpace(candidate.JwkY))
            {
                ecdsa.ImportParameters(new ECParameters
                {
                    Curve = ECCurve.NamedCurves.nistP256,
                    Q = new ECPoint
                    {
                        X = DecodeBase64Url(candidate.JwkX),
                        Y = DecodeBase64Url(candidate.JwkY)
                    }
                });
                result.KeyImportSucceeded = true;
            }
            else
            {
                result.ErrorMessage = "Candidate does not contain an importable public key.";
                return result;
            }

            result.IsValid = ecdsa.VerifyData(
                payloadBytes,
                signatureBytes,
                HashAlgorithmName.SHA256,
                DSASignatureFormat.Rfc3279DerSequence);
            return result;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    private static bool TryImportPublicKey(ECDsa ecdsa, string publicKey, out string? errorMessage, out bool usedBase64Url)
    {
        errorMessage = null;
        usedBase64Url = false;

        var normalizedKey = (publicKey ?? string.Empty)
            .Trim()
            .Replace("\\n", "\n", StringComparison.Ordinal)
            .Replace("\\r", "\r", StringComparison.Ordinal);

        try
        {
            if (normalizedKey.Contains("BEGIN PUBLIC KEY", StringComparison.OrdinalIgnoreCase) ||
                normalizedKey.Contains("BEGIN EC PUBLIC KEY", StringComparison.OrdinalIgnoreCase))
            {
                ecdsa.ImportFromPem(normalizedKey);
                return true;
            }

            var keyBytes = Convert.FromBase64String(normalizedKey);
            ecdsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
            return true;
        }
        catch (FormatException)
        {
            if (normalizedKey.Contains('-') || normalizedKey.Contains('_'))
            {
                try
                {
                    var keyBytes = DecodeBase64Url(normalizedKey);
                    ecdsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
                    usedBase64Url = true;
                    return true;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return false;
                }
            }

            errorMessage = "Webhook public key is not valid base64 SubjectPublicKeyInfo data.";
            return false;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }

    private static bool TryDecodeSignature(string signature, out byte[] signatureBytes, out bool usedBase64Url, out string? errorMessage)
    {
        signatureBytes = Array.Empty<byte>();
        usedBase64Url = false;
        errorMessage = null;
        if (string.IsNullOrWhiteSpace(signature))
        {
            errorMessage = "Signature text is empty.";
            return false;
        }

        if (TryDecodeBase64(signature, out signatureBytes))
        {
            return true;
        }

        if (signature.Contains('-') || signature.Contains('_'))
        {
            try
            {
                signatureBytes = DecodeBase64Url(signature);
                usedBase64Url = true;
                return signatureBytes.Length > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        errorMessage = "Signature text is not valid base64.";
        return false;
    }

    private static bool TryDecodeBase64(string value, out byte[] bytes)
    {
        try
        {
            bytes = Convert.FromBase64String(value);
            return true;
        }
        catch
        {
            bytes = Array.Empty<byte>();
            return false;
        }
    }

    private static byte[] DecodeBase64Url(string value)
    {
        var normalized = value.Replace('-', '+').Replace('_', '/');
        var padding = normalized.Length % 4;
        if (padding > 0)
        {
            normalized = normalized.PadRight(normalized.Length + (4 - padding), '=');
        }

        return Convert.FromBase64String(normalized);
    }

    private static string BuildDiagnosticMessage(
        string requestBody,
        string? timestampText,
        long? parsedTimestamp,
        IEnumerable<LucidMarketplaceSignatureEntry>? signatures = null,
        IEnumerable<LucidMarketplaceWebhookKeyCandidate>? candidates = null,
        IEnumerable<LucidMarketplaceVerificationAttemptDiagnostic>? attempts = null,
        string? signatureHeader = null,
        string? extra = null)
    {
        var previewLength = Math.Min(requestBody?.Length ?? 0, 200);
        var preview = previewLength > 0 ? requestBody[..previewLength] : string.Empty;
        var signedMessageLength = string.IsNullOrWhiteSpace(timestampText)
            ? 0
            : Encoding.UTF8.GetByteCount($"{timestampText}.{requestBody}");

        return JsonSerializer.Serialize(new
        {
            RawBodyLength = requestBody?.Length ?? 0,
            RawBodyUtf8ByteLength = Encoding.UTF8.GetByteCount(requestBody ?? string.Empty),
            RawBodyPreview = preview,
            TimestampText = timestampText,
            Timestamp = parsedTimestamp,
            SignedMessageLength = signedMessageLength,
            SignatureHeader = signatureHeader,
            HeaderKeyIds = (signatures ?? Enumerable.Empty<LucidMarketplaceSignatureEntry>())
                .Select(signature => signature.KeyId)
                .Where(keyId => !string.IsNullOrWhiteSpace(keyId))
                .ToArray(),
            CandidateCount = candidates?.Count() ?? 0,
            Candidates = (candidates ?? Enumerable.Empty<LucidMarketplaceWebhookKeyCandidate>())
                .Select(candidate => new
                {
                    candidate.SubscriptionId,
                    candidate.KeyId,
                    candidate.KeyIdFull,
                    HasPublicKey = !string.IsNullOrWhiteSpace(candidate.PublicKey),
                    HasJwk = !string.IsNullOrWhiteSpace(candidate.JwkX) && !string.IsNullOrWhiteSpace(candidate.JwkY)
                })
                .ToArray(),
            Attempts = (attempts ?? Enumerable.Empty<LucidMarketplaceVerificationAttemptDiagnostic>())
                .Select(attempt => new
                {
                    attempt.HeaderKeyId,
                    attempt.CandidateSubscriptionId,
                    attempt.CandidateKeyId,
                    attempt.CandidateKeyIdFull,
                    attempt.SignatureDecodeSucceeded,
                    attempt.UsedBase64Url,
                    attempt.KeyImportSucceeded,
                    attempt.SignatureFormat,
                    attempt.VerificationSucceeded,
                    attempt.FailureMessage
                })
                .ToArray(),
            Extra = extra
        });
    }

    private static IEnumerable<JsonElement> EnumerateObjects(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            yield return element;

            foreach (var property in element.EnumerateObject())
            {
                foreach (var child in EnumerateObjects(property.Value))
                {
                    yield return child;
                }
            }

            yield break;
        }

        if (element.ValueKind != JsonValueKind.Array)
        {
            yield break;
        }

        foreach (var item in element.EnumerateArray())
        {
            foreach (var child in EnumerateObjects(item))
            {
                yield return child;
            }
        }
    }

    private static string? GetJsonString(JsonElement token, params string[] propertyNames)
    {
        if (token.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        foreach (var property in token.EnumerateObject())
        {
            if (!propertyNames.Any(name => string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            return property.Value.ValueKind switch
            {
                JsonValueKind.String => property.Value.GetString(),
                JsonValueKind.Number => property.Value.ToString(),
                JsonValueKind.True => bool.TrueString,
                JsonValueKind.False => bool.FalseString,
                _ => null
            };
        }

        return null;
    }

    private static string? GetNestedJsonString(JsonElement token, params string[] propertyNames)
    {
        foreach (var objectToken in EnumerateObjects(token))
        {
            var value = GetJsonString(objectToken, propertyNames);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private static string? GetHeuristicPublicKey(JsonElement token)
    {
        if (token.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        foreach (var property in token.EnumerateObject())
        {
            if (property.Value.ValueKind != JsonValueKind.String)
            {
                continue;
            }

            var value = property.Value.GetString();
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            if ((property.Name.Contains("public", StringComparison.OrdinalIgnoreCase) &&
                 property.Name.Contains("key", StringComparison.OrdinalIgnoreCase)) ||
                property.Name.Contains("pem", StringComparison.OrdinalIgnoreCase) ||
                value.Contains("BEGIN PUBLIC KEY", StringComparison.OrdinalIgnoreCase) ||
                value.Contains("BEGIN EC PUBLIC KEY", StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }
        }

        return null;
    }

    private static string? GetHeuristicKeyId(JsonElement token)
    {
        if (token.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        foreach (var property in token.EnumerateObject())
        {
            if (property.Value.ValueKind != JsonValueKind.String && property.Value.ValueKind != JsonValueKind.Number)
            {
                continue;
            }

            if ((property.Name.Contains("key", StringComparison.OrdinalIgnoreCase) &&
                 property.Name.Contains("id", StringComparison.OrdinalIgnoreCase)) ||
                property.Name.Contains("kid", StringComparison.OrdinalIgnoreCase))
            {
                return property.Value.ToString();
            }
        }

        return null;
    }

    private sealed class LucidMarketplaceParsedSignatureHeader
    {
        public string TimestampText { get; set; } = string.Empty;

        public List<LucidMarketplaceSignatureEntry> Signatures { get; } = new();

        public bool HadMalformedSegments { get; set; }
    }

    private sealed class LucidMarketplaceSignatureEntry
    {
        public string? SchemaVersion { get; init; }

        public string? KeyId { get; init; }

        public string Signature { get; init; } = string.Empty;
    }

    private sealed class LucidMarketplaceWebhookKeyCandidate
    {
        public int? SubscriptionId { get; init; }

        public string? KeyId { get; init; }

        public string? KeyIdFull { get; init; }

        public string? PublicKey { get; init; }

        public string? JwkX { get; init; }

        public string? JwkY { get; init; }

        public string? SupplierCode { get; init; }

        public string? SubscriptionType { get; init; }
    }

    private sealed class LucidMarketplaceCandidateVerificationResult
    {
        public bool IsValid { get; set; }

        public bool KeyImportSucceeded { get; set; }

        public bool UsedBase64UrlForKeyImport { get; set; }

        public string? SignatureFormat { get; set; }

        public string? ErrorMessage { get; set; }
    }

    private sealed class LucidMarketplaceVerificationAttemptDiagnostic
    {
        public string? HeaderKeyId { get; init; }

        public int? CandidateSubscriptionId { get; init; }

        public string? CandidateKeyId { get; init; }

        public string? CandidateKeyIdFull { get; init; }

        public bool SignatureDecodeSucceeded { get; init; }

        public bool UsedBase64Url { get; init; }

        public bool KeyImportSucceeded { get; init; }

        public string? SignatureFormat { get; init; }

        public bool VerificationSucceeded { get; init; }

        public string? FailureMessage { get; init; }
    }
}

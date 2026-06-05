using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace NextOnServices.WebUI.VT.Services;

public sealed class LucidMarketplaceLinkSigner : ILucidMarketplaceLinkSigner
{
    private static readonly string[] _managedParameterNames = new[] { "pid", "mid", "clid", "maid", "cint_email", "hash" };
    private readonly ILogger<LucidMarketplaceLinkSigner> _logger;

    public LucidMarketplaceLinkSigner(ILogger<LucidMarketplaceLinkSigner> logger)
    {
        _logger = logger;
    }

    public LucidMarketplaceSignedLinkResult BuildSignedEntryLink(LucidMarketplaceLinkSignRequest request)
    {
        if (request == null)
        {
            return BuildFailure("Lucid Marketplace entry link request is required.");
        }

        var baseLink = request.BaseLink?.Trim();
        if (string.IsNullOrWhiteSpace(baseLink))
        {
            return BuildFailure("Lucid Marketplace entry link is missing.");
        }

        var secretKey = request.SecretKey?.Trim();
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            _logger.LogWarning(
                "Lucid Marketplace entry-link signing skipped because the secret key is missing. OpportunityId={OpportunityId}, InternalProjectId={InternalProjectId}, SurveyId={SurveyId}, AttemptType={AttemptType}",
                request.OpportunityId,
                request.InternalProjectId,
                request.LucidSurveyId,
                request.AttemptType);
            return BuildFailure("Lucid Marketplace entry-link signing secret is not configured.");
        }

        var respondentId = request.RespondentId?.Trim();
        if (string.IsNullOrWhiteSpace(respondentId))
        {
            return BuildFailure("Lucid Marketplace respondent id is required for link signing.");
        }

        try
        {
            var unsignedUrl = RemoveQueryParameter(baseLink, "hash");
            if (ContainsQueryParameter(unsignedUrl, "cint_email") && string.IsNullOrWhiteSpace(request.CintEmailRaw))
            {
                _logger.LogWarning(
                    "Lucid Marketplace entry-link signing failed because cint_email was present but no raw email was supplied. OpportunityId={OpportunityId}, InternalProjectId={InternalProjectId}, SurveyId={SurveyId}, AttemptType={AttemptType}",
                    request.OpportunityId,
                    request.InternalProjectId,
                    request.LucidSurveyId,
                    request.AttemptType);
                return BuildFailure("Lucid Marketplace cint_email is required but no raw email value was supplied.");
            }

            unsignedUrl = ApplyRespondentId(unsignedUrl, respondentId);
            unsignedUrl = UpsertOptionalParameter(unsignedUrl, "mid", request.SessionId);
            unsignedUrl = UpsertOptionalParameter(unsignedUrl, "clid", request.CountryLanguageId);
            unsignedUrl = UpsertOptionalParameter(unsignedUrl, "maid", request.MarketplaceAttemptId);

            var includedCintEmail = false;
            if (!string.IsNullOrWhiteSpace(request.CintEmailRaw))
            {
                var cintEmailHash = ComputeCintEmailHash(request.CintEmailRaw);
                if (string.IsNullOrWhiteSpace(cintEmailHash))
                {
                    return BuildFailure("Unable to prepare Lucid Marketplace cint_email.");
                }

                unsignedUrl = UpsertQueryParameter(unsignedUrl, "cint_email", cintEmailHash);
                includedCintEmail = true;
            }

            if (request.ExtraParameters != null)
            {
                foreach (var entry in request.ExtraParameters)
                {
                    if (string.IsNullOrWhiteSpace(entry.Key) ||
                        string.IsNullOrWhiteSpace(entry.Value) ||
                        string.Equals(entry.Key, "hash", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    unsignedUrl = UpsertQueryParameter(unsignedUrl, entry.Key, entry.Value);
                }
            }

            unsignedUrl = NormalizeManagedParameterNames(unsignedUrl);
            if (string.IsNullOrWhiteSpace(unsignedUrl))
            {
                return BuildFailure("Lucid Marketplace unsigned entry link could not be built.");
            }

            var signingUrl = EnsureHashAppendPosition(unsignedUrl);
            var sanitizedUnsignedUrl = SanitizeUrlForLog(signingUrl);
            _logger.LogInformation(
                "Lucid Marketplace unsigned entry link built. OpportunityId={OpportunityId}, InternalProjectId={InternalProjectId}, SurveyId={SurveyId}, AttemptType={AttemptType}, Url={UnsignedUrl}, HasCintEmail={HasCintEmail}, MaskedEmail={MaskedEmail}",
                request.OpportunityId,
                request.InternalProjectId,
                request.LucidSurveyId,
                request.AttemptType,
                sanitizedUnsignedUrl,
                includedCintEmail,
                MaskEmail(request.CintEmailRaw));

            var hashValue = ComputeUrlSafeHmacSha1(secretKey, signingUrl);
            if (string.IsNullOrWhiteSpace(hashValue))
            {
                _logger.LogWarning(
                    "Lucid Marketplace entry-link hash generation returned an empty signature. OpportunityId={OpportunityId}, InternalProjectId={InternalProjectId}, SurveyId={SurveyId}, AttemptType={AttemptType}",
                    request.OpportunityId,
                    request.InternalProjectId,
                    request.LucidSurveyId,
                    request.AttemptType);
                return BuildFailure("Unable to generate the Lucid Marketplace entry-link hash.");
            }

            var signedUrl = $"{signingUrl}hash={hashValue}";
            var sanitizedSignedUrl = SanitizeUrlForLog(signedUrl);

            _logger.LogInformation(
                "Lucid Marketplace signed entry link built. OpportunityId={OpportunityId}, InternalProjectId={InternalProjectId}, SurveyId={SurveyId}, AttemptType={AttemptType}, Url={SignedUrl}, HashLength={HashLength}, HasCintEmail={HasCintEmail}",
                request.OpportunityId,
                request.InternalProjectId,
                request.LucidSurveyId,
                request.AttemptType,
                sanitizedSignedUrl,
                hashValue.Length,
                includedCintEmail);

            return new LucidMarketplaceSignedLinkResult
            {
                Success = true,
                UnsignedUrl = signingUrl,
                SignedUrl = signedUrl,
                HashValue = hashValue,
                IncludedCintEmail = includedCintEmail
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Lucid Marketplace entry-link signing failed. OpportunityId={OpportunityId}, InternalProjectId={InternalProjectId}, SurveyId={SurveyId}, AttemptType={AttemptType}",
                request.OpportunityId,
                request.InternalProjectId,
                request.LucidSurveyId,
                request.AttemptType);
            return BuildFailure("Unable to build the Lucid Marketplace signed entry link.");
        }
    }

    public string ComputeUrlSafeHmacSha1(string secretKey, string unsignedUrl)
    {
        using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secretKey ?? string.Empty));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(unsignedUrl ?? string.Empty));
        return Convert.ToBase64String(hashBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public string ComputeCintEmailHash(string rawEmail)
    {
        if (string.IsNullOrWhiteSpace(rawEmail))
        {
            return string.Empty;
        }

        var emailBytes = Encoding.UTF8.GetBytes(rawEmail.Trim());
        var hexValue = Convert.ToHexString(emailBytes).ToLowerInvariant();
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(hexValue));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    private static LucidMarketplaceSignedLinkResult BuildFailure(string message)
    {
        return new LucidMarketplaceSignedLinkResult
        {
            Success = false,
            Message = message
        };
    }

    private static string ApplyRespondentId(string url, string respondentId)
    {
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(respondentId))
        {
            return string.Empty;
        }

        var encodedRespondentId = Uri.EscapeDataString(respondentId.Trim());
        if (ContainsRespondentPlaceholder(url))
        {
            return url
                .Replace("[respondentID]", encodedRespondentId, StringComparison.OrdinalIgnoreCase)
                .Replace("[RespondentID]", encodedRespondentId, StringComparison.OrdinalIgnoreCase)
                .Replace("[RESPONDENTID]", encodedRespondentId, StringComparison.OrdinalIgnoreCase);
        }

        return UpsertQueryParameter(url, "pid", respondentId.Trim());
    }

    private static string UpsertOptionalParameter(string url, string parameterName, string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? url
            : UpsertQueryParameter(url, parameterName, value.Trim());
    }

    private static string UpsertQueryParameter(string url, string parameterName, string value)
    {
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(parameterName))
        {
            return url;
        }

        var normalizedParameterName = parameterName.Trim().ToLowerInvariant();
        var encodedValue = Uri.EscapeDataString(value ?? string.Empty);
        var parameterRegex = new Regex($"(?i)([?&]){Regex.Escape(normalizedParameterName)}=([^&]*)");
        var updated = parameterRegex.Replace(
            url,
            match => $"{match.Groups[1].Value}{normalizedParameterName}={encodedValue}",
            1);

        return string.Equals(updated, url, StringComparison.Ordinal)
            ? AppendQueryParameter(url, normalizedParameterName, encodedValue)
            : updated;
    }

    private static string EnsureHashAppendPosition(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        if (url.EndsWith("&", StringComparison.Ordinal) || url.EndsWith("?", StringComparison.Ordinal))
        {
            return url;
        }

        return url.Contains('?', StringComparison.Ordinal)
            ? $"{url}&"
            : $"{url}?";
    }
    private static string AppendQueryParameter(string url, string parameterName, string value)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        var separator = url.Contains('?', StringComparison.Ordinal)
            ? (url.EndsWith("&", StringComparison.Ordinal) || url.EndsWith("?", StringComparison.Ordinal) ? string.Empty : "&")
            : "?";

        return $"{url}{separator}{parameterName.Trim().ToLowerInvariant()}={value}";
    }

    private static string RemoveQueryParameter(string url, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(parameterName))
        {
            return url ?? string.Empty;
        }

        var parameterRegex = new Regex($"(?i)([?&]){Regex.Escape(parameterName)}=[^&]*");
        var normalized = parameterRegex.Replace(url, "$1", 1);
        if (string.Equals(normalized, url, StringComparison.Ordinal))
        {
            return url;
        }

        normalized = normalized.Replace("?&", "?", StringComparison.Ordinal)
                               .Replace("&&", "&", StringComparison.Ordinal);

        if (normalized.EndsWith("?", StringComparison.Ordinal) && url.EndsWith("&", StringComparison.Ordinal))
        {
            return $"{normalized}&";
        }

        return normalized.TrimEnd('&', '?');
    }

    private static bool ContainsQueryParameter(string url, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(parameterName))
        {
            return false;
        }

        return Regex.IsMatch(url, $"(?i)([?&]){Regex.Escape(parameterName.Trim())}=");
    }

    private static string NormalizeManagedParameterNames(string url)
    {
        var normalizedUrl = url ?? string.Empty;
        foreach (var parameterName in _managedParameterNames)
        {
            normalizedUrl = Regex.Replace(
                normalizedUrl,
                $"(?i)([?&]){Regex.Escape(parameterName)}=",
                $"$1{parameterName}=");
        }

        return normalizedUrl;
    }

    private static bool ContainsRespondentPlaceholder(string url)
    {
        return !string.IsNullOrWhiteSpace(url) &&
               (url.Contains("[respondentID]", StringComparison.OrdinalIgnoreCase) ||
                url.Contains("[RespondentID]", StringComparison.OrdinalIgnoreCase) ||
                url.Contains("[RESPONDENTID]", StringComparison.OrdinalIgnoreCase));
    }

    private static string SanitizeUrlForLog(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        return Regex.Replace(
            url,
            @"(?i)([?&]cint_email=)[^&]*",
            "$1[masked]");
    }

    private static string? MaskEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var normalized = email.Trim();
        var atIndex = normalized.IndexOf('@');
        if (atIndex <= 1)
        {
            return "***";
        }

        return $"{normalized[0]}***{normalized[(atIndex - 1)..]}";
    }
}





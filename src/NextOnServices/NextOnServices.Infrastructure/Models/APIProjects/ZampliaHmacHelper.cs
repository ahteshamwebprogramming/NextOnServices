using System.Security.Cryptography;
using System.Text;

namespace NextOnServices.Infrastructure.Models.APIProjects;

public static class ZampliaHmacHelper
{
    public static string NormalizeRawUrlWithoutHash(string rawAbsoluteUrl)
    {
        if (string.IsNullOrWhiteSpace(rawAbsoluteUrl))
        {
            return string.Empty;
        }

        var fragmentIndex = rawAbsoluteUrl.IndexOf('#');
        var withoutFragment = fragmentIndex >= 0 ? rawAbsoluteUrl[..fragmentIndex] : rawAbsoluteUrl;
        var questionIndex = withoutFragment.IndexOf('?');
        if (questionIndex < 0)
        {
            return withoutFragment;
        }

        var baseUrl = withoutFragment[..questionIndex];
        var query = withoutFragment[(questionIndex + 1)..];
        if (string.IsNullOrWhiteSpace(query))
        {
            return baseUrl;
        }

        var remainingSegments = query
            .Split('&', StringSplitOptions.None)
            .Where(segment =>
            {
                if (string.IsNullOrWhiteSpace(segment))
                {
                    return false;
                }

                var equalsIndex = segment.IndexOf('=');
                var key = equalsIndex >= 0 ? segment[..equalsIndex] : segment;
                return !string.Equals(key, "hash", StringComparison.OrdinalIgnoreCase);
            })
            .ToList();

        return remainingSegments.Count == 0 ? baseUrl : $"{baseUrl}?{string.Join("&", remainingSegments)}";
    }

    public static string ComputeHmacSha256Hex(string secret, string message)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret ?? string.Empty));
        return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(message ?? string.Empty))).ToLowerInvariant();
    }

    public static ZampliaHmacValidationResult ValidateIncomingReturnUrl(string secret, string rawAbsoluteUrl, string receivedHash)
    {
        var normalizedUrl = NormalizeRawUrlWithoutHash(rawAbsoluteUrl);
        var calculatedHash = ComputeHmacSha256Hex(secret ?? string.Empty, normalizedUrl);
        var expectedBytes = Encoding.UTF8.GetBytes(calculatedHash);
        var actualBytes = Encoding.UTF8.GetBytes((receivedHash ?? string.Empty).Trim().ToLowerInvariant());
        var isValid = expectedBytes.Length == actualBytes.Length &&
                      CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes);

        return new ZampliaHmacValidationResult
        {
            NormalizedUrl = normalizedUrl,
            CalculatedHash = calculatedHash,
            ReceivedHash = receivedHash,
            IsValid = isValid
        };
    }
}

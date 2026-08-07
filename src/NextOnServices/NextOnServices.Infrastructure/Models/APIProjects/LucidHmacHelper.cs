using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace NextOnServices.Infrastructure.Models.APIProjects;

/// <summary>
/// Lucid Marketplace entry-link HMAC (HMAC-SHA1, URL-safe Base64).
/// Matches the Lucid supplier-link signing contract used by LucidMarketplaceLinkSigner.
/// </summary>
public static class LucidHmacHelper
{
    public const string HashParameterName = "hash";

    /// <summary>
    /// Removes an existing hash parameter and ensures the URL ends with ? or &amp; so hash= can be appended.
    /// The trailing separator is part of the signed payload (Lucid contract).
    /// </summary>
    public static string NormalizeUrlForSigning(string? rawAbsoluteUrl, string parameterName = HashParameterName)
    {
        if (string.IsNullOrWhiteSpace(rawAbsoluteUrl))
        {
            return string.Empty;
        }

        var withoutHash = RemoveQueryParameter(rawAbsoluteUrl.Trim(), parameterName);
        return EnsureHashAppendPosition(withoutHash);
    }

    public static string ComputeUrlSafeHmacSha1(string? secretKey, string? unsignedUrl)
    {
        using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secretKey ?? string.Empty));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(unsignedUrl ?? string.Empty));
        return Convert.ToBase64String(hashBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public static string BuildSignedUrl(string? rawAbsoluteUrl, string? secretKey, string parameterName = HashParameterName)
    {
        var signingUrl = NormalizeUrlForSigning(rawAbsoluteUrl, parameterName);
        if (string.IsNullOrWhiteSpace(signingUrl))
        {
            return string.Empty;
        }

        var hashValue = ComputeUrlSafeHmacSha1(secretKey, signingUrl);
        if (string.IsNullOrWhiteSpace(hashValue))
        {
            return string.Empty;
        }

        var normalizedParameterName = string.IsNullOrWhiteSpace(parameterName)
            ? HashParameterName
            : parameterName.Trim().ToLowerInvariant();

        return $"{signingUrl}{normalizedParameterName}={hashValue}";
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
}

using AutoMapper;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.APIProjects;
using NextOnServices.Infrastructure.Models.Settings;

namespace NextOnServices.WebUI.VT.Services;

public sealed class HashingSettingsService : IHashingSettingsService
{
    private const string HashingSettingTableName = "HashingSetting";
    private const string HashingSettingExistsSql = """
        SELECT COUNT(1)
        FROM sys.objects
        WHERE object_id = OBJECT_ID(N'[dbo].[HashingSetting]')
          AND type = N'U';
        """;
    private const string EnsureHashingSettingTableSql = """
        IF OBJECT_ID(N'[dbo].[HashingSetting]', N'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[HashingSetting](
                [HashingSettingId] [int] IDENTITY(1,1) NOT NULL,
                [HashingType] [nvarchar](50) NOT NULL,
                [HashingKey] [nvarchar](500) NOT NULL,
                [IsActive] [bit] NOT NULL CONSTRAINT [DF_HashingSetting_IsActive] DEFAULT((1)),
                [CreatedDate] [datetime] NULL CONSTRAINT [DF_HashingSetting_CreatedDate] DEFAULT(getdate()),
                [ModifiedDate] [datetime] NULL,
                [CreatedBy] [int] NULL,
                [ModifiedBy] [int] NULL,
                CONSTRAINT [PK_HashingSetting] PRIMARY KEY CLUSTERED ([HashingSettingId] ASC)
            );

            CREATE UNIQUE NONCLUSTERED INDEX [UX_HashingSetting_HashingType]
                ON [dbo].[HashingSetting] ([HashingType] ASC);
        END;
        """;
    private static readonly string[] _supportedHashingTypes = new[] { "SHA1", "SHA3", "HMACSHA256" };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<HashingSettingsService> _logger;

    public HashingSettingsService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<HashingSettingsService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public IReadOnlyList<string> SupportedHashingTypes => _supportedHashingTypes;

    public async Task<List<HashingSettingDTO>> GetAllAsync()
    {
        await EnsureTableAsync();

        const string query = """
            SELECT *
            FROM HashingSetting
            ORDER BY IsActive DESC, HashingType ASC, HashingSettingId DESC;
            """;

        var items = await _unitOfWork.HashingSetting.GetTableData<HashingSettingDTO>(query);
        return items ?? new List<HashingSettingDTO>();
    }

    public async Task<HashingSettingDTO?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        await EnsureTableAsync();

        var entity = await _unitOfWork.HashingSetting.FindByIdAsync(id);
        return entity == null ? null : _mapper.Map<HashingSettingDTO>(entity);
    }

    public async Task<HashingSettingDTO> SaveAsync(HashingSettingDTO inputData, int? userId)
    {
        if (inputData == null)
        {
            throw new InvalidOperationException("Hashing settings payload is required.");
        }

        await EnsureTableAsync();

        var normalizedType = NormalizeHashingType(inputData.HashingType);
        if (!_supportedHashingTypes.Contains(normalizedType, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Please select a supported hashing type.");
        }

        var normalizedKey = inputData.HashingKey?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedKey))
        {
            throw new InvalidOperationException("Hashing key is required.");
        }

        var entity = inputData.HashingSettingId > 0
            ? await _unitOfWork.HashingSetting.FindByIdAsync(inputData.HashingSettingId)
            : null;

        entity ??= await _unitOfWork.HashingSetting.GetEntityData<HashingSetting>(
            """
            SELECT TOP 1 *
            FROM HashingSetting
            WHERE UPPER(LTRIM(RTRIM(HashingType))) = @HashingType
            ORDER BY HashingSettingId DESC;
            """,
            new { HashingType = normalizedType });

        if (entity == null)
        {
            entity = new HashingSetting
            {
                HashingType = normalizedType,
                HashingKey = normalizedKey,
                IsActive = true,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                CreatedBy = userId,
                ModifiedBy = userId
            };

            entity.HashingSettingId = await _unitOfWork.HashingSetting.AddAsync(entity);
            if (entity.HashingSettingId <= 0)
            {
                throw new InvalidOperationException("Unable to save hashing settings.");
            }
        }
        else
        {
            entity.HashingType = normalizedType;
            entity.HashingKey = normalizedKey;
            entity.IsActive = true;
            entity.CreatedDate ??= DateTime.Now;
            entity.CreatedBy ??= userId;
            entity.ModifiedDate = DateTime.Now;
            entity.ModifiedBy = userId;

            var updated = await _unitOfWork.HashingSetting.UpdateAsync(entity);
            if (!updated)
            {
                throw new InvalidOperationException("Unable to update hashing settings.");
            }
        }

        return _mapper.Map<HashingSettingDTO>(entity);
    }

    public async Task<string?> GetHashingKeyAsync(string? hashingType)
    {
        var normalizedType = NormalizeHashingType(hashingType);
        if (string.IsNullOrWhiteSpace(normalizedType))
        {
            return null;
        }

        try
        {
            await EnsureTableAsync();

            var key = await _unitOfWork.HashingSetting.GetEntityData<string>(
                """
                SELECT TOP 1 HashingKey
                FROM HashingSetting
                WHERE IsActive = 1
                  AND UPPER(LTRIM(RTRIM(HashingType))) = @HashingType
                ORDER BY HashingSettingId DESC;
                """,
                new { HashingType = normalizedType });

            return string.IsNullOrWhiteSpace(key) ? null : key.Trim();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to load hashing key for type {HashingType}", normalizedType);
            return null;
        }
    }

    public async Task<HashingApplicationResult> ApplyHashAsync(string requestUrl, int? addHashing, string? hashingType, string? parameterName)
    {
        var result = new HashingApplicationResult
        {
            RequestUrl = requestUrl ?? string.Empty,
            HashCode = string.Empty,
            HashApplied = false
        };

        if (addHashing != 1 || string.IsNullOrWhiteSpace(result.RequestUrl))
        {
            return result;
        }

        var normalizedType = NormalizeHashingType(hashingType);
        if (string.IsNullOrWhiteSpace(normalizedType))
        {
            return result;
        }

        var hashingKey = await GetHashingKeyAsync(normalizedType);
        if (string.IsNullOrWhiteSpace(hashingKey))
        {
            _logger.LogWarning(
                "Hashing is enabled but no hashing key is configured for type {HashingType}.",
                normalizedType);
            return result;
        }

        var normalizedParameterName = string.IsNullOrWhiteSpace(parameterName)
            ? (string.Equals(normalizedType, "HMACSHA256", StringComparison.Ordinal) ? "hash" : "enc")
            : parameterName.Trim();

        if (string.Equals(normalizedType, "HMACSHA256", StringComparison.Ordinal))
        {
            return ApplyZampliaHmacHash(result, hashingKey, normalizedParameterName);
        }

        var hashCode = normalizedType switch
        {
            "SHA3" => Encryption.HashSHA3(result.RequestUrl, hashingKey),
            "SHA1" => Encryption.HashSHA1_C(result.RequestUrl, hashingKey),
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(hashCode))
        {
            return result;
        }

        var separator = result.RequestUrl.Contains('?', StringComparison.Ordinal)
            ? (result.RequestUrl.EndsWith("&", StringComparison.Ordinal) || result.RequestUrl.EndsWith("?", StringComparison.Ordinal) ? string.Empty : "&")
            : "?";

        result.HashCode = hashCode;
        result.RequestUrl = $"{result.RequestUrl}{separator}{normalizedParameterName}={hashCode}";
        result.HashApplied = true;

        return result;
    }

    private async Task EnsureTableAsync()
    {
        var exists = await SafeGetTableExistsAsync();
        if (exists)
        {
            return;
        }

        var created = await _unitOfWork.HashingSetting.ExecuteQueryAsync(EnsureHashingSettingTableSql);
        if (!created)
        {
            _logger.LogWarning("Automatic ensure-table execution returned false for {TableName}.", HashingSettingTableName);
        }

        exists = await SafeGetTableExistsAsync();
        if (!exists)
        {
            throw new InvalidOperationException("Hashing settings storage is not available.");
        }
    }

    private async Task<bool> SafeGetTableExistsAsync()
    {
        try
        {
            return await _unitOfWork.HashingSetting.GetEntityCount(HashingSettingExistsSql) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to verify whether {TableName} exists.", HashingSettingTableName);
            return false;
        }
    }

    private static string NormalizeHashingType(string? hashingType)
        => hashingType?.Trim().ToUpperInvariant() ?? string.Empty;

    private static HashingApplicationResult ApplyZampliaHmacHash(
        HashingApplicationResult result,
        string hashingKey,
        string parameterName)
    {
        var normalizedParameterName = string.IsNullOrWhiteSpace(parameterName) ? "hash" : parameterName.Trim();
        var unsignedUrl = NormalizeUrlWithoutHashParameter(result.RequestUrl, normalizedParameterName);
        if (string.IsNullOrWhiteSpace(unsignedUrl))
        {
            return result;
        }

        var hashCode = ZampliaHmacHelper.ComputeHmacSha256Hex(hashingKey, unsignedUrl);
        if (string.IsNullOrWhiteSpace(hashCode))
        {
            return result;
        }

        var separator = unsignedUrl.Contains('?', StringComparison.Ordinal)
            ? (unsignedUrl.EndsWith("&", StringComparison.Ordinal) || unsignedUrl.EndsWith("?", StringComparison.Ordinal) ? string.Empty : "&")
            : "?";

        result.HashCode = hashCode;
        result.RequestUrl = $"{unsignedUrl}{separator}{normalizedParameterName}={Uri.EscapeDataString(hashCode)}";
        result.HashApplied = true;
        return result;
    }

    private static string NormalizeUrlWithoutHashParameter(string requestUrl, string parameterName)
    {
        var normalizedUrl = ZampliaHmacHelper.NormalizeRawUrlWithoutHash(requestUrl);
        if (string.IsNullOrWhiteSpace(normalizedUrl) || string.IsNullOrWhiteSpace(parameterName) || string.Equals(parameterName, "hash", StringComparison.OrdinalIgnoreCase))
        {
            return normalizedUrl;
        }

        var fragmentIndex = normalizedUrl.IndexOf('#');
        var withoutFragment = fragmentIndex >= 0 ? normalizedUrl[..fragmentIndex] : normalizedUrl;
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
                return !string.Equals(key, parameterName, StringComparison.OrdinalIgnoreCase);
            })
            .ToList();

        return remainingSegments.Count == 0 ? baseUrl : $"{baseUrl}?{string.Join("&", remainingSegments)}";
    }
}

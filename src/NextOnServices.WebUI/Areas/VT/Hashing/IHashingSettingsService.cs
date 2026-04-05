using NextOnServices.Infrastructure.Models.Settings;

namespace NextOnServices.WebUI.VT.Services;

public interface IHashingSettingsService
{
    IReadOnlyList<string> SupportedHashingTypes { get; }

    Task<List<HashingSettingDTO>> GetAllAsync();

    Task<HashingSettingDTO?> GetByIdAsync(int id);

    Task<HashingSettingDTO> SaveAsync(HashingSettingDTO inputData, int? userId);

    Task<string?> GetHashingKeyAsync(string? hashingType);

    Task<HashingApplicationResult> ApplyHashAsync(string requestUrl, int? addHashing, string? hashingType, string? parameterName);
}

public sealed class HashingApplicationResult
{
    public string RequestUrl { get; set; } = string.Empty;

    public string HashCode { get; set; } = string.Empty;

    public bool HashApplied { get; set; }
}

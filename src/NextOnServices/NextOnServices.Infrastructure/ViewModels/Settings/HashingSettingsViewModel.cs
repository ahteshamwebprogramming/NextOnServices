using NextOnServices.Infrastructure.Models.Settings;

namespace NextOnServices.Infrastructure.ViewModels.Settings;

public class HashingSettingsViewModel
{
    public HashingSettingDTO Form { get; set; } = new();

    public List<HashingSettingDTO> Items { get; set; } = new();

    public IReadOnlyList<string> SupportedHashingTypes { get; set; } = Array.Empty<string>();
}

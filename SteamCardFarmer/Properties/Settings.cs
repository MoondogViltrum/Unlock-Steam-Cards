using System.Configuration;
using System.ComponentModel;

namespace SteamCardFarmer.Properties
{
    [SettingsGroupName("SteamCardFarmerSettings")]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings? _default;

        public static Settings Default => _default ??= (Settings)Synchronized(new Settings());

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string SteamLoginSecure
        {
            get => (string)this[nameof(SteamLoginSecure)];
            set => this[nameof(SteamLoginSecure)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string SessionId
        {
            get => (string)this[nameof(SessionId)];
            set => this[nameof(SessionId)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string ProfileUrl
        {
            get => (string)this[nameof(ProfileUrl)];
            set => this[nameof(ProfileUrl)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("Français")]
        public string Language
        {
            get => (string)this[nameof(Language)];
            set => this[nameof(Language)] = value;
        }
    }
}

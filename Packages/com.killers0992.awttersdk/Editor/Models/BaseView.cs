using System;

namespace AwtterSDK.Editor.Models
{
    public class BaseView
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string BaseName { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string InstalledVersion { get; set; }
        public string DownloadUrl { get; set; }
        public bool IsPatreon { get; set; }

        private Version _newVersion;
        private Version _currentVersion;
        public bool IsOutdated
        {
            get
            {
                if (_newVersion == null && !System.Version.TryParse(Version, out _newVersion))
                    return false;

                if (_currentVersion == null && !System.Version.TryParse(InstalledVersion, out _currentVersion))
                    return false;

                return _newVersion.CompareTo(_currentVersion) > 0;
            }
        }
        public bool IsInstalled => AwtterSdkInstaller.InstalledPackages?.BaseModel?.Id == Id;
    }
}

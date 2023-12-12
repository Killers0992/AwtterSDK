using AwtterSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Awtter_SDK.Editor.Models
{
    public class ToolView
    {
        public static List<string> InstallByDefault = new List<string>()
        {
            "awttermerger",
            "poiyomi",
        };

        private bool? _install;
        private string _simpleName;

        public int Id { get; set; }
        public string Icon { get; set; }

        public string SimpleName
        {
            get
            {
                if (_simpleName == null)
                    _simpleName = Name.ToLower().Replace(" ", "");

                return _simpleName;
            }
        }

        public string Name { get; set; }
        public string Version { get; set; }
        public string InstalledVersion { get; set; }
        public string DownloadUrl { get; set; }
        public bool IsInstalled => AwtterSdkInstaller.InstalledPackages != null && AwtterSdkInstaller.InstalledPackages.Tools.ContainsKey(SimpleName);

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

                return _currentVersion.CompareTo(_newVersion) > 0;
            }
        }
        public bool Install
        {
            get
            {
                if (!_install.HasValue)
                {
                    if (InstallByDefault.Contains(SimpleName) && !IsInstalled)
                        _install = true;
                    else
                        _install = false;
                }
                   
                return _install.Value;
            }
            set
            {
                _install = value;
            }
        }
    }
}

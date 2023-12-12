using AwtterSDK.Editor.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwtterSDK.Editor.Installations
{
    public class MergerInstallation : ICheckInstallStatus
    {
        Version _version;
        bool _isInstalled;
        public bool IsInstalled => _isInstalled;
        public Version InstalledVersion => _version;

        public void Check()
        {
            _isInstalled = Directory.Exists("Assets/AwboiMerger");

            if (_isInstalled)
            {
                var targetFile = "Assets/AwboiMerger/AWBOI_MERGER.dll";

                if (File.Exists(targetFile))
                {
                    var ver = FileVersionInfo.GetVersionInfo(targetFile);
                    Version.TryParse(ver.FileVersion, out _version);
                }
            }
        }
    }
}

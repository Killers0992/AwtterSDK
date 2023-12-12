using AwtterSDK.Editor.Interfaces;
using AwtterSDK.Editor.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace AwtterSDK.Editor.Installations
{
    public class PumkinInstallation : ICheckInstallStatus
    {
        bool _isInstalled;
        Version _version;

        public bool IsInstalled => _isInstalled;

        public Version InstalledVersion => _version;

        public void Check()
        {
            _isInstalled = Directory.Exists("Assets/PumkinsAvatarTools");

            if (_isInstalled)
            {
                var targetFile = "Assets/PumkinsAvatarTools/thry_module_manifest.json";

                if (File.Exists(targetFile))
                {
                    var manifest = JsonConvert.DeserializeObject<PackageManifest>(File.ReadAllText(targetFile));

                    Version.TryParse(manifest.Version, out _version);
                }
            }
        }
    }
}

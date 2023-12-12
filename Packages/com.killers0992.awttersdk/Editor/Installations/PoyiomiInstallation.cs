using AwtterSDK.Editor.Interfaces;
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
    public class PoyomiInstallation : ICheckInstallStatus
    {
        bool _isInstalled;
        Version _version;

        public bool IsInstalled => _isInstalled;

        public Version InstalledVersion => _version;

        public void Check()
        {
            _isInstalled = Directory.Exists("Assets/_PoiyomiShaders");

            if (_isInstalled)
            {
                if (Directory.Exists("Assets/_PoiyomiShaders/Shaders"))
                {
                    foreach(var dir in Directory.GetDirectories("Assets/_PoiyomiShaders/Shaders"))
                    {
                        foreach(var file in Directory.GetFiles(dir, "*.shader"))
                        {
                            var content = File.ReadAllLines(file);
                            var versionLine = content.FirstOrDefault(p => p.Contains("shader_master_label"));
                            if (versionLine != null)
                            {
                                var line = versionLine.Split('(', ')')[1];
                                var line2 = line.Split('"')[1];
                                var line3 = line2.Split(' ')[1];

                                var finalVersion = Regex.Replace(line3, @"<[^>]*>", String.Empty);

                                Version.TryParse(finalVersion, out _version);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}

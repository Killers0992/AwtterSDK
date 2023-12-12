using AwtterSDK.Editor.Installations;
using AwtterSDK.Editor.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AwtterSDK.Editor.Models
{
    public class InstalledPackagesModel
    {
        public static Dictionary<string, ICheckInstallStatus> CheckInstallStatuses = new Dictionary<string, ICheckInstallStatus>()
        {
            { "poiyomi", new PoyomiInstallation() },
            { "awttermerger", new MergerInstallation() },
            { "pumkintool", new PumkinInstallation() },
        };

        public InstalledPackageModel BaseModel { get; set; }
        public Dictionary<int, InstalledPackageModel> Dlcs { get; set; } = new Dictionary<int, InstalledPackageModel>();

        [JsonIgnore]
        public Dictionary<string, InstalledPackageModel> Tools = new Dictionary<string, InstalledPackageModel>();

        public void CheckTools()
        {
            Tools.Clear();
            foreach (var tool in CheckInstallStatuses)
            {
                tool.Value.Check();
                if (tool.Value.IsInstalled)
                    Tools.Add(tool.Key, new InstalledPackageModel()
                    {
                        Version = tool.Value.InstalledVersion == null ? string.Empty : tool.Value.InstalledVersion.ToString(),
                    });
            }
        }
    }
}

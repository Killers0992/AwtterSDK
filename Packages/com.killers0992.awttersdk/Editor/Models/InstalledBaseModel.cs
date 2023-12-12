using AwtterSDK.Editor.Models.API;
using Newtonsoft.Json;

namespace AwtterSDK.Editor.Models
{
    public class InstalledPackageModel
    {
        public int Id { get; set; }
        public string Version { get; set; }
        [JsonIgnore]
        public FileModel File { get; set; }
    }
}

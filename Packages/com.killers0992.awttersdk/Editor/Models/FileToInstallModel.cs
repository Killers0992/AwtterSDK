using AwtterSDK.Editor.Enums;
using UnityEngine;

namespace AwtterSDK.Editor.Models
{
    public class FileToInstallModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Displayname { get; set; }
        public string Icon { get; set; }
        public bool IsBaseModel { get; set; }
        public bool IsDLC { get; set; }
        public bool IsUnityPackage { get; set; } = true;
        public bool RequiresAuth { get; set; }
        public string DownloadUrl { get; set; }
        public DownloadStatus DownloadStatus { get; set; } = DownloadStatus.Waiting;
        public float DownloadProgress { get; set; }

        public Color DownloadStatusColor
        {
            get
            {
                switch (DownloadStatus)
                {
                    case DownloadStatus.Waiting:
                        return Color.black;
                    case DownloadStatus.Installing:
                        return Color.cyan;
                    case DownloadStatus.Importing:
                        return Color.yellow;
                    case DownloadStatus.Installed:
                        return Color.green;
                    default:
                        return Color.red;
                }
            }
        }
    }
}

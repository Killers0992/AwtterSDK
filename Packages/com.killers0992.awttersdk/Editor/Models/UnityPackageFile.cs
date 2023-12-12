namespace AwtterSDK.Editor.Models
{
    using AwtterSDK.Editor.Interfaces;

    public class UnityPackageFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public bool RequiresAuth { get; set; }
        public ICheckInstallStatus InstallStatus { get; set; }
    }
}

namespace AwtterSDK.Editor.Models.API
{
    public class ConfigResponseModel
    {
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string ApiVersion { get; set; }
        public TosResponseModel Tos { get; set; }
        public AwtterSdkResponseModel AwtterSdk { get; set; }
        public MergerResponseModel AwtterMerger { get; set; }
    }
}

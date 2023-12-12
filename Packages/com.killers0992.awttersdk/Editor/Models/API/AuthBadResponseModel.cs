namespace AwtterSDK.Editor.Models.API
{
    using AwtterSDK.Editor.Enums;

    public class AuthBadResponseModel
    {
        public StatusType Status { get; set; }
        public string Message { get; set; }
    }
}

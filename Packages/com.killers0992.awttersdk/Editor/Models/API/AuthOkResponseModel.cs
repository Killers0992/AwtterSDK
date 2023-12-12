using AwtterSDK.Editor.Enums;

namespace AwtterSDK.Editor.Models.API
{
    public class AuthOkResponseModel
    {
        public StatusType Status { get; set; }
        public AuthResponseModel Data { get; set; }
    }
}

using AwtterSDK.Editor.Enums;

namespace AwtterSDK.Editor.Models.API
{
    public class CurrentUserResponseModel
    {
        public StatusType Status { get; set; }
        public UserModel Data { get; set; }
    }
}

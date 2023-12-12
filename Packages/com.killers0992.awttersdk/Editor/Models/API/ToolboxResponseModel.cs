using AwtterSDK.Editor.Enums;

namespace AwtterSDK.Editor.Models.API
{
    public class ToolboxResponseModel
    {
        public StatusType Status { get; set; }
        public ToolboxOkResponseModel Data { get; set; }
    }
}

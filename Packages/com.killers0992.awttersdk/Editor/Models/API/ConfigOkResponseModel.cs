using AwtterSDK.Editor.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwtterSDK.Editor.Models.API
{
    public class ConfigOkResponseModel
    {
        public StatusType Status { get; set; }
        public ConfigResponseModel Data { get; set; }
    }
}

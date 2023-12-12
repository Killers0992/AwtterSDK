using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwtterSDK.Editor.Models.API
{
    public class ToolboxOkResponseModel
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public List<FileModel> Files { get; set; }
    }
}

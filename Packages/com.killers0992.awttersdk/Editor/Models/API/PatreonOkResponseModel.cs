using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwtterSDK.Editor.Models.API
{
    public class PatreonOkResponseModel
    {
        public bool Active { get; set; }
        public string Tier { get; set; }
        public List<ProductModel> Benefits { get; set; }
    }
}

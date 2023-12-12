using AwtterSDK.Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwtterSDK.Editor.Interfaces
{
    public interface ICheckInstallStatus
    {
        bool IsInstalled { get; }
        Version InstalledVersion { get; }
        void Check();
    }
}

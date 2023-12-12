using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AwtterSDK.Editor.Interfaces
{
    public interface IPage
    {
        void Load(AwtterSdkInstaller main);
        void DrawGUI(Rect pos);
        void Reset();
    }
}

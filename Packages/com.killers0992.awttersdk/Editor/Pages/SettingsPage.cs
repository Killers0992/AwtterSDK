using AwtterSDK;
using AwtterSDK.Editor.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace AwtterSDK.Editor.Pages
{
    public class SettingsPage : IPage
    {
        private long _directorySize = 0;

        public void Load(AwtterSdkInstaller main)
        {
            _directorySize = Utils.DirectorySize(new System.IO.DirectoryInfo(FilesCache.CachePath));
        }

        public void DrawGUI(Rect pos)
        {
            Utils.CreateBox("Cache");
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Used space {Utils.FormatSize(_directorySize)}");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear cache"))
            {
                foreach(var file in Directory.GetFiles(FilesCache.CachePath))
                {
                    File.Delete(file);
                }
                Load(null);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button($"Go back"))
            {
                AwtterSdkInstaller.ViewSettings = false;
            }
            EditorGUILayout.EndVertical();
        }


        public void Reset()
        {
            _directorySize = 0;
        }
    }
}

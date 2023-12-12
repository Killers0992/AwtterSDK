using AwtterSDK;
using AwtterSDK.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Awtter_SDK.Editor
{
    public class ChangeLogsMenu : EditorWindow
    {
        private static ChangeLogsMenu _window;

        public static void ShowChangelogs()
        {
            _window = (ChangeLogsMenu)EditorWindow.GetWindow(typeof(ChangeLogsMenu), false, "Awtter SDK | Changelogs");
            _window.minSize = new Vector2(600f, 600f);

            var position = _window.position;
            position.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height).center;
            _window.position = position;

            _window.Show();
        }

        public string ChangeLogsPath => Path.Combine(Paths.MainPath, "Editor", "Textures", "changelogs.txt");

        public string[] Changelogs = null;

        public Vector2 scroll = Vector2.zero;

        private void OnGUI()
        {
            if (Changelogs == null)
            {
                Changelogs = File.ReadAllLines(ChangeLogsPath);
            }

            scroll = GUILayout.BeginScrollView(scroll, false, true);
            foreach(var line in Changelogs)
            {
                GUILayout.Label(line);
            }
            GUILayout.EndScrollView();

            if (GUILayout.Button($"CLOSE", GUILayout.MinWidth(50), GUILayout.MinHeight(32)))
            {
                Close();
            }
        }
    }
}

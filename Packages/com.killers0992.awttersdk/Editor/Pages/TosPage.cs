namespace AwtterSDK.Editor.Pages
{
    using AwtterSDK.Editor.Interfaces;
    using System;
    using UnityEditor;
    using UnityEngine;

    public class TosPage : IPage
    {
        private static bool? _tosAccepted;
        private static int? _tosVersion;

        public static bool TosAccepted
        {
            get 
            {
                if (!_tosAccepted.HasValue)
                    _tosAccepted = EditorPrefs.GetBool("AwSdkTos");

                return _tosAccepted.Value;
            }
            set 
            {
                EditorPrefs.SetBool("AwSdkTos", value);
                _tosAccepted = value;
            }
        }

        public static int TosVersion
        {
            get
            {
                if (!_tosVersion.HasValue)
                    _tosVersion = EditorPrefs.GetInt("AwSdkTosVersion");

                return _tosVersion.Value;
            }
            set
            {
                EditorPrefs.SetInt("AwSdkTosVersion", value);
                _tosVersion = value;
            }
        }

        static string[] _tosLines = null;

        public Vector2 ScrollPosition = Vector2.zero;
        public GUIStyle CustomLabel;

        private AwtterSdkInstaller _main;

        public void Load(AwtterSdkInstaller main)
        {
            _main = main;
            CustomLabel = new GUIStyle(GUI.skin.label);
            CustomLabel.richText = true;
        }

        public void DrawGUI(Rect pos)
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical("Terms of service", "window", GUILayout.Height(250));

            if (AwtterSdkInstaller.RemoteConfig != null)
            {
                if (TosAccepted && AwtterSdkInstaller.RemoteConfig.Tos.Version > TosVersion)
                    TosAccepted = false;
            }

            if (_tosLines == null)
            {
                if (AwtterSdkInstaller.RemoteConfig != null)
                    _tosLines = AwtterSdkInstaller.RemoteConfig.Tos.Text.Split(
                        new string[] { Environment.NewLine },
                        StringSplitOptions.None
                    );

                GUILayout.FlexibleSpace();
                GUILayout.Label("Loading TOS...");
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
                return;
            }

            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, false, true);

            foreach(var line in _tosLines)
            {
                if (line.StartsWith("https://"))
                {
                    if (GUILayout.Button(line))
                        Application.OpenURL(line);
                }
                else
                    GUILayout.Label(line, CustomLabel);
            }

            GUILayout.EndScrollView();
            
            if (GUILayout.Button("Accept TOS"))
            {
                TosVersion = AwtterSdkInstaller.RemoteConfig.Tos.Version;
                TosAccepted = true;
            }

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }

        public void Reset()
        {
            _tosLines = null;
            ScrollPosition = Vector2.zero;
        }
    }
}

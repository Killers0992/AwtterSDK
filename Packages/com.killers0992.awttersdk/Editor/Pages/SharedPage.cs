using Assets.Awtter_SDK.Editor;
using AWBOI.SplashScreen;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AwtterSDK.Editor.Pages
{
    public class SharedPage
    {
        private Texture2D _windowBackground;
        private Texture2D _awtterSdkImage;

        internal AwtterSdkInstaller _main;

        public Texture2D WindowBackground
        {
            get
            {
                if (_windowBackground == null)
                    _windowBackground = Utils.CreateTexture((int)_main.maxSize.x, (int)_main.maxSize.y, new Color32(35, 35, 35, 255));

                return _windowBackground;
            }
        }

        public Texture2D AwtterSdkImage
        {
            get
            {
                if (_awtterSdkImage == null)
                    _awtterSdkImage = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(Paths.MainPath, "Editor", "Textures", "sdk.png"));

                return _awtterSdkImage;
            }
        }

        public GUIStyle WindowCustomButton;
        public GUIStyle WindowCustomButton2;
        public GUIStyle WindowCustomButton3;
        public GUIStyle WindowCustomButton4;

        public SharedPage(AwtterSdkInstaller main)
        {
            _main = main;
            Load();
        }

        public void Load()
        {

            if (WindowCustomButton == null)
            {
                WindowCustomButton = new GUIStyle(GUI.skin.button);
                WindowCustomButton.alignment = TextAnchor.MiddleCenter;
                WindowCustomButton.fontStyle = FontStyle.Bold;

                WindowCustomButton.border = new RectOffset(0, 0, 0, 0);
                WindowCustomButton.normal.background = Utils.CreateTexture(150, 150, new Color32(80, 80, 80, 255));
                WindowCustomButton.hover.background = Utils.CreateTexture(150, 150, new Color32(73, 69, 69, 255));
            }

            if (WindowCustomButton2 == null)
            {
                WindowCustomButton2 = new GUIStyle(GUI.skin.button);
                WindowCustomButton2.alignment = TextAnchor.MiddleLeft;
                WindowCustomButton2.fontStyle = FontStyle.Bold;

                WindowCustomButton2.border = new RectOffset(0, 0, 0, 0);
                WindowCustomButton2.normal.background = Utils.CreateTexture(150, 150, new Color32(80, 80, 80, 255));
                WindowCustomButton2.hover.background = Utils.CreateTexture(150, 150, new Color32(73, 69, 69, 255));
            }

            if (WindowCustomButton3 == null)
            {
                WindowCustomButton3 = new GUIStyle(GUI.skin.button);
                WindowCustomButton3.alignment = TextAnchor.MiddleCenter;
                WindowCustomButton3.fontStyle = FontStyle.Bold;
                WindowCustomButton3.fontSize = 20;

                WindowCustomButton3.border = new RectOffset(0, 0, 0, 0);
                WindowCustomButton3.normal.background = Utils.CreateTexture(150, 150, new Color32(80, 80, 80, 255));
                WindowCustomButton3.hover.background = Utils.CreateTexture(150, 150, new Color32(73, 69, 69, 255));
            }

            if (WindowCustomButton4 == null)
            {
                WindowCustomButton4 = new GUIStyle(GUI.skin.button);
                WindowCustomButton4.alignment = TextAnchor.MiddleCenter;
                WindowCustomButton4.fontStyle = FontStyle.Bold;
                WindowCustomButton4.fontSize = 20;

                WindowCustomButton4.border = new RectOffset(0, 0, 0, 0);
                WindowCustomButton4.normal.background = Utils.CreateTexture(150, 150, new Color32(82, 78, 183, 255));
                WindowCustomButton4.hover.background = Utils.CreateTexture(150, 150, new Color32(82, 78, 183, 255));
            }
        }

        public void Top(Rect pos)
        {
            GUI.DrawTexture(new Rect(0, 0, pos.size.x, pos.size.y), WindowBackground, ScaleMode.StretchToFill);
            LinkButtons();
            if (AwtterSdkImage != null)
                GUI.DrawTexture(new Rect(0, 78, pos.size.x, 180), AwtterSdkImage, ScaleMode.ScaleToFit);
            GUILayout.Space(170);

            if (AwtterSdkInstaller.LoggedInUser != null) 
                LoggedIn();
        }

        void LoggedIn()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Logged in as {AwtterSdkInstaller.LoggedInUser?.Username ?? "-|-"} [{AwtterSdkInstaller.LoggedInUser?.Id}]!");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button($"Settings", _main.Shared.WindowCustomButton))
                AwtterSdkInstaller.ViewSettings = !AwtterSdkInstaller.ViewSettings;
            if (GUILayout.Button("Logout", _main.Shared.WindowCustomButton, GUILayout.MinHeight(16)))
            { 
                TokenCache.Token = string.Empty;
                AwtterSdkInstaller.LoggedIn = false;
            }
            EditorGUILayout.EndHorizontal();
        }

        void LinkButtons()
        {
            EditorGUILayout.BeginVertical();
            int count = 0;
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < _main.Settings.Buttons.Count; x++)
            {
                ButtonLink(_main.Settings.Buttons[x]);

                count++;

                switch (count)
                {
                    case 2:
                        EditorGUILayout.EndHorizontal();
                        count = 0;
                        if (x != _main.Settings.Buttons.Count-1)
                            EditorGUILayout.BeginHorizontal();
                        break;
                }
            }

            EditorGUILayout.EndVertical();
        }

        void ButtonLink(SplashButton button)
        {
            GUIContent content = new GUIContent();
            if (button.Image != null)
            {
                content.image = (Texture2D)button.Image;
                content.text = button.ButtonText;
            }
            else
            {
                content = new GUIContent(button.ButtonText);
            }
             
            if (GUILayout.Button(content, WindowCustomButton, GUILayout.MinWidth(160), GUILayout.MinHeight(25)))
            {
                switch (button.ButtonType)
                {
                    case SplashButton.bType.WebLink:
                        Application.OpenURL(button.Link);
                        break;
                    case SplashButton.bType.File:
                        if (button.Link != "")
                        {
                            if (button.Link == "changelogs")
                            {
                                ChangeLogsMenu.ShowChangelogs();
                            }
                            else
                            {
                                Application.OpenURL(Path.Combine(Paths.MainPath, button.Link));
                            }
                        }
                        break;
                }
            }
        }

        public void Bottom(Rect pos)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(32));
            GUILayout.Label("Show window on startup");
            GUI.color = AwtterSdkInstaller.ShowOnStartup ? Color.green : Color.red;
            if (GUILayout.Button(AwtterSdkInstaller.ShowOnStartup ? $"Enabled" : "Disabled", WindowCustomButton, GUILayout.MaxWidth(64)))
            {
                AwtterSdkInstaller.ShowOnStartup = !AwtterSdkInstaller.ShowOnStartup;
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }
    }
}

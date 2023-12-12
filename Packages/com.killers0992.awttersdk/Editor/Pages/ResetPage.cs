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
    public class ResetPage : IPage
    {
        private Texture2D _awttersdkwarningimage;
        public Texture2D AwtterSdkWarningImage
        {
            get
            {
                if (_awttersdkwarningimage == null)
                    _awttersdkwarningimage = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(Paths.MainPath, "Editor", "Textures", "warningsdk.png"));

                return _awttersdkwarningimage;
            }
        }

        private AwtterSdkInstaller _main;

        public void Load(AwtterSdkInstaller main)
        {
            _main = main;
        }

        public void DrawGUI(Rect pos)
        {
            GUI.DrawTexture(new Rect(0, 290, pos.size.x, 120), AwtterSdkWarningImage, ScaleMode.ScaleToFit);
            GUILayout.Space(135);

            EditorGUILayout.BeginVertical();
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("You are about to clean all the Awtter related content");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("from your current project.");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Do you wish to continue ?");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            GUILayout.Space(32);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.color = Color.green;

            if (GUILayout.Button($"YES", _main.Shared.WindowCustomButton3, GUILayout.MinWidth(50), GUILayout.MinHeight(32)))
            {
                FileUtil.DeleteFileOrDirectory("Assets/_Shade The Bat");
                FileUtil.DeleteFileOrDirectory("Assets/AwtterInstalledPackages.json");

                AssetDatabase.Refresh();

                AwtterSdkInstaller.ViewReset = !AwtterSdkInstaller.ViewReset;
            }
            GUI.color = Color.red;
            GUILayout.Space(50);

            if (GUILayout.Button($"BACK", _main.Shared.WindowCustomButton3, GUILayout.MinWidth(50), GUILayout.MinHeight(32)))
            {
                AwtterSdkInstaller.ViewReset = !AwtterSdkInstaller.ViewReset;
            }
            GUILayout.FlexibleSpace();
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }


        public void Reset()
        {
        }
    }
}

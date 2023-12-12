using AwtterSDK.Editor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AwtterSDK.Editor.Pages
{
    public class AddPackagesPage : IPage
    {
        private AwtterSdkInstaller _main;
        public GUIStyle CustomButton;
        public Vector2 Scroll = Vector2.zero;

        public void Load(AwtterSdkInstaller main)
        {
            _main = main;

            CustomButton = new GUIStyle(GUI.skin.button);
            CustomButton.fontSize = 15;
            CustomButton.alignment = TextAnchor.MiddleCenter;
        }

        public void DrawGUI(Rect pos)
        {
            GUILayout.Space(15);
            Utils.CreateBox("Additional packages");
            GUILayout.Space(15);
            Scroll = GUILayout.BeginScrollView(Scroll);
            if (AwtterSdkInstaller.AvaliableDlcs.Any(x => !x.IsProp && x.IsDlc && !x.IsPatreon))
            {
                Utils.CreateBox("DLCS");
                GUILayout.Space(5);
                foreach(var dlc in AwtterSdkInstaller.AvaliableDlcs.Where(x => x.IsDlc && !x.IsPatreon))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(TextureCache.GetTextureOrDownload(dlc.Icon), GUILayout.Height(32), GUILayout.Width(32));
                    GUI.color = dlc.Install ? Color.green : Color.white;
                    if (GUILayout.Button(dlc.Name, CustomButton, GUILayout.Height(32)))
                    {
                        dlc.Install = !dlc.Install;
                    }
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5);
            }

            if (AwtterSdkInstaller.AvaliableDlcs.Any(x => x.IsProp && !x.IsPatreon))
            {
                Utils.CreateBox("Props");
                GUILayout.Space(5);
                foreach (var prop in AwtterSdkInstaller.AvaliableDlcs.Where(x => x.IsProp && !x.IsPatreon ))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(TextureCache.GetTextureOrDownload(prop.Icon), GUILayout.Height(32), GUILayout.Width(32));
                    GUI.color = prop.Install ? Color.green : Color.white;
                    if (GUILayout.Button(prop.Name, CustomButton, GUILayout.Height(32)))
                    {
                        prop.Install = !prop.Install;
                    }
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5);
            }

            if (AwtterSdkInstaller.AvaliableTools.Any(x => !x.IsInstalled))
            {
                Utils.CreateBox("Tools");
                GUILayout.Space(5);
                foreach (var tool in AwtterSdkInstaller.AvaliableTools.Where(x => !x.IsInstalled))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(TextureCache.GetTextureOrDownload(tool.Icon), GUILayout.Height(32), GUILayout.Width(32));
                    GUI.color = tool.Install ? Color.green : Color.white;
                    if (GUILayout.Button(tool.Name, CustomButton, GUILayout.Height(32)))
                    {
                        tool.Install = !tool.Install;
                    }
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5);
            }

            GUILayout.EndScrollView();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUI.color = Color.red;
            if (GUILayout.Button("Go back", _main.Shared.WindowCustomButton3, GUILayout.MinHeight(27)))
                AwtterSdkInstaller.ViewAdditionalPackages = false;
            GUI.color = Color.white;

            GUILayout.Space(10);
            GUI.color = Color.green;
            if (GUILayout.Button("Run SDK Installer", _main.Shared.WindowCustomButton3, GUILayout.MinHeight(27)))
                AwtterSdkInstaller.IsInstalling = true;
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
        }


        public void Reset()
        {
        }
    }
}

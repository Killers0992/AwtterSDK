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
    public class PatreonBenefitsPage : IPage
    {
        private AwtterSdkInstaller _main;
        private Texture2D _awtterInboxImage;

        public Texture2D AwtterInboxImage
        {
            get
            {
                if (_awtterInboxImage == null)
                    _awtterInboxImage = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(Paths.MainPath, "Editor", "Textures", "inbox.png"));

                return _awtterInboxImage;
            }
        }

        public GUIStyle CustomButton;
        public GUIStyle CustomButton2;

        public Vector2 Scroll = Vector2.zero;

        public void Load(AwtterSdkInstaller main)
        {
            _main = main;

            CustomButton = new GUIStyle(GUI.skin.button);
            CustomButton.fontSize = 15;
            CustomButton.alignment = TextAnchor.MiddleCenter;

            CustomButton2 = new GUIStyle(GUI.skin.button);
            CustomButton2.fontSize = 15;
            CustomButton2.alignment = TextAnchor.MiddleCenter;
            CustomButton2.normal.textColor = Color.green;
        }

        public void DrawGUI(Rect pos)
        {
            GUILayout.Space(15);
            Utils.CreateBox("Patreon items");
            GUILayout.Space(15);
         
            if (AwtterSdkInstaller.Patreon == null || !AwtterSdkInstaller.Patreon.Active)
            {
                Utils.CreateBox("Looks like there's nothing here, are you a Patreon ?");
                if (AwtterInboxImage != null)
                    GUI.DrawTexture(new Rect(0, 360, pos.size.x, 180), AwtterInboxImage, ScaleMode.ScaleToFit);
                GUILayout.Space(238);
                End();
                return;
            }
            
            Scroll = GUILayout.BeginScrollView(Scroll);
            if (AwtterSdkInstaller.AvaliableBases.Any(x => x.IsPatreon))
            {
                Utils.CreateBox("BASES");
                GUILayout.Space(5);
                foreach (var baseModel in AwtterSdkInstaller.AvaliableBases.Where(x => x.IsPatreon))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(TextureCache.GetTextureOrDownload(baseModel.Icon), GUILayout.Height(32), GUILayout.Width(32));
                    if (GUILayout.Button(baseModel.Name, AwtterSdkInstaller.CurrentBase?.Id == baseModel.Id ? CustomButton2 : CustomButton, GUILayout.Height(32)))
                    {
                        AwtterSdkInstaller.CurrentBase = AwtterSdkInstaller.CurrentBase?.Id == baseModel.Id ? null : baseModel;
                        _main.UpdateAwtterPackages();
                    }

                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5);
            }
            if (AwtterSdkInstaller.AvaliableDlcs.Any(x => !x.IsProp && x.IsDlc && x.IsPatreon))
            {
                Utils.CreateBox("DLCS");
                GUILayout.Space(5);
                foreach (var dlc in AwtterSdkInstaller.AvaliableDlcs.Where(x => x.IsDlc && x.IsPatreon))
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
            if (AwtterSdkInstaller.AvaliableDlcs.Any(x => x.IsProp && x.IsPatreon))
            {
                Utils.CreateBox("Props");
                GUILayout.Space(5);
                foreach (var prop in AwtterSdkInstaller.AvaliableDlcs.Where(x => x.IsProp && x.IsPatreon))
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
            GUILayout.EndScrollView();
            GUILayout.Space(15);
            End();
        }

        void End()
        {
            GUILayout.BeginHorizontal();
            GUI.color = Color.red;
            if (GUILayout.Button("Go back", _main.Shared.WindowCustomButton3, GUILayout.MinHeight(27)))
                AwtterSdkInstaller.ViewPatreonItems = false;
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

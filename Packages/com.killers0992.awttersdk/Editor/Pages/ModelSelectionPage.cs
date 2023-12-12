using AwtterSDK.Editor.Installations;
using AwtterSDK.Editor.Interfaces;
using AwtterSDK.Editor.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using Unity.SharpZipLib.Zip;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace AwtterSDK.Editor.Pages
{
    public class ModelSelectionPage : IPage
    {
        public static Color OrangeColor = new Color(1.0f, 0.64f, 0.0f);
        public static Texture2D Green;

        public static bool CheckForChanges;

        public List<UnityPackageFile> PackagesToInstall = new List<UnityPackageFile>();

        public GUIStyle CustomButton2;
        public GUIStyle CustomButton;
        public GUIStyle CustomLabel;

        public Vector2 Scroll = Vector2.zero;
        public Vector2 InstallStatus = Vector2.zero;

        public bool ShowingBases;

        public bool IsUpToDate;

        private AwtterSdkInstaller _main;

        public void Load(AwtterSdkInstaller main)
        {
            Green = Utils.CreateTexture(10, 10, Color.green);
            _main = main;
            CustomLabel = new GUIStyle(GUI.skin.label);
            CustomLabel.richText = true;
            CustomLabel.alignment = TextAnchor.MiddleCenter;

            CustomButton = new GUIStyle(GUI.skin.button);
            CustomButton.fontSize = 15;
            CustomButton.alignment = TextAnchor.MiddleCenter;
            CustomButton.normal.textColor = Color.white;

            CustomButton2 = new GUIStyle(GUI.skin.button);
            CustomButton2.fontSize = 15;
            CustomButton2.alignment = TextAnchor.MiddleCenter;
            CustomButton2.normal.textColor = Color.green;

            CheckChanges();
        }

        void RefreshDlcs()
        {
            foreach (var dlc in AwtterSdkInstaller.AvaliableDlcs)
                dlc.Install = false;
        }

        void CheckChanges()
        {
            if (AwtterSdkInstaller.Products == null) return;

            RefreshDlcs();

            PackagesToInstall.Clear();

            foreach (var package in AwtterSdkInstaller.UnityPackages)
            {
                package.InstallStatus.Check();
                if (!package.InstallStatus.IsInstalled)
                    PackagesToInstall.Add(package);
            }
        }

        public void DrawGUI(Rect pos)
        {
            if (CheckForChanges)
                CheckChanges();

            GUILayout.Space(15);
            Utils.CreateBox("BASE MODEL SELECTION");
            GUILayout.Space(15);
            Scroll = GUILayout.BeginScrollView(Scroll);
            GUILayout.Space(5);
            foreach (var baseModel in AwtterSdkInstaller.AvaliableBases.Where(x => !x.IsPatreon))
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
            GUILayout.EndScrollView();
            GUILayout.Space(15);

            GUI.color = Color.cyan;
            if (GUILayout.Button("PATREON ITEMS", _main.Shared.WindowCustomButton3, GUILayout.MinHeight(15)))
                AwtterSdkInstaller.ViewPatreonItems = true;
            GUI.color = Color.white;
            GUILayout.Space(6);

            GUI.color = OrangeColor;
            if (GUILayout.Button("DLCs - Props - Tools ", _main.Shared.WindowCustomButton3, GUILayout.MinHeight(15)))
                AwtterSdkInstaller.ViewAdditionalPackages = true;
            GUI.color = Color.white;
            GUILayout.Space(6);
            GUI.color = Color.green;

            if (GUILayout.Button("Run SDK Installer", _main.Shared.WindowCustomButton3, GUILayout.MinHeight(27)))
                AwtterSdkInstaller.IsInstalling = true;

            GUI.enabled = true;
            GUI.color = Color.white;
            GUILayout.Space(15);

            GUILayout.Space(15f);

            InstallStatus = GUILayout.BeginScrollView(InstallStatus, false, true);
            bool anythingToShow = false;

            if (AwtterSdkInstaller.CurrentBase != null)
            {
                Utils.CreateBox("Base Model");
                GUILayout.BeginHorizontal();
                GUILayout.Box(TextureCache.GetTextureOrDownload(AwtterSdkInstaller.CurrentBase.Icon), GUILayout.Height(32), GUILayout.Width(32));
                GUILayout.Label(AwtterSdkInstaller.CurrentBase.Name, CustomLabel, GUILayout.Height(32));
                GUILayout.EndHorizontal();
                anythingToShow = true;
            }

            if (AwtterSdkInstaller.AvaliableDlcs.Any(x => x.Install))
            {
                Utils.CreateBox("DLCS");

                foreach (var dlc in AwtterSdkInstaller.AvaliableDlcs.Where(x => x.Install))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(TextureCache.GetTextureOrDownload(dlc.Icon), GUILayout.Height(32), GUILayout.Width(32));
                    GUILayout.Label(dlc.Name, CustomLabel, GUILayout.Height(32));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(15);
                }
                anythingToShow = true;
            }

            if (AwtterSdkInstaller.AvaliableTools.Any(x => x.Install))
            {
                Utils.CreateBox("Tools");

                foreach (var tool in AwtterSdkInstaller.AvaliableTools.Where(x => x.Install))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(TextureCache.GetTextureOrDownload(tool.Icon), GUILayout.Height(32), GUILayout.Width(32));
                    GUILayout.Label(tool.Name, CustomLabel, GUILayout.Height(32));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(15);
                }
                anythingToShow = true;
            }

            if (PackagesToInstall.Count != 0)
            {
                Utils.CreateBox("Unity Packages");

                foreach (var file in PackagesToInstall)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(TextureCache.GetTextureOrDownload(file.Icon), GUILayout.Height(32), GUILayout.Width(32));
                    GUILayout.Label(file.Name, CustomLabel, GUILayout.Height(32));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(15);
                }
                anythingToShow = true;
            }

            IsUpToDate = !anythingToShow;
            if (IsUpToDate)
                GUILayout.Label("Everything is up to date", CustomLabel);

            GUILayout.EndScrollView();
        }

        public void Reset()
        {
        }
    }
}

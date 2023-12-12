using AwtterSDK.Editor.Interfaces;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AwtterSDK.Editor.Pages
{
    public class ManagePackagesPage : IPage
    {
        private Vector2 _packagesScroll = Vector2.zero;
        private AwtterSdkInstaller _main;
        private GUIStyle CustomLabel;

        public void Load(AwtterSdkInstaller main)
        {
            _main = main;
            CustomLabel = new GUIStyle(GUI.skin.label);
            CustomLabel.richText = true;
            CustomLabel.alignment = TextAnchor.MiddleCenter;
        }

        public void DrawGUI(Rect pos)
        {
            EditorGUILayout.Space(10);
            _packagesScroll = GUILayout.BeginScrollView(_packagesScroll, false, true, GUILayout.Height(281));
            Utils.CreateBox("Base");
            GUILayout.BeginHorizontal();
            GUILayout.Box(TextureCache.GetTextureOrDownload(AwtterSdkInstaller.CurrentBase.Icon), GUILayout.Height(32), GUILayout.Width(32));
            GUILayout.Label(AwtterSdkInstaller.CurrentBase.Name, CustomLabel, GUILayout.Height(32));
            GUILayout.EndHorizontal();
            GUI.color = AwtterSdkInstaller.CurrentBase.IsOutdated ? Color.yellow : Color.green;
            GUI.enabled = false;
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Version {(AwtterSdkInstaller.CurrentBase.IsOutdated ? $"{AwtterSdkInstaller.CurrentBase.Version} > {AwtterSdkInstaller.CurrentBase.Version}" : AwtterSdkInstaller.CurrentBase.InstalledVersion)}");
            GUILayout.FlexibleSpace();
            GUILayout.Button(AwtterSdkInstaller.CurrentBase.IsOutdated ? $"🗱 Update" : $"✔ Installed", _main.Shared.WindowCustomButton3, GUILayout.Height(26), GUILayout.Width(150));
            GUILayout.EndHorizontal();
            GUI.enabled = true;
            GUI.color = Color.white;
            GUILayout.Space(30);

            if (AwtterSdkInstaller.AvaliableDlcs.Count != 0)
                Utils.CreateBox("DLCS");
            foreach (var dlc in AwtterSdkInstaller.AvaliableDlcs)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(TextureCache.GetTextureOrDownload(dlc.Icon), GUILayout.Height(32), GUILayout.Width(32));
                GUILayout.Label(dlc.Name, CustomLabel, GUILayout.Height(32));
                GUILayout.EndHorizontal();
                GUI.color = dlc.Install ? Color.yellow : dlc.IsInstalled ? Color.green : dlc.IsOutdated ? Color.yellow : Color.cyan;
                GUI.enabled = !dlc.IsInstalled;
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Version {(dlc.IsInstalled ? dlc.IsOutdated ? $"{dlc.Version} > {dlc.Version}" : dlc.InstalledVersion : dlc.Version)}");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(dlc.IsInstalled ? $"✔ Installed" : dlc.IsOutdated ? $"🗱 Update" : $"⇩ Install", _main.Shared.WindowCustomButton3, GUILayout.Height(26), GUILayout.Width(150)))
                {
                    dlc.Install = !dlc.Install;
                }
                GUILayout.EndHorizontal();
                GUI.enabled = true;
                GUI.color = Color.white;
                GUILayout.Space(30);
            }

            if (AwtterSdkInstaller.AvaliableTools.Count != 0)
                Utils.CreateBox("Tools");
            foreach (var tool in AwtterSdkInstaller.AvaliableTools)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(TextureCache.GetTextureOrDownload(tool.Icon), GUILayout.Height(32), GUILayout.Width(32));
                GUILayout.Label(tool.Name, CustomLabel, GUILayout.Height(32));
                GUILayout.EndHorizontal();
                GUI.color = tool.Install ? Color.yellow : tool.IsInstalled ? Color.green : tool.IsOutdated ? Color.yellow : Color.cyan;
                GUI.enabled = !tool.IsInstalled;
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Version {(tool.IsInstalled ? tool.IsOutdated ? $"{tool.Version} > {tool.Version}" : tool.InstalledVersion : tool.Version)}");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(tool.IsInstalled ? $"✔ Installed" : tool.IsOutdated ? $"🗱 Update" : $"⇩ Install", _main.Shared.WindowCustomButton3, GUILayout.Height(26), GUILayout.Width(150)))
                {
                    tool.Install = !tool.Install;
                }
                GUILayout.EndHorizontal();
                GUI.enabled = true;
                GUI.color = Color.white;
                GUILayout.Space(30);
            }
            GUILayout.EndScrollView();

            GUI.enabled = AwtterSdkInstaller.AvaliableDlcs.Any(x => x.Install) || AwtterSdkInstaller.AvaliableTools.Any(x => x.Install);
            GUI.color = (AwtterSdkInstaller.AvaliableDlcs.Any(x => x.Install) || AwtterSdkInstaller.AvaliableTools.Any(x => x.Install)) ? Color.green : Color.gray;
            if (GUILayout.Button("▶   Run SDK Installer", _main.Shared.WindowCustomButton3, GUILayout.Height(27)))
                AwtterSdkInstaller.IsInstalling = true;
            GUI.color = Color.white;
            GUI.enabled = true;
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button($"Show scenes"))
            {
                AwtterSdkInstaller.ViewManagePackages = !AwtterSdkInstaller.ViewManagePackages;
            }
            EditorGUILayout.EndVertical();
        }

        public void Reset()
        {
            _packagesScroll = Vector2.zero;
        }
    }
}

using AwtterSDK.Editor.Interfaces;
using AwtterSDK.Editor.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace AwtterSDK.Editor.Pages
{
    public class InstallProgressPage : IPage
    {
        private AwtterSdkInstaller _main;
        private Vector2 _progressScroll = Vector2.zero;

        private GUIStyle CustomLabel;

        public void Load(AwtterSdkInstaller main)
        {
            _main = main;
            CustomLabel = new GUIStyle(GUI.skin.label);
            CustomLabel.richText = true;
            CustomLabel.alignment = TextAnchor.MiddleCenter;
            EditorCoroutineUtility.StartCoroutine(DownloadFile(AwtterSdkInstaller.FilesToInstall), this);
        }

        public void DrawGUI(Rect pos)
        {
            GUILayout.Space(7);
            GUILayout.Label("Installation progress", CustomLabel);

            _progressScroll = GUILayout.BeginScrollView(_progressScroll, false, true);
            foreach (var file in AwtterSdkInstaller.FilesToInstall)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(TextureCache.GetTextureOrDownload(file.Icon), GUILayout.Height(45), GUILayout.Width(45));
                GUILayout.Label(file.Displayname, CustomLabel, GUILayout.Height(32));
                GUILayout.EndHorizontal();
                Rect myRect = GUILayoutUtility.GetLastRect();
                GUI.color = file.DownloadStatusColor;
                EditorGUI.ProgressBar(new Rect(myRect.x, myRect.y + 32f, myRect.width, 16f), file.DownloadProgress, String.Format("{0:P2}", file.DownloadProgress));
                GUI.color = Color.white;
                GUILayout.Space(30);
            }
            GUILayout.EndScrollView();
            GUILayout.Space(7);
            EditorGUILayout.HelpBox("Wait until installing finishes!", MessageType.Warning);
        }

        private bool Download(int index, int maximumIndex, FileToInstallModel file)
        {
            using (var www = UnityWebRequest.Get(file.DownloadUrl))
            {
                if (file.RequiresAuth)
                    www.SetRequestHeader("Authorization", $"Token {TokenCache.Token}");

                AsyncOperation request = www.SendWebRequest();

                while (!request.isDone)
                {
                    EditorUtility.DisplayProgressBar($"Downloading packages", $"Downloading {file.Displayname} {index}/{maximumIndex}", request.progress);
                }

                switch (www.responseCode)
                {
                    case 200:
                        FilesCache.CacheFile(file, www.downloadHandler.data);
                        return true;
                    default:
                        Debug.LogError($"[<color=orange>Awtter SDK</color>] Failed downloading {file.Displayname}, responseCode {www.responseCode}, message {www.downloadHandler.text}");
                        break;
                }
            }
            return false;
        }

        private IEnumerator DownloadFile(List<FileToInstallModel> files)
        {
            for(int x =0; x < files.Count; x++)
            {
                var file = files[x];

                if (!FilesCache.GetCachedFile(file))
                {
                    Download(x + 1, files.Count, file);
                    if (!FilesCache.GetCachedFile(file)) continue;
                }

                if (file.IsBaseModel)
                {
                    AwtterSdkInstaller.InstalledPackages.BaseModel = new InstalledPackageModel()
                    {
                        Id = file.Id,
                        Version = file.Version
                    };
                }
                else if (file.IsDLC)
                {
                    if (AwtterSdkInstaller.InstalledPackages.Dlcs.ContainsKey(file.Id)) break;

                    AwtterSdkInstaller.InstalledPackages.Dlcs.Add(file.Id, new InstalledPackageModel()
                    {
                        Id = file.Id,
                        Version = file.Version
                    });
                }
            }
            EditorUtility.ClearProgressBar();
            AwtterSdkInstaller.IsInstalling = false;
            yield break;
        }

        public void Reset()
        {
            _progressScroll = Vector2.zero;
        }
    }
}

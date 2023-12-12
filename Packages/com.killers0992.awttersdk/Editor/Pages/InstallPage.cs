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

namespace AwtterSDK.Editor.Pages
{
    class AwtterSdkAssets : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (AwtterSdkInstaller.IsInstalling) return;

            InstallPage.CheckForChanges = true;
            AwtterSdkInstaller.Refresh = true;
        }
    }

    public class InstallPage : IPage
    {
        public static bool CheckForChanges;

        public List<UnityPackageFile> PackagesToInstall = new List<UnityPackageFile>();

        public GUIStyle CustomButton;
        public GUIStyle CustomLabel;

        public Vector2 BasesScroll = Vector2.zero;
        public Vector2 DlcsScroll = Vector2.zero;
        public Vector2 InstallStatus = Vector2.zero;
        public Vector2 OutdateStatus = Vector2.zero;

        public bool ShowingBases;

        public bool IsUpToDate;

        private AwtterSdkInstaller _main;

        public void Load(AwtterSdkInstaller main)
        {
            _main = main;
            CustomLabel = new GUIStyle(GUI.skin.label);
            CustomLabel.richText = true;
            CustomLabel.alignment = TextAnchor.MiddleCenter;

            CustomButton = new GUIStyle(GUI.skin.button);
            CustomButton.fontSize = 15;
            CustomButton.alignment = TextAnchor.MiddleCenter;

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

            GUILayout.Space(10f);

            if (EditorGUILayout.DropdownButton(AwtterSdkInstaller.CurrentBase == null ? new GUIContent("Select base to install") :
                new GUIContent($"Selected base {AwtterSdkInstaller.CurrentBase.Name}"), FocusType.Keyboard, GUILayout.Height(25)))
                ShowingBases = !ShowingBases;

            GUILayout.Space(10f);

            if (ShowingBases)
            {
                GUILayout.BeginVertical("window", GUILayout.Height(130f));
                BasesScroll = GUILayout.BeginScrollView(BasesScroll);
                foreach (var baseModel in AwtterSdkInstaller.AvaliableBases.Where(x => !x.IsInstalled))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(TextureCache.GetTextureOrDownload(baseModel.Icon), GUILayout.Height(32), GUILayout.Width(32));
                    GUI.color = AwtterSdkInstaller.CurrentBase == baseModel ? Color.green : Color.white;
                    if (GUILayout.Button(baseModel.Name, CustomButton, GUILayout.Height(32)))
                    {
                        if (AwtterSdkInstaller.CurrentBase != baseModel)
                        {
                            AwtterSdkInstaller.CurrentBase = baseModel;
                            _main.UpdateAwtterPackages();
                            ShowingBases = false;
                        }
                    }
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.BeginVertical("window", GUILayout.Height(130f));
                if (AwtterSdkInstaller.AvaliableDlcs.Where(x => !x.IsInstalled).Count() == 0)
                {
                    GUILayout.Label("You dont have any owned DLCS!", CustomLabel);
                }
                else
                {
                    DlcsScroll = GUILayout.BeginScrollView(DlcsScroll);
                    foreach (var dlc in AwtterSdkInstaller.AvaliableDlcs.Where(x => !x.IsInstalled))
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
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
            }
            GUILayout.Space(15f);

            if (GUILayout.Button("▶   Run SDK Installer", _main.Shared.WindowCustomButton3, GUILayout.MinHeight(27)))
                AwtterSdkInstaller.IsInstalling = true;

            GUI.enabled = true;

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

      /*  public IEnumerator InstallationProcess()
        {


            int currentPackage = 1;
            foreach (var vrcFile in AwtterSdkInstaller.VrcFilesToInstall)
            {
                using (var www = UnityWebRequest.Get(vrcFile.Url))
                {
                    AsyncOperation request = www.SendWebRequest();

                    while (!request.isDone)
                    {
                        EditorUtility.DisplayProgressBar($"Downloading VRC Packages ( {currentPackage}/{AwtterSdkInstaller.VrcFilesToInstall.Count} )", vrcFile.DisplayName, www.downloadProgress);
                    }

                    switch (www.responseCode)
                    {
                        case 200:
                            File.WriteAllBytes(Path.Combine(packagesFolder, $"{vrcFile.Name}.zip"), www.downloadHandler.data);

                            zip.ExtractZip(Path.Combine(packagesFolder, $"{vrcFile.Name}.zip"), Path.Combine(packagesFolder, vrcFile.Name), null);

                            File.Delete(Path.Combine(packagesFolder, $"{vrcFile.Name}.zip"));
                            break;
                        default:
                            Debug.LogError($"[<color=orange>Awtter SDK</color>] Failed downloading vrchat package {vrcFile.Name}, responseCode {www.responseCode}, message {www.downloadHandler.text}");
                            break;
                    }
                    EditorUtility.ClearProgressBar();
                }
                currentPackage++;
            }

            currentPackage = 1;
            foreach (var package in PackagesToInstall)
            {
                bool poi = false;
                if (package.InstallStatus is PoyomiInstallation d)
                {
                    package.Url = $"";
                    poi = true;
                }

                using (var www = UnityWebRequest.Get(package.Url))
                {
                    if (poi)
                        www.SetRequestHeader("Authorization", $"Token {TokenCache.Token}");

                    AsyncOperation request = www.SendWebRequest();

                    while (!request.isDone)
                    {
                        EditorUtility.DisplayProgressBar($"Downloading Unity Packages ( {currentPackage}/{AwtterSdkInstaller.UnityPackages.Count} )", package.Name, www.downloadProgress);
                    }

                    switch (www.responseCode)
                    {
                        case 200:

                            break;
                        default:
                            Debug.LogError($"[<color=orange>Awtter SDK</color>] Failed downloading unity package {package.Name}, responseCode {www.responseCode}, message {www.downloadHandler.text}");
                            break;
                    }
                    EditorUtility.ClearProgressBar();
                }
                currentPackage++;
            }

            if (!AwtterSdkInstaller.IsBaseIntalled && AwtterSdkInstaller.CurrentBase != null)
            {
                using (var www = UnityWebRequest.Get(AwtterSdkInstaller.CurrentBase.DownloadUrl))
                {
                    www.SetRequestHeader("Authorization", $"Token {TokenCache.Token}");

                    AsyncOperation request = www.SendWebRequest();

                    while (!request.isDone)
                    {
                        EditorUtility.DisplayProgressBar($"Downloading Base Model", AwtterSdkInstaller.CurrentBase.Name, www.downloadProgress);
                    }

                    switch (www.responseCode)
                    {
                        case 200:
                            string fileName = "BaseModel.unitypackage";

                            File.WriteAllBytes(Path.Combine(Application.dataPath, fileName), www.downloadHandler.data);

                            AssetDatabase.ImportPackage(Path.Combine(Application.dataPath, fileName), false);
                            File.Delete(Path.Combine(Application.dataPath, fileName));

                            AwtterSdkInstaller.InstalledPackages.BaseModel = new InstalledPackageModel()
                            {
                                Id = AwtterSdkInstaller.CurrentBase.Id,
                                Version = AwtterSdkInstaller.CurrentBase.Version
                            };
                            break;
                        default:
                            Debug.LogError($"[<color=orange>Awtter SDK</color>] Failed downloading model base {AwtterSdkInstaller.CurrentBase.Name}, responseCode {www.responseCode}, message {www.downloadHandler.text}");
                            break;
                    }
                    EditorUtility.ClearProgressBar();
                }
            }

            currentPackage = 1;
            foreach (var dlc in SelectedDlcs)
            {
                using (var www = UnityWebRequest.Get(dlc.DownloadUrl))
                {
                    www.SetRequestHeader("Authorization", $"Token {TokenCache.Token}");

                    AsyncOperation request = www.SendWebRequest();

                    while (!request.isDone)
                    {
                        EditorUtility.DisplayProgressBar($"Downloading Dlcs ( {currentPackage}/{SelectedDlcs.Count} )", dlc.Name, www.downloadProgress);
                    }

                    switch (www.responseCode)
                    {
                        case 200:
                            string fileName = $"Dlc{dlc.Id}.unitypackage";

                            File.WriteAllBytes(Path.Combine(Application.dataPath, fileName), www.downloadHandler.data);

                            AssetDatabase.ImportPackage(Path.Combine(Application.dataPath, fileName), false);
                            File.Delete(Path.Combine(Application.dataPath, fileName));


                            break;
                        default:
                            Debug.LogError($"[<color=orange>Awtter SDK</color>] Failed downloading dlc {dlc.Name}, responseCode {www.responseCode}, message {www.downloadHandler.text}");
                            break;
                    }

                    EditorUtility.ClearProgressBar();
                }
            }

            _main.SaveInstalledPackagesStorage();
            _main.UpdateAwtterPackages();
            IsInstalling = false;
            yield break;
        }*/

        public void Reset()
        {
        }
    }
}

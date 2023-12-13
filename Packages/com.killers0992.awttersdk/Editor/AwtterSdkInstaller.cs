namespace AwtterSDK
{
    using UnityEditor;

    using System.Collections.Generic;

    using AwtterSDK.Editor.Pages;
    using AwtterSDK.Editor.Interfaces;
    using AwtterSDK.Editor.Models.API;
    using AwtterSDK.Editor;
    using AwtterSDK.Editor.Models;
    using AwtterSDK.Editor.Installations;
    using UnityEngine;
    using AWBOI.SplashScreen;
    using Unity.EditorCoroutines.Editor;
    using System.IO;
    using Newtonsoft.Json;
    using System.Linq;
    using AwtterSDK.Editor.Enums;
    using System;
    using Assets.Awtter_SDK.Editor.Models;

    class SdkRefresh : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            AwtterSdkInstaller.Refresh = true;
        }
    }

    public class AwtterSdkInstaller : EditorWindow
    {
        private static AwtterSdkInstaller _window;
        private static string _installedPackagesPath => Path.Combine(Application.dataPath, "AwtterInstalledPackages.json");

        private static bool? _showOnStartup;
        public static bool ShowOnStartup
        {
            get
            {
                if (!_showOnStartup.HasValue)
                    _showOnStartup = EditorPrefs.GetBool("AwShowOnStartup", true);

                return _showOnStartup.Value;
            }
            set
            {
                EditorPrefs.SetBool("AwShowOnStartup", value);
                _showOnStartup = value;
            }
        }

        static AwtterSdkInstaller()
        {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }

        public static void CloseWindow()
        {
            if (_window == null) return;

            _window.Close();
            _window = null;
        }


        [MenuItem("Awtter tools/ASDK control panel")]
        static void Init()
        {
            _window = (AwtterSdkInstaller)EditorWindow.GetWindow(typeof(AwtterSdkInstaller), false, "Awtter SDK");
            _window.minSize = new Vector2(320f, 640f);

            _window.Show();
        }


        static void OnEditorUpdate()
        {
            EditorApplication.update -= OnEditorUpdate;

            if (ShowOnStartup)
                Init();
        }

        private static bool _isLoggedIn = false;

        public static bool Refresh;

        public static SdkStatus LastPage;

        public static bool UpdateCurrentUser;


        public static ToolboxOkResponseModel Toolbox;
        
        public static UserModel LoggedInUser;

        public static PatreonOkResponseModel Patreon;

        public static bool LoggedIn
        {
            get => _isLoggedIn;
            set
            {
                if (value)
                    UpdateCurrentUser = true;
                else
                {
                    LoggedInUser = null;
                }

                _isLoggedIn = value;
            }
        }


        public static VrcPackages VrcPackages;

        public static bool RefreshAwtterPackages;
        private static ProductsModel _products;

        public static ProductsModel Products
        {
            get => _products;
            set
            {
                if (_products == value) return;

                _products = value;
                RefreshAwtterPackages = true;
            }
        }

        public static Dictionary<SdkStatus, IPage> Pages = new Dictionary<SdkStatus, IPage>()
        {
            { SdkStatus.NotLoggedIn, new LoginPage() },
            { SdkStatus.TosNotAccepted, new TosPage() },
            { SdkStatus.BaseNotInstalled, new ModelSelectionPage() },
            { SdkStatus.ViewAdditionalPackages , new AddPackagesPage() },
            { SdkStatus.InstallInProgress, new InstallProgressPage() },
            { SdkStatus.BaseInstalled, new ScenesPage() },
            { SdkStatus.ViewSettings, new SettingsPage() },
            { SdkStatus.ManagePackages, new ManagePackagesPage() },
            { SdkStatus.PatreonItems, new PatreonBenefitsPage() },
            { SdkStatus.ResetPage, new ResetPage() },
        };

        public static List<UnityPackageFile> UnityPackages = new List<UnityPackageFile>();

        void AddIfMissing(bool force = false)
        {
            if (Shared == null || force)
            {
                Shared = new SharedPage(this);
            }
            else if (Shared != null && Shared._main == null)
            {
                Shared._main = this;
            }

            if (UpdateCurrentUser || force)
            {
                SaveInstalledPackagesStorage();
            
                EditorCoroutineUtility.StartCoroutine(AwtterApi.GetCurrentUser(), this);
                EditorCoroutineUtility.StartCoroutine(AwtterApi.GetPatreon(), this);
                EditorCoroutineUtility.StartCoroutine(AwtterApi.GetToolbox(), this);
                UpdateCurrentUser = false;
            }

            if ((RefreshAwtterPackages || force) && Products?.Data != null)
            {
                UpdateAwtterPackages();
                RefreshAwtterPackages = false;
            }
        }

        public void UpdateAwtterPackages()
        {
            AvaliableBases = Products.Data.SelectMany((x) =>
            {
                var files = x.Files.Where(y =>
                {
                    if (!y.IsBaseModel) return false;

                    if (y.IsInstalled)
                        CurrentBase = y.ToBaseView(x);

                    return true;
                });
                return files.Select(z => z.ToBaseView(x));
            }).ToList();

            if (Patreon?.Benefits != null)
                AvaliableBases.AddRange(Patreon.Benefits.SelectMany((x) =>
                {
                    var files = x.Files.Where(y =>
                    {
                        if (!y.IsBaseModel) return false;

                        if (y.IsInstalled)
                            CurrentBase = y.ToBaseView(x, true);

                        return true;
                    });
                    return files.Select(z => z.ToBaseView(x, true));
                }).ToList());

            AvaliableDlcs = Products.Data.SelectMany((x) =>
            {
                var files = x.Files.Where(y =>
                {
                    if (!y.IsDLC) return false;

                    if (!y.IsProp && x.BaseName != CurrentBase?.BaseName) return false;

                    if (y.Id == CurrentBase?.Id) return false;

                    return true;
                });

                return files.Select(z => z.ToDLCView(x));
            }).ToList();

            if (Patreon != null && Patreon.Benefits != null)
                AvaliableDlcs.AddRange(Patreon.Benefits.SelectMany((x) =>
                {
                    var files = x.Files.Where(y =>
                    {
                        if (!y.IsDLC) return false;

                        if (!y.IsProp && x.BaseName != CurrentBase?.BaseName) return false;

                        if (y.Id == CurrentBase?.Id) return false;

                        return true;
                    });

                    return files.Select(z => z.ToDLCView(x, true));
                }).ToList());

            if (Toolbox?.Files != null)
                AvaliableTools = Toolbox.Files.Select((x) => x.ToToolView(Toolbox)).ToList();
        }
         
        public void SaveInstalledPackagesStorage()
        {
            if (!File.Exists(_installedPackagesPath))
            {
                File.WriteAllText(_installedPackagesPath, JsonConvert.SerializeObject(new InstalledPackagesModel(), Formatting.Indented));
            }
            else if (InstalledPackages != null)
            {
                File.WriteAllText(_installedPackagesPath, JsonConvert.SerializeObject(InstalledPackages, Formatting.Indented));
            }


            InstalledPackages = JsonConvert.DeserializeObject<InstalledPackagesModel>(File.ReadAllText(_installedPackagesPath));
            InstalledPackages.CheckTools();

            if (InstalledPackages.BaseModel == null)
                CurrentBase = null;

            RefreshAwtterPackages = true;
        }

        public static InstalledPackagesModel InstalledPackages { get; private set; }

        private AwboiSplashButtons _settings;
        public AwboiSplashButtons Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = AssetDatabase.LoadAssetAtPath<AwboiSplashButtons>(Path.Combine(Paths.MainPath, "Editor", "Resources", "SplashMenuButtons.asset"));

                    if (_settings == null)
                    {
                        Debug.Log("still null");
                    }

                    SaveInstalledPackagesStorage();
                }

                return _settings;
            }
        }

        public SharedPage Shared { get; private set; }

        public static bool IsBaseIntalled => CurrentBase?.IsInstalled == true;
        public static BaseView CurrentBase { get; internal set; }

        public static List<FileToInstallModel> FilesToInstall = new List<FileToInstallModel>();

        public static List<BaseView> AvaliableBases { get; private set; } = new List<BaseView>();
        public static List<DlcView> AvaliableDlcs { get; private set; } = new List<DlcView>();
        public static List<ToolView> AvaliableTools { get; private set; } = new List<ToolView>();

        private static bool _isInstalling;

        public static bool IsInstalling
        {
            get => _isInstalling;
            set
            {
                if (!value)
                {
                    _isInstalling = value;
                    Refresh = true;
                    ScenesPage.DoRefreshScenes = true;
                    return;
                }

                FilesToInstall.Clear();

                foreach (var pack in UnityPackages)
                {
                    if (pack.InstallStatus.IsInstalled) continue;

                    FilesToInstall.Add(new FileToInstallModel()
                    {
                        Id = pack.Id,
                        Name = pack.Name,
                        Displayname = pack.Name,
                        Icon = pack.Icon,
                        DownloadUrl = pack.Url,
                        RequiresAuth = pack.RequiresAuth
                    });
                }

                if (CurrentBase?.IsInstalled == false)
                {
                    FilesToInstall.Add(new FileToInstallModel()
                    {
                        Id = CurrentBase.Id,
                        Name = CurrentBase.Name,
                        Version = CurrentBase.Version,
                        IsBaseModel = true,
                        Displayname = CurrentBase.Name,
                        Icon = CurrentBase.Icon,
                        DownloadUrl = CurrentBase.DownloadUrl,
                        RequiresAuth = true,
                    });
                }


                foreach (var dlc in AvaliableDlcs.Where(x => x.Install))
                {
                    FilesToInstall.Add(new FileToInstallModel()
                    {
                        Id = dlc.Id,
                        Name = dlc.Name,
                        Version = dlc.Version,
                        IsDLC = true,
                        Displayname = dlc.Name,
                        Icon = dlc.Icon,
                        DownloadUrl = dlc.DownloadUrl,
                        RequiresAuth = true,
                    });
                    dlc.Install = false;
                }

                foreach (var tool in AvaliableTools.Where(x => x.Install))
                {
                    FilesToInstall.Add(new FileToInstallModel()
                    {
                        Id = tool.Id,
                        Name = tool.Name,
                        Version = tool.Version,
                        Displayname = tool.Name,
                        Icon = tool.Icon,
                        DownloadUrl = tool.DownloadUrl,
                        RequiresAuth = true,
                    });
                    tool.Install = false;
                }

                _isInstalling = value;
            }
        }

        public static bool ViewManagePackages;
        public static bool ViewSettings;
        public static bool ViewAdditionalPackages;
        public static bool ViewPatreonItems;
        public static bool ViewReset;

        public static DateTime RemoteConfigGetDelay = DateTime.Now;
        public static ConfigResponseModel RemoteConfig = null;

        public static SdkStatus CurrentStatus
        {
            get
            {
                if (!LoggedIn || RemoteConfig == null)
                    return SdkStatus.NotLoggedIn;

                if (!TosPage.TosAccepted || (RemoteConfig != null && RemoteConfig.Tos.Version > TosPage.TosVersion))
                    return SdkStatus.TosNotAccepted;

                if (IsInstalling)
                    return SdkStatus.InstallInProgress;

                if (ViewSettings)
                    return SdkStatus.ViewSettings;

                if (ViewAdditionalPackages)
                    return SdkStatus.ViewAdditionalPackages;

                if (ViewPatreonItems)
                    return SdkStatus.PatreonItems;

                if (!IsBaseIntalled)
                    return SdkStatus.BaseNotInstalled;

                if (ViewReset)
                    return SdkStatus.ResetPage;

                if (ViewManagePackages)
                    return SdkStatus.ManagePackages;

                return SdkStatus.BaseInstalled;
            }
        }

        void OnGUI()
        {
            if (RemoteConfigGetDelay < DateTime.Now)
            {
                RemoteConfigGetDelay = DateTime.Now.AddSeconds(30);
                EditorCoroutineUtility.StartCoroutine(AwtterApi.GetConfig(), this);
            }

            AddIfMissing(Refresh);

            if (Refresh)
                Refresh = false;

            TextureCache.DownloadIfNeeded();

            if (LastPage != CurrentStatus)
            {
                if (Pages.ContainsKey(LastPage))
                    Pages[LastPage].Reset();

                LastPage = CurrentStatus;
                Pages[LastPage].Load(this);
            }

            Shared.Top(position);

            if (Application.isPlaying)
            {
                EditorGUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Exit playmode to access panel", Shared.WindowCustomButton);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
            }
            else
                Pages[LastPage].DrawGUI(position);

            Shared.Bottom(position);
        }
    }
}

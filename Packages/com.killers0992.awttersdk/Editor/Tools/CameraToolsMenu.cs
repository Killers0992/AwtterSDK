namespace AwtterSDK.Editor.Tools
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering;

    public class CameraToolsMenu : EditorWindow
    {
        public class CameraRes
        {
            public CameraRes(string name, int width, int height, string scan, string ratio)
            {
                Name = name;
                Resolution = new Vector3(width, height);
                Scan = scan;
                Ratio = ratio;
            }

            public string Name { get; set; }
            public Vector3 Resolution { get; set; }
            public string Scan { get; set; }
            public string Ratio { get; set; }
        }

        public static List<CameraRes> Resolutions { get; } = new List<CameraRes>()
        {
            new CameraRes("16K Full", 16384, 8640, "", "16384∶8640"),
            new CameraRes("16K", 15360, 8640, "", "16:9"),
            new CameraRes("12K", 12288, 6480, "", "16:9"),
            new CameraRes("UW10K", 10240, 4320, "", "21:9"),
            new CameraRes("8K UHD", 7680, 4320, "4320p", "16:9"),
            new CameraRes("4K UHD", 3840, 2160, "2160p", "16:9"),
            new CameraRes("UWQHD", 3440, 1440, "", "21:9"),
            new CameraRes("WQHD", 2560, 1440, "", "16:9"),
            new CameraRes("Full HD", 1920, 1080, "1080p", "16:9"),
            new CameraRes("", 1440, 1080, "1080i", "16:9"),
            new CameraRes("", 1280, 1080, "1880i", "16:9"),
            new CameraRes("HD", 1280, 720, "720p", "16:9"),
            new CameraRes("VGA", 640, 480, "", "4:3"),
            new CameraRes("Custom", -1, -1, "", ""),
        };

        public static Camera TargetCamera;
        public static int PictureResolutionWidth = 4096;
        public static int PictureResolutionHeight = 2048;

        [MenuItem("Awtter tools/Camera tool")]
        static void Init()
        {
            CameraToolsMenu window = (CameraToolsMenu)EditorWindow.GetWindow(typeof(CameraToolsMenu), false, "Camera tool");
            window.minSize = new Vector2(288f, 250f);
            window.Show();
        }

        private RenderTexture PictureRT;
        private bool Transparent;
        private string OutputPath = String.Empty;

        private Camera[] CamerasInScene;

        void RefreshCameras()
        {
            CamerasInScene = UnityEngine.Object.FindObjectsOfType<Camera>();
        }

        private Vector2 CamerasScroll = Vector2.zero;


        private Vector2 ResolutionsScroll = Vector2.zero;
        private int SelectedResolutionIndex = 0;

        private GUIStyle ButtonCustom1;
        private GUIStyle ButtonCustom2;

        public void OnGUI()
        {
            if (ButtonCustom1 == null)
            {
                ButtonCustom1 = new GUIStyle(GUI.skin.button);
                ButtonCustom1.alignment = TextAnchor.MiddleLeft;
            }

            if (ButtonCustom2 == null)
            {
                ButtonCustom2 = new GUIStyle(EditorStyles.boldLabel);
                ButtonCustom2.alignment = TextAnchor.MiddleLeft;
                ButtonCustom2.focused.textColor = Color.cyan;
            }

            if (CamerasInScene == null)
                RefreshCameras();

            GUILayout.BeginVertical();
            Utils.CreateBox("Target camera");
            TargetCamera = (Camera)EditorGUILayout.ObjectField(TargetCamera, typeof(Camera), true);
            GUILayout.Space(5);
            GUILayout.Label("Select camera from current scene:");
            CamerasScroll = GUILayout.BeginScrollView(CamerasScroll, false, true, GUILayout.Height(60));

            foreach(var camera in CamerasInScene)
            {
                if (TargetCamera == camera) GUI.color = Color.green;
                if (GUILayout.Button(camera.name))
                {
                    if (TargetCamera == camera)
                        TargetCamera = null;
                    else
                    {
                        TargetCamera = camera;
                        EditorApplication.ExecuteMenuItem("Window/General/Game");
                    }
                }
                GUI.color = Color.white;
            }

            GUILayout.EndScrollView();
            if (GUILayout.Button("Refresh", GUILayout.Height(16)))
            {
                RefreshCameras();
            }
            GUILayout.EndVertical();

            GUILayout.Space(5f);
            MakeBox("Picture");

            GUILayout.Space(5);
            GUILayout.Label("Select resolution:");

            GUILayout.BeginHorizontal();
            Utils.CreateBox("Name", true, GUILayout.Width(55));
            GUILayout.FlexibleSpace();
            Utils.CreateBox("Resolution", true, GUILayout.Width(80));
            GUILayout.FlexibleSpace();
            Utils.CreateBox("Scan", true, GUILayout.Width(50));
            GUILayout.FlexibleSpace();
            Utils.CreateBox("Ratio", true, GUILayout.Width(90));
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
                        
            ResolutionsScroll = GUILayout.BeginScrollView(ResolutionsScroll, false, true, GUILayout.Height(150));

            for(int x = 0; x < Resolutions.Count; x++)
            {
                if (SelectedResolutionIndex == x) GUI.color = Color.green;

                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                if (GUILayout.Button(Resolutions[x].Name, ButtonCustom2, GUILayout.Width(55)))
                {
                    SelectedResolutionIndex = x;
                    PictureResolutionWidth = (int)Resolutions[x].Resolution.x;
                    PictureResolutionHeight = (int)Resolutions[x].Resolution.y;
                }
                GUILayout.FlexibleSpace();
                if (Resolutions[x].Resolution.x == -1 && SelectedResolutionIndex == x)
                {
                    GUI.color = Color.white;
                    PictureResolutionWidth = EditorGUILayout.IntField(PictureResolutionWidth,GUILayout.Width(35));
                    GUILayout.Label("x", GUILayout.Width(10));
                    PictureResolutionHeight = EditorGUILayout.IntField(PictureResolutionHeight, GUILayout.Width(35));
                    GUI.color = Color.green;
                }
                else
                {
                    if (GUILayout.Button(Resolutions[x].Resolution.x != -1 ? $"{Resolutions[x].Resolution.x}x{Resolutions[x].Resolution.y}" : "", ButtonCustom2, GUILayout.Width(80)))
                    {
                        SelectedResolutionIndex = x;
                        PictureResolutionWidth = (int)Resolutions[x].Resolution.x;
                        PictureResolutionHeight = (int)Resolutions[x].Resolution.y;
                    }
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(Resolutions[x].Scan, ButtonCustom2, GUILayout.Width(50)))
                {
                    SelectedResolutionIndex = x;
                    PictureResolutionWidth = (int)Resolutions[x].Resolution.x;
                    PictureResolutionHeight = (int)Resolutions[x].Resolution.y;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(Resolutions[x].Ratio, ButtonCustom2, GUILayout.Width(90)))
                {
                    SelectedResolutionIndex = x;
                    PictureResolutionWidth = (int)Resolutions[x].Resolution.x;
                    PictureResolutionHeight = (int)Resolutions[x].Resolution.y;
                }
                GUILayout.EndHorizontal();

                GUI.color = Color.white;
            }

            GUILayout.EndScrollView();
            
            GUI.enabled = TargetCamera != null;
            Transparent = GUILayout.Toggle(Transparent, "Transparent");

            if (TargetCamera != null)
            {
                if (TargetCamera.clearFlags == CameraClearFlags.Depth && !Transparent)
                    TargetCamera.clearFlags = CameraClearFlags.Skybox;
                else if (TargetCamera.clearFlags != CameraClearFlags.Depth && Transparent)
                    TargetCamera.clearFlags = CameraClearFlags.Depth;
            }

            GUI.enabled = true;
            GUILayout.Space(15f);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Output path");
            if (GUILayout.Button("Open"))
            {
                OutputPath = EditorUtility.OpenFolderPanel("Output path", OutputPath, "Pics");
            }
            GUILayout.EndHorizontal();
            GUI.enabled = false;
            GUILayout.TextField(OutputPath);
            GUI.enabled = true;
            GUI.enabled = TargetCamera != null;

            if (GUILayout.Button("Take picture"))
                CapturePicture(TargetCamera, PictureResolutionWidth, PictureResolutionHeight);

            GUI.enabled = true;
        }

        void MakeBox(string name)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label(name, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public void CapturePicture(Camera targetCamera, int width, int height)
        {
            PictureRT = new RenderTexture(new RenderTextureDescriptor(width, width));
            PictureRT.depth = 24;

            PictureRT.dimension = TextureDimension.Tex2D;

            if (PictureRT.width != width)
                PictureRT.width = width;

            if (PictureRT.height != height)
                PictureRT.height = height;

            targetCamera.targetTexture = PictureRT;
            targetCamera.Render();

            Save("Picture", PictureRT);

            targetCamera.targetTexture = null;
            DestroyImmediate(PictureRT);
        }

        public void Save(string type, RenderTexture rt)
        {
            Texture2D tex = new Texture2D(rt.width, rt.height, Transparent ? TextureFormat.ARGB32 : TextureFormat.RGB565, false);

            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            RenderTexture.active = null;

            byte[] bytes = tex.EncodeToPNG();

            if (!Directory.Exists(OutputPath))
                Directory.CreateDirectory(OutputPath);

            string path = Path.Combine(OutputPath, $"{type}_{DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss")}.png");

            File.WriteAllBytes(path, bytes);
            Debug.Log($"{type} saved at location {path}");
        }
    }
}

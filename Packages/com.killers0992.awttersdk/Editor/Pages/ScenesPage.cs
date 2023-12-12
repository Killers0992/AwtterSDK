using AwtterSDK.Editor.Interfaces;
using AwtterSDK.Editor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AwtterSDK.Editor.Pages
{
    public class ScenesPage : IPage
    {
        private AwtterSdkInstaller _main;
        private Vector2 _scenesScroll = Vector2.zero;
        private List<SceneInfo> _scenes = new List<SceneInfo>();

        public static bool DoRefreshScenes;

        public void Load(AwtterSdkInstaller main)
        {
            _main = main;
            RefreshScenes();
        }

        void RefreshScenes()
        {
            _scenes = Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories)
                .Select(x => new SceneInfo()
                {
                    SceneName = Path.GetFileNameWithoutExtension(x),
                    FullPath = x,
                }).ToList();
        }

        public void DrawGUI(Rect pos)
        {
            if (DoRefreshScenes)
                RefreshScenes();

            EditorGUILayout.Space(10);
            Utils.CreateBox("Select your scene!");
            _scenesScroll = EditorGUILayout.BeginScrollView(_scenesScroll, GUILayout.Height(240));

            foreach (var scene in _scenes)
            {
                GUI.color = EditorSceneManager.GetActiveScene().name == scene.SceneName ? Color.green : Color.white;
                if (GUILayout.Button($"{scene.SceneName}", GUILayout.Height(30)))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        EditorSceneManager.OpenScene(scene.FullPath);
                }
                GUI.color = Color.white;
            }

            EditorGUILayout.EndScrollView();

            if (AwtterSdkInstaller.CurrentBase.IsOutdated || AwtterSdkInstaller.AvaliableDlcs.Any(x => x.IsOutdated))
            {
                EditorGUILayout.HelpBox(String.Concat(
                    AwtterSdkInstaller.CurrentBase.IsOutdated ? $"Your base is outdated!" + Environment.NewLine : String.Empty,
                    AwtterSdkInstaller.AvaliableDlcs.Any(x => x.IsOutdated) ? "Your DLCS are outdated!" : String.Empty), MessageType.Warning);
            }
            else
                GUILayout.Space(5);

            EditorGUILayout.BeginVertical();
            GUI.color = ModelSelectionPage.OrangeColor;
            if (GUILayout.Button($"Manage packages", _main.Shared.WindowCustomButton3))
            {
                AwtterSdkInstaller.ViewManagePackages = !AwtterSdkInstaller.ViewManagePackages;
            }
            GUI.color = Color.red;
            if (GUILayout.Button("Reset", _main.Shared.WindowCustomButton3))
            {
                AwtterSdkInstaller.ViewReset = true;
            }
            GUI.color = Color.white;
            EditorGUILayout.EndVertical();
        }

        public void Reset()
        {
        }
    }
}

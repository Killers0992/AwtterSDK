using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text;
using System.Security.Cryptography;
using System;
using UnityEditor.SceneManagement;
using AwtterSDK;

namespace AWBOI.SplashScreen
{
    [CustomEditor(typeof(AWBOI.SplashScreen.AwboiSplashButtons))]
    public class AwboiSplashButtonsInspector : Editor
    {
        public bool NamesFoldout = false;
        public bool ButtonsFoldout = false;

        #region EDITOR_GUI

        public override void OnInspectorGUI()
        {
            if (AwtterSdkInstaller.LoggedInUser != null && !AwtterSdkInstaller.LoggedInUser.Admin) return;

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.Space(25);
            var SceneFolder = serializedObject.FindProperty(nameof(AwboiSplashButtons.SceneFolder));
            EditorGUILayout.PropertyField(SceneFolder, new GUIContent(nameof(AwboiSplashButtons.SceneFolder), "This should be the path to the folder that contains the scenes listed in Scenes."));

            EditorGUILayout.Separator();
                
                
            HandleButtons();

            serializedObject.ApplyModifiedProperties();
        }

        public void HandleButtons()
        {
            EditorGUILayout.BeginVertical("Box");

            var Buttons = serializedObject.FindProperty("Buttons");
            EditorGUI.indentLevel += 1;
            EditorGUILayout.BeginHorizontal();
            ButtonsFoldout = EditorGUILayout.Foldout(ButtonsFoldout, new GUIContent($"{Buttons.arraySize} Splash Buttons: "));
            if (GUILayout.Button(new GUIContent("Add New Button")))
            {
                var splashButtons = serializedObject.targetObject as AWBOI.SplashScreen.AwboiSplashButtons;

                var button = new AWBOI.SplashScreen.SplashButton("New Button", "");
                splashButtons.Buttons.Add(button);
                ButtonsFoldout = true;
            }
            EditorGUILayout.EndHorizontal();
            

            if (Buttons.arraySize > 0)
            {
                EditorGUI.indentLevel += 1;
                if (ButtonsFoldout)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    for (int i = 0; i < Buttons.arraySize; i++)
                    {
                        var button = Buttons.GetArrayElementAtIndex(i);
                        DrawButton(Buttons, button as SerializedProperty, i);
                        GUILayout.Space(25);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel -= 1;

        }

        public void DrawButton(SerializedProperty buttons, SerializedProperty button, int index)
        {
            var allbuttons = target as AWBOI.SplashScreen.AwboiSplashButtons;
            var thisButton = allbuttons.Buttons[index];

            var bText = button.FindPropertyRelative("ButtonText");
            var bType = button.FindPropertyRelative("ButtonType");
            var link = button.FindPropertyRelative("Link");
            var image = button.FindPropertyRelative("Image");


            EditorGUILayout.PropertyField(bType);
            EditorGUILayout.PropertyField(bText);

            switch (thisButton.ButtonType)
            {
                case AWBOI.SplashScreen.SplashButton.bType.WebLink:
                    EditorGUILayout.PropertyField(link);
                    break;
                case AWBOI.SplashScreen.SplashButton.bType.File:
                    EditorGUILayout.PropertyField(link, new GUIContent("File Path"));
                    break;
            }
            EditorGUILayout.PropertyField(image);

            UIArrayHandler(buttons, index);
        }

        public void DrawName(SerializedProperty names, SerializedProperty _name, int index)
        {
            var name = _name.FindPropertyRelative("Name");
            var image = _name.FindPropertyRelative("Image");

            EditorGUILayout.PropertyField(name);
            EditorGUILayout.PropertyField(image);

            UIArrayHandler(names, index);
        }

        public void UIArrayHandler(SerializedProperty serArray, int itemIdx)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Up", GUILayout.Width(64)))
            {
                if (itemIdx > 0)
                    serArray.MoveArrayElement(itemIdx, itemIdx - 1);
            }
            if (GUILayout.Button("Down", GUILayout.Width(64)))
            {
                if (itemIdx < serArray.arraySize - 1)
                    serArray.MoveArrayElement(itemIdx, itemIdx + 1);
            }
            if (GUILayout.Button("Delete", GUILayout.Width(64)))
            {
                serArray.DeleteArrayElementAtIndex(itemIdx);
                return;
            }
            GUILayout.EndHorizontal();
        }
        #endregion
    }
}

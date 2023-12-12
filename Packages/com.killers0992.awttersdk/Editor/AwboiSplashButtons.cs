using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace AWBOI.SplashScreen
{
    [Serializable]
    [CreateAssetMenu(fileName = "AwboiSplashButtons", menuName = "ScriptableObjects/AwboiSplashButtons", order = 1)]
    public class AwboiSplashButtons : ScriptableObject
    {
        public string credPath;

        public string SceneFolder;
        public List<SplashButton> Buttons;

    }

    [Serializable]
    public class SplashButton
    {
        public bType ButtonType;
        public string ButtonText;
        public string Link;

        [Tooltip("(Optional) Icon shown in the Splash Screen.")]
        public Texture2D Image;
        

        public enum bType
        {
            WebLink,
            File
        }

        public SplashButton(string text, string link)
        {
            ButtonText = text;
            Link = link;
        }
    }
}



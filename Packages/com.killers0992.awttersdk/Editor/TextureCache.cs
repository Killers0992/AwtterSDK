using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace AwtterSDK.Editor
{
    public class TextureCache
    {
        public static TextureCache Object = new TextureCache();

        public static Texture DefaultTexture;
        public static List<string> DownloadingInProgress = new List<string>();
        public static Queue<string> TexturesToDownload = new Queue<string>();
        public static Dictionary<string, Texture> CachedTextures = new Dictionary<string, Texture>();

        public static Texture GetTextureOrDownload(string url)
        {
            if (DefaultTexture == null) DefaultTexture = EditorGUIUtility.FindTexture("Folder Icon");
            if (string.IsNullOrEmpty(url)) return DefaultTexture;

            if (!CachedTextures.ContainsKey(url))
            {
                CachedTextures.Add(url, null);
                TexturesToDownload.Enqueue(url);
            }

            return CachedTextures[url] ?? DefaultTexture;
        }

        public static void DownloadIfNeeded()
        {
            if (TexturesToDownload.Count == 0) return;

            EditorCoroutineUtility.StartCoroutine(Download(), Object);
        }

        public static IEnumerator Download()
        {
            while (TexturesToDownload.Count != 0)
            {
                var link = TexturesToDownload.Dequeue();

                if (DownloadingInProgress.Contains(link)) continue;

                DownloadingInProgress.Add(link);

                using (WebClient client = new WebClient())
                {
                    byte[] data;
                    try
                    {
                        data = client.DownloadData(link);
                    }
                    catch (WebException ex)
                    {
                        Debug.LogError($"[<color=orange>Awtter SDK</color>] Failed download icon for {link},\n{ex}");
                        continue;
                    }
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(data);

                    CachedTextures[link] = tex;
                }
            }
            yield break;
        }
    }
}

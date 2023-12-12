using AwtterSDK.Editor.Models;
using System;
using System.Data;
using System.IO;
using Unity.SharpZipLib.Zip;
using UnityEditor;

namespace AwtterSDK.Editor
{
    public class FilesCache
    {
        private static FastZip _fastZip;

        public static FastZip Zip
        {
            get
            {
                if (_fastZip == null)
                    _fastZip = new FastZip();

                return _fastZip;
            }
        }

        private static string _mainPath;
        public static string MainPath
        {
            get
            {
                if (_mainPath == null)
                {
                    _mainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Awtter SDK");
                    if (!Directory.Exists(_mainPath)) Directory.CreateDirectory(_mainPath);
                }

                return _mainPath;
            }
        }

        private static string _cachePath;
        public static string CachePath
        {
            get
            {
                if (_cachePath == null)
                {
                    _cachePath = Path.Combine(MainPath, "Cache");
                    if (!Directory.Exists(_cachePath)) Directory.CreateDirectory(_cachePath);
                }

                return _cachePath;
            }
        }

        public static string CacheFile(FileToInstallModel file, byte[] bytes)
        {
            string path = Path.Combine(CachePath, file.IsUnityPackage ? $"{file.Id}_{file.Version}" : $"{file.Name}_{file.Version}");

            File.WriteAllBytes(path, bytes);
            return path;
        }

        public static bool GetCachedFile(FileToInstallModel file)
        {
            string path = Path.Combine(CachePath, file.IsUnityPackage ? $"{file.Id}_{file.Version}" : $"{file.Name}_{file.Version}");
            if (!File.Exists(path))
                return false;

            if (file.IsUnityPackage)
                AssetDatabase.ImportPackage(path, false);
            else
                Zip.ExtractZip(path, Path.Combine(Paths.MainPath, file.Name), null);
            return true;
        }
    }
}

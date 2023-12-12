using System.IO;
using UnityEngine;

namespace AwtterSDK.Editor
{
    public class Paths
    {
        private static string _packagesPath;
        public static string PackagesPath
        {
            get
            {
                if (_packagesPath == null)
                    _packagesPath = Path.Combine(Application.dataPath, "../", "Packages");

                return _packagesPath;
            }
        }
        public static string MainPath => "Packages\\com.killers0992.awttersdk";
    }
}

namespace AwtterSDK.Editor.Models.API
{
    using System.Collections.Generic;
    using System.Linq;

    public class ProductModel
    {
        private int? _id;
        public int Id
        {
            get
            {
                if (!_id.HasValue)
                {
                    string splitId = Path.Split('/').Last();

                    if (int.TryParse(splitId, out int id))
                        _id = id;
                }

                return _id.Value;
            }
        }

        public string Path { get; set; }
        public string Name { get; set; }
        public string BaseName { get; set; }
        public bool IsBaseModel { get; set; }
        public string Icon { get; set; }

        public FileModel IsInstalled(bool findBase = false)
        {
            var vrcPackage = Files.FirstOrDefault(x => x.IsVrcUnitypackage);

            if (vrcPackage == null) return null;

            foreach(var file in Files)
            {
                string id = file.Path.Split('/')[5];

                if (!int.TryParse(id, out int fileId)) continue;

                if (AwtterSdkInstaller.InstalledPackages.BaseModel != null &&
                    AwtterSdkInstaller.InstalledPackages.BaseModel.Id == fileId && findBase)
                    return file;

                if (AwtterSdkInstaller.InstalledPackages.Dlcs.ContainsKey(fileId) && !findBase) return file;
            }

            return null;
        }

        public List<FileModel> Files { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ngClothesManager.App {
    public class Project : INotifyPropertyChanged {

        public readonly string FolderPath;

        public readonly string Name;

        private ObservableCollection<Cloth> _clothes = new ObservableCollection<Cloth>();

        private readonly List<string> filesToDelete = new List<string>();
        private readonly List<string> directoriesToDelete = new List<string>();

        public ObservableCollection<Cloth> Clothes {
            get {
                return _clothes;
            }
            set {
                _clothes = value;
                OnPropertyChanged(nameof(Clothes));
            }
        }

        private bool _needsSaving;

        public bool NeedsSaving {
            get {
                return _needsSaving;
            }
            set {
                _needsSaving = value;
                OnPropertyChanged(nameof(NeedsSaving));
            }
        }

        private string ProjectFilePath {
            get {
                return FolderPath + "/" + Name + ".ngcmp";
            }
        }

        private void OnPropertyChanged(string memberName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }

        private Project(string name, string folderPath) {
            this.Name = name;
            this.FolderPath = folderPath;

            using(FileStream fileStream = File.OpenRead(ProjectFilePath)) {
                using(StreamReader reader = new StreamReader(fileStream)) {
                    string clothDatasText = reader.ReadToEnd();

                    if(clothDatasText?.Length > 0) {
                        List<Cloth> list = JsonConvert.DeserializeObject<List<Cloth>>(clothDatasText, new JsonSerializerSettings() {
                            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                        });

                        Clothes = new ObservableCollection<Cloth>(list);
                    }
                }
            }
        }

        public void Save() {
            foreach(string relPath in filesToDelete) {
                File.Delete(FolderPath + "/" + relPath);
            }
            filesToDelete.Clear();

            foreach(string relPath in directoriesToDelete) {
                Directory.Delete(FolderPath + "/" + relPath, true);
            }
            directoriesToDelete.Clear();

            string text = JsonConvert.SerializeObject(Clothes);

            using(FileStream fileStream = File.Open(ProjectFilePath, FileMode.Create)) {
                using(StreamWriter writer = new StreamWriter(fileStream)) {
                    writer.Write(text);
                }
            }

            NeedsSaving = false;
            Logger.Log("Project saved.");
        }

        public static Project Create(string name, string path) {
            string projectFolder = path + "/" + name;
            string projectFile = projectFolder + "/" + name + ".ngcmp";
            Directory.CreateDirectory(projectFolder);
            File.Create(projectFile).Dispose();

            Project project = Open(projectFile);
            Logger.Log("Project created.");

            return project;
        }

        public static Project Open(string filePath) {
            FileInfo info = new FileInfo(filePath);
            string name = Path.GetFileNameWithoutExtension(info.Name);
            Project project = new Project(name, info.Directory.FullName);
            Logger.Log("Project loaded. Total clothes: " + project.Clothes.Count);

            return project;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int GetEmptyClothId() {
            int id = 0;
            while(true) {
                if(Clothes.FirstOrDefault(cloth => cloth.Index == id) == null) {
                    return id;
                }
                id++;
            }
        }

        public void AddFile(string sourcePath, string relTargetPath) {
            NeedsSaving = true;
            string targetPath = FolderPath + "/" + relTargetPath;
            FileInfo fileInfo = new FileInfo(targetPath);
            fileInfo.Directory.Create(); // create all needed folders
            File.Copy(sourcePath, targetPath, true);
        }

        public void RemoveFile(string relPath) {
            NeedsSaving = true;
            filesToDelete.Add(relPath);
        }

        public bool FileExists(string relPath) {
            return File.Exists(FolderPath + "/" + relPath);
        }

        public void RemoveCloth(Cloth cloth) {
            NeedsSaving = true;
            Clothes.Remove(cloth);

            directoriesToDelete.Add(cloth.DrawableType.ToIdentifier() + "/" + cloth.Index);
            Logger.Log("Removed " + cloth.Name + ". Total clothes: " + Clothes.Count);
        }

        public void RemoveTexture(Cloth cloth, Texture texture) {
            NeedsSaving = true;
            cloth.Textures.Remove(texture);
            string texPath = cloth.GetTexturePath(texture.Index);
            RemoveFile(texPath);

            Logger.Log("Removed " + texture.Name + " from " + cloth.Name + ". Textures Count: " + cloth.Textures.Count);
        }
    }
}
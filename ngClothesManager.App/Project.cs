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

        private readonly List<string> filesToDelete = new List<string>();
        private readonly List<string> directoriesToDelete = new List<string>();

        public ObservableCollection<Drawable> Drawables { get; } = new ObservableCollection<Drawable>();

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

        private void OnPropertyChanged(string propertyName) {
            //Logger.Log("PropertyChanged: Project." + propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Project(string name, string folderPath) {
            Drawables = new ObservableCollection<Drawable>();
            this.Name = name;
            this.FolderPath = folderPath;

            using(FileStream fileStream = File.OpenRead(ProjectFilePath)) {
                using(StreamReader reader = new StreamReader(fileStream)) {
                    string drawablesStr = reader.ReadToEnd();

                    if(drawablesStr?.Length > 0) {
                        List<Drawable> list = JsonConvert.DeserializeObject<List<Drawable>>(drawablesStr, new JsonSerializerSettings() {
                            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                        });

                        Drawables.Clear();
                        foreach(Drawable drawable in list) {
                            Drawables.Add(drawable);
                        }
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

            string text = JsonConvert.SerializeObject(Drawables);

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
            Logger.Log("Project loaded. Total drawables: " + project.Drawables.Count);

            return project;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int GetEmptyDrawableId() {
            int id = 0;
            while(true) {
                if(Drawables.FirstOrDefault(drawable => drawable.Id == id) == null) {
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

        public void AddFileFromByteArray(byte[] bytes, string relTargetPath) {
            NeedsSaving = true;
            string targetPath = FolderPath + "/" + relTargetPath;
            FileInfo fileInfo = new FileInfo(targetPath);
            fileInfo.Directory.Create(); // create all needed folders
            File.WriteAllBytes(FolderPath + "/" + relTargetPath, bytes);
        }

        public void RemoveFile(string relPath) {
            NeedsSaving = true;
            filesToDelete.Add(relPath);
        }

        public bool FileExists(string relPath) {
            return File.Exists(FolderPath + "/" + relPath);
        }

        public void RemoveDrawable(Drawable drawable) {
            NeedsSaving = true;
            Drawables.Remove(drawable);

            directoriesToDelete.Add(drawable.DrawableType.ToIdentifier() + "/" + drawable.Id);
            Logger.Log("Removed " + drawable.Name + ". Total drawables: " + Drawables.Count);
        }

        public void RemoveTexture(Drawable drawable, Texture texture) {
            NeedsSaving = true;
            drawable.Textures.Remove(texture);
            string texPath = drawable.GetTexturePath(texture.Id);
            RemoveFile(texPath);

            Logger.Log("Removed " + texture.Name + " from " + drawable.Name + ". Textures Count: " + drawable.Textures.Count);
        }
    }
}
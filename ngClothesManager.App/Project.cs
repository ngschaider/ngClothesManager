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
    public class Project : IDisposable, INotifyPropertyChanged {

        private readonly FileStream fileStream;
        private readonly ZipArchive zipArchive;

        private ObservableCollection<Cloth> _clothes = new ObservableCollection<Cloth>();

        public ObservableCollection<Cloth> Clothes {
            get {
                return _clothes;
            }
            set {
                _clothes = value;
                OnPropertyChanged(nameof(Clothes));
            }
        }

        public Stream OpenEntry(string entryName) {
            return zipArchive.GetEntry(entryName).Open();
        }

        private void OnPropertyChanged(string memberName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }

        private Project(string filePath) {
            fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.None);
            //fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Update, false, Encoding.UTF8);

            ZipArchiveEntry entry = zipArchive.GetEntry("data.json");
            if(entry == null) {
                entry = zipArchive.CreateEntry("data.json");
            }

            using(Stream stream = entry.Open()) {
                using(StreamReader reader = new StreamReader(stream)) {
                    string clothDatasText = reader.ReadToEnd();

                    List<Cloth> list = JsonConvert.DeserializeObject<List<Cloth>>(clothDatasText, new JsonSerializerSettings() {
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    });

                    if(list != null) {
                        Clothes = new ObservableCollection<Cloth>(list);
                    } else {
                        Clothes = new ObservableCollection<Cloth>();
                    }
                }
            }
        }

        public void Save() {
            string text = JsonConvert.SerializeObject(Clothes);

            using(Stream stream = zipArchive.GetEntry("data.json").Open()) {
                using(StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(text);
                }
            }

            Logger.Log("Project saved.");
        }

        public static Project Create(string filePath) {
            Project project = new Project(filePath);
            Logger.Log("Project created.");

            return project;
        }

        public static Project Open(string filePath) {
            Project project = new Project(filePath);
            Logger.Log("Project loaded. Total clothes: " + project.Clothes.Count);

            return project;
        }

        public void Dispose() {
            zipArchive.Dispose();
            fileStream.Dispose();
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

        public void AddFile(string sourcePath, string targetPath) {
            if(zipArchive.Entries.FirstOrDefault(entry => entry.FullName == targetPath) != null) {
                return;
            }

            Logger.Log("Adding file " + sourcePath + " to " + targetPath);
            using(Stream stream = zipArchive.CreateEntry(targetPath).Open()) {
                using(FileStream fileStream = File.OpenRead(sourcePath)) {
                    fileStream.CopyTo(stream);
                }
            }
        }

        public void RemoveFile(string path) {
            ZipArchiveEntry entry = zipArchive.Entries.FirstOrDefault(e => e.FullName == path);
            if(entry == null) {
                return;
            }

            entry.Delete();
        }

        public void RemoveCloth(Cloth cloth) {
            Clothes.Remove(cloth);

            ZipArchiveEntry[] entries = zipArchive.Entries.Where(entry => entry.FullName.StartsWith(cloth.DrawableType.ToIdentifier() + "/" + cloth.Index + "/")).ToArray();
            foreach(ZipArchiveEntry entry in entries) {
                entry.Delete();
            }

            Logger.Log("Removed " + cloth.Name + ". Total clothes: " + Clothes.Count);
        }

        public void RemoveTexture(Cloth cloth, Texture texture) {
            cloth.Textures.Remove(texture);
            string texPath = cloth.GetTexturePath(texture.Index);
            RemoveFile(texPath);

            Logger.Log("Removed " + texture.Name + " from " + cloth.Name + ". Textures Count: " + cloth.Textures.Count);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngClothesManager.App {
    class ClothImporter {

        private readonly Project project;

        public ClothImporter(Project project) {
            this.project = project;
        }

        public void Import(string filePath, Sex sex) {
            string fileName = Path.GetFileName(filePath);

            string baseName = Path.GetFileNameWithoutExtension(filePath);

            // FiveM Naming Convention
            if(baseName.Contains("^")) {
                baseName = baseName.Split('^')[1];
            }

            string[] parts = baseName.Split('_');

            if(parts.Length < 3) {
                throw new Exception("Wrong drawable name");
            }

            ClothType clothType = parts[0].ToLower() == "p" ? ClothType.Prop : ClothType.Component;

            string drawableTypeIdentifier = clothType == ClothType.Component ? parts[0] : parts[0] + "_" + parts[1];
            DrawableType drawableType = drawableTypeIdentifier.ToDrawableType();

            int originalNumber = clothType == ClothType.Component ? Convert.ToInt32(parts[1]) : Convert.ToInt32(parts[2]);

            string suffix = string.Join("_", parts.Skip(clothType == ClothType.Component ? 2 : 3));

            bool isVariation = parts.Length > 3;

            Cloth cloth = new Cloth(project.GetEmptyClothId(), clothType, drawableType, sex, suffix);

            if(isVariation) {
                Logger.Log("Item " + fileName + " can't be added. Looks like it's variant of another item");
                return;
            }

            project.Clothes.Add(cloth);
            project.AddFile(filePath, cloth.DrawableType.ToIdentifier() + "/" + cloth.Index + "/model.ydd");


            List<string> texturePaths = SearchForTextures(filePath, originalNumber, cloth);

            TextureImporter importer = new TextureImporter(project, cloth);
            foreach(string texturePath in texturePaths) {
                importer.Import(texturePath);
            }

            /*if(cloth.IsComponent) {
                string fpModelPath = SearchForFirstPersonModel(filePath);
                if(fpModelPath.Length > 0) {
                    project.AddFile(fpModelPath, drawableType.ToIdentifier() + "/" + cloth.Index + "/model_fp.ydd");
                }
            }*/

            if(cloth.IsComponent) {
                //string fpModelAddition = (!string.IsNullOrEmpty(cloth.FirstPersonModelPath) ? "FP Model found, " : "");
                //Logger.Log(cloth + " added (" + fpModelAddition + "Found " + cloth.Textures.Count + " textures). Total clothes: " + project.Clothes.Count);
                Logger.Log(cloth + " added. (Found " + cloth.Textures.Count + " textures). Total clothes: " + project.Clothes.Count);
            } else {
                Logger.Log(cloth + " added. (Found " + cloth.Textures.Count + " textures). Total clothes: " + project.Clothes.Count);
            }
        }

        /*private string SearchForFirstPersonModel(string filePath) {
            string rootPath = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string relPath = rootPath + "\\" + fileName + "_1.ydd";
            return File.Exists(relPath) ? relPath : "";
        }*/

        public List<string> SearchForTextures(string filePath, int offsetNumber, Cloth cloth) {
            List<string> texturePaths = new List<string>();

            string rootPath = Path.GetDirectoryName(filePath);

            DirectoryInfo dir = new DirectoryInfo(rootPath);

            string paddedNumber = offsetNumber.ToString().PadLeft(3, '0');

            string searchPattern = "*" + cloth.DrawableType.ToIdentifier() + "_diff_" + paddedNumber + "_*.ytd";
            Logger.Log("Seraching textures using search pattern" + searchPattern);
            foreach(FileInfo file in dir.GetFiles(searchPattern)) {
                texturePaths.Add(file.FullName);
            }

            return texturePaths;
        }

    }
}

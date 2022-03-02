using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngClothesManager.App {
    class DrawableImporter {

        private readonly Project project;

        public DrawableImporter(Project project) {
            this.project = project;
        }

        public void Import(string filePath) {
            string fileName = Path.GetFileName(filePath);

            string baseName = Path.GetFileNameWithoutExtension(filePath);

            foreach(Gender gender in Enum.GetValues(typeof(Gender))) {
                if(baseName.StartsWith(gender.ToPrefix())) {
                    Import(filePath, gender);
                    return;
                }
            }

            throw new Exception("Could not detect gender from filename " + baseName);
        }

        public void Import(string filePath, Gender gender) {
            string fileName = Path.GetFileName(filePath);

            string baseName = Path.GetFileNameWithoutExtension(filePath);

            // FiveM Naming Convention
            string prefix = baseName.Contains("^") ? baseName.Split('^')[0] + "^" : "";
            baseName = baseName.Contains("^") ? baseName.Split('^')[1] : baseName;

            string[] parts = baseName.Split('_');

            if(parts.Length < 3) {
                throw new Exception("Wrong drawable name");
            }

            string drawableTypeIdentifier = parts[0].ToLower() == "p" ? parts[0] + "_" + parts[1] : parts[0];
            if(!drawableTypeIdentifier.ToDrawableType(out DrawableType drawableType)) {
                Logger.Log("Could not parse drawable type " + drawableTypeIdentifier + " - skipping");
                return;
            }

            string numberToParse = drawableType.IsComponent() ? parts[1] : parts[2];
            if(!Int32.TryParse(numberToParse, out int originalNumber)) {
                Logger.Log("Could not parse number " + numberToParse + " - skipping");
                return;
            }

            string suffix = string.Join("_", parts.Skip(drawableType.IsComponent() ? 2 : 3));

            bool isVariation = parts.Length > 3;

            Drawable drawable = new Drawable(project.GetEmptyDrawableId(), drawableType, gender, suffix);

            if(isVariation) {
                Logger.Log("Item " + fileName + " can't be added. Looks like it's variant of another item");
                return;
            }

            project.Drawables.Add(drawable);
            project.AddFile(filePath, drawable.DrawableType.ToIdentifier() + "/" + drawable.Id + "/model.ydd");


            List<string> texturePaths = SearchForTextures(filePath, originalNumber, prefix, drawable);

            TextureImporter importer = new TextureImporter(project, drawable);
            foreach(string texturePath in texturePaths) {
                importer.Import(texturePath);
            }

            Logger.Log(drawable.DisplayName + " added. (Found " + drawable.Textures.Count + " textures). Total drawables: " + project.Drawables.Count);
        }

        public List<string> SearchForTextures(string filePath, int offsetNumber, string prefix, Drawable drawable) {
            List<string> texturePaths = new List<string>();

            string rootPath = Path.GetDirectoryName(filePath);

            DirectoryInfo dir = new DirectoryInfo(rootPath);

            string paddedNumber = offsetNumber.ToString().PadLeft(3, '0');

            string searchPattern = prefix + drawable.DrawableType.ToIdentifier() + "_diff_" + paddedNumber + "_*.ytd";
            Logger.Log("Seraching textures using search pattern " + searchPattern);
            foreach(FileInfo file in dir.GetFiles(searchPattern)) {
                texturePaths.Add(file.FullName);
            }

            return texturePaths;
        }

    }
}

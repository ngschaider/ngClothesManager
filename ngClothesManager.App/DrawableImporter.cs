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

        public void Import(string filePath, Sex sex) {
            
            string fileName = Path.GetFileName(filePath);

            string baseName = Path.GetFileNameWithoutExtension(filePath);

            // FiveM Naming Convention
            string prefix = baseName.Contains("^") ? baseName.Split('^')[0] + "^" : "";
            baseName = baseName.Contains("^") ? baseName.Split('^')[1] : baseName;

            string[] parts = baseName.Split('_');

            if(parts.Length < 3) {
                throw new Exception("Wrong drawable name");
            }

            string drawableTypeIdentifier = parts[0].ToLower() == "p" ? parts[0] : parts[0] + "_" + parts[1];
            DrawableType drawableType = drawableTypeIdentifier.ToDrawableType();

            int originalNumber = drawableType.IsComponent() ? Convert.ToInt32(parts[1]) : Convert.ToInt32(parts[2]);

            string suffix = string.Join("_", parts.Skip(drawableType.IsComponent() ? 2 : 3));

            bool isVariation = parts.Length > 3;

            Drawable drawable = new Drawable(project.GetEmptyDrawableId(), drawableType, sex, suffix);

            if(isVariation) {
                Logger.Log("Item " + fileName + " can't be added. Looks like it's variant of another item");
                return;
            }

            project.Drawables.Add(drawable);
            project.AddFile(filePath, drawable.DrawableType.ToIdentifier() + "/" + drawable.Index + "/model.ydd");


            List<string> texturePaths = SearchForTextures(filePath, originalNumber, prefix, drawable);

            TextureImporter importer = new TextureImporter(project, drawable);
            foreach(string texturePath in texturePaths) {
                importer.Import(texturePath);
            }

            Logger.Log(drawable + " added. (Found " + drawable.Textures.Count + " textures). Total drawables: " + project.Drawables.Count);
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

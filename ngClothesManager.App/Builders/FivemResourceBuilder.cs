using System.Collections.Generic;
using System.IO;
using ngClothesManager.App.Builders.Base;

namespace ngClothesManager.App.Builders {
    internal class FivemResourceBuilder : MultiplayerResourceBuilderBase {

        public FivemResourceBuilder(Project project, string outputFolder): base(project, outputFolder) {

        }

        private readonly List<string> resourceLuaMetas = new List<string>();

        #region Resource Props 

        protected override void OnFirstPropAddedToResource(Sex sex) {
            string pedName = sex.ToPrefix() + "freemode_01_p_" + sex.ToPrefix() + OutputName;
            Directory.CreateDirectory(outputFolder + "/stream");
            Directory.CreateDirectory(outputFolder + "/stream/" + pedName);
        }

        protected override void CopyPropTextureToResource(Drawable drawable, Texture texture, Sex sex, string componentNumerics, char offsetLetter) {
            string pedName = sex.ToPrefix() + "freemode_01_p_" + sex.ToPrefix() + OutputName;
            string targetFilePath = outputFolder + "/stream/" + pedName + "/" + pedName + "^" + drawable.Prefix + "_diff_" + componentNumerics + "_" + offsetLetter + ".ytd";
            File.Copy(project.FolderPath + "/" + drawable.GetTexturePath(texture.Index), targetFilePath, true);
        }

        protected override void CopyPropModelToResource(Drawable drawable, Sex sex, string componentNumerics) {
            string pedName = sex.ToPrefix() + "freemode_01_p_" + sex.ToPrefix() + OutputName;
            string targetFilePath = outputFolder + "/stream/" + pedName + "/" + sex.ToPrefix() + "freemode_01_p_" + sex.ToPrefix() + OutputName + "^" + drawable.Prefix + "_" + componentNumerics + ".ydd";
            File.Copy(project.FolderPath + "/" + drawable.ModelPath, targetFilePath, true);
        }

        #endregion

        #region Resource Drawables

        protected override string GetDrawableYmtFilePath(Sex sex) {
            return outputFolder + "/stream/" + sex.ToPrefix() + "freemode_01_" + sex.ToPrefix() + OutputName + ".ymt";
        }

        protected override void OnFirstDrawableAddedToResource(Sex sex) {
            Directory.CreateDirectory(outputFolder + "/stream");
            string pedName = sex.ToPrefix() + "freemode_01_" + sex.ToPrefix() + OutputName;
            Directory.CreateDirectory(outputFolder + "/stream/" + pedName);
        }

        protected override void CopyDrawableTextureToResource(Drawable drawable, Texture texture, Sex sex, string componentNumerics, string ytdSuffix, char offsetLetter) {
            string pedName = sex.ToPrefix() + "freemode_01_" + sex.ToPrefix() + OutputName;
            string targetFilePath = outputFolder + "/stream/" + pedName + "/" + pedName + "^" + drawable.Prefix + "_diff_" + componentNumerics + "_" + offsetLetter + "_" + ytdSuffix + ".ytd";
            File.Copy(project.FolderPath + "/" + drawable.GetTexturePath(texture.Index), targetFilePath, true);
        }

        protected override void CopyDrawableModelToResource(Drawable drawable, Sex sex, string componentNumerics, string yddSuffix) {
            string pedName = sex.ToPrefix() + "freemode_01_" + sex.ToPrefix() + OutputName;
            string targetFilePath = outputFolder + "/stream/" + pedName + "/" + pedName + "^" + drawable.Prefix + "_" + componentNumerics + "_" + yddSuffix + ".ydd";
            File.Copy(project.FolderPath + "/" + drawable.ModelPath, targetFilePath, true);
        }

        #endregion

        protected override void OnResourceBuildingFinished() {
            File.WriteAllText(outputFolder + "/fxmanifest.lua", GetFxmanifestContent(resourceLuaMetas));
        }

        protected override void OnResourceDrawableDataFinished(Sex sex, bool isAnyComponentAdded, bool isAnyPropAdded) {
            if(!isAnyPropAdded && !isAnyComponentAdded) {
                return;
            }

            string shopMetaFilePath = outputFolder + "/" + sex.ToPrefix() + "freemode_01_" + sex.ToPrefix() + OutputName + ".meta";
            File.WriteAllText(shopMetaFilePath, GetShopMetaContent(sex));

            resourceLuaMetas.Add(sex.ToPrefix() + "freemode_01_" + sex.ToPrefix() + OutputName + ".meta");
        }

        private string GetFxmanifestContent(List<string> metas) {
            string manifestContent = "-- Generated with ngClothesManager\n\n";
            manifestContent += "fx_version 'cerulean'\n";
            manifestContent += "game 'gta5'\n\n";

            string filesText = "";
            for(int i = 0; i < metas.Count; ++i) {
                if(i != 0)
                    filesText += ",\n";
                filesText += "  '" + metas[i] + "'";
            }
            manifestContent += "files {\n" + filesText + "\n}\n\n";

            string metasText = "";
            for(int i = 0; i < metas.Count; ++i) {
                if(i != 0) {
                    metasText += "\n";
                }
                metasText += "data_file 'SHOP_PED_APPAREL_META_FILE' '" + metas[i] + "'";
            }
            manifestContent += metasText;

            return manifestContent;
        }
    }
}

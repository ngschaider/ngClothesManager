using ngClothesManager.App.Builders.Base;
using System;
using System.Collections.Generic;
using System.IO;

namespace ngClothesManager.App.Builders {
    internal class AltvResourceBuilder : MultiplayerResourceBuilderBase {
        private readonly List<string> _streamCfgMetas = new List<string>();
        private readonly List<string> _streamCfgIncludes = new List<string>();

        public AltvResourceBuilder(Project project, string outputFolder) : base(project, outputFolder) {

        }

        #region Resource Props

        protected override void OnFirstPropAddedToResource(Gender gender) {
            Directory.CreateDirectory(outputFolder + "/stream");
            Directory.CreateDirectory(outputFolder + "/stream/ped_" + gender.ToString() + "_p.rpf");
            Directory.CreateDirectory(outputFolder + "/stream/ped_" + gender.ToString() + "_p.rpf/" + gender.ToPrefix() + "freemode_01_p_" + gender.ToPrefix() + OutputName);
        }

        protected override void CopyPropTextureToResource(Drawable drawable, Texture texture, Gender gender, string componentNumerics, char offsetLetter) {
            string pedName = gender.ToPrefix() + "freemode_01_p_" + gender.ToPrefix() + OutputName;
            string targetFilePath = outputFolder + "/stream/ped_" + gender.ToString() + "_p.rpf/" + pedName + "/" + drawable.Prefix + "_diff_" + componentNumerics + "_" + offsetLetter + ".ytd";
            File.Copy(project.FolderPath + "/" + drawable.GetTexturePath(texture.Id), targetFilePath, true);
        }

        protected override void CopyPropModelToResource(Drawable drawable, Gender gender, string componentNumerics) {
            string pedName = gender.ToPrefix() + "freemode_01_p_" + gender.ToPrefix() + OutputName;
            string targetFilePath = outputFolder + "/stream/ped_" + gender.ToString() + "_p.rpf/" + pedName + "/" + drawable.Prefix + "_" + componentNumerics + ".ydd";
            File.Copy(project.FolderPath + "/" + drawable.ModelPath, targetFilePath, true);
        }

        #endregion

        #region Resource Drawables

        protected override string GetDrawableYmtFilePath(Gender gender) {
            return $"{outputFolder}/stream/ped_{gender}.rpf/{gender.ToPrefix()}freemode_01_{gender.ToPrefix()}{OutputName}.ymt";
        }

        protected override void OnFirstDrawableAddedToResource(Gender gender) {
            Directory.CreateDirectory($"{outputFolder}/stream");
            Directory.CreateDirectory($"{outputFolder}/stream/ped_{gender}.rpf");
            Directory.CreateDirectory($"{outputFolder}/stream/ped_{gender}.rpf/{gender.ToPrefix()}freemode_01_{gender.ToPrefix()}{OutputName}");
        }

        protected override void CopyDrawableTextureToResource(Drawable drawable, Texture texture, Gender gender, string componentNumerics, string ytdSuffix, char offsetLetter) {
            string targetFilePath = $"{outputFolder}/stream/ped_{gender}.rpf/{gender.ToPrefix()}freemode_01_{gender.ToPrefix()}{OutputName}/{drawable.Prefix}_diff_{componentNumerics}_{offsetLetter}_{ytdSuffix}.ytd";
            File.Copy(project.FolderPath + "/" + drawable.GetTexturePath(texture.Id), targetFilePath);
        }

        protected override void CopyDrawableModelToResource(Drawable drawable, Gender gender, string componentNumerics, string yddSuffix) {
            string targetFilePath = $"{outputFolder}/stream/ped_{gender}.rpf/{gender.ToPrefix()}freemode_01_{gender.ToPrefix()}{OutputName}/{drawable.Prefix}_{componentNumerics}_{yddSuffix}.ydd";
            File.Copy(project.FolderPath + "/" + drawable.ModelPath, targetFilePath);
        }

        #endregion

        protected override void OnResourceDrawableDataFinished(Gender gender, bool isAnyComponentAdded, bool isAnyPropAdded) {
            if(isAnyPropAdded) {
                _streamCfgIncludes.Add($"stream/ped_{gender}_p.rpf/*");
            }

            if(isAnyComponentAdded || isAnyPropAdded) {
                string shopMetaFilePath = outputFolder + "/stream/" + gender.ToPrefix() + "freemode_01_" + gender.ToPrefix() + OutputName + ".meta";
                File.WriteAllText(shopMetaFilePath, GetShopMetaContent(gender));

                _streamCfgMetas.Add($"stream/{gender.ToPrefix()}freemode_01_{gender.ToPrefix()}{OutputName}.meta: SHOP_PED_APPAREL_META_FILE");
                _streamCfgIncludes.Add($"stream/ped_{gender}.rpf/*");
            }
        }

        protected override void OnResourceBuildingFinished() {
            File.WriteAllText(outputFolder + "/stream.cfg", GenerateAltvStreamCfgContent(_streamCfgIncludes, _streamCfgMetas));
            File.WriteAllText(outputFolder + "/resource.cfg", GenerateAltvResourceCfgContent());
        }

        private string GenerateAltvStreamCfgContent(List<string> files, List<string> metas) {
            string filesText = "";
            for(int i = 0; i < files.Count; ++i) {
                if(i != 0)
                    filesText += "\n";
                filesText += "  " + files[i];
            }

            string metasText = "";
            for(int i = 0; i < metas.Count; ++i) {
                if(i != 0)
                    metasText += "\n";
                metasText += "  " + metas[i];
            }

            return $"files: [\n{filesText}\n]\nmeta: {{\n{metasText}\n}}\n";
        }

        private string GenerateAltvResourceCfgContent() {
            return "type: dlc,\nmain: stream.cfg,\nclient-files: [\n  stream/*\n]\n";
        }
    }
}

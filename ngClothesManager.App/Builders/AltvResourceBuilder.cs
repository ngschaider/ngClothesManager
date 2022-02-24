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

        protected override void OnFirstPropAddedToResource(Sex sex) {
            Directory.CreateDirectory(outputFolder + "/stream");
            Directory.CreateDirectory(outputFolder + "/stream/ped_" + sex.ToString() + "_p.rpf");
            Directory.CreateDirectory(outputFolder + "/stream/ped_" + sex.ToString() + "_p.rpf/" + sex.ToPrefix() + "freemode_01_p_" + sex.ToPrefix() + OutputName);
        }

        protected override void CopyPropTextureToResource(Drawable drawable, Texture texture, Sex sex, string componentNumerics, char offsetLetter) {
            string pedName = sex.ToPrefix() + "freemode_01_p_" + sex.ToPrefix() + OutputName;
            string targetFilePath = outputFolder + "/stream/ped_" + sex.ToString() + "_p.rpf/" + pedName + "/" + drawable.Prefix + "_diff_" + componentNumerics + "_" + offsetLetter + ".ytd";
            File.Copy(project.FolderPath + "/" + drawable.GetTexturePath(texture.Index), targetFilePath, true);
        }

        protected override void CopyPropModelToResource(Drawable drawable, Sex sex, string componentNumerics) {
            string pedName = sex.ToPrefix() + "freemode_01_p_" + sex.ToPrefix() + OutputName;
            string targetFilePath = outputFolder + "/stream/ped_" + sex.ToString() + "_p.rpf/" + pedName + "/" + drawable.Prefix + "_" + componentNumerics + ".ydd";
            File.Copy(project.FolderPath + "/" + drawable.ModelPath, targetFilePath, true);
        }

        #endregion

        #region Resource Drawables

        protected override string GetDrawableYmtFilePath(Sex sex) {
            return $"{outputFolder}/stream/ped_{sex}.rpf/{sex.ToPrefix()}freemode_01_{sex.ToPrefix()}{OutputName}.ymt";
        }

        protected override void OnFirstDrawableAddedToResource(Sex sex) {
            Directory.CreateDirectory($"{outputFolder}/stream");
            Directory.CreateDirectory($"{outputFolder}/stream/ped_{sex}.rpf");
            Directory.CreateDirectory($"{outputFolder}/stream/ped_{sex}.rpf/{sex.ToPrefix()}freemode_01_{sex.ToPrefix()}{OutputName}");
        }

        protected override void CopyDrawableTextureToResource(Drawable drawable, Texture texture, Sex sex, string componentNumerics, string ytdSuffix, char offsetLetter) {
            string targetFilePath = $"{outputFolder}/stream/ped_{sex}.rpf/{sex.ToPrefix()}freemode_01_{sex.ToPrefix()}{OutputName}/{drawable.Prefix}_diff_{componentNumerics}_{offsetLetter}_{ytdSuffix}.ytd";
            File.Copy(project.FolderPath + "/" + drawable.GetTexturePath(texture.Index), targetFilePath);
        }

        protected override void CopyDrawableModelToResource(Drawable drawable, Sex sex, string componentNumerics, string yddSuffix) {
            string targetFilePath = $"{outputFolder}/stream/ped_{sex}.rpf/{sex.ToPrefix()}freemode_01_{sex.ToPrefix()}{OutputName}/{drawable.Prefix}_{componentNumerics}_{yddSuffix}.ydd";
            File.Copy(project.FolderPath + "/" + drawable.ModelPath, targetFilePath);
        }

        #endregion

        protected override void OnResourceDrawableDataFinished(Sex sex, bool isAnyComponentAdded, bool isAnyPropAdded) {
            if(isAnyPropAdded) {
                _streamCfgIncludes.Add($"stream/ped_{sex}_p.rpf/*");
            }

            if(isAnyComponentAdded || isAnyPropAdded) {
                string shopMetaFilePath = outputFolder + "/stream/" + sex.ToPrefix() + "freemode_01_" + sex.ToPrefix() + OutputName + ".meta";
                File.WriteAllText(shopMetaFilePath, GetShopMetaContent(sex));

                _streamCfgMetas.Add($"stream/{sex.ToPrefix()}freemode_01_{sex.ToPrefix()}{OutputName}.meta: SHOP_PED_APPAREL_META_FILE");
                _streamCfgIncludes.Add($"stream/ped_{sex}.rpf/*");
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

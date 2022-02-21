using System.Collections.Generic;
using System.Linq;
using RageLib.GTA5.ResourceWrappers.PC.Meta.Structures;
using RageLib.Resources.GTA5.PC.GameFiles;
using RageLib.Resources.GTA5.PC.Meta;

namespace ngClothesManager.App.Builders.Base {
    internal abstract class MultiplayerResourceBuilderBase : ResourceBuilderBase {

        public MultiplayerResourceBuilderBase(Project project, string outputFolder) : base(project, outputFolder) {

        }

        public override void BuildResource() {
            OnResourceBuildingStarted();

            foreach(Sex sex in new Sex[] { Sex.Male, Sex.Female }) {
                string ymtName = sex.ToPrefix() + OutputName;
                YmtPedDefinitionFile ymt = CreateYmtPedDefinitionFile(ymtName, out var componentTextureBindings, out int[] componentIndexes, out int[] propIndexes);

                bool isAnyComponentAdded = false;
                bool isAnyPropAdded = false;

                foreach(Cloth cloth in project.Clothes.Where(cloth => cloth.TargetSex == sex)) {
                    if(cloth.IsComponent) {
                        if(cloth.Textures.Count <= 0) {
                            continue;
                        }
                            
                        MCComponentInfo componentItemInfo = GenerateYmtPedComponentItem(cloth, ref componentTextureBindings);
                        ymt.Unk_376833625.CompInfos.Add(componentItemInfo);

                        var componentTypeId = componentItemInfo.Unk_3509540765;
                        GetClothSuffixes(cloth, out var ytdSuffix, out var yddSuffix);

                        if(!isAnyComponentAdded) {
                            isAnyComponentAdded = true;
                            OnFirstClothAddedToResource(sex);
                        }

                        int currentComponentIndex = componentIndexes[componentTypeId]++;

                        string componentNumerics = currentComponentIndex.ToString().PadLeft(3, '0');

                        CopyClothModelToResource(cloth, sex, componentNumerics, yddSuffix);

                        foreach(Texture texture in cloth.Textures) {
                            CopyClothTextureToResource(cloth, texture, sex, componentNumerics, ytdSuffix, Utils.NumberToLetter(texture.Index));
                        }
                    } else {
                        if(cloth.Textures.Count <= 0) {
                            continue;
                        }

                        Unk_2834549053 anchor = (Unk_2834549053)cloth.PedPropTypeId;
                        List<MUnk_94549140> defs = ymt.Unk_376833625.PropInfo.Props[anchor] ?? new List<MUnk_94549140>();
                        MUnk_94549140 item = GenerateYmtPedPropItem(ymt, anchor, cloth);
                        defs.Add(item);

                        if(!isAnyPropAdded) {
                            isAnyPropAdded = true;
                            OnFirstPropAddedToResource(sex);
                        }

                        int currentPropIndex = propIndexes[(byte)anchor]++;
                        string componentNumerics = currentPropIndex.ToString().PadLeft(3, '0');

                        CopyPropModelToResource(cloth, sex, componentNumerics);

                        foreach(Texture texture in cloth.Textures) {
                            CopyPropTextureToResource(cloth, texture, sex, componentNumerics, Utils.NumberToLetter(texture.Index));
                        }
                    }
                }

                if(isAnyComponentAdded) {
                    UpdateYmtComponentTextureBindings(componentTextureBindings, ymt);
                    string clothYmtFilePath = GetClothYmtFilePath(sex);
                    ymt.Save(clothYmtFilePath);
                }

                OnResourceClothDataFinished(sex, isAnyComponentAdded, isAnyPropAdded);
            }

            OnResourceBuildingFinished();
        }

        protected abstract string GetClothYmtFilePath(Sex sex);

        protected abstract void CopyPropTextureToResource(Cloth cloth, Texture texture, Sex sex, string componentNumerics, char offsetLetter);

        protected abstract void CopyPropModelToResource(Cloth cloth, Sex sex, string componentNumerics);

        protected abstract void CopyClothTextureToResource(Cloth cloth, Texture texture, Sex sex, string componentNumerics, string ytdSuffix, char offsetLetter);

        protected abstract void CopyClothModelToResource(Cloth cloth, Sex sex, string componentNumerics, string yddSuffix);

        protected virtual void OnResourceClothDataFinished(Sex sex, bool isAnyClothAdded, bool isAnyPropAdded) {

        }

        protected virtual void OnFirstPropAddedToResource(Sex sex) {

        }

        protected virtual void OnFirstClothAddedToResource(Sex sex) {

        }

        protected virtual void OnResourceBuildingStarted() {

        }

        protected virtual void OnResourceBuildingFinished() {

        }
    }
}

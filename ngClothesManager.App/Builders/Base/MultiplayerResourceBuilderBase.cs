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
                YmtPedDefinitionFile ymt = GetYmtPedDefinitionFile(ymtName, out var componentTextureBindings, out int[] componentIndexes, out int[] propIndexes);

                bool isAnyComponentAdded = false;
                bool isAnyPropAdded = false;

                foreach(Drawable drawable in project.Drawables.Where(drawable => drawable.IsForSex(sex))) {
                    if(drawable.IsComponent) {
                        if(drawable.Textures.Count <= 0) {
                            continue;
                        }
                            
                        MCComponentInfo componentItemInfo = GetYmtPedComponentItem(drawable, ref componentTextureBindings);
                        ymt.Unk_376833625.CompInfos.Add(componentItemInfo);

                        var componentTypeId = componentItemInfo.Unk_3509540765;
                        GetDrawableSuffixes(drawable, out var ytdSuffix, out var yddSuffix);

                        if(!isAnyComponentAdded) {
                            isAnyComponentAdded = true;
                            OnFirstDrawableAddedToResource(sex);
                        }

                        int currentComponentIndex = componentIndexes[componentTypeId]++;

                        string componentNumerics = currentComponentIndex.ToString().PadLeft(3, '0');

                        CopyDrawableModelToResource(drawable, sex, componentNumerics, yddSuffix);

                        foreach(Texture texture in drawable.Textures) {
                            CopyDrawableTextureToResource(drawable, texture, sex, componentNumerics, ytdSuffix, Utils.NumberToLetter(texture.Index));
                        }
                    } else {
                        if(drawable.Textures.Count <= 0) {
                            continue;
                        }

                        Unk_2834549053 anchor = (Unk_2834549053)drawable.PedPropTypeId;
                        List<MUnk_94549140> defs = ymt.Unk_376833625.PropInfo.Props[anchor] ?? new List<MUnk_94549140>();
                        MUnk_94549140 item = GenerateYmtPedPropItem(ymt, anchor, drawable);
                        defs.Add(item);

                        if(!isAnyPropAdded) {
                            isAnyPropAdded = true;
                            OnFirstPropAddedToResource(sex);
                        }

                        int currentPropIndex = propIndexes[(byte)anchor]++;
                        string componentNumerics = currentPropIndex.ToString().PadLeft(3, '0');

                        CopyPropModelToResource(drawable, sex, componentNumerics);

                        foreach(Texture texture in drawable.Textures) {
                            CopyPropTextureToResource(drawable, texture, sex, componentNumerics, Utils.NumberToLetter(texture.Index));
                        }
                    }
                }

                if(isAnyComponentAdded) {
                    UpdateYmtComponentTextureBindings(componentTextureBindings, ymt);
                    string drawableYmtFilePath = GetDrawableYmtFilePath(sex);
                    ymt.Save(drawableYmtFilePath);
                }

                OnResourceDrawableDataFinished(sex, isAnyComponentAdded, isAnyPropAdded);
            }

            OnResourceBuildingFinished();
        }

        protected abstract string GetDrawableYmtFilePath(Sex sex);

        protected abstract void CopyPropTextureToResource(Drawable drawable, Texture texture, Sex sex, string componentNumerics, char offsetLetter);

        protected abstract void CopyPropModelToResource(Drawable drawable, Sex sex, string componentNumerics);

        protected abstract void CopyDrawableTextureToResource(Drawable drawable, Texture texture, Sex sex, string componentNumerics, string ytdSuffix, char offsetLetter);

        protected abstract void CopyDrawableModelToResource(Drawable drawable, Sex sex, string componentNumerics, string yddSuffix);

        protected virtual void OnResourceDrawableDataFinished(Sex sex, bool isAnyComponentAdded, bool isAnyPropAdded) {

        }

        protected virtual void OnFirstPropAddedToResource(Sex sex) {

        }

        protected virtual void OnFirstDrawableAddedToResource(Sex sex) {

        }

        protected virtual void OnResourceBuildingStarted() {

        }

        protected virtual void OnResourceBuildingFinished() {

        }
    }
}

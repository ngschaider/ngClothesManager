using System;
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

            foreach(Gender gender in Enum.GetValues(typeof(Gender))) {
                string ymtName = gender.ToPrefix() + OutputName;
                YmtPedDefinitionFile ymt = GetYmtPedDefinitionFile(ymtName, out var componentTextureBindings, out int[] componentIndexes, out int[] propIndexes);

                bool isAnyComponentAdded = false;
                bool isAnyPropAdded = false;

                foreach(Drawable drawable in project.Drawables.Where(drawable => drawable.Gender == gender)) {
                    if(drawable.IsComponent) {
                        if(drawable.IsEmpty) {
                            continue;
                        }
                            
                        MCComponentInfo componentItemInfo = GetYmtPedComponentItem(drawable, ref componentTextureBindings);
                        ymt.Unk_376833625.CompInfos.Add(componentItemInfo);

                        var componentTypeId = componentItemInfo.Unk_3509540765;
                        GetDrawableSuffixes(drawable, out var ytdSuffix, out var yddSuffix);

                        if(!isAnyComponentAdded) {
                            isAnyComponentAdded = true;
                            OnFirstDrawableAddedToResource(gender);
                        }

                        int currentComponentIndex = componentIndexes[componentTypeId]++;

                        string componentNumerics = currentComponentIndex.ToString().PadLeft(3, '0');

                        CopyDrawableModelToResource(drawable, gender, componentNumerics, yddSuffix);

                        foreach(Texture texture in drawable.Textures) {
                            CopyDrawableTextureToResource(drawable, texture, gender, componentNumerics, ytdSuffix, Utils.NumberToLetter(texture.Id));
                        }
                    } else {
                        if(drawable.IsEmpty) {
                            continue;
                        }

                        Unk_2834549053 anchor = (Unk_2834549053)drawable.PedPropTypeId;
                        List<MUnk_94549140> defs = ymt.Unk_376833625.PropInfo.Props[anchor] ?? new List<MUnk_94549140>();
                        MUnk_94549140 item = GenerateYmtPedPropItem(ymt, anchor, drawable);
                        defs.Add(item);

                        if(!isAnyPropAdded) {
                            isAnyPropAdded = true;
                            OnFirstPropAddedToResource(gender);
                        }

                        int currentPropIndex = propIndexes[(byte)anchor]++;
                        string componentNumerics = currentPropIndex.ToString().PadLeft(3, '0');

                        CopyPropModelToResource(drawable, gender, componentNumerics);

                        foreach(Texture texture in drawable.Textures) {
                            CopyPropTextureToResource(drawable, texture, gender, componentNumerics, Utils.NumberToLetter(texture.Id));
                        }
                    }
                }

                if(isAnyComponentAdded) {
                    UpdateYmtComponentTextureBindings(componentTextureBindings, ymt);
                    string drawableYmtFilePath = GetDrawableYmtFilePath(gender);
                    ymt.Save(drawableYmtFilePath);
                }

                OnResourceDrawableDataFinished(gender, isAnyComponentAdded, isAnyPropAdded);
            }

            OnResourceBuildingFinished();
        }

        protected abstract string GetDrawableYmtFilePath(Gender gender);

        protected abstract void CopyPropTextureToResource(Drawable drawable, Texture texture, Gender gender, string componentNumerics, char offsetLetter);

        protected abstract void CopyPropModelToResource(Drawable drawable, Gender gender, string componentNumerics);

        protected abstract void CopyDrawableTextureToResource(Drawable drawable, Texture texture, Gender gender, string componentNumerics, string ytdSuffix, char offsetLetter);

        protected abstract void CopyDrawableModelToResource(Drawable drawable, Gender gender, string componentNumerics, string yddSuffix);

        protected virtual void OnResourceDrawableDataFinished(Gender gender, bool isAnyComponentAdded, bool isAnyPropAdded) {

        }

        protected virtual void OnFirstPropAddedToResource(Gender gender) {

        }

        protected virtual void OnFirstDrawableAddedToResource(Gender gender) {

        }

        protected virtual void OnResourceBuildingStarted() {

        }

        protected virtual void OnResourceBuildingFinished() {

        }
    }
}

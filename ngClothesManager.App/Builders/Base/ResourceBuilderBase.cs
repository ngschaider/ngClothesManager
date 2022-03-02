using System.Collections.Generic;
using ngClothesManager.App.Contracts;
using RageLib.GTA5.ResourceWrappers.PC.Meta.Structures;
using RageLib.Resources.GTA5.PC.GameFiles;
using RageLib.Resources.GTA5.PC.Meta;

namespace ngClothesManager.App.Builders.Base {
    public abstract class ResourceBuilderBase : IDrawablesResourceBuilder {

        protected Project project;
        public string OutputName = "unnamed";
        public string outputFolder;

        public ResourceBuilderBase(Project project, string outputFolder) {
            this.project = project;
            this.outputFolder = outputFolder;
        }

        protected string GetShopMetaContent(Gender gender) {
            string targetName = gender == Gender.Male ? "mp_m_freemode_01" : "mp_f_freemode_01";
            string dlcName = (gender == Gender.Male ? "mp_m_" : "mp_f_") + OutputName;
            string character = gender == Gender.Male ? "SCR_CHAR_MULTIPLAYER" : "SCR_CHAR_MULTIPLAYER_F";
            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<ShopPedApparel>
	<pedName>{targetName}</pedName>
	<dlcName>{dlcName}</dlcName>
	<fullDlcName>{targetName}_{dlcName}</fullDlcName>
	<eCharacter>{character}</eCharacter>
	<creatureMetaData>MP_CreatureMetadata_{OutputName}</creatureMetaData>
	<pedOutfits>
	</pedOutfits>
	<pedComponents>
	</pedComponents>
	<pedProps>
	</pedProps>
</ShopPedApparel>";
        }

        protected void UpdateYmtComponentTextureBindings(MUnk_3538495220[] componentTextureBindings, YmtPedDefinitionFile ymt) {
            int arrIndex = 0;
            for(int i = 0; i < componentTextureBindings.Length; ++i) {
                if(componentTextureBindings[i] != null) {
                    byte id = (byte)arrIndex++;
                    ymt.Unk_376833625.Unk_2996560424.SetByte(i, id);
                }

                ymt.Unk_376833625.Components[(Unk_884254308)i] = componentTextureBindings[i];
            }
        }

        private List<MUnk_254518642> GetTexDataForDrawable(Drawable drawable) {
            List<MUnk_254518642> items = new List<MUnk_254518642>();
            for(int i = 0; i < drawable.Textures.Count; i++) {
                byte texId = GetTexIdByDrawableType(drawable, i);
                MUnk_254518642 texture = new MUnk_254518642 {
                    TexId = texId
                };
                items.Add(texture);
            }
            return items;
        }

        // TODO DURTY: verify if its really based on index? Shouldnt be like this actually, because its connected to skin tone or something
        private byte GetTexIdByDrawableType(Drawable drawable, int index = 0) {
            byte texId = (byte)index;
            switch(drawable.DrawableType) {
                case DrawableType.Legs:
                    texId = 1;
                    break;
                case DrawableType.Shoes:
                    texId = 0;
                    break;
                case DrawableType.Mask:
                    texId = 1;
                    break;
            }
            return texId;
        }

        protected MUnk_94549140 GenerateYmtPedPropItem(YmtPedDefinitionFile ymt, Unk_2834549053 anchor, Drawable drawable) {
            var item = new MUnk_94549140(ymt.Unk_376833625.PropInfo) {
                AnchorId = (byte)anchor,
                TexData = GetTexDataForDrawable(drawable)
            };

            // Get or create linked anchor
            var anchorProps = ymt.Unk_376833625.PropInfo.AAnchors.Find(e => e.Anchor == anchor);

            if(anchorProps == null) {
                anchorProps = new MCAnchorProps(ymt.Unk_376833625.PropInfo) {
                    Anchor = anchor,
                    PropsMap = { [item] = (byte)item.TexData.Count }
                };

                ymt.Unk_376833625.PropInfo.AAnchors.Add(anchorProps);
            } else {
                anchorProps.PropsMap[item] = (byte)item.TexData.Count;
            }
            return item;
        }

        protected void GetDrawableSuffixes(Drawable drawable, out string ytdSuffix, out string yddSuffix) {
            yddSuffix = drawable.Suffix.EndsWith("u") ? "u" : "r";
            ytdSuffix = drawable.Suffix.EndsWith("u") ? "uni" : "whi";

            switch(drawable.DrawableType) {
                case DrawableType.Legs:
                    yddSuffix = "r";
                    ytdSuffix = "whi";
                    break;
                case DrawableType.Mask:
                    yddSuffix = "r";
                    ytdSuffix = "whi";
                    break;
                case DrawableType.Shoes:
                    yddSuffix = "r";
                    ytdSuffix = "uni";
                    break;
            }
        }

        protected MCComponentInfo GetYmtPedComponentItem(Drawable drawable, ref MUnk_3538495220[] componentTextureBindings) {
            byte componentTypeId = drawable.ComponentTypeId;
            if(componentTextureBindings[componentTypeId] == null)
                componentTextureBindings[componentTypeId] = new MUnk_3538495220();

            byte nextPropMask = 17;
            switch(componentTypeId) {
                case 2:
                case 7:
                    nextPropMask = 11;
                    break;
                case 5:
                case 8:
                    nextPropMask = 65;
                    break;
                case 9:
                    nextPropMask = 1;
                    break;
                case 10:
                    nextPropMask = 5;
                    break;
                case 11:
                    nextPropMask = 1;
                    break;
                default:
                    break;
            }

            MUnk_1535046754 textureDescription = new MUnk_1535046754 {
                PropMask = nextPropMask,
                //Unk_2806194106 = (byte)(drawable.FirstPersonModelPath != "" ? 1 : 0),
                ClothData =
                {
                    Unk_2828247905 = 0
                }
            };

            byte texId = GetTexIdByDrawableType(drawable);
            foreach(Texture texture in drawable.Textures) {
                MUnk_1036962405 texInfo = new MUnk_1036962405 {
                    Distribution = 255,
                    TexId = texId
                };
                textureDescription.ATexData.Add(texInfo);
            }

            componentTextureBindings[componentTypeId].Unk_1756136273.Add(textureDescription);

            byte componentTextureLocalId = (byte)(componentTextureBindings[componentTypeId].Unk_1756136273.Count - 1);

            return new MCComponentInfo {
                Unk_802196719 = 0,
                Unk_4233133352 = 0,
                Unk_128864925 =
                {
                    b0 = (byte) (drawable.ComponentFlags.UnkFlag1 ? 1 : 0),
                    b1 = (byte) (drawable.ComponentFlags.UnkFlag2 ? 1 : 0),
                    b2 = (byte) (drawable.ComponentFlags.UnkFlag3 ? 1 : 0),
                    b3 = (byte) (drawable.ComponentFlags.UnkFlag4 ? 1 : 0),
                    b4 = (byte) (drawable.ComponentFlags.IsHighHeels ? 1 : 0)
                },
                Flags = 0,
                Inclusions = 0,
                Exclusions = 0,
                Unk_1613922652 = 0,
                Unk_2114993291 = 0,
                Unk_3509540765 = componentTypeId,
                Unk_4196345791 = componentTextureLocalId
            };
        }

        protected YmtPedDefinitionFile GetYmtPedDefinitionFile(string ymtName, out MUnk_3538495220[] componentTextureBindings, out int[] componentIndexes, out int[] propIndexes) {
            //Male YMT generating
            YmtPedDefinitionFile ymt = new YmtPedDefinitionFile {
                metaYmtName = ymtName,
                Unk_376833625 = { DlcName = RageLib.Hash.Jenkins.Hash(ymtName) }
            };

            componentTextureBindings = new MUnk_3538495220[] { null, null, null, null, null, null, null, null, null, null, null, null };
            componentIndexes = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            propIndexes = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            //ymt.Unk_376833625.Unk_1235281004 = 0;
            //ymt.Unk_376833625.Unk_4086467184 = 0;
            //ymt.Unk_376833625.Unk_911147899 = 0;
            //ymt.Unk_376833625.Unk_315291935 = 0;
            //ymt.Unk_376833625.Unk_2996560424 = ;

            return ymt;
        }

        public abstract void BuildResource();
    }
}

using System.Collections.Generic;
using System.IO;
using System.Text;
using ngClothesManager.App.Builders.Base;
using RageLib.Archives;
using RageLib.GTA5.Archives;
using RageLib.GTA5.ArchiveWrappers;
using RageLib.GTA5.ResourceWrappers.PC.Meta.Structures;
using RageLib.Resources.GTA5.PC.GameFiles;
using RageLib.Resources.GTA5.PC.Meta;

namespace ngClothesManager.App.Builders {
    public class SingleplayerResourceBuilder : ResourceBuilderBase {

        public SingleplayerResourceBuilder(Project project, string outputFolder) : base(project, outputFolder) {

        }

        private string GetSingleplayerContentXml(bool hasMale, bool hasFemale, bool hasMaleProps, bool hasFemaleProps) {
            string str = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<CDataFileMgr__ContentsOfDataFileXml>
  <disabledFiles />
  <includedXmlFiles />
  <includedDataFiles />
  <dataFiles>
";

            if(hasMale) {
                str += $@"      <Item>
      <filename>dlc_{OutputName}:/common/data/mp_m_freemode_01_mp_m_{OutputName}.meta</filename>
      <fileType>SHOP_PED_APPAREL_META_FILE</fileType>
      <overlay value=""false"" />
      <disabled value=""true"" />
      <persistent value=""false"" />
    </Item>
    <Item>
      <filename>dlc_{OutputName}:/%PLATFORM%/models/cdimages/{OutputName}_male.rpf</filename>
      <fileType>RPF_FILE</fileType>
      <overlay value=""false"" />
      <disabled value=""true"" />
      <persistent value=""true"" />
    </Item>
";
            }

            if(hasFemale) {
                str += $@"    <Item>
      <filename>dlc_{OutputName}:/common/data/mp_f_freemode_01_mp_f_{OutputName}.meta</filename>
      <fileType>SHOP_PED_APPAREL_META_FILE</fileType>
      <overlay value=""false"" />
      <disabled value=""true"" />
      <persistent value=""false"" />
    </Item>
    <Item>
      <filename>dlc_{OutputName}:/%PLATFORM%/models/cdimages/{OutputName}_female.rpf</filename>
      <fileType>RPF_FILE</fileType>
      <overlay value=""false"" />
      <disabled value=""true"" />
      <persistent value=""true"" />
    </Item>
";
            }

            if(hasMaleProps) {
                str += $@"      <Item>
      <filename>dlc_{OutputName}:/%PLATFORM%/models/cdimages/{OutputName}_male_p.rpf</filename>
      <fileType>RPF_FILE</fileType>
      <overlay value=""false"" />
      <disabled value=""true"" />
      <persistent value=""true"" />
    </Item>
";
            }

            if(hasFemaleProps) {
                str += $@"    <Item>
      <filename>dlc_{OutputName}:/%PLATFORM%/models/cdimages/{OutputName}_female_p.rpf</filename>
      <fileType>RPF_FILE</fileType>
      <overlay value=""false"" />
      <disabled value=""true"" />
      <persistent value=""true"" />
    </Item>
";
            }

            str += $@"</dataFiles>
  <contentChangeSets>
    <Item>
      <changeSetName>{OutputName.ToUpper()}_AUTOGEN</changeSetName>
      <mapChangeSetData />
      <filesToInvalidate />
      <filesToDisable />
      <filesToEnable>
";
            if(hasMale) {
                str += $"    <Item>dlc_{OutputName}:/common/data/mp_m_freemode_01_mp_m_{OutputName}.meta</Item>\n";
                str += $"    <Item>dlc_{OutputName}:/%PLATFORM%/models/cdimages/{OutputName}_male.rpf</Item>\n";
            }

            if(hasFemale) {
                str += $"    <Item>dlc_{OutputName}:/common/data/mp_f_freemode_01_mp_f_{OutputName}.meta</Item>\n";
                str += $"    <Item>dlc_{OutputName}:/%PLATFORM%/models/cdimages/{OutputName}_female.rpf</Item>\n";
            }

            if(hasMaleProps) {
                str += $"    <Item>dlc_{OutputName}:/%PLATFORM%/models/cdimages/{OutputName}_male_p.rpf</Item>\n";
            }

            if(hasFemaleProps) {
                str += $"    <Item>dlc_{OutputName}:/%PLATFORM%/models/cdimages/{OutputName}_female_p.rpf</Item>\n";
            }

            str += $@"      </filesToEnable>
    </Item>
  </contentChangeSets>
  <patchFiles />
</CDataFileMgr__ContentsOfDataFileXml>";

            return str;
        }

        private string GetSingleplayerSetup2Xml() {
            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<SSetupData>
    <deviceName>dlc_{OutputName}</deviceName>
    <datFile>content.xml</datFile>
    <timeStamp>07/07/2077 07:07:07</timeStamp>
    <nameHash>{OutputName}</nameHash>
    <contentChangeSets />
    <contentChangeSetGroups>
        <Item>
            <NameHash>GROUP_STARTUP</NameHash>
            <ContentChangeSets>
                <Item>{OutputName.ToUpper()}_AUTOGEN</Item>
            </ContentChangeSets>
        </Item>
    </contentChangeSetGroups>
    <startupScript />
    <scriptCallstackSize value=""0"" />
    <type>EXTRACONTENT_COMPAT_PACK</type>
    <order value=""1000"" />
    <minorOrder value=""0"" />
    <isLevelPack value=""false"" />
    <dependencyPackHash />
    <requiredVersion />
    <subPackCount value=""0"" />
</SSetupData>";
        }

        public override void BuildResource() {
            Utils.EnsureKeys();

            using(RageArchiveWrapper7 rpf = RageArchiveWrapper7.Create(outputFolder + @"\dlc.rpf")) {
                rpf.archive_.Encryption = RageArchiveEncryption7.NG;

                var dir = rpf.Root.CreateDirectory();
                dir.Name = "common";

                var dataDir = dir.CreateDirectory();
                dataDir.Name = "data";

                dir = rpf.Root.CreateDirectory();
                dir.Name = "x64";

                dir = dir.CreateDirectory();
                dir.Name = "models";

                var cdimagesDir = dir.CreateDirectory();
                cdimagesDir.Name = "cdimages";

                RageArchiveWrapper7 currComponentRpf = null;
                IArchiveDirectory currComponentDir = null;

                RageArchiveWrapper7 currPropRpf = null;
                IArchiveDirectory currPropDir = null;

                bool hasMale = false;
                bool hasFemale = false;
                bool hasMaleProps = false;
                bool hasFemaleProps = false;

                foreach(Sex sex in new Sex[] { Sex.Male, Sex.Female }) {
                    YmtPedDefinitionFile ymt = GetYmtPedDefinitionFile(sex.ToPrefix() + OutputName, out var componentTextureBindings, out int[] componentIndexes, out var propIndexes);

                    bool isAnyComponentAdded = false;
                    bool isAnyPropAdded = false;

                    foreach(Drawable drawable in project.Drawables) {
                        if(drawable.IsComponent) {
                            if(drawable.Textures.Count <= 0 || !drawable.IsForSex(sex)) {
                                continue;
                            }

                            var componentItemInfo = GetYmtPedComponentItem(drawable, ref componentTextureBindings);
                            ymt.Unk_376833625.CompInfos.Add(componentItemInfo);

                            var componentTypeId = componentItemInfo.Unk_3509540765;
                            GetDrawableSuffixes(drawable, out var ytdPostfix, out var yddPostfix);

                            if(!isAnyComponentAdded) {
                                isAnyComponentAdded = true;

                                var ms = new MemoryStream();

                                string sexStr = sex == Sex.Male ? "male" : "female";

                                currComponentRpf = RageArchiveWrapper7.Create(ms, OutputName + "_" + sexStr + ".rpf");
                                currComponentRpf.archive_.Encryption = RageArchiveEncryption7.NG;
                                currComponentDir = currComponentRpf.Root.CreateDirectory();
                                currComponentDir.Name = sex.ToPrefix() + "freemode_01_" + sex.ToPrefix() + project.Name;
                            }

                            int currentComponentIndex = componentIndexes[componentTypeId]++;

                            string componentNumerics = currentComponentIndex.ToString().PadLeft(3, '0');
                            string prefix = drawable.Prefix;

                            var resource = currComponentDir.CreateResourceFile();
                            resource.Name = prefix + "_" + componentNumerics + "_" + yddPostfix + ".ydd";
                            resource.Import(drawable.ModelPath);

                            foreach(Texture texture in drawable.Textures) {
                                resource = currComponentDir.CreateResourceFile();
                                resource.Name = prefix + "_diff_" + componentNumerics + "_" + Utils.NumberToLetter(texture.Index) + "_" + ytdPostfix + ".ytd";
                                resource.Import(drawable.GetTexturePath(texture.Index));
                            }
                        } else {
                            if(drawable.Textures.Count <= 0 || !drawable.IsForSex(sex)) {
                                continue;
                            }

                            Unk_2834549053 anchor = (Unk_2834549053)drawable.PedPropTypeId;
                            var defs = ymt.Unk_376833625.PropInfo.Props[anchor] ?? new List<MUnk_94549140>();
                            var item = GenerateYmtPedPropItem(ymt, anchor, drawable);
                            defs.Add(item);

                            if(!isAnyPropAdded) {
                                isAnyPropAdded = true;

                                var ms = new MemoryStream();

                                currPropRpf = RageArchiveWrapper7.Create(ms, OutputName + "_" + sex.ToString() + "_p.rpf");
                                currPropRpf.archive_.Encryption = RageArchiveEncryption7.NG;
                                currPropDir = currPropRpf.Root.CreateDirectory();
                                currPropDir.Name = sex.ToPrefix() + "freemode_01_p_" + sex.ToPrefix() + project.Name;
                            }

                            int currentPropIndex = propIndexes[(byte)anchor]++;

                            string componentNumerics = currentPropIndex.ToString().PadLeft(3, '0');
                            string prefix = drawable.Prefix;

                            //drawable.SetComponentNumerics(componentNumerics, currentPropIndex);

                            var resource = currPropDir.CreateResourceFile();
                            resource.Name = prefix + "_" + componentNumerics + ".ydd";
                            resource.Import(drawable.ModelPath);

                            foreach(Texture texture in drawable.Textures) {
                                resource = currPropDir.CreateResourceFile();
                                resource.Name = prefix + "_diff_" + drawable.Index + "_" + Utils.NumberToLetter(texture.Index) + ".ytd";
                                resource.Import(drawable.GetTexturePath(texture.Index));
                            }
                            
                        }
                    }

                    if(isAnyComponentAdded) {
                        if(sex == Sex.Male) {
                            hasMale = true;
                        } else if(sex == Sex.Female) {
                            hasFemale = true;
                        }

                        UpdateYmtComponentTextureBindings(componentTextureBindings, ymt);
                    }

                    if(isAnyComponentAdded || isAnyPropAdded) {
                        using(MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(GetShopMetaContent(sex)))) {
                            var binFile = dataDir.CreateBinaryFile();
                            binFile.Name = sex.ToPrefix() + "freemode_01_" + sex.ToPrefix() + project.Name + ".meta";
                            binFile.Import(stream);
                        }
                        currComponentRpf.Flush();

                        var binRpfFile = cdimagesDir.CreateBinaryFile();
                        binRpfFile.Name = OutputName + "_" + sex.ToString() + ".rpf";
                        binRpfFile.Import(currComponentRpf.archive_.BaseStream);

                        currComponentRpf.Dispose();
                    }

                    if(isAnyPropAdded) {
                        if(sex == Sex.Male) {
                            hasMaleProps = true;
                        } else if(sex == Sex.Female) {
                            hasFemaleProps = true;
                        }

                        currPropRpf.Flush();

                        var binRpfFile = cdimagesDir.CreateBinaryFile();
                        binRpfFile.Name = OutputName + "_" + sex.ToString() + "_p.rpf";
                        binRpfFile.Import(currPropRpf.archive_.BaseStream);

                        currPropRpf.Dispose();
                    }
                }

                using(MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(GetSingleplayerContentXml(hasMale, hasFemale, hasMaleProps, hasFemaleProps)))) {
                    var binFile = rpf.Root.CreateBinaryFile();
                    binFile.Name = "content.xml";
                    binFile.Import(stream);
                }

                using(MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(GetSingleplayerSetup2Xml()))) {
                    var binFile = rpf.Root.CreateBinaryFile();
                    binFile.Name = "setup2.xml";
                    binFile.Import(stream);
                }

                rpf.Flush();
                rpf.Dispose();
            }
        }
    }
}
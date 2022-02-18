using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngClothesManager.App {
    class TextureImporter {

        private readonly Cloth cloth;
        private readonly Project project;

        public TextureImporter(Project project, Cloth cloth) {
            this.project = project;
            this.cloth = cloth;
        }

        public void Import(string filePath) {
            int textureIndex = cloth.GetEmptyTextureIndex();

            Texture texture = new Texture(textureIndex);

            cloth.Textures.Add(texture);
            project.AddFile(filePath, cloth.GetTexturePath(textureIndex));
        }

    }
}

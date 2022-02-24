using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngClothesManager.App {
    class TextureImporter {

        private readonly Drawable drawable;
        private readonly Project project;

        public TextureImporter(Project project, Drawable drawable) {
            this.project = project;
            this.drawable = drawable;
        }

        public void Import(string filePath) {
            int textureIndex = drawable.GetEmptyTextureIndex();

            Texture texture = new Texture(textureIndex);

            drawable.Textures.Add(texture);
            project.AddFile(filePath, drawable.GetTexturePath(textureIndex));
        }

    }
}

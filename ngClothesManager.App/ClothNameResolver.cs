using System;
using System.IO;

namespace ngClothesManager.App {
    public partial class ClothNameResolver {

        public ClothType ClothType {
            get;
        }
        public DrawableType DrawableType {
            get;
        }
        public string BindedNumber {
            get;
        }
        public string Postfix { get; } = "";
        public bool IsVariation {
            get;
        }

        public override string ToString() {
            return ClothType + " " + DrawableType + " " + BindedNumber;
        }
    }
}

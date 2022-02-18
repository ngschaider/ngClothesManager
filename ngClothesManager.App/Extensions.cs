using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngClothesManager.App {
    public static class Extensions {
        public static string ToIdentifier(this DrawableType type) {
            switch(type) {
                case DrawableType.Head:
                    return "head";
                case DrawableType.Mask:
                    return "berd";
                case DrawableType.Hair:
                    return "hair";
                case DrawableType.Body:
                    return "uppr";
                case DrawableType.Legs:
                    return "lowr";
                case DrawableType.Bag:
                    return "hand";
                case DrawableType.Shoes:
                    return "feet";
                case DrawableType.Accessories:
                    return "teef";
                case DrawableType.Undershirt:
                    return "accs";
                case DrawableType.Armor:
                    return "task";
                case DrawableType.Decal:
                    return "decl";
                case DrawableType.Top:
                    return "jbib";
                case DrawableType.PropHead:
                    return "p_head";
                case DrawableType.PropEyes:
                    return "p_eyes";
                case DrawableType.PropEars:
                    return "p_ears";
                case DrawableType.PropMouth:
                    return "p_mouth";
                case DrawableType.PropLHand:
                    return "p_lhand";
                case DrawableType.PropRHand:
                    return "p_rhand";
                case DrawableType.PropLWrist:
                    return "p_lwrist";
                case DrawableType.PropRWrist:
                    return "p_rwrist";
                case DrawableType.PropHip:
                    return "p_hip";
                case DrawableType.PropLFoot:
                    return "p_lfoot";
                case DrawableType.PropRFoot:
                    return "p_rfoot";
                case DrawableType.PropUnk1:
                    return "p_unk1";
                case DrawableType.PropUnk2:
                    return "p_unk2";
                default:
                    return "";
            }
        }

        public static DrawableType ToDrawableType(this string input) {
            switch(input) {
                case "head":
                    return DrawableType.Head;
                case "berd":
                    return DrawableType.Mask;
                case "hair":
                    return DrawableType.Hair;
                case "uppr":
                    return DrawableType.Body;
                case "lowr":
                    return DrawableType.Legs;
                case "hand":
                    return DrawableType.Bag;
                case "feet":
                    return DrawableType.Shoes;
                case "teef":
                    return DrawableType.Accessories;
                case "accs":
                    return DrawableType.Undershirt;
                case "task":
                    return DrawableType.Armor;
                case "decl":
                    return DrawableType.Decal;
                case "jbib":
                    return DrawableType.Top;
                case "p_head":
                    return DrawableType.PropHead;
                case "p_eyes":
                    return DrawableType.PropEyes;
                case "p_ears":
                    return DrawableType.PropEars;
                case "p_mouth":
                    return DrawableType.PropMouth;
                case "p_lhand":
                    return DrawableType.PropLHand;
                case "p_rhand":
                    return DrawableType.PropRHand;
                case "p_lwrist":
                    return DrawableType.PropLWrist;
                case "p_rwrist":
                    return DrawableType.PropRWrist;
                case "p_hip":
                    return DrawableType.PropHip;
                case "p_lfoot":
                    return DrawableType.PropLFoot;
                case "p_rfoot":
                    return DrawableType.PropRFoot;
                case "p_unk1":
                    return DrawableType.PropUnk1;
                case "p_unk2":
                    return DrawableType.PropUnk2;
                default:
                    return DrawableType.Invalid;
            }
        }
    }
}

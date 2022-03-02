using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngClothesManager.App {
    public static class Extensions {
        public static string ToPrefix(this Gender gender) {
            switch(gender) {
                case Gender.Male:
                    return "mp_m_";
                case Gender.Female:
                    return "mp_f_";
                default:
                    throw new ArgumentException();
            }
        }
        public static string ToString(this Gender gender) {
            switch(gender) {
                case Gender.Male:
                    return "male";
                case Gender.Female:
                    return "female";
                default:
                    throw new ArgumentException();
            }
        }

        public static bool IsComponent(this DrawableType type) {
            switch(type) {
                case DrawableType.Head:
                case DrawableType.Mask:
                case DrawableType.Hair:
                case DrawableType.Body:
                case DrawableType.Legs:
                case DrawableType.Bag:
                case DrawableType.Shoes:
                case DrawableType.Accessories:
                case DrawableType.Undershirt:
                case DrawableType.Armor:
                case DrawableType.Decal:
                case DrawableType.Top:
                    return true;
                case DrawableType.PropHead:
                case DrawableType.PropEyes:
                case DrawableType.PropEars:
                case DrawableType.PropMouth:
                case DrawableType.PropLHand:
                case DrawableType.PropRHand:
                case DrawableType.PropLWrist:
                case DrawableType.PropRWrist:
                case DrawableType.PropHip:
                case DrawableType.PropLFoot:
                case DrawableType.PropRFoot:
                case DrawableType.PropUnk1:
                case DrawableType.PropUnk2:
                    return false;
                default:
                    throw new ArgumentException();
            }
        }

        public static bool IsProp(this DrawableType type) {
            return !type.IsComponent();
        }

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
                    throw new ArgumentException();
            }
        }

        public static bool ToDrawableType(this string input, out DrawableType output) {
            switch(input) {
                case "head":
                    output = DrawableType.Head;
                    return true;
                case "berd":
                    output = DrawableType.Mask;
                    return true;
                case "hair":
                    output = DrawableType.Hair;
                    return true;
                case "uppr":
                    output = DrawableType.Body;
                    return true;
                case "lowr":
                    output = DrawableType.Legs;
                    return true;
                case "hand":
                    output = DrawableType.Bag;
                    return true;
                case "feet":
                    output = DrawableType.Shoes;
                    return true;
                case "teef":
                    output = DrawableType.Accessories;
                    return true;
                case "accs":
                    output = DrawableType.Undershirt;
                    return true;
                case "task":
                    output = DrawableType.Armor;
                    return true;
                case "decl":
                    output = DrawableType.Decal;
                    return true;
                case "jbib":
                    output = DrawableType.Top;
                    return true;
                case "p_head":
                    output = DrawableType.PropHead;
                    return true;
                case "p_eyes":
                    output = DrawableType.PropEyes;
                    return true;
                case "p_ears":
                    output = DrawableType.PropEars;
                    return true;
                case "p_mouth":
                    output = DrawableType.PropMouth;
                    return true;
                case "p_lhand":
                    output = DrawableType.PropLHand;
                    return true;
                case "p_rhand":
                    output = DrawableType.PropRHand;
                    return true;
                case "p_lwrist":
                    output = DrawableType.PropLWrist;
                    return true;
                case "p_rwrist":
                    output = DrawableType.PropRWrist;
                    return true;
                case "p_hip":
                    output = DrawableType.PropHip;
                    return true;
                case "p_lfoot":
                    output = DrawableType.PropLFoot;
                    return true;
                case "p_rfoot":
                    output = DrawableType.PropRFoot;
                    return true;
                case "p_unk1":
                    output = DrawableType.PropUnk1;
                    return true;
                case "p_unk2":
                    output = DrawableType.PropUnk2;
                    return true;
                default:
                    // it does not matter what we set here
                    // the output should not be used when returning false
                    output = DrawableType.Accessories; 
                    return false;
            }
        }
    }
}

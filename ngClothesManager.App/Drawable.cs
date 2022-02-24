using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ngClothesManager.App {
    public class Drawable : INotifyPropertyChanged {

        private int _index;
        public int Index {
            get {
                return _index;
            }
            set {
                _index = value;
                OnPropertyChanged(nameof(Index));
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        private DrawableType _drawableType;
        public DrawableType DrawableType {
            get {
                return _drawableType;
            }
            set {
                _drawableType = value;
                OnPropertyChanged(nameof(DrawableType));
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        private ComponentFlags _componentFlags;
        public ComponentFlags ComponentFlags {
            get {
                return _componentFlags;
            }
            set {
                _componentFlags = value;
                OnPropertyChanged(nameof(ComponentFlags));
            }
        }

        private PropFlags _propFlags;
        public PropFlags PropFlags {
            get {
                return _propFlags;
            }
            set {
                _propFlags = value;
                OnPropertyChanged(nameof(PropFlags));
            }
        }

        private ObservableCollection<Texture> _textures = new ObservableCollection<Texture>();

        public ObservableCollection<Texture> Textures {
            get {
                return _textures;
            }
            set {
                _textures = value;
                OnPropertyChanged(nameof(Textures));
            }
        }

        private bool _isMale;

        public bool IsMale {
            get {
                return _isMale;
            }
            set {
                _isMale = value;
                OnPropertyChanged(nameof(IsMale));
            }
        }

        private bool _isFemale;

        public bool IsFemale {
            get {
                return _isFemale;
            }
            set {
                _isFemale = value;
                OnPropertyChanged(nameof(IsFemale));
            }
        }

        public bool IsForSex(Sex sex) {
            switch(sex) {
                case Sex.Male:
                    return IsMale;
                case Sex.Female:
                    return IsFemale;
                default:
                    throw new ArgumentException();
            }
        }

        public string ModelPath {
            get {
                return DrawableType.ToIdentifier() + "/" + Index + "/model.ydd";
            }
        }

        private string _name;
        public string Name {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public string DisplayName {
            get {
                return Name + " (ID " + Index + ")";
            }
        }

        private string _suffix;
        public string Suffix {
            get {
                return _suffix;
            }
            set {
                _suffix = value;
                OnPropertyChanged(nameof(Suffix));
            }
        }

        private Drawable() {
            // Needed for deserialization
        }

        public Drawable(int index, DrawableType drawableType, Sex sex, string suffix) {
            Index = index;
            DrawableType = drawableType;
            Name = DrawableType + "" + Index;
            if(sex == Sex.Male) {
                IsMale = true;
            } else if(sex == Sex.Female) {
                IsFemale = true;
            }
            Suffix = suffix;
        }

        public int GetEmptyTextureIndex() {
            int index = 0;
            while(true) {
                if(Textures.FirstOrDefault(texture => texture.Index == index) == null) {
                    return index;
                }
                index++;
            }
        }        

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsComponent {
            get {
                return DrawableType.IsComponent();
            }
        }

        public byte ComponentTypeId {
            get {
                return IsComponent ? (byte)DrawableType : (byte)255;
            }
        }

        public bool IsProp {
            get {
                return DrawableType.IsProp();
            }
        }

        public byte PedPropTypeId {
            get {
                if(IsProp) {
                    return (byte)((int)DrawableType - (int)DrawableType.PropHead);
                } else {
                    return 255;
                }
            }
        }

        public string Prefix {
            get {
                return DrawableType.ToIdentifier();
            }
        }

        public string GetTexturePath(int textureIndex) {
            return DrawableType.ToIdentifier() + "/" + Index + "/tex" + textureIndex + ".ytd";
        }
    }
}

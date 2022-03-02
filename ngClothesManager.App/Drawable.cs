using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ngClothesManager.App {
    public class Drawable : INotifyPropertyChanged {

        #region Properties

        private int _id;
        public int Id {
            get {
                return _id;
            }
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
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

        public ObservableCollection<Texture> Textures { get; } = new ObservableCollection<Texture>();

        private string _name;
        public string Name {
            get {
                return _name;
            }
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(DisplayName));
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

        private Gender _gender;
        public Gender Gender {
            get {
                return _gender;
            }
            set {
                _gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }

        [JsonIgnore]
        public string ModelPath {
            get {
                return DrawableType.ToIdentifier() + "/" + Id + "/model.ydd";
            }
        }

        [JsonIgnore]
        public string DisplayName {
            get {
                if(IsEmpty) {
                    return Name + "(ID " + Id + ") (Empty)";
                } else {
                    return Name + " (ID " + Id + ")";
                }
            }
        }

        [JsonIgnore]
        public bool IsEmpty {
            get {
                return Textures.Count == 0;
            }
        }

        #endregion

        private Drawable() {
            // Needed for deserialization
        }

        public Drawable(int index, DrawableType drawableType, Gender gender) : this(index, drawableType, gender, "") {
        }

        public Drawable(int index, DrawableType drawableType, Gender gender, string suffix) {
            Id = index;
            DrawableType = drawableType;
            Name = DrawableType.ToString();
            Gender = gender;
            Suffix = suffix;
        }

        public int GetEmptyTextureIndex() {
            int index = 0;
            while(true) {
                if(Textures.FirstOrDefault(texture => texture.Id == index) == null) {
                    return index;
                }
                index++;
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = null) {
            //Logger.Log("PropertChanged: Drawable." + propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

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
            return DrawableType.ToIdentifier() + "/" + Id + "/tex" + textureIndex + ".ytd";
        }
    }
}

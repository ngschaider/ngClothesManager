using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ngClothesManager.App {
    public class DrawableListEntry : INotifyPropertyChanged {
        public enum EntryType {
            DrawableType,
            Gender,
            Drawable
        }

        public EntryType Type;
        public Drawable Drawable;
        public DrawableType DrawableType;
        public Gender Gender;

        public string Label {
            get {
                if(Type == EntryType.Drawable) {
                    return Drawable.DisplayName;
                } else if(Type == EntryType.Gender) {
                    return Gender.ToString();
                } else if(Type == EntryType.DrawableType) {
                    return DrawableType.ToString() + " (" + DrawableType.ToIdentifier() + ")";
                }
                return nameof(DrawableListEntry) + " Error!";
            }
        }

        private ObservableCollection<DrawableListEntry> _children = new ObservableCollection<DrawableListEntry>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<DrawableListEntry> Children {
            get {
                return _children;
            }
            set {
                _children = value;
                OnPropertyChanged(nameof(Children));
            }
        }
    }
}

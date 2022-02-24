using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ngClothesManager.App {
    public class DrawableListEntry : INotifyPropertyChanged {
        public Drawable Drawable;
        public DrawableType DrawableType = DrawableType.None;
        public Sex Sex = Sex.None;

        public string Label {
            get {
                if(DrawableType != DrawableType.None) {
                    string ret = DrawableType.ToString();
                    if(DrawableType != DrawableType.Unkown) {
                        ret += " (" + DrawableType.ToIdentifier() + ")";
                    }
                    return ret;
                }

                return Drawable != null ? Drawable.DisplayName : (DrawableType != DrawableType.None ? DrawableType.ToString() : Sex.ToString());
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

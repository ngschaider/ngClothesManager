using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngClothesManager.App {
    public struct ComponentFlags : INotifyPropertyChanged {

        private bool _unkFlag1;

        public bool UnkFlag1 {
            get {
                return _unkFlag1;
            }
            set {
                _unkFlag1 = value;
                OnPropertyChanged(nameof(UnkFlag1));
            }
        }


        private bool _unkFlag2;

        public bool UnkFlag2 {
            get {
                return _unkFlag2;
            }
            set {
                _unkFlag2 = value;
                OnPropertyChanged(nameof(UnkFlag2));
            }
        }


        private bool _unkFlag3;

        public bool UnkFlag3 {
            get {
                return _unkFlag3;
            }
            set {
                _unkFlag3 = value;
                OnPropertyChanged(nameof(UnkFlag3));
            }
        }


        private bool _unkFlag4;

        public bool UnkFlag4 {
            get {
                return _unkFlag4;
            }
            set {
                _unkFlag4 = value;
                OnPropertyChanged(nameof(UnkFlag4));
            }
        }


        private bool _isHighHeels;

        public bool IsHighHeels {
            get {
                return _isHighHeels;
            }
            set {
                _isHighHeels = value;
                OnPropertyChanged(nameof(IsHighHeels));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

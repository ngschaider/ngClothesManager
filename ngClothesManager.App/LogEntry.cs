using System;
using System.ComponentModel;

namespace ngClothesManager.App {
    public class LogEntry : INotifyPropertyChanged {

        private static uint lastIndex = 0;


        private uint _index;

        public uint Index {
            get {
                return _index;
            }
            set {
                _index = value;
                OnPropertyChanged(nameof(Index));
            }
        }

        private string _message;

        public string Message {
            get {
                return _message;
            }
            set {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }


        public LogEntry(string message) {
            lastIndex++;
            Index = lastIndex;
            Message = message;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string memberName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
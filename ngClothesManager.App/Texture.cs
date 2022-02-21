using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngClothesManager.App {
    public class Texture : INotifyPropertyChanged {

		private int _index;

		public int Index {
			get {
				return _index;
			}
			set {
				_index = value;
				OnPropertyChanged(nameof(Index));
			}
		}

		private string _name;

		public string Name {
			get {
				return _name;
			}
			set {
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string memberName) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}


		private Texture() {
			// needed for deserializaton
		}

		public Texture(int index) {
			Index = index;
			Name = "Texture " + index;
		}
	}
}

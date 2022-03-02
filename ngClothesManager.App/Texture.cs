using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngClothesManager.App {
    public class Texture : INotifyPropertyChanged {

        #region Properties

        private int _id;
		public int Id {
			get {
				return _id;
			}
			set {
				_id = value;
				OnPropertyChanged(nameof(Id));
			}
		}

		private string _name = "Texture";
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
		
		[JsonIgnore]
		public string DisplayName {
			get {
				return Name + " (ID " + Id + ")";
			}
		}

		#endregion

		#region INotifyPropertChanged

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName) {
			//Logger.Log("PropertyChanged: Texture." + propertyName);
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		private Texture() {
			// needed for deserializaton
		}

		public Texture(int id) {
			Id = id;
		}
	}
}

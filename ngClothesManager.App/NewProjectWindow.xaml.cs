using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ngClothesManager.App {
    /// <summary>
    /// Interaktionslogik für NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow : INotifyPropertyChanged {

        private string _projectPath;

        public string ProjectPath {
            get {
                return _projectPath;
            }
            set {
                _projectPath = value;
                OnPropertyChanged(nameof(ProjectPath));
            }
        }

        private string _projectName;

        public string ProjectName {
            get {
                return _projectName;
            }
            set {
                _projectName = value;
                OnPropertyChanged(nameof(ProjectName));
            }
        }

        public NewProjectWindow() {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e) {
            using(System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog()) {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if(result == System.Windows.Forms.DialogResult.OK) {
                    ProjectPath = dialog.SelectedPath;
                    Console.WriteLine("ProjectPath: " + ProjectPath);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e) {
            FileInfo info = null;
            try {
                info = new FileInfo(ProjectName);
            } catch(ArgumentException) { } catch(PathTooLongException) { } catch(NotSupportedException) { }

            if(info is null) {
                MessageBox.Show("The entered project name contains invalid characters.");
                return;
            }

            if(!Directory.Exists(ProjectPath)) {
                MessageBox.Show("The entered project path does not exist.");
                return;
            }

            DialogResult = true;
            Close();
        }

    }
}

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using RageLib.GTA5.ResourceWrappers.PC.Drawables;
using RageLib.ResourceWrappers.Drawables;

namespace ngClothesManager.App {
    public partial class MainWindow : Window, INotifyPropertyChanged {

        public static ProjectBuildWindow ProjectBuildWindow;

        private Project _project;
        public Project Project {
            get {
                return _project;
            }
            set {
                _project = value;
                OnPropertyChanged(nameof(Project));
            }
        }

        private LogWindow logWindow;

        private Cloth _selectedCloth;
        public Cloth SelectedCloth {
            get {
                return _selectedCloth;
            }
            set {
                _selectedCloth = value;
                OnPropertyChanged(nameof(SelectedCloth));
                OnPropertyChanged(nameof(ComponentEditBoxVisibility));
                OnPropertyChanged(nameof(PropEditBoxVisibility));
                //OnPropertyChanged(nameof(DisplayedFirstPersonModelPath));
            }
        }

        private Texture _selectedTexture;

        public Texture SelectedTexture {
            get {
                return _selectedTexture;
            }
            set {
                _selectedTexture = value;
                OnPropertyChanged(nameof(SelectedTexture));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ComponentEditBoxVisibility {
            get {
                return SelectedCloth != null && SelectedCloth.IsComponent ? "Visible" : "Collapsed";
            }
        }

        public string PropEditBoxVisibility {
            get {
                return SelectedCloth != null && SelectedCloth.IsProp ? "Visible" : "Collapsed";
            }
        }

        /*public string DisplayedFirstPersonModelPath {
            get {
                return SelectedCloth != null && SelectedCloth.FirstPersonModelPath != "" ? SelectedCloth.FirstPersonModelPath : "Not selected...";
            }
        }*/

        public MainWindow() {
            InitializeComponent();
            this.DataContext = this;

            Logger.OnLogEntryAdded += OnLogEntryAdded;
        }

        private void OnLogEntryAdded(LogEntry log) {
            statusBarText.Text = log.Message;
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("ngClothesManager\nAuthor: Niklas Gschaider");
        }

        private void LogsButton_Click(object sender, RoutedEventArgs e) {
            logWindow = new LogWindow {
                Owner = this,
            };
            logWindow.Show();
        }

        #region MenuItem Commands

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            string fileName = AskForProjectFile(false);

            if(fileName.Length > 0) {
                Project = Project.Create(fileName);
            }
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            if(Project != null) {
                SaveCommand_Executed(this, null);
                CloseCommand_Executed(this, null);
            }

            string fileName = AskForProjectFile(true);
            if(fileName.Length > 0) {
                try {
                    Project = Project.Open(fileName);
                } catch(IOException exception) {
                    Utils.HandleException(exception);
                }
            }
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null;
        }
        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            Project.Save();
        }

        private void CloseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null;
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            Project.Dispose();
            Project = null;
        }

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }
        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            if(Project != null) {
                SaveCommand_Executed(this, null);
                CloseCommand_Executed(this, null);
            }
            Application.Current.Shutdown();
        }


        private void AddMaleClothesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null;
        }
        private void AddMaleClothesCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            AddClothes(Sex.Male);
        }

        private void AddFemaleClothesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null;
        }
        private void AddFemaleClothesCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            AddClothes(Sex.Female);
        }

        private void RemoveSelectedClothCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null && SelectedCloth != null;
        }
        private void RemoveSelectedClothCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            Project.RemoveCloth(SelectedCloth);
        }

        #endregion

        private string AskForProjectFile(bool existingFile) {
            if(existingFile) {
                OpenFileDialog openFileDialog = new OpenFileDialog {
                    CheckFileExists = true,
                    Filter = "ngClothesManager Project (*.ngcmp)|*.ngcmp",
                    FilterIndex = 1,
                    DefaultExt = "ngcmp"
                };

                if(openFileDialog.ShowDialog() != true) {
                    return "";
                }

                return openFileDialog.FileName;
            } else {
                SaveFileDialog saveFileDialog = new SaveFileDialog {
                    Filter = "ngClothesManager Project (*.ngcmp)|*.ngcmp",
                    FilterIndex = 1,
                    DefaultExt = "ngcmp"
                };

                if(saveFileDialog.ShowDialog() != true) {
                    return "";
                }

                return saveFileDialog.FileName;
            }
        }

        private void AddTexture_Click(object sender, RoutedEventArgs e) {
            if(SelectedCloth == null) {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog {
                CheckFileExists = true,
                Filter = "Clothes texture (*.ytd)|*.ytd",
                FilterIndex = 1,
                DefaultExt = "ytd",
                Multiselect = true,
            };

            if(openFileDialog.ShowDialog() != true) {
                return;
            }

            TextureImporter importer = new TextureImporter(Project, SelectedCloth);
            foreach(string filePath in openFileDialog.FileNames) {
                importer.Import(filePath);
            }
        }

        private void RemoveTexture_Click(object sender, RoutedEventArgs e) {
            if(Project == null || SelectedCloth == null || SelectedTexture == null) {
                return;
            }

            Project.RemoveTexture(SelectedCloth, SelectedTexture);
        }

        private void BuildProjectButton_Click(object sender, RoutedEventArgs e) {
            ProjectBuildWindow = new ProjectBuildWindow();
            ProjectBuildWindow.Show();

            ProjectBuildWindow.OnExecuteBuild += (resType, outputFolder, collectionName) => {
                new ClothesResourceBuilderFactory().BuildResource(resType, Project, outputFolder, collectionName);
            };
        }

        /*private void ClearFirstPersonModel_Click(object sender, RoutedEventArgs e) {
            if(SelectedCloth != null) {
                SelectedCloth.FirstPersonModelPath = "";
            }
        }*/

        /*private void SelectFirstPersonModel_Click(object sender, RoutedEventArgs e) {
            if(SelectedCloth == null) {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog {
                CheckFileExists = true,
                Filter = "Clothes drawable (*.ydd)|*.ydd",
                FilterIndex = 1,
                DefaultExt = "ydd",
                Multiselect = false
            };

            if(openFileDialog.ShowDialog() != true) {
                return;
            }

            foreach(string filePath in openFileDialog.FileNames) {
                SelectedCloth.FirstPersonModelPath = filePath;
            }
        }*/

        public void AddClothes(Sex sex) {
            if(Project == null) {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog {
                CheckFileExists = true,
                Filter = "Clothes geometry (*.ydd)|*.ydd",
                FilterIndex = 1,
                DefaultExt = "ydd",
                Multiselect = true,
                Title = "Adding " + (sex == Sex.Male ? "male" : "female") + " clothes",
            };

            if(openFileDialog.ShowDialog() != true) {
                return;
            }

            ClothImporter importer = new ClothImporter(Project);
            foreach(string filePath in openFileDialog.FileNames) {
                importer.Import(filePath, sex);
            }
        }

        private void TextBox_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e) {

        }
    }
}

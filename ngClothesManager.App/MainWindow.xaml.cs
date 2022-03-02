using Microsoft.Win32;
using ngClothesManager.App.Builders;
using ngClothesManager.App.Builders.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ngClothesManager.App {
    public partial class MainWindow : Window, INotifyPropertyChanged {

        private ProjectBuildWindow ProjectBuildWindow;
        private LogWindow logWindow;

        private bool IsSearchingForDuplicates = false;

        #region Properties

        private Project _project;
        public Project Project {
            get {
                return _project;
            }
            private set {
                _project = value;
                OnPropertyChanged(nameof(Project));
            }
        }

        private Drawable _selectedDrawable;
        public Drawable SelectedDrawable {
            get {
                return _selectedDrawable;
            }
            set {
                _selectedDrawable = value;
                OnPropertyChanged(nameof(SelectedDrawable));
                OnPropertyChanged(nameof(ComponentEditBoxVisibility));
                OnPropertyChanged(nameof(PropEditBoxVisibility));
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

        private DrawableList _drawableList;
        public DrawableList DrawableList {
            get {
                return _drawableList;
            }
            set {
                _drawableList = value;
                OnPropertyChanged(nameof(DrawableList));
            }
        }

        public string ComponentEditBoxVisibility {
            get {
                return SelectedDrawable != null && SelectedDrawable.IsComponent ? "Visible" : "Collapsed";
            }
        }

        public string PropEditBoxVisibility {
            get {
                return SelectedDrawable != null && SelectedDrawable.IsProp ? "Visible" : "Collapsed";
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            //Logger.Log("PropertyChanged: MainWindow." + propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public MainWindow() {
            InitializeComponent();
            this.DataContext = this;

            Logger.OnLogEntryAdded += OnLogEntryAdded;

            PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
                if(e.PropertyName == nameof(Project)) {
                    DrawableList?.OnProjectChanged(Project);
                }
            };

            DrawableList = new DrawableList();
        }

        #region Events

        private void AddTexture_Click(object sender, RoutedEventArgs e) {
            if(SelectedDrawable == null) {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog {
                CheckFileExists = true,
                Filter = "Drawables texture (*.ytd)|*.ytd",
                FilterIndex = 1,
                DefaultExt = "ytd",
                Multiselect = true,
            };

            if(openFileDialog.ShowDialog() != true) {
                return;
            }

            TextureImporter importer = new TextureImporter(Project, SelectedDrawable);
            foreach(string filePath in openFileDialog.FileNames) {
                importer.Import(filePath);
            }
        }
        private void RemoveTexture_Click(object sender, RoutedEventArgs e) {
            if(Project == null || SelectedDrawable == null || SelectedTexture == null) {
                return;
            }

            Project.RemoveTexture(SelectedDrawable, SelectedTexture);
        }

        private void OnLogEntryAdded(LogEntry log) {
            statusBarText.Text = log.Message;
        }

        private void SelectedDrawableListEntryChanged(object sender, RoutedEventArgs e) {
            DrawableListEntry entry = (DrawableListEntry)elDrawableList.SelectedItem;
            if(entry?.Type == DrawableListEntry.EntryType.Drawable && entry?.Drawable != null) {
                SelectedDrawable = entry.Drawable;
            } else {
                SelectedDrawable = null;
            }
        }

        #endregion

        #region MenuItem Commands

        private void AboutButton_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("ngClothesManager\nAuthor: Niklas Gschaider");
        }
        private void LogsButton_Click(object sender, RoutedEventArgs e) {
            logWindow = new LogWindow();
            logWindow.Show();
        }

        private void GenerateEmptySlotsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null;
        }
        private void GenerateEmptySlotsCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            GenerateEmptySlotsWindow win = new GenerateEmptySlotsWindow(Project);
            win.ShowDialog();
        }

        private void FindDuplicatesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null;
        }
        private void FindDuplicatesCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            if(IsSearchingForDuplicates) {
                MessageBox.Show("Already searching for duplicates. Watch the logs window for output!");
                return;
            }
            IsSearchingForDuplicates = true;

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (object sender2, DoWorkEventArgs e2) => {
                this.Dispatcher.Invoke(() => {
                    Logger.Log("Checking for duplicates.");
                });

                int duplicates = 0;

                foreach(Drawable a in Project.Drawables) {
                    foreach(Drawable b in Project.Drawables) {
                        if(a == b) {
                            continue;
                        }

                        string path1 = Project.FolderPath + "/" + a.ModelPath;
                        string path2 = Project.FolderPath + "/" + b.ModelPath;

                        if(a.Id < b.Id && Utils.IsFileEqual(path1, path2) && a.Gender == b.Gender) {
                            duplicates++;
                            this.Dispatcher.Invoke(() => {
                                Logger.Log("Duplicate: " + a.ModelPath + " (ID " + a.Id + ") | " + b.ModelPath + "(ID " + b.Id + ")");
                            });
                        }
                    }
                }

                this.Dispatcher.Invoke(() => {
                    Logger.Log("Checked for duplicates. Found: " + duplicates);
                });
                IsSearchingForDuplicates = false;
            };

            worker.RunWorkerAsync();
        }

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            if(!AskAndSaveIfNeeded()) {
                return;
            }
            NewProjectWindow window = new NewProjectWindow();

            if(window.ShowDialog() == true) {
                try {
                    Project = Project.Create(window.ProjectName, window.ProjectPath);
                } catch(Exception ex) {
                    Utils.HandleException(ex);
                }
            }
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            if(!AskAndSaveIfNeeded()) {
                return;
            }

            string fileName = AskForProjectFile();
            if(fileName.Length > 0) {
                try {
                    Project = Project.Open(fileName);
                } catch(Exception ex) {
                    Utils.HandleException(ex);
                }
            }
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null;
        }
        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            try {
                Project.Save();
            } catch(Exception ex) {
                Utils.HandleException(ex);
            }
        }

        private void CloseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null;
        }
        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            if(AskAndSaveIfNeeded()) {
                Project = null;
                Logger.Log("Project closed.");
            }
        }

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }
        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            if(AskAndSaveIfNeeded()) {
                Application.Current.Shutdown();
            }
        }

        private void AddMaleDrawablesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null;
        }
        private void AddMaleDrawablesCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            AddDrawables(Gender.Male);
        }

        private void AddFemaleDrawablesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null;
        }
        private void AddFemaleDrawablesCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            AddDrawables(Gender.Female);
        }

        private void ImportFromFivemCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null;
        }
        private void ImportFromFivemCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            if(Project == null) {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog {
                CheckFileExists = true,
                Filter = "Drawables geometry (*.ydd)|*.ydd",
                FilterIndex = 1,
                DefaultExt = "ydd",
                Multiselect = true,
                Title = "Adding drawables",
            };

            if(openFileDialog.ShowDialog() != true) {
                return;
            }

            DrawableImporter importer = new DrawableImporter(Project);
            foreach(string filePath in openFileDialog.FileNames) {
                importer.Import(filePath);
            }
        }

        private void RemoveSelectedDrawableCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null && SelectedDrawable != null;
        }
        private void RemoveSelectedDrawableCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            try {
                Project.RemoveDrawable(SelectedDrawable);
            } catch(Exception ex) {
                Utils.HandleException(ex);
            }
        }

        private void BuildProjectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null && Project.Drawables.Count > 0;
        }
        private void BuildProjectCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            ProjectBuildWindow = new ProjectBuildWindow(Project);
            ProjectBuildWindow.ShowDialog();
        }

        #endregion

        private string AskForProjectFile() {
            OpenFileDialog openFileDialog = new OpenFileDialog {
                CheckFileExists = true,
                Filter = "ngClothesManager Project (*.ngcmp)|*.ngcmp",
                FilterIndex = 1,
                DefaultExt = "ngcmp",
            };

            if(openFileDialog.ShowDialog() != true) {
                return "";
            }

            return openFileDialog.FileName;
        }
        private bool AskAndSaveIfNeeded() {
            if(Project != null && Project.NeedsSaving) {
                MessageBoxResult result = MessageBox.Show("Do you want to save the currently open project?", "Save?", MessageBoxButton.YesNoCancel);
                if(result == MessageBoxResult.Cancel) {
                    return false;
                } else if(result == MessageBoxResult.Yes) {
                    try {
                        Project.Save();
                    } catch(Exception ex) {
                        Utils.HandleException(ex);
                    }
                }
            }

            return true;
        }

        public void AddDrawables(Gender gender) {
            if(Project == null) {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog {
                CheckFileExists = true,
                Filter = "Drawables geometry (*.ydd)|*.ydd",
                FilterIndex = 1,
                DefaultExt = "ydd",
                Multiselect = true,
                Title = "Adding " + (gender == Gender.Male ? "male" : "female") + " drawables",
            };

            if(openFileDialog.ShowDialog() != true) {
                return;
            }

            DrawableImporter importer = new DrawableImporter(Project);
            foreach(string filePath in openFileDialog.FileNames) {
                importer.Import(filePath, gender);
            }
        }
    }
}

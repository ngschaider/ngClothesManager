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
            set {
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

        private ObservableCollection<DrawableListEntry> _drawablesList = new ObservableCollection<DrawableListEntry>();
        public ObservableCollection<DrawableListEntry> DrawablesList {
            get {
                return _drawablesList;
            }
            private set {
                _drawablesList = value;
                OnPropertyChanged(nameof(DrawablesList));
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public MainWindow() {
            InitializeComponent();
            this.DataContext = this;

            Logger.OnLogEntryAdded += OnLogEntryAdded;

        }

        private void OnDrawablesListChanged(object sender, NotifyCollectionChangedEventArgs e) {
            
        }

        private void OnDrawableChanged(object sender, PropertyChangedEventArgs e) {
            RefreshDrawablesList();
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
            DrawableListEntry entry = (DrawableListEntry)drawablesList.SelectedItem;
            if(entry != null && entry.Drawable != null) {
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

                        if(a.Index < b.Index && Utils.IsFileEqual(path1, path2)) {
                            duplicates++;
                            this.Dispatcher.Invoke(() => {
                                Logger.Log("Duplicate: " + a.ModelPath + " (ID " + a.Index + ") | " + b.ModelPath + "(ID " + b.Index + ")");
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
            AddDrawables(Sex.Male);
        }

        private void AddFemaleDrawablesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null;
        }
        private void AddFemaleDrawablesCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            AddDrawables(Sex.Female);
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
            ProjectBuildWindow = new ProjectBuildWindow();
            ProjectBuildWindow.Show();

            ProjectBuildWindow.OnExecuteBuild += (resType, outputFolder) => {
                ResourceBuilderBase builder;

                if(resType == ResourceType.FiveM) {
                    builder = new FivemResourceBuilder(Project, outputFolder);
                } else if(resType == ResourceType.AltV) {
                    builder = new AltvResourceBuilder(Project, outputFolder);
                } else {
                    builder = new SingleplayerResourceBuilder(Project, outputFolder);
                }

                builder.OutputName = Project.Name;

                builder.BuildResource();
                Logger.Log("Resource built!");
            };
        }

        #endregion

        private void RefreshDrawablesList() {
            DrawablesList.Clear();

            DrawableListEntry maleEntry = new DrawableListEntry() {
                Sex = Sex.Male,
            };
            DrawablesList.Add(maleEntry);

            DrawableListEntry femaleEntry = new DrawableListEntry() {
                Sex = Sex.Female,
            };
            DrawablesList.Add(femaleEntry);

            foreach(DrawableType drawableType in Enum.GetValues(typeof(DrawableType))) {
                Logger.Log("adding " + drawableType);
                maleEntry.Children.Add(new DrawableListEntry() {
                    DrawableType = drawableType,
                });
                femaleEntry.Children.Add(new DrawableListEntry() {
                    DrawableType = drawableType,
                });
            }

            foreach(DrawableListEntry sexEntry in DrawablesList) {
                foreach(Drawable drawable in Project.Drawables) {
                    if(drawable.IsForSex(sexEntry.Sex)) {
                        foreach(DrawableListEntry drawableEntry in sexEntry.Children) {
                            if(drawableEntry.DrawableType == drawable.DrawableType) {
                                drawableEntry.Children.Add(new DrawableListEntry() {
                                    Drawable = drawable
                                });
                            }
                        }
                    }
                }
            }

            Logger.Log("recreating list");
        }


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

        public void AddDrawables(Sex sex) {
            if(Project == null) {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog {
                CheckFileExists = true,
                Filter = "Drawables geometry (*.ydd)|*.ydd",
                FilterIndex = 1,
                DefaultExt = "ydd",
                Multiselect = true,
                Title = "Adding " + (sex == Sex.Male ? "male" : "female") + " drawables",
            };

            if(openFileDialog.ShowDialog() != true) {
                return;
            }

            DrawableImporter importer = new DrawableImporter(Project);
            foreach(string filePath in openFileDialog.FileNames) {
                importer.Import(filePath, sex);
            }
        }
    }
}

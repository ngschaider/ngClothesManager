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
using System.Windows.Controls;
using System.Windows.Input;

namespace ngClothesManager.App {
    public partial class MainWindow : Window, INotifyPropertyChanged {

        private ProjectBuildWindow ProjectBuildWindow;
        private LogWindow logWindow;

        private bool IsSearchingForDuplicates = false;

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

        public MainWindow() {
            InitializeComponent();
            this.DataContext = this;

            Logger.OnLogEntryAdded += OnLogEntryAdded;

            Project.PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
                if(e.PropertyName == nameof(Project.Clothes)) {
                    FillClothesList();
                    Project.Clothes.CollectionChanged += ClothesCollectionChanged;
                }
            };
        }

        private void FillClothesList() {
        
        }

        private void ClothesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            ClothesList.Clear();

            List<ClothListEntry> maleList = new List<ClothListEntry>();
            List<ClothListEntry> femaleList = new List<ClothListEntry>();
            foreach(DrawableType drawableType in Enum.GetValues(typeof(DrawableType)) {
                maleList.Add(new ClothListEntry() {
                    DrawableType = drawableType,
                });
                femaleList.Add(new ClothListEntry() {
                    DrawableType = drawableType,
                });
            }

            ClothesList.Add(new ClothListEntry() {
                Sex = Sex.Male,
                Children = new ObservableCollection<ClothListEntry>(maleList),
            });
            ClothesList.Add(new ClothListEntry() {
                Sex = Sex.Female,
                Children = new ObservableCollection<ClothListEntry>(femaleList),
            });

            foreach(Cloth cloth in Project.Clothes) {
                var sexEntry = ClothesList.Where(entry => cloth.TargetSex == Sex.Both || entry.Sex == cloth.TargetSex).First();
                var drawableEntry = sexEntry.Children.Where(entry => entry.DrawableType == cloth.DrawableType).First();
                drawableEntry.Children.Add(new ClothListEntry() {
                    Cloth = cloth,
                });
            }
        }

        private class ClothListEntry {
            public Cloth Cloth;
            public DrawableType DrawableType = DrawableType.None;
            public Sex Sex = Sex.None;

            public string Label {
                get {
                    return Cloth != null ? Cloth.DisplayName : (DrawableType != DrawableType.None ? DrawableType.ToIdentifier() : Sex.ToString());
                }
            }
            public ObservableCollection<ClothListEntry> Children = new ObservableCollection<ClothListEntry>();
        }

        private ObservableCollection<ClothListEntry> _clothesList = new ObservableCollection<ClothListEntry>();
        public ObservableCollection<ClothListEntry> ClothesList {
            get {
                return ClothesList;
            }
            set {
                ClothesList = value;
                OnPropertyChanged(nameof(ClothesList));
            }
        }


        private void OnLogEntryAdded(LogEntry log) {
            statusBarText.Text = log.Message;
        }

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

                foreach(Cloth a in Project.Clothes) {
                    foreach(Cloth b in Project.Clothes) {
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
            try {
                Project.RemoveCloth(SelectedCloth);
            } catch(Exception ex) {
                Utils.HandleException(ex);
            }
        }

        private void BuildProjectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Project != null && Project.Clothes.Count > 0;
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
    }
}

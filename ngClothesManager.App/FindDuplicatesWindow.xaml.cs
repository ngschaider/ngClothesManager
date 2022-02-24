using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;

namespace ngClothesManager.App {
    /// <summary>
    /// Interaktionslogik für FindDuplicatesWindow.xaml
    /// </summary>
    public partial class FindDuplicatesWindow : Window, INotifyPropertyChanged {

        public class FindDuplicatesEntry : INotifyPropertyChanged {
            private string _leftPath;
            public string LeftPath {
                get {
                    return _leftPath;
                }
                set {
                    _leftPath = value;
                    OnPropertyChanged(nameof(LeftPath));
                }
            }

            private string _rightPath;
            public string RightPath {
                get {
                    return _rightPath;
                }
                set {
                    _rightPath = value;
                    OnPropertyChanged(nameof(RightPath));
                }
            }

            private string _status;
            public string Status {
                get {
                    return _status;
                }
                set {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string propertyName) {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private readonly Project project;

        private ObservableCollection<FindDuplicatesEntry> _list;
        public ObservableCollection<FindDuplicatesEntry> List {
            get {
                return _list;
            }
            set {
                _list = value;
                OnPropertyChanged(nameof(List));
            }
        }

        public FindDuplicatesEntry Selected;

        public FindDuplicatesWindow(Project project) {
            InitializeComponent();

            this.project = project;

            //PopulateCompareList();
            //Compare();
        }

        private void PopulateCompareList() {
            List = new ObservableCollection<FindDuplicatesEntry>();

            BackgroundWorker backgroundWorker = new BackgroundWorker {
                WorkerReportsProgress = true,
            };

            backgroundWorker.DoWork += (object sender, DoWorkEventArgs e) => {
                int index = 0;
                foreach(Drawable a in project.Drawables) {
                    foreach(Drawable b in project.Drawables) {                        
                        if(a == b) {
                            continue;
                        }

                        List.Add(new FindDuplicatesEntry() {
                            LeftPath = a.ModelPath,
                            RightPath = b.ModelPath,
                            Status = "",
                        });

                        index++;
                        backgroundWorker.ReportProgress(index / project.Drawables.Count * 100);
                    }
                }
            };

            backgroundWorker.ProgressChanged += (object sender, ProgressChangedEventArgs e) => {
                ProgressPercentage = e.ProgressPercentage;
            };

            backgroundWorker.RunWorkerAsync();
        }

        private int _progressPercentage;
        public int ProgressPercentage {
            get {
                return _progressPercentage;
            }
            set {
                _progressPercentage = value;
                OnPropertyChanged(nameof(ProgressPercentage));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Compare() {
            BackgroundWorker backgroundWorker = new BackgroundWorker {
                WorkerReportsProgress = true,
            };

            backgroundWorker.DoWork += (object sender, DoWorkEventArgs e) => {
                int index = 0;
                foreach(FindDuplicatesEntry entry in List) {
                    index++;

                    if(AreFileEquivalent(entry.LeftPath, entry.RightPath)) {
                        entry.Status = "Duplicate";
                    } else {
                        entry.Status = "-";
                    }

                    backgroundWorker.ReportProgress(index / List.Count * 100);
                }
            };

            backgroundWorker.ProgressChanged += (object sender, ProgressChangedEventArgs e) => {
                ProgressPercentage = e.ProgressPercentage;
            };

            backgroundWorker.RunWorkerAsync();
        }

        private bool AreFileEquivalent(string relPath1, string relPath2) {
            return true;
        }

    }
}

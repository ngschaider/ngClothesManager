using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Hellosam.Net.Collections;

namespace ngClothesManager.App {
    /// <summary>
    /// Interaktionslogik für GenerateEmptySlotsWindow.xaml
    /// </summary>
    public partial class GenerateEmptySlotsWindow : Window, INotifyPropertyChanged {

        private readonly Project project;

        public GenerateEmptySlotsWindow(Project project) {
            InitializeComponent();
            this.project = project;
            this.DataContext = this;
        }

        private bool _isMaleChecked = true;
        public bool IsMaleChecked {
            get {
                return _isMaleChecked;
            }
            set {
                _isMaleChecked = value;
                OnPropertyChanged(nameof(IsMaleChecked));
            }
        }
        private bool _isFemaleChecked = true;
        public bool IsFemaleChecked {
            get {
                return _isFemaleChecked;
            }
            set {
                _isFemaleChecked = value;
                OnPropertyChanged(nameof(IsFemaleChecked));
            }
        }

        private int _numComponents;
        public int NumComponents {
            get {
                return _numComponents;
            }
            set {
                _numComponents = value;
                OnPropertyChanged(nameof(NumComponents));
            }
        }

        private int _numProps;
        public int NumProps {
            get {
                return _numProps;
            }
            set {
                _numProps = value;
                OnPropertyChanged(nameof(NumProps));
            }
        }

        private int _numTextures;
        public int NumTextures {
            get {
                return _numTextures;
            }
            set {
                _numTextures = value;
                OnPropertyChanged(nameof(NumTextures));
            }
        }

        private ObservableDictionary<DrawableType, bool> _enabledTypes = new ObservableDictionary<DrawableType, bool>() {
            { DrawableType.Head, true },
            { DrawableType.Mask, true },
            { DrawableType.Hair, true },
            { DrawableType.Body, true },
            { DrawableType.Legs, true },
            { DrawableType.Bag, true },
            { DrawableType.Shoes, true },
            { DrawableType.Accessories, true },
            { DrawableType.Undershirt, true },
            { DrawableType.Armor, true },
            { DrawableType.Decal, true },
            { DrawableType.Top, true },
            { DrawableType.PropHead, true },
            { DrawableType.PropEyes, true },
            { DrawableType.PropEars, true },
            { DrawableType.PropMouth, true },
            { DrawableType.PropLHand, true },
            { DrawableType.PropRHand, true },
            { DrawableType.PropLWrist, true },
            { DrawableType.PropRWrist, true },
            { DrawableType.PropHip, true },
            { DrawableType.PropLFoot, true },
            { DrawableType.PropRFoot, true },
            { DrawableType.PropUnk1, true },
            { DrawableType.PropUnk2, true },
        };

        public ObservableDictionary<DrawableType, bool> EnabledTypes {
            get {
                return _enabledTypes;
            }
            set {
                _enabledTypes = value;
                OnPropertyChanged(nameof(EnabledTypes));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isButtonEnabled = true;

        public bool IsButtonEnabled {
            get {
                return _isButtonEnabled;
            }
            set {
                _isButtonEnabled = value;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public void GenerateEmptySlots(object sender1, RoutedEventArgs e1) {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (object sender2, DoWorkEventArgs e2) => {
                foreach(Gender gender in Enum.GetValues(typeof(Gender))) {
                    foreach(DrawableType type in Enum.GetValues(typeof(DrawableType))) {
                        if(EnabledTypes[type]) {
                            int currentCount = this.project.Drawables.Where(drawable => drawable.DrawableType == type && drawable.Gender == gender).Count();

                            int numSlots = type.IsComponent() ? NumComponents : NumProps;
                            int needToAdd = Math.Max(0, numSlots - currentCount);

                            for(int i = 0; i < needToAdd; i++) {
                                Drawable drawable = new Drawable(project.GetEmptyDrawableId(), type, gender);
                                this.Dispatcher.Invoke(() => {
                                    Logger.Log("Adding empty drawable to " + type.ToIdentifier());
                                    project.Drawables.Add(drawable);
                                    project.AddFileFromByteArray(Properties.Resources.modelDummy, drawable.ModelPath);
                                });
                            }
                        }
                    }
                }

                foreach(Drawable drawable in project.Drawables) {
                    bool male = drawable.Gender == Gender.Male && IsMaleChecked;
                    bool female = drawable.Gender == Gender.Female && IsFemaleChecked;
                    if(EnabledTypes[drawable.DrawableType] && (male || female)) {
                        int needToAdd = NumTextures - drawable.Textures.Count;
                        for(int i = 0; i < needToAdd; i++) {
                            int textureId = drawable.GetEmptyTextureIndex();

                            this.Dispatcher.Invoke(() => {
                                Logger.Log("Adding empty texture " + textureId + " to " + drawable.DrawableType.ToIdentifier() + " " + drawable.Id);
                                drawable.Textures.Add(new Texture(textureId));
                                project.AddFileFromByteArray(Properties.Resources.texDummy, drawable.GetTexturePath(textureId));
                            });
                        }
                    }
                }

                this.Dispatcher.Invoke(() => {
                    Close();
                });
            };

            IsButtonEnabled = false;
            worker.RunWorkerAsync();
        }
    }
}

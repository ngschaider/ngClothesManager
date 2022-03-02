using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngClothesManager.App {
    public class DrawableList /*: INotifyPropertyChanged*/ {

        /*public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }*/

        public DrawableList() {
        }

        public void OnProjectChanged(Project project) {
            if(project == null) {
                List.Clear();
            } else {
                RebuildList(project);
                project.Drawables.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => {
                    OnCollectionChanged(project, e);
                };
            }
        }

        private void OnCollectionChanged(Project project, NotifyCollectionChangedEventArgs e) {
            if(e.Action == NotifyCollectionChangedAction.Add) {
                foreach(Drawable drawable in e.NewItems) {
                    AddDrawable(drawable);
                }
            } else if(e.Action == NotifyCollectionChangedAction.Move) {

            } else if(e.Action == NotifyCollectionChangedAction.Remove) {
                foreach(Drawable drawable in e.OldItems) {
                    RemoveDrawable(drawable);
                }
            } else if(e.Action == NotifyCollectionChangedAction.Replace) {
                foreach(Drawable drawable in e.OldItems) {
                    RemoveDrawable(drawable);
                }
                foreach(Drawable drawable in e.NewItems) {
                    AddDrawable(drawable);
                }
            } else if(e.Action == NotifyCollectionChangedAction.Reset) {
                RebuildList(project);
            }
        }

        private void OnDrawableChanged(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == nameof(Drawable.Gender) || e.PropertyName == nameof(Drawable.DrawableType)) {
                Drawable drawable = (Drawable)sender;
                RemoveDrawable(drawable);
                AddDrawable(drawable);
            }
        }

        private void RemoveDrawable(Drawable drawable) {
            for(int a = List.Count - 1; a >= 0; a--) {
                for(int b = List[a].Children.Count - 1; b >= 0; b--) {
                    for(int c = List[a].Children[b].Children.Count - 1; c >= 0; c--) {
                        if(List[a].Children[b].Children[c].Drawable.Id == drawable.Id) {
                            List[a].Children[b].Children.RemoveAt(c);
                        }
                    }
                }
            }
        }

        private void AddDrawable(Drawable drawable) {
            DrawableListEntry genderEntry = List.Where(e => e.Gender == drawable.Gender).First();
            DrawableListEntry drawableTypeEntry = genderEntry.Children.Where(e => e.DrawableType == drawable.DrawableType).First();
            drawableTypeEntry.Children.Add(new DrawableListEntry() {
                Type = DrawableListEntry.EntryType.Drawable,
                Drawable = drawable,
            });
        }

        public ObservableCollection<DrawableListEntry> List { get; } = new ObservableCollection<DrawableListEntry>();

        private void RebuildList(Project project) {
            List.Clear();

            foreach(Gender gender in Enum.GetValues(typeof(Gender))) {
                DrawableListEntry genderEntry = new DrawableListEntry() {
                    Type = DrawableListEntry.EntryType.Gender,
                    Gender = gender,
                };

                foreach(DrawableType drawableType in Enum.GetValues(typeof(DrawableType))) {
                    DrawableListEntry drawableTypeEntry = new DrawableListEntry() {
                        Type = DrawableListEntry.EntryType.DrawableType,
                        DrawableType = drawableType,
                    };

                    genderEntry.Children.Add(drawableTypeEntry);
                }

                List.Add(genderEntry);
            }

            foreach(Drawable drawable in project.Drawables) {
                AddDrawable(drawable);
                drawable.PropertyChanged += OnDrawableChanged;
            }
        }

    }
}

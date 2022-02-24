using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ngClothesManager.App {
    public static class Commands {

        public static readonly RoutedUICommand Exit = new RoutedUICommand("Exit", "Exit", typeof(Commands), new InputGestureCollection() {
            new KeyGesture(Key.F4, ModifierKeys.Alt)
        });

        public static readonly RoutedUICommand AddMaleDrawables = new RoutedUICommand("Add Male Drawables", "AddMaleDrawables", typeof(Commands), new InputGestureCollection() {
            new KeyGesture(Key.M, ModifierKeys.Control)
        });

        public static readonly RoutedUICommand AddFemaleDrawables = new RoutedUICommand("Add Female Drawables", "AddFemaleDrawables", typeof(Commands), new InputGestureCollection() {
            new KeyGesture(Key.F, ModifierKeys.Control)
        });

        public static readonly RoutedUICommand RemoveSelectedDrawable = new RoutedUICommand("Remove Selected Drawables", "RemoveSelectedDrawable", typeof(Commands), new InputGestureCollection() {
            new KeyGesture(Key.D, ModifierKeys.Control)
        });

        public static readonly RoutedUICommand FindDuplicates = new RoutedUICommand("Find Duplicates", "FindDuplicates", typeof(Commands), new InputGestureCollection() {
            new KeyGesture(Key.F, ModifierKeys.Control | ModifierKeys.Shift)
        });

        public static readonly RoutedUICommand BuildProject = new RoutedUICommand("Build Project", "BuildProject", typeof(Commands), new InputGestureCollection() {
            new KeyGesture(Key.B, ModifierKeys.Control | ModifierKeys.Shift)
        });

    }
}

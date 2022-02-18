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

        public static readonly RoutedUICommand AddMaleClothes = new RoutedUICommand("Add Male Clothes", "AddMaleClothes", typeof(Commands), new InputGestureCollection() {
            new KeyGesture(Key.M, ModifierKeys.Control)
        });

        public static readonly RoutedUICommand AddFemaleClothes = new RoutedUICommand("Add Female Clothes", "AddFemaleClothes", typeof(Commands), new InputGestureCollection() {
            new KeyGesture(Key.F, ModifierKeys.Control)
        });

        public static readonly RoutedUICommand RemoveSelectedCloth = new RoutedUICommand("Remove Selected Cloth", "RemoveSelectedCloth", typeof(Commands), new InputGestureCollection() {
            new KeyGesture(Key.D, ModifierKeys.Control)
        });

    }
}

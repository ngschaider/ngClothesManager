using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestApp {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        public enum SexType { 
            Male, 
            Female 
        };

        public enum Drawable {
            jbib,
            berd,
            hair,
            top,
            task,
            teef
        }

        public class User {
            public string Name {
                get; set;
            }

            public int Age {
                get; set;
            }

            public string Mail {
                get; set;
            }

            public SexType Sex {
                get; set;
            }

            public Drawable Drawable {
                get; set;
            }
        }

    }
}

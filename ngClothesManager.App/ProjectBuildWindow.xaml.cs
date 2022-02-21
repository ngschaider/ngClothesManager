﻿using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace ngClothesManager.App {
    public partial class ProjectBuildWindow : Window {

        public string OutputFolder = "";

        public ProjectBuildWindow() {
            InitializeComponent();
        }

        public static Action<ResourceType, string> OnExecuteBuild;

        private void BuildButton_Click(object sender, RoutedEventArgs e) {
            ResourceType resType = ResourceType.AltV;

            if(isSinglePlayerRadio.IsChecked == true) {
                resType = ResourceType.Single;
            } else if(isFivemResourceRadio.IsChecked == true) {
                resType = ResourceType.FiveM;
            }

            if(FilePathHasInvalidChars(OutputFolder)) {
                MessageBox.Show("Output folder path contains invalid characters.\nPlease choose another output location.");
                Logger.Log("Error: Invalid build output folder.");
                return;
            }

            OnExecuteBuild?.Invoke(resType, OutputFolder);
        }

        private bool FilePathHasInvalidChars(string path) {
            bool ret = false;
            if(!string.IsNullOrEmpty(path)) {
                try {
                    string fileName = System.IO.Path.GetFileName(path);
                    string fileDirectory = System.IO.Path.GetDirectoryName(path);
                } catch(ArgumentException) {
                    ret = true;
                }
            }
            return ret;
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e) {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if(!dialog.ShowDialog(this).GetValueOrDefault()) {
                return;
            }

            OutputFolder = dialog.SelectedPath;
            outFolderPathText.Content = OutputFolder;
        }

        private void ValidationTextBox(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex(@"^[A-Za-z_]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void CollectionNameText_PreviewKeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.Space) {
                e.Handled = true;
            }
        }
    }
}

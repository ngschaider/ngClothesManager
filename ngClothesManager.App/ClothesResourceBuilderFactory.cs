using System;
using System.Diagnostics;
using System.Windows;
using ngClothesManager.App.Builders;
using Ookii.Dialogs.Wpf;

namespace ngClothesManager.App {
    internal class ClothesResourceBuilderFactory {
        public void BuildResource(TargetResourceType resourceType, Project project, string outputFolder, string collectionName) {
            try {
                switch(resourceType) {
                    case TargetResourceType.AltV:
                        new AltvResourceBuilder().BuildResource(project, outputFolder, collectionName);
                        MessageBox.Show("alt:V Resource built!");
                        break;

                    case TargetResourceType.Single:
                        new SingleplayerResourceBuilder().BuildResource(project, outputFolder, collectionName);
                        MessageBox.Show("Singleplayer Resource built!");
                        break;

                    case TargetResourceType.FiveM:
                        new FivemResourceBuilder().BuildResource(project, outputFolder, collectionName);
                        MessageBox.Show("FiveM Resource built!");
                        break;
                }
            } catch(Exception exception) {
                ShowExceptionErrorDialog(exception);
            }
        }

        private void ShowExceptionErrorDialog(Exception exception) {
            var reportErrorButton = new TaskDialogButton("Report error");
            var taskDialog = new TaskDialog {
                WindowTitle = "",
                CollapsedControlText = "See error details",
                ExpandedControlText = "Close error details",
                Content = "Building cloth resource failed. Please report this error at https://github.com/DurtyFree/altv-cloth-tool",
                Buttons =
                {
                    reportErrorButton,
                    new TaskDialogButton("Close")
                    {
                        ButtonType = ButtonType.Close
                    },
                },
                ExpandedInformation = exception.ToString(),
                MainIcon = TaskDialogIcon.Error,
                MainInstruction = "Unknown error occured"
            };
            var pressedButton = taskDialog.ShowDialog();
            if(pressedButton == reportErrorButton) {
                var issueBody = "I have the following error:\n" + exception +
                                "\n\nCloth files: [Please provide cloth files (ydd, ytd) and cloth project file here]";
                var issueTitle = "Exception error";
                Process.Start($"https://github.com/DurtyFree/altv-cloth-tool/issues/new?body={Uri.EscapeDataString(issueBody)}&title={Uri.EscapeDataString(issueTitle)}");
            }
        }
    }
}

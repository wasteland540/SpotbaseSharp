using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity;
using Microsoft.Win32;
using SpotbaseSharp.Messages;
using SpotbaseSharp.ViewModels;

namespace SpotbaseSharp.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IMessenger _messenger;

        public MainWindow(IMessenger messenger)
        {
            InitializeComponent();

            _messenger = messenger;
            _messenger.Register<ChooseFilesRequestMsg>(this, OnChooseFilesRequestMsg);
            _messenger.Register<ImportRequestMsg>(this, OnImportRequestMsg);
            _messenger.Register<ImportSuccessMsg>(this, OnImportSuccessMsg);
            _messenger.Register<ImportFaildMsg>(this, OnImportFaildMsg);
            _messenger.Register<HelpFilterMsg>(this, OnHelpFilterMsg);
        }

        [Dependency]
        public MainWindowViewModel ViewModel
        {
            set { DataContext = value; }
        }

        #region Private Methods

        private void WebBrowser_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            const string script = "document.body.style.overflow ='hidden'";
            var wb = (WebBrowser) sender;
            wb.InvokeScript("execScript", new Object[] {script, "JavaScript"});
        }

        private void OnChooseFilesRequestMsg(ChooseFilesRequestMsg msg)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter =
                    "JPG Files (.jpg)|*.jpg|JPEG Files (.jpeg)|*.jpeg|PNG Files (.png)|*.png"
            };
            bool? dialogResult = openFileDialog.ShowDialog();

            if (dialogResult != null && dialogResult.Value)
            {
                string[] filenames = openFileDialog.FileNames;

                MessageBoxResult msgBoxResult = MessageBox.Show("Do you want to keep a copy of the full size image?",
                    "Full Size Image", MessageBoxButton.YesNo, MessageBoxImage.Question);

                _messenger.Send(new ChooseFilesConfirmMsg(filenames, msgBoxResult == MessageBoxResult.Yes));
            }
        }

        private void OnImportRequestMsg(ImportRequestMsg msg)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter =
                    "SpotbaseSharp Import File (.sbsif)|*.sbsif"
            };
            bool? dialogResult = openFileDialog.ShowDialog();

            if (dialogResult != null && dialogResult.Value)
            {
                string filename = openFileDialog.FileName;

                MessageBoxResult msgBoxResult =
                    MessageBox.Show("Do you want to keep a copy of the full size image (if available)?",
                        "Full Size Image", MessageBoxButton.YesNo, MessageBoxImage.Question);

                _messenger.Send(new ImportConfirmMsg(filename, msgBoxResult == MessageBoxResult.Yes));
            }
        }

        private void OnImportSuccessMsg(ImportSuccessMsg msg)
        {
            MessageBox.Show("Import successful.",
                "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnImportFaildMsg(ImportFaildMsg msg)
        {
            MessageBox.Show("Import faild!",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnHelpFilterMsg(HelpFilterMsg msg)
        {
            MessageBox.Show(
                "You can only filter the spots by one out of the three categories: \n\n - by Name\n - by City" +
                "\n - by Type (multiselection allowed)\n\nYou can't combine the filters, because i didn't" +
                " implement that functionality! Maybe in a future version.",
                "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion Private Methods
    }
}
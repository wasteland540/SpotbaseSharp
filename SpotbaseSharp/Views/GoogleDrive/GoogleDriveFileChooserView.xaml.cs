using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity;
using SpotbaseSharp.Messages;
using SpotbaseSharp.ViewModels;

namespace SpotbaseSharp.Views.GoogleDrive
{
    /// <summary>
    ///     Interaction logic for GoogleDriveFileChooserView.xaml
    /// </summary>
    public partial class GoogleDriveFileChooserView
    {
        public GoogleDriveFileChooserView(IMessenger messenger)
        {
            InitializeComponent();

            messenger.Register<GoogleDriveFileChooserViewCancelMsg>(this, OnGoogleDriveFileChooserViewCancelMsg);
        }

        [Dependency]
        public GoogleDriveFileChooserViewModel ViewModel
        {
            set { DataContext = value; }
        }

        #region Private Methods

        private void OnGoogleDriveFileChooserViewCancelMsg(GoogleDriveFileChooserViewCancelMsg msg)
        {
            Close();
        }

        #endregion Private Methods
    }
}
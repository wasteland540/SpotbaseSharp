using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity;
using Microsoft.Win32;
using SpotbaseSharp.Messages;
using SpotbaseSharp.ViewModels;

namespace SpotbaseSharp.Views
{
    /// <summary>
    ///     Interaction logic for ExportView.xaml
    /// </summary>
    public partial class ExportView : ICloseable
    {
        private readonly IMessenger _messenger;

        public ExportView(IMessenger messenger)
        {
            InitializeComponent();

            _messenger = messenger;
            _messenger.Register<ChooseExportPathRequestMsg>(this, OnChooseExportPathRequestMsg);
            _messenger.Register<ExportViewCancelMsg>(this, OnExportViewCancelMsg);
            _messenger.Register<ExportSuccessMsg>(this, OnExportSuccessMsg);
            _messenger.Register<ExportFaildMsg>(this, OnExportFaildMsg);
        }

        [Dependency]
        public ExportViewModel ViewModel
        {
            set { DataContext = value; }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            //unregister messages
            _messenger.Unregister<ChooseExportPathRequestMsg>(this, OnChooseExportPathRequestMsg);
            _messenger.Unregister<ExportSuccessMsg>(this, OnExportSuccessMsg);
            _messenger.Unregister<ExportFaildMsg>(this, OnExportFaildMsg);

            //send close msg
            _messenger.Send(new ExportViewClosedMsg());
        }

        #region Private Methods

        private void OnChooseExportPathRequestMsg(ChooseExportPathRequestMsg msg)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter =
                    "SpotbaseSharp Import File (.sbsif)|*.sbsif"
            };
            bool? dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult != null && dialogResult.Value)
            {
                string filename = saveFileDialog.FileName;
                _messenger.Send(new ChooseExportPatConfirmMsg(filename));
            }
        }

        private void OnExportViewCancelMsg(ExportViewCancelMsg obj)
        {
            Close();
        }

        private void OnExportSuccessMsg(ExportSuccessMsg msg)
        {
            MessageBox.Show(@"Export successful!", @"Hint", MessageBoxButton.OK, MessageBoxImage.Information);

            Close();
        }

        private void OnExportFaildMsg(ExportFaildMsg msg)
        {
            MessageBox.Show("Export faild!\nError: " + msg.ErrorMsg, @"Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion Private Methos
    }
}
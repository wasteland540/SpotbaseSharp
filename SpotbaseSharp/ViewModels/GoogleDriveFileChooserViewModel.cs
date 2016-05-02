using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using Google.Apis.Drive.v3.Data;
using SpotbaseSharp.Commands;
using SpotbaseSharp.Messages;
using SpotbaseSharp.Services;

namespace SpotbaseSharp.ViewModels
{
    public class GoogleDriveFileChooserViewModel : ViewModelBase
    {
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMessenger _messenger;
        private ICommand _cancelCommand;
        private List<File> _fileList;
        private ICommand _okCommand;
        private bool _saveLargeFile;

        public GoogleDriveFileChooserViewModel(IMessenger messenger, IGoogleDriveService googleDriveService)
        {
            _messenger = messenger;
            _googleDriveService = googleDriveService;
        }

        #region Properties

        public ICommand CancelCommand
        {
            get
            {
                _cancelCommand = _cancelCommand ?? new DelegateCommand(Cancel);
                return _cancelCommand;
            }
        }

        public ICommand OkCommand
        {
            get
            {
                _okCommand = _okCommand ?? new DelegateCommand(Ok);
                return _okCommand;
            }
        }

        public List<File> FileList
        {
            get
            {
                if (_fileList == null)
                {
                    File folder = _googleDriveService.GetFolder();
                    _fileList = _googleDriveService.GetFiles(folder.Id);
                }

                return _fileList;
            }
            set
            {
                _fileList = value;
                RaisePropertyChanged(() => FileList);
            }
        }

        public bool SaveLargeFile
        {
            get { return _saveLargeFile; }
            set
            {
                _saveLargeFile = value;
                RaisePropertyChanged(() => SaveLargeFile);
            }
        }

        #endregion Properties

        #region Private Methods

        private void Cancel(object obj)
        {
            _messenger.Send(new GoogleDriveFileChooserViewCancelMsg());
        }

        private void Ok(object obj)
        {
            var items = (IList) obj;

            if (items != null)
            {
                IEnumerable<File> selectedFiles = items.Cast<File>();
                string[] fileIds = selectedFiles.Select(f => f.Id).ToArray();
                
                _messenger.Send(new GoogleDriveFileChooserViewCancelMsg()); //for closing this view
                _messenger.Send(new ChooseFilesGoogleDriveConfirmMsg(fileIds, _saveLargeFile));
            }
        }

        #endregion Private Methods
    }
}
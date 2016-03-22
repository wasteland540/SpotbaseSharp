using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using SpotbaseSharp.Commands;
using SpotbaseSharp.Messages;
using SpotbaseSharp.Model;
using SpotbaseSharp.Services;

namespace SpotbaseSharp.ViewModels
{
    public class ExportViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly IDatabaseService _databaseService;
        private readonly IMessenger _messenger;
        private ICommand _cancelCommand;
        private ICommand _choosePathCommand;
        private ICommand _exportCommand;
        private string _exportPath;
        private SelectableSpot _selectedSpot;
        private ObservableCollection<SelectableSpot> _spots;

        public ExportViewModel(IDatabaseService databaseService, IDataService dataService, IMessenger messenger)
        {
            _databaseService = databaseService;
            _dataService = dataService;
            _messenger = messenger;

            _messenger.Register<ChooseExportPatConfirmMsg>(this, OnChooseExportPatConfirmMsg);
        }

        #region Properties

        public ObservableCollection<SelectableSpot> Spots
        {
            get
            {
                if (_spots == null)
                {
                    List<Spot> tmp = _databaseService.GetSpots();
                    _spots = new ObservableCollection<SelectableSpot>();

                    foreach (Spot spot in tmp)
                    {
                        _spots.Add(new SelectableSpot(spot));
                    }
                }

                return _spots;
            }

            set
            {
                if (value != null && value != _spots)
                {
                    _spots = value;
                    RaisePropertyChanged(() => Spots);
                }
            }
        }

        public SelectableSpot SelectedSpot
        {
            get { return _selectedSpot; }
            set
            {
                _selectedSpot = value;
                _selectedSpot.Selected = !_selectedSpot.Selected;

                RaisePropertyChanged(() => SelectedSpot);
            }
        }

        public ICommand ExportCommand
        {
            get { return _exportCommand = _exportCommand ?? new DelegateCommand(Export); }
        }

        public string ExportPath
        {
            get { return _exportPath; }
            set
            {
                _exportPath = value;
                RaisePropertyChanged(() => ExportPath);
            }
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand = _cancelCommand ?? new DelegateCommand(Cancel); }
        }

        public ICommand ChoosePathCommand
        {
            get { return _choosePathCommand = _choosePathCommand ?? new DelegateCommand(Choose); }
        }

        #endregion Properties

        #region Private Methods

        private void Cancel(object obj)
        {
            _messenger.Send(new ExportViewCancelMsg());
        }

        private void Choose(object obj)
        {
            _messenger.Send(new ChooseExportPathRequestMsg());
        }

        private void OnChooseExportPatConfirmMsg(ChooseExportPatConfirmMsg msg)
        {
            ExportPath = msg.Filename;
        }

        private void Export(object obj)
        {
            if (!string.IsNullOrEmpty(_exportPath))
            {
                Mouse.OverrideCursor = Cursors.Wait;

                List<SelectableSpot> selectedSpots = _spots.Where(s => s.Selected).ToList();

                string errorMsg;
                bool exported = _dataService.Export(_exportPath, selectedSpots, out errorMsg);

                Mouse.OverrideCursor = null;

                if (exported)
                {
                    _messenger.Send(new ExportSuccessMsg());
                }
                else
                {
                    _messenger.Send(new ExportFaildMsg(errorMsg));
                }
            }
        }

        #endregion Private Methods
    }
}
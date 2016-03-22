using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity;
using SpotbaseSharp.Commands;
using SpotbaseSharp.Messages;
using SpotbaseSharp.Model;
using SpotbaseSharp.Services;
using SpotbaseSharp.Views;

namespace SpotbaseSharp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const string LocationUrlTemplate =
            "https://maps.googleapis.com/maps/api/staticmap?size=500x400&maptype=roadmap&markers=color:blue%7C{0}, {1}&key=";

        private const string RouteUrlTemplate = "https://www.google.com/maps/dir/Current+Location/{0}, {1}";
        private readonly IDataService _dataService;

        private readonly IDatabaseService _databaseService;
        private readonly IImageService _imageService;
        private readonly IMessenger _messenger;
        private ICommand _addNewCommand;
        private ObservableCollection<string> _cities;
        private ObservableCollection<string> _citiesForAutocomplete;
        private ICommand _clearFilterCommand;
        private bool _creativeChecked;
        private bool _curbChecked;
        private ICommand _deleteSpotCommand;
        private ICommand _exitCommand;
        private ICommand _exportCommand;
        private ExportView _exportView;
        private bool _gapChecked;
        private bool _hasLargeImage;
        private ICommand _helpFilterCommand;
        private ICommand _importCommand;
        private bool _ledgeChecked;
        private string _locationUrl;
        private bool _notSetChecked;
        private ICommand _openImageCommand;
        private ICommand _openMapCommand;
        private bool _parkChecked;
        private bool _railChecked;
        private ICommand _saveCommand;
        private ICommand _searchByNameCommand;
        private string _searchValue;
        private string _selectedCity;
        private Spot _selectedSpot;
        private ObservableCollection<Spot> _spots;

        public MainWindowViewModel(IDatabaseService databaseService, IMessenger messenger, IImageService imageService,
            IDataService dataService)
        {
            _databaseService = databaseService;
            _messenger = messenger;
            _imageService = imageService;
            _dataService = dataService;

            //register for messages
            _messenger.Register<ChooseFilesConfirmMsg>(this, OnChooseFilesConfirmMsg);
            _messenger.Register<ImportConfirmMsg>(this, OnImportConfirmMsg);
            _messenger.Register<ExportViewClosedMsg>(this, OnExportviewClosedMsg);
        }

        #region Properties

        public ObservableCollection<Spot> Spots
        {
            get { return _spots ?? (_spots = new ObservableCollection<Spot>(_databaseService.GetSpots())); }

            set
            {
                if (value != null && value != _spots)
                {
                    _spots = value;
                    RaisePropertyChanged(() => Spots);
                }
            }
        }

        public Spot SelectedSpot
        {
            get { return _selectedSpot; }
            set
            {
                if (value != null && value != _selectedSpot)
                {
                    _selectedSpot = value;
                    RaisePropertyChanged(() => SelectedSpot);

                    LocationUrl = string.Format(LocationUrlTemplate,
                        _selectedSpot.Lat.ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                        _selectedSpot.Lng.ToString(CultureInfo.InvariantCulture).Replace(",", "."));

                    HasLargeImage = _selectedSpot.LargeFile != Guid.Empty;
                }
            }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand = _saveCommand ?? new DelegateCommand(Save); }
        }

        public ObservableCollection<string> Cities
        {
            get
            {
                if (_cities == null)
                {
                    _cities = new ObservableCollection<string>(_databaseService.GetCitiesDistinct());
                }

                _cities.Insert(0, "");

                return _cities;
            }

            set
            {
                if (value != null && value != _cities)
                {
                    _cities = value;
                    RaisePropertyChanged(() => Cities);
                }
            }
        }

        public ObservableCollection<string> CitiesForAutocomplete
        {
            get
            {
                return _citiesForAutocomplete ??
                       (_citiesForAutocomplete = new ObservableCollection<string>(_databaseService.GetCitiesDistinct()));
            }

            set
            {
                if (value != null && value != _citiesForAutocomplete)
                {
                    _citiesForAutocomplete = value;
                    RaisePropertyChanged(() => CitiesForAutocomplete);
                }
            }
        }

        public string SearchValue
        {
            get { return _searchValue; }
            set
            {
                _searchValue = value;
                RaisePropertyChanged(() => SearchValue);
            }
        }

        public string SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                _selectedCity = value;
                RaisePropertyChanged(() => SelectedCity);

                Spots = !string.IsNullOrEmpty(_selectedCity)
                    ? new ObservableCollection<Spot>(_databaseService.GetSpotsByCity(_selectedCity))
                    : new ObservableCollection<Spot>(_databaseService.GetSpots());
            }
        }

        public ICommand AddNewCommand
        {
            get { return _addNewCommand = _addNewCommand ?? new DelegateCommand(AddNew); }
        }

        public string LocationUrl
        {
            get { return _locationUrl; }
            set
            {
                _locationUrl = value;
                RaisePropertyChanged(() => LocationUrl);
            }
        }

        public ICommand DeleteSpotCommand
        {
            get { return _deleteSpotCommand = _deleteSpotCommand ?? new DelegateCommand(DeleteSpot); }
        }

        public ICommand OpenMapCommand
        {
            get { return _openMapCommand = _openMapCommand ?? new DelegateCommand(OpenMap); }
        }

        public ICommand OpenImageCommand
        {
            get { return _openImageCommand = _openImageCommand ?? new DelegateCommand(OpenImage); }
        }

        public bool HasLargeImage
        {
            get { return _hasLargeImage; }
            set
            {
                _hasLargeImage = value;
                RaisePropertyChanged(() => HasLargeImage);
            }
        }

        public ICommand ExitCommand
        {
            get { return _exitCommand = _exitCommand ?? new DelegateCommand(Exit); }
        }

        public ICommand SearchByNameCommand
        {
            get { return _searchByNameCommand = _searchByNameCommand ?? new DelegateCommand(SearchByName); }
        }

        public bool NotSetChecked
        {
            get { return _notSetChecked; }
            set
            {
                _notSetChecked = value;
                RaisePropertyChanged(() => NotSetChecked);

                ApplyTypeFilter();
            }
        }

        public bool CurbChecked
        {
            get { return _curbChecked; }
            set
            {
                _curbChecked = value;
                RaisePropertyChanged(() => CurbChecked);

                ApplyTypeFilter();
            }
        }

        public bool LedgeChecked
        {
            get { return _ledgeChecked; }
            set
            {
                _ledgeChecked = value;
                RaisePropertyChanged(() => LedgeChecked);

                ApplyTypeFilter();
            }
        }

        public bool RailChecked
        {
            get { return _railChecked; }
            set
            {
                _railChecked = value;
                RaisePropertyChanged(() => RailChecked);

                ApplyTypeFilter();
            }
        }

        public bool GapChecked
        {
            get { return _gapChecked; }
            set
            {
                _gapChecked = value;
                RaisePropertyChanged(() => GapChecked);

                ApplyTypeFilter();
            }
        }

        public bool ParkChecked
        {
            get { return _parkChecked; }
            set
            {
                _parkChecked = value;
                RaisePropertyChanged(() => ParkChecked);

                ApplyTypeFilter();
            }
        }

        public bool CreativeChecked
        {
            get { return _creativeChecked; }
            set
            {
                _creativeChecked = value;
                RaisePropertyChanged(() => CreativeChecked);

                ApplyTypeFilter();
            }
        }

        public ICommand ImportCommand
        {
            get { return _importCommand = _importCommand ?? new DelegateCommand(Import); }
        }

        public ICommand ExportCommand
        {
            get { return _exportCommand = _exportCommand ?? new DelegateCommand(Export); }
        }

        public ICommand ClearFilterCommand
        {
            get { return _clearFilterCommand = _clearFilterCommand ?? new DelegateCommand(Clear); }
        }

        public ICommand HelpFilterCommand
        {
            get { return _helpFilterCommand = _helpFilterCommand ?? new DelegateCommand(HelpFilter); }
        }

        #endregion Properties

        #region Private Methods

        private void Save(object obj)
        {
            if (_selectedSpot != null)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                _databaseService.SaveChanges(_selectedSpot);

                Reload();
                Mouse.OverrideCursor = null;
            }
        }

        private void AddNew(object obj)
        {
            _messenger.Send(new ChooseFilesRequestMsg());
        }

        private void DeleteSpot(object obj)
        {
            if (_selectedSpot != null)
            {
                if (_selectedSpot.LargeFile != Guid.Empty)
                {
                    _imageService.DeleteLargeFile(_selectedSpot.LargeFile);
                }

                _imageService.DeleteSmallFile(_selectedSpot.SmallFile);

                _databaseService.RemoveSpot(_selectedSpot);

                Reload();
            }
        }

        private void Reload()
        {
            Spots = new ObservableCollection<Spot>(_databaseService.GetSpots());

            List<string> cities = _databaseService.GetCitiesDistinct();
            Cities = new ObservableCollection<string>(cities);
            CitiesForAutocomplete = new ObservableCollection<string>(cities);

            SearchValue = string.Empty;
            SelectedCity = string.Empty;
        }

        private void OnChooseFilesConfirmMsg(ChooseFilesConfirmMsg msg)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            foreach (string filename in msg.Filenames)
            {
                DateTime createDate;
                double[] latLng = _imageService.GetLatLongFromImage(filename, out createDate);

                Guid smallFile = _imageService.CopySmallFile(filename);

                Guid largeFile = Guid.Empty;
                if (msg.SaveLargeFile)
                {
                    largeFile = _imageService.CopyLargeFile(filename);
                }

                var spot = new Spot
                {
                    CreatedAt = createDate,
                    Lat = latLng != null ? latLng[0] : 0.0,
                    Lng = latLng != null ? latLng[1] : 0.0,
                    SmallFile = smallFile,
                    LargeFile = largeFile,
                };

                _databaseService.AddSpot(spot);
            }

            if (msg.Filenames.Length > 0)
            {
                Reload();
            }

            Mouse.OverrideCursor = null;
        }

        private void OpenMap(object obj)
        {
            Process.Start(string.Format(RouteUrlTemplate,
                _selectedSpot.Lat.ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                _selectedSpot.Lng.ToString(CultureInfo.InvariantCulture).Replace(",", ".")));
        }

        private void OpenImage(object obj)
        {
            _imageService.OpenLargeFile(_selectedSpot.LargeFile);
        }

        private void Exit(object obj)
        {
            Application.Current.Shutdown();
        }

        private void SearchByName(object obj)
        {
            string searchValue = obj.ToString();

            Spots = !string.IsNullOrEmpty(searchValue)
                ? new ObservableCollection<Spot>(_databaseService.GetSpotsByName(searchValue))
                : new ObservableCollection<Spot>(_databaseService.GetSpots());
        }

        private void Import(object obj)
        {
            _messenger.Send(new ImportRequestMsg());
        }

        private void OnImportConfirmMsg(ImportConfirmMsg msg)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            bool imported = _dataService.Import(msg.Filename, msg.CopyLargeFiles);
            Mouse.OverrideCursor = null;

            if (imported)
            {
                _messenger.Send(new ImportSuccessMsg());

                Reload();
            }
            else
            {
                _messenger.Send(new ImportFaildMsg());
            }
        }

        private void Export(object obj)
        {
            if (_exportView == null)
            {
                _exportView = Container.Resolve<ExportView>();
                _exportView.ShowDialog();
            }
            else
            {
                _exportView.Focus();
            }
        }

        private void OnExportviewClosedMsg(ExportViewClosedMsg msg)
        {
            _exportView = null;
        }

        private void Clear(object obj)
        {
            SearchValue = string.Empty;
            SelectedCity = string.Empty;
            ResetTypeFilter();
        }

        private void HelpFilter(object obj)
        {
            _messenger.Send(new HelpFilterMsg());
        }

        private void ApplyTypeFilter()
        {
            //reset city and name filter
            //SelectedCity = string.Empty;
            //SearchValue = string.Empty;

            var types = new List<SpotType>();

            if (_notSetChecked)
                types.Add(SpotType.NotSet);
            if (_curbChecked)
                types.Add(SpotType.Curb);
            if (_ledgeChecked)
                types.Add(SpotType.Ledge);
            if (_railChecked)
                types.Add(SpotType.Rail);
            if (_gapChecked)
                types.Add(SpotType.Gap);
            if (_parkChecked)
                types.Add(SpotType.Park);
            if (_creativeChecked)
                types.Add(SpotType.Creative);


            Spots = types.Count > 0
                ? new ObservableCollection<Spot>(_databaseService.GetSpotsByType(types))
                : new ObservableCollection<Spot>(_databaseService.GetSpots());
        }

        private void ResetTypeFilter()
        {
            NotSetChecked = false;
            CurbChecked = false;
            LedgeChecked = false;
            RailChecked = false;
            GapChecked = false;
            ParkChecked = false;
            CreativeChecked = false;
        }

        #endregion Private Methods
    }
}
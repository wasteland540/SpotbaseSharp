using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CSharpVitamins;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity;
using SpotbaseSharp.Commands;
using SpotbaseSharp.Messages;
using SpotbaseSharp.Model;
using SpotbaseSharp.Services;
using SpotbaseSharp.Views;
using SpotbaseSharp.Views.GoogleDrive;

namespace SpotbaseSharp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IConfigService _configService;
        private readonly IDataService _dataService;
        private readonly IDatabaseService _databaseService;
        private readonly IFtpService _ftpService;
        private readonly IImageService _imageService;
        private readonly IJsonService _jsonService;
        private readonly IMessenger _messenger;
        private ICommand _activateGoogleDriveCommand;
        private ICommand _addNewCommand;
        private ICommand _addNewGoogleDriveCommand;
        private ObservableCollection<string> _cities;
        private ObservableCollection<string> _citiesForAutocomplete;
        private ICommand _clearFilterCommand;
        private bool _creativeChecked;
        private bool _curbChecked;
        private int _currentDownloadCount;
        private ICommand _deleteSpotCommand;
        private ICommand _exitCommand;
        private ICommand _exportCommand;
        private ExportView _exportView;
        private bool _gapChecked;
        private ICommand _generateMobileKeyCommand;
        private IGoogleDriveService _googleDriveService;
        private Dictionary<string, string> _googledriveDownloadPathList;
        private bool _googledriveSaveLargeFile;
        private bool _hasLargeImage;
        private bool _hasMobileKey;
        private ICommand _helpFilterCommand;
        private ICommand _importCommand;
        private bool _isGoogleDriveEnabled;
        private bool _ledgeChecked;
        private string _locationUrl;
        private string _mobileKey;
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
        private ICommand _showMobileKeyCommand;
        private ObservableCollection<Spot> _spots;
        private ICommand _updateMobileSpotsCommand;

        public MainWindowViewModel(IDatabaseService databaseService, IMessenger messenger, IImageService imageService,
            IDataService dataService, IJsonService jsonService, IFtpService ftpService, IConfigService configService)
        {
            _databaseService = databaseService;
            _messenger = messenger;
            _imageService = imageService;
            _dataService = dataService;
            _jsonService = jsonService;
            _ftpService = ftpService;
            _configService = configService;

            //register for messages
            _messenger.Register<ChooseFilesConfirmMsg>(this, OnChooseFilesConfirmMsg);
            _messenger.Register<ImportConfirmMsg>(this, OnImportConfirmMsg);
            _messenger.Register<ExportViewClosedMsg>(this, OnExportviewClosedMsg);
            _messenger.Register<ChooseFilesGoogleDriveConfirmMsg>(this, OnChooseFilesGoogleDriveConfirmMsg);
            _messenger.Register<FileProcessFinishedMsg>(this, OnFileProcessFinishedMsg);

            IsGoogleDriveEnabled =
                Convert.ToBoolean(_configService.GetAppSettingsValue(AppSettingConstants.IsGoogleDriveEnabled));

            _mobileKey = _configService.GetAppSettingsValue(AppSettingConstants.MobileKey);

            if (!string.IsNullOrEmpty(_mobileKey))
            {
                HasMobileKey = true;
            }

            if (IsGoogleDriveEnabled)
            {
                _googleDriveService = Container.Resolve<IGoogleDriveService>();
                _googleDriveService.DownloadCompleted += GoogleDriveService_DownloadCompleted;
            }
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

                    LocationUrl = string.Format(SpotbaseConstants.LocationUrlTemplate,
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

        public ICommand ActivateGoogleDriveCommand
        {
            get
            {
                _activateGoogleDriveCommand = _activateGoogleDriveCommand ?? new DelegateCommand(ActivateGoogleDrive);
                return _activateGoogleDriveCommand;
            }
        }

        public bool IsGoogleDriveEnabled
        {
            get { return _isGoogleDriveEnabled; }
            set
            {
                _isGoogleDriveEnabled = value;
                RaisePropertyChanged(() => IsGoogleDriveEnabled);
            }
        }

        public ICommand AddNewGoogleDriveCommand
        {
            get
            {
                _addNewGoogleDriveCommand = _addNewGoogleDriveCommand ?? new DelegateCommand(AddNewGoogleDrive);
                return _addNewGoogleDriveCommand;
            }
        }

        public ICommand GenerateMobileKeyCommand
        {
            get
            {
                _generateMobileKeyCommand = _generateMobileKeyCommand ?? new DelegateCommand(GenerateMobileKey);
                return _generateMobileKeyCommand;
            }
        }

        public ICommand ShowMobileKeyCommand
        {
            get
            {
                _showMobileKeyCommand = _showMobileKeyCommand ?? new DelegateCommand(ShowMobileKey);
                return _showMobileKeyCommand;
            }
        }

        public ICommand UpdateMobileSpotsCommand
        {
            get
            {
                _updateMobileSpotsCommand = _updateMobileSpotsCommand ?? new DelegateCommand(UpdateMobileSpots);
                return _updateMobileSpotsCommand;
            }
        }

        public bool HasMobileKey
        {
            get { return _hasMobileKey; }
            set
            {
                _hasMobileKey = value;
                RaisePropertyChanged(() => HasMobileKey);
            }
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
            Process.Start(string.Format(SpotbaseConstants.RouteUrlTemplate,
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

        private void ActivateGoogleDrive(object obj)
        {
            if (!IsGoogleDriveEnabled)
            {
                if (_googleDriveService == null)
                {
                    _googleDriveService = Container.Resolve<IGoogleDriveService>();
                }

                if (_googleDriveService != null && _googleDriveService.IsInitalized())
                {
                    _googleDriveService.CreateApplicationFolder();

                    _configService.SaveAppSettingsValue(AppSettingConstants.IsGoogleDriveEnabled, "true");

                    IsGoogleDriveEnabled = true;
                }
                else
                {
                    _messenger.Send(new GoogleDriveAccessDeniedMsg());
                    _googleDriveService = null;
                }
            }
            else
            {
                _messenger.Send(new GoogleDriveAlreadyEnabledMsg());
            }
        }

        private void AddNewGoogleDrive(object obj)
        {
            var googleDriveFileChooserView = Container.Resolve<GoogleDriveFileChooserView>();
            googleDriveFileChooserView.ShowDialog();
        }

        private void OnChooseFilesGoogleDriveConfirmMsg(ChooseFilesGoogleDriveConfirmMsg msg)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            _googledriveDownloadPathList = new Dictionary<string, string>();
            _currentDownloadCount = 0;
            _googledriveSaveLargeFile = msg.SaveLargeFile;

            foreach (string fileId in msg.FileIds)
            {
                string path = Path.Combine(Path.GetTempPath(), fileId + ".jpg");
                _googledriveDownloadPathList.Add(fileId, path);
            }

            foreach (var keyValuePair in _googledriveDownloadPathList)
            {
                _googleDriveService.DownloadFile(keyValuePair.Key, keyValuePair.Value);
            }

            Mouse.OverrideCursor = null;
        }

        private void GoogleDriveService_DownloadCompleted(object sender, EventArgs e)
        {
            _currentDownloadCount++;

            if (_currentDownloadCount == _googledriveDownloadPathList.Count)
            {
                //all files are downloaded
                foreach (string filename in _googledriveDownloadPathList.Values)
                {
                    DateTime createDate;
                    double[] latLng = _imageService.GetLatLongFromImage(filename, out createDate);

                    Guid smallFile = _imageService.CopySmallFile(filename);

                    Guid largeFile = Guid.Empty;
                    if (_googledriveSaveLargeFile)
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

                if (_googledriveDownloadPathList.Count > 0)
                {
                    Reload();
                }

                _messenger.Send(new FileProcessFinishedMsg());
            }
        }

        private void OnFileProcessFinishedMsg(FileProcessFinishedMsg msg)
        {
            foreach (string tmpPath in _googledriveDownloadPathList.Values)
            {
                File.Delete(tmpPath);
            }
        }

        private void GenerateMobileKey(object obj)
        {
            ShortGuid shortGuid = ShortGuid.NewGuid();
            _mobileKey = shortGuid;

            _configService.SaveAppSettingsValue(AppSettingConstants.MobileKey, _mobileKey);

            HasMobileKey = true;

            _messenger.Send(new MobileKeyGeneratedMsg());
        }

        private void ShowMobileKey(object obj)
        {
            _messenger.Send(new ShowMobileKeyMsg(_mobileKey));
        }

        private void UpdateMobileSpots(object obj)
        {
            string jsonSpots = _jsonService.ToJson(_spots.ToArray());
            _ftpService.UploadSpots(_mobileKey, jsonSpots);

            _messenger.Send(new MobileUpdateFinishedMsg());
        }

        #endregion Private Methods
    }
}
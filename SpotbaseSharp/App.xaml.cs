using System;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity;
using SpotbaseSharp.DataAccessLayer;
using SpotbaseSharp.DataAccessLayer.NDatabase;
using SpotbaseSharp.Services;
using SpotbaseSharp.Views;

namespace SpotbaseSharp
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public IUnityContainer Container;
        private IDataAccessLayer _dbContext;
        public static string AppDirectory = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), "SpotbaseSharp");

        protected override void OnStartup(StartupEventArgs e)
        {
            Container = new UnityContainer();
            
            _dbContext = new NDatabaseConnector(AppDirectory);

            //database registration
            Container.RegisterInstance(typeof (IDataAccessLayer), _dbContext);

            //service registrations
            Container.RegisterType<IDatabaseService, DatabaseService>();
            Container.RegisterType<IImageService, ImageService>();
            Container.RegisterType<IDataService, DataService>();
            Container.RegisterType<IGoogleDriveService, GoogleDriveService>();

            //registraions utils
            //only one instance from messenger can exists! (recipient problems..)
            var messenger = new Messenger();
            Container.RegisterInstance(typeof (IMessenger), messenger);

            var mainView = Container.Resolve<MainWindow>();
            mainView.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _dbContext.Close();

            base.OnExit(e);
        }
    }
}
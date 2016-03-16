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

        protected override void OnStartup(StartupEventArgs e)
        {
            Container = new UnityContainer();
            _dbContext = new NDatabaseConnector();

            //database registration
            Container.RegisterInstance(typeof (IDataAccessLayer), _dbContext);

            //service registrations
            Container.RegisterType<IDatabaseService, DatabaseService>();
            Container.RegisterType<IImageService, ImageService>();
            Container.RegisterType<IDataService, DataService>();

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
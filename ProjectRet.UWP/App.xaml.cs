using System;
using ProjectRet.UWP.Services;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.Storage;
using System.Threading.Tasks;
using ProjectRet.UWP.Helpers;

namespace ProjectRet.UWP
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();
            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
            this.UnhandledException += OnUnhandledException;
        }

        private async void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await new Windows.UI.Popups.MessageDialog("Application Unhandled Exception:\r\n" + e.Exception.Message, "_(:з)∠)_")
                .ShowAsync();
        }

        private async Task CopyMainDb()
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///data.db"));
            if (await storageFolder.TryGetItemAsync("data.db") == null)
            {
                await file.CopyAsync(storageFolder, "data.db");
            }
        }
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            await CopyMainDb();
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
            RegisterExceptionHandlingSynchronizationContext();
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
            RegisterExceptionHandlingSynchronizationContext();
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(ViewModels.MainViewModel));
        }
        private void RegisterExceptionHandlingSynchronizationContext()
        {
            ExceptionHandlingSynchronizationContext
                .Register()
                .UnhandledException += SynchronizationContext_UnhandledException;
        }

        private async void SynchronizationContext_UnhandledException(object sender, Helpers.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await new Windows.UI.Popups.MessageDialog("Synchronization Context Unhandled Exception:\r\n" + GetExceptionDetailMessage(e.Exception), "_(:з)∠)_")
                .ShowAsync();
        }
        private string GetExceptionDetailMessage(Exception ex)
        {
            return $"{ex.Message}\r\n{ex.StackTraceEx()}";
        }
    }
}

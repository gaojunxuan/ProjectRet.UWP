using System;

using ProjectRet.UWP.Services;

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.Storage;

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
        }
        private async void CopyMainDb()
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
            CopyMainDb();
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(ViewModels.MainViewModel));
        }
    }
}

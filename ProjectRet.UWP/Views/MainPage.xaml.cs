using System;

using ProjectRet.UWP.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.System.RemoteSystems;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Security.Credentials.UI;
using ProjectRet.UWP.Helpers;
using Windows.UI.Popups;
using System.Diagnostics;
using Windows.System.Profile;
using Windows.System;

namespace ProjectRet.UWP.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel
        {
            get { return DataContext as MainViewModel; }
        }

        public MainPage()
        {
            InitializeComponent(); 
        }
        FlipView flipView;
        DispatcherTimer timer;
        private void Guide_FlipView_Loaded(object sender, RoutedEventArgs e)
        {
            this.flipView = (FlipView)sender;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }


        private void Timer_Tick(object sender, object e)
        {
            if (flipView != null)
            {
                int count = flipView.Items.Count;
                flipView.SelectedIndex = (flipView.SelectedIndex + 1) % count;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (await WindowsHelloHelper.Auth())
            {
                SigninFailed_Grid.Visibility = Visibility.Collapsed;
                Refresh_Btn.IsEnabled = true;
                await ViewModel.BuildDeviceList();
                ViewModel.IsAuthed = true;
            }
            else
            {
                await new MessageDialog("CouldNotSignIn".GetLocalized(), "Failed".GetLocalized()).ShowAsync();
                Refresh_Btn.IsEnabled = false;
                SigninFailed_Grid.Visibility = Visibility.Visible;
                ViewModel.IsAuthed = false;
            }
        }
        private async void ShowConfigureDialog_Btn_Click(object sender, RoutedEventArgs e)
        {
            ConfigureDialog configureDialog = new ConfigureDialog(((Button)sender).Tag as RemoteSystem);
            await configureDialog.ShowAsync();
            await ViewModel.BuildDeviceList();
        }

        private void ListViewGrid_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void ListViewGrid_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private async void SigninFailed_Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (await WindowsHelloHelper.Auth())
            {
                SigninFailed_Grid.Visibility = Visibility.Collapsed;
                Refresh_Btn.IsEnabled = true;
                await ViewModel.BuildDeviceList();
                ViewModel.IsAuthed = true;
            }
            else
            {
                await new MessageDialog("CouldNotSignIn".GetLocalized(), "Failed".GetLocalized()).ShowAsync();
                Refresh_Btn.IsEnabled = false;
                SigninFailed_Grid.Visibility = Visibility.Visible;
                ViewModel.IsAuthed = false;
            }
        }
    }
}

using ProjectRet.UWP.Models;
using ProjectRet.UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.RemoteSystems;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ProjectRet.UWP.Views
{
    public sealed partial class ConfigureDialog : ContentDialog
    {
        private ConfigureDialogViewModel ViewModel
        {
            get { return DataContext as ConfigureDialogViewModel; }
        }
        public ConfigureDialog()
        {
            this.InitializeComponent();
        }
        public ConfigureDialog(RemoteSystem details)
        {
            this.InitializeComponent();
            ViewModel.Details = new Models.DeviceDetails() { DeviceName = details.DisplayName, UniqueId = details.Id, Type = Helpers.RemoteSystemHelper.ConvertToDeviceType(details.Kind) };
        }

        private void Cancel_Btn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Config_Btn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

    }
}

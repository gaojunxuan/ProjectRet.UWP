using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ProjectRet.UWP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRet.UWP.ViewModels
{
    public class ConfigureDialogViewModel:ViewModelBase
    {
        private DeviceDetails details;

        public DeviceDetails Details
        {
            get { return details; }
            set
            {
                details = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<string> _configureCommand;

        /// <summary>
        /// Gets the ConfigureCommand.
        /// </summary>
        public RelayCommand<string> ConfigureCommand
        {
            get
            {
                return _configureCommand
                    ?? (_configureCommand = new RelayCommand<string>(
                    (x) =>
                    {
                        Details.Credential = x;
                        Helpers.DatabaseHelper.Add(details);
                    }));
            }
        }
    }
}

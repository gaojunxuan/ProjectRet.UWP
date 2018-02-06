using System;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using ProjectRet.UWP.Models;
using System.Threading.Tasks;
using Windows.System.RemoteSystems;
using ProjectRet.UWP.Helpers;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace ProjectRet.UWP.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            
        }
        private ObservableCollection<DeviceDetails> deviceList;

        public ObservableCollection<DeviceDetails> DeviceList
        {
            get { return deviceList; }
            set
            {
                deviceList = value;
                RaisePropertyChanged();
            }
        }
        private bool isAuthed;

        public bool IsAuthed
        {
            get { return isAuthed; }
            set
            {
                isAuthed = value;
                RaisePropertyChanged();
                RaisePropertyChanged("ShowEmptyErrorMessage");
            }
        }

        public Visibility ShowEmptyErrorMessage
        {
            get
            {
                if(IsAuthed)
                {
                    if (DeviceList == null)
                        return Visibility.Visible;
                    else
                        return DeviceList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }
        RemoteSystemWatcher m_remoteSystemWatcher;
        public async Task BuildDeviceList()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if(DeviceList!=null)
                    DeviceList.Clear();
                RaisePropertyChanged("ShowEmptyErrorMessage");
            });
            if(m_remoteSystemWatcher!=null)
            {
                m_remoteSystemWatcher.Stop();
            }
            RemoteSystemAccessStatus accessStatus = await RemoteSystem.RequestAccessAsync();
            if (accessStatus == RemoteSystemAccessStatus.Allowed)
            {
                m_remoteSystemWatcher = RemoteSystem.CreateWatcher();
                // Subscribing to the event raised when a new remote system is found by the watcher.
                m_remoteSystemWatcher.RemoteSystemAdded += RemoteSystemWatcher_RemoteSystemAdded;
                // Subscribing to the event raised when a previously found remote system is no longer available.
                m_remoteSystemWatcher.RemoteSystemRemoved += RemoteSystemWatcher_RemoteSystemRemoved;
                m_remoteSystemWatcher.Start();
            }
        }

        private async void RemoteSystemWatcher_RemoteSystemRemoved(RemoteSystemWatcher sender, RemoteSystemRemovedEventArgs args)
        {
            if(DeviceList!=null)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var item = DeviceList.Where(q => q.UniqueId == args.RemoteSystemId).FirstOrDefault();
                    if (item != null)
                        DeviceList.Remove(item);
                    RaisePropertyChanged("ShowEmptyErrorMessage");
                });
            }
        }

        private async void RemoteSystemWatcher_RemoteSystemAdded(RemoteSystemWatcher sender, RemoteSystemAddedEventArgs args)
        {
            if (DeviceList != null)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    DeviceList.Add(new DeviceDetails() { DeviceName = args.RemoteSystem.DisplayName, UniqueId = args.RemoteSystem.Id, Type = RemoteSystemHelper.ConvertToDeviceType(args.RemoteSystem.Kind),StatusMessage=RemoteSystemHelper.GetStatusMessage(args.RemoteSystem.Status,args.RemoteSystem.Id), Credential=DatabaseHelper.GetKey(args.RemoteSystem.Id), RemoteSys=args.RemoteSystem });
                    RaisePropertyChanged("ShowEmptyErrorMessage");
                });
            }
            else
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    DeviceList = new ObservableCollection<DeviceDetails>();
                    DeviceList.Add(new DeviceDetails() { DeviceName = args.RemoteSystem.DisplayName, UniqueId = args.RemoteSystem.Id, Type = RemoteSystemHelper.ConvertToDeviceType(args.RemoteSystem.Kind),StatusMessage = RemoteSystemHelper.GetStatusMessage(args.RemoteSystem.Status, args.RemoteSystem.Id), Credential = DatabaseHelper.GetKey(args.RemoteSystem.Id), RemoteSys = args.RemoteSystem });
                    RaisePropertyChanged("ShowEmptyErrorMessage");
                });
            }
        }
        private RelayCommand<string> _deleteCommand;

        /// <summary>
        /// Gets the DeleteCommand.
        /// </summary>
        public RelayCommand<string> DeleteCommand
        {
            get
            {
                return _deleteCommand
                    ?? (_deleteCommand = new RelayCommand<string>(
                    async(x) =>
                    {
                        DatabaseHelper.Delete(x);
                        await BuildDeviceList();
                    }));
            }
        }

        private RelayCommand<DeviceDetails> _shutdownCommand;

        /// <summary>
        /// Gets the ShutdownCommand.
        /// </summary>
        public RelayCommand<DeviceDetails> ShutdownCommand
        {
            get
            {
                return _shutdownCommand
                    ?? (_shutdownCommand = new RelayCommand<DeviceDetails>(
                    async(x) =>
                    {
                        var res = await RemoteSystemHelper.ExecuteCommand(x.RemoteSys, new Command() { Body = "shutdown", Credential = x.Credential,GUID=x.UniqueId });
                        if (res == Windows.System.RemoteLaunchUriStatus.Success)
                        {
                            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                            {
                                await new MessageDialog("SuccessMessage".GetLocalized(), "Success".GetLocalized()).ShowAsync();
                            });
                        }
                        else
                        {
                            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                            {
                                await new MessageDialog("FailedMessage".GetLocalized(), "Failed".GetLocalized()).ShowAsync();
                            });
                        }
                    }));
            }
        }

        private RelayCommand<DeviceDetails> _rebootCommand;

        /// <summary>
        /// Gets the ShutdownCommand.
        /// </summary>
        public RelayCommand<DeviceDetails> RebootCommand
        {
            get
            {
                return _rebootCommand
                    ?? (_rebootCommand = new RelayCommand<DeviceDetails>(
                    async (x) =>
                    {
                        var res = await RemoteSystemHelper.ExecuteCommand(x.RemoteSys, new Command() { Body = "reboot", Credential = x.Credential, GUID = x.UniqueId });
                        if(res==Windows.System.RemoteLaunchUriStatus.Success)
                        {
                            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async() =>
                            {
                                await new MessageDialog("SuccessMessage".GetLocalized(), "Success".GetLocalized()).ShowAsync();
                                await BuildDeviceList();
                            });
                        }
                        else
                        {
                            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                            {
                                await new MessageDialog("FailedMessage".GetLocalized(), "Failed".GetLocalized()).ShowAsync();
                            });
                        }
                    }));
            }
        }

        private RelayCommand _refreshCommand;

        /// <summary>
        /// Gets the RefreshCommand.
        /// </summary>
        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand
                    ?? (_refreshCommand = new RelayCommand(
                    async() =>
                    {
                        await BuildDeviceList();
                    }));
            }
        }
    }
}

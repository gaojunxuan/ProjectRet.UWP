using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;
using Windows.System;
using Windows.UI.Popups;

namespace ProjectRet.UWP.Helpers
{
    public static class WindowsHelloHelper
    {
        /// <summary>
        /// Checks to see if Passport is ready to be used.
        /// 
        /// Passport has dependencies on:
        ///     1. Having a connected Microsoft Account
        ///     2. Having a Windows PIN set up for that _account on the local machine
        /// </summary>
        public static async Task<bool> WindowsHelloAvailableCheckAsync()
        {
            bool keyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();
            if (keyCredentialAvailable == false)
            {
                // Key credential is not enabled yet as user 
                // needs to connect to a Microsoft Account and select a PIN in the connecting flow.
                Debug.WriteLine("Microsoft Passport is not setup!\nPlease go to Windows Settings and set up a PIN to use it.");
                return false;
            }

            return true;
        }
        public static async Task<bool> Auth()
        {
            if(await WindowsHelloAvailableCheckAsync())
            {
                UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync("");
                if (consentResult != UserConsentVerificationResult.Verified)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                await new MessageDialog("SetupWindowsHello".GetLocalized(), "Failed".GetLocalized()).ShowAsync();
                await Launcher.LaunchUriAsync(new Uri("ms-settings:signinoptions"));
            }
            return false;
        }
    }
}

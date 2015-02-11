using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;

namespace GameOfTasks.Common
{
    public class PaymentHelper
    {

        public static ProductLicense GetLicenceInfo(string key)
        {
            return App.LicenseInformation.ProductLicenses[key];
        }

        public static async Task<bool> PurchaseLicence(string key, bool includeRecipt = false)
        {
            try
            {
                if (!GetLicenceInfo(key).IsActive)
                {
#if DEBUG
                    StorageFolder installFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
                    StorageFile appSimulatorStorageFile = await installFolder.GetFileAsync("WindowsStoreProxy.xml");

                    await CurrentAppSimulator.ReloadSimulatorAsync(appSimulatorStorageFile);
                    await CurrentAppSimulator.RequestProductPurchaseAsync(key, includeRecipt);
#else
                    await CurrentApp.RequestProductPurchaseAsync(key, includeRecipt);
#endif
                    if (GetLicenceInfo(key).IsActive)
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }

    public class LicenseKeys
    {
        public const string EarlyNotificationLicence = "EarlyNotificationLicence";
    }
}

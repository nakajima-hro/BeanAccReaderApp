using BeanAccReaderApp.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;


namespace BeanAccReaderApp.Model.Device
{
    /*
     * Base class for all device types, any functionality which is generic 
     * across a group of devices is found in thyis class, this includes getting the currently selected service, 
     * and getting generic data from a characteristic.
     */

    public abstract class DeviceBase
    {
        // Selected device id
        public string DeviceID { get; set; }

        // Selected service 
        internal NamedGuid SelectedService { get; set; }
        // Selected index from the list of Uuid's on the main UI.
        internal int SelectedIndex;
        // Callback to return data once it has been recieved from device.
        internal Action<string> CallBack;
        // TODO: move to constants class so can be used across the application.
        internal string AuthorizeError = StringResources.AccessDenied;

        // Initialise object by setting the device id
        public void Initialise(string deviceID)
        {
            this.DeviceID = deviceID;
        }

        // Retrieves a list of supported services from a hard coded list. 
        // This is then return so ythe UI can be updated. 
        // TODO: poss move to Device Helper class?
        public async Task<List<NamedGuid>> PopulateSupportedServices()
        {
            // Initalise empty collection of named Guid's
            List<NamedGuid> services = new List<NamedGuid>();

			try
			{
                // Get current service using the current device id
                var service = await GetService();

				//中島追加
				StartDeviceWatcher(service.Uuid);

				// Itterate through the list of availible services checking the number of 
				// possible characteristics, if there is more than 1, add it to the list to be returned. 
				for (int i = 0; i < Utils.ServiceHandler.GetTotalGuid(); i++)
                {
                    NamedGuid guid = Utils.ServiceHandler.GetGuid(i);
                    var characteristic = service.GetCharacteristics(guid.Guid);

                    if (characteristic.Count > 0)
                    {
                        services.Add(guid);
                    }
                }
            }
            catch (Exception)
            {
                MessageHelper.DisplayBasicMessage(AuthorizeError);
            }
            // Return a list of Named Guid objects which have more then 0 characteristics
            return services;
        }

        // Return a list of Characteristics from a service, and return a list to be displayed on the UI.
        public async Task<List<GattCharacteristic>> GetCharacteristics(NamedGuid guid)
        {
            // Initalise empty collection of GattCharacteristic objects.
            List<GattCharacteristic> characteristics = new List<GattCharacteristic>();
            // Set this selected service value to the value recieved from the UI so it can be used later.
            this.SelectedService = guid;

            try
            {
                // Get current service using the current device id
                var service = await GetService();
                // Get a list of characteristics from the service and add them to the charateristics list.
                var characteristic = service.GetCharacteristics(guid.Guid);
                foreach (var item in characteristic)
                {
                    characteristics.Add(item);
                }
            }
            catch (Exception e)
            {
                MessageHelper.DisplayBasicMessage(StringResources.CharacteristicsError+ " : " + e.Message);
            }
            // return list to be displayed on the UI.
            return characteristics;
        }

        // Gets a single characteristic objec and returns it.
        public async Task<GattCharacteristic> GetCharacteristic(int selectedIndex)
        {
            // set defualt value to null, so can be tested by any calling functions
            GattCharacteristic characteristic = null;
            try
            {
                // Get current service using the current device id
                var service = await GetService();
                // get a characteristic object using the currently selected service uuid.
                // the selected index value is used by the UI, for the device index.
                characteristic = service.GetCharacteristics(this.SelectedService.Guid)[selectedIndex];
            }
            catch (Exception e)
            {
                MessageHelper.DisplayBasicMessage(StringResources.CharacteristicsError + " : " + e.Message);
            }
            return characteristic;
        }

		//中島追加
		//参考
		//http://blogs.msdn.com/b/hirosho/archive/2014/01/06/howtomanageplugandplaydeviceonwindows81.aspx

		private DeviceWatcher deviceWatcher;
		public DeviceWatcher DeviceWatcher
		{
			get { return this.deviceWatcher; }
			set
			{
				this.deviceWatcher = value;
			}
		}

		//周辺機器の監視を開始したいコードで、以下を実行します。Bluetooth SerialPortを例にして説明します。
		//他の種類の周辺機器も処理の流れは一緒で、CreateWatcherの引数を周辺機器の種別に合わせて変更すれば対応可能です。

		void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
		{
			//ペアリング済みならここのイベント発生
		}

		void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
		{
			//ペアリング済みならここのイベント発生
		}

		void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
		{
		}

		void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
		{
		}

		public void StartDeviceWatcher(Guid serviceGuid)
		{
			try
			{
				DeviceWatcher = DeviceInformation.CreateWatcher(GattDeviceService.GetDeviceSelectorFromUuid(serviceGuid));
				DeviceWatcher.Added += DeviceWatcher_Added;
				DeviceWatcher.Removed += DeviceWatcher_Removed;
				DeviceWatcher.Updated += DeviceWatcher_Updated;
				DeviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
				DeviceWatcher.Start();
			}
			catch(Exception e)
			{
				MessageHelper.DisplayBasicMessage(e.Message);
			}
		}
		//中島追加終了

		// Abstract function which must be overriden in any objet which is derived from this class. 
		// This allows new device classes to handle characteristics differently.
		public abstract Task HandleSelectedCharacteristic(Dictionary<string, object> parameters, Action<string> callback);

        // Get currently selected service using the device id. 
        // This is called everytime before an attempt to get data is made, due to the possiblity of the device disconnecting.
        internal async Task<GattDeviceService> GetService()
        {
            // Attempt to get the servcei using device id.
            var service = await GattDeviceService.FromIdAsync(this.DeviceID);

			if (service == null)
            {
                // Error accessing the service.
                throw new UnauthorizedAccessException();
            }
            else
            {
                // Have succesfully retrieved the service, return it.
                return service;
            }
        }

        // Generic get data function, this is not a one function fits all solution. 
        // However despite this it does cover a majority of instances.
        internal async Task<GattReadResult> GetData()
        {
            // returned as the result.
            GattReadResult readValue = null;

            try
            {
                // Call to get the current service
                var service = await GetService();
                // Get the current characteristic and attempt to read the data from the device.
                var characteristic = service.GetCharacteristics(this.SelectedService.Guid)[this.SelectedIndex];
                readValue = await characteristic.ReadValueAsync();
            }
            catch (Exception)
            {
                MessageHelper.DisplayBasicMessage(AuthorizeError);
            }
            // Return read data from the device.
            return readValue;
        }

        // Using the Callback pointer, return any data to the UI facing viewmodel to update the UI. 
        internal void ReturnResult(string result)
        {
            if (this.CallBack != null)
            {
                this.CallBack(result);
            }
        }
    }
}

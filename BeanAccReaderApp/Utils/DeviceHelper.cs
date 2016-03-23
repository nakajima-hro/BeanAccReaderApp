using BeanAccReaderApp.Model;
using BeanAccReaderApp.Model.Device;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

using System;

namespace BeanAccReaderApp.Utils
{

	// Static class to perform device specific functions
	public static class DeviceHelper
    {
		// Service UUID
		// Weight Scale
		static public readonly String WEIGHT_SCALE_SERVICE_UUID_STRING = "0000181d-0000-1000-8000-00805f9b34fb";
		// LightBlue Bean Scratch
		static public readonly String LIGHTBLUE_BEAN_SCRATCH_SERVICE_UUID_STRING = "A495FF20-C5B1-4B44-B512-1370F02D74DE";


		// Initalise a list of supported devices. This list is used to populate the combo box on the UI.
		private static List<NamedGuid> DeviceList = new List<NamedGuid>() 
        {
            new NamedGuid(){ Name = "Battery", Guid = GattServiceUuids.Battery, Type = DeviceType.Battery},
            new NamedGuid(){ Name = "Generic Access", Guid = GattServiceUuids.GenericAccess, Type = DeviceType.GenericAccess},
			new NamedGuid(){ Name = "Heart Rate Monitor", Guid = GattServiceUuids.HeartRate, Type = DeviceType.HeartRate},

			// 中島追加
			new NamedGuid(){ Name = "Blood Pressure Monitor", Guid = GattServiceUuids.BloodPressure, Type = DeviceType.BloodPressure},
			new NamedGuid(){ Name = "Health Thermometer", Guid = GattServiceUuids.HealthThermometer, Type = DeviceType.HealthThermometer},
			new NamedGuid(){ Name = "Weight Scale", Guid = new Guid(WEIGHT_SCALE_SERVICE_UUID_STRING), Type = DeviceType.WeightScale},
			new NamedGuid(){ Name = "LightBlue Bean", Guid = new Guid(LIGHTBLUE_BEAN_SCRATCH_SERVICE_UUID_STRING), Type = DeviceType.LightBlueBean},
		};

        // Return the whole list of supported devices.
        public static List<NamedGuid> GetAllSupportedDeviceTypes()
        {
            return DeviceList;
        }

        // Return a single device object depending on name value, using a linq statement.
        public static NamedGuid GetGuid(string name)
        {
            return DeviceList.Where(r => r.Name == name).FirstOrDefault();
        }

        // Using device type decideds which device object to initilise, this allows for dynamic object creation.
        public static DeviceBase GetDeviceObject(DeviceInformation deviceInfo, DeviceType type)
        {
            DeviceBase device = null;
            // Main switch statement to handle the creation of the device objects.
            switch (type)
            {
                case DeviceType.GenericAccess:
                    device = new GenericAccessDevice();
                    break;
//中島追加
				case DeviceType.LightBlueBean:
					device = new LightBlueBeanDevice();
					break;
			}

			if (device == null)
            {
                // Display error if device does not have a value and return null.
                MessageHelper.DisplayBasicMessage(StringResources.InitialisationError);
                return device;
            }

            device.Initialise(deviceInfo.Id);

            return device;
        }
    }
}

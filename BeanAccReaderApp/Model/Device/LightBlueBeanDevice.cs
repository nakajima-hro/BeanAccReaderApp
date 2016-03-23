using BeanAccReaderApp.Utils;
using BeanAccReaderApp.Model.MyClass;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;


namespace BeanAccReaderApp.Model.Device
{
	/*
     * This class represents a heart rate monitor object. All servcie based functions are handled in the base class.
     * This class should only handle things which are specifically for a heart rate monitor.
     */
	public class LightBlueBeanDevice : DeviceBase
	{
		public override async Task HandleSelectedCharacteristic(Dictionary<string, object> parameters, Action<string> callback)
		{
			// Set a callback so once the data has been recieved from the device, 
			// we have a pointer to the function which will update the UI.
			if (callback != null)
			{
				this.CallBack = callback;
			}

			// Get refrence to the currently selected service, as this saves dipping into the base class and retriveing it.
			var service = this.SelectedService.Guid;

			// Main check to see what characteristic is to be called and how to handle the call.
			if (service == ServiceHandler.GetGuid("LightBlueBeanScratch1").Guid)
			{
				// Passing through the toggle state from the UI. This allows the handler function to either attatch or detatch a value changed event.
				string key = StringResources.ToggleValue;
				if (parameters.ContainsKey(key))
				{
					await HandleLightBlueBeanScratch1(Convert.ToBoolean(parameters[key]));
				}
			}
		}

		bool isHandlerAttached = false;
		// Indicate operation which handles creating a value changed event.
		private async Task HandleLightBlueBeanScratch1(bool handleNotofication)
		{
			// Get service object from base class.
			var service = await GetService();
			// Get current characteristic from current service using the selected values from the base class.
			var characteristic = service.GetCharacteristics(this.SelectedService.Guid)[this.SelectedIndex];
			// Check to see if we are attching or detatching event.
			if (handleNotofication)
			{
				// Attach a listener and assign a pointer to a function which will handle the data as it comes into the application.
				if (!isHandlerAttached)
				{
					characteristic.ValueChanged += Characteristic_ValueChanged;
					isHandlerAttached = true;
				}

				// Tell the device we want to register for the indicate updates, and return the staus of the registration.
				GattCommunicationStatus status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

				// Check to see if the registration was successful by checking the status, if not display a message telling the user. 
				if (status == GattCommunicationStatus.Unreachable)
				{
					MessageHelper.DisplayBasicMessage("Your device is currently unavailible, please try again later.");
					characteristic.ValueChanged -= Characteristic_ValueChanged;
					isHandlerAttached = false;
				}
			}
			else
			{
				// Remove the pointer to the local function to stop processing updates.
				characteristic.ValueChanged -= Characteristic_ValueChanged;
				isHandlerAttached = false;
				ReturnResult("");
			}
		}

		// Handle the recieved data from the indication event.
		// IndicationへのConfirmは、受信時点で自動的に行われる。
		void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
		{
			string result = string.Empty;

			var data = new byte[args.CharacteristicValue.Length];
			DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(data);

			// パケット構成
			// 0-1: カウンタ(16bit)
			// 2-3: AccX(16bit)

			UInt16 count;
			Int16 accXFiltered;
			Int16 accXRaw;

			if (data.Length >= 2)
			{
				int currentOffset = 0;

				//Read LightBlue Bean Scratch1
				count = BitConverter.ToUInt16(new byte[] { data[currentOffset], data[currentOffset + 1] }, 0);
				currentOffset += 2;

				accXFiltered = BitConverter.ToInt16(new byte[] { data[currentOffset], data[currentOffset + 1] }, 0);
				currentOffset += 2;

				accXRaw = BitConverter.ToInt16(new byte[] { data[currentOffset], data[currentOffset + 1] }, 0);
				currentOffset += 2;


				LightBlueBeanScratch1EventArgs LightBlueBeanScratch1Value = new LightBlueBeanScratch1EventArgs();
				LightBlueBeanScratch1Value.Scratch1 = new LightBlueBeanScratch1Value(count, accXFiltered, accXRaw);
				OnLightBlueBeanScratch1Measured(LightBlueBeanScratch1Value);
			}
		}



		protected virtual void OnLightBlueBeanScratch1Measured(LightBlueBeanScratch1EventArgs e)
		{
			EventHandler<LightBlueBeanScratch1EventArgs> handler = LightBlueBeanScratch1_Measured;
			if (handler != null)
			{
				handler(this, e);
			}
		}
		public event EventHandler<LightBlueBeanScratch1EventArgs> LightBlueBeanScratch1_Measured;
	}

	public class LightBlueBeanScratch1EventArgs : EventArgs
	{
		public LightBlueBeanScratch1Value Scratch1 { get; set; }
	}

}

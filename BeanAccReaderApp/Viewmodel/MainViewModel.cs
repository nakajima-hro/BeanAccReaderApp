using BeanAccReaderApp.Model;
using BeanAccReaderApp.Model.Device;
using BeanAccReaderApp.Utils;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

using System.Linq;
using System.Threading;

using System.Diagnostics;

using ZedGraph;


namespace BeanAccReaderApp.Viewmodel
{


	// 中島追加
	public class DeviceMember
	{
		public int DeviceNumber { get; set; }
		public string DeviceName { get; set; }

		public DeviceMember(int deviceNumber, string deviceName)
		{
			this.DeviceNumber = deviceNumber;
			this.DeviceName = deviceName;
		}
	}


	// Main view model class which is databound to the UI, and handles all code behind interactions with the data classes.
	public class MainViewModel : ViewModelBase
	{
		
		// 中島追加
		TimerCallback timerDelegate;
		Timer timer;

		DeviceInformationCollection dInfoLightBlueBean;

		private List<UInt16> counter;
		public List<UInt16> Counter
		{
			get { return this.counter; }
			set
			{
				this.counter = value;
			}
		}

		private PointPairList myDataAccXFiltered;
		public PointPairList MyDataAccXFiltered
		{
			get { return this.myDataAccXFiltered; }
			set
			{
				this.myDataAccXFiltered = value;
				this.RaisePropertyChanged("MyDataAccXFiltered");
			}
		}

		private RollingPointPairList myLineSeriesAccXFiltered;
		public RollingPointPairList MyLineSeriesAccXFiltered
		{
			get { return this.myLineSeriesAccXFiltered; }
			set
			{
				this.myLineSeriesAccXFiltered = value;
				this.RaisePropertyChanged("MyLineSeriesAccXFiltered");
			}
		}

		private PointPairList myDataAccXRaw;
		public PointPairList MyDataAccXRaw
		{
			get { return this.myDataAccXRaw; }
			set
			{
				this.myDataAccXFiltered = value;
				this.RaisePropertyChanged("MyDataAccXRaw");
			}
		}

		private RollingPointPairList myLineSeriesAccXRaw;
		public RollingPointPairList MyLineSeriesAccXRaw
		{
			get { return this.myLineSeriesAccXFiltered; }
			set
			{
				this.myLineSeriesAccXRaw = value;
				this.RaisePropertyChanged("MyLineSeriesAccXRaw");
			}
		}



		private String debugText;
		public String DebugText
		{
			get { return this.debugText; }
			set
			{
				this.debugText = value;
				this.RaisePropertyChanged("DebugText");
			}
		}

		private List<DeviceMember> devices;
		public List<DeviceMember> Devices
		{
			get { return this.devices; }
			set
			{
				this.devices = value;
//				this.RaisePropertyChanged("DeviceMember");
			}

		}



		// Initialisation
		public MainViewModel()
		{
			//中島追加
			DataInitialize();
			DeviceSetter();
//			StartCheckNotigficationTimer();
//			StartPlotChangeTimer();
		}

		//中島追加
		private LightBlueBeanDevice lightBlueBeanDevice;

		private void DataInitialize()
		{
			counter = new List<UInt16>();
			myLineSeriesAccXFiltered = new RollingPointPairList(1500);
			myLineSeriesAccXRaw = new RollingPointPairList(1500);
			myDataAccXFiltered = new PointPairList();
			myDataAccXRaw = new PointPairList();
			Devices = new List<DeviceMember>();
		}

		//中島追加
		public async void DeviceSetter()
		{
			try
			{
				//デバイスUUIDを取得
				NamedGuid dHelperLightBlueBean = DeviceHelper.GetGuid("LightBlue Bean");
				//該当するデバイスを列挙
				dInfoLightBlueBean =
					await DeviceInformation.FindAllAsync(GattDeviceService.GetDeviceSelectorFromUuid(dHelperLightBlueBean.Guid));
				//
				Devices.Clear();
				for (int cnt = 0; cnt < dInfoLightBlueBean.Count; cnt++)
				{
					Devices.Add(new DeviceMember(cnt, dInfoLightBlueBean[cnt].Name));
				}
				this.RaisePropertyChanged("DeviceMember");
			}
			catch
			{
				MessageHelper.DisplayBasicMessage("センサとペアリングされていますか？");
			}

		}

		//中島追加
		public async void DeviceConnector(int cnt)
		{
			try
			{
				lightBlueBeanDevice = (LightBlueBeanDevice)DeviceHelper.GetDeviceObject(dInfoLightBlueBean[cnt], DeviceHelper.GetGuid("LightBlue Bean").Type);
				var characteristicsLightBlueBean = await lightBlueBeanDevice.GetCharacteristics(ServiceHandler.GetGuid("LightBlueBeanScratch1"));
				var characteristicLightBlueBean = characteristicsLightBlueBean.FirstOrDefault();
				lightBlueBeanDevice.LightBlueBeanScratch1_Measured += AddLightBlueBeanScratchData;

				StartCheckNotigficationTimer();
			}
			catch
			{
				MessageHelper.DisplayBasicMessage("センサとペアリングされていますか？");
			}

		}

		void StartCheckNotigficationTimer()
		{
			DebugText = "StartCheckNotigficationTimer";
			timerDelegate = new TimerCallback(CheckNotification);
			timer = new Timer(timerDelegate, null, 1000, Timeout.Infinite);
		}

		public async void CheckNotification(object o)
		{
			DebugText = "CheckNotification";
			try
			{
				Dictionary<string, object> parameters = new Dictionary<string, object>();
				parameters.Add(StringResources.ToggleValue, true);
				parameters.Add(StringResources.FunctionType, StringResources.NotificationFunction);
				await lightBlueBeanDevice.HandleSelectedCharacteristic(parameters, HandleCallbackResponse);
				timer.Dispose();

			}
			catch
			{

			}


		}


		//時刻を0スタートにする
		private void AddLightBlueBeanScratchData(object sender, LightBlueBeanScratch1EventArgs e)
		{
			try
			{
				//
			}
			catch
			{
				//
			};

			String output = String.Format("Count:{0:d5} ", e.Scratch1.Count);
			output = output + String.Format("AccXFiltered:{0:d5} ", e.Scratch1.AccXFiltered);
			output = output + String.Format("AccXRaw:{0:d5} ", e.Scratch1.AccXRaw);

			DebugText = output;
			Debug.WriteLine(output);

			Counter.Clear();
			MyDataAccXFiltered.Clear();
			MyDataAccXRaw.Clear();

			Counter.Add(Convert.ToUInt16(e.Scratch1.Count));
			MyDataAccXFiltered.Add(new PointPair(e.Scratch1.Count, e.Scratch1.AccXFiltered)); //Addでは、PropertyChangedにならない
			MyDataAccXRaw.Add(new PointPair(e.Scratch1.Count, e.Scratch1.AccXRaw)); //Addでは、PropertyChangedにならない
			this.RaisePropertyChanged("MyLineSeriesAccXFiltered");
		}

		// Method which populates the device viewmodel's value propery with a result. A pointer to this function is passed down into the data classes so the all return back to this point.
		private async void HandleCallbackResponse(string result)
		{
			// The function is forced run on the UI thread as there is a possiblity that this function could be 
			// called on a different thread. For example when the data is updated from a notification call, or indicate.
			//await Task.Run(() =>
			//{
			//    this.SelectedDeviceViewModel.Value = result;
			//    this.RaisePropertyChanged("SelectedDeviceViewModel");
			//});

			var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

			DebugText = "HandleCallbackResponse";

			await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			{
//				this.SelectedDeviceViewModel.Value = result;
//				this.RaisePropertyChanged("SelectedDeviceViewModel");
			});

		}

	}


}

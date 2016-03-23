using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BeanAccReaderApp.Viewmodel;
using ZedGraph;
using System.IO;

namespace BeanAccReaderApp
{
	public partial class Form1 : Form
	{

		MainViewModel mainViewModel;
		RollingPointPairList pointBeanAccXFiltered = new RollingPointPairList(1500);
		RollingPointPairList pointBeanAccXRaw = new RollingPointPairList(1500);
		StreamWriter OutputFileStream;


		public Form1()
		{
			InitializeComponent();

			//スレッドチェックをOff
			CheckForIllegalCrossThreadCalls = false;

			string fname = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\"+ DateTime.Now.ToLocalTime().ToString("yyyyMMddhhmmss") + ".csv";
			OutputFileStream = new StreamWriter(fname);
			OutputFileStream.AutoFlush = true;
		}

		~Form1()
		{
			OutputFileStream.Close();
		}

		private static void WriteCsv(List<double> data)
		{
			try
			{
				// appendをtrueにすると，既存のファイルに追記
				// falseにすると，ファイルを新規作成する
				var append = false;
				// 出力用のファイルを開く
				string fname = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\filtered.csv";

				using (var sw = new System.IO.StreamWriter(fname, append))
				{
					foreach (var dt in data)
					{
						// 
						sw.WriteLine("{0}", dt);
					}
				}
			}
			catch (System.Exception e)
			{
				// ファイルを開くのに失敗したときエラーメッセージを表示
				System.Console.WriteLine(e.Message);
			}
		}



		private void Form1_Load(object sender, EventArgs e)
		{
			mainViewModel = new MainViewModel();
			mainViewModel.PropertyChanged += ViewModel_PropertyChanged;

			Create_LineGraph(ref zedGraphControl);
			zedGraphControl.GraphPane.AddCurve("BeanAccXFiltered", null, Color.Black, SymbolType.None);
			zedGraphControl.GraphPane.AddCurve("BeanAccXRaw", null, Color.Red, SymbolType.None);

			this.Refresh();

		}

		private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case "DebugText":
					debugBox.Text = mainViewModel.DebugText;
					break;


				case "MyLineSeriesAccXFiltered":

					List<UInt16> counter = mainViewModel.Counter;
					PointPairList accXFiltered = mainViewModel.MyDataAccXFiltered;
					PointPairList accXRaw = mainViewModel.MyDataAccXRaw;

					pointBeanAccXFiltered.Add(accXFiltered);
					pointBeanAccXRaw.Add(accXRaw);
					zedGraphControl.GraphPane.CurveList["BeanAccXFiltered"].Points = pointBeanAccXFiltered;
					zedGraphControl.GraphPane.CurveList["BeanAccXRaw"].Points = pointBeanAccXRaw;

					string buffer = String.Format("{0},{1},{2}", counter[0], accXFiltered[0].Y, accXRaw[0].Y);

					zedGraphControl.AxisChange();
					zedGraphControl.Refresh();
					break;

				case "DeviceMember":
					ComboBoxDevice.Items.Clear();

					try
					{
						for (int cnt = 0; cnt < mainViewModel.Devices.Count; cnt++)
						{
							ComboBoxDevice.Items.Add(mainViewModel.Devices[cnt].DeviceName);
						}
						ComboBoxDevice.SelectedIndex = 0;
					}
					catch
					{

					}

					break;

			}
		}

		private void zedGraphControl1_Load(object sender, EventArgs e)
		{

		}


		//折れ線グラフ
		private void Create_LineGraph(ref ZedGraphControl zg)
		{
			var myPane = zg.GraphPane;


			//グラフ／X軸、Y軸のタイトル設定

			myPane.Title.Text = "加速度";
			myPane.XAxis.Title.Text = "時間(Sample)";
			myPane.YAxis.Title.Text = "値(Raw)";

			//X軸の目盛りを描画
			myPane.XAxis.MajorGrid.IsVisible = true;

			//Y軸のスケールを赤に
			myPane.YAxis.Scale.FontSpec.FontColor = Color.Red;
			myPane.YAxis.Title.FontSpec.FontColor = Color.Red;

			//反対側のY軸の目盛りの描画
			myPane.YAxis.MajorTic.IsOpposite = false;
			myPane.YAxis.MinorTic.IsOpposite = false;

			//Yが0の位置の水平線
			myPane.YAxis.MajorGrid.IsZeroLine = true;
			myPane.YAxis.Scale.Align = AlignP.Inside;

			//Y軸の範囲を指定する
//			myPane.YAxis.Scale.Min = -20;
//			myPane.YAxis.Scale.Max = 30;
			myPane.YAxis.IsVisible = true;

			//グラフ領域の内側をグラデーションに
			myPane.Chart.Fill = new Fill(Color.White, Color.LightGray, 45.0F);

			//テキストボックスをグラフの左下に描画
			var text = new TextObj("Zoom: 左のマウスボタンでドラッグ" + Convert.ToChar(10) + "Pan: Ctrl+左ボタンでドラッグ" + Convert.ToChar(10) + "Context Menu: 右ボタン",
									   0.05F, 0.2F, CoordType.ChartFraction, AlignH.Left, AlignV.Bottom);
			text.FontSpec.StringAlignment = StringAlignment.Near;
			myPane.GraphObjList.Add(text);

			//スクロールバーは非表示
			zg.IsShowVScrollBar = false;
			zg.IsAutoScrollRange = true;

			//ポイントの値をToolTip風に表示する
			zg.IsShowPointValues = true;



		}

		private void ButtonClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void ComboBoxDevice_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void ButtonScan_Click(object sender, EventArgs e)
		{
			mainViewModel.DeviceSetter();
		}

		private void ButtonConnect_Click(object sender, EventArgs e)
		{
			mainViewModel.DeviceConnector(ComboBoxDevice.SelectedIndex);
		}
	}
}

namespace BeanAccReaderApp
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.zedGraphControl = new ZedGraph.ZedGraphControl();
			this.debugBox = new System.Windows.Forms.TextBox();
			this.ButtonClose = new System.Windows.Forms.Button();
			this.ButtonScan = new System.Windows.Forms.Button();
			this.ButtonConnect = new System.Windows.Forms.Button();
			this.ComboBoxDevice = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// zedGraphControl
			// 
			this.zedGraphControl.Location = new System.Drawing.Point(16, 26);
			this.zedGraphControl.Name = "zedGraphControl";
			this.zedGraphControl.ScrollGrace = 0D;
			this.zedGraphControl.ScrollMaxX = 0D;
			this.zedGraphControl.ScrollMaxY = 0D;
			this.zedGraphControl.ScrollMaxY2 = 0D;
			this.zedGraphControl.ScrollMinX = 0D;
			this.zedGraphControl.ScrollMinY = 0D;
			this.zedGraphControl.ScrollMinY2 = 0D;
			this.zedGraphControl.Size = new System.Drawing.Size(748, 398);
			this.zedGraphControl.TabIndex = 0;
			this.zedGraphControl.UseExtendedPrintDialog = true;
			this.zedGraphControl.Load += new System.EventHandler(this.zedGraphControl1_Load);
			// 
			// debugBox
			// 
			this.debugBox.Location = new System.Drawing.Point(16, 442);
			this.debugBox.Name = "debugBox";
			this.debugBox.Size = new System.Drawing.Size(748, 19);
			this.debugBox.TabIndex = 1;
			// 
			// ButtonClose
			// 
			this.ButtonClose.Location = new System.Drawing.Point(579, 478);
			this.ButtonClose.Name = "ButtonClose";
			this.ButtonClose.Size = new System.Drawing.Size(185, 38);
			this.ButtonClose.TabIndex = 2;
			this.ButtonClose.Text = "CLOSE";
			this.ButtonClose.UseVisualStyleBackColor = true;
			this.ButtonClose.Click += new System.EventHandler(this.ButtonClose_Click);
			// 
			// ButtonScan
			// 
			this.ButtonScan.Location = new System.Drawing.Point(16, 478);
			this.ButtonScan.Name = "ButtonScan";
			this.ButtonScan.Size = new System.Drawing.Size(135, 38);
			this.ButtonScan.TabIndex = 3;
			this.ButtonScan.Text = "SCAN";
			this.ButtonScan.UseVisualStyleBackColor = true;
			this.ButtonScan.Click += new System.EventHandler(this.ButtonScan_Click);
			// 
			// ButtonConnect
			// 
			this.ButtonConnect.Location = new System.Drawing.Point(363, 478);
			this.ButtonConnect.Name = "ButtonConnect";
			this.ButtonConnect.Size = new System.Drawing.Size(133, 38);
			this.ButtonConnect.TabIndex = 4;
			this.ButtonConnect.Text = "CONNECT";
			this.ButtonConnect.UseVisualStyleBackColor = true;
			this.ButtonConnect.Click += new System.EventHandler(this.ButtonConnect_Click);
			// 
			// ComboBoxDevice
			// 
			this.ComboBoxDevice.FormattingEnabled = true;
			this.ComboBoxDevice.Location = new System.Drawing.Point(184, 478);
			this.ComboBoxDevice.Name = "ComboBoxDevice";
			this.ComboBoxDevice.Size = new System.Drawing.Size(149, 20);
			this.ComboBoxDevice.TabIndex = 5;
			this.ComboBoxDevice.SelectedIndexChanged += new System.EventHandler(this.ComboBoxDevice_SelectedIndexChanged);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 561);
			this.Controls.Add(this.ComboBoxDevice);
			this.Controls.Add(this.ButtonConnect);
			this.Controls.Add(this.ButtonScan);
			this.Controls.Add(this.ButtonClose);
			this.Controls.Add(this.debugBox);
			this.Controls.Add(this.zedGraphControl);
			this.Name = "Form1";
			this.Text = "BeanAccReaderApp";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ZedGraph.ZedGraphControl zedGraphControl;
		private System.Windows.Forms.TextBox debugBox;
		private System.Windows.Forms.Button ButtonClose;
		private System.Windows.Forms.Button ButtonScan;
		private System.Windows.Forms.Button ButtonConnect;
		private System.Windows.Forms.ComboBox ComboBoxDevice;
	}
}


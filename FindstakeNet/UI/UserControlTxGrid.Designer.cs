
namespace FindstakeNet.UI
{
	partial class UserControlTxGrid
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.panelMsg = new System.Windows.Forms.Panel();
			this.labelMsg = new System.Windows.Forms.Label();
			this.panelgrid = new System.Windows.Forms.Panel();
			this.dataGridViewTemplates = new System.Windows.Forms.DataGridView();
			this.groupBoxMain = new System.Windows.Forms.GroupBox();
			this.labelAvailable = new System.Windows.Forms.Label();
			this.labelBlockTime = new System.Windows.Forms.Label();
			this.labelDifficulty = new System.Windows.Forms.Label();
			this.labelHeight = new System.Windows.Forms.Label();
			this.buttonStart = new System.Windows.Forms.Button();
			this.panelResultGrid = new System.Windows.Forms.Panel();
			this.dataGridViewResults = new System.Windows.Forms.DataGridView();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonExportResults = new System.Windows.Forms.Button();
			this.panelMsg.SuspendLayout();
			this.panelgrid.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewTemplates)).BeginInit();
			this.groupBoxMain.SuspendLayout();
			this.panelResultGrid.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelMsg
			// 
			this.panelMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.panelMsg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
			this.panelMsg.Controls.Add(this.labelMsg);
			this.panelMsg.Location = new System.Drawing.Point(3, 13);
			this.panelMsg.Name = "panelMsg";
			this.panelMsg.Size = new System.Drawing.Size(781, 19);
			this.panelMsg.TabIndex = 0;
			// 
			// labelMsg
			// 
			this.labelMsg.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelMsg.Location = new System.Drawing.Point(0, 0);
			this.labelMsg.Name = "labelMsg";
			this.labelMsg.Size = new System.Drawing.Size(781, 19);
			this.labelMsg.TabIndex = 0;
			this.labelMsg.Text = "label1";
			// 
			// panelgrid
			// 
			this.panelgrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.panelgrid.AutoScroll = true;
			this.panelgrid.Controls.Add(this.dataGridViewTemplates);
			this.panelgrid.Location = new System.Drawing.Point(0, 0);
			this.panelgrid.Name = "panelgrid";
			this.panelgrid.Size = new System.Drawing.Size(532, 190);
			this.panelgrid.TabIndex = 1;
			// 
			// dataGridViewTemplates
			// 
			this.dataGridViewTemplates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewTemplates.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewTemplates.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewTemplates.Name = "dataGridViewTemplates";
			this.dataGridViewTemplates.Size = new System.Drawing.Size(532, 190);
			this.dataGridViewTemplates.TabIndex = 0;
			this.dataGridViewTemplates.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewTemplatesColumnHeaderMouseClick);
			// 
			// groupBoxMain
			// 
			this.groupBoxMain.Controls.Add(this.labelAvailable);
			this.groupBoxMain.Controls.Add(this.labelBlockTime);
			this.groupBoxMain.Controls.Add(this.labelDifficulty);
			this.groupBoxMain.Controls.Add(this.labelHeight);
			this.groupBoxMain.Location = new System.Drawing.Point(10, 33);
			this.groupBoxMain.Name = "groupBoxMain";
			this.groupBoxMain.Size = new System.Drawing.Size(236, 134);
			this.groupBoxMain.TabIndex = 2;
			this.groupBoxMain.TabStop = false;
			this.groupBoxMain.Text = "Status";
			// 
			// labelAvailable
			// 
			this.labelAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.labelAvailable.Location = new System.Drawing.Point(6, 98);
			this.labelAvailable.Name = "labelAvailable";
			this.labelAvailable.Size = new System.Drawing.Size(223, 23);
			this.labelAvailable.TabIndex = 3;
			this.labelAvailable.Text = "Max time 20-11-2020 03:19:08";
			// 
			// labelBlockTime
			// 
			this.labelBlockTime.Location = new System.Drawing.Point(6, 75);
			this.labelBlockTime.Name = "labelBlockTime";
			this.labelBlockTime.Size = new System.Drawing.Size(223, 23);
			this.labelBlockTime.TabIndex = 2;
			this.labelBlockTime.Text = "Block time 30/10/2020 12:52:54";
			// 
			// labelDifficulty
			// 
			this.labelDifficulty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.labelDifficulty.Location = new System.Drawing.Point(6, 52);
			this.labelDifficulty.Name = "labelDifficulty";
			this.labelDifficulty.Size = new System.Drawing.Size(223, 23);
			this.labelDifficulty.TabIndex = 1;
			this.labelDifficulty.Text = "Difficulty 12.5";
			// 
			// labelHeight
			// 
			this.labelHeight.Location = new System.Drawing.Point(7, 29);
			this.labelHeight.Name = "labelHeight";
			this.labelHeight.Size = new System.Drawing.Size(222, 23);
			this.labelHeight.TabIndex = 0;
			this.labelHeight.Text = "Height 528422";
			// 
			// buttonStart
			// 
			this.buttonStart.Location = new System.Drawing.Point(10, 173);
			this.buttonStart.Name = "buttonStart";
			this.buttonStart.Size = new System.Drawing.Size(236, 23);
			this.buttonStart.TabIndex = 3;
			this.buttonStart.Text = "Start";
			this.buttonStart.UseVisualStyleBackColor = true;
			this.buttonStart.Click += new System.EventHandler(this.ButtonStartClick);
			// 
			// panelResultGrid
			// 
			this.panelResultGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.panelResultGrid.Controls.Add(this.dataGridViewResults);
			this.panelResultGrid.Location = new System.Drawing.Point(0, 227);
			this.panelResultGrid.Name = "panelResultGrid";
			this.panelResultGrid.Size = new System.Drawing.Size(532, 230);
			this.panelResultGrid.TabIndex = 4;
			// 
			// dataGridViewResults
			// 
			this.dataGridViewResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewResults.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewResults.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewResults.Name = "dataGridViewResults";
			this.dataGridViewResults.Size = new System.Drawing.Size(532, 230);
			this.dataGridViewResults.TabIndex = 0;
			this.dataGridViewResults.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewResultsColumnHeaderMouseClick);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.buttonExportResults);
			this.panel1.Controls.Add(this.panelgrid);
			this.panel1.Controls.Add(this.panelResultGrid);
			this.panel1.Location = new System.Drawing.Point(252, 38);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(532, 457);
			this.panel1.TabIndex = 5;
			// 
			// buttonExportResults
			// 
			this.buttonExportResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonExportResults.Enabled = false;
			this.buttonExportResults.Location = new System.Drawing.Point(454, 196);
			this.buttonExportResults.Name = "buttonExportResults";
			this.buttonExportResults.Size = new System.Drawing.Size(75, 23);
			this.buttonExportResults.TabIndex = 5;
			this.buttonExportResults.Text = "Export dates";
			this.buttonExportResults.UseVisualStyleBackColor = true;
			this.buttonExportResults.Click += new System.EventHandler(this.ButtonExportResultsClick);
			// 
			// UserControlTxGrid
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.buttonStart);
			this.Controls.Add(this.groupBoxMain);
			this.Controls.Add(this.panelMsg);
			this.Name = "UserControlTxGrid";
			this.Size = new System.Drawing.Size(787, 498);
			this.Load += new System.EventHandler(this.UserControlTxGridLoad);
			this.panelMsg.ResumeLayout(false);
			this.panelgrid.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewTemplates)).EndInit();
			this.groupBoxMain.ResumeLayout(false);
			this.panelResultGrid.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).EndInit();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		private System.Windows.Forms.Panel panelgrid;
		private System.Windows.Forms.Label labelMsg;
		private System.Windows.Forms.Panel panelMsg;
		private System.Windows.Forms.GroupBox groupBoxMain;
		private System.Windows.Forms.Label labelHeight;
		private System.Windows.Forms.Label labelAvailable;
		private System.Windows.Forms.Label labelBlockTime;
		private System.Windows.Forms.Label labelDifficulty;
		private System.Windows.Forms.Button buttonStart;
		private System.Windows.Forms.Panel panelResultGrid;
		private System.Windows.Forms.DataGridView dataGridViewResults;
		private System.Windows.Forms.DataGridView dataGridViewTemplates;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonExportResults;
	}
}

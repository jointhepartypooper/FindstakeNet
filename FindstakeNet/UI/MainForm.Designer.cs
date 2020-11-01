namespace FindstakeNet.UI
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItemGetFromwallet = new System.Windows.Forms.ToolStripMenuItem();
			this.importUnspentjsonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportUnspentjsonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.getFromWalletToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemReconnect = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.reconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.visitGithubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panelBody = new System.Windows.Forms.Panel();
			this.statusStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripStatusLabel1,
			this.toolStripProgressBar1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 614);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(952, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
			this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
			// 
			// toolStripProgressBar1
			// 
			this.toolStripProgressBar1.Name = "toolStripProgressBar1";
			this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.fileToolStripMenuItemGetFromwallet,
			this.toolStripMenuItemReconnect,
			this.aboutToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(952, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItemGetFromwallet
			// 
			this.fileToolStripMenuItemGetFromwallet.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.importUnspentjsonToolStripMenuItem,
			this.exportUnspentjsonToolStripMenuItem,
			this.getFromWalletToolStripMenuItem});
			this.fileToolStripMenuItemGetFromwallet.Name = "fileToolStripMenuItemGetFromwallet";
			this.fileToolStripMenuItemGetFromwallet.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItemGetFromwallet.Text = "File";
			// 
			// importUnspentjsonToolStripMenuItem
			// 
			this.importUnspentjsonToolStripMenuItem.Name = "importUnspentjsonToolStripMenuItem";
			this.importUnspentjsonToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.importUnspentjsonToolStripMenuItem.Text = "Import listunspent";
			this.importUnspentjsonToolStripMenuItem.Click += new System.EventHandler(this.ImportUnspentjsonToolStripMenuItemClick);
			// 
			// exportUnspentjsonToolStripMenuItem
			// 
			this.exportUnspentjsonToolStripMenuItem.Name = "exportUnspentjsonToolStripMenuItem";
			this.exportUnspentjsonToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.exportUnspentjsonToolStripMenuItem.Text = "Export listunspent";
			this.exportUnspentjsonToolStripMenuItem.Click += new System.EventHandler(this.ExportUnspentjsonToolStripMenuItemClick);
			// 
			// getFromWalletToolStripMenuItem
			// 
			this.getFromWalletToolStripMenuItem.Name = "getFromWalletToolStripMenuItem";
			this.getFromWalletToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.getFromWalletToolStripMenuItem.Text = "Load from wallet";
			this.getFromWalletToolStripMenuItem.Click += new System.EventHandler(this.GetFromWalletToolStripMenuItemClick);
			// 
			// toolStripMenuItemReconnect
			// 
			this.toolStripMenuItemReconnect.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripMenuItem2,
			this.reconnectToolStripMenuItem});
			this.toolStripMenuItemReconnect.Name = "toolStripMenuItemReconnect";
			this.toolStripMenuItemReconnect.Size = new System.Drawing.Size(46, 20);
			this.toolStripMenuItemReconnect.Text = "Tools";
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(119, 22);
			this.toolStripMenuItem2.Text = "Settings";
			this.toolStripMenuItem2.Click += new System.EventHandler(this.ToolStripMenuItem2Click);
			// 
			// reconnectToolStripMenuItem
			// 
			this.reconnectToolStripMenuItem.Name = "reconnectToolStripMenuItem";
			this.reconnectToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
			this.reconnectToolStripMenuItem.Text = "Connect";
			this.reconnectToolStripMenuItem.Click += new System.EventHandler(this.ReconnectToolStripMenuItemClick);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.visitGithubToolStripMenuItem});
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
			this.aboutToolStripMenuItem.Text = "About";
			// 
			// visitGithubToolStripMenuItem
			// 
			this.visitGithubToolStripMenuItem.Name = "visitGithubToolStripMenuItem";
			this.visitGithubToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
			this.visitGithubToolStripMenuItem.Text = "Visit github";
			this.visitGithubToolStripMenuItem.Click += new System.EventHandler(this.VisitGithubToolStripMenuItemClick);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.Transparent;
			this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
			this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.panel1.Location = new System.Drawing.Point(21, 16);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(252, 61);
			this.panel1.TabIndex = 2;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(69)))), ((int)(((byte)(33)))));
			this.panel2.Controls.Add(this.panel1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 24);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(952, 100);
			this.panel2.TabIndex = 3;
			// 
			// panelBody
			// 
			this.panelBody.BackColor = System.Drawing.SystemColors.Control;
			this.panelBody.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelBody.Location = new System.Drawing.Point(0, 124);
			this.panelBody.Name = "panelBody";
			this.panelBody.Size = new System.Drawing.Size(952, 490);
			this.panelBody.TabIndex = 4;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(952, 636);
			this.Controls.Add(this.panelBody);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.Text = "FindstakeNet";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
			this.Shown += new System.EventHandler(this.MainFormShown);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private System.Windows.Forms.ToolStripMenuItem getFromWalletToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportUnspentjsonToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importUnspentjsonToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItemGetFromwallet;
		private System.Windows.Forms.Panel panelBody;
		private System.Windows.Forms.ToolStripMenuItem visitGithubToolStripMenuItem;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem reconnectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemReconnect;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		

	}
}


namespace FindstakeNet.UI
{
	partial class UserControlSettings
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
			this.rpcgroupbox = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxUser = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxurl = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.rpcgroupbox.SuspendLayout();
			this.SuspendLayout();
			// 
			// rpcgroupbox
			// 
			this.rpcgroupbox.Controls.Add(this.label3);
			this.rpcgroupbox.Controls.Add(this.textBoxPassword);
			this.rpcgroupbox.Controls.Add(this.label2);
			this.rpcgroupbox.Controls.Add(this.textBoxUser);
			this.rpcgroupbox.Controls.Add(this.label1);
			this.rpcgroupbox.Controls.Add(this.textBoxurl);
			this.rpcgroupbox.Location = new System.Drawing.Point(4, 4);
			this.rpcgroupbox.Name = "rpcgroupbox";
			this.rpcgroupbox.Size = new System.Drawing.Size(302, 100);
			this.rpcgroupbox.TabIndex = 0;
			this.rpcgroupbox.TabStop = false;
			this.rpcgroupbox.Text = "rpc";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 77);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(74, 20);
			this.label3.TabIndex = 5;
			this.label3.Text = "password";
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Location = new System.Drawing.Point(87, 73);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.Size = new System.Drawing.Size(209, 20);
			this.textBoxPassword.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(7, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(45, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "user";
			// 
			// textBoxUser
			// 
			this.textBoxUser.Location = new System.Drawing.Point(87, 47);
			this.textBoxUser.Name = "textBoxUser";
			this.textBoxUser.Size = new System.Drawing.Size(209, 20);
			this.textBoxUser.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "url";
			// 
			// textBoxurl
			// 
			this.textBoxurl.Location = new System.Drawing.Point(87, 21);
			this.textBoxurl.Name = "textBoxurl";
			this.textBoxurl.Size = new System.Drawing.Size(209, 20);
			this.textBoxurl.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(231, 122);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Cancel";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(134, 122);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 1;
			this.button2.Text = "Save";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.Button2Click);
			// 
			// UserControlSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.rpcgroupbox);
			this.Name = "UserControlSettings";
			this.Size = new System.Drawing.Size(345, 169);
			this.Load += new System.EventHandler(this.UserControlSettingsLoad);
			this.rpcgroupbox.ResumeLayout(false);
			this.rpcgroupbox.PerformLayout();
			this.ResumeLayout(false);

		}
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBoxUser;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxurl;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox rpcgroupbox;
	}
}

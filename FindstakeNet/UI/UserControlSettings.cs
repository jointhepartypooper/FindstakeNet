using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FindstakeNet.Interface;

namespace FindstakeNet.UI
{
	public partial class UserControlSettings : UserControl
	{
		private readonly ISettingsRepository settingsRepository;
		
		public UserControlSettings(ISettingsRepository settingsRepository)
		{
			this.settingsRepository = settingsRepository;
			InitializeComponent();
		}
		
		/// <summary>
		/// on load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void UserControlSettingsLoad(object sender, EventArgs e)
		{
			this.textBoxurl.Text = settingsRepository.GetRpcUri();
			this.textBoxUser.Text = settingsRepository.GetRpcUser();
			this.textBoxPassword.Text = settingsRepository.GetRpcPassword();
		}
		
		/// <summary>
		/// save
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Button2Click(object sender, EventArgs e)
		{
			//save
			if (!string.IsNullOrWhiteSpace(this.textBoxurl.Text)) 
				settingsRepository.SetRpcUri(this.textBoxurl.Text);
									
			if (!string.IsNullOrWhiteSpace(this.textBoxUser.Text))
				settingsRepository.SetRpcUser(this.textBoxUser.Text);
			
			if (!string.IsNullOrWhiteSpace(this.textBoxPassword.Text))
				settingsRepository.SetRpcPassword(this.textBoxPassword.Text);
			
			if (!string.IsNullOrWhiteSpace(this.textBoxurl.Text) ||
			    !string.IsNullOrWhiteSpace(this.textBoxUser.Text) ||
			    !string.IsNullOrWhiteSpace(this.textBoxPassword.Text))
				Button1Click(null, null);
		}
		
		/// <summary>
		/// cancel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Button1Click(object sender, EventArgs e)
		{
			//cancel
			(this.Parent.Parent as MainForm).ShowGrid();
		}
	}
}

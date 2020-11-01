using System;
using FindstakeNet.Interface;

namespace FindstakeNet.UI
{
	/// <summary>
	/// Description of UserControlFactory.
	/// </summary>
	public class UserControlFactory
	{
		private UserControlSettings userControlSettings;
		private UserControlTxGrid userControlgrid;
		private readonly ISettingsRepository settingsRepository;
		
		public UserControlFactory(ISettingsRepository settingsRepository)
		{
			this.settingsRepository = settingsRepository;
		}
		
		public UserControlSettings GetUserControlSettings()
		{
			//singleton
			if (userControlSettings != null)
			{
				return userControlSettings;
			}
			userControlSettings = new UserControlSettings(settingsRepository);
			return userControlSettings;
		}
		
				
		public UserControlTxGrid GetUserControlTxGrid()
		{
			//singleton
			if (userControlgrid != null)
			{
				return userControlgrid;
			}
			userControlgrid = new UserControlTxGrid();
			return userControlgrid;
		}
	}
}

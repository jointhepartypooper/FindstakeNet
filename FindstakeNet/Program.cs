using System;
using System.Windows.Forms;
using SimpleInjector;
using FindstakeNet.UI;
using FindstakeNet.Interface;
using FindstakeNet.Implementation;
using FindstakeNet.Implementation.RPC;

namespace FindstakeNet
{
	internal sealed class Program
	{
		private static Container container;
		
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Bootstrap();
			Application.Run(container.GetInstance<MainForm>());
		}
			
		
		private static void Bootstrap()
	    {
	        container = new SimpleInjector.Container();
	
	        container.Register<ISettingsRepository, SettingRepository>(Lifestyle.Singleton);
	        container.Register<IBlockRepository, BlockRepository>(Lifestyle.Singleton);
	        container.Register<IRPCClient, RPCClient>(Lifestyle.Singleton);
		    container.Register<IBlockParser, BlockParser>(Lifestyle.Singleton);
		    container.Register<ITransactionRepository, TransactionRepository>(Lifestyle.Singleton);
		    container.Register<ITransactionParser, TransactionParser>(Lifestyle.Singleton);
		    
	        container.Register<MainForm>(Lifestyle.Singleton);	        
	        container.Register<UserControlFactory>(Lifestyle.Singleton);
	        // Optional
	        container.Verify();
	    }		
	}	
}

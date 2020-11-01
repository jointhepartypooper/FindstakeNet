using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FindstakeNet.Model;
using FindstakeNet.Interface;
using FindstakeNet.Implementation.RPC;
using Newtonsoft.Json;
using PeercoinUtils;
using PeercoinUtils.Crypto;

namespace FindstakeNet.UI
{
	public partial class MainForm : Form
	{		        
		private CancellationTokenSource source;        
        private CancellationToken token;
        
		private readonly ISettingsRepository settingsRepository;
		private readonly ITransactionParser transactionParser;				
		private readonly ITransactionRepository transactionRepository;
		
		private readonly IRPCClient client;
		private readonly IBlockParser blockParser;
		private readonly UserControlFactory userControlFactory;
		
		private System.Threading.Timer syncTimer;
		private bool connected;
		private SettingState currentState;	 
		private bool forminitialized = false;
		private bool formclosing = false;
		private bool searching = false;				
		private bool modifiersSynchronized = false;
		private long lastblocktime = 0;
		private List<UnspentTransactionData> unspents; 
		private List<CheckStakeResult> results;
		private readonly float minMarginDifficulty = 2.0f;
		private List<StakeModifier> blockModifiers;
		private uint currentTime = 0;
		private List<MintTemplate> templates;
				
		public MainForm(ISettingsRepository settingsRepository, IRPCClient client, IBlockParser blockParser
		                , UserControlFactory userControlFactory, ITransactionParser transactionParser
		                , ITransactionRepository transactionRepository)
		{

			this.settingsRepository = settingsRepository;
			this.client = client;
			this.blockParser = blockParser;
			this.userControlFactory = userControlFactory;
			this.transactionParser = transactionParser;
			this.transactionRepository = transactionRepository;
			this.templates = new List<MintTemplate>();
			this.results = new List<CheckStakeResult>();
			this.blockModifiers = new List<StakeModifier>();
			
			InitializeComponent();			
						
			source = new CancellationTokenSource();
			token = source.Token;
			this.panel2.BackColor = System.Drawing.ColorTranslator.FromHtml("#3cb054");
		}
 	
		
		private void InitForm()
		{			
			this.currentState = settingsRepository.GetSyncState();
			//connect
			ReconnectToolStripMenuItemClick(null,null);
			
			ShowGrid();
			
			if (syncTimer == null) 
				syncTimer = new System.Threading.Timer(new TimerCallback(TickTimer), null, Timeout.Infinite, Timeout.Infinite);
		}
		
		
		public void FillGrid(bool started=false)
		{			
			var moduleInstance = this.userControlFactory.GetUserControlTxGrid();
			moduleInstance.BindTable(this.templates, this.results);
			moduleInstance.SetStarted(started);
		}
		
		public void ShowGrid()
		{					
			var moduleInstance = this.userControlFactory.GetUserControlTxGrid();
            if (!panelBody.Controls.Contains(moduleInstance))
            {
                panelBody.Controls.Add(moduleInstance);
                moduleInstance.Dock = DockStyle.Fill;
                moduleInstance.BringToFront();
            }
            else
                moduleInstance.BringToFront();
            
            if (this.currentState != null)
            {             
            	moduleInstance.SetStatus(this.currentState.CurBH, this.currentState.Diff, this.lastblocktime, this.lastblocktime+PeercoinConstants.Findstakelimit);
            }
            
            if (this.connected)
            	moduleInstance.SetWarning("");
            else 
            	moduleInstance.SetWarning("FindstakeNet not connected to wallet");                        

		}
		
		
		
		public void StartSearching()
		{
			if (this.searching)
				return;
					
			this.searching = true;
							
			FillGrid(true);
			
			if (!this.modifiersSynchronized)
			{								
				SetStatus("Loading stakemodifiers");
				var start = currentState.CurBH - (6 * 24 * 31) - 10;
		        var end = currentState.CurBH;
		        for (uint i = start; i < end; i++)
		        {
		        	SetProgress((int)i, (int)end,(int)start);
		        	blockParser.Parse(i);
		            if (this.token.IsCancellationRequested || this.formclosing)
		            {
		                return;
		            }	            
		        }
		        var status = settingsRepository.GetFindstakeStatus();
		        this.lastblocktime = status.lastupdatedblocktime;
		        this.currentState.Diff = status.difficulty;	
		        this.blockModifiers = status.blockModifiers;
		        this.modifiersSynchronized = true;
		        ShowGrid();
			}
				
			this.currentTime = ConvertToUnixTimestamp(DateTime.UtcNow);
			
			source = new CancellationTokenSource();
			token = source.Token;
								
			FillGrid(true);
									
			SetProgress(0,100);
			SetStatus("Searching...");
			syncTimer.Change(50, Timeout.Infinite);
		}
		
		
		public void StopSearching()
		{
			if (!this.searching)
				return;
			this.searching = false;
			source.Cancel();
		}
		
		
		
		
		
		private void ParseUnspents()		
		{		          							
			Task.Run(() => ParseTxos(token), token);			
		}
		
		
		private void ParseTxos(CancellationToken canceltoken)
		{
			if (this.unspents != null)
            {
				var blockhashes = this.unspents.Select(un=> un.blockhash).Distinct().ToList();
				var count = blockhashes.Count;
				var counter = 0;
				SetStatus("Importing blocks");
				blockhashes.ForEach(hash => {
				                    	SetProgress(counter, count);
				                    	blockParser.Parse(hash);
				                    	counter++;
				                    	
						                  	if (canceltoken.IsCancellationRequested)	            
						                  	{	                
						                  		return;				                  		
						                  	}
				                    	
				                    });
				                    
					 
				var addresses = this.unspents.Select(un=> un.address).Distinct().ToList();
				addresses.ForEach(address => {			
				                  	
				    
				                  	if (canceltoken.IsCancellationRequested)	            
				                  	{	                
				                  		return;				                  		
				                  	}
				                  	
				                  	var unspentsbyaddress = transactionRepository.GetUnspents(address);
				                  	unspentsbyaddress.ForEach( unspent =>{

											                  	if (canceltoken.IsCancellationRequested)	            
											                  	{	                
											                  		return;				                  		
											                  	}
				                  	                          	
				                  	                          	
											                  	var m = new MintTemplate(				                  	
											                  		unspent.Id,
											                  		address,
											                  		unspent.BlockFromTime,
											                  		unspent.PrevTxOffset,
											                  		unspent.PrevTxTime,
											                  		unspent.PrevTxOutIndex,
											                  		(uint)unspent.PrevTxOutValue);
						                  	
				                  	                          	m.SetBitsWithDifficulty(((Convert.ToSingle(this.currentState.Diff) - this.minMarginDifficulty)));
		
										                  		if (this.templates.All(t => t.Id != m.Id))
										                  			this.templates.Add(m);							                  		
				                  	                          });				               
				                  });         
				
            
				

				if (canceltoken.IsCancellationRequested)
				{					
					return;					
				}

				SetStatus("Ready");
				SetProgress(0, 100);
				FillGrid();
			}
		}
		
		
		
		private void TickTimer(object state)
		{
			//stop the timer
			syncTimer.Change(Timeout.Infinite, Timeout.Infinite);
			var start = this.currentTime;
			var end = Math.Min(this.currentTime + 1000, this.lastblocktime+PeercoinConstants.Findstakelimit);
			
			for (uint timestamp = start; timestamp < end; timestamp++) 
			{
				var modifier = Mint.GetModifier(this.blockModifiers, timestamp);
				
				if (modifier.HasValue)
				{
					foreach (var template in templates) 
					{
				
						if (this.formclosing || this.token.IsCancellationRequested)							
						{					
							return;							
						}
						
						var result = Mint.CheckStakeKernelHash(template, timestamp, modifier.Value);
						if (result.success)
						{
							if (this.results.All( r=> r.Id != template.Id && r.txTime!=timestamp))
							{
								results.Add(new CheckStakeResult(){
							            Id=	template.Id,
							            OfAddress = template.OfAddress,
							            minimumDifficulty = result.minimumDifficulty,
							            txTime = timestamp
							            });
								FillGrid(true);
							}							
						}
					}				
				}	
				if (this.formclosing || this.token.IsCancellationRequested)
				{
					return;
				}
					            
  
				this.currentTime++;
			}

			const int hour = 3600;
			
			if (!this.formclosing) 
				SetProgress(
				Convert.ToInt32(this.currentTime/hour), 
				Convert.ToInt32((this.lastblocktime+PeercoinConstants.Findstakelimit)/hour), 
				Convert.ToInt32(this.lastblocktime/hour));
		
				
			if (!this.formclosing && !this.token.IsCancellationRequested &&
				    syncTimer != null && this.currentTime < this.lastblocktime+PeercoinConstants.Findstakelimit)
			{			
				syncTimer.Change(50, Timeout.Infinite);
			}
			else
			{
				SetProgress(0,100);
				SetStatus("Done");
			}			
		}
		
		
		private void SetProgress(int current, int max, int min = 0)
		{				
			Application.DoEvents();
			this.Invoke(new Action(() => {
			                       	toolStripProgressBar1.Minimum = min;						                       	
			                       	toolStripProgressBar1.Maximum = max;			                       	
			                       	toolStripProgressBar1.Value = current;			                       
			                       }));
		}
		
				
		private void SetStatus(string text)
		{			
			this.Invoke(new Action(() => toolStripStatusLabel1.Text = text));	
		}
		
				
		private void SetSyncState()
		{		
			currentState.Diff = client.GetDifficulty().pos;					
			currentState.CurBH = client.GetBlockCount();
			settingsRepository.SetSyncState(currentState);				
		}
		
		 
				
		static uint ConvertToUnixTimestamp(DateTime datetimestamp)
		{
		    var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		    return Convert.ToUInt32((datetimestamp - origin).TotalSeconds);
		}
		
		
		void MainFormShown(object sender, EventArgs e)
		{
			if (!forminitialized)
			{
				forminitialized = true;
				InitForm();
			}
		}
		
		
		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			formclosing = true;
			source.Cancel();
		}
		
		
		/// <summary>
		/// on menuitem Connect click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ReconnectToolStripMenuItemClick(object sender, EventArgs e)
		{						
			this.connected = client.CheckConnection();
			SetStatus(connected ? "Connected" : "Not connected");
			this.importUnspentjsonToolStripMenuItem.Enabled = connected;
			this.exportUnspentjsonToolStripMenuItem.Enabled = connected;
			this.getFromWalletToolStripMenuItem.Enabled = connected;
			
			this.reconnectToolStripMenuItem.Enabled = !connected;
			
			if (connected)
			{								
				SetSyncState();
				blockParser.Parse(currentState.CurBH);
				if (this.currentState != null && this.currentState.CurBH > 0)
				{		
					blockParser.Parse(currentState.CurBH);
					this.lastblocktime = settingsRepository.GetFindstakeStatus().lastupdatedblocktime;

					ShowGrid();
				}
				
				 
			}
		}
		
		
		
		void VisitGithubToolStripMenuItemClick(object sender, EventArgs e)
		{
			OpenUrl("https://github.com/peercoin");
		}
		
		
		private void OpenUrl(string url)
		{
		    try
		    {		    			        		        	
		    	ProcessStartInfo psi = new ProcessStartInfo
		            {
		                FileName = "cmd",
		                Arguments = "/c start " + url
		            };
		        	   
		        	Process.Start(psi);
		    }
		    catch (Exception )
		    {

		    }
		}
		
		/// <summary>
		/// show settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ToolStripMenuItem2Click(object sender, EventArgs e)
		{
			var moduleInstance = this.userControlFactory.GetUserControlSettings();
            if (!panelBody.Controls.Contains(moduleInstance))
            {
                panelBody.Controls.Add(moduleInstance);
                moduleInstance.Dock = DockStyle.Fill;
                moduleInstance.BringToFront();
            }
            else
                moduleInstance.BringToFront();
		}
		
		
		void ExportUnspentjsonToolStripMenuItemClick(object sender, EventArgs e)
		{		
			if (!this.connected) return;
			
			var saveDialog = new SaveFileDialog();
			saveDialog.Title = "Export listunspent";
			saveDialog.FileName = "listunspent.json";
			saveDialog.Filter = "json files|*.json";
			
			if (saveDialog.ShowDialog() == DialogResult.OK)
			{
			    string name = saveDialog.FileName;
			    
			    var unspentsclient = client.GetUnspents();
	 
			    string output = JsonConvert.SerializeObject(unspentsclient, Formatting.Indented);
			    File.WriteAllText(name, output);
			    			  
			    SetUnspents(unspentsclient);
 			    
				ParseUnspents();
				
			}
		}
		
		void SetUnspents(IEnumerable<Unspent> list)
		{					    
			this.unspents = list
					.Select(unspent => new UnspentTransactionData{
					        	txid = unspent.txid,					        	
					        	vout = unspent.vout,
					        	address = unspent.address
					        })
					.ToList();
				        	
			int counter=0;
			SetStatus("Loading blocks");
			this.unspents.ForEach(unspent => {
			                      	SetProgress(counter, unspents.Count);
					                 	unspent.blockhash = transactionParser.GetBlockHash(unspent.txid);
					                 	unspent.blockheight = blockParser.Parse(unspent.blockhash);
					                 	counter++;
					                 });
		}
		
		void ImportUnspentjsonToolStripMenuItemClick(object sender, EventArgs e)
		{			    
			var openFileDialog1 = new OpenFileDialog
		    {  
		        Title = "Import listunspent",  
		  
		        CheckFileExists = true,  
		        CheckPathExists = true,  
		  
		        DefaultExt = "json",  
		      
	 			Filter = "json files|*.json",
		    };  
			
		  
		    if (openFileDialog1.ShowDialog() == DialogResult.OK)  
		    {   
		        try 
		        {		        	
		        	var listunspents = JsonConvert.DeserializeObject<List<Unspent>>(File.ReadAllText(openFileDialog1.FileName));		        			    
		        	SetUnspents(listunspents);		        	
		        	ParseUnspents();		        
		        } 
		        catch (Exception err)
		        {
		        	//show error					
		        	var moduleInstance = this.userControlFactory.GetUserControlTxGrid();
		        	moduleInstance.SetWarning(err.Message);
		        }
		    }  
		}
		
		
		void GetFromWalletToolStripMenuItemClick(object sender, EventArgs e)
		{			
			if (!this.connected) return;
			
			SetUnspents(client.GetUnspents());
 
			ParseUnspents();
			FillGrid();
		}
	}
}

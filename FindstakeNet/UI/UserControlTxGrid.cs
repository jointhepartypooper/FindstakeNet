using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using PeercoinUtils;

namespace FindstakeNet.UI
{
	/// <summary>
	/// Description of UserControlTxGrid.
	/// </summary>
	public partial class UserControlTxGrid : UserControl
	{
		private List<MintTemplate> templates;
		private List<CheckStakeResult> results;
		
				
		private List<PossibleStake> resultsStakes;
		
		public UserControlTxGrid()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
						
			this.dataGridViewTemplates.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;			        
			this.dataGridViewResults.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			
			this.panelMsg.Hide();
			this.buttonStart.Enabled = false;
		 		 
		}
		
		static string ConvertFromUnixTimestamp(long timestamp)
		{
		    var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		    return origin.AddSeconds(timestamp).ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
		}
		
		public void SetWarning(string msg)
		{						
			this.Invoke(new Action(() => {
				if (string.IsNullOrEmpty(msg))
				{
					this.panelMsg.Hide();				
				}
				else
				{
					labelMsg.Text = msg;
					this.panelMsg.Show();
				} 	                       
			                       }));
		}
		
		public void SetStatus(uint height, decimal difficlty, long blocktime, long max)
		{			
			this.Invoke(new Action(() => {
										this.labelHeight.Text = "Last block " + height;	
													
										this.labelBlockTime.Text = "Block time " + ConvertFromUnixTimestamp(blocktime);
										this.labelBlockTime.Visible = (blocktime>10);
										
										this.labelDifficulty.Text = "Difficulty " + difficlty.ToString("0.00");
										
										this.labelAvailable.Visible = (blocktime>10);
										this.labelAvailable.Text = "Maximum search span " + ConvertFromUnixTimestamp(max); 	                       
			                       }));		

		}
		
		
		public void SetStarted(bool started)
		{			
			this.Invoke(new Action(() => {
				this.buttonStart.Enabled = !started;
			                  
			                       }));		

		}		
		
		public void BindTable(List<MintTemplate> templates=null, List<CheckStakeResult> results=null)
		{		                
			if (templates!=null) this.templates = templates;
			if (results!=null) this.results = results;

			var stakeTemplates = new List<StakeTemplate>();
			
            if (this.templates != null)
            {
            	for (int i = 0; i < this.templates.Count; i++) 
            	{
            		if (this.templates[i].Id != null && this.templates[i].Id.StartsWith("to", StringComparison.CurrentCultureIgnoreCase))
            		{
            			int found = 0;
            			if (results != null)
            			{
            				found = results
            					.Where(r=> r.Id == this.templates[i].Id && r.minimumDifficulty > 0.0001)
            					.ToList()
            					.Count;
            			}
            			
            			var tx = this.templates[i].Id.Substring(2,64);
            			var txindex = (this.templates[i].Id.Substring(2 + 64 + 1));
            			
            			var s = new StakeTemplate(this.templates[i].Id, this.templates[i].OfAddress, tx,txindex,found.ToString());
            			stakeTemplates.Add(s);
            		}
            
            	}
            }                

            var possibleStakes = new List<PossibleStake>();
            
            if (this.templates!=null && this.results != null)
            {            	
            	for (int i = 0; i < this.results.Count; i++) 
            	{            	
            		var template = this.templates.FirstOrDefault(tp => tp.Id == this.results[i].Id);
            		if (template!=null && this.results[i].txTime > 0)
            		{
            			var p = new PossibleStake(this.results[i].Id,
            			                          template.OfAddress,
            			                          ConvertFromUnixTimestamp(this.results[i].txTime),
            			                          this.results[i].minimumDifficulty.ToString("0.00"));
            			possibleStakes.Add(p);
            		}            		
            	}
            	this.resultsStakes = possibleStakes;
            }
                        
			this.Invoke(new Action(() => {
                       this.dataGridViewTemplates.DataSource = stakeTemplates;  	
                       this.dataGridViewResults.DataSource = possibleStakes;
                       			
                       this.dataGridViewTemplates.Columns["ID"].Visible = false;   
                       this.dataGridViewTemplates.Columns["Address"].MinimumWidth = 220;
                       this.dataGridViewTemplates.Columns["Transaction"].MinimumWidth = 260;
                       this.dataGridViewTemplates.Columns["StakesFound"].HeaderText = "Stakes found";            
                       this.dataGridViewTemplates.Columns["Index"].MinimumWidth = 40;                                              
                       this.dataGridViewTemplates.Columns["Index"].Width = 40;
                       
                       this.dataGridViewResults.Columns["ID"].Visible = false;                           
                       this.dataGridViewResults.Columns["Address"].MinimumWidth = 220;
                       this.dataGridViewResults.Columns["StakeDate"].MinimumWidth = 220;
                       this.buttonExportResults.Enabled = (this.results!=null && this.results.Count>0);
           	}));           
          
		}
		
		
		void UserControlTxGridLoad(object sender, EventArgs e)
		{
			BindTable();
		}
		
		
		void ButtonStartClick(object sender, EventArgs e)
		{
			var main = this.Parent.Parent as MainForm;
			main.StartSearching();
		}
				
		
		void ButtonExportResultsClick(object sender, EventArgs e)
		{
			if (this.resultsStakes!=null && this.resultsStakes.Count>0)
			{
				var sfd = new SaveFileDialog();			
				sfd.Filter = "CSV (*.csv)|*.csv";				
				sfd.FileName = "StakeDates.csv";
				
       			if (sfd.ShowDialog() == DialogResult.OK)  
		        {   		                
					try 
					{
						const string columnNames = "Address,StakeDate,MaxDifficulty";  
						var outputCsv = new string[this.results.Count + 1];
						var counter = 0;
						outputCsv[counter] += columnNames;  
	                              
						this.resultsStakes.ForEach(r => {	
							counter++;
							outputCsv[counter] += r.Address + "," + r.StakeDate + "," + r.MaxDifficulty;
						});                    
 	  
						File.WriteAllLines(sfd.FileName, outputCsv);//f the target file already exists, it is overwritten.

					// disable once EmptyGeneralCatchClause
					} catch {
						//show error
					}  
		        }  			
			}
			 
		}
		
		
		void DataGridViewResultsColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
	
			var curstakes = dataGridViewResults.DataSource as List<PossibleStake>;
			string col = dataGridViewResults.Columns[e.ColumnIndex].DataPropertyName;
    	    string order =  " ASC";
    	    
    	    if (dataGridViewResults.Tag != null) 	    	
    	    	order = dataGridViewResults.Tag.ToString().Contains(" ASC") ? " DESC" : " ASC";

    		dataGridViewResults.Tag = col + order;


    		if (order.Contains(" ASC"))
    			curstakes = curstakes
    				.OrderBy(x => col == "Address"
    			                      ? x.Address    			                                           
    			                      : x.ID)
    				.ToList();
    
    		else    			
    			curstakes = curstakes
    				.OrderByDescending(x => col == "Address"
    			                      ? x.Address    			                                           
    			                      : x.ID)
    				.ToList();
    
    		dataGridViewResults.DataSource = curstakes;	
		}
		
		
		void DataGridViewTemplatesColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{					
			var curtemplates = dataGridViewTemplates.DataSource as List<StakeTemplate>;
			string col = dataGridViewTemplates.Columns[e.ColumnIndex].DataPropertyName;
    	    string order =  " ASC";
    	    
    	    if (dataGridViewTemplates.Tag != null) 	    	
    	    	order = dataGridViewTemplates.Tag.ToString().Contains(" ASC") ? " DESC" : " ASC";

    		dataGridViewTemplates.Tag = col + order;


    		if (order.Contains(" ASC"))
    			curtemplates = curtemplates
    				.OrderBy(x => col == "Address"
    			                      ? x.Address    			                      
    			                      : col == "StakesFound"                    
    			                      ? x.StakesFound    				                      
    			                      : col == "Transaction"
    			                      ? x.Transaction	                      
    			                      : x.ID)
    				.ToList();
    
    		else    			
    			curtemplates = curtemplates
    				.OrderByDescending(x => col == "Address"
    			                      ? x.Address    			                      
    			                      : col == "StakesFound"                    
    			                      ? x.StakesFound    				                      
    			                      : col == "Transaction"
    			                      ? x.Transaction	                      
    			                      : x.ID)
    				.ToList();

    
    		dataGridViewTemplates.DataSource = curtemplates;			
		}
		
		
		public class StakeTemplate
		{
			public string ID { get; set; }  
		    public string Address { get; set; }  
		    public string Transaction { get; set; }
		    public string Index { get; set; }   
		    public string StakesFound { get; set; }   
		    
		    public StakeTemplate(string id, string address, string transaction, string index, string found)
		    {		    		       
		    	this.ID = id;
		        this.Address = address;  
		        this.Transaction = transaction;  
		        this.Index = index;
		        this.StakesFound = found;
		    }		
		}
		
		
		public class PossibleStake  
		{  
		    public string ID { get; set; }  
		    public string Address { get; set; }  
		    public string StakeDate { get; set; }
		    public string MaxDifficulty { get; set; }   	    
		    
		    public PossibleStake(string id, string address, string timestamp, string max)  
		    {  
		        this.ID = id;  
		        this.Address = address;  
		        this.StakeDate = timestamp;  
		        this.MaxDifficulty = max;
		    }  
		} 
	}
}

using System;
using System.IO;
 
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using CommandLine;
using PeercoinUtils;
using PeercoinUtils.Crypto;

namespace FindstakeMono
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(Findstake);
        }


        public static void Findstake(Options o)
        {
            var blockRepository = new BlockRepository();
            var transactionRepository = new TransactionRepository(blockRepository);
            
            var rpcclient = new RPCClient("http://127.0.0.1:"+o.Port, o.User, o.Password);
            var parser = new BlockChainParser(rpcclient,blockRepository,transactionRepository);

            var blockcount = rpcclient.GetBlockCount();
            var difficulty = rpcclient.GetDifficulty().pos;
            if (blockcount<999)
            {
                Console.WriteLine("Not connected to wallet" + blockcount);
                return;
            }

            var unspents = new List<UnspentTransactionData>();
            var templates = new List<MintTemplate>();
            try 
            {		        	
                var listunspents = JsonConvert.DeserializeObject<List<Unspent>>(File.ReadAllText(System.IO.Directory.GetCurrentDirectory()+Path.DirectorySeparatorChar+o.FileListUnspent));		        			    
                unspents = listunspents
                .Select(unspent => new UnspentTransactionData{
                            txid = unspent.txid,					        	
                            vout = unspent.vout,
                            address = unspent.address
                        })
                .ToList();	        

                Console.WriteLine("");
                Console.Write("Importing blocks");
                using (var progress = new ProgressBar()) 
                {
                    var max = unspents.Count;
                    var counter=0;
                    unspents.ForEach(unspent => {			                       
                        unspent.blockhash = parser.GetBlockHash(unspent.txid);
                        unspent.blockheight = parser.Parse(unspent.blockhash);
                        parser.Parse(unspent.blockhash);
                        progress.Report((double) counter / max);
                        counter++;
                    });	
                }

                float minMarginDifficulty = 2.0f;
                Console.WriteLine("");
                Console.Write("Importing adresses");
                using (var progress = new ProgressBar()) 
                {
                    var addresses = unspents.Select(un=> un.address).Distinct().ToList();
                    var max = addresses.Count;
                    var counter=0;

    				addresses.ForEach(address => {			
				                  	
				        progress.Report((double) counter / max);
                        var unspentsbyaddress = transactionRepository.GetUnspents(address);
                        unspentsbyaddress.ForEach( unspent =>{
                                                
                            var m = new MintTemplate(				                  	
                                unspent.Id,
                                address,
                                unspent.BlockFromTime,
                                unspent.PrevTxOffset,
                                unspent.PrevTxTime,
                                unspent.PrevTxOutIndex,
                                (uint)unspent.PrevTxOutValue);
        
                            m.SetBitsWithDifficulty(((Convert.ToSingle(difficulty) - minMarginDifficulty)));

                            if (templates.All(t => t.Id != m.Id))
                                templates.Add(m);							                  		
                         });		
                         counter++;		               
                    });      
                }	        
            } 
            catch (Exception)
            {
                //show error
				
                Console.WriteLine("Loading "+System.IO.Directory.GetCurrentDirectory()+Path.DirectorySeparatorChar+o.FileListUnspent+" listunspent data not succeeded");
                return;
            }
       	        	

            Console.WriteLine("");
            Console.Write("Loading stakemodifiers");
            using (var progress = new ProgressBar()) 
            {
				var start = blockcount - (6 * 24 * 31) - 10;
		        var end = blockcount;
		        for (uint i = start; i < end; i++)
		        {
                    progress.Report((double) (i-start) / (end-start));
		        	parser.Parse(i);
            
		        }
            } 

			var lastblock = blockRepository.GetBlockState(blockcount-6);//not the very last
            var lastblocktime = lastblock.bt; 
			var blockModifiers = blockRepository.GetStakeModifiers(lastblock.h);

		    var currentTime = ConvertToUnixTimestamp(DateTime.UtcNow);
            var results=new List<CheckStakeResult>();
            var resultsStakes = new List<PossibleStake>();

            Console.WriteLine("");
            Console.Write("Searching");
            using (var progress = new ProgressBar()) 
            {
				var start = currentTime;
		        var end = lastblocktime+PeercoinConstants.Findstakelimit;
		        for (uint timestamp = start; timestamp < end; timestamp++) 
		        {
                    progress.Report((double) (timestamp-start) / (end-start));
                    var modifier = Mint.GetModifier(blockModifiers, timestamp);

					foreach (var template in templates) 
					{				
						var result = Mint.CheckStakeKernelHash(template, timestamp, modifier.Value);
						if (result.success)
						{
							if (results.All( r=> r.Id != template.Id && r.txTime!=timestamp))
							{
                                Console.WriteLine(" "+template.OfAddress+": "+ConvertFromUnixTimestamp(timestamp)+" difficulty: "+result.minimumDifficulty.ToString("0.00"));
								results.Add(new CheckStakeResult(){
							            Id=	template.Id,
							            OfAddress = template.OfAddress,
							            minimumDifficulty = result.minimumDifficulty,
							            txTime = timestamp
							            });
								resultsStakes=Enrich(resultsStakes, templates, results);
                                ExportResults(resultsStakes, results);
							}							
						}
					} 
		        }
            } 
            Console.WriteLine("Done");
            if (results.Count>0) Console.WriteLine("Findstake results exported to StakeDates.csv");
        }//end




		static List<PossibleStake> Enrich(List<PossibleStake> resultsStakes, List<MintTemplate> templates, List<CheckStakeResult> results)
		{		                

			var stakeTemplates = new List<StakeTemplate>();
			
            if (templates != null)
            {
            	for (int i = 0; i < templates.Count; i++) 
            	{
            		if (templates[i].Id != null && templates[i].Id.StartsWith("to", StringComparison.CurrentCultureIgnoreCase))
            		{
            			int found = 0;
            			if (results != null)
            			{
            				found = results
            					.Where(r=> r.Id == templates[i].Id && r.minimumDifficulty > 0.0001)
            					.ToList()
            					.Count;
            			}
            			
            			var tx = templates[i].Id.Substring(2,64);
            			var txindex = (templates[i].Id.Substring(2 + 64 + 1));
            			
            			var s = new StakeTemplate(templates[i].Id, templates[i].OfAddress, tx,txindex,found.ToString());
            			stakeTemplates.Add(s);
            		}
            
            	}
            }                

            var possibleStakes = new List<PossibleStake>();
            
            if (templates!=null && results != null)
            {            	
            	for (int i = 0; i < results.Count; i++) 
            	{            	
            		var template = templates.FirstOrDefault(tp => tp.Id == results[i].Id);
            		if (template!=null && results[i].txTime > 0)
            		{
            			var p = new PossibleStake(results[i].Id,
            			                          template.OfAddress,
            			                          ConvertFromUnixTimestamp(results[i].txTime),
            			                          results[i].minimumDifficulty.ToString("0.00"));
            			possibleStakes.Add(p);
            		}            		
            	}
            	return possibleStakes;
            }
                        
            return possibleStakes;
          
		}
		


		static void ExportResults(List<PossibleStake> resultsStakes, List<CheckStakeResult> results)
		{
			if (resultsStakes!=null && resultsStakes.Count>0)
			{			
				var fileName = "StakeDates.csv";
				
					try 
					{
						const string columnNames = "Address,StakeDate,MaxDifficulty";  
						var outputCsv = new string[results.Count + 1];
						var counter = 0;
						outputCsv[counter] += columnNames;  
	                              
						resultsStakes.ForEach(r => {	
							counter++;
							outputCsv[counter] += r.Address + "," + r.StakeDate + "," + r.MaxDifficulty;
						});                    
 	  
						File.WriteAllLines(fileName, outputCsv);//f the target file already exists, it is overwritten.

					// disable once EmptyGeneralCatchClause
					} catch {
						//show error
					}  	
			}
			 
		}


		static uint ConvertToUnixTimestamp(DateTime datetimestamp)
		{
		    var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		    return Convert.ToUInt32((datetimestamp - origin).TotalSeconds);
		}


		static string ConvertFromUnixTimestamp(long timestamp)
		{
		    var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		    return origin.AddSeconds(timestamp).ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
		}

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



    public class Options
    {
        [Option('u', "rpcuser", Required = true, HelpText = "rpcuser")]
        public string User { get; set; }
                    
        [Option('w', "rpcpassword", Required = true, HelpText = "rpcpassword")]
        public string Password { get; set; }

        [Option('p', "rpcport", Required = false, Default="8332", HelpText = "rpcport")]
        public string Port { get; set; }

        [Option('i', "listunspent", Required = true, Default="", HelpText = "json file with listunspent")]
        public string FileListUnspent { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}
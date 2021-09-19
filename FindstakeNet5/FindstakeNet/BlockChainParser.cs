using System;
using System.Collections.Generic; 
using System.Linq;
using PeercoinUtils;
using Newtonsoft.Json;
using PeercoinUtils.Crypto;


namespace FindstakeMono
{




	public class BlockChainParser 
	{				
		private readonly RPCClient client;
		private readonly TransactionRepository transactionRepository;
		private readonly BlockRepository blockRepository;				 
		private readonly Dictionary<string, uint> parsedHash;
		private readonly Dictionary<uint, bool> parsedHeight;
		
		public BlockChainParser(RPCClient client, BlockRepository blockRepository, TransactionRepository transactionRepository)
		{				
			this.client = client;
			this.transactionRepository = transactionRepository;
            this.blockRepository = blockRepository;
 
			this.parsedHash = new Dictionary<string, uint>();
			this.parsedHeight = new Dictionary<uint, bool>();
		}

				
		public string GetBlockHash(string txId)
		{
			var tx = client.GetRawTransaction(txId, 1);
			return tx.blockhash;
		}
		
		
		public void Parse(uint height, List<string> txIds, uint blocktime)
		{
			var sizeVarintTx = Mint.GetSizeVarInt((long)(txIds.Count));			
			var offset = PeercoinConstants.BlockHeaderSize + sizeVarintTx;
			
			for (int index = 0; index < txIds.Count; index++) 
			{							              	
				var tx = client.GetRawTransaction(txIds[index]);		              	
			    var txraw = client.DecodeRawTransaction(tx.hex);		 
				var rawsize = tx.hex.Length / 2; // 2 char is 1 byte
			           		
				StoreTxState(blocktime, height, txraw, tx, (uint)index, offset, (uint)rawsize);
 
				DeleteSpentFromStore(txraw);
 
				ParseVouts(txraw);
				
				offset += rawsize;
			}					
	
		}
		
		
		private void ParseVouts(DecodeRawTransactionResponse txraw)
		{			
			if (txraw != null && txraw.vout != null && txraw.vout.Length > 0)
			{
				foreach (var txout in txraw.vout) 
				{
					StoreOutput(txraw.txid, txout);
					StoreAddresses(txraw.txid, txout);
				}					
			}
		}
		
		
		private void StoreOutput(string transactionid, Output txout)
		{									
			if (txout != null && (txout.value * PeercoinConstants.Coin >= 0))
			{	
				var hasoptreturn = txout.scriptPubKey!=null && !string.IsNullOrWhiteSpace(txout.scriptPubKey.asm) && txout.scriptPubKey.asm.StartsWith("OP_RETURN");
							
				var json = hasoptreturn ? JsonConvert.SerializeObject(txout, Formatting.None) : null;
				
				var outputState = new OutputState
				{				
					Id = "to" + transactionid + "_"	+txout.n,				
					hash = transactionid,
					idx = (uint)txout.n,
					spent = false,
					units = Convert.ToUInt64(Math.Floor(txout.value * PeercoinConstants.Coin)),
					hasoptreturn = hasoptreturn,
					data = json
				};
				transactionRepository.SetOutputState(outputState);
	          };
		}
		
				
		private void StoreAddresses(string transactionid, Output txout)
		{						        
			if (txout != null && txout.scriptPubKey != null && txout.scriptPubKey.addresses != null && txout.scriptPubKey.addresses.Length > 0)
	        {		
	        	var addresses = txout.scriptPubKey.addresses.ToList();   
				foreach (var address in addresses) 
				{
					var addressState = new AddressTxoState {
						address = address,
						txo = transactionid,
						idx = txout.n,
					};
					transactionRepository.SetAddressState(addressState);
				}	        				
	        }			
		}
				
		
		private void StoreTxState(uint blocktime, uint height, DecodeRawTransactionResponse txraw, RawTransactionResponse tx, 
		                          uint index, long offset, uint rawsize)
		{
			var time = blocktime < PeercoinConstants.ProtocolV10SwitchTime && txraw.time.HasValue
				? (uint)txraw.time.Value
				: blocktime;

			var state = new TxState { 
				hash = txraw.txid,
             	height = height,			                                 	
             	offset = (uint)offset,
				position = index,					
				size = rawsize,
				time = (uint)txraw.time.Value
			};
			
			transactionRepository.SetTxState(state);
		}
		
		
		/// <summary>
		/// to keep storage small, only unspents are of interest
		/// </summary>
		/// <param name="txraw"></param>
		private void DeleteSpentFromStore(DecodeRawTransactionResponse txraw)
		{
			if (txraw != null && txraw.vin != null && txraw.vin.Length > 0)
			{
				var inputs = txraw.vin.ToList().Where(tin => !string.IsNullOrWhiteSpace(tin.txid)).ToList();
				foreach (var txin in inputs) 
				{
					transactionRepository.DeleteSpentOutputState(txin.txid, txin.vout);
				}
			}
		}		


		/// <summary>
		/// parse entire block including its tx
		/// </summary>
		/// <param name="hash"></param>
		/// <returns></returns>
		public uint Parse(string hash)
		{
			if (parsedHash.ContainsKey(hash)) 
			{
				return parsedHash[hash];
			}
			
			var block = GetBlock(hash);
			
			Store(block);
						
			if (block.nTx > 0)
			{
				/*transactionParser.*/Parse((uint)block.height, block.tx.ToList(), (uint)block.time);
			}
			
			if (!parsedHash.ContainsKey(hash)) 
				parsedHash.Add(hash, (uint)block.height);
			
			return (uint)block.height;
		}


		/// <summary>
		/// just to get modifiers
		/// </summary>
		/// <param name="height"></param>
		public void Parse(uint height)
		{						
			if (parsedHeight.ContainsKey(height))
			{
				return;
			}
			
			var hash = GetHash(height);
			var block = GetBlock(hash);
			
			Store(block);
			
			if (!parsedHeight.ContainsKey(height))
				parsedHeight.Add(height, true);
		}
		
		
		private void Store(BlockResponse block)
		{					
			var newSate = new BlockState{
				h = (uint)block.height,
				hash = block.hash,
				f = block.flags == "proof-of-stake" ? "pos" : "pow",
				bt = (uint)block.time,
				mr = block.modifier,
				tx = block.tx.ToList(),
				nTx = (uint)block.nTx
			};
			blockRepository.SetBlockState(newSate);
		}
		
		private string GetHash(uint index)
		{
			return client.GetBlockHash(index);
		}
		
		private BlockResponse GetBlock(string hash)
		{
			return client.GetBlock(hash);
		}		

	}//class parser




	public class AddressTxoState
	{	
		public string address { get; set; }  
		public string txo { get; set; } // (unspent) output tx
		public int idx { get; set; } // position in transaction
	}

   	public class BlockState
	{			 
		public uint h { get; set; } //height
		
		public string f { get; set; } //pow, pos
        public uint bt { get; set; } //block unixtime
		public string mr { get; set; } //blocks are clustered by modifier
		public string hash { get; set; } //64 char
		public List<string> tx { get; set; } 
		public uint nTx { get; set; } //length tx
	}

	public class FindstakeStatus
	{
		public decimal difficulty { get; set; }  
        public uint lastupdatedblocktime { get; set; } 
		public uint lastupdatedblock { get; set; } 
		public List<StakeModifier> blockModifiers { get; set; } 
	}
 
	public class OutputState
	{
		public string Id { get; set; } //tohash_position
		
		public string hash { get; set; }  
		public uint idx { get; set; }
		
		public bool spent { get; set; }		
		public UInt64 units { get; set; }				
		public string data { get; set; }//???		
		public bool hasoptreturn { get; set; }
		
		public string ToId()
		{
			return "to" + this.hash + "_" + this.idx;
		}
	}

	public class TxState
	{		 
		public string hash { get; set; }	
		
		public uint height { get; set; }		
		public uint time { get; set; }		//peercoin specific			
		public uint position { get; set; }	//position in block
		public uint size { get; set; }				
		public uint offset { get; set; }
	}


	public class UnspentTransactionData
	{
		public string txid;
		public uint vout;
		public string address;
		public uint blockheight;
		public string blockhash;
	}

	public class UnspentTransactions
	{					
		public string Id { get; set; }	
		public uint BlockFromTime { get; set; }		
		public uint PrevTxOffset { get; set; }	
		public uint PrevTxTime { get; set; }        
		public UInt64 PrevTxOutValue { get; set; }
		public uint PrevTxOutIndex { get; set; }	
	}


	public class BlockRepository
	{			
		private readonly object listlock = new object();
		private List<BlockState> blocks;
			
		public BlockRepository()
		{				
			blocks = new List<BlockState>();
		}
		
							
		public BlockState GetBlockState(uint height)
		{			
			lock(listlock)
			{
				var results = blocks.Where(x => x.h == height).ToList();
			    if (results.Count > 0)
			    {
			    	return results[0];
			    }			
			}			
								
			return null;
		}	
		
				
		public List<BlockState> GetBlockStates(List<uint> heights)
		{						
			lock(listlock)
			{
				var results = blocks.Where(block => heights.Contains(block.h)).ToList();
			    if (results.Count > 0)
			    {
			    	return results;
			    }			
			}			
					
			return new List<BlockState>();
		}	
		
		
		public List<StakeModifier> GetStakeModifiers(uint height)
		{			
			var start = height - 6 * 24 * 31;			
			var end = height;
				
			lock(listlock)
			{
				var results = blocks.Where(x => x.h > start && x.h <= end).ToList();
			    if (results.Count > 0)
			    {
			    	return results
			    		.Select(result => new StakeModifier{
			    	                      	bt = result.bt,
			    	                      	mr = result.mr,
			    	                      	stakeModifier = Convert.ToUInt64(result.mr, 16)			    	                      	
			    	                      })
			    		.OrderBy(r => r.bt)
			    		.ToList();
			    }			
			}
			return new List<StakeModifier>();
		}
		
				
		public void SetBlockState(BlockState state)
		{
			if (state == null) return;
						
			lock(listlock)
			{
				var results = blocks.Where(x => x.h == state.h).ToList();
			    if (results.Count > 0)
			    {			    	
			    	var oldstate = results[0];
			    	oldstate.bt = state.bt;
			    	oldstate.f = state.f;
			    	oldstate.hash = state.hash;
			    	oldstate.mr = state.mr;			    	
			    	oldstate.tx = state.tx;
			    	oldstate.nTx = state.nTx;			    	     
			    }
			    else
			    {
			       blocks.Add(state);				       
			    }			    
			}
		}
		
		public void DeleteAll()
		{	
							
			lock(listlock)
			{
				blocks = new List<BlockState>();
			}
		}

	}





	public class TransactionRepository 
	{				
		private readonly object listAdrlock = new object();
		private List<AddressTxoState> addresses;
				
		private readonly object listTxlock = new object();
		private List<TxState> transactions;
				
		private readonly object listOutputslock = new object();
		private List<OutputState> outputs;
		
		private readonly BlockRepository blockRepository;
				
		public TransactionRepository(BlockRepository blockRepository)
		{		
			this.blockRepository = blockRepository;
			addresses = new List<AddressTxoState>();
			transactions = new List<TxState>();
			outputs = new List<OutputState>();
		}
			
		
		public List<UnspentTransactions> GetUnspents(string address)
		{			 	
			var unspentTransactions = new List<UnspentTransactions>();
			if (string.IsNullOrWhiteSpace(address)) return unspentTransactions;
						

			var addressTxoStates = GetAddresses(address);
			var txStates = new List<TxState>();						
			var txOutStates = new List<OutputState>();						
			var blockStates = new List<BlockState>();
			
			
			if (addressTxoStates.Count > 0)
			{		
				txOutStates = GetOutputs(addressTxoStates);
			}

						
			if (txOutStates.Count > 0)			
			{		
				txStates = GetTransactions(txOutStates);				
			}
			
			
			if (txStates.Count > 0)
			{
				blockStates = blockRepository.GetBlockStates(txStates.Select(txState=>txState.height).ToList());
							
				foreach (var txState in txStates) 
				{
					var blockstate = blockStates.SingleOrDefault(block => block.h == txState.height);
					if (blockstate != null) 
					{
						txOutStates
							.Where(txOutState => txOutState.hash == txState.hash /*&& txOutState.idx == txState.position*/ && !txOutState.spent)
							.ToList()
							.ForEach(txOutState => {							         								
							         	var unspent = new UnspentTransactions {							         		
												Id = txOutState.ToId(),
												BlockFromTime = blockstate.bt,
												PrevTxOffset = txState.offset,
												PrevTxTime = txState.time,
												PrevTxOutValue = txOutState.units,
												PrevTxOutIndex = txOutState.idx												
							         	};
							         	
							         	unspentTransactions.Add(unspent);							         	
							         });						
					}
				}
				
			}
			
			return unspentTransactions;
		}
		
		
		private List<AddressTxoState> GetAddresses(string address)
		{					
			lock(listAdrlock)
			{
			    return addresses.Where(x => x.address == address).ToList();			
			}	
		}
		
		
		private List<OutputState> GetOutputs(List<AddressTxoState> addressTxoStates)
		{
			var txOutStates = new List<OutputState>();	
			
			lock(listOutputslock)
			{
				foreach (var addressState in addressTxoStates) 
				{
					outputs
						.Where(x => x.hash == addressState.txo && x.idx == addressState.idx && x.spent == false && x.units > 0)
						.ToList()
						.ForEach( result => txOutStates.Add(new OutputState(){
						                                    	Id = result.ToId(),
						                                    	hash = result.hash,
																idx = result.idx,		
																spent = result.spent,		
																units = result.units,									
																data = result.data,
																hasoptreturn = result.hasoptreturn
						                                    }));						
				} 		
			}		
			return txOutStates;			
		}
		
		
		private List<TxState> GetTransactions(IEnumerable<OutputState> txOutStates)
		{		
			var hashes = txOutStates.Select(s => s.hash).ToList();
			var txStates = new List<TxState>();	
			
			lock(listTxlock)
			{
				transactions
					.Where(x => hashes.Contains(x.hash))
					.ToList()
					.ForEach(transaction => txStates.Add(new TxState{
					                                     		 hash = transaction.hash,
					                                     		 height = transaction.height,		 
					                                     		 time = transaction.time,
					                                     		 position = transaction.position,
					                                     		 size = transaction.size,					                                     		 
					                                     		 offset = transaction.offset
					                                     }));
									
			}
			return txStates;			
		}
		
		
		
		
		
		public void SetAddressState(AddressTxoState state)
		{			
			if (state == null) return;
			
			lock(listAdrlock)
			{
			    var results = addresses.Where(x => x.address == state.address && x.txo == state.txo && x.idx == state.idx).ToList();
			    
			    if (results.Count == 0)
			    {
			       addresses.Add(state);				       
			    }		
			}				
		}
		
		
		public void SetOutputState(OutputState state)		
		{			
			if (state == null) return;

			lock(listOutputslock)
			{
			    var results = outputs.Where(x => x.hash == state.hash && x.idx == state.idx).ToList();
			    
			    if (results.Count > 0)
			    {
			    	var oldstate = results[0];
			    	oldstate.spent = state.spent;
			    	oldstate.units = state.units; 		    	
			    	oldstate.hasoptreturn = state.hasoptreturn;	    	
			    }
			    else
			    {			    	
			    	state.Id = state.ToId();
			    	outputs.Add(state);
			    }
			}						
		}
				
				
		public void DeleteSpentOutputState(string hash, int idx)
		{			
			if (hash == null) return;
			
			lock(listOutputslock)
			{
				outputs.Where(x => x.hash == hash && x.idx == idx).ToList().ForEach(item => {item.spent = true;});			    
			}
		}
		
		
		
				
		public void SetTxState(TxState state)
		{
			if (state == null) return;
						
			lock(listTxlock)
			{
				var results = transactions.Where(x => x.hash == state.hash).ToList();	
			    if (results.Count > 0)
			    {
			    	var oldstate = results[0];
			    	oldstate.height = state.height;
			    	oldstate.time = state.time;
			    	oldstate.position = state.position;
			    	oldstate.size = state.size;			   				 
			    	oldstate.offset = state.offset;		    	     
			    			    	
			    }
			    else
			    {
			       transactions.Add(state);				       
			    }				
			}			
		}
		
	
		public void DeleteAll()
		{
			lock(listAdrlock)
			{
				addresses = new List<AddressTxoState>();			
			}			
			
			lock(listTxlock)
			{				
				transactions = new List<TxState>();				
			}
			
			lock(listOutputslock)
			{
				outputs = new List<OutputState>();			
			}		
			
		}
			
				
	}




}
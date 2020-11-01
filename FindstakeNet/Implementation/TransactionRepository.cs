using System;
using System.Collections.Generic;
using System.Linq;
using FindstakeNet.Interface;
using FindstakeNet.Model;

namespace FindstakeNet.Implementation
{
	public class TransactionRepository : ITransactionRepository
	{				
		private readonly object listAdrlock = new object();
		private List<AddressTxoState> addresses;
				
		private readonly object listTxlock = new object();
		private List<TxState> transactions;
				
		private readonly object listOutputslock = new object();
		private List<OutputState> outputs;
		
		private readonly IBlockRepository blockRepository;
				
		public TransactionRepository(IBlockRepository blockRepository)
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

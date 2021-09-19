using System;
using System.Collections.Generic;
using System.Linq;
using FindstakeNet.Interface;
using FindstakeNet.Model;
using PeercoinUtils;
using Newtonsoft.Json;
using PeercoinUtils.Crypto;

namespace FindstakeNet.Implementation.RPC
{
	public class TransactionParser : ITransactionParser
	{				
		private readonly IRPCClient client;
		private readonly ITransactionRepository transactionRepository;
	
		
		public TransactionParser(IRPCClient client, ITransactionRepository transactionRepository)
		{				
			this.client = client;
			this.transactionRepository = transactionRepository;
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
				time = time
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

	}
}

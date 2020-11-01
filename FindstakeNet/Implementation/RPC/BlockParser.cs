using System;
using System.Collections.Generic; 
using System.Linq;
using FindstakeNet.Interface;
using FindstakeNet.Model;

namespace FindstakeNet.Implementation.RPC
{
	public class BlockParser : IBlockParser
	{
		private readonly IRPCClient client;
		private readonly IBlockRepository blockRepository;				
		private readonly ITransactionParser transactionParser;
		private readonly Dictionary<string, uint> parsedHash;
		private readonly Dictionary<uint, bool> parsedHeight;
		
		public BlockParser(IRPCClient client, IBlockRepository blockRepository, ITransactionParser transactionParser)
		{
			this.client = client;
			this.blockRepository = blockRepository;
			this.transactionParser = transactionParser;
			this.parsedHash = new Dictionary<string, uint>();
			this.parsedHeight = new Dictionary<uint, bool>();
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
				transactionParser.Parse((uint)block.height, block.tx.ToList());
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
		
		
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using FindstakeNet.Implementation.RPC;

namespace FindstakeNet.Interface
{
	public interface IRPCClient
	{
		bool CheckConnection();
		//TransactionResponse GetTransaction(string txId);
		RawTransactionResponse GetRawTransaction(string txId, int verbose = 0);
		DecodeRawTransactionResponse DecodeRawTransaction(string transaction);
		BlockResponse GetBlock(string hash);		
		uint GetBlockCount();
		DifficultyResponse GetDifficulty();
		string GetBlockHash(long index);		
		List<Unspent> GetUnspents();
	}
}

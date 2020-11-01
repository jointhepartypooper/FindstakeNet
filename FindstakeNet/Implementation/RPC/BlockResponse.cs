using System;
using System.Collections.Generic;

namespace FindstakeNet.Implementation.RPC
{
	public class BlockResponse
	{
		public string hash;
		public long confirmations;
		public int size;
		public long height;
		public int version;
		public string merkleroot;
		public IEnumerable<string> tx;
		public long time;
		public long nonce;
		public string bits;
		public decimal difficulty;
		public string previousblockhash;
		public string nextblockhash;
		public string flags;
		public string modifier;
		public int nTx;
	}
}

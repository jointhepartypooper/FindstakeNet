using System;

namespace FindstakeNet.Model
{
	public class UnspentTransactionData
	{
		public string txid;
		public uint vout;
		public string address;
		public uint blockheight;
		public string blockhash;
	}
}

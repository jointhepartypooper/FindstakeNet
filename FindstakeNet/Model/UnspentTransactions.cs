using System;

namespace FindstakeNet.Model
{
	public class UnspentTransactions
	{					
		public string Id { get; set; }	
		public uint BlockFromTime { get; set; }		
		public uint PrevTxOffset { get; set; }	
		public uint PrevTxTime { get; set; }        
		public UInt64 PrevTxOutValue { get; set; }
		public uint PrevTxOutIndex { get; set; }	
	}
}

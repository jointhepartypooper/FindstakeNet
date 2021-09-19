using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindstakeNet.Implementation.RPC
{			
	public class ScriptSig
	{
		public string asm;
		public string hex;
	}		
			
	public class ScriptPubKey
	{
		public string asm;
		public string hex;
		public int reqSigs;
		public string type;
		public string[] addresses;
	}
					
	public class Input
	{
		public string txid;
		public int vout;
		public ScriptSig scriptSig;
		public long sequence;
	}

	public class Output
	{
		public decimal value;
		public int n;
		public ScriptPubKey scriptPubKey;

	}
		
	public class DecodeRawTransactionResponse
	{
		public string txid;
		public int version;
		public int? time;			
		public int locktime;
		public int size;
		public int vsize;
		
		public Input[] vin;
		public Output[] vout;
	}
}

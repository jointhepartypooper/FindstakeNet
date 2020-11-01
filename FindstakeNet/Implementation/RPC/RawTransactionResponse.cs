using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindstakeNet.Implementation.RPC
{
	public class RawTransactionResponse
	{	
		public string hex;
		public string blockhash;
		public long blocktime;		
		
		public static implicit operator RawTransactionResponse(String s)
		{
			return new RawTransactionResponse() { hex = s };
		}
	}
}

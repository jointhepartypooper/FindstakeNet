using System;

namespace FindstakeNet.Model
{
	public class AddressTxoState
	{	
		public string address { get; set; }  
		public string txo { get; set; } // (unspent) output tx
		public int idx { get; set; } // position in transaction
	}
}

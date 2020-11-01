using System;
 

namespace FindstakeNet.Model
{
	public class TxState
	{		 
		public string hash { get; set; }	
		
		public uint height { get; set; }		
		public uint time { get; set; }		//peercoin specific			
		public uint position { get; set; }	//position in block
		public uint size { get; set; }				
		public uint offset { get; set; }
	}
}

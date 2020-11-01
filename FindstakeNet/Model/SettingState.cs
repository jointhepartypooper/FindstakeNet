using System;
 

namespace FindstakeNet.Model
{
	public class SettingState
	{	
		public string Id { get; set; }
		
		public string Value { get; set; }
		
									//public int MaxBH { get; set; } //last synced block
							        //public int MaxTx { get; set; } //last synced tx in block
		public decimal Diff { get; set; } //last diff from rpc
        public uint CurBH { get; set; } //last blockcount from rpc   
	}
}

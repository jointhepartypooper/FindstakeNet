using System;

namespace FindstakeNet.Model
{
	public class OutputState
	{
		public string Id { get; set; } //tohash_position
		
		public string hash { get; set; }  
		public uint idx { get; set; }
		
		public bool spent { get; set; }		
		public UInt64 units { get; set; }				
		public string data { get; set; }//???		
		public bool hasoptreturn { get; set; }
		
		public string ToId()
		{
			return "to" + this.hash + "_" + this.idx;
		}
	}
}

using System;
using System.Collections.Generic;

namespace FindstakeNet.Model
{
	public class BlockState
	{			 
		public uint h { get; set; } //height
		
		public string f { get; set; } //pow, pos
        public uint bt { get; set; } //block unixtime
		public string mr { get; set; } //blocks are clustered by modifier
		public string hash { get; set; } //64 char
		public List<string> tx { get; set; } 
		public uint nTx { get; set; } //length tx
	}
}

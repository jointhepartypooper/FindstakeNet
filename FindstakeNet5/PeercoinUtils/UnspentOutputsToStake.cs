using System;
using System.Linq;
using System.Collections.Generic;

namespace PeercoinUtils
{
	public class UnspentOutputsToStake
	{		
//		public MintTemplate template
//		public int Bits;
//		public long TxTime;
//		public long StartTime;
//		public long MaxTime;
//		public bool Stop;
		//  Results: any[];
  		//private orgtpl: any[];

  		private readonly List<MintTemplate> arrStakeKernelTemplates;

  		public UnspentOutputsToStake()
		{
  	 		arrStakeKernelTemplates = new List<MintTemplate>();
		}
  		
  		public void Add(MintTemplate tpldata)
  		{
  			var listed = arrStakeKernelTemplates
  				.Any(el =>  				     
  				     el.PrevTxOffset == tpldata.PrevTxOffset &&
				        el.PrevTxOutIndex == tpldata.PrevTxOutIndex &&
				        el.PrevTxOutValue == tpldata.PrevTxOutValue			      
  				      );
  			
  			if (!listed)
  				this.arrStakeKernelTemplates.Add(tpldata);
  		}
	}
}

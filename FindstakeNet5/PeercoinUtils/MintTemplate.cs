using System;
using System.Linq;
using PeercoinUtils.Crypto;

namespace PeercoinUtils
{
	public class MintTemplate
	{		
		public string Id { get; private set; }
		public string OfAddress { get; private set; }
				
		public UInt32 BlockFromTime{ get; private set; }
  		public UInt32 PrevTxOffset{ get; private set; }
  		public UInt32 PrevTxTime{ get; private set; }
  		public UInt32 PrevTxOutIndex{ get; private set; }
  		public UInt32 PrevTxOutValue{ get; private set; } 		

  		public UInt32? Bits { get; private set; }
		
  		public MintTemplate(string id, 
  		                    string address,
  		                   UInt32 blockFromTime,
  		                   UInt32 prevTxOffset,
  		                   UInt32 prevTxTime,
  		                   UInt32 prevTxOutIndex,
  		                   UInt32 prevTxOutValue)
  		{
  			this.Id = id;
  			this.OfAddress = address;
  			this.BlockFromTime = blockFromTime;
  			this.PrevTxOffset = prevTxOffset;
			this.PrevTxTime = prevTxTime;
			this.PrevTxOutIndex = prevTxOutIndex;
			this.PrevTxOutValue = prevTxOutValue;
  		}
  		
  		  		
  		public MintTemplate(string id,
  		                    string address,
  		                   UInt32 blockFromTime,
  		                   UInt32 prevTxOffset,
  		                   UInt32 prevTxTime,
  		                   UInt32 prevTxOutIndex,
  		                   UInt32 prevTxOutValue,
  		                   UInt32 bits)
  		{
  			this.Id = id;
  			this.OfAddress = address;
  			this.BlockFromTime = blockFromTime;
  			this.PrevTxOffset = prevTxOffset;
			this.PrevTxTime = prevTxTime;
			this.PrevTxOutIndex = prevTxOutIndex;
			this.PrevTxOutValue = prevTxOutValue;
			this.Bits = bits;
  		}
				
		public uint SetBitsWithDifficulty(float diff)
		{
		    this.Bits = Mint.BigToCompact(Mint.DiffToTarget(diff));
		    return this.Bits.Value;
		}
	}
}

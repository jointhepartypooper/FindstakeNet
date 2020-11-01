using System;
using System.Numerics;
using System.Linq;

namespace PeercoinUtils
{		
	public class CheckStakeResult
	{		
		public string Id;		//toHash_Index		
		public string OfAddress;
		public bool success;
		public BigInteger minTarget;
		public Byte[] hash;
		public float minimumDifficulty;
		public UInt32 txTime;
	}
	 
}

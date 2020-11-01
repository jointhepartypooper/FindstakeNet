using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;

namespace PeercoinUtils.Crypto
{
		
	public class StakeModifier
	{		        
		public uint bt { get; set; } //block unixtime
		public string mr { get; set; } //Modifier
		public UInt64 stakeModifier { get; set; }
	}
	
	public static class Mint
	{			
		public static UInt64? GetModifier(List<StakeModifier> blockModifiers, UInt32 curtime)
		{			      
			var tt = curtime - PeercoinConstants.Findstakelimit;
			StakeModifier match = null;
			for (int i = 0; i < blockModifiers.Count; i++) 
			{
				if (blockModifiers[i].bt <= tt)
				{
					match = blockModifiers[i];
				}
				else
				{
					break;
				}
			}
			
			return match == null ? (UInt64?)null : match.stakeModifier;
		}
		
		
		public static long GetSizeVarInt(long n)
		{				  
			if (n < 253) 
				return 1;
			else if (n <= 65535) 
				return 3;
			else if (n <= 4294967295) 
				return 5;			
			else return 9;
		}	
		
		public static CheckStakeResult CheckStakeKernelHash(MintTemplate template, UInt32 txTime, UInt64 stakeModifier)
		{
			var retobj = new CheckStakeResult
			{
				Id = template.Id,
				OfAddress = template.OfAddress,
				txTime = txTime
			};
			    
			if (txTime < template.PrevTxTime) 
			{				
				// Transaction timestamp violation					  
				return retobj;
		    }
			
			    
			if (template.BlockFromTime + PeercoinConstants.StakeMinAge > txTime)
			{				
				// Min age requirement			
				return retobj;
		    }
						  
			var bnTargetPerCoinDay = Mint.CompactToBig(template.Bits.Value);
    			  
			long nTimeWeight = txTime - template.PrevTxTime; 
		    if (nTimeWeight > PeercoinConstants.StakeMaxAge) 
		    {		    	
		    	nTimeWeight = PeercoinConstants.StakeMaxAge;
		    }
								    
		    long timeReduction = PeercoinConstants.StakeMinAge;
		    nTimeWeight -= timeReduction;
			      
		    var t1 = new BigInteger(24 * 60 * 60);
			var t2 = new BigInteger(PeercoinConstants.Coin);
			var t3 = new BigInteger(template.PrevTxOutValue);
			var t4 = new BigInteger(nTimeWeight);
 	    
		    BigInteger bnCoinDayWeight = (((t3 * t4) / t2))/t1;
 
		        
		    BigInteger targetInt  = bnCoinDayWeight * (bnTargetPerCoinDay);
		                
            //byte[] array = new byte[28];
            var buffer = new byte[28]; 
		    		        
		    var bufferindex = 0;

 
			byte[] arrStakemodifier = BitConverter.GetBytes(stakeModifier);

			//put stakemodifier in buffer
			for (var i = 0; i < 8; i++) 
			{
				buffer[bufferindex] = arrStakemodifier[i];
				bufferindex++;
			}
	 
			//put other data in buffer
			new List<UInt32>{(UInt32)template.BlockFromTime, (UInt32)template.PrevTxOffset, (UInt32)template.PrevTxTime, (UInt32)template.PrevTxOutIndex, (UInt32)txTime}
			.ForEach(num => {			             	
			             	  var dn = num;
		             		  for (var i = 0; i < 4; i++) 
						      {		             		  	
		             		  	buffer[bufferindex] = (byte)(dn & 0xff);
						        dn >>= 8;
						        bufferindex++;
						      }
			             });
			
		 
			//no reverse so keep it in little endian
			var hashProofOfStake = Mint.DoubleSha256(buffer.ToList()).ToArray();   //keep it in little-endian .Reverse().ToArray();
			
			//add zero to last in array to make it unsigned:
			// https://docs.microsoft.com/en-us/dotnet/api/system.numerics.biginteger.-ctor?view=netframework-4.5.2#System_Numerics_BigInteger__ctor_System_Byte___
			if ((hashProofOfStake[hashProofOfStake.Length-1] & 0x80) >0)
			{
				byte[] temp = new byte[hashProofOfStake.Length];
				Array.Copy(hashProofOfStake, temp, hashProofOfStake.Length);
				hashProofOfStake = new byte[temp.Length+1];
				Array.Copy(temp, hashProofOfStake, temp.Length);
			}
			
			var hashProofOfStakeInt = new BigInteger(hashProofOfStake);     
						    
			if (hashProofOfStakeInt > targetInt) 
			{
		      return retobj;
		    }
			
			//yeah, below target!			
			    
			retobj.minTarget = (hashProofOfStakeInt / bnCoinDayWeight) - new BigInteger(1);
		    retobj.success = true;
		    retobj.hash = hashProofOfStake;		    
		    retobj.Id = template.Id;
		    
		    
		    var comp = Mint.IncCompact(
            	Mint.BigToCompact(retobj.minTarget)
          	);
		    retobj.minimumDifficulty = Mint.CompactToDiff(comp);
		    
			return retobj;
		}		
				
		
		public static BigInteger DiffToTarget(float difficulty)
		{
			var mantissa = 0x0000ffff / difficulty;
			    
			var exp = 1;
			var tmp = mantissa;
		    while (tmp >= 256.0F) {
		      tmp /= 256.0F;
		      exp++;
		    }
				
			for (var i = 0; i < exp; i++) {
				mantissa *= 256.0F;
			}
 
			var number = new BigInteger(Math.Floor(mantissa));
			var shiftleftby = (26 - exp) * 8;
			return number << shiftleftby;
		}
		
		 
		public static UInt32 IncCompact(UInt32 compact)
		{					    
			var mantissa = compact & 0x007fffff;
			var neg = compact & 0x00800000;
			var exponent = compact >> 24;
		
		    if (exponent <= 3) 
		    {		
		    	var delta = 3 - Convert.ToInt32(exponent);
		    	var leftshift = 8 * delta;

		    	var toadd = 1 << leftshift;
		    	 		    	
		    	mantissa += Convert.ToUInt32(toadd);
		    } 
		    else 
		    {
		      	mantissa++;
		    }
		
		    if (mantissa >= 0x00800000) {
		      mantissa >>= 8;
		      exponent++;
		    }
		   
		    return (exponent << 24) | mantissa | neg;
		}
		
		
		public static float CompactToDiff(UInt32 bits)
		{			
			var nShift = (bits >> 24) & 0xff;			
			
			var diff = ((float)(1.0 * 0x0000ffff)) / ((float)((bits & 0x00ffffff)));
			
			while (nShift < 29)
			{
				diff *= 256.0F;
				nShift++;
			}
						
			while (nShift > 29)
			{
				diff /= 256.0F;
				nShift--;
			}
			
		    return diff;
		}
		
		
		public static List<byte> DoubleSha256(List<byte> bytes)
		{		
            using (SHA256 sha256Hash = SHA256.Create())  
            {  
            	byte[] bytes1 = sha256Hash.ComputeHash(bytes.ToArray());
            	return sha256Hash.ComputeHash(bytes1).ToList();
            } 
		}


	  // BigToCompact converts a whole number N to a compact representation using
	  // an unsigned 32-bit number.  The compact representation only provides 23 bits
	  // of precision, so values larger than (2^23 - 1) only encode the most
	  // significant digits of the number.  See CompactToBig for details.
	  public static UInt32 BigToCompact(BigInteger n)
	  {	 	
	  	// No need to do any work if it's zero.
	  	if (n.IsZero)
	    {  		
	  		return 0;
	    }
	
	    // Since the base for the exponent is 256, the exponent can be treated
	    // as the number of bytes.  So, shift the number right or left
	    // accordingly.  This is equivalent to:
	    // mantissa = mantissa / 256^(exponent-3)
	    UInt32 mantissa;  
	 
	    UInt32 exponent = Convert.ToUInt32((n.ToByteArray()).Length);
	
	    if (exponent <= 3) 
	    {
			var delta = 3 - Convert.ToInt32(exponent);    		
			var leftshift = 8 * delta;       	
	    	
	    	if (n.Sign > 0)
	    	{
	    		//positive
	    		mantissa = UInt32.Parse(n.ToString());
			    	
	    		var temp = mantissa << leftshift;
	    		mantissa = temp;
	    		
	    	}
	    	else
	    	{
	    		//negative
	    		mantissa = Convert.ToUInt32(-1 * Int32.Parse(n.ToString()));
	    		    		    		
	    		var temp = mantissa << leftshift;
	    		mantissa = temp;
	    	}    	
 
	    } 
	    else 
	    {    	
	    	// Use a copy to avoid modifying the caller's original number.
	      
	    	var tn = new BigInteger(n.ToByteArray());
	    	int rightshiftby = (int)(8 * (exponent - 3));
	    	var newmantissa = tn >> rightshiftby;
	    	    	    	
	    	if (newmantissa.Sign > 0)
	    	{
	    		//positive
	    	    mantissa = UInt32.Parse(newmantissa.ToString());
	    	}
	    	else
	    	{    		   		
	    		//negative
	    		mantissa = Convert.ToUInt32(-1 * Int32.Parse(newmantissa.ToString()));
	    	}	    	
	    
	    }
	
	    // When the mantissa already has the sign bit set, the number is too
	    // large to fit into the available 23-bits, so divide the number by 256
	    // and increment the exponent accordingly.
	    if ((mantissa & 0x00800000) != 0) 
	    {	    	
	    	mantissa >>= 8;	    	
	    	exponent++;
	    }
	
	    // Pack the exponent, sign bit, and mantissa into an unsigned 32-bit
	    // int and return it.
	    var compact = (exponent << 24) | mantissa;
	
	    if (n.Sign < 0) 
	    {	    	
	    	compact |= 0x00800000;
	    }
	    
	    return compact;
	  }
	
			
			
	  
	  // CompactToBig converts a compact representation of a whole number N to an
		// unsigned 32-bit number.  The representation is similar to IEEE754 floating
		// point numbers.
		//
		// Like IEEE754 floating point, there are three basic components: the sign,
		// the exponent, and the mantissa.  They are broken out as follows:
		//
		//	* the most significant 8 bits represent the unsigned base 256 exponent
		// 	* bit 23 (the 24th bit) represents the sign bit
		//	* the least significant 23 bits represent the mantissa
		//
		//	-------------------------------------------------
		//	|   Exponent     |    Sign    |    Mantissa     |
		//	-------------------------------------------------
		//	| 8 bits [31-24] | 1 bit [23] | 23 bits [22-00] |
		//	-------------------------------------------------
		//
		// The formula to calculate N is:
		// 	N = (-1^sign) * mantissa * 256^(exponent-3)
		//
		// This compact form is only used in bitcoin to encode unsigned 256-bit numbers
		// which represent difficulty targets, thus there really is not a need for a
		// sign bit, but it is implemented here to stay consistent with bitcoind.		
		public static BigInteger CompactToBig(UInt32 compact)   
		{
			var mantissa = compact & 0x007fffff;
			var isNegative = (compact & 0x00800000) != 0;
			var	exponent = (compact >> 24);
			// Since the base for the exponent is 256, the exponent can be treated
			// as the number of bytes to represent the full 256-bit number.  So,
			// treat the exponent as the number of bytes and shift the mantissa
			// right or left accordingly.  This is equivalent to:
			// N = mantissa * 256^(exponent-3)			
			
			var bn = new BigInteger(0);
				
			if (exponent <= 3)
			{						    	
				var delta = 3 - Convert.ToInt32(exponent);
		    	var rightshift = 8 * delta;
		    			    			    	
		    	mantissa = mantissa >> rightshift;  //C >>= 2 is same as C = C >> 2
				bn = new BigInteger(mantissa);
			}
			else 
			{
				bn = new BigInteger(mantissa);
				var delta = Convert.ToInt32(exponent)-3;
				var leftshift = 8 * delta;
				bn = bn << leftshift;
			}
			
				
			// Make it negative if the sign bit is set.
			if (isNegative) {
				bn = bn * (new BigInteger(-1));
			}

			return bn;
			 
		}
		
	}
}

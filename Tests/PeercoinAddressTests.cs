using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PeercoinUtils.Crypto;

namespace Tests
{
	[TestFixture]
	public class PeercoinAddressTests
	{
				
		public static IEnumerable<string> DataSet
		{
			get
			{
				return new[] {
					"PPiqYtt64yWQJnYSRSDCpGYbdAiUKnXQCu",
					"PVQ3TLyteGLiU2r6rHkUKKM3ZM66Yn8cgY",
					"PVDn8RLPtiTUzbwopNPxpvCJcN5ewCQzU7",
					"PWG4XK4ixFcTPbUPhHeEfkcpx92LQBP3xB",
					"PFqvKd83k4mpuX5t4XVTV92E3dg2cFc5Uw",
					"PR8J1pbRfhV2bbP43sFNtxSQEm8JNwvenj",
				};
			}
		}
		
		
        [Test]		
		public void ShouldConstructProperly()
		{
			foreach (var address in DataSet)
			{
				var p = new PeercoinAddress(address);	 
				            
				bool isValid = p.ToString() == address;
            	Assert.IsTrue(isValid);				
			}
		}
		
		
        [Test]
		public void ShouldNotConstructProperly()
		{
			var bitcoins = new[] {"1sTybTznstbwufxRe4iALvgHV1ZZYq5uT","1GEYVPaaxyzarNbZTB5WFPVoxnx8ekyrgD","1t7BwrqQyTN1NBTYdDTGq3jZwDLL6g8dg"};
					
			var random = new[] {"1hgfZYq5uT","1G NbZTB5WFYTRgD","p1g", ""};
			
			foreach (var address in bitcoins.Concat(random))
			{		
				var valid = PeercoinAddress.IsAddress(address);
				bool isnotValid = !valid;
				Assert.IsTrue(isnotValid);		 
			}
		}
	}
}

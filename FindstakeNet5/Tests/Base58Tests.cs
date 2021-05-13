using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PeercoinUtils.DataEncoders;

namespace Tests
{
	[TestFixture]
	public class Base58Tests
	{	
		public static IEnumerable<object[]> DataSet
		{
			get
			{
				return new[] {
					new object[]{string.Empty, ""},
					new object[]{"61", "2g"},
					new object[]{"626262", "a3gV"},
					new object[]{"636363", "aPEr"},
					new object[]{"73696d706c792061206c6f6e6720737472696e67", "2cFupjhnEsSn59qHXstmK2ffpLv2"},
					new object[]{"00eb15231dfceb60925886b67d065299925915aeb172c06647", "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L"},
					new object[]{"516b6fcd0f", "ABnLTmg"},
					new object[]{"bf4f89001e670274dd", "3SEo3LWLoPntC"},
					new object[]{"572e4794", "3EFU7m"},
					new object[]{"ecac89cad93923c02321", "EJDM8drfXA6uyA"},
					new object[]{"10c8511e", "Rt5zm"},
					new object[]{"00000000000000000000", "1111111111"}
				};
			}
		}
		
		
 
		[Test]
		public void ShouldEncodeProperly()
		{
			foreach (var i in DataSet)
			{
				string data = (string)i[0];
				string encoded = (string)i[1];
				var testBytes = Encoders.Hex.DecodeData(data);
				
				bool isValid = encoded == Encoders.Base58.EncodeData(testBytes);
            	Assert.IsTrue(isValid);
 
			}
		}
		
		[Test]				
		public void ShouldDecodeProperly()
		{
			foreach (var i in DataSet)
			{
				string data = (string)i[0];
				string encoded = (string)i[1];
				var testBytes = Encoders.Base58.DecodeData(encoded);
				var isValid = testBytes.SequenceEqual(Encoders.Hex.DecodeData(data)); // true
	 
            	Assert.IsTrue(isValid);				
			}
		}
		
	}
}

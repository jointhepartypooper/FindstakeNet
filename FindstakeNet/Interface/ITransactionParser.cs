using System;
using System.Collections.Generic;

namespace FindstakeNet.Interface
{
	public interface ITransactionParser
	{
		string GetBlockHash(string txId);
		void Parse(uint height, List<string> txIds);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using FindstakeNet.Model;

namespace FindstakeNet.Interface
{
	public interface ITransactionRepository
	{
		void SetTxState(TxState state);
		void SetOutputState(OutputState state);
		void SetAddressState(AddressTxoState state);
		void DeleteSpentOutputState(string hash, int idx);
		List<UnspentTransactions> GetUnspents(string address);
	}
}

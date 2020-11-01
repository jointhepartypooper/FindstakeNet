using System;

namespace FindstakeNet.Implementation.RPC
{
	public class RPCException: Exception
	{
		public RPCError Error
		{
			get;
			private set;
		}
		
		public RPCException(RPCError rpcError)
			: base (rpcError.message)
		{
			Error = rpcError;
		}

		public RPCException(RPCError rpcError, Exception innerException)
			: base(rpcError.message, innerException)
		{
			Error = rpcError;
		}
	}
}

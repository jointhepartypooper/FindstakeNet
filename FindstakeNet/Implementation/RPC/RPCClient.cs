using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using FindstakeNet.Interface;

namespace FindstakeNet.Implementation.RPC
{
	public class RPCClient : IRPCClient
	{
		protected Uri uri;

		protected string user;
		protected string password;
		
		private readonly ISettingsRepository settings;
		
		public RPCClient(ISettingsRepository settings)
		{
			this.settings = settings;
//			this.uri = new Uri(settings.GetRpcUri());
//			this.user = settings.GetRpcUser();
//			this.password = settings.GetRpcPassword();
		}
		
		
		public bool CheckConnection()
		{
			try
			{
				var height = RpcCall<int>(new RPCRequest("getblockcount"));	
				return height > 0;
			}
			catch(Exception)
			{
				return false;
			}		
		}
		
		public RawTransactionResponse GetRawTransaction(string txId, int verbose = 0)
		{
			return RpcCall<RawTransactionResponse>(new RPCRequest("getrawtransaction", new Object[] { txId, verbose }));
		}
		
		public TransactionResponse GetTransaction(string txId)
		{
			return RpcCall<TransactionResponse>(new RPCRequest("gettransaction", new Object[] { txId }));
		}
		
		public DecodeRawTransactionResponse DecodeRawTransaction(string transaction)
		{
			return RpcCall<DecodeRawTransactionResponse>(new RPCRequest("decoderawtransaction", new Object[] { transaction }));
		}
		
		
		public BlockResponse GetBlock(string hash)
		{
			return RpcCall<BlockResponse>(new RPCRequest("getblock", new Object[] { hash }));
		}

		public uint GetBlockCount()
		{
			return RpcCall<uint>(new RPCRequest("getblockcount"));
		}
		
		public List<Unspent> GetUnspents()
		{
			return RpcCall<List<Unspent>>(new RPCRequest("listunspent"));
		}		
		
		public DifficultyResponse GetDifficulty()
		{
			return RpcCall<DifficultyResponse>(new RPCRequest("getdifficulty"));			
		}

		
		public string GetBlockHash(long index)
		{
			return RpcCall<string>(new RPCRequest("getblockhash", new Object[] { index }));
		}
		
		
		protected string HttpCall(string jsonRequest)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

			request.Method = "POST";
			request.ContentType = "application/json-rpc";

			// always send auth to avoid 401 response
			string auth = this.user + ":" + this.password;
			auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth), Base64FormattingOptions.None);
			request.Headers.Add("Authorization", "Basic " + auth);

			request.ContentLength = jsonRequest.Length;

			using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
			{
				sw.Write(jsonRequest);
			}

			try
			{
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				using (StreamReader sr = new StreamReader(response.GetResponseStream()))
				{
					return sr.ReadToEnd();
				}
			}
			catch (WebException wex)
			{
				using (HttpWebResponse response = (HttpWebResponse)wex.Response)
				using (StreamReader sr = new StreamReader(response.GetResponseStream()))
				{
					if (response.StatusCode != HttpStatusCode.InternalServerError)
					{
						throw;
					}
					return sr.ReadToEnd();
				}
			}
		}
		
				
		private T RpcCall<T>(RPCRequest rpcRequest)
		{			
			this.uri = new Uri(settings.GetRpcUri());
			this.user = settings.GetRpcUser();
			this.password = settings.GetRpcPassword();
			
			string jsonRequest = JsonConvert.SerializeObject(rpcRequest);

			string result = HttpCall(jsonRequest);

			RPCResponse<T> rpcResponse = JsonConvert.DeserializeObject<RPCResponse<T>>(result);

			if (rpcResponse.error != null)
			{
				throw new RPCException(rpcResponse.error);
			}
			return rpcResponse.result;
		}
	}
}

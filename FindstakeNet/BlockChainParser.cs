using Newtonsoft.Json;

namespace FindstakeNet
{
	public class BlockChainParser
	{
		private readonly RPCClient client;
		private readonly TransactionRepository transactionRepository;
		private readonly BlockRepository blockRepository;
		private readonly Dictionary<string, uint> parsedHash;
		private readonly Dictionary<uint, bool> parsedHeight;
        private readonly long protocolV10SwitchTime;

		public BlockChainParser(RPCClient client, BlockRepository blockRepository, TransactionRepository transactionRepository,
            long protocolV10SwitchTime)
		{
			this.protocolV10SwitchTime=protocolV10SwitchTime;
			this.client = client;
			this.transactionRepository = transactionRepository;
			this.blockRepository = blockRepository;

			this.parsedHash = new Dictionary<string, uint>();
			this.parsedHeight = new Dictionary<uint, bool>();
		}


		public async Task<string> GetBlockHash(string txId)
		{
			var tx = await client.GetRawTransaction(txId, 1);
			return tx.blockhash;
		}


		public async Task Parse(uint height, List<string> txIds, uint blocktime)
		{
            // ReSharper disable once RedundantCast
            var sizeVarintTx = Mint.GetSizeVarInt((long)(txIds.Count));
			var offset = PeercoinConstants.BlockHeaderSize + sizeVarintTx;

			for (int index = 0; index < txIds.Count; index++)
			{
				var tx = await client.GetRawTransaction(txIds[index]);
				var txraw = await client.DecodeRawTransaction(tx.hex);
				var rawsize = tx.hex.Length / 2; // 2 char is 1 byte

                StoreTxState(blocktime, height, txraw, (uint) index, offset, (uint) rawsize);

				DeleteSpentFromStore(txraw);

				ParseVouts(txraw);

				offset += rawsize;
			}

		}


		private void ParseVouts(DecodeRawTransactionResponse txraw)
		{
			if (txraw is {vout: {Length: > 0}})
			{
				foreach (var txout in txraw.vout)
				{
					StoreOutput(txraw.txid, txout);
					StoreAddresses(txraw.txid, txout);
				}
			}
		}


		private void StoreOutput(string transactionid, Output txout)
		{
			if ((txout.value * PeercoinConstants.Coin >= 0))
			{
				var hasoptreturn = txout.scriptPubKey != null && !string.IsNullOrWhiteSpace(txout.scriptPubKey.asm) && txout.scriptPubKey.asm.StartsWith("OP_RETURN");

				var json = hasoptreturn ? JsonConvert.SerializeObject(txout, Formatting.None) : null;

                var outputState = new OutputState
                {
                    Id = "to" + transactionid + "_" + txout.n,
                    hash = transactionid,
                    idx = (uint) txout.n,
                    spent = false,
                    units = Convert.ToUInt64(Math.Floor(txout.value * PeercoinConstants.Coin)),
                    hasoptreturn = hasoptreturn,
                    data = json
                };
				transactionRepository.SetOutputState(outputState);
			}
		}


		private void StoreAddresses(string transactionid, Output txout)
		{
			if (txout.scriptPubKey is {addresses: {Length: > 0}})
			{
				var addresses = txout.scriptPubKey.addresses.ToList();
				foreach (var address in addresses)
				{
					var addressState = new AddressTxoState
					{
						address = address,
						txo = transactionid,
						idx = txout.n,
					};
					transactionRepository.SetAddressState(addressState);
				}
			}
		}


		private void StoreTxState(uint blocktime, uint height, DecodeRawTransactionResponse txraw,
			uint index, long offset, uint rawsize)
        {
            var time = blocktime < protocolV10SwitchTime && txraw.time.HasValue
                ? (uint) txraw.time.Value
                : blocktime;

			var state = new TxState
			{
				hash = txraw.txid,
				height = height,
				offset = (uint)offset,
				position = index,
				size = rawsize,
				time = time
			};

			transactionRepository.SetTxState(state);
		}


		/// <summary>
		/// to keep storage small, only unspents are of interest
		/// </summary>
		/// <param name="txraw"></param>
		private void DeleteSpentFromStore(DecodeRawTransactionResponse txraw)
		{
			if (txraw is {vin: {Length: > 0}})
			{
				var inputs = txraw.vin.ToList().Where(tin => !string.IsNullOrWhiteSpace(tin.txid)).ToList();
				foreach (var txin in inputs)
				{
					transactionRepository.DeleteSpentOutputState(txin.txid, txin.vout);
				}
			}
		}


		/// <summary>
		/// parse entire block including its tx
		/// </summary>
		/// <param name="hash"></param>
		/// <returns></returns>
		public async Task<uint> Parse(string hash)
		{
			if (parsedHash.ContainsKey(hash))
			{
				return parsedHash[hash];
			}

			var block = await GetBlock(hash);

			Store(block);

			if (block.nTx > 0)
			{
                await Parse((uint)block.height, block.tx.ToList(), (uint)block.time);
			}

			if (!parsedHash.ContainsKey(hash))
				parsedHash.Add(hash, (uint)block.height);

			return (uint)block.height;
		}


		/// <summary>
		/// just to get modifiers
		/// </summary>
		/// <param name="height"></param>
		public async Task Parse(uint height)
		{
			if (parsedHeight.ContainsKey(height))
			{
				return;
			}

			var hash = await GetHash(height);
			var block = await GetBlock(hash);

			Store(block);

			if (!parsedHeight.ContainsKey(height))
				parsedHeight.Add(height, true);
		}


		private void Store(BlockResponse block)
        {
            var newSate = new BlockState
            {
                h = (uint) block.height,
                hash = block.hash,
                f = block.flags == "proof-of-stake" ? "pos" : "pow",
                bt = (uint) block.time,
                mr = block.modifier,
                tx = block.tx.ToList(),
                nTx = (uint) block.nTx
            };

			blockRepository.SetBlockState(newSate);
		}

		private async Task<string> GetHash(uint index)
		{
			return await client.GetBlockHash(index);
		}

		private async Task<BlockResponse> GetBlock(string hash)
		{
			return await client.GetBlock(hash);
		}
    }
}

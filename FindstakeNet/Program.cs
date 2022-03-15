using CommandLine;
using Newtonsoft.Json;

namespace FindstakeNet
{
    public class ConsoleOutput
    {
        public uint BlockHeight { get; set; }
        public decimal PosDifficulty { get; set; }
        public string? Error { get; set; }
        public IReadOnlyList<PossibleStake>? FutureStakes { get; set; }
    }

    public class Program
    {
        static RPCClient? _rpcclient;
        static BlockRepository? _blockRepository;
        static TransactionRepository? _transactionRepository;

        private static uint BlockHeight { get; set; }
        private static decimal PosDifficulty { get; set; }
        private static uint Findstakelimit { get; set; }
        private static uint StakeMinAge { get; set; }

        public static void Main(string[] args)
        {
            _blockRepository = new BlockRepository();
            _transactionRepository = new TransactionRepository(_blockRepository);
 
            Parser.Default 
                .ParseArguments<Options>(args) 
                .WithParsed<Options>(Findstake); 
 
            // Findstake(new Options 
            // { 
            //     Password = "IamGroot", 
            //     User = "thisisabigpasswwwwword", 
            //     Port = 8332, 
            //     Address = "PTNSKANTVh6mLuCbAWTmKDZeDedddcGeZZ", 
            //     ProtocolV10SwitchTime= 1635768000, 
            //     StakeMinAge = 2592000, 
            //     Findstakelimit= 1830080, 
            // }); 
        }

        public static void Findstake(Options o)
        {
            var uri = $"http://127.0.0.1:{o.Port}";
            _rpcclient = new RPCClient(uri, o.User, o.Password);

            Findstakelimit = (uint) o.Findstakelimit;
            StakeMinAge = (uint) o.StakeMinAge;

            if (!string.IsNullOrEmpty(o.Address))
            {
                FindstakeByAddress(o.Address, o.ProtocolV10SwitchTime).GetAwaiter().GetResult();
            }
            else if (!string.IsNullOrEmpty(o.FileListUnspent))
            {
                FindstakeByListUnspent(o.FileListUnspent!, o.ProtocolV10SwitchTime).GetAwaiter().GetResult();
            }
            else if (o.Test)
            {
                var _ = new TestMint();
            }
            else
            {
                ExitWithJson("ran out of options here", new List<PossibleStake>());
            }
        }

        public static async Task FindstakeByListUnspent(string file,long protocolV10SwitchTime)
        {
            BlockHeight = await _rpcclient!.GetBlockCount();
            PosDifficulty = (await _rpcclient.GetDifficulty()).pos;
            if (BlockHeight < 999)
            {
                ExitWithJson("Not connected to wallet " + BlockHeight, new List<PossibleStake>());
                return;
            }

            var parser = new BlockChainParser(_rpcclient, _blockRepository!, _transactionRepository!,
                protocolV10SwitchTime);

            var listunspents = JsonConvert.DeserializeObject<List<Unspent>>(
                await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + file));

            var unspents = listunspents!
                .Select(unspent => new UnspentTransactionData
                {
                    txid = unspent.txid,
                    vout = unspent.vout,
                    address = unspent.address
                })
                .ToList();

            await FindStakes(parser, unspents);
        }


        public static async Task FindstakeByAddress(string peercoinAddress, long protocolV10SwitchTime)
        {
            BlockHeight = await _rpcclient!.GetBlockCount();
            PosDifficulty = (await _rpcclient.GetDifficulty()).pos;
            if (BlockHeight < 999)
            {
                ExitWithJson("Not connected to wallet " + BlockHeight, new List<PossibleStake>());
                return;
            }

            var parser = new BlockChainParser(_rpcclient, _blockRepository!, _transactionRepository!,
                protocolV10SwitchTime);

            var unspents = (await _rpcclient.GetUnspents())
                .Where(unspent=>unspent.address.Equals(peercoinAddress))
                .Select(unspent => new UnspentTransactionData
                {
                    txid = unspent.txid,
                    vout = unspent.vout,
                    address = unspent.address
                })
                .ToList();
            
            if (unspents.Count == 0)
            {
                ExitWithJson("No unspents to look for ", new List<PossibleStake>());
            }

            await FindStakes(parser, unspents);
        }

        public static async Task FindStakes(BlockChainParser parser, List<UnspentTransactionData> unspents)
        {
            foreach (var unspent in unspents)
            {
                unspent.blockhash = await parser.GetBlockHash(unspent.txid);
                unspent.blockheight = await parser.Parse(unspent.blockhash);
                await parser.Parse(unspent.blockhash);
            }

            var minMarginDifficulty = 0.25f;
            var templates = new List<MintTemplate>();

            var addresses = unspents.Select(un => un.address)
                .Distinct()
                .ToList();

            foreach (var address in addresses)
            {
                var unspentsbyaddress = _transactionRepository!.GetUnspents(address);
                unspentsbyaddress.ForEach(unspent => {
                    var mintTemplate = new MintTemplate(
                        unspent.Id,
                        address,
                        unspent.BlockFromTime,
                        unspent.PrevTxOffset,
                        unspent.PrevTxTime,
                        unspent.PrevTxOutIndex,
                        (uint)unspent.PrevTxOutValue);

                    mintTemplate.SetBitsWithDifficulty(((Convert.ToSingle(PosDifficulty) - minMarginDifficulty)));

                    if (templates.All(t => t.Id != mintTemplate.Id))
                        templates.Add(mintTemplate);
                });
            }

            //load modifiers:
            var start = BlockHeight - (6 * 24 * 31) - 10;
            var end = BlockHeight;
            for (var i = start; i < end; i++)
            {
                await parser.Parse(i);
            }

            var futurestakes = StartSearch(templates);

            ExitWithJson(null, futurestakes);
        }

        public static IReadOnlyList<PossibleStake> StartSearch(List<MintTemplate> templates)
        {
            var lastblock = _blockRepository!.GetBlockState(BlockHeight - 6); //not the very last
            var lastblocktime = lastblock!.bt;

            var currentTime = ConvertToUnixTimestamp(DateTime.UtcNow);
            var start = currentTime;
            var end = lastblocktime + Findstakelimit;

            var blockModifiers = _blockRepository.GetStakeModifiers(lastblock.h);

            var results = new List<CheckStakeResult>();
            var resultsStakes = new List<PossibleStake>();

            for (var timestamp = start; timestamp < end; timestamp++)
            {
                var modifier = Mint.GetModifier(blockModifiers, timestamp, Findstakelimit);

                foreach (var template in templates)
                {
                    var result = Mint.CheckStakeKernelHash(template, timestamp, modifier!.Value, StakeMinAge);

                    switch (result.success)
                    {
                        case true when results.All(r => r.Id != template.Id && r.FutureTimestamp != timestamp):
                            //  Console.WriteLine(" " + template.OfAddress + ": " + ConvertFromUnixTimestamp(timestamp) + " difficulty: " + result.minimumDifficulty.ToString("0.00"));
                            results.Add(new CheckStakeResult
                            {
                                Id = template.Id,
                                OfAddress = template.OfAddress,
                                minimumDifficulty = result.minimumDifficulty,
                                FutureTimestamp = timestamp,
                                StakeModifier = result.StakeModifier,
                                BlockFromTime = result.BlockFromTime,
                                PrevTxOffset = result.PrevTxOffset,
                                PrevTxTime = result.PrevTxTime
                            });
                            resultsStakes = EnrichWithTemplateData(resultsStakes, templates, results);
                            //ExportResults(resultsStakes, results);
                            break;
                    }
                }
            }

            return resultsStakes;
        }


        public static void ExitWithJson(string? error, IReadOnlyList<PossibleStake> results)
        {
            var strJson = JsonConvert.SerializeObject(new ConsoleOutput
            {
                BlockHeight = BlockHeight,
                PosDifficulty = PosDifficulty,
                FutureStakes = results,
                Error = error
            }, Formatting.Indented);


            Console.WriteLine(strJson);
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), "findstakes.json");

            File.WriteAllText(fileName, strJson);
        }

        static List<PossibleStake> EnrichWithTemplateData(List<PossibleStake> resultsStakes, List<MintTemplate> templates, List<CheckStakeResult> results)
        {
            var possibleStakes = resultsStakes;

            possibleStakes.AddRange(from result in results
                let template = templates.FirstOrDefault(tp => tp.Id == result.Id)
                where template != null && result.FutureTimestamp > 0
                select new PossibleStake(result.Id,
                    template.OfAddress,
                    ConvertFromUnixTimestamp(result.FutureTimestamp),
                    result.minimumDifficulty,
                    result.FutureTimestamp,
                    result.StakeModifier,
                    result.BlockFromTime,
                    result.PrevTxOffset,
                    result.PrevTxTime,
                    result.PrevTxOutIndex));

            return possibleStakes;
        }

        private static uint ConvertToUnixTimestamp(DateTime datetimestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToUInt32((datetimestamp - origin).TotalSeconds);
        }

        private static string ConvertFromUnixTimestamp(long timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp).ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}

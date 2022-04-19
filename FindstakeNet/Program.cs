﻿using CommandLine;
using Newtonsoft.Json;
using System.Linq;

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
        private static List<string> SignAddresses { get; set; } = new List<string>();

        public static void Main(string[] args)
        {
            _blockRepository = new BlockRepository();
            _transactionRepository = new TransactionRepository(_blockRepository);
 
            // Parser.Default 
            //     .ParseArguments<Options>(args) 
            //     .WithParsed<Options>(Findstake); 

           // lets test: 
            Findstake(new Options 
            {                                   
                User = "IamGroot",     
                Password = "thisisabigpasswwwwword",             
                Port = 9002,                  
                Address = "2N947RynbLu8xxXJVrTkXr77jwo2UDpnjbT", 
                // StakeMinAge = 2592000, 
                StakeMinAge = 86400, 
                Findstakelimit= 44099, 
                //Findstakelimit= 1830080, 
                RawCoinstakeAddresses = "PACKERrvBkmkPSNNnDsPepbeT72hgwfztz",
                //Test = true
            }); 
        }

        public static void Findstake(Options o)
        {
            var uri = $"http://127.0.0.1:{o.Port}";
            _rpcclient = new RPCClient(uri, o.User, o.Password);

            Findstakelimit = (uint) o.Findstakelimit;
            StakeMinAge = (uint) o.StakeMinAge;

            if (!string.IsNullOrEmpty(o.RawCoinstakeAddresses))
            {
                SignAddresses = o.RawCoinstakeAddresses.Split(',').Select(a => a.Trim()).ToList();
            }

            if (!string.IsNullOrEmpty(o.Address))
            {
                FindstakeByAddress(o.Address).GetAwaiter().GetResult();
            }
            else if (!string.IsNullOrEmpty(o.FileListUnspent))
            {
                FindstakeByListUnspent(o.FileListUnspent!).GetAwaiter().GetResult();
            }
            else if (o.Test)
            {
                var _ = new TestMint();
                var raw = _rpcclient.CreateRawCoinStakeTransaction(new List<RawTxStakeInputs>{
                    new RawTxStakeInputs { txid = "13d55957137169ea5367341aeef82802014e3c527f18415e0fa26d1fa625b3e9", vout = 0 }
                }, new List<RawTxStakeOutput>{
                    new RawTxStakeOutput("PACKERrvBkmkPSNNnDsPepbeT72hgwfztz", 0),
                    new RawTxStakeOutput("p92W3t7YkKfQEPDb7cG9jQ6iMh7cpKLvwK", 2022.0)
                }, 1647877833)
                .GetAwaiter()
                .GetResult();

                if (raw != "01000000c99e386201e9b325a61f6da20f5e41187f523c4e010228f8ee1a346753ea6971135759d51300000000ad532102633a97eab667d165b28b19ad0848cc4f3f3e06e6b19b15cdc910d4b13f4e611f21027260ccc4dba64b04c2c07bd02da5257058ad464857919789ad9c983025fd2cba2102b813e6335216f3ae8547d283f3ab600d08c1c444f5d34fa38cfd941d939001422103131f4fb6fdc603ad3859c2c5b3f246f1ee3ba5391600e960b9be4c59f609b3dd2103b12c1b22ebbdf8e7b1c19db701484fd6fdfb63e4b117800a6838c6eb0f0e881b55aeffffffff0300000000000000000000000000000000001976a914119d6d38c00856de795ae3e6975705672ce0f5e288ac804585780000000017a91426308eea0cfcbe5bc51a5d28f297b92842db43578700000000")
                {
                    ExitWithJson("expected an different rawtransaction ", new List<PossibleStake>());
                }
            }
            else
            {
                ExitWithJson("ran out of options here", new List<PossibleStake>());
            }
        }

        public static async Task FindstakeByListUnspent(string file)
        {
            BlockHeight = await _rpcclient!.GetBlockCount();
            PosDifficulty = (await _rpcclient.GetDifficulty()).pos;
            if (BlockHeight < 999)
            {
                ExitWithJson("Not connected to wallet " + BlockHeight, new List<PossibleStake>());
                return;
            }

            var parser = new BlockChainParser(_rpcclient, _blockRepository!, _transactionRepository!);

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


        public static async Task FindstakeByAddress(string peercoinAddress)
        {
            BlockHeight = await _rpcclient!.GetBlockCount();
            PosDifficulty = (await _rpcclient.GetDifficulty()).pos;
            if (BlockHeight < 999)
            {
                ExitWithJson("Not connected to wallet " + BlockHeight, new List<PossibleStake>());
                return;
            }

            var parser = new BlockChainParser(_rpcclient, _blockRepository!, _transactionRepository!);

            var unspents = (await _rpcclient.GetUnspents())
                .Where(unspent => unspent.address.Equals(peercoinAddress))
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
                var block =_blockRepository!.GetBlockState(unspent.blockheight);
                unspent.blocktime = block!.bt;
                var output = _transactionRepository!.GetOutput(unspent.txid, unspent.vout);
                unspent.units = output!.units;
            }

            // set negative if expecting a slight increase in POS diff in future:
            // test reduce the thresthold by half:
            var minMarginDifficulty = 2.25f;
            var templates = new List<MintTemplate>();

            var addresses = unspents.Select(un => un.address)
                .Distinct()
                .ToList();

            foreach (var address in addresses)
            {
                var unspentsbyaddress = _transactionRepository!.GetUnspents(address)
                    .Where(unspent => unspents.Any(u => u.txid == unspent.Id!.Substring(2, 64) && u.vout.ToString() == unspent.Id.Substring(67)))
                    .ToList(); 

                unspentsbyaddress.ForEach(unspent =>
                {                    
                    var mintTemplate = new MintTemplate(
                        unspent.Id,
                        address,
                        unspent.BlockFromTime,
                        unspent.PrevTxOffset,
                        unspent.PrevTxTime,
                        unspent.PrevTxOutIndex,
                        unspent.PrevTxOutValue);

                    mintTemplate.SetBitsWithDifficulty(((Convert.ToSingle(PosDifficulty) - minMarginDifficulty)));
 
                    if (templates.All(t => t.Id != mintTemplate.Id))
                        templates.Add(mintTemplate);
                });
            }

            //load modifiers:
            // start with a block way back assuming there are 6 blocks in an hour: 
            // see also: consensus.nStakeTargetSpacing = 10 * 60; // 10-minute block spacing
            var start = BlockHeight - (6 * 24 * 31) - 10;
            var end = BlockHeight;
            for (var i = start; i < end; i++)
            {
                await parser.Parse(i);
            }

            var futurestakes = await StartSearch(templates);

            ExitWithJson(null, futurestakes);
        }

        public static async Task<IReadOnlyList<PossibleStake>> StartSearch(List<MintTemplate> templates)
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

                            resultsStakes = await EnrichWithTemplateData(resultsStakes, templates, result, template.PrevTxOutValue);
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

        static async Task<List<PossibleStake>> EnrichWithTemplateData(List<PossibleStake> resultsStakes, List<MintTemplate> templates, 
            CheckStakeResult result, ulong prevTxOutValue)
        {
            var possibleStakes = resultsStakes;
            var template = templates.FirstOrDefault(tp => tp.Id == result.Id);

            if (template != null && 
                result.FutureTimestamp > 0 && 
                !possibleStakes.Any(ps => ps.ID == result.Id && ps.FutureTimestamp == result.FutureTimestamp))
            {
                var txid = result.Id!.Substring(2, 64);
                var vout = result.Id.Substring(67);

                var possibleStake = new PossibleStake(prevTxOutValue, result);

                var signers = SignAddresses;
                await Task.CompletedTask;
                // possibleStake.RawTransaction = await CreateRawTransactionHash(signers, possibleStake.Address, possibleStake.Uxto,
                //         possibleStake.Vout, possibleStake.FutureTimestamp,
                //         possibleStake.NewValueAtFutureTimestamp);

                possibleStakes.Add(possibleStake);
            }
         
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

        private static async Task<string> CreateRawTransactionHash(List<string> signers, string address, string txid, int vout, long futureTimestamp, double futureOutput)
        {
            var inputs = new List<RawTxStakeInputs>{ new RawTxStakeInputs { txid = txid, vout = vout } };

            var outputs = signers!.Select(s => new RawTxStakeOutput(s, 0)).ToList();

            outputs.Add(new RawTxStakeOutput(address, futureOutput));

           return await _rpcclient!.CreateRawCoinStakeTransaction(inputs, outputs, futureTimestamp);
        }
    }
}
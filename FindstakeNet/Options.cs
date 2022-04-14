using CommandLine;

namespace FindstakeNet
{
    public class Options
    {
        [Option('u', "rpcuser", Required = true, HelpText = "rpcuser")]
        public string User { get; set; } = null!;

        [Option('w', "rpcpassword", Required = true, HelpText = "rpcpassword")]
        public string Password { get; set; } = null!;

        [Option('p', "rpcport", Required = false, Default = 8332, HelpText = "rpcport")]
        public int? Port { get; set; } 

        [Option('i', "listunspent", Required = false, Default = "", HelpText = "input json file with listunspent")]
        public string? FileListUnspent { get; set; }

        [Option('a', "peercoinaddress", Required = false, HelpText = "Use this address to findstakes. Does a rpc call listunspent")]
        public string? Address { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('m', "stakeminage", Required = false, Default= 2592000, HelpText = "time to wait after stake. 86400 in testnet")]
        public long StakeMinAge { get; set; }

        [Option('l', "findstakelimit", Required = false, Default= 1830080, HelpText = "StakeMinAge - 761920. 44099 in testnet")]
        public long Findstakelimit { get; set; }

        [Option('r', "rawcoinstakesigners", Required = false, HelpText = "comma separated peercoin addresses used for signing")]
        public string? RawCoinstakeAddresses { get; set; }

        [Option('t', "test", Required = false, HelpText = "run build-in tests")]
        public bool Test { get; set; }
    }

}

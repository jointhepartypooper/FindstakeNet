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

        [Option('s', "protocolv10switchtime", Required = false, Default= 1635768000, HelpText = "Set switchtime protocol v10. Default is Mon  1 Nov 12:00:00 UTC 2021")]
        public long ProtocolV10SwitchTime { get; set; }

        [Option('m', "stakeminage", Required = false, Default= 2592000, HelpText = "time to wait after stake")]
        public long StakeMinAge { get; set; }

        [Option('l', "findstakelimit", Required = false, Default= 1830080, HelpText = "StakeMinAge - 761920")]
        public long Findstakelimit { get; set; }

        [Option('t', "test", Required = false, HelpText = "run build-in tests")]
        public bool Test { get; set; }
    }

}

FindstakeMono
=====
### Getting to know in advance when to mint your Peercoins (protocol version v.05)


Run with: 
mono FindstakeMono.exe --rpcuser myusername --rpcpassword mypassword --listunspent listunspent.json

or:
mono FindstakeMono.exe --help

Install mono:
sudo apt-get install mono-complete

Compile with:  
mcs FindstakeMono.cs RPCClient.cs BlockChainParser.cs ProgressBar.cs -r:CommandLine.dll -r:Newtonsoft.Json.dll -r:System.Numerics.dll -r:PeercoinUtils.dll



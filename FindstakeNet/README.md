FindstakeNet
=====
### Getting to know in advance when to mint your Peercoins (protocol version v.05)

FindstakeNet originates from FindstakeJS project, which itself originates from Kac- UMint project. 
It is rewritten in .NET for .NET platforms to predict stakes for Peercoin.

Most cryptocoin mining requires specialized hardware, but Peercoin minting can be done on any computer. Minting is energy-efficient, because it is based on the Peercoins you hold, rather than on your processing power.

**But Peercoin can even be more energy-efficient! **

With FindStakeNet, you can find out in advance when your coin will mint to let you know to unlock your wallet just before it will find a stake and earn more coins. 

How to use
----------
Start up Peercoin with an wallet and a configured peercoin.conf. No unlocking required.

Enable txindex=1 in your peercoin.conf (You'll need to rebuild the database as the transaction index is normally not maintained, start using -reindex to do so)

* make sure to have the following in file peercoin.conf:
``` bash
listen=1
server=1
txindex=1
rpcuser=change_this_to_a_long_random_user
rpcpassword=change_this_to_a_long_random_password
rpcport=8332
```

start application in ".\FindstakeNet\bin\Release\FindstakeNet.exe"

go to Settings and set the rpcuser en rpcpassword

Connect

Load either a json file from a listunspent command (recommended), or load from some wallet

press Start

Eport to csv when done

Dependencies for usage:
.NET Framework 4.5.2 https://dotnet.microsoft.com/download/dotnet-framework/net452 

Dependencies for developers:
------------
 * SharpDevelop 5.1.0.5216 downloadeded from https://www.techspot.com/downloads/7214-sharpdevelop.html
 * optional: LiteDB Studio https://github.com/mbdavid/LiteDB.Studio
  
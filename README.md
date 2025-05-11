FindstakeNet
=====
### Getting to know in advance when to mint your Peercoins (protocol version v.05)

FindstakeNet originates from FindstakeJS project, which itself originates from Kac- UMint project. 
It is rewritten in .NET for .NET platforms to predict stakes for Peercoin.

Most cryptocoin mining requires specialized hardware, but Peercoin minting can be done on any computer. Minting is energy-efficient, because it is based on the Peercoins you hold, rather than on your processing power.

**But Peercoin can even be more energy-efficient! **

With FindStakeNet, you can find out in advance when your coin will mint to let you know to unlock your wallet just before it will find a stake and earn more coins. 

#### Screenshot:
![Alt text](https://i.imgur.com/elyOpLM.png "peercoind command listunspent")

How to use
----------

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

Dependencies for usage:
.NET 8 runtime/SDK

How to compile app:
dotnet publish FindstakeNet.csproj -c Release --runtime linux-x64 --no-self-contained

examples:
./FindstakeNet -u IamGroot -w thisisabigpasswwwwword -a PTNSKANTVh6mLuCbAWTmKDZeDedddcGeZZ -s 1635768000 -m 2592000 -l 1830080

tip: spend some time reading Options.cs 
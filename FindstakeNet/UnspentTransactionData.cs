// ReSharper disable InconsistentNaming
namespace FindstakeNet
{
    public class UnspentTransactionData
    {
        public string txid = null!;
        public uint vout;
        public string address = null!;
        public uint blockheight;
        public string? blockhash;
    }
}

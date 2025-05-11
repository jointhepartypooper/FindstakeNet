// ReSharper disable InconsistentNaming

namespace FindstakeNet
{
    public class UnspentTransactions
    {
        public string Id { get; set; } = null!;
        public uint BlockFromTime { get; set; }
        public uint PrevTxOffset { get; set; }
        public uint PrevTxTime { get; set; }
        public ulong PrevTxOutValue { get; set; }
        public uint PrevTxOutIndex { get; set; }
    }
}

namespace FindstakeNet
{
    public class MintTemplate
    {
        public string Id { get; private set; }
        public string OfAddress { get; private set; }

        public uint BlockFromTime { get; private set; }
        public uint PrevTxOffset { get; private set; }
        public uint PrevTxTime { get; private set; }
        public uint PrevTxOutIndex { get; private set; }
        public uint PrevTxOutValue { get; private set; }

        public uint? Bits { get; private set; }

        public MintTemplate(string id,
            string address,
            uint blockFromTime,
            uint prevTxOffset,
            uint prevTxTime,
            uint prevTxOutIndex,
            uint prevTxOutValue)
        {
            this.Id = id;
            this.OfAddress = address;
            this.BlockFromTime = blockFromTime;
            this.PrevTxOffset = prevTxOffset;
            this.PrevTxTime = prevTxTime;
            this.PrevTxOutIndex = prevTxOutIndex;
            this.PrevTxOutValue = prevTxOutValue;
        }


        public MintTemplate(string id,
            string address,
            uint blockFromTime,
            uint prevTxOffset,
            uint prevTxTime,
            uint prevTxOutIndex,
            uint prevTxOutValue,
            uint bits)
        {
            this.Id = id;
            this.OfAddress = address;
            this.BlockFromTime = blockFromTime;
            this.PrevTxOffset = prevTxOffset;
            this.PrevTxTime = prevTxTime;
            this.PrevTxOutIndex = prevTxOutIndex;
            this.PrevTxOutValue = prevTxOutValue;
            this.Bits = bits;
        }

        public uint SetBitsWithDifficulty(float diff)
        {
            this.Bits = Mint.BigToCompact(Mint.DiffToTarget(diff));
            return this.Bits.Value;
        }
    }

}

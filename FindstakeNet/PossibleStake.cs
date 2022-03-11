// ReSharper disable InconsistentNaming
namespace FindstakeNet
{
    public class PossibleStake
    {
        /// <summary>
        /// format id: to{{hash}}_{{vout}}		e.g.:to4044279f77475566da7c94a2e96a3bf1796f02a39f96e42e4b1a48cc59fdbf23_1
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Peercoin address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// datetime notation of FutureTimestamp
        /// </summary>
        public string StakeDate { get; set; }

        /// <summary>
        /// the pos difficulty at FutureTimestamp should stay below max:
        /// </summary>
        public float MaxDifficulty { get; set; }

        public ulong StakeModifier;
        public uint BlockFromTime;
        public uint PrevTxOffset;
        public uint PrevTxTime;
        public uint PrevTxOutIndex;

        /// <summary>
        /// unixtime in seconds
        /// </summary>
        public uint FutureTimestamp;

        public PossibleStake(string id, string address, string timestamp, float minimumDifficulty,
            uint txTime, ulong stakeModifier, uint blockFromTime, uint prevTxOffset,
            uint prevTxTime, uint prevTxOutIndex)
        {
            this.ID = id;
            this.Address = address;
            this.StakeDate = timestamp;
            this.MaxDifficulty = minimumDifficulty;
            this.StakeModifier = stakeModifier;
            this.BlockFromTime = blockFromTime;
            this.PrevTxOffset = prevTxOffset;
            this.PrevTxTime = prevTxTime;
            this.PrevTxOutIndex = prevTxOutIndex;
            this.FutureTimestamp = txTime;
        }
    }
}

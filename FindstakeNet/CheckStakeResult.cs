// ReSharper disable InconsistentNaming
using System.Numerics;
#pragma warning disable CS8618

namespace FindstakeNet
{
    public class CheckStakeResult
    {
        /// <summary>
        /// format id: to{{hash}}_{{vout}}		e.g.:to4044279f77475566da7c94a2e96a3bf1796f02a39f96e42e4b1a48cc59fdbf23_1
        /// </summary>
        public string Id;       

        public string OfAddress;
   
        public bool success;
   
        public BigInteger minTarget;
   
        public byte[] hash;
   
        public float minimumDifficulty;

        //as used in the CheckStakeKernelHash:
        public ulong StakeModifier;
        public uint BlockFromTime;
        public uint PrevTxOffset;
        public uint PrevTxTime;
        public uint PrevTxOutIndex;
        public uint FutureTimestamp;
    }
}

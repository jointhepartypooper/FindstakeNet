﻿// ReSharper disable InconsistentNaming
namespace FindstakeNet
{
    public class PossibleStake
    {
        /// <summary>
        /// format id: to{{Uxto}}_{{vout}}		e.g.:to4044279f77475566da7c94a2e96a3bf1796f02a39f96e42e4b1a48cc59fdbf23_1
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// e.g.: 4044279f77475566da7c94a2e96a3bf1796f02a39f96e42e4b1a48cc59fdbf23
        /// </summary>
        public string Uxto { get; set; }

        /// <summary>
        /// e.g.: 1
        /// </summary>
        public int Vout { get; set; }

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

        public ulong StakeModifier { get; set; }

        public string StakeModifierHex { get; set; }
        public uint BlockFromTime { get; set; }
        public uint PrevTxOffset { get; set; }
        public uint PrevTxTime { get; set; }
        public uint PrevTxOutIndex { get; set; }

        /// <summary>
        /// unixtime in seconds
        /// </summary>
        public uint FutureTimestamp { get; set; }

        /// <summary>
        /// New output value at mint time
        /// </summary>
        public double NewValueAtFutureTimestamp { get; set; }

        public string RawTransaction { get; set; } = "";

        public PossibleStake(ulong units, CheckStakeResult result)
        {
            this.ID = result.Id;
            this.Uxto =  result.Id.Substring(2, 64);
            this.Vout = int.Parse(result.Id.Substring(67));
            this.Address = result.OfAddress;
            this.StakeDate = ConvertFromUnixTimestamp(result.FutureTimestamp);
            this.MaxDifficulty = result.minimumDifficulty;
            this.StakeModifier = result.StakeModifier;
            this.StakeModifierHex = result.StakeModifierHex;
            this.BlockFromTime = result.BlockFromTime;
            this.PrevTxOffset = result.PrevTxOffset;
            this.PrevTxTime = result.PrevTxTime;
            this.PrevTxOutIndex = result.PrevTxOutIndex;
            this.FutureTimestamp = result.FutureTimestamp;
            this.NewValueAtFutureTimestamp = CalcNewUnits(result.BlockFromTime, result.FutureTimestamp, units);
        }     

        //todo move calculation elsewhere
        private double CalcNewUnits(uint blocktime, uint futureTimestamp, ulong units)
        {
            /*
            Equation at 2.9975% plus a basis 1.2 peercoin:
            A = P(1 + 0.029975*t) + 1.2      
            */   
            var perc = 0.029975;
            var YEAR_IN_SECONDS = 31556952; // Average length of year in Gregorian calendar
 
            var time = futureTimestamp - blocktime;
            var seconds = Math.Min(time, YEAR_IN_SECONDS); // cap at 1 year max
            var fractionyears = (seconds) / (1.0 * YEAR_IN_SECONDS);
            
            // just floor the double:
            var newUnits = Convert.ToUInt64((units * (1 + (perc * fractionyears))) + (1.2 * 1000000));
            return Math.Round(0.000001 * newUnits, 6);           
        }

        private static string ConvertFromUnixTimestamp(long timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp).ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}
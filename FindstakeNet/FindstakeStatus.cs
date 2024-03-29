﻿// ReSharper disable InconsistentNaming

namespace FindstakeNet
{
    public class FindstakeStatus
    {
        public decimal difficulty { get; set; }
        public uint lastupdatedblocktime { get; set; }
        public uint lastupdatedblock { get; set; }
        public List<StakeModifier> blockModifiers { get; set; } = null!;
    }
}

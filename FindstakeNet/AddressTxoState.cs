﻿// ReSharper disable InconsistentNaming
namespace FindstakeNet
{
    public class AddressTxoState
    {
        public string address { get; set; } = null!;
        public string txo { get; set; } = null!; // (unspent) output tx
        public int idx { get; set; } // position in transaction
    }
}

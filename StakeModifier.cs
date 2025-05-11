// ReSharper disable InconsistentNaming
namespace FindstakeNet
{
    public class StakeModifier
    {
        public uint bt { get; set; } //block unixtime
        public string mr { get; set; } = null!; //Modifier
        public ulong stakeModifier { get; set; }
    }
}
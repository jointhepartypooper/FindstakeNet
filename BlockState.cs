// ReSharper disable InconsistentNaming
namespace FindstakeNet
{
    public class BlockState
    {
        public uint h { get; set; } //height

        public string f { get; set; } = null!; //pow, pos
        public uint bt { get; set; } //block unixtime
        public string mr { get; set; } = null!; //blocks are clustered by modifier
        public string hash { get; set; } = null!; //64 char
        public List<string> tx { get; set; } = null!;
        public uint nTx { get; set; } //length tx
    }
}
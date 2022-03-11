// ReSharper disable InconsistentNaming

namespace FindstakeNet
{
    public class OutputState
    {
        public string Id { get; set; } = null!; //tohash_position

        public string hash { get; set; } = null!;
        public uint idx { get; set; }

        public bool spent { get; set; }
        public ulong units { get; set; }
        public string? data { get; set; } 
        public bool hasoptreturn { get; set; }

        public string ToId()
        {
            return "to" + this.hash + "_" + this.idx;
        }
    }
}

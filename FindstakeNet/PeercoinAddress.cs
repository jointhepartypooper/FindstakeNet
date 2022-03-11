namespace FindstakeNet
{
    public class PeercoinAddress
    {
        private readonly List<byte> hash;
        private readonly byte version;

        public PeercoinAddress(string address)
        {
            this.hash = DecodeString(address);
            this.version = PeercoinConstants.NetworkVersion;
        }


        public static bool IsAddress(string address)
        {
            var catched = false;
            try
            {
                // ReSharper disable once UnusedVariable
                var peercoinAddress = new PeercoinAddress(address);
            }
            catch (Exception)
            {
                catched = true;
            }
            return !catched;
        }


        public List<byte> DecodeString(string address)
        {
            var bytes = Encoders.Base58.DecodeData(address);

            var firsthash = bytes.Take(21).ToArray();
            var checksum = Mint.DoubleSha256(firsthash.ToList()).ToArray();

            if (firsthash[0] != PeercoinConstants.NetworkVersion ||
                checksum[0] != bytes[21] ||
                checksum[1] != bytes[22] ||
                checksum[2] != bytes[23] ||
                checksum[3] != bytes[24])
            {
                throw new ArgumentException($"{address} is not an address", nameof(address));
            }

            return firsthash.ToList().Skip(1).ToList();
        }


        public override string ToString()
        {
            var target = (Array.ConvertAll(this.hash.ToArray(), x => (x))).ToList();
            target.Insert(0, version);

            var checksum = Mint.DoubleSha256(target);
            var bytes = target.Concat(checksum.Take(4)).ToArray();

            return Encoders.Base58.EncodeData(bytes);
        }
    }
}

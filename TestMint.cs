using System.Numerics;

namespace FindstakeNet
{
    internal class TestMint
    {
        public TestMint()
        {
            ShouldCovertToTarget();
            ShouldCovertToCompact();
            TestRoundtrip();
            BigToCompactTest();
            IncTests();
            BitLenTests();
            CheckStakeKernelHashTest();
        }

        public void AssertThrow(bool result, string error)
        {
            if (!result)
            {
                throw new Exception(error);
            }
        }

        public void ShouldCovertToTarget()
        {
            var set = new[] {
                new object[]{21.345F, "1263037534708007065233903449077122707924414303996398674892185141248"},
                new object[]{6.15F, "4383664314154799371057726451385090274339071401122701325781576974336"},
                new object[]{56.08F, "480733493559447088382085585624766859301132742736115421675251564544"},
                new object[]{18.5007F, "1457217015401058230710519728509336428455980242272185129797989433344"},
                new object[]{7F, "3851362069648898194808762653391849309347085353924666742105907920896"}
            };

            foreach (var i in set)
            {
                float data = (float)i[0];
                string encoded = (string)i[1];

                var bn = Mint.DiffToTarget(data);

                bool isValid = bn.ToString() == encoded;
                AssertThrow(isValid, "ShouldCovertToTarget failed");
            }
        }

        public void ShouldCovertToCompact()
        {
            var dataset = new[] {

                new long[]{7, 16842752},
                new long[]{734, 16842754},
                new long[]{42, 16842752},
                new long[]{1984, 16842759}
            };

            foreach (var i in dataset)
            {
                var data = (uint)i[0];
                var expected = i[1];

                var bn = Mint.IncCompact(data);

                bool isValid = bn == expected;
                AssertThrow(isValid, "ShouldCovertToCompact failed");
            }
        }

        /// <summary>
        /// https://github.com/kac-/umint/blob/master/mint_test.go
        /// </summary>
        public void TestRoundtrip()
        {
            var set = new List<uint>{ 471105047 };

            set.ForEach(compact => {
                var expected = Mint.BigToCompact(Mint.DiffToTarget(Mint.CompactToDiff(compact)));

                bool isValid = compact == expected;
                AssertThrow(isValid, "TestRoundtrip failed");
            });
        }

        public void BigToCompactTest()
        {
            var set = new[] {
                new uint[]{8388608, 67141632},
            };


            foreach (var i in set)
            {
                // ReSharper disable once RedundantCast
                long data = (long)i[0];
                var b0 = new BigInteger(data);
                var c0 = Mint.BigToCompact(b0);
                // ReSharper disable once RedundantCast
                uint encoded = (uint)i[1];
                
                bool isValid = c0 == encoded;
                AssertThrow(isValid, "BigToCompactTest failed");
            }
        }

        public void IncTests()
        {

            var list = new List<long>(){
                //0x1, 0x2, 0x12, 0x123, 0x1234, 0x12345, 0x123456,0x800000,0x1234567,				
                0x7fffff, 0x7ffffffff,
            };
            //			var big = new BigInteger(0x1234567);
            //			var g = long.Parse(Mint.CompactToBig(Mint.BigToCompact(big)).ToString());
            //			list.Add(g);					
            //			list.Add(g-1);

            list.ForEach(num => {

                bool isValid = IncTest(num);
                AssertThrow(isValid, "IncTests failed");

            });

        }

        public void BitLenTests()
        {
            var set = new[] {

                new long[]{8388608, 4},
                new[]{7778388608, 5},
                new []{5476765465654, 6},
                new []{54767656543465654, 8}
            };


            foreach (var i in set)
            {
                long data = i[0];
                long encoded = i[1];

                var b1 = new BigInteger(data);

                var bytes = BigInteger.Abs(b1).ToByteArray().Length;

                bool isValid = bytes == (int)encoded;
                AssertThrow(isValid, "BitLenTests failed");
            }
        }
        

        /// <summary>
        /// https://github.com/kac-/umint/blob/master/ukernel_test.go
        /// </summary>
        public void CheckStakeKernelHashTest()
        {
            var result = Mint.CheckStakeKernelHash(new MintTemplate(
                    "totest",
                    "Pxxxx",
                    1394219584,
                    160,
                    1394219584,
                    1,
                    210090000,
                    471087779
                ),
                1411634680,
                15161125480764745506, 2592000);

            bool isValid = result.success;

            AssertThrow(isValid, "CheckStakeKernelHash failed");

            var tpl0Hash = new List<byte>{
                0x00, 0x00, 0x00, 0xdb, 0x33, 0x30, 0x88, 0x15,
                0x19, 0xa4, 0xf3, 0x2b, 0x90, 0x91, 0xb0, 0x93,
                0x0f, 0x24, 0xec, 0x6f, 0xb0, 0x90, 0x0a, 0xcf,
                0xbf, 0xb0, 0xc2, 0x26, 0xc7, 0xbc, 0x31, 0x92,
            };

            //little endian
            tpl0Hash.Reverse();

            for (int i = 0; i < result.hash.Length; i++)
            {
                isValid = result.hash[i] == tpl0Hash[i];

                AssertThrow(isValid, "CheckStakeKernelHash part 2 failed");
            }


            // check if template satisfies min target
            var minbits = Mint.IncCompact(Mint.BigToCompact(result.minTarget));

            var minresult = Mint.CheckStakeKernelHash(new MintTemplate(
                    "totest",
                    "Pxxx",
                    1394219584,
                    160,
                    1394219584,
                    1,
                    210090000,
                    minbits
                ),
                1411634680,
                15161125480764745506, 2592000);


            isValid = minresult.success;

            AssertThrow(isValid, "CheckStakeKernelHash part 3 failed");

        }


        private bool IncTest(long num)
        {
            var b0 = new BigInteger(num);
            var c0 = Mint.BigToCompact(b0);
            BigInteger b1;
            if (num < 0x800000)
            {
                b1 = new BigInteger(num + 1);
            }
            else
            {
                b1 = new BigInteger(num);
                var bytes = BigInteger.Abs(b1).ToByteArray().Length;
                var leftshift = 8 * (bytes - 3);
                var toadd = 1 << leftshift;

                b1 = BigInteger.Add(b1, new BigInteger(toadd));
            }
            var c1 = Mint.BigToCompact(b1);
            if (c1 == c0)
            {
                return false;
            }
            var newC1 = Mint.IncCompact(c0);

            return newC1 == c1;
        }

    }
}

namespace FindstakeNet
{
    public class BlockRepository
    {
        private readonly object listlock = new object();
        private List<BlockState> blocks = new();


        public BlockState? GetBlockState(uint height)
        {
            lock (listlock)
            {
                var results = blocks.Where(x => x.h == height).ToList();
                if (results.Count > 0)
                {
                    return results[0];
                }
            }

            return null;
        }


        public List<BlockState> GetBlockStates(List<uint> heights)
        {
            lock (listlock)
            {
                var results = blocks.Where(block => heights.Contains(block.h)).ToList();
                if (results.Count > 0)
                {
                    return results;
                }
            }

            return new List<BlockState>();
        }


        public IReadOnlyList<StakeModifier> GetStakeModifiers(uint height)
        {
            var start = height - 6 * 24 * 31;
            var end = height;
            var results = new List<StakeModifier>();

            lock (listlock)
            {
                foreach (var block in blocks.Where(x => x.h > start && x.h <= end).OrderBy(o => o.h))
                {
                    if (results.All(r => r.mr != block.mr))
                    {
                        results.Add(new StakeModifier
                        {
                            bt = block.bt,
                            mr = block.mr,
                            stakeModifier = Convert.ToUInt64(block.mr, 16)
                        });
                    }
                }
            }
            return results;
        }


        public void SetBlockState(BlockState state)
        {
            //if (state == null) return;

            lock (listlock)
            {
                var results = blocks.Where(x => x.h == state.h).ToList();
                if (results.Count > 0)
                {
                    var oldstate = results[0];
                    oldstate.bt = state.bt;
                    oldstate.f = state.f;
                    oldstate.hash = state.hash;
                    oldstate.mr = state.mr;
                    oldstate.tx = state.tx;
                    oldstate.nTx = state.nTx;
                }
                else
                {
                    blocks.Add(state);
                }
            }
        }

        public void DeleteAll()
        {

            lock (listlock)
            {
                blocks = new List<BlockState>();
            }
        }
    }
}
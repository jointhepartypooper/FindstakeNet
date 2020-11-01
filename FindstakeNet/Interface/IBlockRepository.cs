using System;
using PeercoinUtils.Crypto;
using FindstakeNet.Model;
using System.Collections.Generic;

namespace FindstakeNet.Interface
{
	public interface IBlockRepository
	{
		BlockState GetBlockState(uint height);
		void SetBlockState(BlockState state);
		List<StakeModifier> GetStakeModifiers(uint height);
		List<BlockState> GetBlockStates(List<uint> heights);
	}
}

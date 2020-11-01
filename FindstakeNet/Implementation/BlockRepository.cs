﻿using System;
using System.Linq;
using System.Collections.Generic;
using FindstakeNet.Interface;
using FindstakeNet.Model;
using PeercoinUtils.Crypto;

namespace FindstakeNet.Implementation
{
	public class BlockRepository: IBlockRepository
	{			
		private readonly object listlock = new object();
		private List<BlockState> blocks;
			
		public BlockRepository()
		{				
			blocks = new List<BlockState>();
		}
		
							
		public BlockState GetBlockState(uint height)
		{			
			lock(listlock)
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
			lock(listlock)
			{
				var results = blocks.Where(block => heights.Contains(block.h)).ToList();
			    if (results.Count > 0)
			    {
			    	return results;
			    }			
			}			
					
			return new List<BlockState>();
		}	
		
		
		public List<StakeModifier> GetStakeModifiers(uint height)
		{			
			var start = height - 6 * 24 * 31;			
			var end = height;
				
			lock(listlock)
			{
				var results = blocks.Where(x => x.h > start && x.h <= end).ToList();
			    if (results.Count > 0)
			    {
			    	return results
			    		.Select(result => new StakeModifier{
			    	                      	bt = result.bt,
			    	                      	mr = result.mr,
			    	                      	stakeModifier = Convert.ToUInt64(result.mr, 16)			    	                      	
			    	                      })
			    		.OrderBy(r => r.bt)
			    		.ToList();
			    }			
			}
			return new List<StakeModifier>();
		}
		
				
		public void SetBlockState(BlockState state)
		{
			if (state == null) return;
						
			lock(listlock)
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
							
			lock(listlock)
			{
				blocks = new List<BlockState>();
			}
		}

	}
}

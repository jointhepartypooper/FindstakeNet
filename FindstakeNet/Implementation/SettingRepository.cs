using System;
using System.Linq;
using LiteDB;
using FindstakeNet.Interface;
using FindstakeNet.Model;

namespace FindstakeNet.Implementation
{
	public class SettingRepository: ISettingsRepository
	{
		private readonly IBlockRepository blockRepository;
		
		private string connectionstring;
		public SettingRepository(IBlockRepository blockRepository)
		{
			this.connectionstring = "Filename=" + @"SettingsData.db;Connection=shared;InitialSize=1MB";
			this.blockRepository = blockRepository;
		}
		
		public FindstakeStatus GetFindstakeStatus()
		{						
			var settingState = GetSyncState();
				
			var block = blockRepository.GetBlockState(settingState.CurBH);
			var modifiers = blockRepository.GetStakeModifiers(block.h);
			return new FindstakeStatus
			{
				difficulty = settingState.Diff,
				lastupdatedblock = block.h,
				lastupdatedblocktime = block.bt,
				blockModifiers = modifiers
			};
		}
		
		public string GetRpcUser()
		{						
			var val = GetValue("rpcuser");
			
			return string.IsNullOrWhiteSpace(val) 
				? "puthereanuniqueusername"
				: val;
		}
		
		
		public void SetRpcUser(string newvalue)
		{
			SetState("rpcuser", newvalue);
		}
		
		
		public string GetRpcPassword()
		{				
			var val = GetValue("rpcpassword");
			
			return string.IsNullOrWhiteSpace(val) 
				? "puthereanuniqueuserpasswordforwallet"
				: val;
		}

		
		public void SetRpcPassword(string newvalue)
		{
			SetState("rpcpassword", newvalue);
		}
		
		
		public string GetRpcUri()
		{
			var val = GetValue("rpcuri");
			
			return string.IsNullOrWhiteSpace(val) 
				? "http://127.0.0.1:8332"
				: val;
		}
		
		public void SetRpcUri(string newvalue)
		{
			SetState("rpcuri", newvalue);
		}
		
		
		private string GetValue(string key)
		{				
			using(var db = new LiteDatabase(connectionstring))
			{				
				var col = GetCollection(db);
			    var results = col.Query().Where(x => x.Id.Equals(key)).Limit(1).ToList();
			    if (results.Count > 0)
			    {
			    	return results[0].Value;
			    }			    
			}	
			return "";
		}
		
		public SettingState GetSyncState()
		{			
			using(var db = new LiteDatabase(connectionstring))
			{				
				var col = GetCollection(db);
			    var results = col.Query().Where(x => x.Id.Equals("SyncState")).Limit(1).ToList();
			    if (results.Count > 0)
			    {
			    	return results[0];
			    }			    
			}						
			return new SettingState();
		}		
		
		

		
		public void SetSyncState(SettingState state)
		{
			using(var db = new LiteDatabase(connectionstring))
			{		    
			    var col = GetCollection(db);
			    var results = col.Query().Where(x => x.Id.Equals("SyncState")).Limit(1).ToList();
			    if (results.Count > 0)
			    {
			    	var oldstate = results[0];
			    	oldstate.Id = "SyncState";
			    	oldstate.CurBH = state.CurBH;
			    	oldstate.Diff = state.Diff;

			    	col.Update(oldstate);			    	
			    }
			    else
			    {		    				    	
			    	state.Id = "SyncState";			    	
			    	col.Insert(state);
			    }
			}
		}
		
				
		private void SetState(string key, string newvalue)
		{
			using(var db = new LiteDatabase(connectionstring))
			{		    
			    var col = GetCollection(db);
			    var results = col.Query().Where(x => x.Id.Equals(key)).Limit(1).ToList();
			    if (results.Count > 0)
			    {
			    	var oldstate = results[0];
			    	oldstate.Id = key;
 
			    	oldstate.Value = newvalue;
			    	col.Update(oldstate);			    	
			    }
			    else
			    {		    				    	   	
			    	col.Insert(new SettingState{Id = key, Value = newvalue});
			    }
			}
		}
				
		private ILiteCollection<SettingState> GetCollection(LiteDatabase db)
		{
			// Get a collection (or create, if doesn't exist)
			return db.GetCollection<SettingState>("metadata");
		}		
	
	}
}

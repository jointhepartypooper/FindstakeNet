using System;
using FindstakeNet.Model;

namespace FindstakeNet.Interface
{
	public interface ISettingsRepository
	{
		string GetRpcUser();
		string GetRpcPassword();
		string GetRpcUri();
		void SetRpcUri(string newvalue);
		void SetRpcPassword(string newvalue);
		void SetRpcUser(string newvalue);
		SettingState GetSyncState();
		void SetSyncState(SettingState state);
		FindstakeStatus GetFindstakeStatus();
	}
}

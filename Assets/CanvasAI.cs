using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Steamworks;


public class CanvasAI : MonoBehaviour
{

	public Dropdown friendLobbyiesDropdown;

	List<CSteamID> friendSteamIDList = new List<CSteamID>();
	List<CSteamID> friendLobbyIDList = new List<CSteamID>();



	public void PrintID()
    {
        print(SteamUser.GetSteamID());
    }

	public void RequestFriendLobbyList()
	{
		friendLobbyiesDropdown.ClearOptions();
		if (SteamManager.Initialized)
		{
			int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
			Debug.Log("Found " + friendCount + " friends!");
			var friendLobbyOptions = new List<Dropdown.OptionData>();
			var inviteFriendOptions = new List<Dropdown.OptionData>();
			for (int i = 0; i < friendCount; i++)
			{
				CSteamID friendSteamID = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);

				friendSteamIDList.Add(friendSteamID);
				inviteFriendOptions.Add(new Dropdown.OptionData(SteamFriends.GetFriendPersonaName(friendSteamID)));

				//Debug.Log("Friend: " + SteamFriends.GetFriendPersonaName(friendSteamID) + " - " + friendSteamID + " - Level " + SteamFriends.GetFriendSteamLevel(friendSteamID));
				if (SteamFriends.GetFriendGamePlayed(friendSteamID, out FriendGameInfo_t friendGameInfo) && friendGameInfo.m_steamIDLobby.IsValid())
				{
					// friendGameInfo.m_steamIDLobby is a valid lobby, you can join it or use RequestLobbyData() get its metadata
					//Debug.Log(SteamFriends.GetFriendPersonaName(friendSteamID) + " is hosting a lobby!");
					friendLobbyOptions.Add(new Dropdown.OptionData(SteamFriends.GetFriendPersonaName(friendSteamID) + "'s Room"));
					friendLobbyIDList.Add(friendGameInfo.m_steamIDLobby);
				}
				else
				{
					//Debug.Log(SteamFriends.GetFriendPersonaName(friendSteamID) + " is not hosting a lobby, ignore.");
				}
			}
			friendLobbyiesDropdown.AddOptions(friendLobbyOptions);
			//inviteFriendDropdown.AddOptions(inviteFriendOptions);
		}
	}

	public void JoinFriendLobby()
	{
		SteamMatchmaking.JoinLobby(friendLobbyIDList[friendLobbyiesDropdown.value]);
	}
}

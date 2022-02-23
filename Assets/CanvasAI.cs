using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Steamworks;
using Netcode.Transports;
using static FrontMan;


public class CanvasAI : MonoBehaviour
{

	public Dropdown friendLobbyiesDropdown;

	List<CSteamID> friendSteamIDList = new List<CSteamID>();
	List<CSteamID> friendLobbyIDList = new List<CSteamID>();


	Callback<LobbyEnter_t> lobbyEnter;
	Callback<LobbyCreated_t> lobbyCreated;

	CSteamID lobbyID;
	CSteamID hostSteamID;


	private void Start()
	{

		lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered); // Fires on entering a lobby in both host and client
		lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated); // Fires on creating a lobby in host

	}
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

	

	public void StartHost()
	{
		NetworkManager.Singleton.OnClientConnectedCallback += (clientId) => {
			Debug.Log($"Client connected, clientId={clientId}");
		};

		NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) => {
			Debug.Log($"Client disconnected, clientId={clientId}");
		};

		NetworkManager.Singleton.OnServerStarted += () => {
			Debug.Log("Server started");
			//GameObject spawnedObject = Instantiate(spawnedObjectPerfab);
			//spawnedObject.GetComponent<NetworkObject>().Spawn();
		};


		NetworkManager.Singleton.StartHost();

		SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);

		hostSteamID = SteamUser.GetSteamID();
	}

	public void StartClient()
	{
		NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
		NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;

		NetworkManager.Singleton.StartClient();

		NetworkManager.Singleton.GetComponent<SteamNetworkingTransport>().ConnectToSteamID = hostSteamID.m_SteamID;

		Debug.Log($"Joining room hosted by {NetworkManager.Singleton.GetComponent<SteamNetworkingTransport>().ConnectToSteamID}");

		//SceneManager.LoadScene("MultiplayerDemo");
		//SceneManager.sceneLoaded += (scene, mode) => {
		//	NetworkManager.Singleton.StartClient();
		//	SwitchToGameplay();
		//};

	}
	void ClientConnected(ulong clientId)
	{
		Debug.Log($"I'm connected, clientId={clientId}");
	}

	void ClientDisconnected(ulong clientId)
	{
		Debug.Log($"I'm disconnected, clientId={clientId}");
		NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnected;   // remove these else they will get called multiple time if we reconnect this client again
		NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnected;
	}

	public void JoinFriendLobby()
	{
		print(friendLobbyIDList[friendLobbyiesDropdown.value]);
		SteamMatchmaking.JoinLobby(friendLobbyIDList[friendLobbyiesDropdown.value]);
		//StartClient();
		//SteamMatchmaking.JoinLobby(friendLobbyIDList[friendLobbyiesDropdown.value]);
	}
	void OnLobbyCreated(LobbyCreated_t result)
	{
		if (result.m_eResult == EResult.k_EResultOK)
			Debug.Log("Lobby created successfully - LobbyID=" + result.m_ulSteamIDLobby);
		else
			Debug.Log("Lobby created failed - LobbyID=" + result.m_ulSteamIDLobby);
		lobbyID = (CSteamID)result.m_ulSteamIDLobby;
		string personalName = SteamFriends.GetPersonaName();
		SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, "name", personalName + "'s Room");
	}

	void OnLobbyEntered(LobbyEnter_t result)
	{
		print(SteamMatchmaking.GetLobbyOwner(friendLobbyIDList[friendLobbyiesDropdown.value]));
		FM.Transport.ConnectToSteamID = (ulong)SteamMatchmaking.GetLobbyOwner(friendLobbyIDList[friendLobbyiesDropdown.value]);

		lobbyID = (CSteamID)result.m_ulSteamIDLobby;
		Debug.Log(result.m_EChatRoomEnterResponse);
		if (result.m_EChatRoomEnterResponse == 1)
			Debug.Log($"Successfully joined lobby {SteamMatchmaking.GetLobbyData((CSteamID)result.m_ulSteamIDLobby, "name")}!");
		else
			Debug.Log("Failed to join lobby.");

		int playerCount = SteamMatchmaking.GetNumLobbyMembers(lobbyID);

		Debug.Log("PlayerCount: " + playerCount);
		// Join host's game directly
		if (playerCount > 1)
		{
			var ownerSteamID = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, 0);
			hostSteamID = ownerSteamID;
			StartClient();
		}

		//GameObject spawnedObject = Instantiate(spawnedObjectPerfab);
		//spawnedObject.GetComponent<NetworkObject>().Spawn();
	}
}

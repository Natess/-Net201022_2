using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsideRoomPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button LeaveGameButton;
    [SerializeField] private Button StartGameButton;
    [SerializeField] private Toggle CloseRoomToggle;

    [SerializeField] private PanelsManager panelManager;
    public GameObject PlayerListEntryPrefab;

    private Dictionary<int, GameObject> playerListEntries;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        LeaveGameButton.onClick.AddListener(OnLeaveGameButtonClicked);
        StartGameButton.onClick.AddListener(OnStartGameButtonClicked);
        CloseRoomToggle.onValueChanged.AddListener(OnCloseRoomToggleValueChange);
        playerListEntries = new Dictionary<int, GameObject>();
    }

    private void OnCloseRoomToggleValueChange(bool value)
    {
        PhotonNetwork.CurrentRoom.IsOpen = !value;
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnLeftRoom()
    {
        panelManager.SetActivePanel("SelectionPanel"); 

        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject entry = Instantiate(PlayerListEntryPrefab);
        entry.transform.SetParent(panelManager.InsideRoomPanel.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<NewPlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListEntries.Add(newPlayer.ActorNumber, entry);

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (playerListEntries.ContainsKey(otherPlayer.ActorNumber))
        {
            Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherPlayer.ActorNumber);
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
            CloseRoomToggle.gameObject.SetActive(true);
            CloseRoomToggle.isOn = !PhotonNetwork.CurrentRoom.IsOpen;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<NewPlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            }
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();

    }

    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel("DemoAsteroids-GameScene");
    }

    internal void Init()
    {
        CloseRoomToggle.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        CloseRoomToggle.isOn = false;

        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(panelManager.InsideRoomPanel.transform);
            entry.transform.localScale = Vector3.one;

            entry.GetComponent<NewPlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<NewPlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            }

            playerListEntries.Add(p.ActorNumber, entry);
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());

        Hashtable props = new Hashtable
            {
                {AsteroidsGame.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient || !CloseRoomToggle.isOn)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }
    public void LocalPlayerPropertiesUpdated()
    {
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

}

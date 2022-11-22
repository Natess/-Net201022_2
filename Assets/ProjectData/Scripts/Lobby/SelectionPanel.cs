using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanel : MonoBehaviour
{
    [SerializeField] private Button _createRoomButton;
    [SerializeField] private Button _randomRoomButton;
    [SerializeField] private Button _listRoomsButton;
    [SerializeField] private Button _findRoomButton;
    [SerializeField] private PanelsManager panelManager;

    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    void Start()
    {
        _createRoomButton.onClick.AddListener(() => panelManager.SetActivePanel("CreateRoomPanel"));
        _randomRoomButton.onClick.AddListener(() => OnJoinRandomRoomButtonClicked());
        _listRoomsButton.onClick.AddListener(() => OnRoomListButtonClicked());
        _findRoomButton.onClick.AddListener(() => OnFindRoomButtonClicked());

    }

    private void OnFindRoomButtonClicked()
    {
        panelManager.SetActivePanel("FindRoomPanel");
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        panelManager.SetActivePanel("JoinRandomRoomPanel");

        PhotonNetwork.JoinRandomRoom();
    }


    public void OnRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        panelManager.SetActivePanel("RoomListPanel");
    }

}

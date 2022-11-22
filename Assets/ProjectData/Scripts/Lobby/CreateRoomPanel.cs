using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField RoomNameInputField;
    [SerializeField] private InputField MaxPlayersInputField;
    [SerializeField] private Button CreateRoomButton;
    [SerializeField] private Button BackButton;
    [SerializeField] private Toggle InvisinleToggle;

    [SerializeField] private PanelsManager panelManager;


    private void Awake()
    {
        InvisinleToggle.isOn = false;
        BackButton.onClick.AddListener(OnBackButtonClicked);
        CreateRoomButton.onClick.AddListener(() => { OnCreateRoomButtonClicked(); });
    }

    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        panelManager.SetActivePanel("SelectionPanel");
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = RoomNameInputField.text;
        roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

        byte maxPlayers;
        byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000, IsVisible = !InvisinleToggle.isOn };

        GUIUtility.systemCopyBuffer = roomName;
        PhotonNetwork.CreateRoom(roomName, options, null);
        //panelManager.SetActivePanel("InsideRoomPanel");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        panelManager.SetActivePanel("SelectionPanel");
    }
    public override void OnJoinedRoom()
    {
        panelManager.SetActivePanel("InsideRoomPanel");
    }

}

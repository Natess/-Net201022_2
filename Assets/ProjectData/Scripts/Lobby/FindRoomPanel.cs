using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindRoomPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField RoomNameInputField;
    [SerializeField] private Button FindRoomButton;
    [SerializeField] private Button BackButton;

    [SerializeField] private PanelsManager panelManager;


    private void Awake()
    {
        BackButton.onClick.AddListener(OnBackButtonClicked);
        FindRoomButton.onClick.AddListener(() => { OnFindRoomButtonClicked(); });
    }

    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        panelManager.SetActivePanel("SelectionPanel");
    }

    public void OnFindRoomButtonClicked()
    {
        string roomName = RoomNameInputField.text;
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        panelManager.SetActivePanel("SelectionPanel");
    }

    public override void OnJoinedRoom()
    {
        panelManager.SetActivePanel("InsideRoomPanel");
    }

}

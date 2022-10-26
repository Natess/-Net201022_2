using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Authorization : MonoBehaviourPunCallbacks
{
    [SerializeField] private string _playFabTitle;

    [SerializeField] private Text _playFabConnectionLabel;
    [SerializeField] private Button _playFabLoginButton;

    [SerializeField] private Text _photonConnectionLabel;
    [SerializeField] private Button _photonConnectButton;
    [SerializeField] private Text _photonConnectButtonText;

    void Start()
    {
        _photonConnectButton.enabled = false;

        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            PlayFabSettings.staticSettings.TitleId = _playFabTitle;

        var request = new LoginWithCustomIDRequest
        {
            CustomId = "TestUser",
            CreateAccount = true
        };

        _playFabLoginButton.onClick.AddListener(() => PlayFabLogIn(request));
        _photonConnectButton.onClick.AddListener(() => onPhotonConnectButtonClick());

    }

    private void PlayFabLogIn(LoginWithCustomIDRequest request)
    {
        PlayFabClientAPI.LoginWithCustomID(request,
           result =>
           {
               _playFabConnectionLabel.text = "PlayFab connection success";
               _playFabConnectionLabel.color = Color.green;
               _playFabLoginButton.enabled = false;
               _photonConnectButton.enabled = true;

               Debug.Log(result.PlayFabId);
               PhotonNetwork.AuthValues = new AuthenticationValues(result.PlayFabId);
               PhotonNetwork.NickName = result.PlayFabId;
              // Connect();
           },
           error =>
           {
               _playFabConnectionLabel.text = "PlayFab connection error";
               _playFabConnectionLabel.color = Color.red;
               Debug.LogError(error);
           });
    }

    private void onPhotonConnectButtonClick()
    {
        if (PhotonNetwork.IsConnected)
        {
            Disconnect();
            _photonConnectionLabel.text = "You are disconnected";
            _photonConnectionLabel.color = Color.red;
            _photonConnectButtonText.text = "Photon Connect";
        }
        else
        {
            Connect();
            _photonConnectionLabel.text = "You are connected";
            _photonConnectionLabel.color= Color.green;
            _photonConnectButtonText.text = "Photon Disconnect";
        }
    }

    private void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{Random.Range(0, 9999)}");
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = PhotonNetwork.AppVersion;
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("OnConnectedToMaster");
        if (!PhotonNetwork.InRoom)
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{Random.Range(0, 9999)}");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("OnCreatedRoom");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"OnJoinedRoom {PhotonNetwork.CurrentRoom.Name}");
    }

    private void Disconnect()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("OnDisconnected");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("OnLeftRoom");
    }
}

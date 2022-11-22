using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Authorization : MonoBehaviourPunCallbacks
{
    [SerializeField] private string _playFabTitle;

    [SerializeField] private PanelsManager panelManager;

    [SerializeField] private Text _playFabConnectionLabel;
    [SerializeField] private Button _playFabLoginButton;
    [SerializeField] private Text _playFabRemberUserLabel;
    [SerializeField] private Button _playFabFogetUserDataButton;
    [SerializeField] private Button _checkInButton;
    [SerializeField] private Button _logInButton;


    [SerializeField] private Text _photonConnectionLabel;
    [SerializeField] private Button _photonConnectButton;
    [SerializeField] private Text _photonConnectButtonText;

    private const string AUTHENTICATION_KEY = "AUTHENTICATION_KEY";
    private const string AUTHENTICATION_NAME = "AUTHENTICATION_NAME";

    private struct Data
    {
        public bool NeedCreation;
        public string Id;
    }

    void Start()
    {
        _photonConnectButton.enabled = false;

        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            PlayFabSettings.staticSettings.TitleId = _playFabTitle;
        
        _photonConnectButton.onClick.AddListener(() => onPhotonConnectButtonClick());

        _playFabLoginButton.onClick.AddListener(onPlayFabLoginButtonClick);
        _checkInButton.onClick.AddListener(panelManager.onCheckInClick);
        _logInButton.onClick.AddListener(panelManager.onLogInClick);
        _playFabFogetUserDataButton.onClick.AddListener(FogetUserData);
    }

    private void onPlayFabLoginButtonClick()
    {
        var needCreation = !PlayerPrefs.HasKey(AUTHENTICATION_KEY);
        var id = PlayerPrefs.GetString(AUTHENTICATION_KEY, Guid.NewGuid().ToString());
        if (!needCreation)
        {
            _playFabRemberUserLabel.text = "Your user was found";
        }

        var request = new LoginWithCustomIDRequest
        {
            CustomId = id,
            CreateAccount = true
        };

        PlayFabLogIn(request, new Data { Id = id, NeedCreation = needCreation });
    }



    private void PlayFabLogIn(LoginWithCustomIDRequest request, Data data)
    {
        var slider = panelManager.RunSlider();
        PlayFabClientAPI.LoginWithCustomID(request,
           result =>
           {
               PlayerPrefs.SetString(AUTHENTICATION_KEY, data.Id);
               //_playFabConnectionLabel.text = "PlayFab connection success";
               //_playFabConnectionLabel.color = Color.green;
               //_playFabLoginButton.enabled = false;
               //_photonConnectButton.enabled = true;

               Debug.Log(result.PlayFabId);
               Debug.Log(data.Id);

               panelManager.StopSlider(slider);

               panelManager.GoToUnauthorizationLoginPanel(gameObject, result.PlayFabId);
           },
           error =>
           {
               _playFabConnectionLabel.text = "PlayFab connection error";
               _playFabConnectionLabel.color = Color.red;
               Debug.LogError(error);
               panelManager.StopSlider(slider);
           },
           data);
    }

    private void FogetUserData()
    {
        var slider = panelManager.RunSlider();
        PlayFabClientAPI.ForgetAllCredentials();
        PlayerPrefs.DeleteKey(AUTHENTICATION_KEY);

        _playFabRemberUserLabel.text = "";
        panelManager.StopSlider(slider);
    }

    #region Photon

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
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{UnityEngine.Random.Range(0, 9999)}");
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
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{UnityEngine.Random.Range(0, 9999)}");
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

    #endregion

}

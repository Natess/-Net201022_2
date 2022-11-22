using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnauthorizationLoginPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField _userNameInput;
    [SerializeField] private Button _logInButton;
    [SerializeField] private PanelsManager panelManager;
    [SerializeField] private Button LogInBackButton;

    private string _userName;
    private string _playerId;

    private void Awake()
    {
        _userNameInput.onValueChanged.AddListener(SetUserName);
        _logInButton.onClick.AddListener(LogInClick);
        LogInBackButton.onClick.AddListener(() => { panelManager.BackOnMainPanel(gameObject); });
        _userNameInput.text = "Player " + UnityEngine.Random.Range(1000, 10000);

    }
    internal void Init(string playerId)
    {
        _playerId = playerId;
    }

    void SetUserName(string value)
    {
        _userName = value;
    }

    void LogInClick()
    {
        PhotonNetwork.AuthValues = new AuthenticationValues(_playerId);
        PhotonNetwork.NickName = _userName;
        PhotonNetwork.LocalPlayer.NickName = _userName;
        Connect();
    }

    private void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = PhotonNetwork.AppVersion;
        }
        panelManager.GoToSelectionPanel(gameObject);
    }

    public override void OnConnectedToMaster()
    {
        panelManager.GoToSelectionPanel(gameObject);
    }

}

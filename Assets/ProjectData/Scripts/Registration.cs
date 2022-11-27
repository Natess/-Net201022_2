using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Registration : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField _userNameInput;
    [SerializeField] private InputField _userEmailInput;
    [SerializeField] private InputField _userPasswordInput;
    [SerializeField] private Button _registrationButton;
    [SerializeField] private Text _errorText;
    [SerializeField] private PanelsManager panelManager;
    [SerializeField] private Button CheckInBackButton;


    private string _userName;
    private string _userEmail;
    private string _userPassword;


    private void Awake()
    {
        _userNameInput.onValueChanged.AddListener(SetUserName);
        _userEmailInput.onValueChanged.AddListener(SetUserEmail);
        _userPasswordInput.onValueChanged.AddListener(SetUserPassword);
        _registrationButton.onClick.AddListener(Registrate);
        CheckInBackButton.onClick.AddListener(() => { panelManager.BackOnMainPanel(gameObject); });
    }

    void SetUserName(string value)
    {
        _userName = value;
    }
    void SetUserEmail(string value)
    {
        _userEmail = value;
    }
    void SetUserPassword(string value)
    {
        _userPassword = value;
    }

    void Registrate()
    {
        var slider = panelManager.RunSlider();
        PlayFabClientAPI.RegisterPlayFabUser(new PlayFab.ClientModels.RegisterPlayFabUserRequest
        {
            Username = _userName,
            Email = _userEmail,
            Password = _userPassword,
            RequireBothUsernameAndEmail = true
        },
        result =>
        {
            _errorText.gameObject.SetActive(false);
            Debug.Log(result.Username);
            _userPasswordInput.text = "";
            _userPassword = "";
            PlayfabUserData.GetUserData(result.PlayFabId);
            panelManager.StopSlider(slider);

            PhotonNetwork.AuthValues = new AuthenticationValues(result.PlayFabId);
            PhotonNetwork.NickName = _userName;
            PhotonNetwork.LocalPlayer.NickName = _userName;
            Connect();
            //panelManager.GoToStorePanel(gameObject);
        },
        error =>
        {
            _errorText.gameObject.SetActive(true);
            _errorText.text = $"{error.ErrorMessage}\n{error.ErrorDetails.First().Value.First()} ";
            _userPasswordInput.text = "";
            _userPassword = "";
            panelManager.StopSlider(slider);
        }); ;
        ;
        ;
    }

    private void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = PhotonNetwork.AppVersion;
        }
        //panelManager.GoToSelectionPanel(gameObject);
    }

    public override void OnConnectedToMaster()
    {
        panelManager.GoToCharacterSelecPanel(gameObject);
    }
}

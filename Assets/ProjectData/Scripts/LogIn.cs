using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LogIn : MonoBehaviour
{
    [SerializeField] private InputField _userNameInput;
    [SerializeField] private InputField _userPasswordInput;
    [SerializeField] private Button _logInButton;
    [SerializeField] private Text _errorText;
    [SerializeField] private PanelsManager panelManager;
    [SerializeField] private Button LogInBackButton;

    private string _userName;
    private string _userPassword;

    private void Awake()
    {
        _userNameInput.onValueChanged.AddListener(SetUserName);
        _userPasswordInput.onValueChanged.AddListener(SetUserPassword);
        _logInButton.onClick.AddListener(LogInClick);
        LogInBackButton.onClick.AddListener(() => { panelManager.BackOnMainPanel(gameObject); });
    }

    void SetUserName(string value)
    {
        _userName = value;
    }

    void SetUserPassword(string value)
    {
        _userPassword = value;
    }

    void LogInClick()
    {
        var slider = panelManager.RunSlider();
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = _userName,
            Password = _userPassword,
        },
        result =>
        {
            _errorText.gameObject.SetActive(false);
            Debug.Log(result.PlayFabId);
            panelManager.StopSlider(slider);
            panelManager.GoToStorePanel(gameObject);
        },
        error =>
        {
            _errorText.gameObject.SetActive(true);
            _errorText.text = $"{error.ErrorMessage}\n";
            if(error.ErrorDetails is not null)
                _errorText.text += error.ErrorDetails.First().Value.First();
            panelManager.StopSlider(slider);
        });
    }
}


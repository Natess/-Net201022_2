using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfo : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private Button UserInfoBackButton;
    [SerializeField] private PanelsManager panelManager;

    private void Awake()
    {
        UserInfoBackButton.onClick.AddListener(() => {
            panelManager.BackOnMainPanel(gameObject); });

    }

    public void onSetActive()
    {
        var slider = panelManager.RunSlider();
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
            result => {
                //_text.text = result.AccountInfo.ToJson();
                _text.text = $"Name: {result.AccountInfo.Username ?? ""}\n";
                _text.text += $"DisplayName: {result.AccountInfo.TitleInfo.DisplayName ?? ""}\n";
                _text.text += $"Email: {result.AccountInfo.PrivateInfo.Email ?? ""}\n";
                _text.text += $"LastLogin: {result.AccountInfo.TitleInfo.LastLogin.ToString() ?? ""}\n";
                panelManager.StopSlider(slider);
            },
            error => {
                Debug.Log(error);
                panelManager.StopSlider(slider);
            }
            );


    }
}

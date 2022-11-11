using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelsManager : MonoBehaviour
{
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject CheckInPanel;
    [SerializeField] private GameObject LogInPanel;
    [SerializeField] private GameObject UserPanel;
    [SerializeField] private GameObject StorePanel;

    [SerializeField] private GameObject SliderPanel;
    [SerializeField] private Slider Slider;

    public void BackOnMainPanel(GameObject currentPanel)
    {
        MainPanel.SetActive(true);
        currentPanel.SetActive(false);
    }

    public void GoToUserPanel(GameObject currentPanel)
    {
        UserPanel.SetActive(true);
        currentPanel.SetActive(false);
        UserPanel.GetComponent<UserInfo>().onSetActive();
    }

    public void GoToStorePanel(GameObject currentPanel)
    {
        StorePanel.SetActive(true);
        currentPanel.SetActive(false);
        StorePanel.GetComponent<StoreManager>().Init();
    }

    public void onLogInClick()
    {
        MainPanel.SetActive(false);
        LogInPanel.SetActive(true);
    }

    public void onCheckInClick()
    {
        MainPanel.SetActive(false);
        CheckInPanel.SetActive(true);
    }

    public Coroutine RunSlider()
    {
        return StartCoroutine(SliderCorutine());
    }

    private IEnumerator SliderCorutine()
    {
        Slider.value = 0;
        SliderPanel.SetActive(true);
        for (int i = 0; i < 600; i++)
        {
            Slider.value = i;
            yield return new WaitForFixedUpdate();
        }
        SliderPanel.SetActive(false);
    }

    public void StopSlider(Coroutine coroutine)
    {
        SliderPanel.SetActive(false);
        StopCoroutine(coroutine);
    }
}

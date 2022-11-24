using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class PlayfabUserData
{
    public static int PlayFabHealth { get; set;}

    public static void SetUserData()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                {"Health", "20"}
            }
        },
        result => Debug.Log("Successfully updated user data"),
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public static void UpdateUserData()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                {"Health", PlayFabHealth.ToString()}
            }
        },
        result => Debug.Log("Successfully updated user data " + PlayFabHealth),
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });

    }

    public static void GetUserData(string myPlayFabId)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myPlayFabId,
            Keys = null
        }, result =>
        {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey("Health"))
            {
                Debug.Log("No Health");
                SetUserData();
                PlayFabHealth = 20;
            }
            else
            {
                Debug.Log("Health: " + result.Data["Health"].Value);
                PlayFabHealth = int.Parse(result.Data["Health"].Value);
            }

        }, (error) =>
        {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }
}

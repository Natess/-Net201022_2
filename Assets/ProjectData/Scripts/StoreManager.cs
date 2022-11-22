using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private Button UserInfoBackButton;
    [SerializeField] private PanelsManager panelManager;
    private Coroutine slider;
    private readonly Dictionary<string, CatalogItem> _catalog = new Dictionary<string, CatalogItem>();

    private void Start()
    {
        UserInfoBackButton.onClick.AddListener(() =>
        {
            _text.text = string.Empty;
            _catalog.Clear();
            panelManager.BackOnMainPanel(gameObject);
        });
    }

    public void Init()
    {
        slider = panelManager.RunSlider();
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess,
        OnFailure);
    }

    private void OnFailure(PlayFabError error)
    {
        try
        {
            var errorMessage = error.GenerateErrorReport();
            Debug.LogError($"Something went wrong: {errorMessage}");
        }
        finally
        {
            panelManager.StopSlider(slider);
        }
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        try
        {
            HandleCatalog(result.Catalog);
            Debug.Log($"Catalog was loaded successfully!");
        }
        finally
        {
            panelManager.StopSlider(slider);
        }
    }

    private void HandleCatalog(List<CatalogItem> catalog)
    {
        var bStr = new StringBuilder();
        foreach (var item in catalog)
        {
            bStr.Append($"{item.DisplayName, 50}  " +
                $"{string.Join("; ", item.VirtualCurrencyPrices.Select(v => $"{v.Value} {v.Key}")), 20}   " +
                $"{item.Description, 40}\n");
            _catalog.Add(item.ItemId, item);
            Debug.Log($"Catalog item {item.ItemId} was added successfully!");
        }
        _text.text = bStr.ToString();
    }

}

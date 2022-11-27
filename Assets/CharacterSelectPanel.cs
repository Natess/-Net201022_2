using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPanel : MonoBehaviour
{
    private const string GOLD = "GL";

    [SerializeField] private Text InfoText;
    [SerializeField] private GameObject newCharacterCreatePanel;
    [SerializeField]
    private Button EmptySlotOneButton;
    [SerializeField]
    private Button EmptySlotTwoButton;
    [SerializeField]
    private Text CharacterSlotOneText;
    [SerializeField]
    private Text CharacterSlotTwoText;
    [SerializeField]
    private Button BackButton;
    [SerializeField]
    private Button CreateButton;
    [SerializeField]
    private InputField NameInputField;
    [SerializeField]
    private Dropdown HeroTypeDropdown;


    private string _characterName;
    private int _characterType;

    private List<CharacterResult> _characters = new List<CharacterResult>();

    private void Start()
    {
        GetCharacters();
        EmptySlotOneButton.onClick.AddListener(OpenCreateNewCharacterPrompt);
        EmptySlotTwoButton.onClick.AddListener(OpenCreateNewCharacterPrompt);
        BackButton.onClick.AddListener(CloseCreateNewCharacterPrompt);
        CreateButton.onClick.AddListener(GetCharacterToken);
        NameInputField.onValueChanged.AddListener(OnNameInputFieldChanged);
        HeroTypeDropdown.onValueChanged.AddListener(OnHeroTypeDropdownChanged);
    }

    private void OnHeroTypeDropdownChanged(int value)
    {
        _characterType = value;
    }

    public void GetCharacters()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
        res =>
        {
            Debug.Log($"Characters owned: + {res.Characters.Count}");
            if (_characters.Count > 0)
            {
                _characters.Clear();
            }
            var slotId = 0;
            foreach (var characterResult in res.Characters)
            {
                _characters.Add(characterResult);
                GetCharacterInfo(characterResult, slotId++);
            }
            ShowCharacterSlotButtons();
        },
        Debug.LogError);
    }

    public void GetCharacterInfo(CharacterResult character, int numberSlot)
    {
        PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest { CharacterId = character.CharacterId },
            result =>
            {
                if (numberSlot == 0)
                    CharacterSlotOneText.text = GetCharacterInfo(character, result);
                else
                    CharacterSlotTwoText.text = GetCharacterInfo(character, result);
            },
            Debug.LogError);
    }

    private string GetCharacterInfo(CharacterResult character, GetCharacterStatisticsResult characterStats)
    {
        return $"{character.CharacterName}\n" +
            $"{string.Join(";\n", characterStats.CharacterStatistics.Select(v => $"{v.Key}: {v.Value}"))}";
    }

    private void ShowCharacterSlotButtons()
    {
        EmptySlotOneButton.gameObject.SetActive(false);
        EmptySlotTwoButton.gameObject.SetActive(false);
        CharacterSlotOneText.gameObject.SetActive(false);
        CharacterSlotTwoText.gameObject.SetActive(false);
        switch (_characters.Count)
        {
            case 0:
                //No characters
                EmptySlotOneButton.gameObject.SetActive(true);
                EmptySlotTwoButton.gameObject.SetActive(true);
                break;
            case 1:
                //One character
                CharacterSlotOneText.gameObject.SetActive(true);
                EmptySlotTwoButton.gameObject.SetActive(true);
                break;

            case 2:
                //Two characters
                CharacterSlotOneText.gameObject.SetActive(true);
                CharacterSlotTwoText.gameObject.SetActive(true);
                break;
        }
    }

    public void OpenCreateNewCharacterPrompt()
    {
        EmptySlotOneButton.interactable = false;
        EmptySlotTwoButton.interactable = false;
        newCharacterCreatePanel.SetActive(true);
    }

    public void CloseCreateNewCharacterPrompt()
    {
        EmptySlotOneButton.interactable = true;
        EmptySlotTwoButton.interactable = true;
        newCharacterCreatePanel.SetActive(false);
    }

    public void OnNameInputFieldChanged(string changedName)
    {
        _characterName = changedName;
    }

    private void GetCharacterToken()
    {
        PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest
        {
            CatalogVersion = "0.1",
            StoreId = "CharactersStore"
        },
        result => {
            CreateNewCharavter(result.Store[_characterType]);
        },
       err => Error(err));
    }

    public void CreateNewCharavter(StoreItem item)
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            CatalogVersion = "0.1",
            ItemId = item.ItemId,
            Price = (int)item.VirtualCurrencyPrices[GOLD],
            VirtualCurrency = GOLD
        },
        result => {
            var item = result.Items[0];
            CreateCharacterWithItemId(item.ItemId);
        },
        Debug.LogError);
    }

    public void CreateCharacterWithItemId(string itemId)
    {
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
        {
            CharacterName = _characterName,
            ItemId = itemId
        }, result =>
        {
            UpdateCharacterStatistics(result.CharacterId);
        }, 
        Debug.LogError);
    }

    private void UpdateCharacterStatistics(string characterId)
    {
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
        {
            CharacterId = characterId,
            CharacterStatistics = new Dictionary<string, int>
            {
                {"Level", 1},
                {"XP", 0},
                {"Gold", 0},
                {"Damage", _characterType == 0 ? 10 : 20},
                {"HP", _characterType == 0? 100 : 50 }
}
        }, result =>
        {
            Debug.Log($"Initial stats set, telling client to update character list");
            CloseCreateNewCharacterPrompt();
            GetCharacters();
        },
        Debug.LogError);
    }


    private void Error(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        CloseCreateNewCharacterPrompt();
    }
}


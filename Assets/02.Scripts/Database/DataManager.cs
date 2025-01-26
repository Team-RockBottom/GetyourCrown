using ExitGames.Client.Photon.StructWrapping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

namespace GetyourCrown.Database

{
    public class DataManager : MonoBehaviour
    {
        public static DataManager instance { get; private set; }
        public PlayerData CurrentPlayerData { get; private set; } = new PlayerData();

        private const string NICKNAME_KEY = "Nickname";
        private const string COINS_KEY = "Coins";
        private const string CHARACTER_KEY = "Chracters";

        public event Action<string> OnNicknameChanged;
        public event Action<int> OnCoinsChanged;

        public string Nickname { get; private set; }
        public int Coins { get; private set; }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async void Start()
        {
            await UnityServices.InitializeAsync();
        }

        /// <summary>
        /// �÷��̾� ������ ����
        /// </summary>
        /// <returns></returns>
        public async Task LoadPlayerDataAsync()
        {
            try
            {
                //�ҷ��� ������ Ű�� ����
                var keys = new HashSet<string> { NICKNAME_KEY, COINS_KEY, CHARACTER_KEY };
                //cloudsave���� ������ �ε�
                var loadedData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

                if (loadedData.ContainsKey(NICKNAME_KEY))
                {
                    //Item.Value ��ü�� ���ڿ��� ���������� ��ȯ���� �ʰ� ����ȭ�� ���·� ����
                    //GetAsString() �޼���� �� ����ȭ�� �����͸� ���ڿ��� ��ȯ�ϴ� ����
                    Nickname = loadedData[NICKNAME_KEY].Value.GetAsString();
                    OnNicknameChanged?.Invoke(Nickname); //���� �̺�Ʈ
                }
                if (loadedData.ContainsKey(COINS_KEY))
                {
                    Coins = loadedData[COINS_KEY].Value.GetAs<int>();
                    OnCoinsChanged?.Invoke(Coins);
                }
                if (loadedData.ContainsKey(CHARACTER_KEY))
                {
                    //Debug.Log(CurrentPlayerData);
                    //var characterData = loadedData[CHARACTER_KEY].Value;
                    //Debug.Log(characterData);

                    // Dictionary<string, Item> ���¿��� ���� ��������
                    //if (characterData is Dictionary<string, Unity.Services.CloudSave.Models.Item> items)
                    //{
                    //    Debug.Log(characterData);
                    //    var parsedCharacterData = new Dictionary<int, bool>();

                    //     �� �������� ��ȸ�ϸ鼭 ������ ó��
                    //    foreach (var item in items)
                    //    {
                    //        int itemId = int.Parse(item.Key);  
                    //        bool isLocked = item.Value.Get<bool>();

                    //        parsedCharacterData.Add(itemId, isLocked);
                    //        Debug.Log($"{itemId}, {isLocked}");
                    //    }

                    //     �Ľ̵� �����͸� CurrentPlayerData�� ����
                    //    CurrentPlayerData.CharactersLocked = parsedCharacterData;

                    //    foreach (var item in CurrentPlayerData.CharactersLocked)
                    //    {
                    //        Debug.Log($"{item.Key}, {item.Value}");
                    //    }
                    //}

                    var characterData = loadedData[CHARACTER_KEY].Value;

                    // CloudSave Item ��ü�� Dictionary<string, Item>���� ��ȯ
                    var jsonString = characterData.GetAsString();
                    var parsedItems = JsonUtility.FromJson<Dictionary<string, bool>>(jsonString);

                    var parsedCharacterData = new Dictionary<int, bool>();
                    foreach (var kvp in parsedItems)
                    {
                        int itemId = int.Parse(kvp.Key);
                        bool isLocked = kvp.Value;
                        parsedCharacterData[itemId] = isLocked;
                    }

                    CurrentPlayerData.CharactersLocked = parsedCharacterData;

                    foreach (var item in CurrentPlayerData.CharactersLocked)
                    {
                        Debug.Log($"{item.Key}, {item.Value}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log($"error LoadPlayerdata  : {e}");
            }
        }
        
        public async Task SaveNicknameAsync(string newnickname)
        {
            try
            {
                Nickname = newnickname;
                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    { NICKNAME_KEY, newnickname }
                });

                OnNicknameChanged?.Invoke(Nickname);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public async Task SaveCoinsAsync(int coins)
        {
            try
            {
                Coins = coins;
                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    { COINS_KEY, Coins }
                });

                OnCoinsChanged?.Invoke(Coins);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public async Task UpdatePlayerCoinsAsync(int coins)
        {
            if (Coins + coins < 0)
            {
                Debug.Log("Coin shortage");
                return;
            }

            Coins += coins;
            await SaveCoinsAsync(Coins);

            Debug.Log("Player Coin Update");
        }

        public async Task DefaultCharacterAsync()
        {
            try
            {
                var defaultCharacters = new Dictionary<int, bool>
                {
                    { 0, true },
                    { 1, false },
                    { 2, false },
                    { 3, false },
                };

                CurrentPlayerData.CharactersLocked = defaultCharacters;

                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    { CHARACTER_KEY, CurrentPlayerData.CharactersLocked }
                });
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}

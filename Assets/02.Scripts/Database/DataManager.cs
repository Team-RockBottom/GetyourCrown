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
        /// 플레이어 데이터 저장
        /// </summary>
        /// <returns></returns>
        public async Task LoadPlayerDataAsync()
        {
            try
            {
                //불러올 데이터 키를 저장
                var keys = new HashSet<string> { NICKNAME_KEY, COINS_KEY, CHARACTER_KEY };
                //cloudsave에서 데이터 로드
                var loadedData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

                if (loadedData.ContainsKey(NICKNAME_KEY))
                {
                    //Item.Value 자체는 문자열을 직접적으로 반환하지 않고 직렬화된 형태로 저장
                    //GetAsString() 메서드는 이 직렬화된 데이터를 문자열로 변환하는 역할
                    Nickname = loadedData[NICKNAME_KEY].Value.GetAsString();
                    OnNicknameChanged?.Invoke(Nickname); //변경 이벤트
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

                    // Dictionary<string, Item> 형태에서 직접 가져오기
                    //if (characterData is Dictionary<string, Unity.Services.CloudSave.Models.Item> items)
                    //{
                    //    Debug.Log(characterData);
                    //    var parsedCharacterData = new Dictionary<int, bool>();

                    //     각 아이템을 순회하면서 데이터 처리
                    //    foreach (var item in items)
                    //    {
                    //        int itemId = int.Parse(item.Key);  
                    //        bool isLocked = item.Value.Get<bool>();

                    //        parsedCharacterData.Add(itemId, isLocked);
                    //        Debug.Log($"{itemId}, {isLocked}");
                    //    }

                    //     파싱된 데이터를 CurrentPlayerData에 적용
                    //    CurrentPlayerData.CharactersLocked = parsedCharacterData;

                    //    foreach (var item in CurrentPlayerData.CharactersLocked)
                    //    {
                    //        Debug.Log($"{item.Key}, {item.Value}");
                    //    }
                    //}

                    var characterData = loadedData[CHARACTER_KEY].Value;

                    // CloudSave Item 객체를 Dictionary<string, Item>으로 변환
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

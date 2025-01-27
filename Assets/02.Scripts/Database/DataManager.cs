using GetyourCrown.UI;
using Newtonsoft.Json;
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
                    string jsonString = loadedData[CHARACTER_KEY].Value.GetAsString();
                    CurrentPlayerData.CharactersLocked = JsonConvert.DeserializeObject<Dictionary<int, bool>>(jsonString);
                    //저장된 JSON문자열을 원래 객체로 역직렬화하여 불러오기
                }
            }
            catch (Exception e)
            {
                ShowConfirmWindow($"사용자의 데이터를 불러오지 못했습니다. 다시 시도해주세요. {e}");
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
                ShowConfirmWindow($"닉네임을 저장을 실패했습니다. 다시 시도해주세요. {e}");

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
                ShowConfirmWindow($"코인을 저장을 실패했습니다. 다시 시도해주세요. {e}");

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
                string jsonString = JsonConvert.SerializeObject(CurrentPlayerData.CharactersLocked);
                //데이터를 직렬화해서 저장
                //데이터를 JSON 문자열로 변환

                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    { CHARACTER_KEY, jsonString }
                });
            }
            catch (Exception e)
            {
                ShowConfirmWindow($"캐릭터 데이터를 저장하지 못하였습니다. 다시 시도해주세요. {e}");
            }
        }

        public async Task<bool> UnLockCharacterAsync(int characterId)
        {
            try
            {
                if (CurrentPlayerData.CharactersLocked.ContainsKey(characterId))
                {
                    CurrentPlayerData.CharactersLocked[characterId] = false;
                    string jsonString = JsonConvert.SerializeObject(CurrentPlayerData.CharactersLocked);

                    await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                    {
                        { CHARACTER_KEY, jsonString },
                    });

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                ShowConfirmWindow($"캐릭터 해제를 실패하였습니다. 다시 시도해주세요 {e}");
                return false;
            }
        }

        void ShowConfirmWindow(string message)
        {
            UI_ConfirmWindow _uiConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            _uiConfirmWindow.Show(message);
        }
    }
}

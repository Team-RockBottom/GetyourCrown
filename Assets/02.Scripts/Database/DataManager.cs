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
                    string jsonString = loadedData[CHARACTER_KEY].Value.GetAsString();
                    CurrentPlayerData.CharactersLocked = JsonConvert.DeserializeObject<Dictionary<int, bool>>(jsonString);
                    //����� JSON���ڿ��� ���� ��ü�� ������ȭ�Ͽ� �ҷ�����
                }
            }
            catch (Exception e)
            {
                ShowConfirmWindow($"������� �����͸� �ҷ����� ���߽��ϴ�. �ٽ� �õ����ּ���. {e}");
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
                ShowConfirmWindow($"�г����� ������ �����߽��ϴ�. �ٽ� �õ����ּ���. {e}");

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
                ShowConfirmWindow($"������ ������ �����߽��ϴ�. �ٽ� �õ����ּ���. {e}");

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
                //�����͸� ����ȭ�ؼ� ����
                //�����͸� JSON ���ڿ��� ��ȯ

                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    { CHARACTER_KEY, jsonString }
                });
            }
            catch (Exception e)
            {
                ShowConfirmWindow($"ĳ���� �����͸� �������� ���Ͽ����ϴ�. �ٽ� �õ����ּ���. {e}");
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
                ShowConfirmWindow($"ĳ���� ������ �����Ͽ����ϴ�. �ٽ� �õ����ּ��� {e}");
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

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
                var keys = new HashSet<string> { NICKNAME_KEY, COINS_KEY };
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
                    Coins = int.Parse(loadedData[COINS_KEY].Value.GetAsString());
                    OnCoinsChanged?.Invoke(Coins);
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
    }
}

using ExitGames.Client.Photon;
using GetyourCrown.Network;
using GetyourCrown.UI;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
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

        [SerializeField] CharacterSpecRepository _characterSpecRepository;
        //public bool isGuset = false;
        private const string NICKNAME_KEY = "Nickname";
        private const string COINS_KEY = "Coins";
        private const string CHARACTER_KEY = "Chracters";
        private const string LAST_CHARACTER_KEY = "LastCharacter";

        public event Action<string> OnNicknameChanged;
        public event Action<int> OnCoinsChanged;

        public string Nickname { get; private set; }
        public int Coins { get; private set; }
        public int LastCharacter { get; private set; }

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
                var keys = new HashSet<string> { NICKNAME_KEY, COINS_KEY, CHARACTER_KEY, LAST_CHARACTER_KEY };
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
                if (loadedData.ContainsKey(LAST_CHARACTER_KEY))
                {
                    LastCharacter = loadedData[LAST_CHARACTER_KEY].Value.GetAs<int>();

                    Hashtable props = new Hashtable
                    {
                        { PlayerInRoomProperty.CHARACTER_ID, LastCharacter }
                    };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                ShowConfirmWindow($"������� �����͸� �ҷ����� ���߽��ϴ�. �ٽ� �õ����ּ���. {e}");
            }
        }

        //public void GuestData()
        //{
        //    string guestId = "Guest" + UnityEngine.Random.Range(1000, 9999).ToString();
        //    var defaultCharacters = new Dictionary<int, bool>();

        //    for (int i = 0; i < _characterSpecRepository.specs.Count; i++)
        //    {
        //        var characterData = _characterSpecRepository.specs[i];
        //        bool isLocked = characterData.id != 0;
        //        defaultCharacters.Add(i, isLocked);
        //    }

        //    CurrentPlayerData = new PlayerData
        //    {
        //        Nickname = guestId,
        //        Coins = 0,
        //        LastCharacter = 0,
        //        CharactersLocked = defaultCharacters
        //    };

        //    Nickname = guestId;
        //    Coins = 0;
        //    LastCharacter = 0;
        //    PhotonNetwork.NickName = guestId;
        //}
        
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
                Debug.Log(e);
                ShowConfirmWindow($"������ ������ �����߽��ϴ�. �ٽ� �õ����ּ���. {e}");
            }
        }

        public async Task UpdatePlayerCoinsAsync(int coins)
        {
            if (Coins + coins < 0)
            {
                return;
            }

            Coins += coins;
            await SaveCoinsAsync(Coins);
        }

        public async Task DefaultCharacterAsync()
        {
            try
            {
                var defaultCharacters = new Dictionary<int, bool>();

                for (int i = 0; i < _characterSpecRepository.specs.Count; i++)
                {
                    var characterData = _characterSpecRepository.specs[i];
                    bool isLocked = characterData.id != 0;
                    defaultCharacters.Add(i, isLocked);
                }

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
                Debug.Log(e);
                //�񵿱�� ���������� �����带 ��Ƽ�½�ŷ ������� ó��
                //await�� ȣ��Ǹ� ���� ������� �񵿱� �۾��� �Ϸ�� ������ ���ܵ��� �ʰ�, �ٸ� �۾��� ó���Ͽ�
                //�η��۷������� ����Ƽ���� ������ ����
                ShowConfirmWindow($"ĳ���� �����͸� �������� ���Ͽ����ϴ�. �ٽ� �õ����ּ���. {e}");
            }
        }
        public async Task SaveLastCharacterAsync(int characterId)
        {
            try
            {
                LastCharacter = characterId;
                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    { LAST_CHARACTER_KEY, LastCharacter },
                });
            }
            catch (Exception e)
            {
                Debug.Log(e);
                ShowConfirmWindow($"������ ������ �����߽��ϴ�. �ٽ� �õ����ּ���. {e}");
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
                Debug.Log(e);
                ShowConfirmWindow($"ĳ���� ������ �����Ͽ����ϴ�. �ٽ� �õ����ּ���. {e}");
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
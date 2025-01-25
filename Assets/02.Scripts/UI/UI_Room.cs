using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using GetyourCrown.Database;
using GetyourCrown.Network;
using GetyourCrown.UI.UI_Utilities;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace GetyourCrown.UI
{
    public class UI_Room : UI_Screen, IInRoomCallbacks
    {
        public static UI_Room Instance;

        [Resolve] RoomPlayerInfoSlot _roomPlayerInfoSlot;
        [Resolve] RectTransform _roomPlayerInfoPanel;
        [Resolve] Button _startGame;
        [Resolve] Button _gameReady;
        [Resolve] Button _leftRoom;
        [Resolve] Button _characterChange;
        [Resolve] TMP_Text _roomName;
        [Resolve] TMP_Text _coin;
        [Resolve] TMP_InputField _chatInputField;
        private Dictionary<int, GameObject> _slotCharacterPrefabs;
        Camera _characterCamera;
        public Dictionary<int, (Player player, RoomPlayerInfoSlot slot)> _roomPlayerInfoPairs;
        private CharacterSpec[] _characterSpecs;
        const int DEFAULT_CHARACTERSELECT = 0;
        bool isFirst = true;
        private Coroutine chatCoroutine;

        protected override void Awake()
        {
            base.Awake();

            _roomPlayerInfoPairs = new Dictionary<int, (Player player, RoomPlayerInfoSlot slot)>(16);
            _slotCharacterPrefabs = new Dictionary<int, GameObject>(16);
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        protected override void Start()
        {
            base.Start();
            DataManager.instance.OnCoinsChanged += UpdataCoins;
            _roomPlayerInfoSlot.gameObject.SetActive(false);
            _startGame.onClick.AddListener(() =>
            {
                bool allReady = _roomPlayerInfoPairs.Values
                    .Where(pair => !pair.slot.isMasterClient)
                    .All(pair => pair.slot.isReady);

                if (_roomPlayerInfoPairs.Count < 2)
                {
                    UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                    uI_ConfirmWindow.Show("인원이 적어요!");
                    return;
                }
                else if (allReady)
                {
                    SceneManager.LoadScene("GameScene-Workflow");
                }
                else
                {
                    UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                    uI_ConfirmWindow.Show("모든 플레이어가 준비되지 않았습니다.");
                }
            });

            _gameReady.onClick.AddListener(() =>
            {
                Player player = PhotonNetwork.LocalPlayer;
                bool isReady = _roomPlayerInfoPairs[player.ActorNumber].slot.isReady;

                player.SetCustomProperties(new Hashtable()
                {
                    { PlayerInRoomProperty.IS_READY, isReady == false},
                });
            });

            _characterChange.onClick.AddListener(() =>
            {
                Player player = PhotonNetwork.LocalPlayer;
                bool isCharacterSelection = _roomPlayerInfoPairs[player.ActorNumber].slot.isCharacterSelect;

                player.SetCustomProperties(new Hashtable()
                {
                    { PlayerInRoomProperty.SHOW_CHARACTER_SELECTION, isCharacterSelection == false},
                });

                UI_CharacterSelect uI_CharacterSelect = UI_Manager.instance.Resolve<UI_CharacterSelect>();
                uI_CharacterSelect.Show();
            });


            _leftRoom.onClick.AddListener(() =>
            {
                PhotonNetwork.LeaveRoom();

                if (DataManager.instance != null)
                {
                    DataManager.instance.OnCoinsChanged -= UpdataCoins;
                }
            });

            _chatInputField.onSubmit.AddListener(OnChatMessage);
        }

        public override void Show()
        {
            base.Show();

            if (DataManager.instance == null)
            {
                DataManager.instance.OnCoinsChanged += UpdataCoins;
            }

            UpdataCoins(DataManager.instance.Coins);

            foreach (int actorNumber in _roomPlayerInfoPairs.Keys.ToList())
            {
                Destroy(_roomPlayerInfoPairs[actorNumber].slot.gameObject);
                _roomPlayerInfoPairs.Remove(actorNumber);
            }

            _characterSpecs = Resources.LoadAll<CharacterSpec>("CharacterSpecs");

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                RoomPlayerInfoSlot slot = Instantiate(_roomPlayerInfoSlot, _roomPlayerInfoPanel);
                slot.gameObject.SetActive(true);
                slot.playerName = player.NickName;
                slot.actorNumber = player.ActorNumber;
                slot.isMasterClient = player.IsMasterClient;

                if (player.CustomProperties.TryGetValue(PlayerInRoomProperty.IS_READY, out bool isReady))
                {
                    slot.isReady = isReady;
                }
                else
                {
                    slot.isReady = false;
                }

                if (player.CustomProperties.TryGetValue(PlayerInRoomProperty.SHOW_CHARACTER_SELECTION, out bool showCharacterSelection))
                {
                    slot.isCharacterSelect = showCharacterSelection;
                }
                else
                {
                    slot.isCharacterSelect = false;
                }

                if (player.CustomProperties.TryGetValue(PlayerInRoomProperty.CHAT_STATE, out bool isChat))
                {
                    slot.isChat = isChat;
                }
                else
                {
                    slot.isChat = false;
                }

                if (player.CustomProperties.TryGetValue(PlayerInRoomProperty.CHAT_MESSAGE, out object chatMessage))
                {
                    string slotMessage = chatMessage as string;
                    slot.chatMessage = slotMessage;
                }
                else
                {
                    slot.chatMessage = string.Empty;
                }

                PlayerCharacterUpdate(slot, player);
                slot.gameObject.SetActive(true);
                _roomPlayerInfoPairs.Add(player.ActorNumber, (player, slot));
            }

            _roomName.text = PhotonNetwork.CurrentRoom.Name;
            TogglePlayerButtons(PhotonNetwork.LocalPlayer);
        }

        void TogglePlayerButtons(Player player)
        {
            if (player.IsMasterClient)
            {
                _startGame.gameObject.SetActive(true);
                _gameReady.gameObject.SetActive(false);
            }
            else
            {
                _startGame.gameObject.SetActive(false);
                _gameReady.gameObject.SetActive(true);
            }
        }

        void PlayerCharacterUpdate(RoomPlayerInfoSlot slot, Player player)
        {
            if (player.CustomProperties.TryGetValue(PlayerInRoomProperty.CHARACTER_ID, out int characterId))
            {
                CharacterSpec selectedCharacter = _characterSpecs.FirstOrDefault(spec => spec.id == characterId);

                if (selectedCharacter != null) // 캐릭터가 선택 되어있다면
                {
                    RenderTexture slotRenderTexture = new RenderTexture(512, 512, 32);
                    slot.playerCharacter = RenderCharacterToTexture(selectedCharacter.prefab, player.ActorNumber);
                }
                else // 캐릭터 선택을 하지 않았다면
                {
                    RenderTexture slotRenderTexture = new RenderTexture(512, 512, 32);
                    slot.playerCharacter = RenderCharacterToTexture(_characterSpecs[DEFAULT_CHARACTERSELECT].prefab, player.ActorNumber);
                }
            }
            else // 캐릭터 아이디가 없다면
            {
                RenderTexture slotRenderTexture = new RenderTexture(512, 512, 32);
                slot.playerCharacter = RenderCharacterToTexture(_characterSpecs[DEFAULT_CHARACTERSELECT].prefab, player.ActorNumber);
            }
        }

        public Texture RenderCharacterToTexture(GameObject characterPrefab, int slotIndex)
        {
            string cameraName = $"CharacterCamera_Slot{slotIndex}";

            GameObject lastCamera = GameObject.Find(cameraName);
            if (lastCamera != null)
            {
                Destroy(lastCamera);
            }

            if (_slotCharacterPrefabs.TryGetValue(slotIndex, out GameObject lastCharacter))
            {
                Destroy(lastCharacter);
                _slotCharacterPrefabs.Remove(slotIndex);
            }

            GameObject characterClone = Instantiate(characterPrefab);
            characterClone.transform.position = new Vector3(-20 * slotIndex, -1, 0);
            characterClone.transform.localScale = Vector3.one;
            characterClone.transform.localRotation = Quaternion.Euler(0, 180, 0);
            _slotCharacterPrefabs[slotIndex] = characterClone;

            _characterCamera = new GameObject(cameraName).AddComponent<Camera>();
            _characterCamera.transform.position = new Vector3(-20 * slotIndex, 0, -2.25f);
            _characterCamera.clearFlags = CameraClearFlags.SolidColor;
            RenderTexture renderTexture = new RenderTexture(512, 512, 32);

            _characterCamera.targetTexture = renderTexture;
            _characterCamera.Render();  

            return renderTexture;
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            KeyValuePair<int, (Player player, RoomPlayerInfoSlot slot)> masterClientPair = _roomPlayerInfoPairs.First(pair => pair.Value.slot.isMasterClient);
            masterClientPair.Value.slot.isMasterClient = false;
            _roomPlayerInfoPairs[newMasterClient.ActorNumber].slot.isMasterClient = true;
            _roomPlayerInfoPairs[newMasterClient.ActorNumber].slot.isReady = false;

            if (newMasterClient.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                TogglePlayerButtons(newMasterClient);
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            RoomPlayerInfoSlot slot = Instantiate(_roomPlayerInfoSlot, _roomPlayerInfoPanel);
            slot.gameObject.SetActive(true);
            slot.actorNumber = newPlayer.ActorNumber;
            slot.playerName = newPlayer.NickName;
            slot.isReady = false;
            slot.isCharacterSelect = false;
            slot.isChat = false;
            PlayerCharacterUpdate(slot, newPlayer);
            _roomPlayerInfoPairs.Add(newPlayer.ActorNumber, (newPlayer, slot));
            Debug.Log($"Player entered room {newPlayer.ActorNumber}");
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (_roomPlayerInfoPairs.TryGetValue(otherPlayer.ActorNumber, out (Player player, RoomPlayerInfoSlot slot) pair))
            {
                Destroy(pair.slot.gameObject);
                _roomPlayerInfoPairs.Remove(otherPlayer.ActorNumber);
            }
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (_roomPlayerInfoPairs.TryGetValue(targetPlayer.ActorNumber, out (Player player, RoomPlayerInfoSlot slot) pair))
            {
                if (changedProps.TryGetValue(PlayerInRoomProperty.IS_READY, out bool isReady))
                {
                    pair.slot.isReady = isReady;
                }

                //캐릭터 업데이트 
                if (changedProps.TryGetValue(PlayerInRoomProperty.CHARACTER_ID, out int characterId))  // 캐릭터 ID 동기화
                {
                    CharacterSpec selectedCharacter = _characterSpecs.FirstOrDefault(spec => spec.id == characterId);
                    // _characterSpecs 배열에서 선택된 캐릭터 ID와 일치하는 CharacterSpec을 찾음

                    if (selectedCharacter != null) // 선택된 캐릭터가 존재하는 경우
                    {
                        RenderTexture slotRenderTexture = new RenderTexture(512, 512, 32);
                        // 캐릭터 이미지를 설정
                        pair.slot.playerCharacter = RenderCharacterToTexture(selectedCharacter.prefab, targetPlayer.ActorNumber); // 선택된 캐릭터의 이미지를 해당 플레이어 슬롯에 표시
                    }
                }

                // 캐릭터 선택 중 UI 업데이트
                if (changedProps.TryGetValue(PlayerInRoomProperty.SHOW_CHARACTER_SELECTION, out bool isCharacterSelection))
                {
                    pair.slot.isCharacterSelect = isCharacterSelection;
                }

                if (changedProps.TryGetValue(PlayerInRoomProperty.CHAT_STATE, out bool isChatMessage))
                {
                    pair.slot.isChat = isChatMessage;
                }

                if (changedProps.TryGetValue(PlayerInRoomProperty.CHAT_MESSAGE, out object slotMessageObj))
                {
                    string slotMessage = slotMessageObj as string;
                    pair.slot.chatMessage = slotMessage;
                }
            }   
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
        }

        private void UpdataCoins(int coins)
        {
            _coin.text = coins.ToString();
        }

        private void OnChatMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            Player player = PhotonNetwork.LocalPlayer;
            bool isChat = _roomPlayerInfoPairs[player.ActorNumber].slot.isChat;
            string slotMessage = _roomPlayerInfoPairs[player.ActorNumber].slot.chatMessage;

            player.SetCustomProperties(new Hashtable()
            {
                { PlayerInRoomProperty.CHAT_STATE, isChat == false},
                { PlayerInRoomProperty.CHAT_MESSAGE, slotMessage = message }
            });

            _chatInputField.text = string.Empty;

            if (chatCoroutine != null)
            {
                StopCoroutine(chatCoroutine);
            }
            chatCoroutine = StartCoroutine(ResetChatMessage(player));
        }

        private IEnumerator ResetChatMessage(Player player)
        {
            float delay = 3f;
            yield return new WaitForSeconds(delay);
            bool isChat = _roomPlayerInfoPairs[player.ActorNumber].slot.isChat;
            string slotMessage = _roomPlayerInfoPairs[player.ActorNumber].slot.chatMessage;

            player.SetCustomProperties(new Hashtable()
            {
                { PlayerInRoomProperty.CHAT_STATE, isChat == false },
                { PlayerInRoomProperty.CHAT_MESSAGE, slotMessage = ""},
            });
        }
    }
}

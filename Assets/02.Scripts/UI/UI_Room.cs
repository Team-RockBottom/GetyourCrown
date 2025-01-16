using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using GetyourCrown.Network;
using GetyourCrown.UI.UI_Utilities;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_Room : UI_Screen, IInRoomCallbacks
    {
        [Resolve] RoomPlayerInfoSlot _roomPlayerInfoSlot;
        [Resolve] RectTransform _roomPlayerInfoPanel;
        [Resolve] Button _startGame;
        [Resolve] Button _gameReady;
        [Resolve] Button _leftRoom;
        [Resolve] Button _characterChange; //추가 하기
        Dictionary<int, (Player player, RoomPlayerInfoSlot slot)> _roomPlayerInfoPairs;

        protected override void Awake()
        {
            base.Awake();

            _roomPlayerInfoPairs = new Dictionary<int, (Player player, RoomPlayerInfoSlot slot)>(16);
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

            _roomPlayerInfoSlot.gameObject.SetActive(false);
            _startGame.onClick.AddListener(() =>
            {
                //SceneManager.LoadScene("GameScene");
                Debug.Log("GameScene");
            });

            _gameReady.onClick.AddListener(() =>
            {
                Player player = PhotonNetwork.LocalPlayer;
                bool isReady = _roomPlayerInfoPairs[player.ActorNumber].slot.isReady;

                player.SetCustomProperties(new Hashtable()
                {
                    { PlayerInRoomProperty.IS_READY, isReady == false}
                });
            });

            _leftRoom.onClick.AddListener(() =>
            {
                PhotonNetwork.LeaveRoom();
            });

            //_characterChange.onClick.AddListener(() =>
            //{
            //    UI_NickNameChange uI_NickNameChange = UI_Manager.instance.Resolve<UI_NickNameChange>();
            //    uI_NickNameChange.Show();
            //});
        }

        public override void Show()
        {
            base.Show();

            foreach (int actorNumber in _roomPlayerInfoPairs.Keys.ToList())
            {
                Destroy(_roomPlayerInfoPairs[actorNumber].slot.gameObject);
                _roomPlayerInfoPairs.Remove(actorNumber);
            }

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

                slot.gameObject.SetActive(true);
                _roomPlayerInfoPairs.Add(player.ActorNumber, (player, slot));
                Debug.Log(player.ActorNumber);
            }

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

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            KeyValuePair<int, (Player player, RoomPlayerInfoSlot slot)> masterClientPair = _roomPlayerInfoPairs.First(pair => pair.Value.slot.isMasterClient);
            masterClientPair.Value.slot.isMasterClient = false;
            _roomPlayerInfoPairs[newMasterClient.ActorNumber].slot.isMasterClient = true;

            if (newMasterClient.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                TogglePlayerButtons(newMasterClient);
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            RoomPlayerInfoSlot slot = Instantiate(_roomPlayerInfoSlot, _roomPlayerInfoPanel);
            slot.gameObject.SetActive(true);
            slot.actorNumber = newPlayer.ActorNumber;
            slot.isReady = false;
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
            }
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
        }

        public void PlayerNickName(Player player, string nickName)
        {
            Debug.Log(player.ActorNumber);
            _roomPlayerInfoPairs[player.ActorNumber].slot.playerName = nickName;

        }
    }
}

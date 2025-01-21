using GetyourCrown.Network;
using GetyourCrown.UI.UI_Utilities;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_Lobby : UI_Screen, ILobbyCallbacks, IMatchmakingCallbacks
    {
        [Resolve] RectTransform _roomListSlotContent;
        [Resolve] RoomListSlot _roomListSlot;
        [Resolve] Button _createRoom;
        [Resolve] Button _joinRoom;
        [Resolve] Button _exitLobby;
        [Resolve] Button _nickNameChange;
        [Resolve] TMP_Text _nickNameText;
        List<RoomListSlot> _roomListSlots = new List<RoomListSlot>(10);
        List<RoomInfo> _roomInfosCashed = new List<RoomInfo>(10);
        int _roomIdSelected = -1;

        protected override void Start()
        {
            base.Start();

            _roomListSlot.gameObject.SetActive(false);
            playerInputActions.UI.Click.performed += OnClick;
            _createRoom.onClick.AddListener(() =>
            {
                UI_CreateRoom createRoom = UI_Manager.instance.Resolve<UI_CreateRoom>();
                createRoom.Show();
            });

            _joinRoom.interactable = false;
            _joinRoom.onClick.AddListener(() =>
            {
                RoomInfo roomInfo = _roomInfosCashed[_roomIdSelected];

                if (!roomInfo.IsOpen)
                {
                    UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                    uI_ConfirmWindow.Show("방이 닫혀 있습니다.");
                    return;
                }
                if (roomInfo.PlayerCount >= roomInfo.MaxPlayers)
                {
                    UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                    uI_ConfirmWindow.Show("이 방은 더 이상 참여할 수 없습니다.");
                    return;
                }

                PhotonNetwork.JoinRoom(roomInfo.Name);
            });

            _nickNameChange.onClick.AddListener(() =>
            {
                UI_NickNameChange uI_NickNameChange = UI_Manager.instance.Resolve<UI_NickNameChange>();
                uI_NickNameChange.Show();
            });

            _exitLobby.onClick.AddListener(() =>
            {
                Hide();
                UI_MainMenu ui_MainMenu = UI_Manager.instance.Resolve<UI_MainMenu>();
                ui_MainMenu.Show();
            });
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void OnCreatedRoom()
        {
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            UI_ConfirmWindow confirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            confirmWindow.Show("방을 만드는 데 실패하였습니다.");
            Debug.Log(message);
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
        }

        public void OnJoinedLobby()
        {
            //UI_ConfirmWindow confirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            //confirmWindow.Show("로비에 접속하였습니다.");

            NickNameChange();
        }

        public void OnJoinedRoom()
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { PlayerInRoomProperty.IS_READY, false }
            });


            UI_Manager.instance.Resolve<UI_Room>().Show();
        }


        public void OnJoinRandomFailed(short returnCode, string message)
        {
            UI_ConfirmWindow confirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            confirmWindow.Show("방을 찾지 못했습니다.");
            Debug.Log(message);
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            UI_ConfirmWindow confirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            confirmWindow.Show("방에 입장하지 못했습니다.");
            Debug.Log(message);
        }

        public void OnLeftLobby()
        {
        }

        public void OnLeftRoom()
        {
            Show();
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            RefreshSlot(roomList);
        }

        void OnClick(InputAction.CallbackContext context)
        {
            if (TryGraphicRaycast(Mouse.current.position.ReadValue(), out RoomListSlot slot))
            {
                SelectRoom(slot.roomId);
            }
        }

        void SelectRoom(int roomId)
        {
            RoomInfo roomInfo = _roomInfosCashed[roomId];

            if (!roomInfo.IsOpen)
            {
                _joinRoom.interactable = false;
                return;
            }
            
            if (roomInfo.PlayerCount >= roomInfo.MaxPlayers)
            {
                _joinRoom.interactable = false;
                return;
            }

            _joinRoom.interactable = true;

            if (_roomIdSelected >= 0)
                _roomListSlots[_roomIdSelected].isSelected = false;

            _roomListSlots[roomId].isSelected = true;
            _roomIdSelected = roomId;
        }

        void RefreshSlot(List<RoomInfo> roomList)
        {
            RoomListSlot slotSeleted = _roomListSlots.Find(slot => slot.roomId == _roomIdSelected);
            string selectedRoomName = slotSeleted?.name;
            _joinRoom.interactable = false;
            _roomIdSelected = -1;

            for (int i = 0; i < _roomListSlots.Count; i++)
            {
                Destroy(_roomListSlots[i].gameObject);
            }

            _roomListSlots.Clear();
            _roomInfosCashed.Clear();

            for (int i = 0; i < roomList.Count; i++)
            {
                RoomListSlot slot = Instantiate(_roomListSlot, _roomListSlotContent);
                slot.gameObject.SetActive(true);

                slot.roomId = i;
                slot.roomName = roomList[i].Name;
                slot.roomPlayerCount = roomList[i].PlayerCount;
                slot.roomMaxPlayers = roomList[i].MaxPlayers;
                slot.gameObject.SetActive((roomList[i].RemovedFromList == false) && (roomList[i].PlayerCount > 0));
                _roomListSlots.Add(slot);
                _roomInfosCashed.Add(roomList[i]);

                if (roomList[i].Name.Equals(selectedRoomName))
                {
                    _roomIdSelected = i;
                    _joinRoom.interactable = true;
                }
            }
        }

        public void NickNameChange()
        {
            _nickNameText.text = PhotonNetwork.NickName;
        }
    }
}

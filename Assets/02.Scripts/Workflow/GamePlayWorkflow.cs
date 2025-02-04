using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;
using GetyourCrown.UI;
using Augment;
using GetyourCrown.UI.UI_Utilities; 
using UnityEngine.UI;
using System;
using GetyourCrown.CharacterContorller;
using GetyourCrown.Database;

namespace GetyourCrown.Network
{
    [RequireComponent(typeof(PhotonView))]
    public class GamePlayWorkflow : ComponentResolvingBehaviour
    {
        [SerializeField] GameObject _crown;
        [SerializeField] CharacterRepository _characterRepository;


        [Header ("GameTimer")]
        [SerializeField] int timeCountValue = 30;
        [SerializeField] Image _longTimerImage;
        [SerializeField] TMP_Text _longTimer;
        [SerializeField] Image _eventCountImage;
        [SerializeField] TMP_Text _eventCountText;
        float _gamePlayTimeCount = 180;

        int _timeCount = 0;
        WaitForSeconds _waitFor1Seconds = new WaitForSeconds(1);

        [Header("Augment System")]
        [SerializeField] Canvas _augmentCanvas;

        PhotonView _view;
        [SerializeField] ScoreCounter scoreCounter;

        UI_ConfirmWindow _uI_ConfirmWindow;
        private void Start()
        {
            _view = GetComponent<PhotonView>();
            _crown.SetActive(false);
            StartCoroutine(C_Workflow());
        }

        IEnumerator C_Workflow()
        {
            SpawnPlayerCharacterRandomly();

            yield return StartCoroutine(C_WaitUntilAllPlayerSelectAugment());

            yield return StartCoroutine(C_WaitUntilAllPlayerCharactersAreSpawned());

            if (PhotonNetwork.IsMasterClient)
            {
                yield return StartCoroutine(C_WaitUntilCountDown());
                yield return StartCoroutine(C_WaitUntilGamePlayTime());
            }
        }

        void SpawnPlayerCharacterRandomly()
        {
            Vector2 xz = UnityEngine.Random.insideUnitCircle * 5f;
            Vector3 randomPosition = new Vector3(-12 + xz.x, 0f,  -2 +xz.y);
            
            
            if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerInRoomProperty.CHARACTER_ID ,out int id))
            {
                CharacterSpec characterSpec = _characterRepository.Get(id);
                GameObject testPlayer = PhotonNetwork.Instantiate($"Character/{characterSpec.name}",
                                        randomPosition,
                                      Quaternion.identity);
            }
            else
            {
                GameObject testPlayer = PhotonNetwork.Instantiate("Character/TestPlayer",
                                          randomPosition,
                                          Quaternion.identity);

            }

        }

        IEnumerator C_WaitUntilAllPlayerCharactersAreSpawned()
        {
            while (true)
            {
                bool allReady = true;

                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (player.CustomProperties.TryGetValue(PlayerInGamePlayPropertyKey.IS_CHARACTER_SPAWNED, out bool isCharacterSpawned))
                    {
                        if (isCharacterSpawned == false)
                        {
                            allReady = false;
                            break;
                        }
                    }
                    else
                    {
                        allReady = false;
                        break;
                    }
                }

                if (allReady)
                    break;

                yield return _waitFor1Seconds;
            }
        }

        IEnumerator C_WaitUntilAllPlayerSelectAugment() 
        {
            UI_Augment uI_Augment = _augmentCanvas.GetComponent<UI_Augment>();
            uI_Augment.AugmentSlotRefresh();
            Cursor.lockState = CursorLockMode.Confined;

            int timeCount = timeCountValue;

            while (true)
            {
                bool selected = true;

                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (player.CustomProperties.TryGetValue(PlayerInGamePlayPropertyKey.IS_AUGMENT_SELECTED, out bool isAugmentSelected)) //증강 선택 여부 체크
                    {
                        if (isAugmentSelected == false)
                        {
                            selected = false;
                            break;
                        }
                    }
                    else
                    {
                        selected = false;
                        break;
                    }
                }

                _longTimer.text = timeCount.ToString();

                if (selected)
                {
                    _longTimer.text = "준비중";
                    break;
                }

                yield return _waitFor1Seconds;

                timeCount--;


                if (timeCount <= 0)
                {
                    _longTimer.text = "준비중";
                    break;
                }
            }

        }

        IEnumerator C_WaitUntilCountDown()
        {
            _timeCount = 4;

            while(true) 
            {
                if(_timeCount > 0)
                {
                    _timeCount--;
                    _view.RPC("ShowTimer",RpcTarget.All,_timeCount);
                }
                else if(_timeCount == 0)
                {
                    _view.RPC("GameStart", RpcTarget.All);
                    _timeCount--;
                }
                else
                {
                    _view.RPC("GameStartDisable", RpcTarget.All);
                    break;
                }

                yield return _waitFor1Seconds; 
            }
        }

        [PunRPC]
        void ShowTimer(int timer)
        {
            _eventCountText.text = timer.ToString();
        }

        [PunRPC]
        void GameStart()
        {
            _eventCountText.text = "Start!";
            _crown.transform.position = new Vector3(0, 10, 0);
            _crown.SetActive(true);
        }

        [PunRPC]
        void GameStartDisable()
        {
            _eventCountText.enabled = false;
            _eventCountImage.enabled = false;
        }

        IEnumerator C_WaitUntilGamePlayTime()
        {
            double gamePlayTimeCount = PhotonNetwork.Time;

            while (true)
            {
                double elesedTime = PhotonNetwork.Time - gamePlayTimeCount;
                int intTime = (int)_gamePlayTimeCount - (int)elesedTime;

                if (intTime == 0)
                {
                    _view.RPC(nameof(ConfirmWindowShow), RpcTarget.All, "게임 종료");
                    _view.RPC("ShowGameTimer", RpcTarget.All, intTime);
                }
                else if(intTime < 0)
                {
                    _view.RPC("GameEnd", RpcTarget.All);
                    break;
                }
                else
                {
                    _view.RPC("ShowGameTimer", RpcTarget.All, intTime);
                }

                yield return _waitFor1Seconds;
            }
        }

        [PunRPC]
        void ShowGameTimer(int timer)
        {
            string timerText = TimeSpan.FromSeconds(timer).ToString("m\\:ss");
            string[] tokens = timerText.Split(':');

            _longTimer.text = $"{tokens[0]} : {tokens[1]}";
        }

        [PunRPC]
        void ConfirmWindowShow(string message)
        {
            foreach (int num in ExampleCharacterController.controllers.Keys)
            {
                ExampleCharacterController.controllers[num].gameObject.layer = 0;
            }
            _uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            _uI_ConfirmWindow.Show(message);
            Cursor.lockState = CursorLockMode.Confined;
            _uI_ConfirmWindow.ConfirmInteractable = false;
            _uI_ConfirmWindow.onHide += () => 
            {
                scoreCounter.Show(); 
                _longTimerImage.gameObject.SetActive(false);
                _longTimer.gameObject.SetActive(false);
            };
        }

        [PunRPC]
        void GameEnd()
        {
            scoreCounter.ScroeTransferToLeaderBoard();
            _uI_ConfirmWindow.ConfirmInteractable = true;
        }
    }
}
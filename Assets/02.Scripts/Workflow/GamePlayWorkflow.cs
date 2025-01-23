using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;
using GetyourCrown.UI;
using GetyourCrown.UI.UI_Utilities;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

namespace Practices.PhotonPunClient
{
    [RequireComponent(typeof(PhotonView))]
    public class GamePlayWorkflow : ComponentResolvingBehaviour
    {
        [SerializeField] int timeCountValue = 30;
        [Resolve] TMP_Text _longTimer;
        [Resolve] Image _eventCountImage;
        [Resolve] TMP_Text _eventCountText;
        float _gamePlayTimeCount = 120;

        int _timeCount = 0;
        WaitForSeconds _waitFor1Seconds = new WaitForSeconds(1);
        PhotonView _view;

        private void Start()
        {
            _view = GetComponent<PhotonView>();
            StartCoroutine(C_Workflow());
        }

        IEnumerator C_Workflow()
        {
            SpawnPlayerCharacterRandomly();
            //yield return StartCoroutine(C_WaitUntilAllPlayerCharactersAreSpawned());
            // TODO -> 증강 보여주는 기능
            //yield return StartCoroutine(C_WaitUntilAllPlayerSelectAugment());

            if (PhotonNetwork.IsMasterClient)
            {
                yield return StartCoroutine(C_WaitUntilCountDown());
                yield return StartCoroutine(C_WaitUntilGamePlayTime());
        }
    }

        void SpawnPlayerCharacterRandomly()
        {
            Vector2 xz = UnityEngine.Random.insideUnitCircle * 5f;
            Vector3 randomPosition = new Vector3(xz.x, 0f, xz.y);
            GameObject testPlayer = PhotonNetwork.Instantiate("Character/TestPlayer",
                                      randomPosition,
                                      Quaternion.identity);
        }

        IEnumerator C_WaitUntilAllPlayerCharactersAreSpawned()
        {
            while (true)
            {
                bool allReady = true;

                foreach (Player player in PhotonNetwork.PlayerListOthers)
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

        IEnumerator C_WaitUntilAllPlayerSelectAugment() //증강선택 체크
        {
            int timeCount = timeCountValue;

            while (true)
            {
                bool selected = true; //루프 탈출 조건 변수


                foreach (Player player in PhotonNetwork.PlayerListOthers) //Room의 플레이어 순회
                {
                    if (player.CustomProperties.TryGetValue(PlayerInGamePlayPropertyKey.IS_AUGMENT_SELECTED, out bool isAugmentSelected)) //증강 선택 여부 체크
                    {
                        if (isAugmentSelected == false) //증강 선택하지 않은 경우
                        {
                            selected = false;
                            break; //foreach 탈출
                        }
                    }
                    else //다른 customproperty일 경우
                    {
                        selected = false;
                        break; //foreach 탈출
                    }
                }

                _longTimer.text = timeCount.ToString();

                if (selected) //증강이 선택된 경우
                    break;//루프 탈출

                yield return _waitFor1Seconds; //1초 딜레이

                timeCount--; //카운드 감소


                if (timeCount <= 0) //제한 시간 내에 선택하지 않은 경우
                {
                    //TODO -> 표시된 증강 3개중 랜덤으로 하나 선택하는 기능

                    break; //루프 탈출
                }
            }
        }

        IEnumerator C_WaitUntilCountDown()
        {
            _longTimer.text = "준비중";
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
                    _eventCountText.enabled = false;
                    _eventCountImage.enabled = false;
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
        }

        IEnumerator C_WaitUntilGamePlayTime()
        {
            double gamePlayTimeCount = PhotonNetwork.Time;

            while (true)
            {
                double elesedTime = PhotonNetwork.Time - gamePlayTimeCount;
                int intTime = (int)_gamePlayTimeCount - (int)elesedTime;

                if (intTime < 0)
                {
                    _view.RPC(nameof(ConfirmWindowShow), RpcTarget.All, "게임 종료");

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
            UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            uI_ConfirmWindow.Show(message);
            uI_ConfirmWindow.onHide += () => { SceneManager.LoadScene(""); };
        }
    }
}
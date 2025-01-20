using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using ExitGames.Client.Photon.StructWrapping;

namespace Practices.PhotonPunClient
{
    public class GamePlayWorkflow : MonoBehaviour
    {
        [SerializeField] int timeCountValue = 30;
        WaitForSeconds _waitFor1Seconds = new WaitForSeconds(1);
        private void Start()
        {
            StartCoroutine(C_Workflow());
        }

        IEnumerator C_Workflow()
        {
            SpawnPlayerCharacterRandomly();
            yield return StartCoroutine(C_WaitUntilAllPlayerCharactersAreSpawned());
            // TODO -> 증강 보여주는 기능
            yield return StartCoroutine(C_WaitUntilAllPlayerSelectAugment());
        }

        void SpawnPlayerCharacterRandomly()
        {
            Vector2 xz = Random.insideUnitCircle * 5f;
            Vector3 randomPosition = new Vector3(xz.x, 0f, xz.y);
            GameObject testPlayer = PhotonNetwork.Instantiate("PhotonNetworkObjects/TestPlayer",
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
    }
}
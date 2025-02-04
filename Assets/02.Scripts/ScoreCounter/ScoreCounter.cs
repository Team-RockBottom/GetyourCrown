using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using GetyourCrown.UI;
using GetyourCrown.UI.UI_Utilities;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Data;
using GetyourCrown.Database;

namespace GetyourCrown.Network
{ 
public class ScoreCounter : UI_Screen, IOnEventCallback
    {
        public struct TotalScore
        {
            public int id;
            public float crownEquipTime;
            public int suceedingCount;
            public int kickCrownCount;
            public string nickName;
            public int totalscore;
        }
        private List<TotalScore> totalLeaderBoard = new List<TotalScore>();

        private double _stackCount = 0;
        private double _startTime = 0;
        private int _suceedCount = 0;
        private int _kickCount = 0;

        private bool _augmentOnWork = false;
        public float _augmentScoreCount = 0;
        private const int GET_COINS = 30;

        [Resolve] RectTransform _leaderBoardSlotGrid;
        [Resolve] RectTransform _leaderBoard;
        [Resolve] LeaderBoardSlot _leaderBoardSlot;
        [Resolve] Button _roomBackButton;

        protected override void Start()
        {
            base.Start();

            _leaderBoardSlot.gameObject.SetActive(false);
            _leaderBoard.gameObject.SetActive(false);

            _roomBackButton.onClick.AddListener(() =>
            {
                SoundManager.instance.StopBGM();
                PhotonNetwork.LoadLevel("MainMenuScene");
            });

            _roomBackButton.gameObject.SetActive(false);
        }

        protected virtual void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }
    

        protected virtual void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    
    
        private void ScoreCountUp(double timeAdd)
        {
            _stackCount += timeAdd;
        }


        public async override void Show()
        {
            base.Show();
            _leaderBoard.gameObject.SetActive(true);

            List<TotalScore> totalScores = totalLeaderBoard.OrderByDescending(x => x.totalscore).ToList();
            

            for(int i = 0; i < totalLeaderBoard.Count; i++)
            {
                LeaderBoardSlot slot = Instantiate(_leaderBoardSlot, _leaderBoardSlotGrid);
                slot.gameObject.SetActive(true);
                slot.Rank = i+1;;

                switch (slot.Rank)
                {
                    case 1:
                        await DataManager.instance.UpdatePlayerCoinsAsync(GET_COINS);
                        break;
                    case 2:
                        await DataManager.instance.UpdatePlayerCoinsAsync(GET_COINS / 2);
                        break; 
                    case 3:
                        await DataManager.instance.UpdatePlayerCoinsAsync(GET_COINS / 3);
                        break;
                    case 4:
                        break;
                }

                slot.CrownEquipScore = totalScores[i].crownEquipTime;
                slot.NickName = totalScores[i].nickName;
                slot.SuccedingScore = totalScores[i].suceedingCount;
                slot.KickScore = totalScores[i].kickCrownCount;
                slot.gameObject.SetActive(true);
            }

            _roomBackButton.gameObject.SetActive(true);
        }

        public void CountUpStart()
        {
            _startTime = PhotonNetwork.Time;
        }
    

        public void CountUpEnd()
        {
            double timeAdd = PhotonNetwork.Time - _startTime;
            if (_augmentOnWork)
            {
                ScoreCountUp(timeAdd-_augmentScoreCount);
                ScoreCountUp(_augmentScoreCount * 2);
            }
            else
            { 
                ScoreCountUp(timeAdd);
            }
            Debug.Log(timeAdd);
            _startTime = 0;
        }

        public void KickCountUp()
        {
            _kickCount += 1;
        }

        public void SuceedCountUp()
        {
            _suceedCount += 1;
        }

        private void CountReset()
        {
            _kickCount = 0;
            _suceedCount = 0;
            _stackCount = 0;
        }

        public void ScroeTransferToLeaderBoard()
        {
            Debug.Log("LB Transfer Call");
            TotalScore score = new TotalScore();
            score.id = PhotonNetwork.LocalPlayer.ActorNumber;
            float roundStackCount = Mathf.Round((float)_stackCount);
            score.crownEquipTime = roundStackCount;
            score.nickName = PhotonNetwork.NickName;
            score.kickCrownCount = _kickCount;
            score.suceedingCount = _suceedCount;
            score.totalscore = (int)(roundStackCount / 10) + _kickCount + _suceedCount;

            RaiseEventOptions raiseEventOption = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
            };
            object[] content = new object[] {score.id, score.crownEquipTime, score.suceedingCount, score.kickCrownCount, score.nickName, score.totalscore};
            PhotonNetwork.RaiseEvent(PhotonEventCode.GAMESTOP, content, raiseEventOption, SendOptions.SendReliable);
            
            CountReset();
        }


        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            if (eventCode == PhotonEventCode.GAMESTOP)
            {
                Debug.Log("Add Call");
                object[] data = (object[])photonEvent.CustomData;
                TotalScore score = new TotalScore();
                score.id = (int)data[0];
                score.crownEquipTime = (float)data[1];
                score.suceedingCount = (int)data[2];
                score.kickCrownCount = (int)data[3];
                score.nickName = (string)data[4];
                score.totalscore = (int)data[5];
                totalLeaderBoard.Add(score);
            }
         }
    }
}
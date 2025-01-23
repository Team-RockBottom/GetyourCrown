using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace GetyourCrown.Network
{ 
public class ScoreCounter : MonoBehaviour, IOnEventCallback
    {
        public struct TotalScore
        {
            public int id;
            public float crownEquipTime;
            public int suceedingCount;
            public int kickCrownCount;
        }
        private Dictionary<int, TotalScore> totalLeaderBoard  = new Dictionary<int, TotalScore>();

        private double _stackCount = 0;
        private double _startTime = 0;
    
        private bool _augmentOnWork = false;
        public float _augmentScoreCount = 0;

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

        private void Update()
        {
            Debug.Log(_stackCount);
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
    

        public void ScroeTransferToLeaderBoard()
        {
            Debug.Log("LB Transfer Call");
            TotalScore score = new TotalScore();
            score.id = PhotonNetwork.LocalPlayer.ActorNumber;
            score.crownEquipTime = (float)_stackCount;
            RaiseEventOptions raiseEventOption = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
            };
            object[] content = new object[] {score.id, score.crownEquipTime, score.suceedingCount, score.kickCrownCount};
            PhotonNetwork.RaiseEvent(PhotonEventCode.GAMESTOP, content, raiseEventOption, SendOptions.SendReliable);
            _stackCount = 0;
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
                totalLeaderBoard.Add(score.id, score);
            }
         }
    }
}
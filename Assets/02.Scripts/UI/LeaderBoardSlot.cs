using UnityEngine;
using GetyourCrown.UI.UI_Utilities;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class LeaderBoardSlot : ComponentResolvingBehaviour
{
    public int Rank
    {
        get => _rankValue;
        set
        {
            _rankValue = value;
            if(value == 1)
            {
                _goldenMedal.gameObject.SetActive(true);
            }

            _rank.text = value.ToString();
        }
    }

    public string NickName
    {
        get => _nickNameValue;
        set
        {
            if(value == PhotonNetwork.NickName)
            {
                _leaderBoardSlot.color = Color.yellow;
            }

            _nickNameValue = value;
            _nickName.text = value;
        }
    }

    public float CrownEquipScore
    {
        get => _crownEquipScoreValue;
        set
        {
            _crownEquipScoreValue = value;
            _crownEquipScore.text = value.ToString();
        }
    }

    public int SuccedingScore
    {
        get => _suceedingScoreValue;
        set
        {
            _suceedingScoreValue = value;
            _suceedingScroe.text = value.ToString();
        }
    }

    public int KickScore
    {
        get => _kickScoreValue;
        set
        {
            _kickScoreValue = value;
            _kickScore.text = value.ToString();
        }
    }

    public int TotalScore
    {
        get => _totalScoreValue = _suceedingScoreValue + _kickScoreValue + (int)_crownEquipScoreValue;
        set
        {
            _totalScoreValue = value;
            _totalScore.text = value.ToString();
        }
    }


    int _rankValue;
    string _nickNameValue;
    float _crownEquipScoreValue;
    int _suceedingScoreValue;
    int _kickScoreValue;
    int _totalScoreValue;
    [Resolve] Image _goldenMedal;
    [Resolve] Image _leaderBoardSlot;
    [Resolve] TMP_Text _rank;
    [Resolve] TMP_Text _nickName;
    [Resolve] TMP_Text _crownEquipScore;
    [Resolve] TMP_Text _suceedingScroe;
    [Resolve] TMP_Text _kickScore;
    [Resolve] TMP_Text _totalScore;
}

using UnityEngine;
using GetyourCrown.UI.UI_Utilities;
using TMPro;
using UnityEngine.UI;

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


    int _rankValue;
    string _nickNameValue;
    float _crownEquipScoreValue;
    [Resolve] Image _goldenMedal;
    [Resolve] TMP_Text _rank;
    [Resolve] TMP_Text _nickName;
    [Resolve] TMP_Text _crownEquipScore;
    [Resolve] TMP_Text _suceedingScroe;
    [Resolve] TMP_Text _kickScore;
    [Resolve] TMP_Text _totalScore;
}

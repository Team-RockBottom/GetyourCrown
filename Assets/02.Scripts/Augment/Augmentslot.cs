using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Augmentslot : MonoBehaviour
{
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _descriptionText;
    [SerializeField] Image _iconImage;
    private int _idValue;
    private string _descriptionValue;
    private string _nameValue;

    public Sprite iconimage
    {
        get => _iconImage.sprite;
        set => _iconImage.sprite = value;
    }

    public int id
    {
        get => _idValue;
        set => _idValue = value;
    }

    public string descriptionValue
    {
        get => _descriptionValue;
        set => _descriptionText.text = value;
    }

    public string nameValue
    {
        get => _nameValue;
        set => _nameText.text = value;
    }

}

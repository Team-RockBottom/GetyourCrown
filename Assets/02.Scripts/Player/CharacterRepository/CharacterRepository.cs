using Augment;
using GetyourCrown.Database;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRepository : MonoBehaviour
{
    [SerializeField] List<CharacterSpec> characterSpecs = new List<CharacterSpec>();

    public IDictionary<int, CharacterSpec> IcharacterDic => _characterDic;
    public Dictionary<int, CharacterSpec> _characterDic = new Dictionary<int, CharacterSpec>();

    private void Awake()
    {
        foreach (CharacterSpec spec in characterSpecs)
        {
            _characterDic.Add(spec.id, spec);
        }
    }

    public CharacterSpec Get(int id)
    {
        return _characterDic[id];
    }

}

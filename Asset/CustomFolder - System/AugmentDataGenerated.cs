using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AugmentData", menuName = "Scriptable Objects/Augment Data")]
public class AugmentData : ScriptableObject
{
    [System.Serializable]
    public class Attribute
    {
        public int Id
        public string Name
        public string Description
        public string Icon
        public float speed
        public float Increasedelay
        public float MaxSpeed
        public float IncreaseValue
        public float CoolDown
    }

    public List<Attribute> list = new List<Attribute>();
}

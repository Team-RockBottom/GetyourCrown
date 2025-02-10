using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AugmentData : ScriptableObject
{
    [System.Serializable]
    public class Attribute
    {
        public int id;
        public string name;
        public string description;
        public string iconPath;
        public float speed;
        public float increaseDelay;
        public float maxSpeed;
        public float increaseValue;
        public float coolDown;
    }

    public List<Attribute> list = new List<Attribute>();
}
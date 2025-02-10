using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerLevelData : ScriptableObject
{
	[System.Serializable]
	public class Attribute
	{
		public int level;
		public int maxHP;
		public float baseAttack;
		public int reqExp;
		public float moveSpeed;
		public float turnSpeed;
		public float attackRange;
	}

	public List<Attribute> list = new List<Attribute>();
}

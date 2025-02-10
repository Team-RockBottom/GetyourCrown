using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterLevelData : ScriptableObject
{
	public List<Race> infos = new List<Race>();

	[System.Serializable]
	public class Race
	{
		public string name;
		//정보 리스트
		public List<Attribute> list = new List<Attribute>();
	}

	//정보 인자들
	//에디터에 표시되게 설정
	[System.Serializable]
	public class Attribute
	{
		//레벨
		public int level;
		//체력
		public int maxHP;
		//공격력
		public int attack;
		//방어력
		public int defence;
		//얻는 경험치
		public int gainExp;
		//걷는 속도
		public float walkSpeed;
		//뛰는 속도
		public float runSpeed;
		//회전 속도
		public int turnSpeed;
		//공격 범위
		public float attackRange;
		//얻는 금화
		public int gainGold;
	}
}
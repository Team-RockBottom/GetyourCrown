using UnityEngine;
using System.Collections;

public class AugmentDataManager : MonoBehaviour
{
	static AugmentDataManager instance;
	
	public PlayerLevelData playerLevelData;
	public MonsterLevelData monsterLevelData;
	
	public static AugmentDataManager Instance
	{
		get
		{
			return instance;
		}
	}

	void Awake()
	{
		if ( instance == null )
		{
            Destroy(gameObject);	
			instance = this;
			//자료 로딩
			playerLevelData = Resources.Load ("Data/PlayerLevelData") as PlayerLevelData;
			monsterLevelData = Resources.Load ("Data/MonsterLevelData") as MonsterLevelData;
		}
		else
		{
			Destroy(gameObject);
		}
	}
	
	void Update()
	{
		if ( Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit ();
		}
	}
}

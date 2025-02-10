using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

/// <summary>
/// 유니티에서 지원하는 AssetPostprocessor를
/// 이용해서 엑셀 파일을 ScriptableObject로 변경하는
/// 스크립트
/// </summary>
public class ImportRPGExcel : AssetPostprocessor
{
    static readonly string filePath = "Assets/Editor/Data/RPGData.xlsx";
    static readonly string playerExportPath = "Assets/Resources/Data/PlayerLevelData.asset";
	static readonly string enemyExportPath = "Assets/Resources/Data/MonsterLevelData.asset";

	[MenuItem("DataImport/ExcelImport #&g")]
    static void ExcelImport()
    {
        Debug.Log("Excel data covert start.");

        MakePlayerData();
        MakeEnemyData();

        Debug.Log("Excel data covert complete.");
    }

    /// <summary>
    /// 에셋이 유니티 엔진에 추가되면 실행되는 엔진 함수
    /// </summary>
    /// <param name="importedAssets"></param>
    /// <param name="deletedAssets"></param>
    /// <param name="movedAssets"></param>
    /// <param name="movedFromAssetPaths"></param>
    static void OnPostprocessAllAssets(
        string[] importedAssets, string[] deletedAssets,
        string[] movedAssets, string[] movedFromAssetPaths)
    {

        //임포트 된 모픈 파일을 검색함
        foreach (string s in importedAssets)
        {
            //우리가 원하는 파일 일때만 수행
            if (s == filePath)
            {
                Debug.Log("Excel data covert start.");

                MakePlayerData();
                MakeEnemyData();

                Debug.Log("Excel data covert complete.");
            }
        }
    }

    /// <summary>
    /// 주인공 정보를 ScriptableObject만듬
    /// </summary>
    static void MakePlayerData()
    {
        //SciprtableObject를 생성
        PlayerLevelData data = ScriptableObject.CreateInstance<PlayerLevelData>();
        //ScriptableObject를 파일로 만듬
        AssetDatabase.CreateAsset((ScriptableObject)data, playerExportPath);
        //수정 불가능하게 설정(에디터에서 수정 하게 하려면 주석처리)
        data.hideFlags = HideFlags.NotEditable;

        //자료를 삭제(초기화)
        data.list.Clear();

        //엑셀 파일을 Open
        using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            //Open된 엑셀 파일 메모리에 생성
            IWorkbook book = new XSSFWorkbook(stream);

            //두번째(주인공 정보) Sheet를 열기
            ISheet sheet = book.GetSheetAt(1);
            //3번쨰 줄(row)부터 읽기
            for (int i = 2; i <= sheet.LastRowNum; i++)
            {
                //줄(row)읽기
                IRow row = sheet.GetRow(i);
                //시리얼라이즈를 위한 임시 객체 생성
                PlayerLevelData.Attribute a = new PlayerLevelData.Attribute();
                //레벨 
                a.level = (int)row.GetCell(0).NumericCellValue;
                //최대 체력
                a.maxHP = (int)row.GetCell(1).NumericCellValue;
                //기본 공격력
                a.baseAttack = (int)row.GetCell(2).NumericCellValue;
                //필요 경험치
                a.reqExp = (int)row.GetCell(3).NumericCellValue;
                //이동 속도
                a.moveSpeed = (int)row.GetCell(4).NumericCellValue;
                //회전 속도
                a.turnSpeed = (int)row.GetCell(5).NumericCellValue;
                //공격 범위
                a.attackRange = (float)row.GetCell(6).NumericCellValue;
                //리스트에 추가하기
                data.list.Add(a);
            }

            stream.Close();
        }

        //위에서 생성된 SciprtableObject의 파일을 찾음
        ScriptableObject obj =
            AssetDatabase.LoadAssetAtPath(playerExportPath, typeof(ScriptableObject)) as ScriptableObject;
        //디스크에 쓰기
        EditorUtility.SetDirty(obj);
    }


	/// <summary>
	/// 슬라임 정보를 ScriptableObject만듬
	/// </summary>
	static void MakeEnemyData()
	{
		//SciprtableObject를 생성
		MonsterLevelData data = ScriptableObject.CreateInstance<MonsterLevelData>();
		//ScriptableObject를 파일로 만듬
		AssetDatabase.CreateAsset((ScriptableObject)data, enemyExportPath);
		//수정 불가능하게 설정(에디터에서 수정 하게 하려면 주석처리)
		data.hideFlags = HideFlags.NotEditable;
		
		//자료를 삭제(초기화)
		data.infos.Clear();
		
		//엑셀 파일을 Open
		using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
		{
			//Open된 엑셀 파일 메모리에 생성
            IWorkbook book = new XSSFWorkbook(stream);


			for(int j = 2; j < 5; j++)
			{
				ISheet sheet = book.GetSheetAt(j);
				MonsterLevelData.Race info = new MonsterLevelData.Race();
				//몬스터 이름 가져오기
                info.name = sheet.SheetName.ToString();

				//4번쨰 줄(row)부터 읽기
				for (int i = 2; i <= sheet.LastRowNum; i++)
				{
					//줄(row)읽기
					IRow row = sheet.GetRow(i);
					//시리얼라이즈를 위한 임시 객체 생성
					MonsterLevelData.Attribute a = new MonsterLevelData.Attribute();
					//레벨 
					a.level = (int)row.GetCell(0).NumericCellValue;
					//최대 체력
					a.maxHP = (int)row.GetCell(1).NumericCellValue;
					//기본 공격력
					a.attack = (int)row.GetCell(2).NumericCellValue;
					//방어력
					a.defence = (int)row.GetCell(3).NumericCellValue;
					//얻는 경험치
					a.gainExp = (int)row.GetCell(4).NumericCellValue;
					a.walkSpeed = (float)row.GetCell(5).NumericCellValue;
					a.runSpeed = (float)row.GetCell(6).NumericCellValue;
					//회전 속도
					a.turnSpeed = (int)row.GetCell(7).NumericCellValue;
					//공격 범위
					a.attackRange = (float)row.GetCell(8).NumericCellValue;
					//얻는 금화
					a.gainGold = (int)row.GetCell(9).NumericCellValue;
					//리스트에 추가하기
					info.list.Add (a);
				}

				data.infos.Add (info);
			}

			stream.Close();
		}
		//위에서 생성된 SciprtableObject의 파일을 찾음
		ScriptableObject obj =
			AssetDatabase.LoadAssetAtPath(enemyExportPath, typeof(ScriptableObject)) as ScriptableObject;
		//디스크에 쓰기
		EditorUtility.SetDirty(obj);
	}
}

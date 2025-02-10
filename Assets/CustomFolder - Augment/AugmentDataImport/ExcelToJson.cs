using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;

public class ExcelToJson : MonoBehaviour
{
    // 기획 데이터가 있는 엑셀 데이터 경로.
    private static string filePath = "Assets/Editor/Data/RPGData.xlsx";
    // Player json 데이터 경로.
    private static string playerDataPath = "Assets/Resources/Data/PlayerLevelData.json";
    // Enemy json 데이터 경로.
    //private static string enemyDataPath = "Assets/Resources/Data/EnemyLevelData.json";

    // Json 데이터 생성.
    [MenuItem("Excel To Json/Create Data")]
    static void CreateData()
    {
        CreatePlayerData();
    }

    // 플레이어 json 데이터 생성 ( json 파일 생성 )
    static void CreatePlayerData()
    {
        List<PlayerLevelData.Attribute> list = new List<PlayerLevelData.Attribute>();

        using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            IWorkbook book = new XSSFWorkbook(stream);
            ISheet sheet = book.GetSheetAt(1);

            for (int ix = 2; ix < sheet.LastRowNum + 1; ++ix)
            {
				//줄(row)읽기
				IRow row = sheet.GetRow(ix);
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
				list.Add(a);
            }

            stream.Close();

            // json 데이터를 파일에 쓰기.
            string levelJsonData = SimpleJson.SimpleJson.SerializeObject(list);
            File.WriteAllText(playerDataPath, levelJsonData, System.Text.Encoding.UTF8);
        }
    }
}
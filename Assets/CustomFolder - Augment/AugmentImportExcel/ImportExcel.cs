using UnityEngine;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using UnityEditor;
using System.IO;
using Augment;
public class ImportExcel : AssetPostprocessor
{
    static readonly string filePath = "Assets/CustomFolder - Augment/AugmentData.xlsx";
    static readonly string augmentExportPath = "Assets/Resources/Data/AugmentData.asset";

    [MenuItem("DataImport/Import Augment Data")]
    public static void ExcelImport()
    {
        Debug.Log("Excel data covert start.");

        MakeAugmentData();
 
        Debug.Log("Excel data covert complete.");
    }

    /// <summary>
    /// 에셋이 유니티 엔진에 추가되면 실행되는 엔진 함수
    /// </summary>
    static void OnPostprocessAllAssets(
        string[] importedAssets, string[] deletedAssets,
        string[] movedAssets, string[] movedFromAssetPaths)
    {

        //임포트 된 모든 파일을 검색함
        foreach (string s in importedAssets)
        {
            //우리가 원하는 파일 일때만 수행
            if (s == filePath)
            {
                Debug.Log("Excel data covert start.");

                MakeAugmentData();
                Debug.Log("Excel data covert complete.");
            }
        }
    }

    static void MakeAugmentData()
    {
        AugmentData data = ScriptableObject.CreateInstance<AugmentData>();
        AssetDatabase.CreateAsset((ScriptableObject)data, augmentExportPath);

        //data.list.Clear();
        
        using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {

            IWorkbook book = new XSSFWorkbook(stream);

            ISheet sheet = book.GetSheetAt(1);

            for (int i = 2; i < sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                
                AugmentData.Attribute augment =  new AugmentData.Attribute();
                
                augment.id = (int)row.GetCell(0).NumericCellValue;
                augment.name = (string)row.GetCell(1).StringCellValue;
                augment.description = (string)row.GetCell(2).StringCellValue;
                augment.iconPath = (string)row.GetCell(3).StringCellValue;
                augment.speed = (float)row.GetCell(4).NumericCellValue;
                augment.increaseDelay = (float)row.GetCell(5).NumericCellValue;
                augment.maxSpeed = (float)row.GetCell(6).NumericCellValue;
                augment.increaseValue = (float)row.GetCell(7).NumericCellValue;
                augment.coolDown = (float)row.GetCell(8).NumericCellValue;

                data.list.Add(augment);
            }

            stream.Close();
        }
        ScriptableObject obj = AssetDatabase.LoadAssetAtPath(augmentExportPath, typeof(ScriptableObject)) as ScriptableObject;
        EditorUtility.SetDirty(obj);
    }

 

   
}

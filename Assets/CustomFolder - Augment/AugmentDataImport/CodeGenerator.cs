using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Text;
using UnityEngine;

public class CodeGenerator : MonoBehaviour
{
    static readonly string filePath = "Assets/CustomFolder - Augment/AugmentData.xlsx";

    private void Start()
    {
        GenerateAugmentDataScript("Assets/CustomFolder - Augment");
        GenerateImportExcelScript("Assets/CustomFolder - Augment/AugmentimportExcel");
    }



    /// <summary>
    /// AugmentData 스크립트 코드를 생성하여 지정된 디렉토리에 .cs 파일로 저장합니다.
    /// </summary>
    /// <param name="outputDirectory">생성된 .cs 파일을 저장할 디렉터리</param>
    public static void GenerateAugmentDataScript(string outputDirectory)
    {
        // C# 스크립트 코드 문자열을 생성
        var codeBuilder = new StringBuilder();
        codeBuilder.AppendLine("using UnityEngine;");
        codeBuilder.AppendLine("using System.Collections;");
        codeBuilder.AppendLine("using System.Collections.Generic;");
        codeBuilder.AppendLine("");
        codeBuilder.AppendLine("[CreateAssetMenu(fileName = \"AugmentData\", menuName = \"Scriptable Objects/Augment Data\")]");
        codeBuilder.AppendLine("public class AugmentDataGenerated : ScriptableObject");
        codeBuilder.AppendLine("{");
        codeBuilder.AppendLine("    [System.Serializable]");
        codeBuilder.AppendLine("    public class Attribute");
        codeBuilder.AppendLine("    {");

        ReadExcelDataForAugmentDataAttribute(codeBuilder);

        codeBuilder.AppendLine("    }");
        codeBuilder.AppendLine("");
        codeBuilder.AppendLine("    public List<Attribute> list = new List<Attribute>();");
        codeBuilder.AppendLine("}");
        // 출력 디렉터리가 없으면 생성
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // 생성된 스크립트를 .cs 파일로 저장
        string outputFilePath = Path.Combine(outputDirectory, "AugmentDataGenerated.cs");
        File.WriteAllText(outputFilePath, codeBuilder.ToString());

        Debug.Log($"스크립트 {outputFilePath}를 생성 완료하였습니다.");
    }

    public static void GenerateImportExcelScript(string outputDirectory)
    {
        // C# 스크립트 코드 문자열을 생성
        var codeBuilder = new StringBuilder();

        codeBuilder.Append("using UnityEngine;\r\n" +
                            "using NPOI.HSSF.UserModel;\r\n" +
                            "using NPOI.XSSF.UserModel;\r\n" +
                            "using NPOI.SS.UserModel;\r\n" +
                            "using UnityEditor;\r\n" +
                            "using System.IO;\r\n" +
                            "using Augment;\r\n" +
                            "public class ImportExcelGenerated : AssetPostprocessor\r\n" +
                            "{\r\n" +
                            "    static readonly string filePath = \"Assets/CustomFolder - Augment/AugmentData.xlsx\";\r\n" +
                            "    static readonly string augmentExportPath = \"Assets/Resources/Data/AugmentData.asset\";\r\n" +
                            "\r\n" +
                            "    [MenuItem(\"DataImport/Import Augment Data\")]\r\n" +
                            "    public static void ExcelImport()\r\n" +
                            "    {\r\n" +
                            "        Debug.Log(\"Excel data covert start.\");\r\n" +
                            "\r\n" +
                            "        MakeAugmentData();\r\n" +
                            " \r\n" +
                            "        Debug.Log(\"Excel data covert complete.\");\r\n" +
                            "    }\r\n" +
                            "\r\n" +
                            "    /// <summary>\r\n" +
                            "    /// 에셋이 유니티 엔진에 추가되면 실행되는 엔진 함수\r\n" +
                            "    /// </summary>\r\n" +
                            "    static void OnPostprocessAllAssets(\r\n" +
                            "        string[] importedAssets, string[] deletedAssets,\r\n" +
                            "        string[] movedAssets, string[] movedFromAssetPaths)\r\n" +
                            "    {\r\n" +
                            "\r\n" +
                            "        //임포트 된 모든 파일을 검색함\r\n" +
                            "        foreach (string s in importedAssets)\r\n" +
                            "        {\r\n" +
                            "            //우리가 원하는 파일 일때만 수행\r\n" +
                            "            if (s == filePath)\r\n" +
                            "            {\r\n" +
                            "                Debug.Log(\"Excel data covert start.\");\r\n" +
                            "\r\n" +
                            "                MakeAugmentData();\r\n" +
                            "                Debug.Log(\"Excel data covert complete.\");\r\n" +
                            "            }\r\n" +
                            "        }\r\n" +
                            "    }\r\n" +
                            "\r\n" +
                            "    static void MakeAugmentData()\r\n" +
                            "    {\r\n" +
                            "        AugmentDataGenerated data = ScriptableObject.CreateInstance<AugmentDataGenerated>();\r\n" +
                            "        AssetDatabase.CreateAsset((ScriptableObject)data, augmentExportPath);\r\n" +
                            "\r\n" +
                            "        data.list.Clear();\r\n" +
                            "        \r\n" +
                            "        using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))\r\n" +
                            "        {\r\n" +
                            "\r\n" +
                            "            IWorkbook book = new XSSFWorkbook(stream);\r\n" +
                            "\r\n" +
                            "            ISheet sheet = book.GetSheetAt(0);\r\n" +
                            "\r\n            for (int i = 1; i <= sheet.LastRowNum; i++)\r\n" +
                            "            {\r\n" +
                            "                IRow row = sheet.GetRow(i);\r\n" +
                            "                \r\n" +
                            "                AugmentDataGenerated.Attribute augment =  new AugmentDataGenerated.Attribute();\r\n ");

        ReadExcelDataToImportExcel(codeBuilder);

        codeBuilder.AppendLine("\r\n" +
                                "                data.list.Add(augment);\r\n" +
                                "            }\r\n" +
                                "\r\n" +
                                "            stream.Close();\r\n" +
                                "        }\r\n" +
                                "        ScriptableObject obj = AssetDatabase.LoadAssetAtPath(augmentExportPath, typeof(ScriptableObject)) as ScriptableObject;\r\n" +
                                "        EditorUtility.SetDirty(obj);\r\n" +
                                "    }\r\n" +
                                "}");
        
        // 출력 디렉터리가 없으면 생성
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // 생성된 스크립트를 .cs 파일로 저장
        string outputFilePath = Path.Combine(outputDirectory, "ImportExcelGenerated.cs");
        File.WriteAllText(outputFilePath, codeBuilder.ToString());

        Debug.Log($"스크립트 {outputFilePath}를 생성 완료하였습니다.");
    }

    private static void ReadExcelDataForAugmentDataAttribute(StringBuilder codeBuilder)
    {
        using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {

            IWorkbook book = new XSSFWorkbook(stream);

            ISheet sheet = book.GetSheetAt(0);

            IRow nameRow = sheet.GetRow(0);
            IRow typeRow = sheet.GetRow(1);

            for (int i = 0; i < nameRow.LastCellNum-1; i++)
            {
                codeBuilder.AppendLine($"        public {typeRow.GetCell(i).StringCellValue} {nameRow.GetCell(i).StringCellValue};");
            }

            stream.Close();
        }
    }

    private static void ReadExcelDataToImportExcel(StringBuilder codeBuilder)
    {
        using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            IWorkbook book = new XSSFWorkbook(stream);

            ISheet sheet = book.GetSheetAt(0);

            IRow nameRow = sheet.GetRow(0);
            IRow typeRow = sheet.GetRow(1);

            for (int i = 0; i < nameRow.LastCellNum - 1; i++)
            {
                codeBuilder.AppendLine($"               augment.{nameRow.GetCell(i).StringCellValue} = ({typeRow.GetCell(i)})row.GetCell({i}).{NumericOrString(typeRow.GetCell(i).ToString())}");
            }
        }
    }

    private static string NumericOrString(string typeRow)
    {
        if(typeRow == "string")
        {
            return "StringCellValue;";
        }
        else
        {
            return "NumericCellValue;";
        }
    }
}


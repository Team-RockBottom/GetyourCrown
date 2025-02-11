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
    /// AugmentData ��ũ��Ʈ �ڵ带 �����Ͽ� ������ ���丮�� .cs ���Ϸ� �����մϴ�.
    /// </summary>
    /// <param name="outputDirectory">������ .cs ������ ������ ���͸�</param>
    public static void GenerateAugmentDataScript(string outputDirectory)
    {
        // C# ��ũ��Ʈ �ڵ� ���ڿ��� ����
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
        // ��� ���͸��� ������ ����
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // ������ ��ũ��Ʈ�� .cs ���Ϸ� ����
        string outputFilePath = Path.Combine(outputDirectory, "AugmentDataGenerated.cs");
        File.WriteAllText(outputFilePath, codeBuilder.ToString());

        Debug.Log($"��ũ��Ʈ {outputFilePath}�� ���� �Ϸ��Ͽ����ϴ�.");
    }

    public static void GenerateImportExcelScript(string outputDirectory)
    {
        // C# ��ũ��Ʈ �ڵ� ���ڿ��� ����
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
                            "    /// ������ ����Ƽ ������ �߰��Ǹ� ����Ǵ� ���� �Լ�\r\n" +
                            "    /// </summary>\r\n" +
                            "    static void OnPostprocessAllAssets(\r\n" +
                            "        string[] importedAssets, string[] deletedAssets,\r\n" +
                            "        string[] movedAssets, string[] movedFromAssetPaths)\r\n" +
                            "    {\r\n" +
                            "\r\n" +
                            "        //����Ʈ �� ��� ������ �˻���\r\n" +
                            "        foreach (string s in importedAssets)\r\n" +
                            "        {\r\n" +
                            "            //�츮�� ���ϴ� ���� �϶��� ����\r\n" +
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
        
        // ��� ���͸��� ������ ����
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // ������ ��ũ��Ʈ�� .cs ���Ϸ� ����
        string outputFilePath = Path.Combine(outputDirectory, "ImportExcelGenerated.cs");
        File.WriteAllText(outputFilePath, codeBuilder.ToString());

        Debug.Log($"��ũ��Ʈ {outputFilePath}�� ���� �Ϸ��Ͽ����ϴ�.");
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


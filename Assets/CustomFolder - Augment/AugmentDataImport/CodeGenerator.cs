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
        GenerateAugmentDataScript("Assets/CustomFolder - System");
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

        ReadExcelData(codeBuilder);

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

    private static void ReadExcelData(StringBuilder codeBuilder)
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
}


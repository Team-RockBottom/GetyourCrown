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

        ReadExcelData(codeBuilder);

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


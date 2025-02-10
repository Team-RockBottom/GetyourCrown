using System.IO;
using System.Text;
using UnityEngine;

public class ExampleCodegenerator : MonoBehaviour
{
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
        codeBuilder.AppendLine("public class AugmentData : ScriptableObject");
        codeBuilder.AppendLine("{");
        codeBuilder.AppendLine("    [System.Serializable]");
        codeBuilder.AppendLine("    public class Attribute");
        codeBuilder.AppendLine("    {");
        codeBuilder.AppendLine("        public int id;");
        codeBuilder.AppendLine("        public string name;");
        codeBuilder.AppendLine("        public string description;");
        codeBuilder.AppendLine("        public string iconPath;");
        codeBuilder.AppendLine("        public float speed;");
        codeBuilder.AppendLine("        public float increaseDelay;");
        codeBuilder.AppendLine("        public float maxSpeed;");
        codeBuilder.AppendLine("        public float increaseValue;");
        codeBuilder.AppendLine("        public float coolDown;");
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
        string outputFilePath = Path.Combine(outputDirectory, "AugmentData.cs");
        File.WriteAllText(outputFilePath, codeBuilder.ToString());

        Debug.Log($"스크립트 'AugmentData.cs'가 성공적으로 생성되었습니다: {outputFilePath}");
    }
}


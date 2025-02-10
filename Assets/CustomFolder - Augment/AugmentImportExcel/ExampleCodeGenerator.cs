using System.IO;
using System.Text;
using UnityEngine;

public class ExampleCodegenerator : MonoBehaviour
{
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
        // ��� ���͸��� ������ ����
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // ������ ��ũ��Ʈ�� .cs ���Ϸ� ����
        string outputFilePath = Path.Combine(outputDirectory, "AugmentData.cs");
        File.WriteAllText(outputFilePath, codeBuilder.ToString());

        Debug.Log($"��ũ��Ʈ 'AugmentData.cs'�� ���������� �����Ǿ����ϴ�: {outputFilePath}");
    }
}


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GetyourCrown.UI.UI_Utilities
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ResolveAttribute : Attribute
    {

    }

    public static class ResolvePrefixTable
    {
        public static string GetPrefix(Type type)
        {
            if (s_prefixes.TryGetValue(type, out string prefix))
                return prefix;

            return string.Empty;
        }

        private static Dictionary<Type, string> s_prefixes = new Dictionary<Type, string>()
        {
            { typeof(Transform), "" },
            { typeof(RectTransform), "" },
            { typeof(GameObject), "" },
            { typeof(RawImage), "" },
            { typeof(LeaderBoardSlot), "" },
            { typeof(TMP_Text), "Text (TMP) - " },
            { typeof(TextMeshProUGUI), "Text (TMP) - " },
            { typeof(TextMeshPro), "Text (TMP) - " },
            { typeof(TMP_InputField), "InputField (TMP) - " },
            { typeof(Image), "Image - " },
            { typeof(Button), "Button - " },
            { typeof(Toggle), "Toggle - " },
            { typeof(Slider), "Slider - " }
        };
    }

    public abstract class ComponentResolvingBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            ResolveAll();
        }

        private void ResolveAll()
        {
            Type type = GetType();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            StringBuilder stringBuilder = new StringBuilder(40);

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                ResolveAttribute resolveAttribute = fieldInfos[i].GetCustomAttribute<ResolveAttribute>();

                if (resolveAttribute != null)
                {
                    stringBuilder.Clear();
                    string prefix = ResolvePrefixTable.GetPrefix(fieldInfos[i].FieldType);
                    stringBuilder.Append(prefix);
                    string fieldName = fieldInfos[i].Name;
                    bool isFirstCharacter = true;

                    for (int j = 0; j < fieldName.Length; j++)
                    {
                        if (isFirstCharacter)
                        {
                            if (fieldName[j].Equals('_'))
                                continue;

                            stringBuilder.Append(char.ToUpper(fieldName[j]));
                            isFirstCharacter = false;
                        }
                        else
                        {
                            stringBuilder.Append(fieldName[j]);
                        }
                    }

                    Transform child = transform.FindChildReculsively(stringBuilder.ToString());

                    if (child)
                    {
                        Component childComponent = child.GetComponent(fieldInfos[i].FieldType);
                        fieldInfos[i].SetValue(this, childComponent);
                    }
                    else
                    {
                        Debug.LogError($"[{name}] :Cannot resolve field {fieldInfos[i].Name}");
                    }
                }
            }
        }
    }
}
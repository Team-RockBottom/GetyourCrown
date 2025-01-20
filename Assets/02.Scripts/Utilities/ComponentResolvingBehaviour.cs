using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Practices.UGUI_Management.Utilities
{
    /// <summary>
    /// 자동 의존성 주입을 위한 특성
    /// </summary>
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
            { typeof(TMP_Text), "Text (TMP) - " },
            { typeof(TextMeshProUGUI), "Text (TMP) - " },
            { typeof(TextMeshPro), "Text (TMP) - " },
            { typeof(TMP_InputField), "InputField (TMP) - " },
            { typeof(Image), "Image - " },
            { typeof(Button), "Button - " }
        };
    }

    /// <summary>
    /// 모든 자식들을 탐색하여 의존성주입이 가능한 필드의 의존성을 알아서 해결해주는 기반클래스.
    /// </summary>
    public abstract class ComponentResolvingBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            ResolveAll();
        }

        private void ResolveAll()
        {
            // Reflection: 런타임중에 "메타" 데이터에 접근하는 기능
            // ex) GetType()/ typeof() <- "자료형" 데이터에 접근하는 기능
            // FieldInfo <- "변수" 데이터에 접근하는 기능

            // GetType() : 현재 객체의 원본 타입에 대한 데이터를 가지고있는 Type 타입의 객체참조를 반환하는함수.
            Type type = GetType();
            // 종속된 자식에 있는 컴포넌트참조가 public 으로 공개된다 <- 객체지향 컨셉에 맞지 않다. 그래서 NonPublic 만 허용할거다.
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

                    // _camelCase -> PascalCase
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
                        // fieldInfos[i].GetType() == typeof(Transform).GetType()
                        // fieldInfos[i].FieldType() == typeof(Transform)
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

    public class A : ComponentResolvingBehaviour
    {
        [Resolve] Transform _content;
    }
}
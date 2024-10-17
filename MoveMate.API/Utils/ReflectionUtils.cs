using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.API.Utils
{
    public static class ReflectionUtils
    {
        public static void DoWithFields<T>(T source, Action<FieldInfo> action)
        {
            foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                                                      BindingFlags.Instance))
            {
                action(field);
            }
        }

        public static void UpdateFields<TSource, TTarget>(TSource source, TTarget target)
        {
            DoWithFields(source, field =>
            {
                var newValue = field.GetValue(source);

                // Ensure the field is not null and the types match
                if (newValue != null && field.FieldType == newValue.GetType())
                {
                    var fieldName = field.Name;
                    var existingField = target.GetType().GetField(fieldName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (existingField != null)
                    {
                        // Convert boolean to string if necessary
                        if (newValue is bool boolValue && existingField.FieldType == typeof(string))
                        {
                            existingField.SetValue(target, boolValue.ToString());
                        }
                        else
                        {
                            existingField.SetValue(target, newValue);
                        }
                    }
                }
            });
        }
    }
}
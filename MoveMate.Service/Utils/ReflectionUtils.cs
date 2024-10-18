using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MoveMate.Service.Utils
{
    public static class ReflectionUtils
    {
        public static void DoWithProperties<T>(T source, Action<PropertyInfo> action)
        {
            foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                action(property);
            }
        }

        public static void UpdateProperties<TSource, TTarget>(TSource source, TTarget target)
        {
            DoWithProperties(source, property =>
            {
                var newValue = property.GetValue(source);

                // Ensure the property is not null and can be set (has both getter and setter)
                if (newValue != null && property.CanRead && property.CanWrite)
                {
                    var propertyName = property.Name;
                    var targetProperty = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (targetProperty != null && targetProperty.CanWrite)
                    {
                        // Ensure the types match or can be implicitly converted
                        if (targetProperty.PropertyType == property.PropertyType)
                        {
                            targetProperty.SetValue(target, newValue);
                        }
                        // Convert boolean to string if necessary
                        else if (newValue is bool boolValue && targetProperty.PropertyType == typeof(string))
                        {
                            targetProperty.SetValue(target, boolValue.ToString());
                        }
                    }
                }
            });
        }
    }
}


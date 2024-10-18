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

        public static void UpdateProperties<TSource, TDestination>(TSource source, TDestination destination)
        {
            var sourceProperties = typeof(TSource).GetProperties();
            foreach (var property in sourceProperties)
            {
                if (property.CanRead)
                {
                    if(property.Name != "Id")
                    {
                        var value = property.GetValue(source);
                        if (value != null)
                        {
                            var destProperty = typeof(TDestination).GetProperty(property.Name);
                            if (destProperty != null && destProperty.CanWrite)
                            {
                                // Check if the types are compatible
                                if (destProperty.PropertyType.IsAssignableFrom(value.GetType()))
                                {
                                    destProperty.SetValue(destination, value);
                                }
                                else
                                {
                                    throw new InvalidOperationException($"Property type mismatch for {property.Name}");
                                }
                            }
                        }
                    }
                }
                   
            }
        }
    }
}


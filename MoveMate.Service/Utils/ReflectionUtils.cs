using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MoveMate.Service.Utils
{
    public static class ReflectionUtils
    {
        public static void DoWithProperties<T>(T source, Action<PropertyInfo> action)
        {
            // Iterate through all properties of the source type
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
                // Skip the Id property
                if (property.CanRead && property.Name != "Id")
                {
                    var value = property.GetValue(source);
                    if (value != null)
                    {
                        var destProperty = typeof(TDestination).GetProperty(property.Name);
                        if (destProperty != null && destProperty.CanWrite)
                        {
                            // Check if the destination property is a collection
                            if (typeof(IEnumerable).IsAssignableFrom(destProperty.PropertyType) && destProperty.PropertyType != typeof(string))
                            {
                                HandleCollectionUpdate(value, destProperty, destination);
                            }
                            // Check for compatible types for single value properties
                            else if (destProperty.PropertyType.IsAssignableFrom(value.GetType()))
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

        private static void HandleCollectionUpdate(object sourceValue, PropertyInfo destProperty, object destination)
        {
            var sourceCollection = sourceValue as IEnumerable;
            var destCollection = destProperty.GetValue(destination) as IList;

            if (sourceCollection != null && destCollection != null)
            {
                var itemType = destProperty.PropertyType.GenericTypeArguments[0];

                foreach (var sourceItem in sourceCollection)
                {
                    var sourceServiceId = sourceItem.GetType().GetProperty("ServiceId")?.GetValue(sourceItem);

                    // Find corresponding item in destCollection based on ServiceId
                    var existingItem = destCollection.Cast<object>()
                        .FirstOrDefault(destItem =>
                            destItem.GetType().GetProperty("ServiceId")?.GetValue(destItem)?.Equals(sourceServiceId) == true);

                    if (existingItem != null)
                    {
                        // If service already exists, update quantity
                        var sourceQuantity = sourceItem.GetType().GetProperty("Quantity")?.GetValue(sourceItem);
                        var destQuantityProperty = existingItem.GetType().GetProperty("Quantity");

                        if (destQuantityProperty != null && sourceQuantity != null)
                        {
                            destQuantityProperty.SetValue(existingItem, sourceQuantity);
                        }
                    }
                    else
                    {
                        // If service does not exist, add new item to destCollection
                        var mappedItem = MapToDestinationType(sourceItem, itemType);
                        destCollection.Add(mappedItem);
                    }
                }
            }
        }

        private static object MapToDestinationType(object sourceItem, Type destType)
        {
            // Create an instance of the destination type
            var destItem = Activator.CreateInstance(destType);

            // Perform property mapping
            foreach (var sourceProp in sourceItem.GetType().GetProperties())
            {
                var destProp = destType.GetProperty(sourceProp.Name);
                if (destProp != null && destProp.CanWrite && destProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                {
                    var value = sourceProp.GetValue(sourceItem);
                    destProp.SetValue(destItem, value);
                }
            }

            return destItem;
        }
    }
}

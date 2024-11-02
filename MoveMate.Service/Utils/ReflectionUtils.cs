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
                if (property.CanRead && property.Name != "Id") // Skip Id property
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
                            // Check if the types are compatible for single value properties
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

        public static void UpdateProperties<TSource, TDestination>(TSource source, TDestination destination, List<string> excludeProperties = null)
        {
            var sourceProperties = typeof(TSource).GetProperties();
            foreach (var property in sourceProperties)
            {
                if (property.CanRead && (excludeProperties == null || !excludeProperties.Contains(property.Name)))
                {
                    if (property.CanRead && property.Name != "Id")
                    {
                        var value = property.GetValue(source);
                        if (value != null)
                        {
                            var destProperty = typeof(TDestination).GetProperty(property.Name);
                            if (destProperty != null && destProperty.CanWrite)
                            {
                                if (typeof(IEnumerable).IsAssignableFrom(destProperty.PropertyType) && destProperty.PropertyType != typeof(string))
                                {
                                    HandleCollectionUpdate(value, destProperty, destination);
                                }
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
        }


        private static void HandleCollectionUpdate(object sourceValue, PropertyInfo destProperty, object destination)
        {
            var sourceCollection = sourceValue as IEnumerable;
            var destCollection = destProperty.GetValue(destination) as IList;

            if (sourceCollection != null && destCollection != null)
            {
                var itemType = destProperty.PropertyType.GenericTypeArguments[0];

                // Use a HashSet to track existing ServiceIds
                var existingServiceIds = new HashSet<object>(
                    destCollection.Cast<object>()
                                  .Select(item => item.GetType().GetProperty("ServiceId")?.GetValue(item)));

                foreach (var sourceItem in sourceCollection)
                {
                    var sourceServiceId = sourceItem.GetType().GetProperty("ServiceId")?.GetValue(sourceItem);

                    // Check if the ServiceId already exists
                    if (sourceServiceId != null && existingServiceIds.Contains(sourceServiceId))
                    {
                        // If the service already exists, update its quantity
                        var existingItem = destCollection.Cast<object>()
                            .First(destItem => destItem.GetType().GetProperty("ServiceId")?.GetValue(destItem)?.Equals(sourceServiceId) == true);

                        UpdateQuantity(sourceItem, existingItem);
                    }
                    else
                    {
                        // If the service doesn't exist, add it to the collection
                        var mappedItem = MapToDestinationType(sourceItem, itemType);
                        destCollection.Add(mappedItem);

                        // Add the new ServiceId to the HashSet to track it
                        existingServiceIds.Add(sourceServiceId);
                    }
                }
            }
        }

        private static void UpdateQuantity(object sourceItem, object existingItem)
        {
            var sourceQuantity = sourceItem.GetType().GetProperty("Quantity")?.GetValue(sourceItem);
            var destQuantityProperty = existingItem.GetType().GetProperty("Quantity");

            if (destQuantityProperty != null && sourceQuantity != null)
            {
                // Update the existing item's quantity
                destQuantityProperty.SetValue(existingItem, sourceQuantity);
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

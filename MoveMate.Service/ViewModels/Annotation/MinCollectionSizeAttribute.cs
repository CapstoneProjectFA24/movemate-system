using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoveMate.Service.ViewModels.Annotation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class MinCollectionSizeAttribute : ValidationAttribute
    {
        private readonly int _minSize;

        public MinCollectionSizeAttribute(int minSize)
        {
            _minSize = minSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IEnumerable<object> collection)
            {
                var count = 0;

                foreach (var _ in collection)
                {
                    count++;
                    if (count >= _minSize)
                    {
                        return ValidationResult.Success; // Dừng lại khi đủ kích thước tối thiểu
                    }
                }
            }

            return new ValidationResult(ErrorMessage ?? $"The collection must contain at least {_minSize} items.");
        }
    }
}
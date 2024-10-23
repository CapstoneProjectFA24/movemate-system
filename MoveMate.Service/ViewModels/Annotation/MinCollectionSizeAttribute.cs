using System.Collections;
using System.ComponentModel.DataAnnotations;
using MoveMate.Service.Exceptions;

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
            if (value == null)
            {
                throw new BadRequestException( $"{validationContext.DisplayName} must contain at least {_minSize} items.");
            }
            
            if (value is IEnumerable collection)
            {
                var count = 0;

                foreach (var _ in collection)
                {
                    count++;
                    if (count >= _minSize)
                    {
                        return ValidationResult.Success; 
                    }
                }
            }
            
            throw new BadRequestException( $"{validationContext.DisplayName} must contain at least {_minSize} items." );
        }
    }
}
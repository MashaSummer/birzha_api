using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BlazorClient.Attributes
{
    [AttributeUsage(AttributeTargets.Property |
    AttributeTargets.Field, AllowMultiple = false)]
    sealed public class BasePriceAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var castedValue = (double)value;

            return ValidatePrice(castedValue);
        }
        public bool ValidatePrice(double price)
        {
            try
            {
                var priceToString = price.ToString();
                var unitsNanos = priceToString.Split(".");
                if(unitsNanos.Count() == 1)
                {
                    try
                    {
                        var intValue = Convert.ToInt32(unitsNanos[0]);
                        return true;
                    }
                    catch(Exception ex)
                    {
                        return false;
                    }
                }
                if (unitsNanos.Count() != 2)
                {
                    return false;
                }
                if (Convert.ToInt32(unitsNanos[1]) > 99)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public override string FormatErrorMessage(string name)
        {
            return ErrorMessageString;
        }
    }
}

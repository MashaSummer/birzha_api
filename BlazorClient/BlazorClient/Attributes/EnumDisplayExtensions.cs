using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BlazorClient.Attributes
{
    public static class EnumDisplayExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            MemberInfo[] memberInfo = enumValue.GetType().GetMember(enumValue.ToString());

            Type attributeType = typeof(DisplayAttribute);

            object[] attributes = memberInfo[0].GetCustomAttributes(attributeType, false);

            var attribute = attributes.SingleOrDefault() as DisplayAttribute;

            return attribute?.Name;
        }
    }
}

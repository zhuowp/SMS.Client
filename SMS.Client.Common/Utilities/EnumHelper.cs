using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SMS.Client.Common.Utilities
{
    public class EnumHelper
    {
        public static void GetEnumMemberList(Type enumType)
        {
            Array values = Enum.GetValues(enumType);
            for (int i = 0; i < values.Length; i++)
            {
                var v = values.GetValue(i);
                var member = enumType.GetMember(v.ToString());
                DescriptionAttribute des = (DescriptionAttribute)Attribute.GetCustomAttribute(member[0], typeof(DescriptionAttribute));
            }
        }
    }
}

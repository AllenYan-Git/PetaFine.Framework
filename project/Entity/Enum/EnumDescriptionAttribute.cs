using System;

namespace Entity.Enum
{
    /// <summary>
    /// 枚举注释的自定义属性类
    /// </summary>
    public class EnumDescriptionAttribute : Attribute
    {
        public EnumDescriptionAttribute(string strPrinterName)
        {
            Description = strPrinterName;
        }

        public string Description { get; }
    }
}

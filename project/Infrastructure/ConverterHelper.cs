using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure
{
    /// <summary>
    /// 各种值类型转换
    /// </summary>
    public static class ConverterHelper
    {
        #region 字符类型转换成其他类型
        /// <summary>
        /// 字符串转int
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defalut"></param>
        /// <returns></returns>
        public static int ToInt(this string s, int defalut = 0)
        {
            int result = defalut;
            if (int.TryParse(s, out result))
                return result;
            else
                return defalut;
        }

        /// <summary>
        /// 字符串转bool
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defalut"></param>
        /// <returns></returns>
        public static bool ToBool(this string s, bool defalut = false)
        {
            bool result = defalut;
            if (bool.TryParse(s, out result))
                return result;
            else
                return defalut;
        }

        /// <summary>
        /// 字符串转double
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defalut"></param>
        /// <returns></returns>
        public static double ToDouble(this string s, double defalut = 0)
        {
            double result = defalut;
            if (double.TryParse(s, out result))
                return result;
            else
                return defalut;
        }

        /// <summary>
        /// 字符串转decimal
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defalut"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string s, decimal defalut = 0)
        {
            decimal result = defalut;
            if (decimal.TryParse(s, out result))
                return result;
            else
                return defalut;
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloatExt(this string strValue, float defalut = 0)
        {
            float result = defalut;
            if (float.TryParse(strValue, out result))
                return result;
            else
                return defalut;
        }

        /// <summary>
        /// 字符串转GUID
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string s)
        {
            Guid result = Guid.Empty;
            if (Guid.TryParse(s, out result))
                return result;
            else
                return Guid.Empty;
        }

        /// <summary>
        /// 字符串转日期
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defalut"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string s, DateTime defalut = new DateTime())
        {
            DateTime result = defalut;
            if (DateTime.TryParse(s, out result))
                return result;
            else
                return defalut;
        }

        /// <summary>
        /// 字符串转Enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string s) where T : struct
        {
            T result = default(T);
            Enum.TryParse<T>(s, true, out result);
            return result;
        }
        #endregion

        #region  时间类型转换
        /// <summary>
        /// 将时间精确到哪个级别
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="cutTicks"></param>
        /// <returns></returns>
        public static DateTime CutOff(this DateTime dateTime, long cutTicks = TimeSpan.TicksPerSecond)
        {
            return new DateTime(dateTime.Ticks - (dateTime.Ticks % cutTicks), dateTime.Kind);
        }

        /// <summary>
        /// 把时间转换成字符串如：2013-8-2
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        /// <returns></returns>
        public static string ToCnDataString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 日期转当前天，跟今天比，如转成“今天”，“昨天”，不符和就转成如“2012-8-2”
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToDay(this DateTime date)
        {
            string s = "";
            var now = DateTime.Now.Day;
            if (now == date.Day)
            {
                s = "今天";
            }
            else if (now - date.Day == 1)
            {
                s = "昨天";
            }
            else
            {
                s = date.ToString("yyyy-MM-dd");
            }
            return s;
        }

        /// <summary>
        /// 日期转星期几，如"星期日", "星期一"
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToWeek(this DateTime date)
        {
            var dayOfWeek = Convert.ToInt32(date.DayOfWeek);

            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            return weekdays[dayOfWeek];
        }

        #endregion

        /// <summary>
        /// 小数转整数，类似四舍五入
        /// </summary>
        /// <param name="value">小数</param>
        /// <returns>整数</returns>
        public static int ToInt(this decimal value)
        {
            var decimalNum = value - (int)value;
            if (decimalNum >= 0.5m)
                return ((int)value) + 1;
            else
                return (int)value;
        }

        /// <summary>
        /// double转整数，类似四舍五入
        /// </summary>
        /// <param name="value">double</param>
        /// <returns>整数</returns>
        public static int ToInt(this double value)
        {
            return ((decimal)value).ToInt();
        }
        /// <summary>
        /// 64位编码加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToBase64String(string value)
        {
            string v = "";
            if (value != "")
            {
                try
                {
                    v = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(value));
                }
                catch
                {
                    v = "";
                }
            }
            else
            {
                v = "";
            }
            return v;
        }
        /// <summary>
        /// 64位编码解密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FromBase64String(string value)
        {
            string v = "";
            if (value != "")
            {
                try
                {
                    v = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
                }
                catch
                {
                    v = "";
                }
            }
            else
            {
                v = "";
            }
            return v;
        }
        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(object expression, bool defValue)
        {
            if (expression != null)
                return StrToBool(expression, defValue);

            return defValue;
        }

        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(string expression, bool defValue)
        {
            if (expression != null)
            {
                if (string.Compare(expression, "true", true) == 0)
                    return true;
                else if (string.Compare(expression, "false", true) == 0)
                    return false;
            }
            return defValue;
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ObjectToInt(object expression)
        {
            return ObjectToInt(expression, 0);
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ObjectToInt(object expression, int defValue)
        {
            if (expression != null)
                return StrToInt(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// 将对象转换为Int32类型,转换失败返回0
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(string str)
        {
            return StrToInt(str, 0);
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(string str, int defValue)
        {
            if (string.IsNullOrEmpty(str) || str.Trim().Length >= 11 || !Regex.IsMatch(str.Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
                return defValue;

            int rv;
            if (Int32.TryParse(str, out rv))
                return rv;

            return Convert.ToInt32(StrToFloat(str, defValue));
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(object strValue, float defValue)
        {
            if ((strValue == null))
                return defValue;

            return StrToFloat(strValue.ToString(), defValue);
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float ObjectToFloat(object strValue, float defValue)
        {
            if ((strValue == null))
                return defValue;

            return StrToFloat(strValue.ToString(), defValue);
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float ObjectToFloat(object strValue)
        {
            return ObjectToFloat(strValue.ToString(), 0);
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(object strValue)
        {
            if ((strValue == null))
                return 0;

            return StrToFloat(strValue.ToString(), 0);
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(string strValue, float defValue)
        {
            if ((strValue == null) || (strValue.Length > 10))
                return defValue;

            float intValue = defValue;
            if (strValue != null)
            {
                bool IsFloat = Regex.IsMatch(strValue, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                    float.TryParse(strValue, out intValue);
            }
            return intValue;
        }

        /// <summary>
        /// string型转换为DateTime型
        /// </summary>
        /// <param name="strValue">要转换的string型</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的datetime类型结果，转换失败默认返回当前时间</returns>
        public static DateTime ObjectToDateTime(object expression, DateTime defValue)
        {
            if (expression != null)
            {
                try
                {
                    defValue = Convert.ToDateTime(expression);
                }
                catch (Exception)
                {

                    if (defValue == null)
                    {
                        defValue = DateTime.Now;
                    }
                }

            }
            return defValue;
        }

        /// <summary>
        /// 对象转换为DateTime型
        /// </summary>
        /// <param name="strValue">要转换的对象</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的datetime类型结果</returns>
        public static DateTime StringToDateTime(object expression, DateTime defValue)
        {
            return ObjectToDateTime(expression, defValue);
        }

        /// <summary>
        /// 对象转换为DateTime型
        /// </summary>
        /// <param name="strValue">要转换的对象</param>
        /// <returns>转换后的datetime类型结果,转换失败默认返回当前时间</returns>
        public static DateTime StringToDateTime(object expression)
        {
            return ObjectToDateTime(expression, DateTime.Now);
        }
        
    }
}

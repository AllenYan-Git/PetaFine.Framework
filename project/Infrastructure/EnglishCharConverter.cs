using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure
{
    public class EnglishCharConverter
    {
        private static string _englishCharList = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// 把数字（从 1 开始）转换成大写的英文字母
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static char ConvertNumberToEnglishChar(int num)
        {
            if (num <= 0)
            {
                throw new ArgumentException("无法把小于或等于 0 的数字转换成大写的英文字母！");
            }
            if (num > 26)
            {
                throw new ArgumentException("无法把大于 26 的数字转换成大写的英文字母！");
            }
            return _englishCharList[num - 1];
        }

        /// <summary>
        /// 把包含数字（从 1 开始）的字符串转换成大写的英文字母
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static char ConvertNumberToEnglishChar(string num)
        {
            if (string.IsNullOrEmpty(num))
            {
                throw new ArgumentNullException("num");
            }
            int intNum;
            if (!int.TryParse(num, out intNum))
            {
                throw new ArgumentException("无法把非数字类型的参数 \"num\" 转换为 Int 类型！");
            }
            return ConvertNumberToEnglishChar(intNum);
        }

        /// <summary>
        /// 把大写的英文字母转换成数字（从 1 开始）
        /// </summary>
        /// <param name="englishChar"></param>
        /// <returns></returns>
        public static int ConvertEnglishCharToNumber(char englishChar)
        {
            if (!_englishCharList.Contains(englishChar))
            {
                throw new ArgumentException("参数必须是大写的英文字母！");
            }
            return _englishCharList.IndexOf(englishChar) + 1;
        }

        /// <summary>
        /// 把一个英文集合转换为数字集合
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        public static IEnumerable<string> ConvertEnglishCharArrayToNumberArray(IEnumerable<string> nums)
        {
            return ConvertEnglishCharArrayToNumberArray<string>(nums, d => d.ToString());
        }

        /// <summary>
        /// 把一个英文集合转换为数字集合
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> ConvertEnglishCharArrayToNumberArray<TResult>(IEnumerable<string> nums, Func<int, TResult> funcConversion)
        {
            if (funcConversion == null)
            {
                throw new ArgumentNullException("funcConversion");
            }
            if (nums == null)
            {
                yield return default(TResult);
            }
            foreach (string item in nums)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                yield return funcConversion(ConvertEnglishCharToNumber(item[0]));
            }
        }

        /// <summary>
        /// 把一个数字集合转换为英文集合
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        public static IEnumerable<string> ConvertNumberArrayToEnglishCharArray(IEnumerable<string> nums)
        {
            if (nums == null)
            {
                yield return string.Empty;
            }
            foreach (string item in nums)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                yield return ConvertNumberToEnglishChar(item).ToString();
            }
        }

        /// <summary>
        /// 把一个以逗号分隔的数字字符串转换成英文字符串
        /// </summary>
        /// <param name="numStr"></param>
        /// <returns></returns>
        public static string ConvertNumberStrToEnglishCharStr(string numStr)
        {
            if (string.IsNullOrEmpty(numStr))
            {
                return null;
            }

            string[] numArr = numStr.Split(',');
            IEnumerable<string> enArr = ConvertNumberArrayToEnglishCharArray(numArr);
            if (enArr != null)
            {
                return string.Join("", enArr);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 生成英文单词数组，最小值为 1，最大值为 26
        /// </summary>
        /// <param name="maxElement"></param>
        /// <returns></returns>
        public static List<string> GenerateEnglishCharArray(int maxElement)
        {
            if (maxElement <= 0 || maxElement > 26)
            {
                throw new ArgumentException("无法生成非 1-26 数字之间的英文字母数组");
            }
            List<string> result = new List<string>(maxElement);
            for (int i = 1; i <= maxElement; i++)
            {
                result.Add(ConvertNumberToEnglishChar(i).ToString());
            }
            return result;
        }
    }
}

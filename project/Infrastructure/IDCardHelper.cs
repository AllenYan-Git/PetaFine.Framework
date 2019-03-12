using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure
{
    /// <summary>
    /// 身份证解析类(目前有效身份证为18位和15位)
    /// </summary>
    public class IDCardHelper
    {
        #region 根据身份证获取性别 + static string GetSex(string IDCard)
        /// <summary>
        /// 根据身份证获取性别(目前有效身份证为18位和15位)
        /// </summary>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        public static string GetSex(string IDCard)
        {
            string sexNum;
            int sexType;
            if (IDCard.Length == 18)
            {
                sexNum = IDCard.Substring(16, 1);
            }
            else
            {
                //15位身份证
                sexNum = IDCard.Substring(14, 1);
            }
            sexType = Convert.ToInt32(sexNum) % 2;
            return sexType == 1 ? "男" : "女";
        } 
        #endregion

        #region 根据身份证获取出生日期 + static DateTime GetBirthday(string IDCard)
        /// <summary>
        /// 根据身份证获取出生日期
        /// </summary>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        public static DateTime GetBirthday(string IDCard)
        {
            string birNum;
            string birthdayStr;
            DateTime birthday;
            if (IDCard.Length == 18)
            {
                birNum = IDCard.Substring(6, 8);
                birthdayStr = birNum.Substring(0, 4) + "-" + birNum.Substring(4, 2) + "-" + birNum.Substring(6, 2);
            }
            else
            {
                //15位身份证
                birNum = IDCard.Substring(6, 6);
                birthdayStr = "19" + birNum.Substring(0, 2) + "-" + birNum.Substring(2, 2) + "-" + birNum.Substring(4, 2);
            }

            if (DateTime.TryParse(birthdayStr, out birthday))
            {
                return birthday;
            }
            else
            {
                return new DateTime();
            }
        } 
        #endregion

        #region 获取年龄
        /// <summary>
        /// 根据身份证获取年龄
        /// </summary>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        public static int GetAge(string IDCard)
        {
            var birthday = GetBirthday(IDCard);
            return GetAgeByBirthday(birthday);
        }

        /// <summary>
        /// 根据出生日期获取年龄
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public static int GetAgeByBirthday(DateTime birthday)
        {
            int birthYear = birthday.Year;
            int nowYear = DateTime.Now.Year;
            return nowYear - birthYear;
        } 
        #endregion
    }
}

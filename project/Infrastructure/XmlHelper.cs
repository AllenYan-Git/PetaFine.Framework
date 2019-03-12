using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Infrastructure
{
    /// <summary>
    /// XML
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// 将C#数据实体转化为xml数据
        /// </summary>
        /// <param name="obj">要转化的数据实体</param>
        /// <returns>xml格式字符串</returns>
        public static string XmlSerialize<T>(T obj)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            stream.Position = 0;

            StreamReader sr = new StreamReader(stream);
            string resultStr = sr.ReadToEnd();
            sr.Close();
            stream.Close();

            return resultStr;
        }

        /// <summary>
        /// 将xml数据转化为C#数据实体
        /// </summary>
        /// <param name="json">符合xml格式的字符串</param>
        /// <returns>T类型的对象</returns>
        public static T XmlDeserialize<T>(string xml)
        {

            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml.ToCharArray()));
            T obj = (T)serializer.ReadObject(ms);
            ms.Close();

            return obj;
        }

        /// <summary>
        /// 把 XElement 转换为 XML 格式的字符串
        /// </summary>
        /// <param name="rootElement"></param>
        /// <param name="saveOptions"></param>
        /// <returns></returns>
        public static string ToXmlText(this XElement rootElement, SaveOptions saveOptions)
        {
            XDocument xdoc = new XDocument(rootElement);
            xdoc.Declaration = new XDeclaration("1.0", "gb2312", null);

            MemoryStream htmlStream = new MemoryStream();
            xdoc.Save(htmlStream, saveOptions);

            string htmlText = Encoding.GetEncoding("GB2312").GetString(htmlStream.ToArray());
            htmlStream.Close();

            return htmlText;
        }
    }
}

using System;
using System.Web.Configuration;
using System.Xml;

namespace Infrastructure
{
    /// <summary>
    /// 匹配文件类型
    /// 胡维文
    /// 20130126
    /// </summary>
    public class ContentTypeHelper
    {
        /// <summary>
        /// 根据文件后缀获取ContentType
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string GetContentType(string ext)
        {
            switch (ext)
            {
                case "pic":return "application/x-pic";
                case "swf":return "application/x-shockwave-flash";
                case "rar":return "application/rar";
                case "zip":return "application/zip";
                case "asf":return "video/x-ms-asf";
                case "avi": return "video/avi";
                case "mov":return "video/quicktime";
                case "mpeg":return "video/mpg";
                case "ram":return "audio/x-pn-realaudio";
                case "rm":return "application/vnd.rn-realmedia";
                case "rmvb":return "application/vnd.rn-realmedia-vbr";
                case "wmv":return "video/x-ms-wmv";
                case "bmp":return "application/x-bmp";
                case "cdr":return "application/x-cdr";
                case "cgm":return "application/x-cgm";
                case "dxf":return "application/x-dxf";
                case "emf":return "application/x-emf";
                case "eps":return "application/x-ps";
                case "gif":return "image/gif";
                case "jpeg":return "image/jpeg";
                case "pcx":return "application/x-pcx";
                case "pict":return "image/pict";
                case "psd":return "image/x-photoshop";
                case "tga":return "application/x-tga";
                case "tif":return "application/x-tif";
                case "tiff":return "image/tiff";
                case "wmf":return "application/x-wmf";
                case "doc":return "application/msword";
                case "docx":return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case "html":return "text/html";
                case "mht":return "message/rfc822";
                case "pdf":return "application/pdf";
                case "ppt":return "applications-powerpoint、application/x-ppt";
                case "pptx":return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case "rtf":return "application/msword、application/x-rtf";
                case "sgml":return "text/sgml";
                case "txt":return "text/plain";
                case "wpd":return "application/x-wpd";
                case "xls":return "application/x-xls";
                case "xlsx":return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case "xml":return "text/xml";
                case "midi":return "audio/mid";
                case "mp3":return "audio/mp3";
                case "ogg":return "application/ogg";
                case "ra":return "audio/vnd.rn-realaudio、audio/x-realaudio";
                case "wav":return "audio/wav";
                case "wma":return "audio/x-ms-wma";
                case "png":return "application/x-png";
                case "jpg":return "application/x-jpg";
                case "pps":return "application/vnd.ms-powerpoint";
                case "flv":return "video/x-flv";
                default:return "text/plain";
            }
        }
    }
}

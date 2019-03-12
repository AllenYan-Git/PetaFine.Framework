using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;

namespace Infrastructure
{
    /// <summary>
    /// Page操作
    /// </summary>
    public class PageHelper
    {
        #region 服务器URL
        /// <summary>
        /// 服务器URL
        /// </summary>
        /// <returns></returns>
        public static string GetServerUrl()
        {
            return string.Format("http://{0}/", HttpContext.Current.Request.ServerVariables["HTTP_HOST"]);
        }
        #endregion

        #region 获得当前绝对路径 public static string GetMapPath(string strPath)
        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用
            {
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith("\\"))
                {
                    strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
                }
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }
        #endregion

        #region  判断当前页面是否接收到了Post请求
        /// <summary>
        /// 判断当前页面是否接收到了Post请求
        /// </summary>
        /// <returns>是否接收到了Post请求</returns>
        public static bool IsPost()
        {
            return HttpContext.Current.Request.HttpMethod.Equals("POST");
        }
        #endregion

        #region  判断当前页面是否接收到了Get请求
        /// <summary>
        /// 判断当前页面是否接收到了Get请求
        /// </summary>
        /// <returns>是否接收到了Get请求</returns>
        public static bool IsGet()
        {
            return HttpContext.Current.Request.HttpMethod.Equals("GET");
        }
        #endregion

        #region 返回指定的服务器变量信息
        /// <summary>
        /// 返回指定的服务器变量信息
        /// </summary>
        /// <param name="strName">服务器变量名</param>
        /// <returns>服务器变量信息</returns>
        public static string GetServerString(string strName)
        {
            if (HttpContext.Current.Request.ServerVariables[strName] == null)
                return "";

            return HttpContext.Current.Request.ServerVariables[strName].ToString();
        }
        #endregion

        #region 返回上一个页面的地址
        /// <summary>
        /// 返回上一个页面的地址
        /// </summary>
        /// <returns>上一个页面的地址</returns>
        public static string GetUrlReferrer()
        {
            string retVal = null;

            try
            {
                retVal = HttpContext.Current.Request.UrlReferrer.ToString();
            }
            catch { }

            if (retVal == null)
                return "";

            return retVal;
        }
        #endregion

        #region 得到当前完整主机头
        /// <summary>
        /// 得到当前完整主机头
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFullHost()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (!request.Url.IsDefaultPort)
                return string.Format("{0}:{1}", request.Url.Host, request.Url.Port.ToString());

            return request.Url.Host;
        }
        #endregion

        #region   得到主机头
        /// <summary>
        /// 得到主机头
        /// </summary>
        /// <returns></returns>
        public static string GetHost()
        {
            return HttpContext.Current.Request.Url.Host;
        }
        #endregion

        #region  获取指定URL下的文件名
        /// <summary>
        /// 获取指定URL下的文件名
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetUrlFileName(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return "";
            //如果不是合法的URL地址
            if (!ValidationHelper.IsURL(filePath)) return "";
            string[] urlArr = filePath.Split('/');
            string filename = urlArr[urlArr.Length - 1].ToLower();
            if (filename.Split('.').Length >= 2) return filename;
            return "";
        }
        #endregion

        #region 获取当前请求的原始 URL(URL 中域信息之后的部分,包括查询字符串(如果存在))
        /// <summary>
        /// 获取当前请求的原始 URL(URL 中域信息之后的部分,包括查询字符串(如果存在))
        /// </summary>
        /// <returns>原始 URL</returns>
        public static string GetRawUrl()
        {
            return HttpContext.Current.Request.RawUrl;
        }
        #endregion

        #region 判断当前访问是否来自浏览器软件
        /// <summary>
        /// 判断当前访问是否来自浏览器软件
        /// </summary>
        /// <returns>当前访问是否来自浏览器软件</returns>
        public static bool IsBrowserGet()
        {
            string[] BrowserName = { "ie", "opera", "netscape", "mozilla", "konqueror", "firefox" };
            string curBrowser = HttpContext.Current.Request.Browser.Type.ToLower();
            for (int i = 0; i < BrowserName.Length; i++)
            {
                if (curBrowser.IndexOf(BrowserName[i]) >= 0)
                    return true;
            }
            return false;
        }
        #endregion

        #region  判断是否来自搜索引擎链接
        /// <summary>
        /// 判断是否来自搜索引擎链接
        /// </summary>
        /// <returns>是否来自搜索引擎链接</returns>
        public static bool IsSearchEnginesGet()
        {
            if (HttpContext.Current.Request.UrlReferrer == null)
                return false;

            string[] SearchEngine = { "google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou" };
            string tmpReferrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
            for (int i = 0; i < SearchEngine.Length; i++)
            {
                if (tmpReferrer.IndexOf(SearchEngine[i]) >= 0)
                    return true;
            }
            return false;
        }
        #endregion

        #region  获得当前完整Url地址
        /// <summary>
        /// 获得当前完整Url地址
        /// </summary>
        /// <returns>当前完整Url地址</returns>
        public static string GetUrl()
        {
            return HttpContext.Current.Request.Url.ToString();
        }
        #endregion

        #region  获得指定Url参数的值
        /// <summary>
        /// 获得指定Url参数的值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(string strName)
        {
            return GetQueryString(strName, false);
        }
        #endregion

        #region   获得指定Url参数的值
        /// <summary>
        /// 获得指定Url参数的值
        /// </summary> 
        /// <param name="strName">Url参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(string strName, bool sqlSafeCheck)
        {
            if (HttpContext.Current.Request.QueryString[strName] == null)
                return "";

            if (sqlSafeCheck && !ValidationHelper.IsSafeSqlString(HttpContext.Current.Request.QueryString[strName]))
                return "unsafe string";

            return HttpContext.Current.Request.QueryString[strName];
        }
        #endregion

        #region   获得当前页面的名称
        /// <summary>
        /// 获得当前页面的名称
        /// </summary>
        /// <returns>当前页面的名称</returns>
        public static string GetPageName()
        {
            string[] urlArr = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
            return urlArr[urlArr.Length - 1].ToLower();
        }
        #endregion

        #region  返回表单或Url参数的总个数
        /// <summary>
        /// 返回表单或Url参数的总个数
        /// </summary>
        /// <returns></returns>
        public static int GetParamCount()
        {
            return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count;
        }
        #endregion

        #region  获得指定表单参数的值
        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <returns>表单参数的值</returns>
        public static string GetFormString(string strName)
        {
            return GetFormString(strName, false);
        }
        #endregion

        #region  获得指定表单参数的值
        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>表单参数的值</returns>
        public static string GetFormString(string strName, bool sqlSafeCheck)
        {
            if (HttpContext.Current.Request.Form[strName] == null)
                return "";

            if (sqlSafeCheck && !ValidationHelper.IsSafeSqlString(HttpContext.Current.Request.Form[strName]))
                return "unsafe string";

            return HttpContext.Current.Request.Form[strName];
        }
        #endregion

        #region  获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">参数</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetString(string strName)
        {
            return GetString(strName, false);
        }
        #endregion

        #region  获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetString(string strName, bool sqlSafeCheck)
        {
            if ("".Equals(GetQueryString(strName)))
                return GetFormString(strName, sqlSafeCheck);
            else
                return GetQueryString(strName, sqlSafeCheck);
        }
        #endregion

        #region  获得指定Url参数的int类型值
        /// <summary>
        /// 获得指定Url参数的int类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <returns>Url参数的int类型值</returns>
        public static int GetQueryInt(string strName)
        {
            return ConverterHelper.StrToInt(HttpContext.Current.Request.QueryString[strName], 0);
        }
        #endregion

        #region  获得指定Url参数的int类型值
        /// <summary>
        /// 获得指定Url参数的int类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url参数的int类型值</returns>
        public static int GetQueryInt(string strName, int defValue)
        {
            return ConverterHelper.StrToInt(HttpContext.Current.Request.QueryString[strName], defValue);
        }
        #endregion

        #region  获得指定表单参数的int类型值
        /// <summary>
        /// 获得指定表单参数的int类型值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>表单参数的int类型值</returns>
        public static int GetFormInt(string strName, int defValue)
        {
            return ConverterHelper.StrToInt(HttpContext.Current.Request.Form[strName], defValue);
        }
        #endregion

        #region  获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// <summary>
        /// 获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static int GetInt(string strName, int defValue)
        {
            if (GetQueryInt(strName, defValue) == defValue)
                return GetFormInt(strName, defValue);
            else
                return GetQueryInt(strName, defValue);
        }
        #endregion

        #region  获得指定Url参数的float类型值
        /// <summary>
        /// 获得指定Url参数的float类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url参数的int类型值</returns>
        public static float GetQueryFloat(string strName, float defValue)
        {
            return ConverterHelper.StrToFloat(HttpContext.Current.Request.QueryString[strName], defValue);
        }
        #endregion

        #region  获得指定表单参数的float类型值
        /// <summary>
        /// 获得指定表单参数的float类型值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>表单参数的float类型值</returns>
        public static float GetFormFloat(string strName, float defValue)
        {
            return ConverterHelper.StrToFloat(HttpContext.Current.Request.Form[strName], defValue);
        }
        #endregion

        #region  获得指定Url或表单参数的float类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// <summary>
        /// 获得指定Url或表单参数的float类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static float GetFloat(string strName, float defValue)
        {
            if (GetQueryFloat(strName, defValue) == defValue)
                return GetFormFloat(strName, defValue);
            else
                return GetQueryFloat(strName, defValue);
        }
        #endregion

        #region  获得当前页面客户端的IP
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            HttpRequest request = HttpContext.Current.Request;
            string result = String.Empty;

            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            // 如果使用代理，获取真实IP 
            if (result != null && result.IndexOf(".") == -1)    //没有“.”肯定是非IPv4格式 
                result = null;
            else if (result != null)
            {
                if (result.IndexOf(",") != -1)
                {
                    //有“,”，估计多个代理。取第一个不是内网的IP。 
                    result = result.Replace(" ", "").Replace("'", "");
                    string[] temparyip = result.Split(",;".ToCharArray());
                    for (int i = 0; i < temparyip.Length; i++)
                    {
                        if (ValidationHelper.IsIP(temparyip[i])
                            && temparyip[i].Substring(0, 3) != "10."
                            && temparyip[i].Substring(0, 7) != "192.168"
                            && temparyip[i].Substring(0, 7) != "172.16.")
                        {
                            return temparyip[i];    //找到不是内网的地址 
                        }
                    }
                }
                else if (ValidationHelper.IsIP(result)) //代理即是IP格式 
                    return result;
                else
                    result = null;    //代理中的内容 非IP，取IP 
            }
            if (string.IsNullOrEmpty(result))
                result = request.UserHostAddress;

            return result;
        }
        #endregion

        #region  保存用户上传的文件
        /// <summary>
        /// 保存用户上传的文件
        /// </summary>
        /// <param name="path">保存路径</param>
        public static void SaveRequestFile(string path)
        {
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                HttpContext.Current.Request.Files[0].SaveAs(path);
            }
        }
        #endregion

        #region  根据网页的HTML内容提取网页的Encoding
        /// <summary>
        /// 根据网页的HTML内容提取网页的Encoding
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static Encoding GetEncoding(string html)
        {
            string pattern = @"(?i)\bcharset=(?<charset>[-a-zA-Z_0-9]+)";
            string charset = Regex.Match(html, pattern).Groups["charset"].Value;
            try { return Encoding.GetEncoding(charset); }
            catch (ArgumentException) { return null; }
        }
        #endregion

        #region 根据网页的HTML内容提取网页的Title
        /// <summary>
        /// 根据网页的HTML内容提取网页的Title
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        static string GetTitle(string html)
        {
            string pattern = @"(?si)<title(?:\s+(?:""[^""]*""|'[^']*'|[^""'>])*)?>(?<title>.*?)</title>";
            return Regex.Match(html, pattern).Groups["title"].Value.Trim();
        }
        #endregion

        #region "获得sessionid"
        /// <summary>
        /// 获得sessionid
        /// </summary>
        public static string GetSessionID
        {
            get
            {
                return HttpContext.Current.Session.SessionID;
            }
        }
        #endregion

        #region "获取页面url"
        /// <summary>
        /// 获取当前访问页面地址
        /// </summary>
        public static string GetScriptName
        {
            get
            {
                return HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"].ToString();
            }
        }

        /// <summary>
        /// 检测当前url是否包含指定的字符
        /// </summary>
        /// <param name="sChar">要检测的字符</param>
        /// <returns></returns>
        public static bool CheckScriptNameChar(string sChar)
        {
            bool rBool = false;
            if (GetScriptName.ToLower().LastIndexOf(sChar) >= 0)
                rBool = true;
            return rBool;
        }

        /// <summary>
        /// 获取当前页面的扩展名
        /// </summary>
        public static string GetScriptNameExt
        {
            get
            {
                return GetScriptName.Substring(GetScriptName.LastIndexOf(".") + 1);
            }
        }

        /// <summary>
        /// 获取当前访问页面地址参数
        /// </summary>
        public static string GetScriptNameQueryString
        {
            get
            {
                return HttpContext.Current.Request.ServerVariables["QUERY_STRING"].ToString();
            }
        }

        /// <summary>
        /// 获得页面文件名和参数名
        /// </summary>
        public static string GetScriptNameUrl
        {
            get
            {
                string Script_Name = PageHelper.GetScriptName;
                Script_Name = Script_Name.Substring(Script_Name.LastIndexOf("/") + 1);
                Script_Name += "?" + GetScriptNameQueryString;
                return Script_Name;
            }
        }

        /// <summary>
        /// 获得当前页面的文件名
        /// </summary>
        public static string GetScriptFileName
        {
            get
            {
                string Script_Name = PageHelper.GetScriptName;
                Script_Name = Script_Name.Substring(Script_Name.LastIndexOf("/") + 1);
                return Script_Name;
            }
        }

        /// <summary>
        /// 获取当前访问页面Url
        /// </summary>
        public static string GetScriptUrl
        {
            get
            {
                return PageHelper.GetScriptNameQueryString == "" ? PageHelper.GetScriptName : string.Format("{0}?{1}", PageHelper.GetScriptName, PageHelper.GetScriptNameQueryString);
            }
        }

        /// <summary>
        /// 返回当前页面目录的url
        /// </summary>
        /// <param name="FileName">文件名</param>
        /// <returns></returns>
        public static string GetHomeBaseUrl(string FileName)
        {
            string Script_Name = PageHelper.GetScriptName;
            return string.Format("{0}/{1}", Script_Name.Remove(Script_Name.LastIndexOf("/")), FileName);
        }

        /// <summary>
        /// 返回当前网站网址
        /// </summary>
        /// <returns></returns>
        public static string GetHomeUrl()
        {
            return HttpContext.Current.Request.Url.Authority;
        }

        /// <summary>
        /// 获取当前访问文件物理目录
        /// </summary>
        /// <returns>路径</returns>
        public static string GetScriptPath
        {
            get
            {
                string Paths = HttpContext.Current.Request.ServerVariables["PATH_TRANSLATED"].ToString();
                return Paths.Remove(Paths.LastIndexOf("\\"));
            }
        }
        #endregion

        #region 获取文件流类型 public static string GetFileType(string type)
        /// <summary>
        /// 获取文件流类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFileType(string type)
        {
            switch (type.ToLower())
            {
                case "doc":
                    return "application/vnd.ms-word";
                case "xls":
                    return "application/vnd.ms-excel";
                case "txt":
                    return "application/vnd.text";
                default:
                    return "application/attachment";
            }
        }
        #endregion

        #region 向浏览器输入文件下载
        /// <summary>
        /// 向浏览器输入文件下载
        /// </summary>
        /// <param name="tempFilePath">文件路径</param>
        /// <param name="tempFileName">文件名</param>
        /// <param name="tempFileType">文件类别</param>
        public static void WriteResponFile(string tempFilePath, string tempFileName, string tempFileType)
        {
            if (!Directory.Exists(tempFilePath))
            {
                Directory.CreateDirectory(tempFilePath);
            }
            if (File.Exists(tempFilePath + tempFileName))
            {
                FileInfo file = new FileInfo(tempFilePath + tempFileName);
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Charset = "GB2312";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                // 添加头信息，为"文件下载/另存为"对话框指定默认文件名
                // HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(file.Name, System.Text.Encoding.UTF8));
                // 添加头信息，指定文件大小，让浏览器能够显示下载进度
                HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
                // 指定返回的是一个不能被客户端读取的流，必须被下载
                HttpContext.Current.Response.ContentType = GetFileType(tempFileType);
                // 把文件流发送到客户端
                //此处需优化，采用分段返回
                HttpContext.Current.Response.WriteFile(file.FullName);
                HttpContext.Current.Response.Flush();
                //输入以后删除这个临时文件
                if (File.Exists(file.FullName))
                    File.Delete(file.FullName);
                // 停止页面的执行
                HttpContext.Current.Response.End();
            }
        }
        #endregion

        #region 向浏览器输入文件下载
        /// <summary>
        /// 向浏览器输入文件下载
        /// </summary>
        /// <param name="tempFilePath">文件路径(包含文件路径和文件名)</param>
        /// <param name="tempFileName">文件真实名子</param>
        /// <param name="tempFileType">文件类别</param>
        public static void WriteFile(string tempFilePath, string tempFileName, string tempFileType)
        {
            try
            {
                if (File.Exists(tempFilePath))
                {
                    FileInfo file = new FileInfo(tempFilePath);
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.Charset = "GB2312";
                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                    // 添加头信息，为"文件下载/另存为"对话框指定默认文件名
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + tempFileName);
                    // 添加头信息，指定文件大小，让浏览器能够显示下载进度
                    HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
                    // 指定返回的是一个不能被客户端读取的流，必须被下载
                    HttpContext.Current.Response.ContentType = GetFileType(tempFileType);
                    // 把文件流发送到客户端
                    //此处需优化，采用分段返回
                    HttpContext.Current.Response.WriteFile(file.FullName);
                    HttpContext.Current.Response.Flush();
                    //输入以后删除这个临时文件
                    //if (File.Exists(file.FullName))
                    //    File.Delete(file.FullName);
                    // 停止页面的执行
                    HttpContext.Current.Response.End();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 向浏览器输入文件下载
        /// </summary>
        /// <param name="tempFilePath">文件路径(包含文件路径和文件名)</param>
        /// <param name="tempFileName">文件真实名子</param>
        /// <param name="tempFileType">文件类别</param>
        /// /// <param name="tempFileType">输出文件后是否删除本地文件</param>
        public static void WriteFile(string tempFilePath, string tempFileName, string tempFileType, bool isDeleteFile)
        {
            try
            {
                if (File.Exists(tempFilePath))
                {
                    FileInfo file = new FileInfo(tempFilePath);
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.Charset = "GB2312";
                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                    // 添加头信息，为"文件下载/另存为"对话框指定默认文件名
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + tempFileName);
                    // 添加头信息，指定文件大小，让浏览器能够显示下载进度
                    HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
                    // 指定返回的是一个不能被客户端读取的流，必须被下载
                    HttpContext.Current.Response.ContentType = GetFileType(tempFileType);
                    // 把文件流发送到客户端
                    //此处需优化，采用分段返回
                    HttpContext.Current.Response.WriteFile(file.FullName);
                    HttpContext.Current.Response.Flush();
                    if (isDeleteFile)
                    {
                        //输入以后删除这个临时文件
                        if (File.Exists(file.FullName))
                            File.Delete(file.FullName);
                    }
                    // 停止页面的执行
                    HttpContext.Current.Response.End();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 向浏览器输入文件下载
        /// </summary>
        /// <param name="tempFilePath">文件路径(包含文件路径和文件名)</param>
        /// <param name="tempFileName">文件真实名子</param>
        /// <param name="tempFileType">文件类别</param>
        public static void WriteBigFile(string tempFilePath, string tempFileName, string tempFileType)
        {
            ///分段下载大小
            ///单位:字节
            int size = 1000000;
            System.IO.Stream iStream = null;
            // Buffer to read 10K bytes in chunk:
            byte[] buffer = new Byte[size];
            // Length of the file:
            int length;
            // Total bytes to read:
            long dataToRead;
            // Identify the file to download including its path.
            //  string filepath = "DownloadFileName";
            try
            {
                // Open the file.
                iStream = new System.IO.FileStream(tempFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                // Total bytes to read:
                dataToRead = iStream.Length;
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + tempFileName);
                // Read the bytes.
                while (dataToRead > 0)
                {
                    // Verify that the client is connected.
                    if (HttpContext.Current.Response.IsClientConnected)
                    {
                        // Read the data in buffer.
                        length = iStream.Read(buffer, 0, size);
                        // Write the data to the current output stream.
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                        // Flush the data to the HTML output.
                        HttpContext.Current.Response.Flush();
                        buffer = new Byte[size];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        //prevent infinite loop if user disconnects
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                // Trap the error, if any.
                HttpContext.Current.Response.Write("Error : " + ex.Message);
                throw ex;
            }
            finally
            {
                if (iStream != null)
                {
                    //Close the file.
                    iStream.Close();
                }
            }
        }


        #region
        /// <summary>
        /// 向浏览器输入文件下载
        /// </summary>
        /// <param name="tempFilePath">文件路径(包含文件路径和文件名)</param>
        /// <param name="tempFileName">文件真实名子</param>
        /// <param name="tempFileType">文件类别</param>
        ///<param name="tempSize">文件大小</param>
        public static void WriteBigFile(int gtaDownLoad, string tempFilePath, string tempFileName, string tempFileType, long tempSize)
        {
            if (gtaDownLoad == 0)
            {
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + tempFileName);
                //HttpContext.Current.Response.AddHeader("Content-Length", tempSize.ToString());
                HttpContext.Current.Response.Write("");
                HttpContext.Current.Response.Flush();
            }
            else
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(tempFilePath);
                if (fileInfo.Exists == true)
                {
                    long ChunkSize = tempSize <= 102400 ? tempSize : 102400;
                    //const long ChunkSize = tempSize;//100K 每次读取文件，只读取100Ｋ，这样可以缓解服务器的压力
                    byte[] buffer = new byte[ChunkSize];
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ClearHeaders();
                    System.IO.FileStream iStream = System.IO.File.OpenRead(tempFilePath);
                    long dataLengthToRead = iStream.Length;//获取下载的文件总大小
                    HttpContext.Current.Response.ContentType = "application/octet-stream";
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + tempFileName);
                    HttpContext.Current.Response.AddHeader("Content-Length", tempSize.ToString());
                    while (dataLengthToRead > 0 && HttpContext.Current.Response.IsClientConnected)
                    {
                        int lengthRead = iStream.Read(buffer, 0, Convert.ToInt32(ChunkSize));//读取的大小
                        //HttpContext.Current.Response.Flush();
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, lengthRead);
                        HttpContext.Current.Response.Flush();
                        dataLengthToRead = dataLengthToRead - lengthRead;
                    }
                    HttpContext.Current.Response.Close();
                }
            }

        }

        #endregion

        #endregion
    }
}

// vs2013项目添加引用应该找到这个包
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class IISHelper
    {
        #region 应用程序池操作

        /// <summary>
        /// 创建应用程序池
        /// </summary>
        /// <param name="poolName">应用程序池名称</param>
        /// <param name="enable32BitOn64">是否启用32位应用程序</param>
        /// <param name="mode">经典模式或者集成模式</param>
        /// <param name="runtimeVersion">CLR版本</param>
        /// <param name="autoStart"></param>
        public static bool CreateAppPool(string poolName, bool enable32BitOn64 = true, ManagedPipelineMode mode = ManagedPipelineMode.Classic, string runtimeVersion = "v4.0", bool autoStart = true)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                if (serverManager.ApplicationPools[poolName] != null)
                {
                    throw new Exception("已经存在的应用程序池，请更换应用池名称！");
                }
                ApplicationPool newPool = serverManager.ApplicationPools.Add(poolName);
                newPool.ManagedRuntimeVersion = runtimeVersion;
                newPool.Enable32BitAppOnWin64 = enable32BitOn64;
                newPool.ManagedPipelineMode = mode;
                newPool.AutoStart = autoStart;
                serverManager.CommitChanges();
                return true;
            }
        }

        /// <summary>
        /// 删除应用程序池
        /// </summary>
        /// <param name="poolName">应用程序池名字</param>
        /// <returns></returns>
        public static bool DeleteAppPool(string poolName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var poolObj = serverManager.ApplicationPools[poolName];
                if (poolObj != null)
                {
                    serverManager.ApplicationPools.Remove(poolObj);
                }
                serverManager.CommitChanges();
                return true;
            }
        }

        /// <summary>
        /// 根据应用程序池名称判断应用程序池是否存在
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public static bool IsAppPoolExisted(string poolName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var poolObj = serverManager.ApplicationPools[poolName];
                if (poolObj == null)
                {
                    return false;
                }
                return true;
            }
        }

        #endregion

        #region 端口操作

        /// <summary>
        /// 判断端口是否被占用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool IsPortUsing(int port)
        {
            bool inUse = false;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }
            return inUse;
        }
        #endregion

        #region 站点操作

        /// <summary>
        /// 获取所有的站点
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllSites()
        {
            using (ServerManager serverManager = new ServerManager())
            {
                return serverManager.Sites.Select(it => it.Name).ToList();
            }
        }

        /// <summary>
        /// 创建站点
        /// </summary>
        /// <param name="siteName">站点名称</param>
        /// <param name="path">站点路径</param>
        /// <param name="port">端口</param>
        /// <param name="host">主机</param>
        /// <param name="adminPassword"></param>
        /// <param name="requestType">请求类型：http或者https</param>
        /// <param name="adminUserName"></param>
        public static void CreateSite(string siteName, string path, string port = "8000", string host = "*", string adminUserName = "", string adminPassword = "", string requestType = "http")
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var sites = serverManager.Sites;
                if (sites[siteName] == null)
                {
                    var site = sites.Add(siteName, requestType, $"{host}:{port}:", path);
                    if (!string.IsNullOrEmpty(adminUserName) && !string.IsNullOrEmpty(adminPassword))
                    {
                        var virtualDic = site.Applications[0].VirtualDirectories[0];
                        virtualDic.UserName = adminUserName;
                        virtualDic.Password = adminPassword;
                    }
                    serverManager.CommitChanges();
                }
            }
        }

        /// <summary>
        /// 删除站点
        /// </summary>
        /// <param name="siteName">站点名称</param>
        public static bool DeleteSite(string siteName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var sites = serverManager.Sites;
                if (sites[siteName] != null)
                {
                    sites.Remove(sites[siteName]);
                    serverManager.CommitChanges();
                }
            }
            return true;
        }

        /// <summary>
        /// 获取站点
        /// </summary>
        /// <param name="serverManager"></param>
        /// <param name="siteName"></param>
        /// <returns></returns>
        private static Site GetSite(ServerManager serverManager, string siteName)
        {
            return serverManager.Sites[siteName];
        }

        /// <summary>
        /// 获取站点
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        public static Site GetSite(string siteName)
        {
            return GetSite(siteName);
        }

        /// <summary>
        /// 判断站点是否存在
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        public static bool IsSiteExisted(string siteName)
        {
            using (var serverManager = new ServerManager())
            {
                if (serverManager.Sites[siteName] == null)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 获取站点的物理路径
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        public static string GetSitePhysicalPath(string siteName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var app = serverManager.Sites[siteName].Applications.Where(it => it.Path == "/").SingleOrDefault();
                if (app != null)
                {
                    return app.VirtualDirectories[0].PhysicalPath;
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region 应用程序操作
        /// <summary>
        /// 判断应用程序是否存在
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static bool IsApplicationExisted(string siteName, string appName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var site = serverManager.Sites[siteName];
                if (GetApplication(serverManager, siteName, appName) == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 创建应用程序
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="applicationName"></param>
        /// <param name="path"></param>
        public static void CreateApplication(string siteName, string applicationName, string path)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var site = GetSite(serverManager, siteName);
                var config = site.GetWebConfiguration();
                var applications = site.Applications;
                if (applications["/" + applicationName] == null)
                {
                    var app = applications.Add("/" + applicationName, path);
                    serverManager.CommitChanges();
                }
            }
        }

        /// <summary>
        /// 创建虚拟路径
        /// </summary>
        /// <param name="siteName">站点名称</param>
        /// <param name="applicationName">应用名称</param>
        /// <param name="virtualDirectoryName">虚拟路径名称</param>
        /// <param name="path">路径</param>
        public static void CreateVirtualDirectory(string siteName, string applicationName, string virtualDirectoryName, string path)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                Application application = GetApplication(serverManager, siteName, applicationName);
                application.VirtualDirectories.Add("/" + virtualDirectoryName, path);
                serverManager.CommitChanges();
            }
        }

        /// <summary>
        /// 给站点设置应用程序池
        /// </summary>
        /// <param name="siteName">站点名称</param>
        /// <param name="applicationPoolName">应用名称</param>
        public static void SetSiteApplicationPool(string siteName, string applicationPoolName, string adminUserName = "", string adminPassword = "")
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var site = GetSite(serverManager, siteName);
                if (site != null)
                {
                    site.ApplicationDefaults.ApplicationPoolName = applicationPoolName;
                    if (!string.IsNullOrEmpty(adminUserName) && !string.IsNullOrEmpty(adminPassword))
                    {
                        var virtualDic = site.Applications[0].VirtualDirectories[0];
                        virtualDic.UserName = adminUserName;
                        virtualDic.Password = adminPassword;
                    }
                }
                serverManager.CommitChanges();
            }
        }

        /// <summary>
        /// 给应用程序设置应用程序池
        /// </summary>
        /// <param name="siteName">站点名称</param>
        /// <param name="appName">应用程序名称</param>
        /// <param name="applicationPoolName">引用程序池名称</param>
        /// <param name="adminUserName">连接为用户名</param>
        /// <param name="adminPassword">连接为用户密码</param>
        public static void SetApplicationApplicationPool(string siteName, string appName, string applicationPoolName, string adminUserName = "", string adminPassword = "")
        {
            using (ServerManager serverManager = new ServerManager())
            {
                var site = GetSite(serverManager, siteName);
                if (site != null)
                {
                    Application application = GetApplication(serverManager, siteName, appName);
                    if (application != null)
                    {
                        application.ApplicationPoolName = applicationPoolName;
                        if (!string.IsNullOrEmpty(adminUserName) && !string.IsNullOrEmpty(adminPassword))
                        {
                            application.VirtualDirectories[0].UserName = adminUserName;
                            application.VirtualDirectories[0].Password = adminPassword;
                        }
                    }
                }
                serverManager.CommitChanges();
            }
        }

        /// <summary>
        /// 获取指定站点的应用程序
        /// </summary>
        /// <param name="serverManager">服务器管理对象</param>
        /// <param name="siteName">站点名称</param>
        /// <param name="applicationName">应用程序名称</param>
        /// <returns></returns>
        private static Application GetApplication(ServerManager serverManager, string siteName, string applicationName)
        {
            var site = serverManager.Sites[siteName];
            if (site == null)
            {
                throw new Exception($"名称{siteName}为站点不存在！");
            }
            if (applicationName != "/")
            {
                applicationName = "/" + applicationName;
            }

            return site.Applications.Where(it => it.Path == (applicationName)).FirstOrDefault();
        }
        #endregion

        #region IIS操作
        /// <summary>
        /// 判断IIS服务器是否存在
        /// </summary>
        /// <returns></returns>
        public static bool IsIISExist()
        {
            return ExistService("W3SVC");
        }

        /// <summary>
        /// 判断IIS服务器是否存在
        /// </summary>
        /// <returns></returns>
        public static bool ExistService(string serviceName)
        {
            var services = ServiceController.GetServices();
            return services.Count(it => it.ServiceName.Equals(serviceName, StringComparison.Ordinal)) > 0;
        }

        /// <summary>
        /// 判断IIS服务器是否在运行
        /// </summary>
        /// <returns></returns>
        public static bool IsIISRunning()
        {
            var services = ServiceController.GetServices();
            return services.Count(it => it.ServiceName == "W3SVC" && it.Status == ServiceControllerStatus.Running) > 0;
        }
        #endregion

    }
}


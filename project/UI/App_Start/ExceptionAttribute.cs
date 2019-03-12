using System;
using System.Globalization;
using System.Web.Mvc;
using Infrastructure;
using log4net.Config;
using Entity.View;

namespace UI
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class ExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        public virtual void OnException(ExceptionContext filterContext)
        {
            var objUser = SessionHelper.GetSession(AppSettingsHelper.GetString("userInfoOfSession"));
            UserInfoView userInfo = new UserInfoView();
            if (objUser == null)
            {
                userInfo.Id = "";
                userInfo.Account = "未知用户";
            }
            else
            {
                userInfo = objUser as UserInfoView;
            }
            string message = string.Format(
@"当前登陆用户：{0}
ID号：{1}"
, userInfo.Account, userInfo.Id);

            //记录日志
            //待添加log4net
            XmlConfigurator.Configure();
            //string logName = DateTime.Now.ToString("yyyy-MM-dd");
            log4net.ILog log = log4net.LogManager.GetLogger("logerror");
            log.Error(message, filterContext.Exception);
            //如果是ajax请求
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = new JsonResult()
                {
                    Data = new { Success = false, Message = "系统异常，请与管理员联系" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                //抛出异常信息
                filterContext.Controller.TempData["ExceptionMessage"] = message;
                //转向
                filterContext.ExceptionHandled = true;
                filterContext.Result = new RedirectResult("/500.html");
            }
        }
    }
}
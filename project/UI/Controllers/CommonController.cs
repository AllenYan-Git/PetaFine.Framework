using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using Bll;
using Entity.Enum;

namespace UI.Controllers
{
    public class CommonController : BaseController
    {
        private readonly CommonBll _commonBll = new CommonBll();
        private readonly UserBll _userInfoBll = new UserBll();

        public ActionResult GetClientsDataJson()
        {
            var data = _commonBll.GetClientsData();

            var a = new
            {
                user = GetUser(),                
                handleType = GetHandleType()      
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private object GetUser()
        {
            var data = _userInfoBll.GetUser();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (var item in data)
            {
                var fieldItem = new
                {
                    account = item.Account,
                    name = item.Name,
                };
                dictionary.Add(item.Id, fieldItem);
            }
            return dictionary;
        }

        private object GetHandleType()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            Type t = typeof(HandleTypeEnum);
            Array arrays = Enum.GetValues(t);
            for (int i = 0; i < arrays.LongLength; i++)
            {
                HandleTypeEnum status = (HandleTypeEnum)arrays.GetValue(i);
                FieldInfo fieldInfo = status.GetType().GetField(status.ToString());
                object[] attribArray = fieldInfo.GetCustomAttributes(false);
                EnumDescriptionAttribute attrib = (EnumDescriptionAttribute)attribArray[0];
                dictionary.Add(status.GetHashCode().ToString(), attrib.Description);
            }

            return dictionary;
        }

        
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Dal;
using Dal.Interface;
using Entity.Enum;
using Entity.Model;
using Entity.Respose;

namespace Bll
{
    public class CommonBll
    {

        private readonly ICommonDal _service = new CommonDal();

        private readonly IUnitWork _unit = new UnitWork();

        public object GetClientsData()
        {
            var data = new
            {
                user = this.GetUser(),
               
            };

            return data;
        }

        private object GetUser()
        {
            var data = _unit.FindList<User>();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (var item in data)
            {
                var fieldItem = new
                {
                    name = item.Name,
                    sex = item.Sex,
                    userCount = item.Account
                };
                dictionary.Add(item.Id.ToString(), fieldItem);
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

        public string Upload()
        {
            HttpFileCollection hfc = HttpContext.Current.Request.Files;
            string tempPath = "/Upload/Temp/";
            if (!System.IO.Directory.Exists(System.Web.Hosting.HostingEnvironment.MapPath(tempPath)))
                System.IO.Directory.CreateDirectory(System.Web.Hosting.HostingEnvironment.MapPath(tempPath));
            string filePath = "";
            if (hfc.Count > 0) //文件保存 
            {
                if (hfc[0].ContentLength > 0)
                {
                    string extension = hfc[0].FileName.Substring(hfc[0].FileName.LastIndexOf("."));
                    filePath = tempPath + DateTime.Now.ToString("yyyyMMddhhmmss") + "-" +
                               hfc[0].FileName; //文件保存临时路径
                    hfc[0].SaveAs(System.Web.Hosting.HostingEnvironment.MapPath(filePath));
                }
            }

            if (filePath == "")
            {
                throw new Exception("文件不能为空");
            }
            return filePath;
        }
    }
}

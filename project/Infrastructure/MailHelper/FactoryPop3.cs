using System;

namespace Infrastructure.MailHelper
{
    /// <summary>
    /// pop3工厂创建具体的pop3对象接收邮件
    /// </summary>
    public class FactoryPop3
    {
        public String Pop3Type = "OpenPop";

        public Pop3 CreatePop3()
        {
            if (Pop3Type == "OpenPop")
            {
                return new OpenPopPop3();
            }
            else if (Pop3Type == "LumiSoft")
            {
                return new LumiSoftPop3();
            }
            else
            {
                return null;
            }
        }       
    }
}

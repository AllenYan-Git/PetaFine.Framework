using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Infrastructure.MailHelper
{
    /// <summary>
    /// 发送邮件类 的摘要说明
    /// </summary>
    public class SendMail
    {
        #region 数据成员
        //收件人地址
        private string m_To = "";
        //抄送人地址
        private string m_Cc = "";
        //密送人地址
        private string m_Bcc = "";
        //发件人地址
        private string m_From = "";
        //邮件标题
        private string m_Subject = "";
        //邮件正文
        private string m_Body = "";
        //发送服务器名或地址
        private string m_Host = "";
        //发送邮件服务器端口
        private int m_Port = 25;
        //发件人用户名
        private string m_UserName = "";
        //发件人密码
        private string m_Password = "";
        //邮件附件
        private string m_File = "";
        //是否采用Ssl加密
        private bool m_Ssl = false;
        //內容是否未HTML格式
        private bool m_IsBodyHtml = true;
        #endregion

        #region 构造函数
        /// <summary>
        /// Initializes a new instance of the <see cref="SendMail"/> class. 然後最屬性赋值
        /// </summary>
        public SendMail() { }

        /**/
        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="to">收件人地址多个用","隔开</param>
        /// <param name="from">发件人地址</param>
        /// <param name="subject">邮件标题</param>
        /// <param name="body">邮件正文</param>
        /// <param name="port">发送邮件服务器名或地址</param>
        /// <param name="host">发送邮件服务器名或地址端口</param>
        /// <param name="userName">发件人用户名</param>
        /// <param name="password">发件人密码</param>
        /// <param name="fileName">附件绝对地址多个附件以";"分割</param>
        /// <param name="ssl">是否加密发送</param>
        /// <param name="cc">抄送人地址多个地址用","隔开</param>
        /// <param name="bcc">秘送人地址多个地址用","隔开</param>
        public SendMail(string to, string from, string subject, string body, string host, int port, string userName, string password, string fileName, bool ssl, string cc = "", string bcc = "")
        {
            m_To = to;
            m_From = from;
            m_Subject = subject;
            m_Body = body;
            m_Host = host;
            m_Port = port;
            m_UserName = userName;
            m_Password = password;
            m_File = fileName;
            m_Ssl = ssl;
            m_Cc = cc;
            m_Bcc = bcc;
        }
        #endregion

        #region 数据属性
        //收件人地址
        public string TO
        {
            get
            {
                return m_To;
            }
            set
            {
                m_To = value;
            }
        }

        //发件人地址
        public string From
        {
            get
            {
                return m_From;
            }
            set
            {
                m_From = value;
            }
        }

        //邮件标题
        public string Subject
        {
            get
            {
                return m_Subject;
            }
            set
            {
                m_Subject = value;
            }
        }

        //邮件正文
        public string Body
        {
            get
            {
                return m_Body;
            }
            set
            {
                m_Body = value;
            }
        }

        //服务器地址（名）
        public string Host
        {
            get
            {
                return m_Host;
            }
            set
            {
                m_Host = value;
            }
        }

        //服务器端口
        public int Port
        {
            get
            {
                return m_Port;
            }
            set
            {
                m_Port = value;
            }
        }

        //发件人的用户名
        public string UserName
        {
            get
            {
                return m_UserName;
            }
            set
            {
                m_UserName = value;
            }
        }

        //发件人的密码
        public string Password
        {
            get
            {
                return m_Password;
            }
            set
            {
                m_Password = value;
            }
        }

        /// <summary>
        /// 邮件附件
        /// </summary>
        public string File
        {
            get
            {
                return m_File;
            }
            set
            {
                m_File = value;
            }
        }
        /// <summary>
        /// 是否加密发送
        /// </summary>
        public bool Ssl
        {
            get
            {
                return m_Ssl;
            }
            set
            {
                m_Ssl = value;
            }
        }
        /// <summary>
        /// 內容是否未HTML格式 默認為true
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is body HTML; otherwise, <c>false</c>.
        /// </value>
        public bool IsBodyHtml
        {
            get
            {
                return m_IsBodyHtml;
            }
            set
            {
                m_IsBodyHtml = value;
            }
        }

        public string CC
        {
            get
            {
                return m_Cc;
            }
            set
            {
                m_Cc = value;
            }
        }
        public string BCC
        {
            get
            {
                return m_Bcc;
            }
            set
            {
                m_Bcc = value;
            }
        }
        #endregion

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <returns>发送是否成功</returns>
        public bool Send()
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(m_From);
                msg.Subject = m_Subject;
                //邮件正文
                msg.Body = m_Body;
                char[] ch = { ',' };
                if (!string.IsNullOrEmpty(m_To))
                {
                    string[] address = m_To.Split(ch, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < address.Length; i++)
                    {
                        MailAddress toAddress = new MailAddress(address[i]);
                        msg.To.Add(toAddress);
                    }
                }
                if (!string.IsNullOrEmpty(m_Cc))
                {
                    string[] addressCc = m_Cc.Split(ch, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < addressCc.Length; i++)
                    {
                        MailAddress toAddress = new MailAddress(addressCc[i]);
                        msg.CC.Add(toAddress);
                    }
                }
                if (!string.IsNullOrEmpty(m_Bcc))
                {
                    string[] addressBcc = m_Bcc.Split(ch, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < addressBcc.Length; i++)
                    {
                        MailAddress toAddress = new MailAddress(addressBcc[i]);
                        msg.Bcc.Add(toAddress);
                    }
                }
                //內容是否未HTML格式
                msg.IsBodyHtml = m_IsBodyHtml;

                //获取所有邮件附件
                char[] cr = { ';' };
                string[] file = m_File.Split(cr);
                for (int n = 0; n < file.Length; n++)
                {
                    if (file[n] != "")
                    {
                        //附件对象
                        Attachment data = new Attachment(file[n], MediaTypeNames.Application.Octet);
                        //附件资料
                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = System.IO.File.GetCreationTime(file[n]);
                        disposition.ModificationDate = System.IO.File.GetLastWriteTime(file[n]);
                        disposition.ReadDate = System.IO.File.GetLastAccessTime(file[n]);
                        //加入邮件附件
                        msg.Attachments.Add(data);
                    }
                }

                //使用简单邮件传输协议来传送邮件
                SmtpClient sendMail = new SmtpClient();
                //发送邮件的服务器名或地址
                sendMail.Host = m_Host;
                sendMail.Port = m_Port;
                //验证发件人的身份
                sendMail.Credentials = new NetworkCredential(m_UserName, m_Password);
                //处理待发送邮件的方法
                sendMail.DeliveryMethod = SmtpDeliveryMethod.Network;
                sendMail.EnableSsl = m_Ssl;
                //发送邮件
                sendMail.Send(msg);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("EMail发送失败" + ex);
            }
        }
    }

}

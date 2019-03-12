using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenPop.Mime;
using OpenPop.Mime.Header;
using OpenPop.Pop3;

namespace Infrastructure.MailHelper
{
    /// <summary>
    /// OpenPopPop3
    /// </summary>
    public class OpenPopPop3 : Pop3
    {

        #region 窗体变量

        /// <summary>
        /// 是否存在错误
        /// </summary>
        public override Boolean ExitsError { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public override String ErrorMessage { get; set; }
        /// <summary>
        /// POP3端口号
        /// </summary>
        public override Int32 Pop3Port { set; get; }
        /// <summary>
        /// POP3地址
        /// </summary>
        public override String Pop3Address { set; get; }
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public override String EmailAddress { set; get; }
        /// <summary>
        /// 邮箱密码
        /// </summary>
        public override String EmailPassword { set; get; }

        #endregion

        #region 私有变量
        private Pop3Client _pop3Client;

        // private List<POP3_ClientMessage> pop3MessageList = new List<POP3_ClientMessage>();

        private Int32 _mailTotalCount;
        #endregion

        #region 构造函数
        public OpenPopPop3() { }
        #endregion

        #region 链接至服务器并读取邮件集合
        /// <summary>
        /// 链接至服务器并读取邮件集合
        /// </summary>
        public override Boolean Authenticate()
        {
            try
            {
                _pop3Client = new Pop3Client();
                if (_pop3Client.Connected)
                    _pop3Client.Disconnect();
                _pop3Client.Connect(Pop3Address, Pop3Port, Pop3Address.Contains("qq.com"));
                _pop3Client.Authenticate(EmailAddress, EmailPassword, AuthenticationMethod.UsernameAndPassword);
                _mailTotalCount = _pop3Client.GetMessageCount();

                return ExitsError = true;
            }
            catch (Exception ex) { ErrorMessage = ex.Message; return ExitsError = false; }
        }
        #endregion

        #region 获取邮件数量
        /// <summary>
        /// 获取邮件数量
        /// </summary>
        /// <returns></returns>
        public override Int32 GetMailCount()
        {
            return _mailTotalCount;
        }
        #endregion

        public Message message { get; set; }
        /// <summary>
        /// 一次性获取邮件
        /// </summary>
        /// <param name="mailIndex"></param>
        public override void GetMessage(int mailIndex)
        {
            message = _pop3Client.GetMessage(mailIndex);
        }

        #region 获取收件人
        /// <summary>
        /// 获取收件人
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public override String GetToMialAddress(Int32 mailIndex)
        {
            try
            {
                //List<RfcMailAddress> list = pop3Client.GetMessageHeaders(mailIndex).To;
                List<RfcMailAddress> list = message.Headers.To;
                StringBuilder sb = new StringBuilder();
                list.ForEach(a => sb.Append(a.Address + ","));
                if (sb.ToString().Length <= 0)
                    return "";
                return sb.ToString().Substring(0, sb.ToString().Length - 1);
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region 抄送人
        /// <summary>
        /// 获取收件人
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public override String GetCcMialAddress(Int32 mailIndex)
        {
            try
            {
                //List<RfcMailAddress> list = pop3Client.GetMessageHeaders(mailIndex).Cc;
                List<RfcMailAddress> list = message.Headers.Cc;
                StringBuilder sb = new StringBuilder();
                list.ForEach(a => sb.Append(a.Address + ","));
                if (sb.ToString().Length <= 0)
                    return "";
                return sb.ToString().Substring(0, sb.ToString().Length - 1);

            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region 密送人
        /// <summary>
        /// 获取收件人
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public override String GetBccMialAddress(Int32 mailIndex)
        {
            try
            {
                //List<RfcMailAddress> list = pop3Client.GetMessageHeaders(mailIndex).Bcc;
                List<RfcMailAddress> list = message.Headers.Bcc;
                StringBuilder sb = new StringBuilder();
                list.ForEach(a => sb.Append(a.Address + ","));
                if (sb.ToString().Length <= 0)
                    return "";
                return sb.ToString().Substring(0, sb.ToString().Length - 1);
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region 获取发件人邮箱
        /// <summary>
        /// 获取发件人
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public override String GetSendMialAddress(Int32 mailIndex)
        {
            //RfcMailAddress address = pop3Client.GetMessageHeaders(mailIndex).From;
            RfcMailAddress address = message.Headers.From;
            return address.Address;
        }
        #endregion

        #region 获取发件人姓名
        /// <summary>
        /// 获取发件人
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public override String GetSenderName(Int32 mailIndex)
        {
            // RfcMailAddress address = pop3Client.GetMessageHeaders(mailIndex).From;
            RfcMailAddress address = message.Headers.From;
            return address.DisplayName;
        }
        #endregion

        #region 获取邮件的UID
        /// <summary>
        /// 获取邮件的UID
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public override String GetMailUid(Int32 mailIndex)
        {
            // return pop3Client.GetMessageUid(mailIndex);
            return message.Headers.MessageId;
        }
        #endregion

        #region 获取邮件的主题
        /// <summary>
        /// 获取邮件的主题
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public override String GetMailSubject(Int32 mailIndex)
        {
            try
            {
                // return pop3Client.GetMessageHeaders(mailIndex).Subject;
                return message.Headers.Subject;
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion

        #region 获取邮件发送时间
        /// <summary>
        /// 获取邮件发送时间
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public override DateTime GetMailSendDate(Int32 mailIndex)
        {
            // return pop3Client.GetMessageHeaders(mailIndex).DateSent;
            return message.Headers.DateSent;
        }
        #endregion

        #region 获取邮件正文
        /// <summary>
        /// 获取邮件正文
        /// </summary>
        /// <param name="mailIndex">邮件顺序</param>
        /// <returns></returns>
        public override String GetMailBodyAsText(Int32 mailIndex)
        {
            try
            {
                //Message message = pop3Client.GetMessage(mailIndex);

                MessagePart messagePart = message.MessagePart;
                string body = " ";
                if (messagePart.IsText)
                {
                    body = messagePart.GetBodyAsText();
                }
                else if (messagePart.IsMultiPart)
                {
                    MessagePart plainTextPart = message.FindFirstHtmlVersion();
                    if (plainTextPart != null)
                    {
                        // The message had a text/plain version - show that one
                        body = plainTextPart.GetBodyAsText();
                    }
                    else
                    {
                        // Try to find a body to show in some of the other text versions
                        List<MessagePart> textVersions = message.FindAllTextVersions();
                        if (textVersions.Count >= 1)
                            body = textVersions[0].GetBodyAsText();
                        else
                            body = "";
                    }
                }
                return body;
            }
            catch (Exception ex) { ErrorMessage = ex.Message; return ""; }
        }
        #endregion

        #region 获取邮件的附件
        public override Boolean GetMailAttachment(Int32 mailIndex, String receiveBackpath)
        {
            if (mailIndex == 0)
                return false;
            else if (mailIndex > _mailTotalCount)
                return false;
            else
            {
                try
                {
                    Message message = _pop3Client.GetMessage(mailIndex);
                    //邮件的全部附件.
                    List<MessagePart> attachments = message.FindAllAttachments();
                    foreach (MessagePart attachment in attachments)
                    {
                        string fileName = attachment.FileName;
                        string fileFullName = receiveBackpath + "\\" + fileName;
                        FileInfo fileInfo = new FileInfo(fileFullName);
                        if (fileInfo.Exists) fileInfo.Delete();
                        attachment.Save(fileInfo);
                    }
                    // pop3Client.DeleteMessage(mailIndex);
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    return false;
                }
            }
        }
        #endregion

        #region 获取邮件的附件返回文件名和文件byte 集合
        /// <summary>
        /// 获取邮件的附件返回文件名和文件byte 集合
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public override List<ReceiveAccessory> GetMailAttachment(Int32 mailIndex)
        {
            if (mailIndex == 0)
                return null;
            else if (mailIndex > _mailTotalCount)
                return null;
            else
            {
                List<ReceiveAccessory> list = new List<ReceiveAccessory>();
                try
                {
                    //Message message = pop3Client.GetMessage(mailIndex);
                    //邮件的全部附件.
                    List<MessagePart> attachments = message.FindAllAttachments();
                    foreach (MessagePart attachment in attachments)
                    {
                        string fileName = attachment.FileName;

                        list.Add(new ReceiveAccessory() { FileName = fileName, Boyd = attachment.Body, Length = attachment.Body.Length });
                    }
                    // pop3Client.DeleteMessage(mailIndex);
                    return list;
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    return list;
                }
            }
        }
        #endregion

        #region 删除邮件
        /// <summary>
        /// 删除邮件
        /// </summary>
        /// <param name="mailIndex"></param>
        public override void DeleteMail(Int32 mailIndex)
        {
            _pop3Client.DeleteMessage(mailIndex);
        }
        #endregion

        #region 关闭邮件服务器
        public override void Pop3Close()
        {
            _pop3Client.Disconnect();
            _pop3Client.Dispose();
        }
        #endregion


    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using LumiSoft.Net.Mail;
using LumiSoft.Net.MIME;
using LumiSoft.Net.POP3.Client;

namespace Infrastructure.MailHelper
{
    /// <summary>
    /// LumiSoftPop3 
    /// </summary>
    public class LumiSoftPop3 : Pop3
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
        private POP3_Client _pop3Client;

        private readonly List<POP3_ClientMessage> _pop3MessageList = new List<POP3_ClientMessage>();

        private Int32 _mailTotalCount;
        #endregion

        #region 构造函数
        public LumiSoftPop3() { }
        #endregion

        #region 链接至服务器并读取邮件集合
        /// <summary>
        /// 链接至服务器并读取邮件集合
        /// </summary>
        public override Boolean Authenticate()
        {
            try
            {
                _pop3Client = new POP3_Client();
                _pop3Client.Connect(Pop3Address, Pop3Port);//通过POP3地址，端口连接服务器。
                _pop3Client.Login(EmailAddress, EmailPassword);//登录之后验证用户的合法性

                foreach (POP3_ClientMessage pop3ClientMessage in _pop3Client.Messages)
                {
                    _pop3MessageList.Add(pop3ClientMessage);
                }

                _mailTotalCount = _pop3MessageList.Count;

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
            return _pop3Client.Messages.Count;
        }
        #endregion

        #region 获取发件人
        /// <summary>
        /// 获取发件人
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public override String GetSendMialAddress(Int32 mailIndex)
        {
            if (mailIndex == 0)
                return "";
            else if (mailIndex > _mailTotalCount)
                return "";

            LumiSoft.Net.Mail.Mail_Message mMessage = Mail_Message.ParseFromByte(_pop3MessageList[mailIndex - 1].HeaderToByte());
            if (mMessage.From != null)
            {
                return mMessage.From[0].Address;
            }
            return "";
        }
        #endregion
        #region 获取发件人
        /// <summary>
        /// 获取发件人
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public override String GetSenderName(Int32 mailIndex)
        {
            if (mailIndex == 0)
                return "";
            else if (mailIndex > _mailTotalCount)
                return "";

            LumiSoft.Net.Mail.Mail_Message mMessage = Mail_Message.ParseFromByte(_pop3MessageList[mailIndex - 1].HeaderToByte());
            if (mMessage.From != null)
            {
                return mMessage.From[0].DisplayName;
            }
            return "";
        }
        #endregion

        #region 获取邮件的主题
        /// <summary>
        /// 获取邮件的主题
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public override String GetMailUid(Int32 mailIndex)
        {
            LumiSoft.Net.Mail.Mail_Message mMessage = Mail_Message.ParseFromByte(_pop3MessageList[mailIndex - 1].HeaderToByte());
            if (mMessage.From != null)
            {
                return mMessage.Subject;
            }
            return "";
        }
        #endregion

        #region 获取邮件的UID
        /// <summary>
        /// 获取邮件的UID
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public override String GetMailSubject(Int32 mailIndex)
        {
            return _pop3MessageList[mailIndex - 1].UID;
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
            LumiSoft.Net.Mail.Mail_Message mMessage = Mail_Message.ParseFromByte(_pop3MessageList[mailIndex - 1].HeaderToByte());
            if (mMessage.From != null)
            {
                return mMessage.Date;
            }
            return DateTime.MinValue;
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
            LumiSoft.Net.Mail.Mail_Message mMessage = Mail_Message.ParseFromByte(_pop3MessageList[mailIndex - 1].HeaderToByte());
            if (mMessage.From != null)
            {
                return mMessage.BodyText;
                //return MMessage.BodyHtmlText;
            }
            return "";
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
                    byte[] messageBytes = _pop3MessageList[mailIndex - 1].MessageToByte();
                    Mail_Message mMessage = Mail_Message.ParseFromByte(messageBytes);
                    if (mMessage == null) return false;
                    //LumiSoft.Net.Mail.Mail_Message MMessage = Mail_Message.ParseFromByte(pop3MessageList[mailIndex - 1].HeaderToByte());
                    foreach (MIME_Entity entity in mMessage.GetAttachments(true, true))
                    {
                        if (entity.ContentDisposition != null &&
                                       entity.ContentDisposition.Param_FileName != null)
                        {
                            String fileName = entity.ContentDisposition.Param_FileName;
                            String fileFullName = receiveBackpath + "\\" + fileName;
                            //FileInfo fileInfo = new FileInfo(fileFullName);
                            //if (fileInfo.Exists) fileInfo.Delete();
                            MIME_b_SinglepartBase byteObj = (MIME_b_SinglepartBase)entity.Body;
                            if (byteObj != null)
                            {
                                FileInfo fileInfo = new FileInfo(fileFullName);
                                FileStream fs = null;   //声明一个文件流对象.
                                fs = new FileStream(fileInfo.FullName, FileMode.Create);
                                fs.Write(byteObj.Data, 0, byteObj.Data.Length);
                                fs.Close();
                                //FileUtil.CreateFile(filePath, byteObj.Data);
                                //fileSize = byteObj.Data.Length;
                                //entity.ContentDisposition.DispositionType == MIME_DispositionTypes.Attachment
                                return true;
                            }
                        }
                    }
                    ErrorMessage = "获取附件失败，请重试！";
                    return false;
                }
                catch (Exception ex) { ErrorMessage = ex.Message; return false; }
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
            foreach (POP3_ClientMessage pop3ClientMessage in _pop3Client.Messages)
            {
                if (pop3ClientMessage.SequenceNumber == mailIndex)
                {
                    pop3ClientMessage.MarkForDeletion();
                }
            }
        }
        #endregion

        #region 关闭邮件服务器
        public override void Pop3Close()
        {
            _pop3Client.Disconnect();
            _pop3Client.Dispose();
        }
        #endregion

        public override string GetToMialAddress(int mailIndex)
        {
            throw new NotImplementedException();
        }

        public override string GetCcMialAddress(int mailIndex)
        {
            throw new NotImplementedException();
        }

        public override string GetBccMialAddress(int mailIndex)
        {
            throw new NotImplementedException();
        }

        public override List<ReceiveAccessory> GetMailAttachment(int mailIndex)
        {
            throw new NotImplementedException();
        }

        public override void GetMessage(int mailIndex)
        {
            throw new NotImplementedException();
        }
    }
}

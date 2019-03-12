using System;
using System.Collections.Generic;

namespace Infrastructure.MailHelper
{
    /// <summary>
    /// Pop3抽象类
    /// </summary>
    public abstract class Pop3
    {
        #region 窗体变量

        /// <summary>
        /// 是否存在错误
        /// </summary>
        public abstract Boolean ExitsError { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public abstract String ErrorMessage { get; set; }
        /// <summary>
        /// POP3端口号
        /// </summary>
        public abstract Int32 Pop3Port { set; get; }
        /// <summary>
        /// POP3地址
        /// </summary>
        public abstract String Pop3Address { set; get; }
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public abstract String EmailAddress { set; get; }
        /// <summary>
        /// 邮箱密码
        /// </summary>
        public abstract String EmailPassword { set; get; }

        #endregion

        #region 链接至服务器并读取邮件集合
        /// <summary>
        /// 链接至服务器并读取邮件集合
        /// </summary>
        public abstract Boolean Authenticate();

        #endregion

        #region 获取邮件数量
        /// <summary>
        /// 获取邮件数量
        /// </summary>
        /// <returns></returns>
        public abstract Int32 GetMailCount();

        #endregion

        /// <summary>
        /// 一次性获取邮件
        /// </summary>
        /// <param name="mailIndex"></param>
        public abstract void GetMessage(int mailIndex);


        #region 获取收件人
        /// <summary>
        /// 获取收件人
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public abstract String GetSendMialAddress(Int32 mailIndex);
        #endregion

        #region 抄送人
        /// <summary>
        /// 获取收件人
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public abstract String GetToMialAddress(Int32 mailIndex);
        #endregion

        #region 密送人
        /// <summary>
        /// 获取收件人
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public abstract String GetCcMialAddress(Int32 mailIndex);
        #endregion

        #region 获取发件人
        /// <summary>
        /// 获取发件人
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public abstract String GetBccMialAddress(Int32 mailIndex);

        #endregion

        #region 获取发件人名字
        /// <summary>
        /// 获取发件人
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public abstract String GetSenderName(Int32 mailIndex);

        #endregion

        #region 取邮件的UID
        /// <summary>
        /// 取邮件的UID
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public abstract String GetMailUid(Int32 mailIndex);

        #endregion

        #region 获取邮件的主题
        /// <summary>
        /// 获取邮件的主题
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public abstract String GetMailSubject(Int32 mailIndex);
        #endregion

        #region 获取邮件发送时间
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public abstract DateTime GetMailSendDate(Int32 mailIndex);
        #endregion

        #region 获取邮件正文
        /// <summary>
        /// 获取邮件正文
        /// </summary>
        /// <param name="mailIndex">邮件顺序</param>
        /// <returns></returns>
        public abstract String GetMailBodyAsText(Int32 mailIndex);
        #endregion

        #region 获取邮件的附件
        public abstract Boolean GetMailAttachment(Int32 mailIndex, String receiveBackpath);

        #endregion

        #region 获取邮件的附件返回文件名和文件byte 集合
        /// <summary>
        /// 获取邮件的附件返回文件名和文件byte 集合
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns></returns>
        public abstract List<ReceiveAccessory> GetMailAttachment(Int32 mailIndex);
        #endregion

        #region 删除邮件
        /// <summary>
        /// 删除邮件
        /// </summary>
        /// <param name="mailIndex"></param>
        public abstract void DeleteMail(Int32 mailIndex);
        #endregion

        #region 关闭邮件服务器
        public abstract void Pop3Close();
        #endregion
    }
}

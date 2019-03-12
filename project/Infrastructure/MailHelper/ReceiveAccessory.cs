namespace Infrastructure.MailHelper
{
    /// <summary>
    /// 邮件中接收的附件对象
    /// </summary>
    public class ReceiveAccessory
    {
        /// <summary>
        /// 附件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 附件长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 附件主体
        /// </summary>
        public byte[] Boyd { get; set; }

    }
}

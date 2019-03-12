namespace Entity.Enum
{
    public enum HandleTypeEnum
    {

        /// <summary>
        /// 发起
        /// </summary>
        [EnumDescription("发起")]
        Submit = 1,

        /// <summary>
        /// 通过
        /// </summary>
        [EnumDescription("通过")]
        Pass = 2,

        /// <summary>
        /// 驳回
        /// </summary>
        [EnumDescription("驳回")]
        Return = 3,

        /// <summary>
        /// 移交
        /// </summary>
        [EnumDescription("移交")]
        Change = 4,

        /// <summary>
        /// 修改
        /// </summary>
        [EnumDescription("修改")]
        Modify = 5,

        /// <summary>
        /// 撤回
        /// </summary>
        [EnumDescription("撤回")]
        Recall = 6,

        /// <summary>
        /// 取消
        /// </summary>
        [EnumDescription("取消")]
        Cancel = 7,

        /// <summary>
        /// 不通过
        /// </summary>
        [EnumDescription("不通过")]
        UnPass = 8
    }
}

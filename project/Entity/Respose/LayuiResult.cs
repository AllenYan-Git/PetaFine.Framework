namespace Entity.Respose
{
    public class LayuiResult
    {
        public int code { get; set; }

        public string msg { get; set; }

        public LayuiResult()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg">消息</param>
        public LayuiResult(int code, string msg)
        {
            this.code = code;
            this.msg = msg;
        }
    }

    public class LayuiSingleResult<T> : LayuiResult
    {
        public T data { get; set; }

        public LayuiSingleResult(int code, string msg)
        {
            this.code = code;
            this.msg = msg;
        }

        public LayuiSingleResult(int code, string msg, T data)
            : this(code, msg)
        {
            this.data = data;
        }
    }

    public class LayuiFileData
    {
        public string title { get; set; }

        public string src { get; set; }
    }
}

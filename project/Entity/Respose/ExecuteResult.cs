using System.Collections.Generic;

namespace Entity.Respose
{
    public class ExecuteResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExecuteResult()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="success">是否成功</param>
        /// <param name="message">消息</param>
        public ExecuteResult(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }


        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 返回实体执行结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleExecuteResult<T> : ExecuteResult
    {
        public T Data { get; set; }

        public SingleExecuteResult(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
        }

        public SingleExecuteResult(bool success, string messeage, T data)
            : this(success, messeage)
        {
            this.Data = data;
        }
    }

    /// <summary>
    /// 返回列表执行结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListExecuteResult<T> : ExecuteResult
    {
        public List<T> Data { get; set; }

        public ListExecuteResult()
        {
            this.Data = new List<T>();
        }

        public ListExecuteResult(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
        }

        public ListExecuteResult(bool success, string messeage, List<T> data)
            : this(success, messeage)
        {
            this.Data = data;
        }
    }
}

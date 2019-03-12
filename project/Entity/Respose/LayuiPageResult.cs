using System.Collections.Generic;

namespace Entity.Respose
{
    public class LayuiPageResult<T>
    {
        public List<T> data { get; set; }

        public int code { get; set; }

        public int count { get; set; }

        public string msg { get; set; }

        public LayuiPageResult(int code, int count, string msg, List<T> pageData)
        {
            this.code = code;
            this.count = count;
            this.msg = msg;
            data = pageData;
        }
    }
}

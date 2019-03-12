using System;

namespace Entity.Request
{
    /// <summary>
    /// layui table 参数
    /// </summary>
    public class PageParam 
    {
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }

        public string searchKey { get; set; }

        public PageParam()
        {
            page = 1;
            limit = 10;            
        }
    }
}

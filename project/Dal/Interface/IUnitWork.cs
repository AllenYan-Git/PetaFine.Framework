using System.Collections.Generic;

namespace Dal.Interface
{
    /// <summary>
    /// 工作单元接口
    /// </summary>
    public interface IUnitWork
    {
        object Insert<T>(T entity) where T: class;
        bool Insert<T>(List<T> entities) where T : class;
        int Update<T>(T entity) where T : class;
        int Delete<T>(T entity) where T : class;
        int Delete<T>(object id) where T : class;
        int LogicDelete<T>(T entity) where T : class;
        bool IsExists<T>(string sqlWhere, params object[] args) where T : class;
        bool IsExists<T>(object id) where T : class;
        T FindEntity<T>(object id) where T : class;
        T FindEntity<T>(string where, params object[] args) where T : class;
        List<T> FindList<T>() where T : class;
        List<T> FindList<T>(string sqlWhere, params object[] args) where T : class;
        List<T> PageList<T>(int pageIndex, int pageSize,string sqlWhere, out int total, params object[] args) where T : class;
        List<T> PageList<T>(int pageIndex, int pageSize, out int total) where T : class;

    }
}

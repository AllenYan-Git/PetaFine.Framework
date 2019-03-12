using System.Collections.Generic;

namespace Dal.Interface
{
    public interface IDal<T> where T : class
    {
        object Insert(T entity);
        bool Insert(List<T> entities);
        int Update(T entity);
        int Delete(T entity);
        int Delete(object id);
        int LogicDelete(T entity);
        bool IsExists(string sqlWhere, params object[] args);
        bool IsExists(object id);
        T FindEntity(object id);
        T FindEntity(string where, params object[] args);
        List<T> FindList();
        List<T> FindList(string sqlWhere, params object[] args);
        List<T> PageList(int pageIndex, int pageSize, string sqlWhere, out int total, params object[] args);
        List<T> PageList(int pageIndex, int pageSize, out int total);

    }
}

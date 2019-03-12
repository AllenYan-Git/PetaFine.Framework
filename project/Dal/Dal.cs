using System.Collections.Generic;
using Dal.DbContext;
using Dal.Interface;

namespace Dal
{
    public class Dal<T> : IDal<T> where T : class, new()
    {
        public object Insert(T entity)
        {
            return DbProvider.Insert(entity);
        }

        public bool Insert(List<T> entities)
        {
            using (var db = new PetaDbContext())
            {
                try
                {
                    db.BeginTransaction();
                    foreach (var item in entities)
                    {
                        db.Insert(item);
                    }
                    db.CompleteTransaction();
                }
                catch
                {
                    db.AbortTransaction();
                    throw;
                }
            }
            return true;
        }

        public int Update(T entity)
        {
            return DbProvider.Update(entity);
        }

        public int Delete(T entity)
        {
            return DbProvider.Delete(entity);
        }

        public int Delete(object id)
        {
            using (var db = new PetaDbContext())
            {
                return db.Delete<T>(id);
            }
        }

        public int LogicDelete(T entity)
        {
            using (var db = new PetaDbContext())
            {
                return db.Update(entity, new List<string> { "recordstatus" });
            }
        }

        public bool IsExists(object id)
        {
            using (var db = new PetaDbContext())
            {
                return db.Exists<T>(id);
            }
        }

        public bool IsExists(string sqlWhere, params object[] args)
        {
            using (var db = new PetaDbContext())
            {
                return db.Exists<T>(sqlWhere, args);
            }
        }

        public T FindEntity(object id)
        {
            return DbProvider.SingleOrDefault<T>(id);
        }

        public T FindEntity(string sqlWhere, params object[] args)
        {
            return DbProvider.FirstOrDefault<T>(sqlWhere, args);
        }

        public List<T> FindList()
        {
            return DbProvider.Fetch<T>();
        }

        public List<T> FindList(string sqlWhere, params object[] args)
        {
            return DbProvider.Fetch<T>(sqlWhere, args);
        }

        public List<T> PageList(int pageIndex, int pageSize, string sqlWhere, out int total, params object[] args)
        {
            return DbProvider.PageList<T>(pageIndex, pageSize, sqlWhere, out total, args);
        }

        public List<T> PageList(int pageIndex, int pageSize, out int total)
        {
            return DbProvider.PageList<T>(pageIndex, pageSize, "", out total);
        }
    }
}

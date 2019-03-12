using System;
using System.Collections.Generic;
using Dal.DbContext;
using Dal.Interface;

namespace Dal
{
    public class UnitWork : IUnitWork
    {
        public object Insert<T>(T entity) where T : class
        {
            return DbProvider.Insert(entity);
        }

        public bool Insert<T>(List<T> entities) where T : class
        {
            using (var db = new PetaDbContext())
            {
                try
                {
                    db.BeginTransaction();
                    foreach (var entity in entities)
                    {
                        db.Insert(entity);
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

        public int Update<T>(T entity) where T : class
        {
            return DbProvider.Update(entity);
        }

        public int Delete<T>(T entity) where T : class
        {
            return DbProvider.Delete(entity);
        }

        public int Delete<T>(object id) where T : class
        {
            using (var db = new PetaDbContext())
            {
                return db.Delete<T>(id);
            }
        }

        public int LogicDelete<T>(T entity) where T : class
        {
            using (var db = new PetaDbContext())
            {
                return db.Update(entity, new List<string> { "recordstatus" });
            }
        }

        public bool IsExists<T>(string sqlWhere, params object[] args) where T : class
        {
            using (var db = new PetaDbContext())
            {
                return db.Exists<T>(sqlWhere, args);
            }
        }

        public bool IsExists<T>(object id) where T : class
        {
            using (var db = new PetaDbContext())
            {
                return db.Exists<T>(id);
            }
        }

        public T FindEntity<T>(object id) where T : class
        {
            return DbProvider.SingleOrDefault<T>(id);
        }

        public T FindEntity<T>(string where, params object[] args) where T : class
        {
            return DbProvider.FirstOrDefault<T>(where, args);
        }

        public List<T> FindList<T>() where T : class
        {
            return DbProvider.Fetch<T>();
        }

        public List<T> FindList<T>(string sqlWhere, params object[] args) where T : class
        {
            return DbProvider.Fetch<T>(sqlWhere, args);
        }

        public List<T> PageList<T>(int pageIndex, int pageSize, out int total) where T : class
        {
            return DbProvider.PageList<T>(pageIndex, pageSize, "", out total);
        }

        public List<T> PageList<T>(int pageIndex, int pageSize, string sqlWhere, out int total, params object[] args) where T : class
        {
            return DbProvider.PageList<T>(pageIndex, pageSize, sqlWhere, out total, args);
        }
    }
}

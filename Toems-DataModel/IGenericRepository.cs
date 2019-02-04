using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Toems_DataModel
{
    public interface IGenericRepository<TEntity>
    {
        string Count(Expression<Func<TEntity, bool>> filter = null);

        void Delete(object id);

        void DeleteRange(Expression<Func<TEntity, bool>> filter = null);
        void ExecuteRawSql(string query, params object[] parameters);

        bool Exists(Expression<Func<TEntity, bool>> filter = null);

        List<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        TEntity GetById(object id);

        TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        void Insert(TEntity entity);

        void InsertRange(List<TEntity> entity);

        void DeleteRange(List<TEntity> entity);

        void Update(TEntity entity, object id);
    }
}
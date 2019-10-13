using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace MessengerClientDB.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;

        protected readonly DbSet<TEntity> Set;

        public Repository(DbContext context)
        {
            Set = context.Set<TEntity>();

            Context = context;  // Used by children.
        }

        public TEntity Get(int id)
        {
            return Set.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Set.ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.Where(predicate);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.SingleOrDefault(predicate);
        }

        public void Add(TEntity entity)
        {
            Set.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            Set.AddRange(entities);
        }

        public void Remove(TEntity entity)
        {
            Set.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            Set.RemoveRange(entities);
        }
    }
}
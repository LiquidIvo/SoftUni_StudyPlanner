using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data.Repositories.Interfaces;
namespace StudyPlanner.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext context;
        protected readonly DbSet<T> dbSet; 

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public IQueryable<T> All() => dbSet;
        public async Task<T?> GetByIdAsync(object id) => await dbSet.FindAsync(id);
        public async Task AddAsync(T entity) => await dbSet.AddAsync(entity);
        public void Update(T entity) => dbSet.Update(entity);
        public void Delete(T entity) => dbSet.Remove(entity);
        public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
    }

}

using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace AdministracionActivosSobrantes.EntityFramework.Repositories
{
    public abstract class AdministracionActivosSobrantesRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<AdministracionActivosSobrantesDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected AdministracionActivosSobrantesRepositoryBase(IDbContextProvider<AdministracionActivosSobrantesDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class AdministracionActivosSobrantesRepositoryBase<TEntity> : AdministracionActivosSobrantesRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected AdministracionActivosSobrantesRepositoryBase(IDbContextProvider<AdministracionActivosSobrantesDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}

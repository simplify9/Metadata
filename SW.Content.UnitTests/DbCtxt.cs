using Microsoft.EntityFrameworkCore;
using SW.Content.Search.EF;
using System.Threading;
using System.Threading.Tasks;

namespace SW.Content.UnitTests
{
    public class DbCtxt:DbContext
    {
        public DbCtxt(DbContextOptions<DbCtxt> options) : base(options)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SearchExtensions.AddContentSearchIndex(modelBuilder);


        

        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await base.SaveChangesAsync(cancellationToken);

           
            return result;
        }



        public override int SaveChanges()
        {
            int result = base.SaveChanges();

            return result;

        }
    }


}
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace SW.Content.UnitTests
{
    class TestStartup
    {
        readonly IConfiguration configuration;

        public TestStartup(IConfiguration configuration)
        {
            this.configuration = configuration;




        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DbContext, DbCtxt>(c =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                c.UseSqlite(connection);
            },
         ServiceLifetime.Scoped,
         ServiceLifetime.Singleton);

        }


    }
}

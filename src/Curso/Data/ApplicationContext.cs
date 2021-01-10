using CursoEFCore.Data.Configurations;
using CursoEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace CursoEFCore.Data
{
    public class ApplicationContext : DbContext
    {
        private static readonly ILoggerFactory _logger = LoggerFactory.Create(p => p.AddConsole());

        public DbSet<Pedido> Pedidos { get; set; }

        public DbSet<Produto> Produtos { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(_logger)
                .EnableSensitiveDataLogging() // habilita os parametros das instrucoes sql
                .UseSqlServer("Data source=localhost; Initial Catalog=CursoEFCore; User ID=sa; Password=xxxxxxxxxxxx;",
                 p => p.EnableRetryOnFailure(
                     maxRetryCount: 2,
                     maxRetryDelay: TimeSpan.FromSeconds(5),
                     errorNumbersToAdd: null)
                 .MigrationsHistoryTable("NameTableMigrations", "dbo"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new ClienteConfiguration());

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
            MapearPropriedadesEsquecidas(modelBuilder);
            //base.OnModelCreating(modelBuilder);
        }

        private void MapearPropriedadesEsquecidas(ModelBuilder modelBuilder)
        {
            foreach (var item in modelBuilder.Model.GetEntityTypes())
            {
                var prop = item.GetProperties().Where(c => c.ClrType == typeof(string));

                foreach (var itemProp in prop)
                {
                    if (string.IsNullOrEmpty(itemProp.GetColumnType())
                        && !itemProp.GetMaxLength().HasValue)
                    {
                        //itemProp.SetMaxLength(100);
                        itemProp.SetColumnType("VARCHAR(100)");
                    }
                }
            }
        }
    }
}
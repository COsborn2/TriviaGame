using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using IntelliTect.Coalesce;
using TriviaGame.Data.Models;

namespace TriviaGame.Data
{
    [Coalesce]
    public class AppDbContext : DbContext
    {
        public DbSet<TriviaBoard> TriviaBoards { get; set; }
        
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Remove cascading deletes.
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        /// <summary>
        /// Migrates the database and sets up items that need to be set up from scratch.
        /// </summary>
        public void Initialize()
        {
            try
            {
                Database.Migrate();
            }
            catch (InvalidOperationException e) when (e.Message == "No service for type 'Microsoft.EntityFrameworkCore.Migrations.IMigrator' has been registered.")
            {
                // this exception is expected when using an InMemory database
            }
        }
    }
}

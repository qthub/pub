using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using qtee.ServerApp;
using Qtee.Data;

namespace Qtee.ServerApp.ContextModels
{
    public class QteeDataContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<QModuleItem> QModuleItems { get; set; }
        public DbSet<QModuleCollection> QModuleCollections { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(Config.DbConnectionForApplication);
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Blog>().HasMany(s => s.Posts).WithOne(s => s.Blog);
        //    modelBuilder.Entity<QModuleCollection>().HasMany(s => s.CollectionItems).WithOne(s => s.Collection);
        //}
    }

    public class Blog
    {
        public Blog()
        {
            Posts = new List<Post>();
        }
        [Key] public long Id { get; set; }

        public string Url { get; set; }
        public int Rating { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }

    public class Post
    {
        [Key] public long Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }


        public long BlogId { get; set; }

        [ForeignKey("BlogId")] public virtual Blog Blog { get; set; }
    }
}
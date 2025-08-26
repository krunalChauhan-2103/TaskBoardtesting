using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBoard.Core.Models;

namespace TaskBoard.Core.Data
{
    public sealed class TaskBoardDbContext : DbContext
    {
        public TaskBoardDbContext(DbContextOptions<TaskBoardDbContext> options) : base(options) { }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var task = modelBuilder.Entity<TaskItem>();
            task.ToTable("Tasks");
            task.HasKey(t => t.Id);

            task.Property(t => t.Title).IsRequired().HasMaxLength(200);
            task.Property(t => t.CreatedUtc).IsRequired();

            task.HasIndex(t => t.CreatedUtc);
        }
    }
}

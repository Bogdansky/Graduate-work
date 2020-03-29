﻿using Data_Access_Layer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Data_Access_Layer
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }

        public Context(DbContextOptionsBuilder builder) : base(builder.Options) { }
        public Context(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamMember>()
                .HasKey(tm => new { tm.EmployeeId, tm.ProjectId, tm.RoleId });
            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Project)
                .WithMany(p => p.Team)
                .HasForeignKey(tm => tm.ProjectId);
            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Employee)
                .WithMany(e => e.Projects)
                .HasForeignKey(tm => tm.EmployeeId);
            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<User>(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Organization)
                .WithMany(o => o.Employees)
                .HasForeignKey(e => e.OrganizationId);
            modelBuilder.Entity<Project>()
                .HasOne(e => e.Organization)
                .WithMany(o => o.Projects)
                .HasForeignKey(e => e.OrganizationId);
            modelBuilder.Entity<Task>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId);
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskId);
        }
    }
}

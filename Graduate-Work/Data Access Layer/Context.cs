﻿using Microsoft.EntityFrameworkCore;
using Data_Access_Layer.Models;
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
        public DbSet<Project> Projects { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }
        public DbSet<TaskStatus> TaskStatuses { get; set; }
        public DbSet<TrackingHistory> TrackingHistory { get; set; }

        public Context(DbContextOptionsBuilder builder) : base(builder.Options) { }
        public Context(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().Property(r => r.Id).ValueGeneratedNever();
            modelBuilder.Entity<TaskType>().Property(t => t.Id).ValueGeneratedNever();
            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<User>(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TeamMember>()
                .HasKey(tm => new { tm.EmployeeId, tm.ProjectId, tm.RoleId });
            modelBuilder.Entity<Employee>()
                .HasOne(u => u.Role)
                .WithMany(e => e.Employees)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Project)
                .WithMany(p => p.TeamMembers)
                .HasForeignKey(tm => tm.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TeamMember>()
                .HasOne(t => t.Role)
                .WithMany(r => r.TeamMembers)
                .HasForeignKey(t => t.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Employee)
                .WithMany(e => e.TeamMembers)
                .HasForeignKey(tm => tm.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<User>(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Task>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId);
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Tasks)
                .WithOne(t => t.Employee)
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TrackingHistory>()
                .HasOne(t => t.Task);
            modelBuilder.Entity<TrackingHistory>()
                .HasOne(t => t.Employee);
        }
    }
}

﻿// <auto-generated />
using System;
using Data_Access_Layer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Data_Access_Layer.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("Data_Access_Layer.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AuthorId");

                    b.Property<string>("Description");

                    b.Property<int?>("TaskId");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("TaskId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Data_Access_Layer.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Birthday");

                    b.Property<string>("FullName");

                    b.Property<int>("RoleId");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Data_Access_Layer.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Data_Access_Layer.Models.Role", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Data_Access_Layer.Models.Task", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<long>("Effort");

                    b.Property<int?>("EmployeeId");

                    b.Property<int>("Priority");

                    b.Property<int>("ProjectId");

                    b.Property<long>("Recent");

                    b.Property<string>("Severity");

                    b.Property<int>("TaskStatusId");

                    b.Property<int>("TaskTypeId");

                    b.Property<string>("Title");

                    b.Property<DateTime>("UpdateDate");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("TaskStatusId");

                    b.HasIndex("TaskTypeId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Data_Access_Layer.Models.TaskStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("TaskStatuses");
                });

            modelBuilder.Entity("Data_Access_Layer.Models.TaskType", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("TaskTypes");
                });

            modelBuilder.Entity("Data_Access_Layer.Models.TeamMember", b =>
                {
                    b.Property<int?>("EmployeeId");

                    b.Property<int?>("ProjectId");

                    b.Property<int?>("RoleId");

                    b.Property<int>("Id");

                    b.Property<short>("IsAdmin")
                        .HasColumnType("BIT");

                    b.HasKey("EmployeeId", "ProjectId", "RoleId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("RoleId");

                    b.ToTable("TeamMembers");
                });

            modelBuilder.Entity("Data_Access_Layer.Models.TrackingHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<int>("EmployeeId");

                    b.Property<int>("TaskId");

                    b.Property<long>("TrackedTime");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("TaskId");

                    b.ToTable("TrackingHistory");
                });

            modelBuilder.Entity("Data_Access_Layer.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("EmployeeId");

                    b.Property<string>("Login");

                    b.Property<string>("Password");

                    b.Property<string>("Salt");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Data_Access_Layer.Models.Comment", b =>
                {
                    b.HasOne("Data_Access_Layer.Models.Employee", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("Data_Access_Layer.Models.Task", "Task")
                        .WithMany("Comments")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Data_Access_Layer.Models.Employee", b =>
                {
                    b.HasOne("Data_Access_Layer.Models.Role", "Role")
                        .WithMany("Employees")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Data_Access_Layer.Models.Task", b =>
                {
                    b.HasOne("Data_Access_Layer.Models.Employee", "Employee")
                        .WithMany("Tasks")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Data_Access_Layer.Models.Project", "Project")
                        .WithMany("Tasks")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data_Access_Layer.Models.TaskStatus", "TaskStatus")
                        .WithMany("Tasks")
                        .HasForeignKey("TaskStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data_Access_Layer.Models.TaskType", "TaskType")
                        .WithMany("Tasks")
                        .HasForeignKey("TaskTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Data_Access_Layer.Models.TeamMember", b =>
                {
                    b.HasOne("Data_Access_Layer.Models.Employee", "Employee")
                        .WithMany("TeamMembers")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data_Access_Layer.Models.Project", "Project")
                        .WithMany("TeamMembers")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Data_Access_Layer.Models.Role", "Role")
                        .WithMany("TeamMembers")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Data_Access_Layer.Models.TrackingHistory", b =>
                {
                    b.HasOne("Data_Access_Layer.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data_Access_Layer.Models.Task", "Task")
                        .WithMany()
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Data_Access_Layer.Models.User", b =>
                {
                    b.HasOne("Data_Access_Layer.Models.Employee", "Employee")
                        .WithOne("User")
                        .HasForeignKey("Data_Access_Layer.Models.User", "EmployeeId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}

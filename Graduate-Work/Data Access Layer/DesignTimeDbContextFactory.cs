using Data_Access_Layer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Data_Access_Layer
{
    public enum RoleEnum
    {
        None = 0,
        [Description("Junior инженер-программист")]
        JuniorSoftwareEngineer = 1,
        [Description("Middle инженер-программист")]
        MiddleSoftwareEngineer = 2,
        [Description("Senior инженер-программист")]
        SeniorSoftwareEngineer = 3,
        [Description("Team lead команды разработки")]
        TeamLeadSoftwareEngineer = 4,
        [Description("QA-инженер")]
        QAEngineer = 5,
        [Description("Team lead QA команды")]
        QATeamLeader = 6,
        [Description("Бизнес-аналитик")]
        BusinessAnalyst = 7,
        [Description("GUI-дизайнер")]
        GUIDesigner = 8,
        [Description("Data scientist")]
        DataScientist = 9,
        [Description("QA-инженер")]
        QAAutomationEngineer = 10,
        [Description("Проектный менеджер")]
        ProjectManager = 11,
        [Description("Data инженер")]
        DataEngineer = 12,
        [Description("Data аналитик")]
        DataAnalyst = 13
    }
    public enum TaskTypeEnum
    {
        [Description("Не указано")]
        None,
        [Description("Таск")]
        Task,
        [Description("Баг")]
        Bug
    }
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var databaseSettings = configuration.GetSection("DBSettings");
            var connectionString = string.Format("server={0};UserId={1};Password={2};database={3};",
                databaseSettings["Server"], databaseSettings["User"], databaseSettings["Password"], databaseSettings["Database"]);
            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            optionsBuilder.UseMySQL(connectionString);
            var context = new Context(optionsBuilder.Options);
            context.TaskTypes.AddRange(
                new TaskType { Id = (int)TaskTypeEnum.Task, Name = TaskTypeEnum.Task.GetDescription() },
                new TaskType { Id = (int)TaskTypeEnum.Bug, Name = TaskTypeEnum.Bug.GetDescription() }
                );
            context.Roles.AddRange(
                new[]
                {
                    new Role { Id = (int)RoleEnum.JuniorSoftwareEngineer, Name = RoleEnum.JuniorSoftwareEngineer.GetDescription() },
                    new Role {Id = (int)RoleEnum.MiddleSoftwareEngineer, Name = RoleEnum.MiddleSoftwareEngineer.GetDescription()},
                    new Role { Id = (int)RoleEnum.SeniorSoftwareEngineer, Name = RoleEnum.SeniorSoftwareEngineer.GetDescription() },
                    new Role {Id = (int)RoleEnum.TeamLeadSoftwareEngineer, Name = RoleEnum.TeamLeadSoftwareEngineer.GetDescription()},
                    new Role { Id = (int)RoleEnum.QAEngineer, Name = RoleEnum.QAEngineer.GetDescription() },
                    new Role {Id = (int)RoleEnum.QATeamLeader, Name = RoleEnum.QATeamLeader.GetDescription()},
                    new Role { Id = (int)RoleEnum.BusinessAnalyst, Name = RoleEnum.BusinessAnalyst.GetDescription() },
                    new Role {Id = (int)RoleEnum.GUIDesigner, Name = RoleEnum.GUIDesigner.GetDescription()},
                    new Role {Id = (int)RoleEnum.DataScientist, Name = RoleEnum.DataScientist.GetDescription()},
                    new Role {Id = (int)RoleEnum.QAAutomationEngineer, Name = RoleEnum.QAAutomationEngineer.GetDescription()},
                    new Role {Id = (int)RoleEnum.ProjectManager, Name = RoleEnum.ProjectManager.GetDescription()},
                    new Role {Id = (int)RoleEnum.DataEngineer, Name = RoleEnum.DataEngineer.GetDescription()},
                    new Role {Id = (int)RoleEnum.DataAnalyst, Name = RoleEnum.DataAnalyst.GetDescription()}
                }
            );
            context.SaveChanges();
            return context;
        }
    }
    public static class EnumHelper
    {
        public static string GetDescription(this Enum @enum)
        {
            FieldInfo fi = @enum.GetType().GetField(@enum.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return @enum.ToString();
        }
        /// <summary>
        /// Cast int value to description of TEnum value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Int value of enum (more than 255 - dangerous)</param>
        /// <returns></returns>
        public static string GetDescriptionByValue<T>(this int value)
        {
            var type = typeof(T);
            if (!type.IsEnum || !type.IsEnumDefined(value))
            {
                return "";
            }
            var enumValue = (T)Enum.ToObject(type, value) as Enum;
            return enumValue.GetDescription();
        }

        public static T GetMemberByValue<T>(this int value)
        {
            var type = typeof(T);
            if (!type.IsEnum || !type.IsEnumDefined(value))
            {
                return default;
            }
            return (T)Enum.ToObject(type, value);
        }

        public static List<KeyValuePair<int, string>> GetEnumValues<T>()
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                return null;
            }
            var list = Enum.GetValues(type).Cast<Enum>();
            var values = list.Select(e => new KeyValuePair<int, string>((int)Enum.Parse(type, e.ToString()), e.GetDescription())).ToList();
            return values;
        }
    }
}

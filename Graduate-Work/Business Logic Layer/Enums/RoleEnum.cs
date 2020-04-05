using System.ComponentModel;

namespace Business_Logic_Layer
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
}
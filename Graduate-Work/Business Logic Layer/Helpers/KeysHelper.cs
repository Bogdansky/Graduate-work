using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Helpers
{
    public class KeysHelper
    {
        const string Invite = "employee_{0}_project_{1}";

        public static string InviteKey(int employeeId, int projectId)
        {
            return string.Format(Invite, employeeId, projectId);
        }
    }
}

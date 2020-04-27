using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Graduate_Work.Models.Interfaces
{
    public interface IConnectionManager
    {
        bool AddConnection(string employeeId, string connectionId);
        bool RemoveConnection(string connectionId);
        string GetConnection(int employeeId);
        IEnumerable<int> OnlineUsers { get; }
        public string[] GetAllConnections();
        public bool ClearConnections();
    }
}

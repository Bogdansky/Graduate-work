using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transport_Layer.Models.Interfaces
{
    public interface IConnectionManager
    {
        bool AddConnection(string employeeId, string connectionId);
        bool RemoveConnection(string connectionId);
        string GetConnection(int employeeId);
        int GetUser(string connectionId);
        IEnumerable<int> OnlineUsers { get; }
        public string[] GetAllConnections();
        public bool ClearConnections();
    }
}

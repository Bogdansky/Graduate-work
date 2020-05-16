using Business_Logic_Layer.Helpers;
using Transport_Layer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transport_Layer.Models
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly static Dictionary<string, int> userMap = new Dictionary<string, int>();
        public IEnumerable<int> OnlineUsers
        {
            get
            {
                return userMap.Keys.Select(k => userMap[k]);
            }
        }

        public bool AddConnection(string connectionId, string employeeId)
        {
            lock (userMap)
            {
                return userMap.TryAdd(connectionId, employeeId.ToInt());
            }
        }

        public bool RemoveConnection(string connectionId)
        {
            lock (userMap)
            {
                try
                {
                    return userMap.Remove(connectionId);
                }
                catch
                {
                    return false;
                }
            }
        }

        public string GetConnection(int employeeId)
        {
            lock (userMap)
            {
                try
                {
                    return userMap.Keys.Where(k => userMap[k] == employeeId).SingleOrDefault();
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public int GetUser(string connectionId)
        {
            lock(userMap)
            {
                try
                {
                    userMap.TryGetValue(connectionId, out int employeeId);
                    return employeeId;
                }
                catch
                {
                    return default;
                }
            }
        }

        public string[] GetAllConnections()
        {
            lock (userMap)
            {
                return userMap.Keys.ToArray();
            }
        }

        public bool ClearConnections()
        {
            lock(userMap)
            {
                try
                {
                    userMap.Clear();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}

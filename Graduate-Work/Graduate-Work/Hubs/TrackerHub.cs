using AutoMapper;
using Business_Logic_Layer.Helpers;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Services.Crud;
using Graduate_Work.Helpers;
using Graduate_Work.Models.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Graduate_Work.Hubs
{
    public class TrackerHub : Hub
    {
        private readonly IConnectionManager _manager;
        private readonly ServiceCache _cache;
        private readonly TaskService _taskService;
        private readonly IMapper _mapper;

        private const string Key = "employee_{0}";
        private const long tenSeconds = 10000;

        public TrackerHub(IConnectionManager manager, ServiceCache cache, TaskService taskService, IMapper mapper)
        {
            _manager = manager;
            _cache = cache;
            _taskService = taskService;
            _mapper = mapper;
        }

        private string GetKey(int employeeId)
        {
            return string.Format(Key, employeeId);
        }

        public override Task OnConnectedAsync()
        {
            var employeeId = Context.GetHttpContext().Request.Query["employeeId"].ToString();
            _manager.AddConnection(employeeId, Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _manager.RemoveConnection(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public void CancelTrackForAll()
        {
            var connections = _manager.GetAllConnections();
            UpdateTasks(connections);
            _manager.ClearConnections();
        }

        public IEnumerable<int> GetOnlineUsers()
        {
            throw new NotImplementedException();
        }

        public async Task<Task> SendUpdate(int employeeId)
        {
            var connection = _manager.GetConnection(employeeId);
            try
            {
                if (connection != null)
                {
                    try
                    {
                        var key = GetKey(employeeId);
                        var task = UpdateTask(key);
                        await Clients.Caller.SendAsync("updateTask", task);
                    }
                    catch
                    {
                        return null;
                    }
                }
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                throw new Exception("ERROR: " + e.Message);
            }
        }

        private TrackedTask UpdateTask(string key, long timeout = tenSeconds)
        {
            if (string.IsNullOrEmpty(key)) return null;

            _cache.TryGetValue(key, out TrackedTask task);

            task.NextRecent -= timeout;
            task.UpdateDate = DateTime.Now;

            _cache.Set(key, task);
            return task;
        }

        public void UpdateTasks(string[] keys)
        {
            var ids = keys.Select(k => k.Split("_").Last().ToInt()).Where(k => k != default);

        }

        public async Task StopTracking()
        {
            var connectionId = Context.ConnectionId;
            var employeeId = _manager.OnlineUsers.SingleOrDefault();
            var key = GetKey(employeeId);
            _cache.TryGetValue(key, out TrackedTask task);
            if (task != null)
            {
                _taskService.UpdateTrackedAsync(task.Id, task.PreviousRecent);
                await Clients.Caller.SendAsync("stop", task.Id);
            }
        }
    }
}

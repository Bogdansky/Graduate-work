using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Helpers;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Services.Crud;
using Graduate_Work.Models.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TrackerHub> _logger;

        private const string Key = "employee_{0}";
        private const long millisecondsAtMinute = 60000;
        private const string startErrorMessage = "Не удалось начать отслеживание задачи";

        public TrackerHub(IConnectionManager manager, ServiceCache cache, TaskService taskService, IMapper mapper, ILogger<TrackerHub> logger)
        {
            _manager = manager;
            _cache = cache;
            _taskService = taskService;
            _mapper = mapper;
            _logger = logger;
        }

        private string GetKey(int employeeId)
        {
            return string.Format(Key, employeeId);
        }

        public override Task OnConnectedAsync()
        {
            var employeeId = Context.GetHttpContext().Request.Query["employeeId"].ToString();
            _ = _manager.AddConnection(Context.ConnectionId, employeeId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Task.WaitAll(StopTracking());
            _manager.RemoveConnection(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public void CancelTrackForAll()
        {
            var connections = _manager.GetAllConnections();
            UpdateTasks(connections);
            _manager.ClearConnections();
        }

        public async Task<Task> SendUpdate()
        {
            var employeeId = _manager.GetUser(Context.ConnectionId);
            try
            {
                try
                {
                    var key = GetKey(employeeId);
                    var task = UpdateTask(key);
                    await Clients.All.SendAsync("updateTask", task);
                }
                catch
                {
                    return null;
                }
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ERROR: " + e.Message);
                return Clients.Caller.SendAsync("updateTask", null);
            }
        }

        private TrackedTask UpdateTask(string key, long timeout = millisecondsAtMinute)
        {
            if (string.IsNullOrEmpty(key)) return null;

            _cache.TryGetValue(key, out TrackedTask task);

            task.PreviousRecent = task.NextRecent;
            task.NextRecent -= timeout;
            task.UpdateDate = DateTime.Now;

            _cache.Set(key, task);
            return task;
        }

        public void UpdateTasks(string[] keys)
        {
            var ids = keys.Select(k => k.Split("_").Last().ToInt()).Where(k => k != default);

        }

        public async Task StartTracking(int taskId)
        {
            var connectionId = Context.ConnectionId;    
            var employeeId = _manager.GetUser(connectionId);
            var key = GetKey(employeeId);

            if (_cache.TryGetValue(key, out TrackedTask _))
                await Clients.Caller.SendAsync("start", new Error { Description = "Вы уже отслеживаете задачу" });
            else
            {
                OperationResult result = new OperationResult();
                try
                {
                    var task = _taskService.Read(taskId).Result as TaskDTO;
                    var trackedTask = _mapper.Map<TrackedTask>(task);
                    _cache.Set(key, trackedTask);
                    result.Result = new { task.Id, Name = task.Title, timeout = millisecondsAtMinute };
                }
                catch (Exception e)
                {
                    _logger.LogError(e, startErrorMessage);
                    result.Error = new Error { Description = startErrorMessage };
                }

                await Clients.Caller.SendAsync("start", result);
            }
        }

        public async Task StopTracking()
        {
            var employeeId = _manager.GetUser(Context.ConnectionId);
            var key = GetKey(employeeId);
            _cache.TryGetValue(key, out TrackedTask task);
            if (task != null)
            {
                _cache.DeleteKey(key);
                var diff = (DateTime.Now - task.UpdateDate).Milliseconds;
                var newTask = await _taskService.UpdateTrackedAsync(task.Id, task.PreviousRecent - diff);
                await Clients.All.SendAsync("stop", newTask);
            }
        }
    }
}

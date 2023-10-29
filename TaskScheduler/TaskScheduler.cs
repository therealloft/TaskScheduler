using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Scheduler
{
    public class TaskScheduler
    {
        private int SleepTime = 750;
        private List<ScheduledTask> taskList;
        private List<ScheduledTask> addList;
        private bool running = false;
        private Thread thread;
        public TaskScheduler()
        {
            Init();
        }
        public TaskScheduler(int interval)
        {
            Init();
            SleepTime = interval;
        }
        public void Init()
        {
            this.taskList = new List<ScheduledTask>();
            this.addList = new List<ScheduledTask>();
            this.thread = new Thread(new ThreadStart(Run));
        }
        public void Destroy()
        {
            StopService();
        }
        public void StartService()
        {
            this.running = true;
            this.thread.IsBackground = true;
            this.thread.Start();
        }
        public void StopService()
        {
            this.running = false;
            this.thread.Abort();
        }
        public void Run()
        {
            while (this.running)
            {
                try
                {
                    ExecuteTasks();
                    Thread.Sleep(this.SleepTime);
                }
                catch (Exception) { }
            }
        }
        public void AddScheduledTask(Task task, int interval, bool loop, ITaskHandler callback)
        {
            this.addList.Add(new ScheduledTask(task, interval, loop, callback));
        }
        public void RemoveScheduledTask(Task task)
        {
            ScheduledTask taskToRemove = taskList.Where(x => x.task == task).FirstOrDefault();
            if (taskToRemove != null)
            {
                taskList.Remove(taskToRemove);
            }
        }
        public bool IsRunning() => this.running;
        private void ExecuteTasks()
        {
            DateTime now = DateTime.UtcNow;
            foreach (ScheduledTask t in taskList)
            {
                if (!t.task.active)
                {
                    taskList.Remove(t);
                    continue;
                }
                if (now >= t.expiry)
                {
                    try
                    {
                        t.callback.DoTask(t.task);
                    }
                    catch (Exception) { }
                    if (t.loop)
                    {
                        t.expiry = DateTime.UtcNow.AddSeconds(t.interval);
                        continue;
                    }
                }
            }
            if (addList.Count > 0)
            {
                taskList.AddRange(addList);
                addList.Clear();
            }
        }
    }
}

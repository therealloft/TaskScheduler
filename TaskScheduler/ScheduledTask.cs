using System;

namespace Scheduler
{
    public class ScheduledTask
    {
        internal DateTime expiry;
        internal int interval;
        internal bool loop;
        internal ITaskHandler callback;
        internal Task task;

        public ScheduledTask(Task task, int interval, bool loop, ITaskHandler callback)
        {
            this.task = task;
            this.interval = interval;
            this.expiry = DateTime.UtcNow.AddSeconds(interval);
            this.callback = callback;
            this.loop = loop;
        }

        public int GetInterval()
        {
            return this.interval;
        }

        public Task GetTask()
        {
            return this.task;
        }

        public ITaskHandler GetCallback()
        {
            return this.callback;
        }

        public DateTime GetExpiry()
        {
            return this.expiry;
        }

        public bool IsLooping()
        {
            return this.loop;
        }
    }
}

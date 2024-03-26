using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Util
{
    public static class Watchdog
    {
        private static readonly List<Task> tasks = new();
        public static void Init()
        {
            Task.Run(Loop);
        }

        public static void Loop()
        {
            while (true)
            {
                Thread.Sleep(1000);
                Task[] array;
                lock (tasks) { array = tasks.ToArray(); }
                foreach (Task task in array)
                {
                    if (task.IsCompleted)
                    {
                        using (task)
                        {
                            lock (tasks) { tasks.Remove(task); }
                        }
                    }
                }
            }
        }

        public static Task Watch
        {
            set { lock (tasks) { tasks.Add(value); } }
        }

        public static Task Add(Task task)
        {
            lock (tasks) { tasks.Add(task); }
            return task;
        }

        public static Task Delay(int millis)
        {
            Task task;
            lock(tasks) { tasks.Add(task = Task.Delay(millis)); }
            return task;
        }
    }
}

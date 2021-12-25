using System;
using System.Collections.Generic;
using System.Linq;
using WorkTimeStat.Models;
using WorkTimeStat.Storage;

namespace WorkTimeStat.Services
{
    internal static class TaskService
    {
        public static List<DateTime> GetDatesWithTasksRecorded()
        {
            return WorkKeeper.Instance.Tasks.Keys.ToList();
        }

        public static List<MeasuredTask> GetTodaysTasks()
        {
            return GetDaysTasks(DateTime.Today);
        }

        public static List<MeasuredTask> GetDaysTasks(DateTime date)
        {
            WorkKeeper.Instance.Tasks.TryGetValue(date.Date, out List<MeasuredTask> tasks);
            return tasks;
        }

        public static void UpdateTaskList(List<MeasuredTask> tickets)
        {
            WorkKeeper.Instance.Tasks.TryGetValue(DateTime.Today, out List<MeasuredTask> storedTasks);
            if (storedTasks == null)
            {
                storedTasks = new List<MeasuredTask>();
                WorkKeeper.Instance.Tasks.Add(DateTime.Today, storedTasks);
            }

            storedTasks.Clear();
            storedTasks.AddRange(tickets);
        }

        public static void PauseActiveTask()
        {
            MeasuredTask activeTask = GetTodaysTasks()
                                        .FirstOrDefault(ticket => ticket.CurrentTimePair.EndTime == default);

            SetActiveTask(activeTask.TaskName);
            activeTask.PauseTask();
        }

        public static void RestartActiveTask()
        {
            MeasuredTask activeTask = GetTodaysTasks()
                                        .FirstOrDefault(ticket => ticket.TaskName == WorkKeeper.Instance.ActiveTaskName);
            activeTask.StartTask();
            SetActiveTask(string.Empty);
        }

        private static void SetActiveTask(string taskName)
        {
            WorkKeeper.Instance.ActiveTaskName = taskName;
        }
    }
}

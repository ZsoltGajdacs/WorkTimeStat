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
            return tasks ?? new List<MeasuredTask>();
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

        public static void UpdateTaskbarTooltipWithActiveTask()
        {
            MeasuredTask activeTask = FindActiveTaskInList();
            if (activeTask != null)
            {
                SetActiveTaskName(activeTask.TaskName);
            }
            else
            {
                SetActiveTaskName(string.Empty);
            }
        }

        public static void PauseActiveTask()
        {
            MeasuredTask activeTask = FindActiveTaskInList();

            SetActiveTaskName(activeTask.TaskName);
            activeTask.PauseTask();
        }

        public static void RestartActiveTask()
        {
            MeasuredTask activeTask = GetTodaysTasks()
                                        .FirstOrDefault(ticket => ticket.TaskName == WorkKeeper.Instance.ActiveTaskName);
            activeTask.StartTask();
            SetActiveTaskName(string.Empty);
        }

        private static MeasuredTask FindActiveTaskInList()
        {
            return GetTodaysTasks().FirstOrDefault(ticket => ticket.CurrentTimePair.EndTime == default);
        }

        private static void SetActiveTaskName(string taskName)
        {
            WorkKeeper.Instance.ActiveTaskName = taskName;
        }
    }
}

using System;
using System.Collections.Generic;
using Ado.Net_Example.Models;
using Ado.Net_Example.Repositories;
using Task = Ado.Net_Example.Models.Task;

namespace Ado.Net_Example
{
    class Program
    {
        private static TaskRepository _taskRepository = new TaskRepository();

        static void Main(string[] args)
        {
            Console.WriteLine(" Personal Task Manager with ADO.NET & MySQL");
            Console.WriteLine("=============================================\n");

            ShowMainMenu();
        }

        static void ShowMainMenu()
        {
            while (true)
            {
                Console.WriteLine("\n MAIN MENU");
                Console.WriteLine("1. View All Tasks");
                Console.WriteLine("2. View Pending Tasks");
                Console.WriteLine("3. View Completed Tasks");
                Console.WriteLine("4. View Overdue Tasks");
                Console.WriteLine("5. Add New Task");
                Console.WriteLine("6. Update Task");
                Console.WriteLine("7. Mark Task as Completed");
                Console.WriteLine("8. Delete Task");
                Console.WriteLine("9. Exit");
                Console.Write("\nChoose an option: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ViewAllTasks();
                            break;
                        case "2":
                            ViewPendingTasks();
                            break;
                        case "3":
                            ViewCompletedTasks();
                            break;
                        case "4":
                            ViewOverdueTasks();
                            break;
                        case "5":
                            AddNewTask();
                            break;
                        case "6":
                            UpdateTask();
                            break;
                        case "7":
                            MarkTaskCompleted();
                            break;
                        case "8":
                            DeleteTask();
                            break;
                        case "9":
                            Console.WriteLine("Goodbye! ");
                            return;
                        default:
                            Console.WriteLine(" Invalid option. Press any key to continue...");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Error: {ex.Message}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void ViewAllTasks()
        {
            Console.Clear();
            Console.WriteLine(" ALL TASKS\n");

            var tasks = _taskRepository.GetAllTasks();
            DisplayTasks(tasks);
            WaitForUser();
        }

        static void ViewPendingTasks()
        {
            Console.Clear();
            Console.WriteLine(" PENDING TASKS\n");

            var tasks = _taskRepository.GetTasksByStatus(false);
            DisplayTasks(tasks);
            WaitForUser();
        }

        static void ViewCompletedTasks()
        {
            Console.Clear();
            Console.WriteLine(" COMPLETED TASKS\n");

            var tasks = _taskRepository.GetTasksByStatus(true);
            DisplayTasks(tasks);
            WaitForUser();
        }

        static void ViewOverdueTasks()
        {
            Console.Clear();
            Console.WriteLine(" OVERDUE TASKS\n");

            var tasks = _taskRepository.GetOverdueTasks();
            DisplayTasks(tasks);
            WaitForUser();
        }

        static void AddNewTask()
        {
            Console.Clear();
            Console.WriteLine("ADD NEW TASK\n");

            Console.Write("Title: ");
            var title = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Title is required!");
                WaitForUser();
                return;
            }

            Console.Write("Description (optional): ");
            var description = Console.ReadLine();

            Console.Write("Due Date (yyyy-mm-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dueDate))
            {
                Console.WriteLine("Invalid date format!");
                WaitForUser();
                return;
            }

            var newTask = new Models.Task
            {
                Tittle = title,
                Description = string.IsNullOrWhiteSpace(description) ? null : description,
                DueDate = dueDate,
                ISCompleted = false
            };

            var newId = _taskRepository.AddTask(newTask);
            Console.WriteLine($"\n Task added successfully! ID: {newId}");
            WaitForUser();
        }

        static void UpdateTask()
        {
            Console.Clear();
            Console.WriteLine("UPDATE TASK\n");

            Console.Write("Enter Task ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int taskId))
            {
                Console.WriteLine("Invalid Task ID!");
                WaitForUser();
                return;
            }

            var existingTask = _taskRepository.GetTaskById(taskId);
            if (existingTask == null)
            {
                Console.WriteLine("Task not found!");
                WaitForUser();
                return;
            }

            Console.WriteLine($"Current Title: {existingTask.Tittle}");
            Console.Write("New Title (press enter to keep current): ");
            var newTitle = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newTitle))
            {
                existingTask.Tittle = newTitle;
            }

            Console.WriteLine($"Current Description: {existingTask.Description}");
            Console.Write("New Description (press enter to keep current): ");
            var newDescription = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newDescription))
            {
                existingTask.Description = newDescription;
            }

            Console.WriteLine($"Current Due Date: {existingTask.DueDate:yyyy-MM-dd}");
            Console.Write("New Due Date (yyyy-mm-dd, press enter to keep current): ");
            var dueDateInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dueDateInput) && DateTime.TryParse(dueDateInput, out DateTime newDueDate))
            {
                existingTask.DueDate = newDueDate;
            }

            Console.Write("Mark as completed? (y/n, press enter to keep current): ");
            var completedInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(completedInput))
            {
                existingTask.ISCompleted = completedInput.ToLower() == "y";
            }

            var success = _taskRepository.UpdateTask(existingTask);
            if (success)
            {
                Console.WriteLine("\n Task updated successfully!");
            }
            else
            {
                Console.WriteLine("\n Failed to update task!");
            }

            WaitForUser();
        }

        static void MarkTaskCompleted()
        {
            Console.Clear();
            Console.WriteLine(" MARK TASK AS COMPLETED\n");

            Console.Write("Enter Task ID to mark as completed: ");
            if (!int.TryParse(Console.ReadLine(), out int taskId))
            {
                Console.WriteLine(" Invalid Task ID!");
                WaitForUser();
                return;
            }

            var success = _taskRepository.MarkTaskAsCompleted(taskId);
            if (success)
            {
                Console.WriteLine("\n Task marked as completed!");
            }
            else
            {
                Console.WriteLine("\n Task not found or already completed!");
            }

            WaitForUser();
        }

        static void DeleteTask()
        {
            Console.Clear();
            Console.WriteLine(" DELETE TASK\n");

            Console.Write("Enter Task ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int taskId))
            {
                Console.WriteLine(" Invalid Task ID!");
                WaitForUser();
                return;
            }

            var task = _taskRepository.GetTaskById(taskId);
            if (task == null)
            {
                Console.WriteLine(" Task not found!");
                WaitForUser();
                return;
            }

            Console.WriteLine($"You are about to delete: {task.Tittle}");
            Console.Write("Are you sure? (y/n): ");
            var confirmation = Console.ReadLine();

            if (confirmation?.ToLower() == "y")
            {
                var success = _taskRepository.DeleteTask(taskId);
                if (success)
                {
                    Console.WriteLine("\n Task deleted successfully!");
                }
                else
                {
                    Console.WriteLine("\n Failed to delete task!");
                }
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }

            WaitForUser();
        }

        static void DisplayTasks(List<Task> tasks)
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks found. ");
                return;
            }

            foreach (var task in tasks)
            {
                Console.WriteLine($"ID: {task.Id}");
                Console.WriteLine($"Title: {task.Tittle}");
                if (!string.IsNullOrEmpty(task.Description))
                    Console.WriteLine($"Description: {task.Description}");
                Console.WriteLine($"Due Date: {task.DueDate:yyyy-MM-dd} ({task.DueInfo})");
                Console.WriteLine($"Status: {task.Status}");
                Console.WriteLine($"Created: {task.CreatedAt:yyyy-MM-dd}");
                Console.WriteLine("────────────────────────────");
            }
        }

        static void WaitForUser()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
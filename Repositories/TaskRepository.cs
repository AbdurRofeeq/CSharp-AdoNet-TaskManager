using System;
using Ado.Net_Example.Data;
using Ado.Net_Example.Models;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Task = Ado.Net_Example.Models.Task;

namespace Ado.Net_Example.Repositories
{
    public class TaskRepository
    {
        private readonly DatabaseHelper _databaseHelper;

        public TaskRepository()
        {
            _databaseHelper = new DatabaseHelper();
        }

        public int AddTask(Task task)
        {
            const string query = @"INSERT INTO Tasks (Tittle, Description, DueDate, ISCompleted)
                                VALUES (@Tittle, @Description, @DueDate, @ISCompleted, @CreatedAt, @UpdateAt);
                                SELECT LAST_INSERT_ID();";

            using var connection = _databaseHelper.GetConnection();
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@Tittle", task.Tittle);
            command.Parameters.AddWithValue("@Description", task.Description);

            command.Parameters.AddWithValue("@DueDate", task.DueDate);
            command.Parameters.AddWithValue("@ISCompleted", task.ISCompleted);

            connection.Open();
            var result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }


        public List<Task> GetAllTasks()
        {
            var tasks = new List<Task>();
            const string query = "SELECT * FROM Tasks ORDER BY ISCompleted, DueDate, CreatedAt Desc";
        

            using var connection = _databaseHelper.GetConnection();
            using var command = new MySqlCommand(query, connection);

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tasks.Add(MapReaderToTask(reader));
            }
            return tasks;
        }

        public bool UpdateTask(Task task)
        {
            const string query = @"UPDATE Tasks
                                SET Tittle = @Tittle,
                                    Description = @Description,
                                    DueDate = @DueDate,
                                    ISCompleted = @ISCompleted,
                                    UpdateAt = @UpdateAt
                                WHERE Id = @Id";

            using var connection = _databaseHelper.GetConnection();
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@Tittle", task.Tittle);
            command.Parameters.AddWithValue("@Description", task.Description);
            command.Parameters.AddWithValue("@DueDate", task.DueDate);
            command.Parameters.AddWithValue("@ISCompleted", task.ISCompleted);
            command.Parameters.AddWithValue("@UpdateAt", DateTime.Now);
            command.Parameters.AddWithValue("@Id", task.Id);

            connection.Open();
            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool DeleteTask(int taskId)
        {
            const string query = "DELETE FROM Tasks WHERE Id = @Id";

            using var connection = _databaseHelper.GetConnection();
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", taskId);

            connection.Open();
            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }

       public bool MarkTaskAsCompleted(int taskId)
        {
            const string query = @"UPDATE Tasks
                                SET ISCompleted = @ISCompleted,
                                    UpdateAt = @UpdateAt
                                WHERE Id = @Id";

            using var connection = _databaseHelper.GetConnection();
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@ISCompleted", true);
            command.Parameters.AddWithValue("@UpdateAt", DateTime.Now);
            command.Parameters.AddWithValue("@Id", taskId);

            connection.Open();
            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<Task> GetTasksByStatus(bool isCompleted)
        {
            var tasks = new List<Task>();
            const string sql = "SELECT * FROM Tasks WHERE ISCompleted = @ISCompleted ORDER BY DueDate";
            
            using var connection = _databaseHelper.GetConnection();
            using var command = new MySqlCommand(sql, connection);
            
            command.Parameters.AddWithValue("@ISCompleted", isCompleted);
            
            connection.Open();
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                tasks.Add(MapReaderToTask(reader));
            }

            return tasks;
        }

        public Task GetTaskById(int taskId)
        {
            const string sql = "SELECT * FROM Tasks WHERE Id = @Id";
            
            using var connection = _databaseHelper.GetConnection();
            using var command = new MySqlCommand(sql, connection);
            
            command.Parameters.AddWithValue("@Id", taskId);
            
            connection.Open();
            using var reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                return MapReaderToTask(reader);
            }

            return null!;
        }



        // READ - Get overdue tasks
        public List<Task> GetOverdueTasks()
        {
            var tasks = new List<Task>();
            const string sql = @"
                SELECT * FROM Tasks 
                WHERE DueDate < CURDATE() AND ISCompleted = FALSE 
                ORDER BY DueDate";

            using var connection = _databaseHelper.GetConnection();
            using var command = new MySqlCommand(sql, connection);
            
            connection.Open();
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                tasks.Add(MapReaderToTask(reader));
            }

            return tasks;
        }

        private Task MapReaderToTask(MySqlDataReader reader)
        {
            return new Task
            {
                Id = Convert.ToInt32(reader["Id"]),
                Tittle = reader["Tittle"].ToString()!,
                Description = reader["Description"].ToString()!,
                DueDate = Convert.ToDateTime(reader["DueDate"]),
                ISCompleted = Convert.ToBoolean(reader["ISCompleted"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                UpdateAt = Convert.ToDateTime(reader["UpdateAt"])
            };
        }


    }
}
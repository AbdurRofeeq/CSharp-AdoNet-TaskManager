using MySql.Data.MySqlClient;
using System;
namespace Ado.Net_Example.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;
        public DatabaseHelper(string server = "localhost", string database = "taskmanager", string username = "root", string password = "k34532")
        {
            _connectionString = $"Server={server};Database={database};User Id={username};Password={password};";
            TestConnection();
        }

        private void TestConnection()
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connection successful.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                throw;
            }   
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
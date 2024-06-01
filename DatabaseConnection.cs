using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Pizzeria
{
    internal class DatabaseConnection
    {
        private static string connectionString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=nasa;";

        public static NpgsqlConnection GetConnection()
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            try
            {
                conn.Open();
                return conn;

            } catch (NpgsqlException ex)
            {
                Console.WriteLine($"Ошибка при подключении к базе данных: {ex.Message}");
                throw;
            }
        }
    }
}

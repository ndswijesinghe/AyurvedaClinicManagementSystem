using System;
using Microsoft.Data.SqlClient;

namespace AyurvedaClinicManagementSystem.Data
{
    public class DatabaseHelper
    {
        private string connectionString =
     "Server=localhost\\SQLEXPRESS;Database=AyurvedaClinicDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
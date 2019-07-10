using System;
using System.Data;
using System.Data.SqlClient;

namespace Tailwind.Traders.Rewards.Registration.Api.Repositories
{
    public abstract class BaseRepository : IDisposable
    {
        private readonly SqlConnection _sqlConnection;
        public BaseRepository(string connectionString)
        {
            var actualConnection = Environment.GetEnvironmentVariable("ConnectionString") ?? connectionString;
            _sqlConnection = new SqlConnection(actualConnection);
        }

        protected SqlConnection Connection
        {
            get
            {
                if (_sqlConnection.State != ConnectionState.Open)
                {
                    _sqlConnection.Open();
                }
                return _sqlConnection;
            }
        }

        protected DataTable ExecuteSelect(string query, SqlParameter[] parameters)
        {
            DataTable table = new DataTable();
            using (SqlCommand command = new SqlCommand(query, Connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {
                    dataAdapter.Fill(table);
                }
            }

            return table;
        }

        protected void ExecuteNonSelect(string query, SqlParameter[] parameters = null)
        {
            using (SqlCommand command = new SqlCommand(query, Connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                command.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            if (_sqlConnection != null && _sqlConnection.State == ConnectionState.Open)
            {
                _sqlConnection.Close();
            }
        }
    }
}
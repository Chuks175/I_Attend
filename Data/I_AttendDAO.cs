using I_Attend.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace I_Attend.Data
{
    public interface IAttendDAO
    {
        Task<List<View>> GetViewsAsync();
        Task AddViewAsync(View view);
        Task UpdateViewAsync(View view);
        Task DeleteViewAsync(int id);
        Task<View> GetUserCredentialsAsync(string email, string password, string matricNumber);
        Task<bool> RegisterCredentialsAsync(View view);
        Task<bool> ViewExistsAsync(int id);
    }

    public class I_AttendDAO : IAttendDAO
    {
        private readonly string _connectionString;
        private readonly ILogger<I_AttendDAO> _logger;

        public I_AttendDAO(string connectionString, ILogger<I_AttendDAO> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger;
        }

        public async Task<List<View>> GetViewsAsync()
        {
            var views = new List<View>();
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand("SELECT Id, UserNames, Department, Email, Matric_Number, Password FROM details", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var view = new View
                                {
                                    Id = reader.GetInt32(0),
                                    UserNames = reader.GetString(1),
                                    Department = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    Matric_Number = reader.GetString(4),
                                    Password = reader.GetString(5)
                                };
                                views.Add(view);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Error fetching views from database");
                throw;
            }
            return views;
        }

        public async Task AddViewAsync(View view)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(
                        "INSERT INTO details (UserNames, Department, Email, Matric_Number, Password) " +
                        "VALUES (@UserNames, @Department, @Email, @Matric_Number, @Password)", connection))
                    {
                        command.Parameters.AddWithValue("@UserNames", view.UserNames);
                        command.Parameters.AddWithValue("@Department", view.Department);
                        command.Parameters.AddWithValue("@Email", view.Email);
                        command.Parameters.AddWithValue("@Matric_Number", view.Matric_Number);
                        command.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(view.Password));
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Error adding view to database");
                throw;
            }
        }

        public async Task UpdateViewAsync(View view)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(
                        "UPDATE details SET UserNames = @UserNames, Department = @Department, Email = @Email, Matric_Number = @Matric_Number " +
                        "WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", view.Id);
                        command.Parameters.AddWithValue("@UserNames", view.UserNames);
                        command.Parameters.AddWithValue("@Department", view.Department);
                        command.Parameters.AddWithValue("@Email", view.Email);
                        command.Parameters.AddWithValue("@Matric_Number", view.Matric_Number);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Error updating view with ID {Id}", view.Id);
                throw;
            }
        }

        public async Task DeleteViewAsync(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand("DELETE FROM details WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Error deleting view with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> ViewExistsAsync(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand("SELECT COUNT(*) FROM details WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                        return count > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Error checking if view exists with ID {Id}", id);
                throw;
            }
        }

        public async Task<View> GetUserCredentialsAsync(string email, string password, string matricNumber)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(
                        "SELECT Id, UserNames, Department, Email, Matric_Number, Password FROM details " +
                        "WHERE Email = @Email AND Matric_Number = @Matric_Number", connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Matric_Number", matricNumber);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var storedPassword = reader.GetString(5);
                                if (BCrypt.Net.BCrypt.Verify(password, storedPassword))
                                {
                                    return new View
                                    {
                                        Id = reader.GetInt32(0),
                                        UserNames = reader.GetString(1),
                                        Department = reader.GetString(2),
                                        Email = reader.GetString(3),
                                        Matric_Number = reader.GetString(4),
                                        Password = storedPassword
                                    };
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Error fetching user credentials for Email {Email}", email);
                throw;
            }
            return null;
        }

        public async Task<bool> RegisterCredentialsAsync(View view)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(
                        "INSERT INTO details (UserNames, Department, Email, Matric_Number, Password) " +
                        "VALUES (@UserNames, @Department, @Email, @Matric_Number, @Password)", connection))
                    {
                        command.Parameters.AddWithValue("@UserNames", view.UserNames);
                        command.Parameters.AddWithValue("@Department", view.Department);
                        command.Parameters.AddWithValue("@Email", view.Email);
                        command.Parameters.AddWithValue("@Matric_Number", view.Matric_Number);
                        command.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(view.Password));
                        return await command.ExecuteNonQueryAsync() > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Error registering user with Email {Email}", view.Email);
                throw;
            }
        }
    }
}
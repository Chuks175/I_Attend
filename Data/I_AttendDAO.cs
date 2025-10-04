//using I_Attend.Models;
//using MySql.Data.MySqlClient;
//using Microsoft.Extensions.Logging;
//using BCrypt.Net;

//namespace I_Attend.Data
//{
//    public interface IAttendDAO
//    {
//        Task<List<View>> GetViewsAsync();
//        Task AddViewAsync(View view);
//        Task UpdateViewAsync(View view);
//        Task DeleteViewAsync(int id);
//        Task<View> GetUserCredentialsAsync(string email, string password, string matricNumber);
//        Task<bool> RegisterCredentialsAsync(View view);
//        Task<bool> ViewExistsAsync(int id);
//        Task<bool> AddStudentImageAsync(string matricNumber, byte[] imageData);
//    }

//    public class I_AttendDAO : IAttendDAO
//    {
//        private readonly string _connectionString;
//        private readonly ILogger<I_AttendDAO> _logger;

//        public I_AttendDAO(string connectionString, ILogger<I_AttendDAO> logger)
//        {
//            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
//            _logger = logger;
//        }

//        public async Task<List<View>> GetViewsAsync()
//        {
//            var views = new List<View>();
//            try
//            {
//                using (var connection = new MySqlConnection(_connectionString))
//                {
//                    await connection.OpenAsync();
//                    using (var command = new MySqlCommand("SELECT Id, UserNames, Department, Email, Matric_Number, Password, ImageData, Course_code FROM details", connection))
//                    {
//                        using (var reader = await command.ExecuteReaderAsync())
//                        {
//                            while (await reader.ReadAsync())
//                            {
//                                var view = new View
//                                {
//                                    Id = reader.GetInt32(0),
//                                    UserNames = reader.GetString(1),
//                                    Department = reader.GetString(2),
//                                    Email = reader.GetString(3),
//                                    Matric_Number = reader.GetString(4),
//                                    Password = reader.GetString(5),
//                                    CourseCode = reader.GetString(6),
//                                    ImageData = reader.IsDBNull(reader.GetOrdinal("ImageData")) ? null : (byte[])reader["ImageData"]
//                                    //CourseCode = reader.IsDBNull(reader.GetOrdinal("Course_code")) ? null : reader.GetString["Course_code"]
//                                };
//                                views.Add(view);
//                            }
//                        }
//                    }
//                }
//            }
//            catch (MySqlException ex)
//            {
//                _logger.LogError(ex, "Error fetching views from database");
//                throw;
//            }
//            return views;
//        }

//        public async Task<bool> AddStudentImageAsync(string matricNumber, byte[] imageData)
//        {
//            try
//            {
//                using (var connection = new MySqlConnection(_connectionString))
//                {
//                    await connection.OpenAsync();
//                    using (var command = new MySqlCommand(
//                        "UPDATE details SET ImageData = @ImageData WHERE Matric_Number = @Matric_Number", connection))
//                    {
//                        command.Parameters.AddWithValue("@ImageData", imageData ?? (object)DBNull.Value);
//                        command.Parameters.AddWithValue("@Matric_Number", matricNumber);
//                        var rowsAffected = await command.ExecuteNonQueryAsync();
//                        return rowsAffected > 0;
//                    }
//                }
//            }
//            catch (MySqlException ex)
//            {
//                _logger.LogError(ex, "Error adding student image for Matric_Number {Matric_Number}", matricNumber);
//                throw;
//            }
//        }

//        public async Task AddViewAsync(View view)
//        {
//            try
//            {
//                using (var connection = new MySqlConnection(_connectionString))
//                {
//                    await connection.OpenAsync();
//                    using (var command = new MySqlCommand(
//                        "INSERT INTO details (UserNames, Department, Email, Matric_Number, Password, Course_code) " +
//                        "VALUES (@UserNames, @Department, @Email, @Matric_Number, @Password, @CourseCode)", connection))
//                    {
//                        command.Parameters.AddWithValue("@UserNames", view.UserNames);
//                        command.Parameters.AddWithValue("@Department", view.Department);
//                        command.Parameters.AddWithValue("@Email", view.Email);
//                        command.Parameters.AddWithValue("@Matric_Number", view.Matric_Number);
//                        command.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(view.Password));
//                        command.Parameters.AddWithValue("@CourseCode", (object)view.CourseCode ?? DBNull.Value);
//                        await command.ExecuteNonQueryAsync();
//                    }
//                }
//            }
//            catch (MySqlException ex)
//            {
//                _logger.LogError(ex, "Error adding view to database");
//                throw;
//            }
//        }

//        public async Task UpdateViewAsync(View view)
//        {
//            try
//            {
//                using (var connection = new MySqlConnection(_connectionString))
//                {
//                    await connection.OpenAsync();
//                    using (var command = new MySqlCommand(
//                        "UPDATE details SET UserNames = @UserNames, Department = @Department, Email = @Email, Matric_Number = @Matric_Number, Course_code = @CourseCode " +
//                        "WHERE Id = @Id", connection))
//                    {
//                        command.Parameters.AddWithValue("@Id", view.Id);
//                        command.Parameters.AddWithValue("@UserNames", view.UserNames);
//                        command.Parameters.AddWithValue("@Department", view.Department);
//                        command.Parameters.AddWithValue("@Email", view.Email);
//                        command.Parameters.AddWithValue("@Matric_Number", view.Matric_Number);
//                        command.Parameters.AddWithValue("@CourseCode", (object)view.CourseCode ?? DBNull.Value);
//                        await command.ExecuteNonQueryAsync();
//                    }
//                }
//            }
//            catch (MySqlException ex)
//            {
//                _logger.LogError(ex, "Error updating view with ID {Id}", view.Id);
//                throw;
//            }
//        }

//        public async Task DeleteViewAsync(int id)
//        {
//            try
//            {
//                using (var connection = new MySqlConnection(_connectionString))
//                {
//                    await connection.OpenAsync();
//                    using (var command = new MySqlCommand("DELETE FROM details WHERE Id = @Id", connection))
//                    {
//                        command.Parameters.AddWithValue("@Id", id);
//                        await command.ExecuteNonQueryAsync();
//                    }
//                }
//            }
//            catch (MySqlException ex)
//            {
//                _logger.LogError(ex, "Error deleting view with ID {Id}", id);
//                throw;
//            }
//        }

//        public async Task<bool> ViewExistsAsync(int id)
//        {
//            try
//            {
//                using (var connection = new MySqlConnection(_connectionString))
//                {
//                    await connection.OpenAsync();
//                    using (var command = new MySqlCommand("SELECT COUNT(*) FROM details WHERE Id = @Id", connection))
//                    {
//                        command.Parameters.AddWithValue("@Id", id);
//                        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
//                        return count > 0;
//                    }
//                }
//            }
//            catch (MySqlException ex)
//            {
//                _logger.LogError(ex, "Error checking if view exists with ID {Id}", id);
//                throw;
//            }
//        }

//        public async Task<View> GetUserCredentialsAsync(string email, string password, string matricNumber)
//        {
//            try
//            {
//                using (var connection = new MySqlConnection(_connectionString))
//                {
//                    await connection.OpenAsync();
//                    using (var command = new MySqlCommand(
//                        "SELECT Id, UserNames, Department, Email, Matric_Number, Password, ImageData, Course_code FROM details " +
//                        "WHERE Email = @Email AND Matric_Number = @Matric_Number", connection))
//                    {
//                        command.Parameters.AddWithValue("@Email", email);
//                        command.Parameters.AddWithValue("@Matric_Number", matricNumber);
//                        using (var reader = await command.ExecuteReaderAsync())
//                        {
//                            if (await reader.ReadAsync())
//                            {
//                                var storedPassword = reader.GetString(5);
//                                if (BCrypt.Net.BCrypt.Verify(password, storedPassword))
//                                {
//                                    return new View
//                                    {
//                                        Id = reader.GetInt32(0),
//                                        UserNames = reader.GetString(1),
//                                        Department = reader.GetString(2),
//                                        Email = reader.GetString(3),
//                                        Matric_Number = reader.GetString(4),
//                                        Password = storedPassword,
//                                        CourseCode = reader.GetString(5),
//                                        ImageData = reader.IsDBNull(reader.GetOrdinal("ImageData")) ? null : (byte[])reader["ImageData"]
//                                        //CourseCode = reader.IsDBNull(reader.GetOrdinal("Course_code")) ? null : reader.GetString("Course_code")
//                                    };
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            catch (MySqlException ex)
//            {
//                _logger.LogError(ex, "Error fetching user credentials for Email {Email}", email);
//                throw;
//            }
//            return null;
//        }

//        public async Task<bool> RegisterCredentialsAsync(View view)
//        {
//            try
//            {
//                using (var connection = new MySqlConnection(_connectionString))
//                {
//                    await connection.OpenAsync();
//                    using (var command = new MySqlCommand(
//                        "INSERT INTO details (UserNames, Department, Email, Matric_Number, Password, Course_code) " +
//                        "VALUES (@UserNames, @Department, @Email, @Matric_Number, @Password, @CourseCode)", connection))
//                    {
//                        command.Parameters.AddWithValue("@UserNames", view.UserNames);
//                        command.Parameters.AddWithValue("@Department", view.Department);
//                        command.Parameters.AddWithValue("@Email", view.Email);
//                        command.Parameters.AddWithValue("@Matric_Number", view.Matric_Number);
//                        command.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(view.Password));
//                        command.Parameters.AddWithValue("@CourseCode", (object)view.CourseCode ?? DBNull.Value);
//                        return await command.ExecuteNonQueryAsync() > 0;
//                    }
//                }
//            }
//            catch (MySqlException ex)
//            {
//                _logger.LogError(ex, "Error registering user with Email {Email}", view.Email);
//                throw;
//            }
//        }
//    }
//}










using I_Attend.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace I_Attend.Data
{
    public interface IAttendDAO
    {
        Task<List<View>> GetViewsAsync();
        Task AddViewAsync(View view);
        Task UpdateViewAsync(View view);
        Task DeleteViewAsync(int id);
        Task<bool> SaveChangesAsync(View view);
        Task<View> GetByIdAsync(int id);
        Task<View> GetUserCredentialsAsync(string email, string password, string matricNumber);
        Task<bool> RegisterCredentialsAsync(View view);
        Task<bool> ViewExistsAsync(int id);
        Task<bool> AddStudentImageAsync(string matricNumber, byte[] imageData);
        DataTable GetData();
    }

    public class I_AttendDAO : IAttendDAO
    {
        //MySql.Data.MySqlClient.MySqlConnection connection;
        //readonly String connectionString = "Server=db26011.databaseasp.net; Uid=db26011; Pwd=5d=N-Xc9e3L#; Database=db26011;";
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
                    using (var command = new MySqlCommand("SELECT Id, UserNames, Department, Course_code, Email, Matric_Number, Password, ImageData FROM details", connection))
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
                                    Course_code = reader.IsDBNull(reader.GetOrdinal("Course_code")) ? null : reader.GetString(3),
                                    Email = reader.GetString(4),
                                    Matric_Number = reader.GetString(5),
                                    Password = reader.GetString(6),
                                    ImageData = reader.IsDBNull(reader.GetOrdinal("ImageData")) ? null : (byte[])reader["ImageData"]
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
        public async Task<bool> AddStudentImageAsync(string matricNumber, byte[] imageData)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(
                        "UPDATE details SET ImageData = @ImageData WHERE Matric_Number = @Matric_Number", connection))
                    {
                        command.Parameters.AddWithValue("@ImageData", imageData);
                        command.Parameters.AddWithValue("@Matric_Number", matricNumber);
                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Error adding student image for Matric_Number {Matric_Number}", matricNumber);
                throw;
            }
        }

        public async Task AddViewAsync(View view)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(
                        "UPDATE details SET Course_code = @Course_code, Matric_Number = @Matric_Number WHERE Matric_Number = @Matric_Number", connection))
                    {
                        //command.Parameters.AddWithValue("@UserNames", view.UserNames);
                        //command.Parameters.AddWithValue("@Department", view.Department);
                        command.Parameters.AddWithValue("@Course_code", view.Course_code);
                        //command.Parameters.AddWithValue("@Email", view.Email);
                        command.Parameters.AddWithValue("@Matric_Number", view.Matric_Number);
                        //command.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(view.Password));
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
                        "UPDATE details SET UserNames = @UserNames, Department = @Department, Course_code = @Course_code, Email = @Email, Matric_Number = @Matric_Number, Password = @Password" +
                        "WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", view.Id);
                        command.Parameters.AddWithValue("@UserNames", view.UserNames);
                        command.Parameters.AddWithValue("@Department", view.Department);
                        command.Parameters.AddWithValue("@Course_code", view.Course_code);
                        command.Parameters.AddWithValue("@Email", view.Email);
                        command.Parameters.AddWithValue("@Matric_Number", view.Matric_Number);
                        command.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(view.Password));
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

        public async Task<bool> SaveChangesAsync(View view)
        {
            try
            {
                View initialview = await GetByIdAsync(view.Id);
                if (initialview == null)
                {
                    return false;
                }

                string passwordParam = initialview.Password;
                if (!string.IsNullOrEmpty(view.Password) && view.Password != initialview.Password)
                {
                    passwordParam = BCrypt.Net.BCrypt.HashPassword(view.Password);
                }

                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(
                        "UPDATE details SET UserNames = @UserNames, Department = @Department, Course_code = @Course_code, Email = @Email, Matric_Number = @Matric_Number, Password = @Password " +
                        "WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", view.Id);
                        command.Parameters.AddWithValue("@UserNames", view.UserNames ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Department", view.Department ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Course_code", (object)view.Course_code ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Email", view.Email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Matric_Number", view.Matric_Number ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Password", passwordParam);
                        return await command.ExecuteNonQueryAsync() > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Error Saving changes to ID {Id}", view.Id);
                throw;
            }
        }

        public async Task<View> GetByIdAsync(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(
                        "SELECT Id, UserNames, Department, Course_code, Email, Matric_Number, Password FROM details WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new View
                                {
                                    Id = reader.GetInt32("Id"),
                                    UserNames = reader.IsDBNull("UserNames") ? null : reader.GetString("UserNames"),
                                    Department = reader.IsDBNull("Department") ? null : reader.GetString("Department"),
                                    Course_code = reader.IsDBNull("Course_code") ? null : reader.GetString("Course_code"),
                                    Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                                    Matric_Number = reader.IsDBNull("Matric_Number") ? null : reader.GetString("Matric_Number"),
                                    Password = reader.IsDBNull("Password") ? null : reader.GetString("Password")
                                };
                            }
                        }
                    }
                    return null;
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Error fetching content of Id from database");
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
                        "SELECT Id, UserNames, Department, Course_code, Email, Matric_Number, Password, ImageData FROM details " +
                        "WHERE Email = @Email AND Matric_Number = @Matric_Number", connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Matric_Number", matricNumber);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var storedPassword = reader.GetString(6);
                                if (BCrypt.Net.BCrypt.Verify(password, storedPassword))
                                {
                                    return new View
                                    {
                                        Id = reader.GetInt32(0),
                                        UserNames = reader.GetString(1),
                                        Department = reader.GetString(2),
                                        Course_code = reader.IsDBNull(reader.GetOrdinal("Course_code")) ? null : reader.GetString(3),
                                        Email = reader.GetString(4),
                                        Matric_Number = reader.GetString(5),
                                        Password = storedPassword,
                                        ImageData = reader.IsDBNull(reader.GetOrdinal("ImageData")) ? null : (byte[])reader["ImageData"]
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
                        "INSERT INTO details (UserNames, Department, Course_code, Email, Matric_Number, Password) " +
                        "VALUES (@UserNames, @Department, @Course_code, @Email, @Matric_Number, @Password)", connection))
                    {
                        command.Parameters.AddWithValue("@UserNames", view.UserNames);
                        command.Parameters.AddWithValue("@Department", view.Department);
                        command.Parameters.AddWithValue("@Course_code", view.Course_code);
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

        public DataTable GetData() 
        {
            var dt = new DataTable();
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand("SELECT * FROM ECE5251", connection))
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }

                }
                return dt;
            }
            catch (MySqlException ex) 
            {
                _logger.LogError(ex, "Error fetching data from database");
                throw;
            }
        }
    }
}
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Test1.Models;

namespace Test1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private List<Dictionary<string, object>> ConvertDataTableToList(DataTable dt)
        {
            var columns = dt.Columns.Cast<DataColumn>();
            return dt.Rows.Cast<DataRow>()
                .Select(row => columns.ToDictionary(column => column.ColumnName, column => row[column])).ToList();
        }

        private readonly string connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True";

        /*[HttpGet]
        public IActionResult GetTasks(string orderBy = "name")
        {
            string query = $"SELECT * FROM Task ORDER BY  {orderBy}";
            DataTable table = new DataTable();
            using (SqlConnection mycon = new SqlConnection(connectionString))
            {
                mycon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, mycon))
                {
                    using (SqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        table.Load(myReader);
                    }
                }
                mycon.Close();
            }
            var list = ConvertDataTableToList(table);
            return Ok(list);
        }*/


        [HttpGet]
        public IActionResult GetTasks(int projectId)
        {
            try
            {
                string query = @"SELECT Task.*, Project.Name AS ProjectName, 
                                     Creator.LastName AS CreatorLastName, 
                                     AssignedTo.LastName AS AssignedToLastName, 
                                     TaskType.Name AS TaskTypeName
                             FROM Task
                             INNER JOIN Project ON Task.IdProject = Project.IdProject
                             INNER JOIN TeamMember AS Creator ON Task.IdCreator = Creator.IdTeamMember
                             INNER JOIN TeamMember AS AssignedTo ON Task.IdAssignedTo = AssignedTo.IdTeamMember
                             INNER JOIN TaskType ON Task.IdTaskType = TaskType.IdTaskType
                             ORDER BY Task.Deadline ASC";

                DataTable table = new DataTable();
                using (SqlConnection mycon = new SqlConnection(connectionString))
                {
                    mycon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, mycon))
                    {
                        using (SqlDataReader myReader = myCommand.ExecuteReader())
                        {
                            table.Load(myReader);
                        }
                    }
                    mycon.Close();

                    var tasks = ConvertDataTableToList(table);
                    return Ok(tasks);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occured: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult AddTask([FromBody] TaskRequest request)
        {
            using (SqlConnection mycon = new SqlConnection(connectionString))
            {
                mycon.Open();
                var transaction = mycon.BeginTransaction();
                try
                {


                    var InsertTask = mycon.CreateCommand();
                    InsertTask.Transaction = transaction;
                    InsertTask.Parameters.AddWithValue("@Name", request.Name);
                    InsertTask.Parameters.AddWithValue("@Description", request.Description);
                    InsertTask.Parameters.AddWithValue("@Deadline", request.Deadline);
                    InsertTask.Parameters.AddWithValue("@IdProject", request.IdProject);
                    InsertTask.Parameters.AddWithValue("@IdTaskType", request.IdTaskType);
                    InsertTask.Parameters.AddWithValue("@IdAssignedTo", request.IdAssignedTo);
                    InsertTask.Parameters.AddWithValue("@IdCreator", request.IdCreator);
                    InsertTask.CommandText = (@"INSERT INTO Task (Name, Description, Deadline, IdProject, IdTaskType, IdAssignedTo, IdCreator)" +
                                               "VALUES (@Name, @Description, @Deadline, @IdProject, @IdTaskType, @IdAssignedTo, @IdCreator)");

                    InsertTask.ExecuteNonQuery();

                    var getTaskName = mycon.CreateCommand();
                    getTaskName.Transaction = transaction;
                    getTaskName.Parameters.AddWithValue("@Name", request.Name);
                    getTaskName.CommandText = @"SELECT Name FROM Task WHERE Name = @Name";

                    object result = getTaskName.ExecuteScalar();
                    string name = result.ToString();
                    transaction.Commit();
                    return Ok(name);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, $"An error occured {ex.Message}");
                }
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTeamMember(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string checkQuery = $"SELECT 1 FROM TeamMember WHERE IdTeamMember = {id}";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection, transaction);
                    object result = checkCommand.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        transaction.Rollback();
                        return NotFound($"Team member with id {id} not found.");
                    }

                    string deleteQuery = $"DELETE FROM TeamMember WHERE IdTeamMember = {id}";
                    SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection, transaction);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        transaction.Rollback();
                        return BadRequest($"Failed to delete team member with id {id}.");
                    }

                    transaction.Commit();
                    return Ok($"Team member with id {id} deleted successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }

            }
        }
    }
}

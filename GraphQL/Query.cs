using Dapper;
using Microsoft.Data.SqlClient;
using ProjectmanagementAPI.Models;
using System.Data;
using System.Linq;

namespace ProjectmanagementAPI;

public class Query
{
    public async Task<Project?> GetProjectDetails([Service] IConfiguration config, int projectId)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));

        using var multi = await connection.QueryMultipleAsync(
            "dbo.usp_GetProjectDetails",
            new { ProjectID = projectId },
            commandType: CommandType.StoredProcedure
        );

        var project = await multi.ReadFirstOrDefaultAsync<Project>();
        if (project != null)
        {
            project.Tasks = (await multi.ReadAsync<ProjectmanagementAPI.Models.Task>()).ToList();
        }

        return project;
    }
}
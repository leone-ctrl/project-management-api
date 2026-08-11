using HotChocolate.Execution;
using Microsoft.Data.SqlClient;

namespace ProjectmanagementAPI;

public class GraphQLErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        // Catch SQL Server Exceptions (e.g., RAISERROR from Stored Procedures)
        if (error.Exception is SqlException sqlException)
        {
            return error
                .WithMessage(sqlException.Message)
                .WithCode("DATABASE_ERROR");
        }

        return error;
    }
}
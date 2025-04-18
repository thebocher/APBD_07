using Containers.Models;
using Microsoft.Data.SqlClient;

namespace Containers.Application;

public class ContainerService : IContainerService
{
    private string _connectionString;
    
    public ContainerService(string connectionString)
    {
        _connectionString = connectionString;        
    }
    
    public IEnumerable<Container> GetAllContainers()
    {
        List<Container> containers = [];
        const string queryString = "SELECT * FROM Containers";

        using (var connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var containerRow = new Container
                        {
                            Id = reader.GetInt32(0),
                            ContainerTypeId = reader.GetInt32(1),
                            IsHazardous = reader.GetBoolean(2),
                            Name = reader.GetString(3),
                        };
                        containers.Add(containerRow);
                    }
                }
                
                return containers;
            }
            finally 
            {
                reader.Close();
            }
        }
    }

    public bool AddContainer(Container container)
    {
        const string insertString = @"
            INSERT INTO Containers (CotnainerTypeId, isHazardous, Name) 
            VALUES (@ContainerTypeId, @IsHazardous, @Name)
        ";

        int countRowsAdded = -1;

        using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(insertString, sqlConnection);
            command.Parameters.AddWithValue("@ContainerTypeId", container.ContainerTypeId);
            command.Parameters.AddWithValue("@IsHazardous", container.IsHazardous);
            command.Parameters.AddWithValue("@Name", container.Name);
            
            sqlConnection.Open();
            
            countRowsAdded = command.ExecuteNonQuery();
        }

        return countRowsAdded > 0;
    }
}
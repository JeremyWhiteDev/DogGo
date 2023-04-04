using DogGo.Models;
using Microsoft.Data.SqlClient;

namespace DogGo.Repositories;

public class OwnerRepository : IOwnerRepository
{
    private readonly IConfiguration _config;

    public OwnerRepository(IConfiguration config)
    {
        _config = config;
    }

    public SqlConnection Connection
    {
        get
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }
    }

    public List<Owner> GetAllOwners()
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                        SELECT [Owner].[Id]
                              ,[Email]
                              ,[Owner].[Name]
                              ,[Address]
                              ,[NeighborhoodId]
	                          ,[Neighborhood].[Name] AS NeighborhoodName
                              ,[Phone]
                          FROM [DogWalkerMVC].[dbo].[Owner]
                          LEFT JOIN Neighborhood 
                          ON [Owner].NeighborhoodId = [Neighborhood].Id
                    ";

                SqlDataReader reader = cmd.ExecuteReader();

                List<Owner> owners = new List<Owner>();
                while (reader.Read())
                {
                    Owner owner = new Owner
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Address = reader.GetString(reader.GetOrdinal("Address")),
                        Phone = reader.GetString(reader.GetOrdinal("Phone")),
                        NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                        Neighborhood = new Neighborhood()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Name = reader.GetString(reader.GetOrdinal("NeighborhoodName")),
                        }
                    };

                    owners.Add(owner);
                }

                reader.Close();

                return owners;
            }
        }
    }
    public Owner GetOwnerById(int id)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                        SELECT [Owner].[Id]
                              ,[Email]
                              ,[Owner].[Name]
                              ,[Address]
                              ,[NeighborhoodId]
	                          ,[Neighborhood].[Name] AS NeighborhoodName
                              ,[Phone]
                          FROM [DogWalkerMVC].[dbo].[Owner]
                          LEFT JOIN Neighborhood 
                          ON [Owner].NeighborhoodId = [Neighborhood].Id
                          WHERE [Owner].[Id] = @Id
                    ";

                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                Owner owner = null;
                if (reader.Read())
                {
                    owner = new Owner
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Address = reader.GetString(reader.GetOrdinal("Address")),
                        Phone = reader.GetString(reader.GetOrdinal("Phone")),
                        NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                        Neighborhood = new Neighborhood()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Name = reader.GetString(reader.GetOrdinal("NeighborhoodName")),
                        }
                    };

                }

                reader.Close();

                return owner;
            }
        }
    }
}

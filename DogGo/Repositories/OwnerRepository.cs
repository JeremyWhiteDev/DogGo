using DogGo.Models;
using Microsoft.Data.SqlClient;
using System.Data.Common;

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
                              ,[Dog].[Id] AS DogId
	                          ,[Dog].[Name] AS DogName
	                          ,[Dog].[Breed]
	                          ,[Dog].[Notes]
	                          ,[Dog].[ImageURL]
                          FROM [DogWalkerMVC].[dbo].[Owner]
                          LEFT JOIN Neighborhood 
                          ON [Owner].NeighborhoodId = [Neighborhood].Id
                          LEFT JOIN Dog 
                          ON [Owner].Id = Dog.OwnerId
                          WHERE [Owner].[Id] = @Id
                    ";

                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                Owner owner = null;
                while (reader.Read())
                {
                    if (owner == null)
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
                            },
                            Dogs = new List<Dog>()
                        };
                    }
                    int dogId = reader.GetInt32(reader.GetOrdinal("DogId"));
                   if (dogId != null)
                    {
                        owner.Dogs.Add(new Dog()
                        {
                            Id = dogId,
                            Name = reader.GetString(reader.GetOrdinal("DogName")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("Id")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("Breed")) ? null : reader.GetString(reader.GetOrdinal("Breed")),
                            ImageURL = reader.IsDBNull(reader.GetOrdinal("ImageURL")) ? null : reader.GetString(reader.GetOrdinal("ImageURL"))
                        });
                    }

                }

                reader.Close();

                return owner;
            }
        }
    }
}

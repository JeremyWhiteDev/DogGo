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
                    int? dogId = reader.IsDBNull(reader.GetOrdinal("DogId")) ? null : reader.GetInt32(reader.GetOrdinal("DogId"));
                   if (dogId != null)
                    {
                        owner.Dogs.Add(new Dog()
                        {
                            Id = (int)dogId,
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

    public Owner GetOwnerByEmail(string email)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                        SELECT Id, [Name], Email, Address, Phone, NeighborhoodId
                        FROM Owner
                        WHERE Email = @email";

                cmd.Parameters.AddWithValue("@email", email);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Owner owner = new Owner()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Address = reader.GetString(reader.GetOrdinal("Address")),
                        Phone = reader.GetString(reader.GetOrdinal("Phone")),
                        NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                    };

                    reader.Close();
                    return owner;
                }

                reader.Close();
                return null;
            }
        }
    }

    public void AddOwner(Owner owner)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO Owner ([Name], Email, Phone, Address, NeighborhoodId)
                    OUTPUT INSERTED.ID
                    VALUES (@name, @email, @phoneNumber, @address, @neighborhoodId);
                ";

                cmd.Parameters.AddWithValue("@name", owner.Name);
                cmd.Parameters.AddWithValue("@email", owner.Email);
                cmd.Parameters.AddWithValue("@phoneNumber", owner.Phone);
                cmd.Parameters.AddWithValue("@address", owner.Address);
                cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId);

                int id = (int)cmd.ExecuteScalar();

                owner.Id = id;
            }
        }
    }

    public void UpdateOwner(Owner owner)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                            UPDATE Owner
                            SET 
                                [Name] = @name, 
                                Email = @email, 
                                Address = @address, 
                                Phone = @phone, 
                                NeighborhoodId = @neighborhoodId
                            WHERE Id = @id";

                cmd.Parameters.AddWithValue("@name", owner.Name);
                cmd.Parameters.AddWithValue("@email", owner.Email);
                cmd.Parameters.AddWithValue("@address", owner.Address);
                cmd.Parameters.AddWithValue("@phone", owner.Phone);
                cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId);
                cmd.Parameters.AddWithValue("@id", owner.Id);

                cmd.ExecuteNonQuery();
            }
        }
    }

    public void DeleteOwner(int ownerId)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                            DELETE FROM Owner
                            WHERE Id = @id
                        ";

                cmd.Parameters.AddWithValue("@id", ownerId);

                cmd.ExecuteNonQuery();
            }
        }
    }
}

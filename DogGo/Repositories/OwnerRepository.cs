using DogGo.Models;
using DogGo.Models.ViewModels;
using DogGo.Utils;
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
                cmd.CommandText = $@"
                        SELECT {Owner.COLUMNS.SelectAll},
                              {Neighborhood.COLUMNS.SelectAll},
                              {Dog.COLUMNS.SelectAll},
                              {Walker.COLUMNS.SelectAll}
                          FROM {Owner.TABLE.Name}
                          LEFT JOIN {Neighborhood.TABLE.Name} 
                          ON {Owner.COLUMNS.NeighborhoodId} = {Neighborhood.COLUMNS.Id}
                          LEFT JOIN {Dog.TABLE.Name} 
                          ON {Owner.COLUMNS.Id} = {Dog.COLUMNS.OwnerId}
                          LEFT JOIN {Walker.TABLE.Name}
                          ON {Owner.COLUMNS.NeighborhoodId} = {Walker.COLUMNS.NeighborhoodId}
        
                          WHERE {Owner.COLUMNS.Id} = @Id
                    ";

                cmd.Parameters.AddWithValue("@Id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                Owner owner = null;
                while (reader.Read())
                {
          
                    if (owner == null)
                    {
                    owner= new Owner(reader);
                    }

                    var dogId = DbUtils.GetNullableInt(reader, Dog.COLUMNS.Id);
                    var existingDog = owner.Dogs.FirstOrDefault(d => d.Id == dogId);

                    if (owner !=null && dogId != null && existingDog == null)
                    {
                        owner.Dogs.Add(new Dog(reader, false));
                    }

                    var walkerId = DbUtils.GetNullableInt(reader, Walker.COLUMNS.Id);
                    var existingWalker = owner.NeighborhoodWalkers.FirstOrDefault(w => w.Id == walkerId);
                    if (owner != null && walkerId != null && existingWalker == null)
                    {
                        owner.NeighborhoodWalkers.Add(new Walker(reader, false));
                    }


                }
                reader.Close();

                return owner;
            }
        }
    }


    public OwnerFormViewModel GetOwnerByIdWithAllNeighborhoods(int id)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = $@"
                                    SELECT 
                                    {Owner.COLUMNS.SelectAll},
                                    {Neighborhood.COLUMNS.SelectAll}
                                    FROM
                                    {Owner.TABLE.Name},
                                    {Neighborhood.TABLE.Name}
                                    WHERE 
                                    {Owner.COLUMNS.Id} = @Id";
                cmd.Parameters.AddWithValue("@Id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                OwnerFormViewModel model = new OwnerFormViewModel();

                if (reader.Read())
                {
                    if (DbUtils.GetNullableInt(reader, Owner.COLUMNS.Id) == null)
                    {
                        return model;
                    }
                    if (model.Owner == null)
                    {
                        model.Owner = new Owner(reader);
                    }
                    var neighborhooodId = DbUtils.GetNullableInt(reader, Neighborhood.COLUMNS.Id);
                    var existingNeighborhood = model.Neighborhoods.FirstOrDefault(w => w.Id == neighborhooodId);
                    if (existingNeighborhood == null)
                    {
                        model.Neighborhoods.Add(new Neighborhood(reader));
                    }

                }
                reader.Close();
                return model;
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

using DogGo.Models;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;

namespace DogGo.Repositories;

public class DogRepository : IDogRepository
{
    private readonly IConfiguration _config;

    public DogRepository(IConfiguration config)
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


    public List<Dog> GetAllDogs()
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $@"
                                    SELECT {Dog.COLUMNS.SelectAll}
                                    FROM {Dog.TABLE.Name}";
                var reader = cmd.ExecuteReader();
                List<Dog> dogs = new List<Dog>();
                while (reader.Read())
                {
                    dogs.Add(new Dog(reader, true));
                };

                reader.Close();
                return dogs;    
            }
        }
      
    }
 
    

    public Dog GetById(int id)
    {

    using (var conn = Connection)
    {
        conn.Open();
        using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $@"
                                    SELECT {Dog.COLUMNS.SelectAll}
                                    FROM {Dog.TABLE.Name}
                                    WHERE {Dog.COLUMNS.Id} = @Id";

                cmd.Parameters.AddWithValue("@Id", id);
                var reader = cmd.ExecuteReader();

                Dog dog = null;
                if (reader.Read())
                {
                    dog = new Dog(reader, true);
                }
                reader.Close();
                 return dog;
            }

        }
    }

    public List<Dog> GetDogsByOwnerId(int ownerId)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                SELECT Id, Name, Breed, Notes, ImageUrl, OwnerId 
                FROM Dog
                WHERE OwnerId = @ownerId
            ";

                cmd.Parameters.AddWithValue("@ownerId", ownerId);

                SqlDataReader reader = cmd.ExecuteReader();

                List<Dog> dogs = new List<Dog>();

                while (reader.Read())
                {
                    Dog dog = new Dog()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Breed = reader.GetString(reader.GetOrdinal("Breed")),
                        OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"))
                    };

                    // Check if optional columns are null
                    if (reader.IsDBNull(reader.GetOrdinal("Notes")) == false)
                    {
                        dog.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("ImageUrl")) == false)
                    {
                        dog.ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"));
                    }

                    dogs.Add(dog);
                }
                reader.Close();
                return dogs;
            }
        }
    }

    public void Add(Dog dog)
    {

    }

    public void Update(Dog dog)
    {

    }

    public void DeleteById(int id)
    {

    }
}

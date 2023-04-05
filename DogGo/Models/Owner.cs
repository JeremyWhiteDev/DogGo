using DogGo.Utils;
using Microsoft.Data.SqlClient;

namespace DogGo.Models;

public class Owner
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public int NeighborhoodId { get; set; }
    public Neighborhood Neighborhood { get; set; }  
    public string Phone { get; set; }   
    public List<Dog> Dogs { get; set; }
    public List<Walker> NeighborhoodWalkers { get; set; }


    public static class TABLE
    {
        public static string Name = "[DogWalkerMVC].[dbo].[Owner]";
    }

    public static class COLUMNS
    {
        public static readonly string Id = $"{TABLE.Name}.[Id]";
        public static readonly string Name = $"{TABLE.Name}.[Name]";
        public static readonly string Email = $"{TABLE.Name}.[Email]";
        public static readonly string Address = $"{TABLE.Name}.[Address]";
        public static readonly string Phone = $"{TABLE.Name}.[Address]";
        public static readonly string NeighborhoodId = $"{TABLE.Name}.[NeighborhoodId]";

        public static readonly string SelectAll = $@"{Id} AS '{Id}',
                              {Email} AS '{Email}',
                              {Name} AS '{Name}',
                              {Address} AS '{Address}', 
                              {NeighborhoodId} AS  '{NeighborhoodId}',
                              {Phone} AS '{Phone}'";


    }

   

    public Owner(SqlDataReader reader)
    {
        Id = DbUtils.GetInt(reader, COLUMNS.Id);
        Email = DbUtils.GetString(reader, COLUMNS.Email);
        Name = DbUtils.GetString(reader, COLUMNS.Name);
        Address = DbUtils.GetString(reader, COLUMNS.Address);
        Phone = DbUtils.GetString(reader, COLUMNS.Phone);
        NeighborhoodId = DbUtils.GetInt(reader, COLUMNS.NeighborhoodId);
        Neighborhood = new Neighborhood(reader);
        Dogs = new List<Dog>();
        NeighborhoodWalkers = new List<Walker>();

    }

    public Owner() { }
}
    

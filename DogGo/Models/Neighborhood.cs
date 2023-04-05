using DogGo.Utils;
using Microsoft.Data.SqlClient;

namespace DogGo.Models;

public class Neighborhood
{
    public int Id { get; set; }
    public string Name { get; set; }

    public static class TABLE
    {
        public static string Name = "[DogWalkerMVC].[dbo].[Neighborhood]";
    }

    public static class COLUMNS
    {
        public static readonly string Id = $"{TABLE.Name}.[Id]";
        public static readonly string Name = $"{TABLE.Name}.[Name]";

        public static readonly string SelectAll = $@"{Id} AS '{Id}',
	                                                 {Name} AS '{Name}'";
    }

    public Neighborhood(SqlDataReader reader)
    {
        Id = DbUtils.GetInt(reader, COLUMNS.Id);
        Name = DbUtils.GetString(reader, COLUMNS.Name);
    }


    public Neighborhood() { }
}
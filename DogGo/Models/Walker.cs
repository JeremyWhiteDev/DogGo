using DogGo.Utils;
using Microsoft.Data.SqlClient;
using static Azure.Core.HttpHeader;
using System.Drawing.Drawing2D;

namespace DogGo.Models;

public class Walker
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int NeighborhoodId { get; set; }
    public string ImageUrl { get; set; }
    public Neighborhood Neighborhood { get; set; }

    public static class TABLE
    {
        public static readonly string Name = "[DogWalkerMVC].[dbo].[Walker]";
    }

    public static class COLUMNS
    {
        public static readonly string Id = $"{TABLE.Name}.[Id]";
        public static readonly string Name = $"{TABLE.Name}.[Name]";
        public static readonly string NeighborhoodId = $"{TABLE.Name}.[NeighborhoodId]";
        public static readonly string ImageUrl = $"{TABLE.Name}.[ImageUrl]";

        public static readonly string SelectAll = $@"{Id} AS '{Id}',
                                                       {Name} AS '{Name}',
	                                                   {NeighborhoodId} AS '{NeighborhoodId}',
	                                                   {ImageUrl} AS '{ImageUrl}'";
    }
    public Walker(SqlDataReader reader, bool withNeighborhood)
    {
        Id = DbUtils.GetInt(reader, COLUMNS.Id);
        Name = DbUtils.GetString(reader, COLUMNS.Name);
        NeighborhoodId = DbUtils.GetInt(reader, COLUMNS.NeighborhoodId);
        ImageUrl = DbUtils.GetNullableString(reader, COLUMNS.ImageUrl);
        if (withNeighborhood) Neighborhood = new Neighborhood(reader);
    }
    public Walker() { }

}


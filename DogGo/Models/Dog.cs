using DogGo.Utils;
using Microsoft.Data.SqlClient;

namespace DogGo.Models;

public class Dog
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int OwnerId { get; set; }
    public Owner? Owner { get; set; } = null;
    public string Breed { get; set; }
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }

    public static class TABLE
    {
        public static string Name = "[DogWalkerMVC].[dbo].[Dog]";
    }

    public static class COLUMNS
    {
        public static readonly string Id = $"{TABLE.Name}.[Id]";
        public static readonly string Name = $"{TABLE.Name}.[Name]";
        public static readonly string OwnerId = $"{TABLE.Name}.[OwnerId]";
        public static readonly string Breed = $"{TABLE.Name}.[Breed]";
        public static readonly string Notes = $"{TABLE.Name}.[Notes]";
        public static readonly string ImageUrl = $"{TABLE.Name}.ImageUrl";

        public static readonly string SelectAll = $@"  {Id} AS '{Id}',
                                                       {OwnerId} AS '{OwnerId}',
	                                                   {Name} AS '{Name}',
	                                                   {Breed} AS '{Breed}',
	                                                   {Notes} AS '{Notes}',
	                                                   {ImageUrl} AS '{ImageUrl}'";

    }


    /// <summary>
    ///  Creates a dog using the sql data reader and an option to include an owner. WARNING. Improperly including withOwner can cause a stack overflow!.
    /// </summary>
    /// <param name="reader">A SqlDataReader that has not exhausted it's result set.</param>
    /// <param name="withOwner">A boolean of whether the child element owner should be included
    /// 
    /// WARNING. Improperly including withOwner can cause a stack overflow!.</param>

    public Dog(SqlDataReader reader, bool withOwner)
    {
        Id = DbUtils.GetInt(reader, COLUMNS.Id);
        Name = DbUtils.GetString(reader, COLUMNS.Name);
        OwnerId = DbUtils.GetInt(reader, COLUMNS.OwnerId);
        Breed = DbUtils.GetString(reader, COLUMNS.Breed);
        Notes = DbUtils.GetNullableString(reader, COLUMNS.Notes);
        ImageUrl = DbUtils.GetNullableString(reader, COLUMNS.ImageUrl);
        if (withOwner) Owner = new Owner(reader);

    }

    public Dog() { }
}

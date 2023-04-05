using System.Collections.Generic;

namespace DogGo.Models.ViewModels;

public class OwnerFormViewModel
{
    public Owner Owner { get; set; } = null;
    public List<Neighborhood> Neighborhoods { get; set; } = new List<Neighborhood>();
}
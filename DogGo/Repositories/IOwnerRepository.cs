using DogGo.Models;
using DogGo.Models.ViewModels;

namespace DogGo.Repositories
{
    public interface IOwnerRepository
    {
        List<Owner> GetAllOwners();
        Owner GetOwnerById(int id);
        Owner GetOwnerByEmail(string email);
        void AddOwner(Owner owner);
        void UpdateOwner(Owner owner);
        void DeleteOwner(int ownerId);
        OwnerFormViewModel GetOwnerByIdWithAllNeighborhoods(int id);
    }
}
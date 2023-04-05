using DogGo.Models;

namespace DogGo.Repositories
{
    public interface IDogRepository
    {
        void DeleteById(int id);
        List<Dog> GetAllDogs();
        Dog GetById(int id);

        List<Dog> GetDogsByOwnerId(int ownerId);
        void Update(Dog dog);

        void Add(Dog dog);
    }
}
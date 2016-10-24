using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();
        IEnumerable<Trip> GetTripsByUsername(string name);
        Trip GetTripByName(string tripName);
        Trip GetUserTripByName(string tripName, string userName);

        void AddTrip(Trip trip);
        void AddStop(string tripName, string userName, Stop stop);

        Task<bool> SaveChangesAsync();
    }
}
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public class WorldRepository: IWorldRepository
    {
        private WorldContext _context;

        public WorldRepository(WorldContext context)
        {
            _context = context;
        }

        public void AddStop(string tripName, string userName, Stop stop)
        {
            var trip = GetUserTripByName(tripName, userName);
            if (trip != null)
            {
                trip.Stops.Add(stop);
                _context.Stops.Add(stop);
            }
        }

        public void AddTrip(Trip trip)
        {
            _context.Add(trip);
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            return _context.Trips.ToList();
        }

        public IEnumerable<Trip> GetTripsByUsername(string name)
        {
            return _context.Trips
                .Include(testc => testc.Stops)
                .Where(t => t.UserName == name)
                .ToList();
        }

        public Trip GetUserTripByName(string tripName, string userName)
        {
            return _context.Trips
                .Include(t => t.Stops)
                .Where(t => t.Name == tripName && t.UserName == userName)
                .FirstOrDefault();
        }

        public Trip GetTripByName(string tripName)
        {
            return _context.Trips
                .Include(t => t.Stops)
                .Where(t => t.Name == tripName)
                .FirstOrDefault();
        }

        public async Task<bool> SaveChangesAsync()
        {
            int res = await _context.SaveChangesAsync();
            return res > 0;
        }
    }
}

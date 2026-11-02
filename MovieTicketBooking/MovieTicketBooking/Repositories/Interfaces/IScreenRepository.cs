
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IScreenRepository
{
    Task<Screen> CreateAsync(Screen screen);

    Task<List<Screen>> GetAllAsync(int? theatreId);

    Task<Screen?> GetByIdAsync(int screenId);

    Task<Screen?> UpdateAsync(Screen screen);

    Task<bool> DeleteAsync(int screenId);

    Task<List<Screen>> GetByTheatreIdAsync(int theatreId);
}

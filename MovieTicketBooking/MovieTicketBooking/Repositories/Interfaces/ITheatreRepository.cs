using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface ITheatreRepository
{
    Task<Theatre> CreateAsync(Theatre theatre);

    Task<List<Theatre>> GetAllAsync();

    Task<Theatre?> GetByIdAsync(int theatreId);

    Task<Theatre?> UpdateAsync(Theatre theatre);

    Task<bool> DeleteAsync(int theatreId);
}

using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;
using MovieTicketBooking.Exceptions;

namespace MovieTicketBooking.Services;

public class ShowService : IShowService
{
    private readonly IShowRepository _showRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IScreenRepository _screenRepository;

    public ShowService(
        IShowRepository showRepository,
        IMovieRepository movieRepository,
        IScreenRepository screenRepository)
    {
        _showRepository = showRepository;
        _movieRepository = movieRepository;
        _screenRepository = screenRepository;
    }

    // CREATE SHOW
    public async Task<ShowResponse> CreateAsync(CreateShowRequest request)
    {
        if (request.EndTime <= request.StartTime)
            throw new ValidationException(
                "End time must be later than start time.");

        if (request.TicketPrice < 0)
            throw new ValidationException(
                "Ticket price cannot be negative.");

        var movie = await _movieRepository.GetByIdAsync(request.MovieId);

        if (movie == null)
            throw new NotFoundException("Movie not found.");

        var screen = await _screenRepository.GetByIdAsync(request.ScreenId);

        if (screen == null)
            throw new NotFoundException("Screen not found.");

        bool hasOverlap = await _showRepository.HasOverlapAsync(
            request.ScreenId,
            request.StartTime,
            request.EndTime);

        if (hasOverlap)
            throw new ConflictException(
                "Another show is already scheduled for this screen during this time.");

        // Get all physical seats belonging to this screen.
        var seats = await _showRepository.GetSeatsByScreenIdAsync(
            request.ScreenId);

        if (seats.Count == 0)
            throw new ConflictException(
                "Cannot schedule a show because this screen has no seats.");

        var show = new Show
        {
            MovieId = request.MovieId,
            ScreenId = request.ScreenId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TicketPrice = request.TicketPrice
        };

        var createdShow = await _showRepository.CreateAsync(show);

        // Create an Available ShowSeat record for every physical seat.
        var showSeats = seats.Select(seat => new ShowSeat
        {
            ShowId = createdShow.ShowId,
            SeatId = seat.SeatId,
            Status = "Available"
        }).ToList();

        await _showRepository.CreateShowSeatsAsync(showSeats);

        return MapToResponse(createdShow);
    }

    // GET ALL SHOWS
    public async Task<List<ShowResponse>> GetAllAsync(
        DateTime? date,
        int? movieId,
        int? theatreId)
    {
        var shows = await _showRepository.GetAllAsync(
            date, movieId, theatreId);

        return shows.Select(MapToResponse).ToList();
    }

    // GET SHOW BY ID
    public async Task<ShowResponse?> GetByIdAsync(int showId)
    {
        var show = await _showRepository.GetByIdAsync(showId);

        return show == null ? null : MapToResponse(show);
    }

    // DELETE SHOW
    public async Task<bool> DeleteAsync(int showId)
    {
        var show = await _showRepository.GetByIdAsync(showId);

        if (show == null)
            throw new NotFoundException("Show not found. Cannot delete.");

        bool isDeleted = await _showRepository.DeleteAsync(showId);

        if (!isDeleted)
            throw new ConflictException(
                "Show could not be deleted. It may have related bookings.");

        return true;
    }

    // GET ALL SEATS FOR A SHOW
    public async Task<List<ShowSeatResponse>> GetShowSeatsAsync(int showId)
    {
        var show = await _showRepository.GetByIdAsync(showId);

        if (show == null)
            throw new NotFoundException("Show not found.");

        var showSeats = await _showRepository.GetShowSeatsAsync(showId);

        return showSeats.Select(showSeat => new ShowSeatResponse
        {
            SeatId = showSeat.SeatId,
            RowName = showSeat.Seat.RowName,
            SeatNumber = showSeat.Seat.SeatNumber,
            SeatLabel = $"{showSeat.Seat.RowName}{showSeat.Seat.SeatNumber}",
            SeatType = showSeat.Seat.SeatType,
            Price = show.TicketPrice,
            Status = showSeat.Status
        }).ToList();
    }

    // GET AVAILABLE SEATS
    public async Task<List<ShowSeatResponse>> GetAvailableSeatsAsync(int showId)
    {
        var seats = await GetShowSeatsAsync(showId);

        return seats
            .Where(seat => seat.Status == "Available")
            .ToList();
    }

    // GET BOOKED SEATS
    public async Task<List<ShowSeatResponse>> GetBookedSeatsAsync(int showId)
    {
        var seats = await GetShowSeatsAsync(showId);

        return seats
            .Where(seat => seat.Status == "Booked")
            .ToList();
    }

    // GET SHOWS BY MOVIE
    public async Task<List<ShowResponse>> GetByMovieIdAsync(int movieId)
    {
        var shows = await _showRepository.GetByMovieIdAsync(movieId);

        return shows.Select(MapToResponse).ToList();
    }

    // GET SHOWS BY THEATRE
    public async Task<List<ShowResponse>> GetByTheatreIdAsync(int theatreId)
    {
        var shows = await _showRepository.GetByTheatreIdAsync(theatreId);

        return shows.Select(MapToResponse).ToList();
    }

    // SEARCH SHOWS BY DATE
    public async Task<List<ShowResponse>> SearchByDateAsync(DateTime date)
    {
        var shows = await _showRepository.SearchByDateAsync(date);

        return shows.Select(MapToResponse).ToList();
    }

    // UPDATE SHOW
    public async Task<ShowResponse?> UpdateAsync(
        int showId,
        UpdateShowRequest request)
    {
        var existingShow = await _showRepository.GetByIdAsync(showId);

        if (existingShow == null)
            throw new NotFoundException("Show not found. Cannot update.");

        if (request.EndTime <= request.StartTime)
            throw new ValidationException(
                "End time must be later than start time.");

        if (request.TicketPrice < 0)
            throw new ValidationException(
                "Ticket price cannot be negative.");

        var movie = await _movieRepository.GetByIdAsync(request.MovieId);

        if (movie == null)
            throw new NotFoundException("Movie not found.");

        // Keep the original screen because ShowSeats are linked to its seats.
        bool hasOverlap = await _showRepository.HasOverlapAsync(
            existingShow.ScreenId,
            request.StartTime,
            request.EndTime,
            showId);

        if (hasOverlap)
            throw new ConflictException(
                "Another show is already scheduled for this screen during this time.");

        existingShow.MovieId = request.MovieId;
        existingShow.StartTime = request.StartTime;
        existingShow.EndTime = request.EndTime;
        existingShow.TicketPrice = request.TicketPrice;

        var updatedShow = await _showRepository.UpdateAsync(existingShow);

        if (updatedShow == null)
            throw new NotFoundException("Show not found. Cannot update.");

        return MapToResponse(updatedShow);
    }

    // MAP SHOW ENTITY TO RESPONSE DTO
    private static ShowResponse MapToResponse(Show show)
    {
        return new ShowResponse
        {
            ShowId = show.ShowId,
            MovieId = show.MovieId,
            ScreenId = show.ScreenId,
            StartTime = show.StartTime,
            EndTime = show.EndTime,
            TicketPrice = show.TicketPrice
        };
    }
}

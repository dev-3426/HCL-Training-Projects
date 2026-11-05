
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.Authorization.Requirements;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Authorization.Handlers;

public class BookingOwnerHandler
    : AuthorizationHandler<BookingOwnerRequirement, int>
{
    private readonly IBookingRepository _bookingRepository;

    public BookingOwnerHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BookingOwnerRequirement requirement,
        int bookingId)
    {
        // Get the logged-in customer's ID from JWT
        var userIdValue = context.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdValue, out int userId))
        {
            return;
        }

        // Retrieve the requested booking
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        // Check whether the logged-in customer owns the booking
        if (booking != null && booking.UserId == userId)
        {
            context.Succeed(requirement);
        }
    }
}

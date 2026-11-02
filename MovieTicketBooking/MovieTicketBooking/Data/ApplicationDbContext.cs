
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Theatre> Theatres { get; set; }
    public DbSet<Screen> Screens { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<Show> Shows { get; set; }
    public DbSet<ShowSeat> ShowSeats { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingSeat> BookingSeats { get; set; }

    // Authentication
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(user => user.Email)
            .HasMaxLength(150)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(user => user.Role)
            .HasMaxLength(20)
            .IsRequired();

        modelBuilder.Entity<Show>()
        .Property(show => show.TicketPrice)
        .HasPrecision(18, 2);

        modelBuilder.Entity<Booking>()
            .Property(booking => booking.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<BookingSeat>()
            .Property(bookingSeat => bookingSeat.Price)
            .HasPrecision(18, 2);


        // Screen - Theatre
        modelBuilder.Entity<Screen>()
            .HasOne(screen => screen.Theatre)
            .WithMany(theatre => theatre.Screens)
            .HasForeignKey(screen => screen.TheatreId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seat - Screen
        modelBuilder.Entity<Seat>()
            .HasOne(seat => seat.Screen)
            .WithMany(screen => screen.Seats)
            .HasForeignKey(seat => seat.ScreenId)
            .OnDelete(DeleteBehavior.Restrict);

        // Show - Movie
        modelBuilder.Entity<Show>()
            .HasOne(show => show.Movie)
            .WithMany(movie => movie.Shows)
            .HasForeignKey(show => show.MovieId)
            .OnDelete(DeleteBehavior.Restrict);

        // Show - Screen
        modelBuilder.Entity<Show>()
            .HasOne(show => show.Screen)
            .WithMany(screen => screen.Shows)
            .HasForeignKey(show => show.ScreenId)
            .OnDelete(DeleteBehavior.Restrict);

        // ShowSeat - Show
        modelBuilder.Entity<ShowSeat>()
            .HasOne(showSeat => showSeat.Show)
            .WithMany(show => show.ShowSeats)
            .HasForeignKey(showSeat => showSeat.ShowId)
            .OnDelete(DeleteBehavior.Restrict);

        // ShowSeat - Seat
        modelBuilder.Entity<ShowSeat>()
            .HasOne(showSeat => showSeat.Seat)
            .WithMany(seat => seat.ShowSeats)
            .HasForeignKey(showSeat => showSeat.SeatId)
            .OnDelete(DeleteBehavior.Restrict);

        // Booking - Show
        modelBuilder.Entity<Booking>()
            .HasOne(booking => booking.Show)
            .WithMany(show => show.Bookings)
            .HasForeignKey(booking => booking.ShowId)
            .OnDelete(DeleteBehavior.Restrict);

        // BookingSeat - Booking
        modelBuilder.Entity<BookingSeat>()
            .HasOne(bookingSeat => bookingSeat.Booking)
            .WithMany(booking => booking.BookingSeats)
            .HasForeignKey(bookingSeat => bookingSeat.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        // BookingSeat - Seat
        modelBuilder.Entity<BookingSeat>()
            .HasOne(bookingSeat => bookingSeat.Seat)
            .WithMany(seat => seat.BookingSeats)
            .HasForeignKey(bookingSeat => bookingSeat.SeatId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
    .HasOne(booking => booking.User)
    .WithMany(user => user.Bookings)
    .HasForeignKey(booking => booking.UserId)
    .OnDelete(DeleteBehavior.Restrict);


        // Unique indexes
        modelBuilder.Entity<Screen>()
            .HasIndex(screen => new
            {
                screen.TheatreId,
                screen.ScreenName
            })
            .IsUnique();

        modelBuilder.Entity<Seat>()
            .HasIndex(seat => new
            {
                seat.ScreenId,
                seat.RowName,
                seat.SeatNumber
            })
            .IsUnique();

        modelBuilder.Entity<ShowSeat>()
            .HasIndex(showSeat => new
            {
                showSeat.ShowId,
                showSeat.SeatId
            })
            .IsUnique();

        modelBuilder.Entity<Booking>()
            .HasIndex(booking => booking.BookingReference)
            .IsUnique();

        // Show constraints
        modelBuilder.Entity<Show>()
            .ToTable(table =>
                table.HasCheckConstraint(
                    "CK_Shows_EndTime",
                    "[EndTime] > [StartTime]"
                ));

        modelBuilder.Entity<Show>()
            .ToTable(table =>
                table.HasCheckConstraint(
                    "CK_Shows_TicketPrice",
                    "[TicketPrice] >= 0"
                ));

        // Booking constraints
        modelBuilder.Entity<Booking>()
            .ToTable(table =>
                table.HasCheckConstraint(
                    "CK_Bookings_TotalAmount",
                    "[TotalAmount] >= 0"
                ));

        modelBuilder.Entity<BookingSeat>()
            .ToTable(table =>
                table.HasCheckConstraint(
                    "CK_BookingSeats_Price",
                    "[Price] >= 0"
                ));
    }
}

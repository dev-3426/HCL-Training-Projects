using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Screen>()
            .HasOne<Theatre>()
            .WithMany()
            .HasForeignKey(screen => screen.TheatreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Seat>()
            .HasOne<Screen>()
            .WithMany()
            .HasForeignKey(seat => seat.ScreenId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Show>()
            .HasOne<Movie>()
            .WithMany()
            .HasForeignKey(show => show.MovieId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShowSeat>()
            .HasOne<Show>()
            .WithMany()
            .HasForeignKey(showSeat => showSeat.ShowId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShowSeat>()
            .HasOne<Seat>()
            .WithMany()
            .HasForeignKey(showSeat => showSeat.SeatId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne<Show>()
            .WithMany()
            .HasForeignKey(booking => booking.ShowId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BookingSeat>()
            .HasOne<Booking>()
            .WithMany()
            .HasForeignKey(bookingSeat => bookingSeat.BookingId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<BookingSeat>()
            .HasOne<Seat>()
            .WithMany()
            .HasForeignKey(bookingSeat => bookingSeat.SeatId)
            .OnDelete(DeleteBehavior.Restrict);


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

        // Booking total amount must not be negative
        modelBuilder.Entity<Booking>()
            .ToTable(table =>
                table.HasCheckConstraint(
                    "CK_Bookings_TotalAmount",
                    "[TotalAmount] >= 0"
                ));

        // Booking seat price must not be negative
        modelBuilder.Entity<BookingSeat>()
            .ToTable(table =>
                table.HasCheckConstraint(
                    "CK_BookingSeats_Price",
                    "[Price] >= 0"
                ));

    }


}
# CineBook – Angular Movie Ticket Booking Frontend

A responsive Angular frontend for the ASP.NET Core Movie Ticket Booking API.

## Included features

### Customer
- Register and sign in
- Browse and search movies
- View movie details and available shows
- View available seats and select seats
- Create a booking using the signed-in customer's details
- View booking history and cancel own booking

### Admin
- Admin-only dashboard
- Add and delete movies
- Add and delete theatres
- Schedule and delete shows
- View bookings
- Screen/seat endpoint methods are available in `ApiService`; the admin screen/seat panel is an extension point.

## Requirements
- Node.js (LTS recommended)
- npm
- Angular CLI compatible with Angular 21
- Your ASP.NET Core API running

## Run the frontend

```bash
npm install
npm start
```

Open `http://localhost:4200`.

## API configuration

The API base URL is configured in:

`src/app/core/api.service.ts`

Default:

```ts
private readonly baseUrl = 'https://localhost:7120/api';
```

Change it if your API uses a different URL/port.

## Backend CORS

In `Program.cs`, register this before `var app = builder.Build();`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

In the middleware pipeline, place CORS after HTTPS redirection and before authentication:

```csharp
app.UseHttpsRedirection();
app.UseCors("AngularPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

Restart the API after changing `Program.cs`.

If the browser reports a local HTTPS certificate error, run this in a terminal:

```bash
dotnet dev-certs https --trust
```

## Important backend contract notes

- The frontend uses the DTO property names supplied for this backend (`movieId`, `showId`, `seatId`, etc.).
- The movie list endpoint returns a plain array. It does not expose a total-page count in the supplied contract.
- The booking request sends `customerName`, `customerEmail`, `customerPhone`, `showId`, and `seatIds`.
- Customer booking history uses `GET /api/bookings/my-bookings`.
- Customer cancellation uses `PUT /api/bookings/{id}/cancel-my-booking`.
- Admin booking list uses `GET /api/bookings`.
- Movie posters are styled placeholders because the supplied `MovieResponse` DTO does not include a poster/image URL.
- TheatreManager screens are not included because no TheatreManager controller/authorization workflow was supplied.

## Troubleshooting

1. Start the API first and confirm `https://localhost:7120/api/health/database` works.
2. Confirm the Angular CORS policy is registered and applied.
3. Check the browser console and Network tab for 401, 403, CORS, or certificate errors.
4. A 401 usually means you need to sign in again. A 403 means the signed-in role does not have permission for that endpoint.
5. If Angular CLI reports package version conflicts, use the Angular CLI version that matches the installed Angular dependencies or regenerate the scaffold with your installed CLI and copy `src/` into it.

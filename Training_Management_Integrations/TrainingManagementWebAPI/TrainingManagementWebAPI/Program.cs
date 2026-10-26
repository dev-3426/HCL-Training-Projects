
using TrainingManagementWebAPI.Generics;
using TrainingManagementWebAPI.Models;
using TrainingManagementWebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register repositories as shared instances.0
builder.Services.AddSingleton<Repository<Course>>();
builder.Services.AddSingleton<Repository<Learner>>();
builder.Services.AddSingleton<Repository<Enrollment>>();

// Register services.
builder.Services.AddSingleton<CourseService>();
builder.Services.AddSingleton<LearnerService>();
builder.Services.AddSingleton<EnrollmentService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();

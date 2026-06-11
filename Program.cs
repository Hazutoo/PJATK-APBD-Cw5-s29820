using Microsoft.EntityFrameworkCore;
using PJATK_APBD_Hospital.Database;
using PJATK_APBD_Hospital.Middleware;
using PJATK_APBD_Hospital.Services.BedAssignments;
using PJATK_APBD_Hospital.Services.Patients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalDb")));

builder.Services.AddScoped<IPatientQueryService, PatientQueryService>();
builder.Services.AddScoped<IBedAssignmentService, BedAssignmentService>();

var app = builder.Build();

app.UseMiddleware<ApiExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

using Business.Services;
using DAL.Infrastructure;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IndividualsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IIndividualRepository, IndividualRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IRelationRepository, RelationRepository>();
builder.Services.AddScoped<IPhoneNumberRepository, PhoneNumberRepository>();

builder.Services.AddScoped<IIndividualService, IndividualService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IRelationService, RelationService>();
builder.Services.AddScoped<IPhoneNumberService, PhoneNumberService>();
builder.Services.AddScoped<IOsintProvider, MockOsintProvider>();
builder.Services.AddScoped<IOsintScraperService, OsintScraperService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

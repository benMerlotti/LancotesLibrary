using LancotesLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore.Query.Internal;
using LancotesLibrary.Models.DTOs;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<LancotesLibraryDbContext>(builder.Configuration["LancotesLibraryDbConnectionString"]);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/materials", (LancotesLibraryDbContext db, int? Id, int? MaterialTypeId, int? GenreId) =>
{

    var query = db.Materials
    .Include(m => m.Genre)
    .Include(m => m.MaterialType)
    .Where(m => m.OutOfCirculationSince == null)
    .AsQueryable();

    if (Id.HasValue)
    {
        query = query.Where(m => m.Id == Id.Value);
    }

    if (MaterialTypeId.HasValue)
    {
        query = query.Where(m => m.MaterialTypeId == MaterialTypeId.Value);
    }

    if (GenreId.HasValue)
    {
        query = query.Where(m => m.GenreId == GenreId.Value);
    }

    var result = query
    .Select(m =>
        new MaterialDTO
        {
            Id = m.Id,
            MaterialName = m.MaterialName,
            MaterialType = new MaterialTypeDTO
            {
                Id = m.MaterialType.Id,
                Name = m.MaterialType.Name,
                CheckoutDays = m.MaterialType.CheckoutDays
            },
            MaterialTypeId = m.MaterialTypeId,
            Genre = new GenreDTO
            {
                Id = m.Genre.Id,
                Name = m.Genre.Name
            },
            GenreId = m.GenreId,
            OutOfCirculationSince = m.OutOfCirculationSince
        }).ToList();

    return result;

});

app.MapGet("/materialTypes", (LancotesLibraryDbContext db) =>
{
    return db.MaterialTypes
    .Select(mt => new MaterialTypeDTO
    {
        Id = mt.Id,
        Name = mt.Name,
        CheckoutDays = mt.CheckoutDays

    }).ToList();
});

app.MapGet("/genres", (LancotesLibraryDbContext db) =>
{
    return db.Genres
    .Select(g => new GenreDTO
    {
        Id = g.Id,
        Name = g.Name

    }).ToList();
});

app.MapGet("/patrons", (LancotesLibraryDbContext db) =>
{
    return db.Patrons
    .Select(p => new PatronDTO
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Address = p.Address,
        Email = p.Email

    }).ToList();
});

app.MapGet("/patrons/{id}", (LancotesLibraryDbContext db, int id) =>
{
    return db.Patrons

    .Select(p => new PatronDTO
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Address = p.Address,
        Email = p.Email,
        Checkouts = p.Checkouts
        .Select(c => new CheckoutDTO
        {
            Id = c.Id,
            MaterialId = c.MaterialId,
            CheckoutDate = c.CheckoutDate,
            ReturnDate = c.ReturnDate,
            Material = new MaterialDTO
            {
                Id = c.Material.Id,
                MaterialName = c.Material.MaterialName,
                MaterialType = new MaterialTypeDTO
                {
                    Id = c.Material.MaterialType.Id,
                    Name = c.Material.MaterialType.Name,
                }
            }
        }).ToList()

    }).Single(p => p.Id == id);
});

app.MapPost("/materials", (LancotesLibraryDbContext db, Material Material) =>
{
    db.Materials.Add(Material);
    db.SaveChanges();
    return Results.Created($"/materials/{Material.Id}", Material);
});

app.MapPut("/patrons", (LancotesLibraryDbContext db, int? id, string email, string address) =>
{
    Patron selectedPatron = db.Patrons.FirstOrDefault(p => p.Id == id);

    selectedPatron.Email = email;
    selectedPatron.Address = address;

    db.SaveChanges();
    return Results.Ok(selectedPatron);

});

app.MapPut("/patrons/deactivate", (LancotesLibraryDbContext db, int? id, bool deactivate) =>
{
    Patron selectedPatron = db.Patrons.FirstOrDefault(p => p.Id == id);

    selectedPatron.IsActive = false;

    db.SaveChanges();
    return Results.Ok(selectedPatron);

});

app.MapPost("/checkout", (LancotesLibraryDbContext db, Checkout checkout, int patronId, int materialId) =>
{
    checkout.PatronId = patronId;
    checkout.MaterialId = materialId;
    checkout.CheckoutDate = DateTime.Today;
    db.Checkouts.Add(checkout);
    db.SaveChanges();
    return Results.Ok(checkout);
});

app.MapPut("/return", (LancotesLibraryDbContext db, int id) =>
{
    Checkout selectedCheckout = db.Checkouts.FirstOrDefault(c => c.Id == id);

    selectedCheckout.ReturnDate = DateTime.Now;
    db.SaveChanges();
    return Results.Ok(selectedCheckout);
});

app.MapPut("/materials", (LancotesLibraryDbContext db, int? id) =>
{
    Material selectedMaterial = db.Materials.FirstOrDefault(m => m.Id == id);

    selectedMaterial.OutOfCirculationSince = DateTime.Now;

    db.SaveChanges();
    return Results.NoContent();
});

app.Run();

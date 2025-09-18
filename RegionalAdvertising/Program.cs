using Scalar.AspNetCore;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<FileService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Advisitings");
    });
}

app.UseHttpsRedirection();

app.MapGet("/mufaso", () => ("Zdarowa Mufaso"));
app.MapPost("/readLocations", async (string path, FileService fileService) =>
{
   var result = await fileService.ReadFile(path);
   if (result is null) 
       return Results.NotFound();
       
    return Results.Ok(result);
});
app.MapGet("/ads", (string location,  FileService fileService) =>
{
    var result = fileService.GetAdvertisingsByLocation(location);
    return Results.Ok(result);
});

app.Run();



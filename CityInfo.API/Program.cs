using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    // Return status code if API consumer requests information in a format that is not currently supported
    options.ReturnHttpNotAcceptable = true;
}).AddXmlDataContractSerializerFormatters(); // this adds XML input and output formatters

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

// Once services are added and/or configured, application can now be built:
var app = builder.Build();

// Configure the HTTP request pipeline (i.e. build the application's request pipeline)
// Middleware is added to the request pipeline (in this case, Swagger)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();

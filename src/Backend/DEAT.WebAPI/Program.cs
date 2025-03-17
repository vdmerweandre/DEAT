using DEAT.WebAPI.Services.Extensions;
using DEAT.WebApi.TemporalServices.Extensions;
using DEAT.WebAPI.TigerBeetleServices;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDEATWebAPIServices(builder.Configuration);
builder.Services.AddTemporalServices(builder.Configuration);

// Register TigerBeetle services with local instance configuration
var clusterAddress = builder.Configuration["TigerBeetle:ClusterAddress"] ?? "127.0.0.1:3000";
//var clusterID = uint.Parse(builder.Configuration["TigerBeetle:ClusterID"] ?? "0");

// Format the address correctly for TigerBeetle
//var formattedAddress = clusterAddress.StartsWith("tcp://") ? clusterAddress : $"tcp://{clusterAddress}";
var clusterID = UInt128.Zero;
var addresses = new[] { clusterAddress };

builder.Services.AddTigerBeetleServices(
    clusterAddresses: addresses,
    clusterID: clusterID);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

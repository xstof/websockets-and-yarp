using Yarp.ReverseProxy.Transforms;
using Azure.Messaging.WebPubSub;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using yarp_proxy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAzureClients(azureBuilder =>
{
    var connString = builder.Configuration["WebPubSub:ConnectionString"];
    var hubName = builder.Configuration["WebPubSub:HubName"];
    azureBuilder.AddWebPubSubServiceClient(connString, hubName);
});
builder.Services.AddRazorPages();
builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
                .AddTransformFactory<WebPubSubAccessTokenTransformFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//app.MapRazorPages();
app.MapReverseProxy();

app.Run();

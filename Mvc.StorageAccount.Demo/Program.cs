using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Azure;
using Mvc.StorageAccount.Demo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var serviceConnectionString = builder.Configuration["StorageConnectionString"];

builder.Services.AddAzureClients(b =>
{
    b.AddClient<TableClient, TableClientOptions>((_) =>
    {
        return new TableClient(builder.Configuration["AzureStorage:ConnectionString"], builder.Configuration["AzureStorage:TableName"]);
    });
    b.AddClient<BlobContainerClient, BlobClientOptions>((_) =>
    {
        return new BlobContainerClient(builder.Configuration["AzureStorage:ConnectionString"], builder.Configuration["AzureStorage:BlobContainerName"]);
    });
    b.AddClient<QueueClient, QueueClientOptions>((_) =>
    {
        return new QueueClient(builder.Configuration["AzureStorage:ConnectionString"], builder.Configuration["AzureStorage:QueueName"],
            new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });
    });
});

builder.Services.AddScoped<ITableStorageService, TableStorageService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IQueueService, QueueService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

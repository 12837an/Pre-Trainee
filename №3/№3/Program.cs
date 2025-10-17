using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.data;
using TaskManager.repositories;
using TaskManager.services;
using TaskManager.ui;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(config);
services.AddSingleton<IDatabaseConnectionFactory, SqlConnectionFactory>();
services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
services.AddScoped<ITaskRepository, TaskRepository>();
services.AddScoped<ITaskService, TaskService>();
services.AddScoped<ConsoleInterface>();

var provider = services.BuildServiceProvider();
var databaseInitializer = provider.GetRequiredService<IDatabaseInitializer>();
await databaseInitializer.InitializeAsync();

var consoleInterface = provider.GetRequiredService<ConsoleInterface>();
await consoleInterface.RunAsync();
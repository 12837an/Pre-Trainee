using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.data;
using TaskManager.repositories;
using TaskManager.services;
using TaskManager.models;
using Dapper;
using System.Data;
using TaskItem = TaskManager.models.TaskItem;
using Microsoft.Data.SqlClient;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(config);
services.AddSingleton<IDatabaseConnectionFactory, SqlConnectionFactory>();
services.AddScoped<ITaskRepository, TaskRepository>();
services.AddScoped<TaskService>();

var provider = services.BuildServiceProvider();
var taskService = provider.GetRequiredService<TaskService>();

await CreateTable();
while (true)
{
    Console.Clear();
    Console.WriteLine("Менеджер задач");
    Console.WriteLine("1. Все задачи");
    Console.WriteLine("2. Добавить задачу");
    Console.WriteLine("3. Изменить статус");
    Console.WriteLine("4. Удалить задачу");
    Console.WriteLine("5. Выход");
    Console.Write("Выберите цифру: ");

    var input = Console.ReadLine();
    switch (input)
    {
        case "1": await ShowTasks(); break;
        case "2": await AddTask(); break;
        case "3": await ChangeStatus(); break;
        case "4": await RemoveTask(); break;
        case "5": return;
        default:
        Console.WriteLine("Неверный выбор");
        Console.ReadKey();
        break;
    }
}

async Task CreateTable()
{
    var factory = new SqlConnectionFactory(config);
    var masterConnectionString = config.GetConnectionString("DefaultConnection")
    .Replace("Database=TaskManager", "Database=master");
    using var masterConnection = new SqlConnection(masterConnectionString);
    var createDbSql = @"
        IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'TaskManager')
        BEGIN
            CREATE DATABASE TaskManager;
        END";

    await masterConnection.ExecuteAsync(createDbSql);
    using var connection = factory.CreateConnection();

    var createTableSql = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tasks' AND xtype='U')
        CREATE TABLE Tasks (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Title NVARCHAR(255) NOT NULL,
            Description NVARCHAR(MAX),
            IsCompleted BIT NOT NULL DEFAULT 0,
            CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
        )";
    await connection.ExecuteAsync(createTableSql);
}

async Task ShowTasks()
{
    Console.Clear();
    Console.WriteLine("Списолк задач\n");

    var tasks = await taskService.GetAllTasksAsync();

    if (!tasks.Any())
    {
        Console.WriteLine("Задачи отсутствуют");
    }
    else
    {
        foreach (var task in tasks)
        {
            Console.WriteLine($"ID: {task.Id}");
            Console.WriteLine($"Название: {task.Title}");
            Console.WriteLine($"Описание: {task.Description}");
            Console.WriteLine($"Статус: {(task.IsCompleted ? "Выполнена" : "Не выполнена")}");
            Console.WriteLine($"Дата: {task.CreatedAt:dd.MM.yyyy HH:mm}");
        }
    }

    Console.WriteLine("\nНажмите любую клавишу");
    Console.ReadKey();
}

async Task AddTask()
{
    Console.Clear();
    Console.WriteLine("Новая задача\n");

    Console.Write("Название: ");
    var title = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(title))
    {
        Console.WriteLine("Необходимо ввести задачу");
        Console.ReadKey();
        return;
    }

    Console.Write("Описание: ");
    var description = Console.ReadLine();

    try
    {
        var id = await taskService.AddTaskAsync(title, description ?? "");
        Console.WriteLine($"\nЗадача добавлена. ID: {id}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nОшибка: {ex.Message}");
    }

    Console.WriteLine("Нажмите любую клавишу");
    Console.ReadKey();
}

async Task ChangeStatus()
{
    Console.Clear();
    Console.WriteLine("Изменить статус\n");

    int id = GetValidatedId();

    bool completed = GetValidatedStatus();

    try
    {
        var success = await taskService.UpdateTaskStatusAsync(id, completed);
        if (success)
        {
            Console.WriteLine($"\nСтатус задачи ID: {id} обновлен на '{(completed ? "Выполнена" : "Не выполнена")}'");
        }
        else
        {
            Console.WriteLine($"\nЗадача с ID {id} не найдена");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nОшибка: {ex.Message}");
    }

    Console.WriteLine("\nНажмите любую клавишу");
    Console.ReadKey();
}
int GetValidatedId()
{
    int id;
    while (true)
    {
        Console.Write("ID задачи: ");
        var input = Console.ReadLine();

        if (int.TryParse(input, out id) && id > 0)
            break;

        Console.WriteLine("Ошибка: ID должен быть положительным числом\n");
    }
    return id;
}

bool GetValidatedStatus()
{
    while (true)
    {
        Console.Write("Задача выполнена? (y/n/д/н): ");
        var input = Console.ReadLine()?.ToLower().Trim();

        if (input == "y" || input == "yes" || input == "д" || input == "да")
            return true;
        else if (input == "n" || input == "no" || input == "н" || input == "нет")
            return false;
        else
            Console.WriteLine("Ошибка. Необходимо ввести  y/n (д/н)\n");
    }
} 

async Task RemoveTask()
{
    Console.Clear();
    Console.WriteLine("Удалить задачу\n");

    Console.Write("ID задачи: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Неверный ID");
        Console.ReadKey();
        return;
    }

    try
    {
        var success = await taskService.DeleteTaskAsync(id);
        if (success)
        {
            Console.WriteLine("\nЗадача удалена");
        }
        else
        {
            Console.WriteLine($"\nЗадача с ID {id} не найдена");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nОшибка: {ex.Message}");
    }

    Console.WriteLine("Нажмите любую клавишу");
    Console.ReadKey();
}
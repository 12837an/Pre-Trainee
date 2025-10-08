using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class Worker
{
    readonly string _name;
    readonly int _delay;

    public Worker(string name, int delay = 3000)
    {
        _name = name;
        _delay = delay;
    }

    public string DoWork(string workItem)
    {
        Console.WriteLine($"[{_name}] Начало синхронной работы: {workItem}");

        Thread.Sleep(_delay);

        string output = $"Работа '{workItem}' сделана за {_delay / 1000} сек";
        Console.WriteLine($"[{_name}] Конец синхронной работы: {workItem}");

        return output;
    }

    public async Task<string> DoWorkAsync(string workItem)
    {
        Console.WriteLine($"[{_name}] Начало асинхронной работы: {workItem}");

        await Task.Delay(_delay);

        string output = $"Работа '{workItem}' сделана за {_delay / 1000} сек";
        Console.WriteLine($"[{_name}] Конец асинхронной работы: {workItem}");

        return output;
    }
}

class Program
{
    static List<string> _jobs = new List<string> { "Первая задача", "Вторая задача", "Третья задача" };

    static void SyncTest()
    {
        Console.WriteLine("Синхронный вариант");
        Console.WriteLine($"Старт: {DateTime.Now:HH:mm:ss}");

        var worker = new Worker("Синхронный воркер");
        var results = new List<string>();
        var timer = DateTime.Now;

        foreach (var job in _jobs)
        {
            var result = worker.DoWork(job);
            results.Add(result);
        }

        var total = DateTime.Now - timer;

        Console.WriteLine("\nИтоги синхронной работы:");
        foreach (var item in results)
        {
            Console.WriteLine($" * {item}");
        }
        Console.WriteLine($"Общее время: {total.TotalSeconds:F1} сек\n");
    }

    static async Task AsyncTest()
    {
        Console.WriteLine("Асинхронный вариант");
        Console.WriteLine($"Старт: {DateTime.Now:HH:mm:ss}");

        var worker = new Worker("Асинхронный воркер");
        var tasks = new List<Task<string>>();
        var timer = DateTime.Now;

        foreach (var job in _jobs)
        {
            var task = worker.DoWorkAsync(job);
            tasks.Add(task);
        }

        Console.WriteLine("Все задачи запущены");

        var outputs = await Task.WhenAll(tasks);

        var total = DateTime.Now - timer;

        Console.WriteLine("\nИтоги асинхронной работы:");
        foreach (var item in outputs)
        {
            Console.WriteLine($" * {item}");
        }
        Console.WriteLine($"Общее время: {total.TotalSeconds:F1} сек\n");
    }

    static async Task Main(string[] args)
    {
        SyncTest();
        await AsyncTest();

        Console.WriteLine("\nНажмите Enter для выхода");
        Console.ReadLine();
    }
}
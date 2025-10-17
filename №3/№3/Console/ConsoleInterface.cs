using TaskManager.services;

namespace TaskManager.Cons

{
    public class ConsoleInterface
    {
        private readonly ITaskService _taskService;

        public ConsoleInterface(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Менеджер задач");
                Console.WriteLine($"{ConsoleConstants.MENU_SHOW_TASKS}. Все задачи");
                Console.WriteLine($"{ConsoleConstants.MENU_ADD_TASK}. Добавить задачу");
                Console.WriteLine($"{ConsoleConstants.MENU_CHANGE_STATUS}. Изменить статус");
                Console.WriteLine($"{ConsoleConstants.MENU_REMOVE_TASK}. Удалить задачу");
                Console.WriteLine($"{ConsoleConstants.MENU_EXIT}. Выход");
                Console.Write("Выберите цифру: ");

                var input = Console.ReadLine();
                switch (input)
                {
                    case ConsoleConstants.MENU_SHOW_TASKS: 
                        await ShowTasks(); 
                        break;
                    case ConsoleConstants.MENU_ADD_TASK:
                        await AddTask(); 
                        break;
                    case ConsoleConstants.MENU_CHANGE_STATUS: 
                        await ChangeStatus(); 
                        break;
                    case ConsoleConstants.MENU_REMOVE_TASK: 
                        await RemoveTask(); 
                        break;
                    case ConsoleConstants.MENU_EXIT: 
                        return;
                    default:
                        Console.WriteLine("Неверный выбор");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private async Task ShowTasks()
        {
            Console.Clear();
            Console.WriteLine("Список задач\n");

            var tasks = await _taskService.GetAllTasksAsync();

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

        private async Task AddTask()
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
                var id = await _taskService.AddTaskAsync(title, description ?? "");
                Console.WriteLine($"\nЗадача добавлена. ID: {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка: {ex.Message}");
            }

            Console.WriteLine("Нажмите любую клавишу");
            Console.ReadKey();
        }

        private async Task ChangeStatus()
        {
            Console.Clear();
            Console.WriteLine("Изменить статус\n");

            int id = GetValidatedId();
            bool completed = GetValidatedStatus();

            try
            {
                var success = await _taskService.UpdateTaskStatusAsync(id, completed);
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

        private async Task RemoveTask()
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
                var success = await _taskService.DeleteTaskAsync(id);
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

        private int GetValidatedId()
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

        private bool GetValidatedStatus()
        {
            while (true)
            {
                Console.Write(ConsoleConstants.Confirmation.PromptMessage);
                var input = Console.ReadLine()?.ToLower().Trim();

                if (ConsoleConstants.Confirmation.AllYesValues.Contains(input))
                    return true;
                else if (ConsoleConstants.Confirmation.AllNoValues.Contains(input))
                    return false;
                else
                    Console.WriteLine(ConsoleConstants.Confirmation.ErrorMessage);
            }
        }
    }
}
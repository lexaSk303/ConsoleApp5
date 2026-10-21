using ConsoleApp5;
using ConsoleApp5.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

string connString =
    builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "Не найдена строка подключения 'Default'.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connString));

using var host = builder.Build();
using var scope = host.Services.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//// П1
//try
//{
//    bool canConnect = await db.Database.CanConnectAsync();

//    if (!canConnect)
//    {
//        Console.WriteLine("Не удалось подключиться к PostgreSQL.");
//        return;
//    }

//    Console.WriteLine("Подключение к PostgreSQL успешно!");
//    Console.WriteLine($"База данных: {db.Database.GetDbConnection().Database}\n");

//    Console.Write("Минимальный возраст: ");
//    int minAge = int.Parse(Console.ReadLine()!);

//    Console.Write("Максимальный возраст: ");
//    int maxAge = int.Parse(Console.ReadLine()!);

//    Console.Write("Номер страницы: ");
//    int page = int.Parse(Console.ReadLine()!);

//    const int pageSize = 3;
//    if (page < 1) page = 1;

//    var query = db.Students
//        .Where(s => s.Age >= minAge && s.Age <= maxAge)
//        .OrderBy(s => s.LastName)
//        .ThenBy(s => s.FirstName)
//        .Select(s => new { s.FirstName, s.LastName, s.Age });

//    var pagedStudents = await query
//        .Skip((page - 1) * pageSize)
//        .Take(pageSize)
//        .ToListAsync();

//    Console.WriteLine("\n=== Результат поиска ===");
//    if (pagedStudents.Count == 0)
//    {
//        Console.WriteLine("Студенты не найдены для выбранных параметров.");
//    }
//    else
//    {
//        foreach (var student in pagedStudents)
//        {
//            Console.WriteLine($"{student.FirstName} {student.LastName}, {student.Age}");
//        }
//    }

//    bool hasMinors = await db.Students.AnyAsync(s => s.Age < 18);
//    Console.WriteLine($"\nЕсть несовершеннолетние студенты: {hasMinors}");
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"\nОшибка работы с базой данных: {ex.Message}");
//}

//// П2
//try
//{
//    bool canConnect = await db.Database.CanConnectAsync();

//    if (!canConnect)
//    {
//        Console.WriteLine("Не удалось подключиться к PostgreSQL.");
//        return;
//    }

//    Console.WriteLine("Подключение к PostgreSQL успешно!\n");
//    Console.WriteLine("=== Аналитика учебного центра ===\n");
//    int totalCount = await db.Students.CountAsync();

//    if (totalCount == 0)
//    {
//        Console.WriteLine("База данных пуста. Нечего анализировать.");
//        return;
//    }
//    double averageAge = await db.Students.AverageAsync(s => (double?)s.Age) ?? 0.0;
//    int minAge = await db.Students.MinAsync(s => (int?)s.Age) ?? 0;
//    int maxAge = await db.Students.MaxAsync(s => (int?)s.Age) ?? 0;

//    Console.WriteLine($"Всего студентов: {totalCount}");
//    Console.WriteLine($"Средний возраст: {averageAge:F1}");
//    Console.WriteLine($"Минимальный возраст: {minAge}");
//    Console.WriteLine($"Максимальный возраст: {maxAge}\n");

//    var ageDistribution = await db.Students
//        .GroupBy(s => s.Age)
//        .Select(g => new
//        {
//            Age = g.Key,
//            Count = g.Count()
//        })
//        .OrderBy(x => x.Age)
//        .ToListAsync();

//    Console.WriteLine("Распределение по возрасту:");
//    foreach (var item in ageDistribution)
//    {
//        Console.WriteLine($"{item.Age} {GetAgeWord(item.Age)} — {item.Count}");
//    }
//    Console.WriteLine();

//    bool allAdults = await db.Students.AllAsync(s => s.Age >= 18);
//    Console.WriteLine($"Все студенты совершеннолетние: {allAdults}");
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"\nОшибка работы с базой данных: {ex.Message}");
//}

//string GetAgeWord(int age)
//{
//    int mod10 = age % 10;
//    int mod100 = age % 100;

//    if (mod10 == 1 && mod100 != 11) return "год";
//    if (mod10 >= 2 && mod10 <= 4 && (mod100 < 10 || mod100 >= 20)) return "года";
//    return "лет";
//}

//// П1 вторая пара
//try
//{
//    bool canConnect = await db.Database.CanConnectAsync();

//    if (!canConnect)
//    {
//        Console.WriteLine("Не удалось подключиться к PostgreSQL.");
//        return;
//    }

//    Console.WriteLine("Подключение к PostgreSQL успешно!\n");
//    Console.WriteLine("=== Список учебных групп и студентов ===\n");
//    var groups = await db.Groups
//        .Include(g => g.Students)
//        .OrderBy(g => g.Name)
//        .ToListAsync();

//    if (groups.Count == 0)
//    {
//        Console.WriteLine("В базе данных пока нет ни одной учебной группы.");
//        return;
//    }
//    foreach (var group in groups)
//    {
//        Console.WriteLine($"Группа: {group.Name} (Студентов: {group.Students.Count})");
//        if (group.Students.Count == 0)
//        {
//            Console.WriteLine("В группе пока нет студентов.");
//        }
//        else
//        {
//            var sortedStudents = group.Students
//                .OrderBy(s => s.LastName)
//                .ThenBy(s => s.FirstName)
//                .ToList();

//            foreach (var student in sortedStudents)
//            {
//                Console.WriteLine($"   - {student.LastName} {student.FirstName}, возраст: {student.Age}");
//            }
//        }
//        Console.WriteLine();
//    }
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"\nОшибка работы с базой данных: {ex.Message}");
//}

////П2 вторая пара
//try
//{
//    bool canConnect = await db.Database.CanConnectAsync();

//    if (!canConnect)
//    {
//        Console.WriteLine("Не удалось подключиться к PostgreSQL.");
//        return;
//    }
//    Console.WriteLine("Подключение к PostgreSQL успешно!\n");
//    Console.WriteLine("=== Сводная статистика по группам ===\n");
//    var statistics = await db.Groups
//        .OrderBy(g => g.Name) 
//        .Select(g => new
//        {
//            GroupName = g.Name,
//            StudentCount = g.Students.Count,
//            MinorCount = g.Students.Count(s => s.Age < 18),
//            AverageAge = g.Students.Select(s => (double?)s.Age).Average(),
//            HasMinors = g.Students.Any(s => s.Age < 18)
//        })
//        .ToListAsync(); 

//    if (statistics.Count == 0)
//    {
//        Console.WriteLine("В базе данных нет ни одной группы.");
//    }
//    else
//    {
//        foreach (var item in statistics)
//        {
//            Console.WriteLine($"Группа: {item.GroupName}");
//            Console.WriteLine($"Всего студентов: {item.StudentCount}");
//            Console.WriteLine($"Несовершеннолетних: {item.MinorCount}");
//            if (item.AverageAge.HasValue)
//            {
//                Console.WriteLine($"Средний возраст: {item.AverageAge.Value:F1}");
//            }
//            else
//            {
//                Console.WriteLine("Средний возраст: нет данных");
//            }
//            Console.WriteLine(item.HasMinors
//                ? "В группе есть несовершеннолетние"
//                : "Несовершеннолетних нет");

//            Console.WriteLine();
//        }
//    }
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"\nОшибка работы с базой данных: {ex.Message}");
//}



//П1 третья пара
if (!await db.Database.CanConnectAsync())
{
    Console.WriteLine("Не удалось подключиться к PostgreSQL. Проверьте настройки.");
    return;
}

Console.WriteLine("Подключение к PostgreSQL успешно!\n");
while (true)
{
    Console.Clear();
    Console.WriteLine("=== Меню управления студентами ===");
    Console.WriteLine("1. Показать всех студентов");
    Console.WriteLine("2. Добавить студента");
    Console.WriteLine("3. Изменить возраст студента");
    Console.WriteLine("4. Удалить студента");
    Console.WriteLine("5. Перевести студента в другую группу");
    Console.WriteLine("0. Выход");
    Console.Write("\nВыберите действие: ");

    string? choice = Console.ReadLine()?.Trim();

    switch (choice)
    {
        case "1":
            await ShowAllStudentsAsync(db);
            break;
        case "2":
            await AddStudentAsync(db);
            break;
        case "3":
            await UpdateStudentAgeAsync(db);
            break;
        case "4":
            await DeleteStudentAsync(db);
            break;
        case "5": 
            await TransferStudentToGroupAsync(db);
            break;
        case "0":
            Console.WriteLine("Выход из программы. До свидания!");
            return;
        default:
            Console.WriteLine("Неверный ввод. Попробуйте снова.");
            break;
    }

    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
    Console.ReadKey();
}

static async Task ShowAllStudentsAsync(AppDbContext db)
{
    Console.WriteLine("\n=== Список всех студентов ===");
    var students = await db.Students
        .Include(s => s.Group)
        .OrderBy(s => s.LastName)
        .ThenBy(s => s.FirstName)
        .ToListAsync();

    if (students.Count == 0)
    {
        Console.WriteLine("Список студентов пуст.");
        return;
    }

    foreach (var s in students)
    {
        string groupName = s.Group != null ? s.Group.Name : "Группа не назначена";
        Console.WriteLine($"[{s.Id}] {s.LastName} {s.FirstName}, возраст: {s.Age}, группа: {groupName}");
    }
}

static async Task AddStudentAsync(AppDbContext db)
{
    Console.WriteLine("\n=== Добавление нового студента ===");

    Console.Write("Имя: ");
    string firstName = Console.ReadLine()?.Trim() ?? "";

    Console.Write("Фамилия: ");
    string lastName = Console.ReadLine()?.Trim() ?? "";

    Console.Write("Возраст: ");
    string ageInput = Console.ReadLine()?.Trim() ?? "";

    Console.Write("ID группы: ");
    string groupIdInput = Console.ReadLine()?.Trim() ?? "";
    if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
    {
        Console.WriteLine("Ошибка: пустые строчки ИФ");
        return;
    }
    if (!int.TryParse(ageInput, out int age) || age <= 0)
    {
        Console.WriteLine("Ошибка: возраст больше нуля");
        return;
    }
    if (!int.TryParse(groupIdInput, out int groupId) || !await db.Groups.AnyAsync(g => g.Id == groupId))
    {
        Console.WriteLine($"Ошибка: Группа с ID {groupId} не существует.");
        return;
    }
    var newStudent = new Student
    {
        FirstName = firstName,
        LastName = lastName,
        Age = age,
        GroupId = groupId
    };

    await db.Students.AddAsync(newStudent);
    await db.SaveChangesAsync();

    Console.WriteLine($"Студент {firstName} {lastName} успешно добавлен с ID: {newStudent.Id}");
}

static async Task UpdateStudentAgeAsync(AppDbContext db)
{
    Console.WriteLine("\n=== Изменение возраста студента ===");
    Console.Write("Введите ID студента: ");

    if (!int.TryParse(Console.ReadLine()?.Trim(), out int id))
    {
        Console.WriteLine("Ошибка: Неверный формат ID");
        return;
    }
    var student = await db.Students.FindAsync(id);

    if (student == null)
    {
        Console.WriteLine($"Ошибка: Студент с ID {id} не найден.");
        return;
    }

    Console.Write($"Текущий возраст: {student.Age}. Введите новый возраст: ");
    if (!int.TryParse(Console.ReadLine()?.Trim(), out int newAge) || newAge <= 0)
    {
        Console.WriteLine("Ошибка: возраст больше нуля");
        return;
    }
    student.Age = newAge;
    await db.SaveChangesAsync();
    Console.WriteLine($"Возраст студента {student.FirstName} {student.LastName} успешно изменен на {newAge}.");
}

static async Task DeleteStudentAsync(AppDbContext db)
{
    Console.WriteLine("\n=== Удаление студента ===");
    Console.Write("Введите ID студента для удаления: ");

    if (!int.TryParse(Console.ReadLine()?.Trim(), out int id))
    {
        Console.WriteLine("Ошибка: Неверный формат ID.");
        return;
    }

    var student = await db.Students.FindAsync(id);

    if (student == null)
    {
        Console.WriteLine($"Ошибка: Студент с ID {id} не найден.");
        return;
    }

    db.Students.Remove(student);
    await db.SaveChangesAsync();

    Console.WriteLine($"Студент {student.FirstName} {student.LastName} (ID: {id}) успешно удален.");
}


//П2 третья пара
static async Task TransferStudentToGroupAsync(AppDbContext db)
{
    Console.WriteLine("\n=== Перевод студента в другую группу ===");

    Console.Write("Введите ID студента: ");
    if (!int.TryParse(Console.ReadLine()?.Trim(), out int studentId))
    {
        Console.WriteLine("Ошибка: Неверный формат ID.");
        return;
    }

    var student = await db.Students.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == studentId);

    if (student == null)
    {
        Console.WriteLine($"Ошибка: Студент с ID {studentId} не найден.");
        return;
    }

    string currentGroupName = student.Group != null ? student.Group.Name : "Группа не назначена";
    Console.WriteLine($"Текущая группа студента: {currentGroupName}");
    var groups = await db.Groups.OrderBy(g => g.Name).ToListAsync();
    if (groups.Count == 0)
    {
        Console.WriteLine("Ошибка: В базе данных нет групп. Сначала добавьте группы.");
        return;
    }
    Console.WriteLine("\nДоступные группы:");
    foreach (var g in groups)
    {
        Console.WriteLine($"  [{g.Id}] {g.Name}");
    }
    Console.Write("\nВведите ID новой группы: ");
    if (!int.TryParse(Console.ReadLine()?.Trim(), out int newGroupId))
    {
        Console.WriteLine("Ошибка: Неверный формат ID группы.");
        return;
    }

    var targetGroup = groups.FirstOrDefault(g => g.Id == newGroupId);
    if (targetGroup == null)
    {
        Console.WriteLine($"Ошибка: Группа с ID {newGroupId} не найдена.");
        return;
    }
    if (student.GroupId == newGroupId)
    {
        Console.WriteLine("Студент уже есть в группе. Перевод не требуется.");
        return;
    }

    student.GroupId = newGroupId;
    await db.SaveChangesAsync();
    Console.WriteLine($"\nУспех! Студент {student.FirstName} {student.LastName} переведен в группу '{targetGroup.Name}'.");
}


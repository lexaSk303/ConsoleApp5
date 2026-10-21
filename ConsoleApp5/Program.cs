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

// П1
try
{
    bool canConnect = await db.Database.CanConnectAsync();

    if (!canConnect)
    {
        Console.WriteLine("Не удалось подключиться к PostgreSQL.");
        return;
    }

    Console.WriteLine("Подключение к PostgreSQL успешно!");
    Console.WriteLine($"База данных: {db.Database.GetDbConnection().Database}\n");

    Console.Write("Минимальный возраст: ");
    int minAge = int.Parse(Console.ReadLine()!);

    Console.Write("Максимальный возраст: ");
    int maxAge = int.Parse(Console.ReadLine()!);

    Console.Write("Номер страницы: ");
    int page = int.Parse(Console.ReadLine()!);

    const int pageSize = 3;
    if (page < 1) page = 1;

    var query = db.Students
        .Where(s => s.Age >= minAge && s.Age <= maxAge)
        .OrderBy(s => s.LastName)
        .ThenBy(s => s.FirstName)
        .Select(s => new { s.FirstName, s.LastName, s.Age });

    var pagedStudents = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    Console.WriteLine("\n=== Результат поиска ===");
    if (pagedStudents.Count == 0)
    {
        Console.WriteLine("Студенты не найдены для выбранных параметров.");
    }
    else
    {
        foreach (var student in pagedStudents)
        {
            Console.WriteLine($"{student.FirstName} {student.LastName}, {student.Age}");
        }
    }

    bool hasMinors = await db.Students.AnyAsync(s => s.Age < 18);
    Console.WriteLine($"\nЕсть несовершеннолетние студенты: {hasMinors}");
}
catch (Exception ex)
{
    Console.WriteLine($"\nОшибка работы с базой данных: {ex.Message}");
}

// П2
try
{
    bool canConnect = await db.Database.CanConnectAsync();

    if (!canConnect)
    {
        Console.WriteLine("Не удалось подключиться к PostgreSQL.");
        return;
    }

    Console.WriteLine("Подключение к PostgreSQL успешно!\n");
    Console.WriteLine("=== Аналитика учебного центра ===\n");
    int totalCount = await db.Students.CountAsync();

    if (totalCount == 0)
    {
        Console.WriteLine("База данных пуста. Нечего анализировать.");
        return;
    }
    double averageAge = await db.Students.AverageAsync(s => (double?)s.Age) ?? 0.0;
    int minAge = await db.Students.MinAsync(s => (int?)s.Age) ?? 0;
    int maxAge = await db.Students.MaxAsync(s => (int?)s.Age) ?? 0;

    Console.WriteLine($"Всего студентов: {totalCount}");
    Console.WriteLine($"Средний возраст: {averageAge:F1}");
    Console.WriteLine($"Минимальный возраст: {minAge}");
    Console.WriteLine($"Максимальный возраст: {maxAge}\n");

    var ageDistribution = await db.Students
        .GroupBy(s => s.Age)
        .Select(g => new
        {
            Age = g.Key,
            Count = g.Count()
        })
        .OrderBy(x => x.Age)
        .ToListAsync();

    Console.WriteLine("Распределение по возрасту:");
    foreach (var item in ageDistribution)
    {
        Console.WriteLine($"{item.Age} {GetAgeWord(item.Age)} — {item.Count}");
    }
    Console.WriteLine();

    bool allAdults = await db.Students.AllAsync(s => s.Age >= 18);
    Console.WriteLine($"Все студенты совершеннолетние: {allAdults}");
}
catch (Exception ex)
{
    Console.WriteLine($"\nОшибка работы с базой данных: {ex.Message}");
}

string GetAgeWord(int age)
{
    int mod10 = age % 10;
    int mod100 = age % 100;

    if (mod10 == 1 && mod100 != 11) return "год";
    if (mod10 >= 2 && mod10 <= 4 && (mod100 < 10 || mod100 >= 20)) return "года";
    return "лет";
}

// П1 вторая пара
try
{
    bool canConnect = await db.Database.CanConnectAsync();

    if (!canConnect)
    {
        Console.WriteLine("Не удалось подключиться к PostgreSQL.");
        return;
    }

    Console.WriteLine("Подключение к PostgreSQL успешно!\n");
    Console.WriteLine("=== Список учебных групп и студентов ===\n");
    var groups = await db.Groups
        .Include(g => g.Students)
        .OrderBy(g => g.Name)
        .ToListAsync();

    if (groups.Count == 0)
    {
        Console.WriteLine("В базе данных пока нет ни одной учебной группы.");
        return;
    }
    foreach (var group in groups)
    {
        Console.WriteLine($"Группа: {group.Name} (Студентов: {group.Students.Count})");
        if (group.Students.Count == 0)
        {
            Console.WriteLine("В группе пока нет студентов.");
        }
        else
        {
            var sortedStudents = group.Students
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToList();

            foreach (var student in sortedStudents)
            {
                Console.WriteLine($"   - {student.LastName} {student.FirstName}, возраст: {student.Age}");
            }
        }
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\nОшибка работы с базой данных: {ex.Message}");
}

//П2 вторая пара
try
{
    bool canConnect = await db.Database.CanConnectAsync();

    if (!canConnect)
    {
        Console.WriteLine("Не удалось подключиться к PostgreSQL.");
        return;
    }
    Console.WriteLine("Подключение к PostgreSQL успешно!\n");
    Console.WriteLine("=== Сводная статистика по группам ===\n");
    var statistics = await db.Groups
        .OrderBy(g => g.Name) 
        .Select(g => new
        {
            GroupName = g.Name,
            StudentCount = g.Students.Count,
            MinorCount = g.Students.Count(s => s.Age < 18),
            AverageAge = g.Students.Select(s => (double?)s.Age).Average(),
            HasMinors = g.Students.Any(s => s.Age < 18)
        })
        .ToListAsync(); 

    if (statistics.Count == 0)
    {
        Console.WriteLine("В базе данных нет ни одной группы.");
    }
    else
    {
        foreach (var item in statistics)
        {
            Console.WriteLine($"Группа: {item.GroupName}");
            Console.WriteLine($"Всего студентов: {item.StudentCount}");
            Console.WriteLine($"Несовершеннолетних: {item.MinorCount}");
            if (item.AverageAge.HasValue)
            {
                Console.WriteLine($"Средний возраст: {item.AverageAge.Value:F1}");
            }
            else
            {
                Console.WriteLine("Средний возраст: нет данных");
            }
            Console.WriteLine(item.HasMinors
                ? "В группе есть несовершеннолетние"
                : "Несовершеннолетних нет");

            Console.WriteLine();
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\nОшибка работы с базой данных: {ex.Message}");
}
using System;
using System.Collections.Generic;
using LocalGuide.Models;
using LocalGuide.Services;

namespace LocalGuide
{
    // SonarQube Fix: Додано ключове слово 'static', оскільки клас має лише статичні методи
    static class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Веб-додаток 'Місцевий гід' (Варіант 5) ===\n");

            // 1. Ініціалізація тестових даних
            var initialData = new List<Location>
            {
                new Location {
                    Id = 1, Name = "Парк Шевченка", Category = LocationCategory.Park,
                    AverageRating = 4.8, Description = "Центральний парк з гарними алеями.",
                    IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new Location {
                    Id = 2, Name = "Ресторан 'Затишок'", Category = LocationCategory.Restaurant,
                    AverageRating = 4.2, Description = "Смачна українська кухня.",
                    IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Location {
                    Id = 3, Name = "Стара Фортеця", Category = LocationCategory.Landmark,
                    AverageRating = 3.9, Description = "Історична пам'ятка архітектури.",
                    IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-20)
                },
                new Location {
                    Id = 4, Name = "Нічний клуб 'Зірка'", Category = LocationCategory.Entertainment,
                    AverageRating = 2.5, Description = "Розважальний заклад.",
                    IsActive = false, CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };

            // 2. Створення екземплярів сервісів
            var locationService = new LocationService(initialData);
            var authService = new UserAuth("my-very-secure-secret-key-123");

            // 3. Демонстрація роботи складного методу FilterAndSort (для ЛР 2)
            Console.WriteLine("--- Тестування фільтрації (Рефакторований метод) ---");

            // Створюємо об'єкт фільтра з потрібними параметрами
            var filter = new LocationFilter(
                Category: null,
                MinRating: 4.0,
                MaxRating: 5.0,
                SearchText: "Парк",
                SortBy: "rating",
                Ascending: false,
                OnlyActive: true,
                MaxResults: 5
            );

            // Викликаємо рефакторований метод, передаючи йому об'єкт фільтра
            var filtered = locationService.FilterAndSortRefactored(filter);

            foreach (var loc in filtered)
            {
                Console.WriteLine($"Знайдено: {loc.Name} | Рейтинг: {loc.AverageRating} | Категорія: {loc.Category}");
            }

            // 4. Демонстрація роботи сервісу аутентифікації (для ЛР 5)
            Console.WriteLine("\n--- Тестування системи безпеки ---");
            string pass = "AdminPassword123";
            string hashed = authService.HashPassword(pass);
            bool isValid = authService.VerifyPassword(pass, hashed);

            Console.WriteLine($"Пароль вірний: {isValid}");
            Console.WriteLine($"Хеш у базі: {hashed.Substring(0, 20)}...");

            // 5. Виведення метрик (підказка для ЛР 2)
            Console.WriteLine("\n[INFO] Проєкт готовий до аналізу метрик у Visual Studio.");
            Console.WriteLine("[HINT] Перейдіть у 'Analyze' -> 'Calculate Code Metrics' для отримання звіту.");

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}
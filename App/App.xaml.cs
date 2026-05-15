using System.Windows;
using System.Diagnostics;
using LearningPlatform.Core; // Подключаем наш модуль Core

namespace LearningPlatform.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            /// <summary>Тестовая проверка подключения к БД.</summary>
            var courses = Data.GetCourses();
            Debug.WriteLine($"База данных подключена. Найдено курсов: {courses.Count}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка подключения: {ex.Message}");
            MessageBox.Show($"Ошибка БД: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
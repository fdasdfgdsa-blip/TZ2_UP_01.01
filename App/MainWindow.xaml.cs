using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using LearningPlatform.Core;
using System.Linq;

namespace LearningPlatform.App;

/// <summary>
/// Главное окно приложения: навигация между каталогом, личным кабинетом и списком пользователей.
/// </summary>
public partial class MainWindow : Window
{
    // ID текущего пользователя (заглушка, как в Data.cs)
    private const int CurrentUserId = 1;

    public MainWindow()
    {
        InitializeComponent();
        LoadCatalog();
    }

    /// <summary>Загружает и отображает все доступные курсы в каталоге.</summary>
    private void LoadCatalog()
    {
        var allCourses = Data.GetCourses();
        var myEnrolledIds = Data.GetMyEnrolledCourseIds(CurrentUserId);

        CatalogPanel.Children.Clear();
        foreach (var course in allCourses)
        {
            bool isEnrolled = myEnrolledIds.Contains(course.Id);
            var card = new CourseCard(course, isEnrolled);
            CatalogPanel.Children.Add(card);
        }
    }

    /// <summary>Загружает и отображает только курсы, на которые записан текущий студент.</summary>
    private void LoadMyCourses()
    {
        var myIds = Data.GetMyEnrolledCourseIds(CurrentUserId);
        var allCourses = Data.GetCourses();

        MyCoursesPanel.Children.Clear();

        if (myIds.Count == 0)
        {
            MyCoursesPanel.Children.Add(new TextBlock { Text = "Вы еще не записаны ни на один курс. Перейдите в Каталог.", Margin = new Thickness(20), Foreground = System.Windows.Media.Brushes.Gray });
            return;
        }

        foreach (var course in allCourses)
        {
            if (myIds.Contains(course.Id))
            {
                var card = new CourseCard(course, true);
                MyCoursesPanel.Children.Add(card);
            }
        }
    }

    /// <summary>Переключает интерфейс на вкладку "Каталог курсов".</summary>
    private void TabCatalog_Click(object sender, RoutedEventArgs e)
    {
        CatalogPanel.Visibility = Visibility.Visible;
        MyCoursesPanel.Visibility = Visibility.Collapsed;
        UsersPanel.Visibility = Visibility.Collapsed;
        LoadCatalog();
    }

    /// <summary>Переключает интерфейс на вкладку "Мои курсы".</summary>
    private void TabMyCourses_Click(object sender, RoutedEventArgs e)
    {
        CatalogPanel.Visibility = Visibility.Collapsed;
        MyCoursesPanel.Visibility = Visibility.Visible;
        UsersPanel.Visibility = Visibility.Collapsed;
        LoadMyCourses();
    }

    /// <summary>Переключает интерфейс на вкладку "Пользователи" и загружает их список.</summary>
    private void TabUsers_Click(object sender, RoutedEventArgs e)
    {
        CatalogPanel.Visibility = Visibility.Collapsed;
        MyCoursesPanel.Visibility = Visibility.Collapsed;
        UsersPanel.Visibility = Visibility.Visible;

        // Загружаем список пользователей и добавляем метку "(Вы)"
        var users = Data.GetAllUsers();
        var displayList = users.Select(u => new
        {
            Username = u.Id == CurrentUserId ? $"{u.Username} (Вы)" : u.Username,
            Role = u.Role
        }).ToList();

        UsersPanel.ItemsSource = displayList;
    }
}
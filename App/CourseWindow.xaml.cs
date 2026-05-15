using System.Windows;
using System.Collections.Generic;
using System.Linq;
using LearningPlatform.Core;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;

namespace LearningPlatform.App;

/// <summary>
/// Окно детального просмотра курса: уроки, задания, пользователи и отчёт.
/// </summary>
public partial class CourseWindow : Window
{
    private Course _course;

    /// <summary>Инициализирует окно и загружает данные выбранного курса.</summary>
    public CourseWindow(Course course)
    {
        InitializeComponent();
        _course = course;
        TitleText.Text = course.Title;
        LoadData();
    }

    /// <summary>Обновляет все вкладки: прогресс, уроки, задания, пользователей и отчёт.</summary>
    private void LoadData()
    {
        // Прогресс
        double progress = Data.GetCourseProgress(_course.Id);
        if (progress > 100) progress = 100;
        CourseProgress.Value = progress;
        ProgressLabel.Text = $"Прогресс: {progress}%";

        // Уроки, Задания, Пользователи
        LessonsList.ItemsSource = Data.GetLessonsByCourseId(_course.Id);
        AssignmentsList.ItemsSource = Data.GetAssignmentsByCourseId(_course.Id);
        UsersList.ItemsSource = Data.GetAllUsers();

        // Отчёт
        GenerateReport();
    }


    /// <summary>Формирует итоговый отчёт и текст сертификата на основе текущих данных.</summary>   
    private void GenerateReport()
    {
        ReportCourseName.Text = $"Курс: {_course.Title}";
        ReportStudentName.Text = "Студент: Настя (ID: 1)";
        ReportProgress.Text = ProgressLabel.Text;

        var grades = Data.GetGradesJournal(_course.Id);

        // ✅ ИСПРАВЛЕНИЕ: Суммируем баллы, а не усредняем. Максимум за курс = 100.
        int totalScore = grades.Sum(g => g.Score);
        if (totalScore > 100) totalScore = 100;

        ReportFinalScore.Text = $"Итоговый балл: {totalScore} ({GradeService.GetGradeText(totalScore)})";

        // Формируем текст сертификата напрямую
        string certText = $"СЕРТИФИКАТ\n\nСтудент: Настя\nКурс: {_course.Title}\nИтоговый балл: {totalScore} ({GradeService.GetGradeText(totalScore)})\nДата: {DateTime.Now:dd.MM.yyyy}";
        ReportCertificateText.Text = certText;
    }

    /// <summary>Открывает окно лекции при клике на урок.</summary>
    private void OpenLesson_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int lessonId)
        {
            var lesson = Data.GetLessonsByCourseId(_course.Id).FirstOrDefault(l => l.Id == lessonId);
            if (lesson != null)
            {
                var win = new LessonWindow(lesson);
                if (win.ShowDialog() == true) LoadData();
            }
        }
    }

    /// <summary>Открывает окно выполнения задания при клике на кнопку "Выполнить".</summary>
    private void OpenAssignment_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int assignmentId)
        {
            var assignment = Data.GetAssignmentsByCourseId(_course.Id).FirstOrDefault(a => a.Id == assignmentId);
            if (assignment != null)
            {
                var win = new AssignmentWindow(assignment, _course.Id);
                if (win.ShowDialog() == true) LoadData();
            }
        }
    }

    /// <summary>Сохраняет текстовый отчёт в файл по выбору пользователя.</summary>
    private void DownloadReport_Click(object sender, RoutedEventArgs e)
    {
        SaveFileDialog dlg = new SaveFileDialog();
        dlg.FileName = $"Отчет_{_course.Title}.txt";
        dlg.DefaultExt = ".txt";
        dlg.Filter = "Text documents (.txt)|*.txt";

        if (dlg.ShowDialog() == true)
        {
            File.WriteAllText(dlg.FileName, ReportCertificateText.Text);
            MessageBox.Show("Отчет сохранен!", "Успех");
        }
    }

    /// <summary>Закрывает окно курса.</summary>
    private void Close_Click(object sender, RoutedEventArgs e) => this.Close();
}
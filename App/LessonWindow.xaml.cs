using System.Windows;
using LearningPlatform.Core;

namespace LearningPlatform.App;

/// <summary>
/// Окно для просмотра текстовой лекции с контролем прокрутки.
/// </summary>
public partial class LessonWindow : Window
{
    private Lesson _lesson;

    /// <summary>Инициализирует окно и загружает текст лекции.</summary>
    public LessonWindow(Lesson lesson)
    {
        InitializeComponent();
        _lesson = lesson;
        TitleText.Text = lesson.Topic;
        ContentText.Text = lesson.Content;
    }

    /// <summary>
    /// Отслеживает прокрутку текста и показывает кнопку завершения, когда достигнут низ.
    /// </summary>
    private void LectureScroll_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
    {
        // Проверяем, достиг ли пользователь низа текста
        if (e.VerticalOffset >= e.ExtentHeight - e.ViewportHeight - 5)
        {
            CompleteBtn.Visibility = Visibility.Visible;
        }
    }

    /// <summary>Отмечает урок как пройденный и закрывает окно.</summary>   
    private void CompleteBtn_Click(object sender, RoutedEventArgs e)
    {
        Data.MarkLessonAsCompleted(_lesson.Id);
        MessageBox.Show("Урок успешно засчитан!", "Готово");
        this.DialogResult = true;
        this.Close();
    }
}
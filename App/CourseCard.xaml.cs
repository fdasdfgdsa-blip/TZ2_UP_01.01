using System.Windows;
using System.Windows.Controls;
using LearningPlatform.Core;

namespace LearningPlatform.App;

/// <summary>
/// Пользовательский элемент управления, отображающий карточку курса в каталоге.
/// </summary>
public partial class CourseCard : UserControl
{
    /// <summary>Получает данные курса, привязанные к этой карточке.</summary>
    public Course CourseData { get; set; }
    /// <summary>Получает или задаёт статус записи студента на данный курс.</summary>
    public bool IsEnrolled { get; set; } = false;

    /// <summary>
    /// Инициализирует карточку курса и настраивает привязку данных.
    /// </summary>
    public CourseCard(Course course, bool isEnrolled)
    {
        InitializeComponent();
        CourseData = course;
        IsEnrolled = isEnrolled;
        this.DataContext = course;

        if (IsEnrolled)
        {
            ActionBtn.Content = "Перейти";
            ActionBtn.Background = System.Windows.Media.Brushes.Gray;
        }
    }

    /// <summary>
    /// Обработчик нажатия на кнопку: записывает на курс или открывает окно просмотра.
    /// </summary>
    private void ActionBtn_Click(object sender, RoutedEventArgs e)
    {
        if (!IsEnrolled)
        {
            Data.EnrollStudent(1, CourseData.Id);
            IsEnrolled = true;
            ActionBtn.Content = "Перейти";
            ActionBtn.Background = System.Windows.Media.Brushes.Gray;
            MessageBox.Show("Вы успешно записались на курс!", "Успех");
        }
        else
        {
            var win = new CourseWindow(CourseData);
            win.ShowDialog();
        }
    }
}
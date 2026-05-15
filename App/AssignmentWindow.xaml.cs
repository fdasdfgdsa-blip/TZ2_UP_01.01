using System.Windows;
using LearningPlatform.Core;

namespace LearningPlatform.App;

/// <summary>
/// Окно выполнения задания с вводом оценки и валидацией.
/// </summary>
public partial class AssignmentWindow : Window
{
    private Assignment _assignment;
    private int _courseId;

    /// <summary>Инициализирует окно, рассчитывает максимум баллов за задание.</summary>
    public AssignmentWindow(Assignment assignment, int courseId)
    {
        InitializeComponent();
        _assignment = assignment;
        _courseId = courseId;

        TitleText.Text = assignment.Title;
        DescText.Text = assignment.Description;

        /// <summary>Расчет максимума баллов за одно задание (100 / кол-во заданий в курсе).</summary>
        var allAssignments = Data.GetAssignmentsByCourseId(courseId);
        int total = allAssignments.Count > 0 ? allAssignments.Count : 1;
        int maxPerTask = (int)Math.Round(100.0 / total);

        string typeName = assignment.Type == "exam" ? "Контрольная работа" : "Практическое задание";
        TypeInfo.Text = $"{typeName}. Максимум баллов за это задание: {maxPerTask}. Для зачета необходима оценка ≥ 3.";
    }

    /// <summary>Проверяет валидность оценки, сохраняет результат и закрывает окно.</summary>
    private void Submit_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(GradeInput.Text, out int grade) || grade < 2 || grade > 5)
        {
            WarningText.Text = "Введите корректную оценку от 2 до 5.";
            WarningText.Visibility = Visibility.Visible;
            return;
        }

        /// <summary>Баллы рассчитаются автоматически на стороне Data.cs.</summary>
        Data.SaveAssignmentGrade(_assignment.Id, grade);

        if (grade >= 3)
            MessageBox.Show("Задание засчитано! Баллы начислены автоматически.");
        else
            MessageBox.Show("Оценка ниже 3. Задание не засчитано (0 баллов).");

        this.DialogResult = true;
        this.Close();
    }

    /// <summary>Закрывает окно без сохранения.</summary>
    private void Cancel_Click(object sender, RoutedEventArgs e) => this.Close();
}
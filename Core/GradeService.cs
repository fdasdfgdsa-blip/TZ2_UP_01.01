using System.Collections.Generic;
using System.Linq;

namespace LearningPlatform.Core;

/// <summary>
/// Сервис для расчёта итоговых оценок и формирования текстовых сертификатов.
/// </summary>
public static class GradeService
{
    /// <summary>
    /// Рассчитывает средний балл по списку оценок за задания.
    /// </summary>
    /// <param name="scores">Список полученных баллов.</param>
    /// <returns>Среднее арифметическое баллов, округлённое до 1 знака.</returns>
    public static double CalculateAverageScore(List<int> scores)
    {
        if (scores == null || !scores.Any()) return 0;
        return Math.Round(scores.Average(), 1);
    }

    /// <summary>
    /// Преобразует числовой балл в текстовую оценку по принятой шкале.
    /// </summary>
    /// <param name="score">Числовой балл от 0 до 100.</param>
    /// <returns>Текстовая оценка: Отлично, Хорошо, Удовлетворительно или Неудовлетворительно.</returns>
    public static string GetGradeText(double score)
    {
        if (score >= 90) return "Отлично";
        if (score >= 75) return "Хорошо";
        if (score >= 50) return "Удовлетворительно";
        return "Неудовлетворительно";
    }

    /// <summary>
    /// Формирует объект сертификата на основе данных студента и курса.
    /// </summary>
    /// <param name="studentName">Имя студента.</param>
    /// <param name="courseTitle">Название курса.</param>
    /// <param name="scores">Список баллов за выполненные задания.</param>
    /// <returns>Готовый объект Certificate с рассчитанным итоговым баллом.</returns>
    public static Certificate GenerateCertificate(string studentName, string courseTitle, List<int> scores)
    {
        double avg = CalculateAverageScore(scores);
        return new Certificate
        {
            StudentName = studentName,
            CourseTitle = courseTitle,
            FinalScore = avg,
            IssueDate = DateTime.Now
        };
    }
}
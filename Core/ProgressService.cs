namespace LearningPlatform.Core;

/// <summary>
/// Сервис для расчёта прогресса прохождения курса и проверки его завершения.
/// </summary>
public static class ProgressService
{
    /// <summary>
    /// Рассчитывает процент выполнения курса на основе количества пройденных элементов.
    /// </summary>
    /// <param name="completedElements">Количество успешно пройденных уроков или заданий.</param>
    /// <param name="totalElements">Общее количество элементов в курсе.</param>
    /// <returns>Процент выполнения от 0 до 100.</returns>
    public static double CalculateCourseProgress(int completedElements, int totalElements)
    {
        if (totalElements <= 0) return 0;
        double progress = (double)completedElements / totalElements * 100;
        return Math.Round(progress, 1);
    }

    /// <summary>
    /// Проверяет, считается ли курс полностью завершённым.
    /// </summary>
    /// <param name="progressPercent">Текущий процент выполнения курса.</param>
    /// <returns>True, если прогресс равен или превышает 100%.</returns>
    public static bool IsCourseCompleted(double progressPercent)
    {
        return progressPercent >= 100.0;
    }
}
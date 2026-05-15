using Xunit;
using LearningPlatform.Core;
using System.Collections.Generic;

namespace LearningPlatform.Tests;

/// <summary>
/// Набор unit-тестов для проверки бизнес-логики модуля Core.
/// </summary>
public class CoreServicesTests
{
    /// <summary>Проверяет расчёт прогресса при полном прохождении.</summary>
    [Fact] public void TestProgress_Full_Returns100() => Assert.Equal(100.0, ProgressService.CalculateCourseProgress(5, 5));

    /// <summary>Проверяет корректное завершение курса при 100% прогрессе.</summary>
    [Fact] public void TestProgress_Complete_ReturnsTrue() => Assert.True(ProgressService.IsCourseCompleted(100));

    /// <summary>Проверяет расчёт среднего балла по списку оценок.</summary>
    [Fact] public void TestGrade_Average_Calc() => Assert.Equal(80.0, GradeService.CalculateAverageScore(new List<int> { 70, 90 }));

    /// <summary>Проверяет формирование сертификата с корректными данными.</summary>
    [Fact]
    public void TestCert_Generate_ContainsData()
    {
        var cert = GradeService.GenerateCertificate("Настя", "C#", new List<int> { 100 });
        Assert.Contains("Настя", cert.GetText());
        Assert.Equal(100.0, cert.FinalScore);
    }
}
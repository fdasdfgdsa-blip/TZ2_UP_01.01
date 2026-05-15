namespace LearningPlatform.Core;

/// <summary>
/// Представляет пользователя образовательной платформы.
/// </summary>
public class User
{
    /// <summary>Получает или задаёт уникальный идентификатор пользователя.</summary>
    public int Id { get; set; }
    /// <summary>Получает или задаёт имя пользователя для отображения в интерфейсе.</summary>
    public string Username { get; set; } = string.Empty;
    /// <summary>Получает или задаёт адрес электронной почты пользователя.</summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>Получает или задаёт роль пользователя: student, teacher или admin.</summary>
    public string Role { get; set; } = "student";
}

/// <summary>
/// Представляет учебный курс, доступный для записи.
/// </summary>
public class Course
{
    /// <summary>Получает или задаёт уникальный идентификатор курса.</summary>
    public int Id { get; set; }
    /// <summary>Получает или задаёт название курса.</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>Получает или задаёт описание программы обучения.</summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>Получает или задаёт идентификатор преподавателя, ведущего курс.</summary>
    public int TeacherId { get; set; }
    /// <summary>Получает или задаёт имя преподавателя (заполняется при JOIN с таблицей users).</summary>
    public string TeacherName { get; set; } = string.Empty;
    /// <summary>Получает или задаёт стоимость курса в рублях.</summary>
    public decimal Price { get; set; }
}

/// <summary>
/// Представляет отдельный урок (лекцию) внутри курса.
/// </summary>
public class Lesson
{
    /// <summary>Получает или задаёт уникальный идентификатор урока.</summary>
    public int Id { get; set; }
    /// <summary>Получает или задаёт идентификатор курса, к которому относится урок.</summary>
    public int CourseId { get; set; }
    /// <summary>Получает или задаёт тему или название урока.</summary>
    public string Topic { get; set; } = string.Empty;
    /// <summary>Получает или задаёт полный текст лекции для отображения в интерфейсе.</summary>
    public string Content { get; set; } = string.Empty;
    /// <summary>Получает или задаёт время начала доступности урока.</summary>
    public DateTime? OpenTime { get; set; }
    /// <summary>Получает или задаёт время окончания доступности урока.</summary>
    public DateTime? CloseTime { get; set; }
    /// <summary>Получает или задаёт флаг, указывающий, что урок доступен всегда.</summary>
    public bool IsAlwaysOpen { get; set; }
    /// <summary>Получает или задаёт длительность урока в минутах.</summary>
    public int DurationMinutes { get; set; }
}

/// <summary>
/// Представляет задание (практику или контрольную) к уроку.
/// </summary>
public class Assignment
{
    /// <summary>Получает или задаёт уникальный идентификатор задания.</summary>
    public int Id { get; set; }
    /// <summary>Получает или задаёт идентификатор урока, к которому привязано задание.</summary>
    public int LessonId { get; set; }
    /// <summary>Получает или задаёт название задания.</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>Получает или задаёт описание и условия выполнения задания.</summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>Получает или задаёт тип задания: practical или exam.</summary>
    public string Type { get; set; } = "practical";
    /// <summary>Получает максимальное количество баллов за задание в зависимости от типа.</summary>
    public int MaxPoints => Type == "exam" ? 100 : 50;
}

/// <summary>
/// Представляет оценку, выставленную за выполненное задание.
/// </summary>
public class Grade
{
    /// <summary>Получает или задаёт уникальный идентификатор записи об оценке.</summary>
    public int Id { get; set; }
    /// <summary>Получает или задаёт идентификатор записи студента на курс.</summary>
    public int EnrollmentId { get; set; }
    /// <summary>Получает или задаёт идентификатор выполненного задания.</summary>
    public int AssignmentId { get; set; }
    /// <summary>Получает или задаёт начисленное количество баллов.</summary>
    public int Score { get; set; }
    /// <summary>Получает или задаёт комментарий преподавателя или системы.</summary>
    public string Feedback { get; set; } = string.Empty;
    /// <summary>Получает или задаёт дату и время выставления оценки.</summary>
    public DateTime GradedAt { get; set; }
}

/// <summary>
/// Представляет текстовый сертификат о прохождении курса.
/// </summary>
public class Certificate
{
    /// <summary>Получает или задаёт имя студента.</summary>
    public string StudentName { get; set; } = string.Empty;
    /// <summary>Получает или задаёт название пройденного курса.</summary>
    public string CourseTitle { get; set; } = string.Empty;
    /// <summary>Получает или задаёт итоговый балл за курс.</summary>
    public double FinalScore { get; set; }
    /// <summary>Получает или задаёт дату выдачи сертификата.</summary>
    public DateTime IssueDate { get; set; }

    /// <summary>
    /// Формирует готовый текстовый шаблон сертификата для вывода или сохранения.
    /// </summary>
    /// <returns>Строка с форматированным текстом сертификата.</returns>
    public string GetText() => $"СЕРТИФИКАТ\n\nСтудент: {StudentName}\nКурс: {CourseTitle}\nИтоговый балл: {FinalScore}\nДата: {IssueDate:dd.MM.yyyy}";
}
using Npgsql;
using LearningPlatform.Core;
using System.Collections.Generic;
using System;
using System.Linq;

namespace LearningPlatform.App;

public static class Data
{
    private static string ConnectionString = "Host=localhost;Port=5432;Database=learning_platform;Username=postgres;Password=sa";
    private static int CurrentUserId = 1;

    private static NpgsqlConnection GetConnection() => new NpgsqlConnection(ConnectionString);

    /// <summary>
    /// Возвращает список всех курсов с информацией о преподавателях.
    /// </summary>
    public static List<Course> GetCourses()
    {
        var list = new List<Course>();
        using (var conn = GetConnection())
        {
            conn.Open();
            string sql = @"SELECT c.id, c.title, c.description, c.teacher_id, u.username as teacher_name, c.price 
                           FROM courses c LEFT JOIN users u ON c.teacher_id = u.id ORDER BY c.id";
            using (var cmd = new NpgsqlCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    list.Add(new Course { Id = reader.GetInt32(0), Title = reader.GetString(1), Description = reader.IsDBNull(2) ? "" : reader.GetString(2), TeacherId = reader.GetInt32(3), TeacherName = reader.IsDBNull(4) ? "Не указан" : reader.GetString(4), Price = reader.GetDecimal(5) });
            }
        }
        return list;
    }

    /// <summary>
    /// Возвращает список ID курсов, на которые записан указанный пользователь.
    /// </summary>
    public static List<int> GetMyEnrolledCourseIds(int userId)
    {
        var ids = new List<int>();
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("SELECT course_id FROM enrollments WHERE user_id = @uid", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) ids.Add(reader.GetInt32(0));
            }
        }
        return ids;
    }

    /// <summary>
    /// Записывает пользователя на указанный курс.
    /// </summary>
    public static void EnrollStudent(int userId, int courseId)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            string sql = "INSERT INTO enrollments (user_id, course_id, status) VALUES (@uid, @cid, 'active') ON CONFLICT (user_id, course_id) DO NOTHING";
            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@cid", courseId);
                cmd.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Возвращает список всех пользователей системы.
    /// </summary>
    public static List<User> GetAllUsers()
    {
        var list = new List<User>();
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("SELECT id, username, email, role FROM users ORDER BY id", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    list.Add(new User { Id = reader.GetInt32(0), Username = reader.GetString(1), Email = reader.GetString(2), Role = reader.GetString(3) });
            }
        }
        return list;
    }

    /// <summary>
    /// Возвращает список уроков для указанного курса.
    /// </summary>
    public static List<Lesson> GetLessonsByCourseId(int courseId)
    {
        var list = new List<Lesson>();
        using (var conn = GetConnection())
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("SELECT id, course_id, topic, content, open_time, close_time, is_always_open, duration_minutes FROM lessons WHERE course_id = @cid ORDER BY sort_order", conn))
            {
                cmd.Parameters.AddWithValue("@cid", courseId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(new Lesson
                        {
                            Id = reader.GetInt32(0),
                            CourseId = reader.GetInt32(1),
                            Topic = reader.GetString(2),
                            Content = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            OpenTime = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                            CloseTime = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                            IsAlwaysOpen = reader.IsDBNull(6) ? false : reader.GetBoolean(6),
                            DurationMinutes = reader.GetInt32(7)
                        });
                }
            }
        }
        return list;
    }

    /// <summary>
    /// Возвращает список заданий, привязанных к урокам указанного курса.
    /// </summary>
    public static List<Assignment> GetAssignmentsByCourseId(int courseId)
    {
        var list = new List<Assignment>();
        using (var conn = GetConnection())
        {
            conn.Open();
            string sql = @"SELECT a.id, a.lesson_id, a.title, a.description, a.type 
                           FROM assignments a JOIN lessons l ON a.lesson_id = l.id 
                           WHERE l.course_id = @cid ORDER BY a.id";
            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@cid", courseId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(new Assignment
                        {
                            Id = reader.GetInt32(0),
                            LessonId = reader.GetInt32(1),
                            Title = reader.GetString(2),
                            Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Type = reader.GetString(4)
                        });
                }
            }
        }
        return list;
    }


    /// <summary>
    /// Отмечает урок как успешно пройденный для текущего пользователя.
    /// </summary>  
    public static void MarkLessonAsCompleted(int lessonId)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            int courseId = 0;
            using (var cmd = new NpgsqlCommand("SELECT course_id FROM lessons WHERE id = @lid", conn)) { cmd.Parameters.AddWithValue("@lid", lessonId); var r = cmd.ExecuteScalar(); if (r != null) courseId = Convert.ToInt32(r); }

            int enrollmentId = GetEnrollmentId(conn, CurrentUserId, courseId);
            if (enrollmentId == 0) return;

            using (var cmd = new NpgsqlCommand("INSERT INTO progress (enrollment_id, lesson_id, is_completed) VALUES (@eid, @lid, true) ON CONFLICT (enrollment_id, lesson_id) DO UPDATE SET is_completed = true", conn))
            {
                cmd.Parameters.AddWithValue("@eid", enrollmentId);
                cmd.Parameters.AddWithValue("@lid", lessonId);
                cmd.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Сохраняет оценку за задание и автоматически рассчитывает баллы.
    /// </summary>
    public static void SaveAssignmentGrade(int assignmentId, int grade)
    {
        using var conn = GetConnection(); conn.Open();
        int lessonId = 0, courseId = 0, enrollmentId = 0;

        using (var cmd = new NpgsqlCommand("SELECT lesson_id FROM assignments WHERE id = @aid", conn))
        {
            cmd.Parameters.AddWithValue("@aid", assignmentId); var r = cmd.ExecuteScalar(); if (r != null) lessonId = Convert.ToInt32(r);
        }
        using (var cmd = new NpgsqlCommand("SELECT course_id FROM lessons WHERE id = @lid", conn))
        {
            cmd.Parameters.AddWithValue("@lid", lessonId); var r = cmd.ExecuteScalar(); if (r != null) courseId = Convert.ToInt32(r);
        }
        using (var cmd = new NpgsqlCommand("SELECT id FROM enrollments WHERE user_id = @uid AND course_id = @cid LIMIT 1", conn))
        {
            cmd.Parameters.AddWithValue("@uid", CurrentUserId); cmd.Parameters.AddWithValue("@cid", courseId); var r = cmd.ExecuteScalar(); if (r != null) enrollmentId = Convert.ToInt32(r);
        }
        if (enrollmentId == 0) return;

        // 1. Считаем общее количество заданий в курсе
        int totalAssignments = 0;
        using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM assignments a JOIN lessons l ON a.lesson_id = l.id WHERE l.course_id = @cid", conn))
        {
            cmd.Parameters.AddWithValue("@cid", courseId); var r = cmd.ExecuteScalar(); if (r != null) totalAssignments = Convert.ToInt32(r);
        }
        if (totalAssignments == 0) totalAssignments = 1;

        // 2. Максимум баллов за одно задание = 100 / кол-во заданий
        int maxPointsPerTask = (int)Math.Round(100.0 / totalAssignments);

        // 3. Начисление: оценка 3 → 33%, 4 → 66%, 5 → 100%, 2 → 0%
        int points = grade >= 3 ? (int)Math.Round(maxPointsPerTask * (grade - 2) / 3.0) : 0;

        // 4. Сохраняем в БД
        using var cmd2 = new NpgsqlCommand("INSERT INTO grades (enrollment_id, assignment_id, score, feedback, graded_at) VALUES (@eid, @aid, @score, @fb, NOW())", conn);
        cmd2.Parameters.AddWithValue("@eid", enrollmentId);
        cmd2.Parameters.AddWithValue("@aid", assignmentId);
        cmd2.Parameters.AddWithValue("@score", points);
        cmd2.Parameters.AddWithValue("@fb", grade >= 3 ? $"Оценка: {grade}" : "Не пройдено (оценка < 3)");
        cmd2.ExecuteNonQuery();
    }

    /// <summary>
    /// Возвращает журнал оценок студента по указанному курсу.
    /// </summary>
    public static List<Grade> GetGradesJournal(int courseId)
    {
        var list = new List<Grade>();
        using (var conn = GetConnection())
        {
            conn.Open();
            int enrollmentId = GetEnrollmentId(conn, CurrentUserId, courseId);
            if (enrollmentId == 0) return list;

            string sql = @"SELECT g.id, g.enrollment_id, g.assignment_id, g.score, g.feedback, g.graded_at 
                           FROM grades g JOIN assignments a ON g.assignment_id = a.id JOIN lessons l ON a.lesson_id = l.id 
                           WHERE l.course_id = @cid";

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@cid", courseId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(new Grade { Id = reader.GetInt32(0), EnrollmentId = reader.GetInt32(1), AssignmentId = reader.GetInt32(2), Score = reader.GetInt32(3), Feedback = reader.IsDBNull(4) ? "" : reader.GetString(4), GradedAt = reader.GetDateTime(5) });
                }
            }
        }
        return list;
    }

    /// <summary>
    /// Рассчитывает общий прогресс курса (уроки + задания) в процентах.
    /// </summary>
    public static double GetCourseProgress(int courseId)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            int totalLessons = 0, completedLessons = 0, totalAssignments = 0, passedAssignments = 0;

            using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM lessons WHERE course_id = @cid", conn))
            {
                cmd.Parameters.AddWithValue("@cid", courseId);
                totalLessons = Convert.ToInt32(cmd.ExecuteScalar());
            }

            int enrollmentId = GetEnrollmentId(conn, CurrentUserId, courseId);
            if (enrollmentId == 0) return 0;

            using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM progress WHERE enrollment_id = @eid AND is_completed = true", conn))
            {
                cmd.Parameters.AddWithValue("@eid", enrollmentId);
                completedLessons = Convert.ToInt32(cmd.ExecuteScalar());
            }

            using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM assignments a JOIN lessons l ON a.lesson_id = l.id WHERE l.course_id = @cid", conn))
            {
                cmd.Parameters.AddWithValue("@cid", courseId);
                totalAssignments = Convert.ToInt32(cmd.ExecuteScalar());
            }

            using (var cmd = new NpgsqlCommand("SELECT COUNT(DISTINCT g.assignment_id) FROM grades g JOIN assignments a ON g.assignment_id = a.id JOIN lessons l ON a.lesson_id = l.id WHERE l.course_id = @cid AND g.score > 0 AND g.enrollment_id = @eid", conn))
            {
                cmd.Parameters.AddWithValue("@cid", courseId);
                cmd.Parameters.AddWithValue("@eid", enrollmentId);
                passedAssignments = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // Если нет заданий, считаем только по урокам
            if (totalAssignments == 0)
            {
                return totalLessons == 0 ? 0 : Math.Min(100, Math.Round((double)completedLessons / totalLessons * 100, 1));
            }

            // Если нет уроков, считаем только по заданиям
            if (totalLessons == 0)
            {
                return Math.Min(100, Math.Round((double)passedAssignments / totalAssignments * 100, 1));
            }

            // Смешанный расчет: 50% уроки + 50% задания
            double lessonProgress = (double)completedLessons / totalLessons * 50;
            double assignmentProgress = (double)passedAssignments / totalAssignments * 50;
            double totalProgress = lessonProgress + assignmentProgress;

            return Math.Min(100, Math.Round(totalProgress, 1));
        }
    }
    private static int GetEnrollmentId(NpgsqlConnection conn, int userId, int courseId)
    {
        using (var cmd = new NpgsqlCommand("SELECT id FROM enrollments WHERE user_id = @uid AND course_id = @cid LIMIT 1", conn))
        {
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@cid", courseId);
            var res = cmd.ExecuteScalar();
            return res != null ? Convert.ToInt32(res) : 0;
        }
    }

    /// <summary>
    /// Вспомогательный метод для расчёта баллов в зависимости от типа задания и оценки.
    /// </summary>
    public static int CalculatePoints(string type, int grade)
    {
        if (grade < 3) return 0;
        int max = type == "exam" ? 100 : 50;
        return Convert.ToInt32(Math.Round((grade - 2) * (max / 3.0)));
    }
}
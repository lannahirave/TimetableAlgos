namespace TimetableGen.Types;

internal class Lesson
{
    public Lesson()
    {
        IsFree = true;
    }

    public Lesson(Lesson les)
    {
        IsFree = false;
        LessonType = les.LessonType;
        Subject = les.Subject;
        Teacher = les.Teacher;
        Group = les.Group;
        Audience = les.Audience;
    }


    public LessonType LessonType { get; private set; }

    public string Subject { get; private set; } = null!;

    public string? Teacher { get; private set; }

    public string Group { get; private set; } = null!;

    public int Audience { get; private set; }

    public bool IsFree { get; private set; }

    public override string ToString()
    {
        return IsFree
            ? "free"
            : string.Concat(LessonType, ", ", Subject, ", ", Teacher, ", Audience: ", Audience, ", Group(s): ",
                Group);
    }

    public void FillForHour(string group, string subject, LessonType type)
    {
        LessonType = type;
        var subjectLecturer = Data.Data.SubjectLecturer;
        var teachers = Data.Data.Teachers;
        var audiences = Data.Data.Audiences;
        if (type == LessonType.Lecture)
        {
            Teacher = subjectLecturer[subject];
        }
        else
        {
            var chosenTeacher = Utilities.PickRandomNumber(0, teachers.Count);
            Teacher = teachers[chosenTeacher];
        }

        Subject = subject;
        Group = group;
        Audience = audiences[Utilities.PickRandomNumber(0, audiences.Count)];
        IsFree = false;
    }

    public override bool Equals(object? obj)
    {
        return obj is Lesson lesson &&
               LessonType == lesson.LessonType &&
               Subject == lesson.Subject &&
               Group == lesson.Group &&
               IsFree == lesson.IsFree;
    }
}
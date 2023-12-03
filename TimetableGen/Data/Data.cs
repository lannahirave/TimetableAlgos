namespace TimetableGen.Data;

public static class Data
{
    public static readonly Dictionary<string, string> SubjectLecturer = new()
    {
        { "Methods of system modelling", "Trotsenko" },
        { "RPZ", "Stadnyk" },
        { "Machine learning", "Krak" },
        { "IS", "Fedorus" },
        { "AI", "Efremov" },
        { "Statistical modelling", "Pashko" }
    };

    public static readonly List<string> Teachers = new()
    {
        "Trotsenko", "Stadnyk", "Krak", "Fedorus", "Efremov", "Pashko"
    };

    public static readonly List<int> Audiences = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    public static readonly List<int> AudienceForLectures = new() { 1, 2, 3, 4 };

    public static readonly List<string> WorkingDays = new()
    {
        "Monday", "Tuesday", "Wednesday", "Thursday", "Friday"
    };

    public static readonly List<string> Specialty = new()
    {
        "MI", "TK", "TTP"
    };

    private static readonly List<string> MiGroups = new() { "MI1", "MI2", "MI3" };
    private static readonly List<string> TkGroups = new() { "TK1", "TK2", "TK3" };
    private static readonly List<string> TtpGroups = new() { "TTP1", "TTP2" };

    private static readonly List<string> MiSubjects = new()
    {
        "Methods of system modelling", "IS", "AI"
    };

    private static readonly List<string> TkSubjects = new()
    {
        "AI", "RPZ", "Statistical modelling"
    };

    private static readonly List<string> TtpSubjects = new()
    {
        "Methods of system modelling", "AI", "IS"
    };

    public static readonly List<string> LessonsTime = new()
    {
        "8:40 - 10:15", "10:35 - 12:10", "12:20 - 13:55", "14:05 - 15:40"
    };

    public static List<string> GetGroups(string specialty)
    {
        return specialty switch
        {
            "MI" => MiGroups,
            "TK" => TkGroups,
            "TTP" => TtpGroups,
            _ => throw new ArgumentException("Wrong specialty!")
        };
    }

    public static List<string> GetSubjects(string specialty)
    {
        return specialty switch
        {
            "MI" => MiSubjects,
            "TK" => TkSubjects,
            "TTP" => TtpSubjects,
            _ => throw new ArgumentException("Wrong specialty!")
        };
    }
}
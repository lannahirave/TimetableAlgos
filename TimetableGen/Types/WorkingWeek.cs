namespace TimetableGen.Types;

internal class WorkingWeek
{
    public readonly Dictionary<string, WorkingDay> Week;
    private bool _extraDay;


    public WorkingWeek(string specialty)
    {
        var workingDays = Data.Data.WorkingDays;
        Week = new Dictionary<string, WorkingDay>();
        foreach (var day in workingDays) Week.Add(day, new WorkingDay());
        FillForWeek(specialty);
    }

    public WorkingWeek(WorkingWeek week)
    {
        Week = new Dictionary<string, WorkingDay>(week.Week);
    }

    private void FillForWeek(string specialty)
    {
        var workingDays = Data.Data.WorkingDays;
        var specialtySubjects = Data.Data.GetSubjects(specialty);
        var specialtyGroups = Data.Data.GetGroups(specialty);

        foreach (var subject in specialtySubjects)
        {
            //for practices
            foreach (var group in specialtyGroups)
                Week[ChooseRandomDay(workingDays)]
                    .FillForDay(group, subject, LessonType.Practice);
            Utilities.PickRandomNumber(0, workingDays.Count);
            //for lectures
            Week[ChooseRandomDay(workingDays)].FillForDay(string.Join(", ", specialtyGroups), subject,
                LessonType.Lecture);
        }
    }
    
    private string ChooseRandomDay(List<string> workingDays)
    {
        while (workingDays.Any())
        {
            var chosenDayNum = Utilities.PickRandomNumber(0, workingDays.Count);
            var chosenDay = workingDays[chosenDayNum];
            if (!Week[chosenDay].IsFull()) return chosenDay;
            workingDays = workingDays.Where(x => x != chosenDay).ToList();
        }

        var extra = "Extra lesson (Didn't fit in the working week)";
        if (!_extraDay)
        {
            Week.Add(extra, new WorkingDay());
            _extraDay = true;
        }

        return extra;
    }
}
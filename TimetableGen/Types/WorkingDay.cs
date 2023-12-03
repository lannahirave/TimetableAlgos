namespace TimetableGen.Types;

internal class WorkingDay
{
    public readonly Dictionary<string, Lesson> Day;

    public WorkingDay()
    {
        var workingHours = Data.Data.LessonsTime;
        Day = new Dictionary<string, Lesson>();
        foreach (var hours in workingHours) Day.Add(hours, new Lesson());
    }

    public bool IsFull()
    {
        return Day.All(hour => !hour.Value.IsFree);
    }

    public void FillForDay(string group, string subject, LessonType type)
    {
        var workingHours = Data.Data.LessonsTime;
        var successfullyAdded = false;
        while (!successfullyAdded)
        {
            var chosenHour = Utilities.PickRandomNumber(0, workingHours.Count);
            if (Day[workingHours[chosenHour]].IsFree)
            {
                Day[workingHours[chosenHour]].FillForHour(group, subject, type);
                successfullyAdded = true;
            }
        }
    }
}
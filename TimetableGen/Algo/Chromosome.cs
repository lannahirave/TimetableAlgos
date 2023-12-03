using TimetableGen.Types;

namespace TimetableGen.Algo;

internal class Chromosome
{
    private readonly Dictionary<string, WorkingWeek> _timetable;


    public Chromosome()
    {
        _timetable = new Dictionary<string, WorkingWeek>();
        GenerateChromosome();
    }

    public Chromosome(Chromosome chr)
    {
        _timetable = new Dictionary<string, WorkingWeek>(chr._timetable);
    }

    public float Deviation { get; private set; }

    public float Likelihood { get; set; }

    public int AmountOfWorkingDays()
    {
        return _timetable.First().Value.Week.Count;
    }

    public int AmountOfSpecialties()
    {
        return _timetable.Count;
    }

    public List<string> Specialties()
    {
        return _timetable.Keys.ToList();
    }

    public override string ToString()
    {
        var toString = "";
        foreach (var spec in _timetable)
        {
            toString += string.Concat(spec.Key, " :\n");
            foreach (var day in spec.Value.Week)
            {
                toString += string.Concat(" ", day.Key, " :\n");
                foreach (var hours in day.Value.Day)
                    toString += string.Concat("  ", hours.Key, " : ", hours.Value.ToString(), "\n");
            }
        }

        return toString;
    }


    private void GenerateChromosome()
    {
        FillForSpecialties();
    }


    private void FillForSpecialties()
    {
        var arr = Data.Data.Specialty;
        foreach (var specialty in arr) _timetable.Add(specialty, new WorkingWeek(specialty));
    }

    public float ComputeDeviation()
    {
        for (var i = 0; i < _timetable.Count() - 1; i++)
        for (var j = i + 1; j < _timetable.Count(); j++)
        {
            var weekSpec1 = _timetable.ElementAt(i).Value.Week; //weeks
            var weekSpec2 = _timetable.ElementAt(j).Value.Week;
            for (var k = 0; k < weekSpec1.Count; k++)
            {
                var daySpec1 = weekSpec1.ElementAt(k).Value; //days
                var daySpec2 = weekSpec2.ElementAt(k).Value;
                for (var m = 0; m < daySpec1.Day.Count; m++)
                {
                    var hourSpec1 = daySpec1.Day.ElementAt(m).Value; //hours
                    var hourSpec2 = daySpec2.Day.ElementAt(m).Value;

                    CheckBetweenSpecialties(hourSpec1, hourSpec2); //check conflicts between different specialties
                    AudiencesTypeFitness(hourSpec1);
                }
            }
        }

        RequiredLessonsFitness();
        return Deviation;
    }

    private void CheckBetweenSpecialties(Lesson hourSpec1, Lesson hourSpec2)
    {
        if (!hourSpec1.IsFree && !hourSpec2.IsFree)
        {
            if (hourSpec1.Teacher == hourSpec2.Teacher) ++Deviation;
            if (hourSpec1.Audience == hourSpec2.Audience) ++Deviation;
        }
    }

    private void AudiencesTypeFitness(Lesson hourSpec1)
    {
        if (!hourSpec1.IsFree)
            if (hourSpec1.LessonType == LessonType.Lecture &&
                !Utilities.LectureAudiences.Contains(hourSpec1.Audience))
                ++Deviation;
    }


    private void RequiredLessonsFitness()
    {
        var currentLessonsSet = GetAllLessonsSet();
        var requiredLessonsSet = new Dictionary<string, List<Lesson>>(Utilities.RequiredLessonsSet);

        var lackLessons = 0;
        var overLessons = 0;
        for (var i = 0; i < currentLessonsSet.Count; i++)
        {
            var specialty = currentLessonsSet.ElementAt(i).Key;
            for (var j = 0; j < requiredLessonsSet[specialty].Count; j++)
                if (!currentLessonsSet[specialty].Contains(requiredLessonsSet[specialty].ElementAt(j)))
                    ++lackLessons;
            var rawOverLessons = currentLessonsSet[specialty].Count - requiredLessonsSet[specialty].Count;
            overLessons += rawOverLessons >= 0 ? rawOverLessons : 0;
        }

        Deviation += lackLessons + overLessons;
    }


    public Chromosome[] DoubleDaysCrossover(Chromosome secondParent, int crossoverLine, string specialty)
    {
        var first = new Chromosome(this);
        var second = new Chromosome(secondParent);

        for (var i = 0; _timetable.ElementAt(i).Value != _timetable[specialty]; i++)
        {
            second._timetable[second._timetable.ElementAt(i).Key] = _timetable.ElementAt(i).Value;

            first._timetable[_timetable.ElementAt(i).Key] = secondParent._timetable.ElementAt(i).Value;
        }


        var weekFirst = new WorkingWeek(_timetable[specialty]);
        var weekSecond = new WorkingWeek(secondParent._timetable[specialty]);
        for (var j = 0; j < crossoverLine; j++)
        {
            weekFirst.Week[weekFirst.Week.ElementAt(j).Key] =
                secondParent._timetable[specialty].Week.ElementAt(j).Value;
            weekSecond.Week[weekSecond.Week.ElementAt(j).Key] = _timetable[specialty].Week.ElementAt(j).Value;
        }

        first._timetable[specialty] = weekFirst;
        second._timetable[specialty] = weekSecond;

        return new[] { first, second };
    }

    //required for checking amount of specific lectures/practices
    public Dictionary<string, List<Lesson>> GetAllLessonsSet()
    {
        var lessonsSet = new Dictionary<string, List<Lesson>>();
        for (var i = 0; i < _timetable.Count; i++)
        {
            lessonsSet.Add(_timetable.ElementAt(i).Key, new List<Lesson>());
            var specialtyWeek = _timetable.ElementAt(i).Value;
            for (var j = 0; j < specialtyWeek.Week.Count; j++)
            {
                var day = specialtyWeek.Week.ElementAt(j).Value;
                for (var k = 0; k < day.Day.Count; k++)
                    if (!day.Day.ElementAt(k).Value.IsFree)
                        lessonsSet[_timetable.ElementAt(i).Key].Add(new Lesson(day.Day.ElementAt(k).Value));
            }
        }

        return lessonsSet;
    }
}
using System.Security.Cryptography;
using TimetableGen.Algo;
using TimetableGen.Types;

namespace TimetableGen;

internal static class Utilities
{
    public static readonly List<int> LectureAudiences = Data.Data.AudienceForLectures;
    public static Dictionary<string, List<Lesson>> RequiredLessonsSet = null!;

    public static void LoadRequiredLessonsSet()
    {
        var amountSatisfying = new Chromosome();
        RequiredLessonsSet = amountSatisfying.GetAllLessonsSet();
    }

    public static int PickRandomNumber(int from, int to)
    {
        using var rng = RandomNumberGenerator.Create();
        var rno = new byte[4];
        rng.GetBytes(rno);

        var randomNumber = BitConverter.ToInt32(rno, 0);
        return from + Math.Abs(randomNumber % (to - from));
    }
}
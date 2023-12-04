namespace TimetableGen.Algo;

internal class GeneticMachine
{
    private const int PercentageOfMutations = 45;

    private float _currentFitness;
    private List<Chromosome> _generation;
    private int _generationNum;
    private float _previousFitness;


    public GeneticMachine(int startPopulation)
    {
        LoadStaticLimitations();
        _generation = new List<Chromosome>(startPopulation);
        for (var i = 0; i < startPopulation; i++)
        {
            var start = new Chromosome();
            _generation.Add(start);
        }

        ComputeParameters();
    }

    private void LoadStaticLimitations()
    {
        Utilities.LoadRequiredLessonsSet();
    }

    private void ComputeParameters()
    {
        var inversedCoefficientSum =
            _generation.Select(t => t.ComputeDeviation()).Select(deviation => deviation == 0 ? 0 : 1 / deviation).Sum();

        _previousFitness = _currentFitness;
        var j = 1;
        _currentFitness = 0;
        for (var i = 0; i < _generation.Count; i++, j++)
        {
            _generation[i].Likelihood = _generation[i].Deviation == 0
                ? 100
                : 1 / _generation[i].Deviation / inversedCoefficientSum * 100;
            _currentFitness += _generation[i].Deviation;
        }

        _currentFitness /= j;
    }

    private bool NextGeneration()
    {
        if (!CheckForRightAnswers())
        {
            var probabilityLine = BuildProbabilityLine();
            _generation = FillNewGeneration(probabilityLine);
            ComputeParameters();
            ++_generationNum;
            return false;
        }

        return true;
    }

    public void FindAnswer()
    {
        Console.WriteLine(this);
        while (!NextGeneration())
        {
            Console.WriteLine(this);
            if (_currentFitness >= _previousFitness) Mutation();
        }
    }

    private bool CheckForRightAnswers()
    {
        foreach (var t in _generation)
            if ((int)Math.Floor(t.Likelihood) == 100)
            {
                Console.WriteLine("Found answer:");
                Console.WriteLine(t);
                return true;
            }

        return false;
    }

    private void Mutation()
    {
        var numOfMutations = PercentageOfMutations * _generation.Count / 100;
        for (var i = 0; i < numOfMutations; i++)
        {
            var randMutation = Utilities.PickRandomNumber(0, _generation.Count - 1);
            _generation[randMutation] = new Chromosome();
        }

        ComputeParameters();
    }

    private List<Chromosome> FillNewGeneration(List<IEnumerable<int>> probabilityLine)
    {
        var children = new List<Chromosome>();
        var daysNum = _generation[0].AmountOfWorkingDays();
        var specNum = _generation[0].AmountOfSpecialties();
        var specialties = _generation[0].Specialties();
        for (int i = 0, j = 0; i < daysNum * specNum; i++, j++)
        {
            if (j > daysNum - 1)
            {
                j = 0;
                specialties.RemoveAt(0);
            }

            var parent1 = new Chromosome(ChooseFirstParent(probabilityLine, out var parent1Range));
            var parent2 = new Chromosome(ChooseSecondParent(probabilityLine, parent1Range));

            var parentsChildren = parent1.DoubleDaysCrossover(parent2, j, specialties[0]);
            children.Add(parentsChildren[0]);
        }

        return children;
    }

    private Chromosome ChooseFirstParent(List<IEnumerable<int>> probabilityLine,
        out IEnumerable<int> firstParentRange)
    {
        var randParent1 = Utilities.PickRandomNumber(0, 100);

        for (var j = 0; j < probabilityLine.Count; j++)
            if (probabilityLine[j].Contains(randParent1))
            {
                firstParentRange = probabilityLine[j];
                return _generation[j];
            }

        firstParentRange = Enumerable.Range(0, 0);
        return new Chromosome();
    }

    private Chromosome ChooseSecondParent(List<IEnumerable<int>> probabilityLine, IEnumerable<int> firstParentRange)
    {
        var randParent2 = Utilities.PickRandomNumber(0, 100);
        // choosing number that don't give same parent (avoid picking parent1)
        while (true)
            if (firstParentRange.Contains(randParent2))
                randParent2 = Utilities.PickRandomNumber(0, 100);
            else
                break;

        for (var j = 0; j < probabilityLine.Count; j++)
            if (probabilityLine[j].Contains(randParent2))
                return _generation[j];
        
        return new Chromosome();
    }


    private List<IEnumerable<int>> BuildProbabilityLine()
    {
        var result = new List<IEnumerable<int>>();

        float prev = 0;
        foreach (var next in _generation.Select(t => prev + t.Likelihood))
        {
            result.Add(Enumerable.Range((int)prev, (int)Math.Ceiling(next - prev)));
            prev = next;
        }

        return result;
    }

    public override string ToString()
    {
        var result = string.Concat("Generation number = ", _generationNum, "\n");
        var i = 1;
        foreach (var chromosome in _generation)
        {
            result += string.Concat("Chromosome #", i, " , likelihood: ", chromosome.Likelihood.ToString("F2"),
                " % , fitness: ",
                chromosome.Deviation, chromosome.Deviation == 0 ? " ANSWER FOUND" : "", "\n");
            ++i;
        }

        result += string.Concat("Average fitness: ", _currentFitness.ToString("F2"), "\n");
        return result;
    }
}
using System;

class Program
{
    static int populationSize = 8;
    static int[][] parent = new int[populationSize][];
    static int[][] children = new int[populationSize][];
    static int[] parentFitness = new int[populationSize];
    static int[] childrenFitness = new int[populationSize];

    static void Main(string[] args)
    {
        InitialPopulation();
        int count = 0;
        double find = double.NaN;

        while (true)
        {
            count++;
            if (count % 1000 == 0)
            {
                Console.WriteLine($"First {count} Subsets");
            }

            for (int k = 0; k < populationSize / 2; k++)
            {
                Hybridization();
            }

            for (int k = 0; k < populationSize; k++)
            {
                if (childrenFitness[k] == 28)
                {
                    find = k;
                    break;
                }
            }

            if (!double.IsNaN(find))
            {
                break;
            }

            Array.Copy(children, parent, populationSize);
            Array.Copy(childrenFitness, parentFitness, populationSize);

            Array.Clear(children, 0, populationSize);
            Array.Clear(childrenFitness, 0, populationSize);
        }

        int[] res = children[(int)find];
        Console.WriteLine("Result:");
        foreach (int item in res)
        {
            Console.Write($"{item} ");
        }

        Console.WriteLine("\nFind results:");
        int[,] resQueen = new int[8, 8];

        for (int t = 0; t < 8; t++)
        {
            resQueen[res[t], t] = 1;
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Console.Write(resQueen[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    static void InitialIndividual()
    {
        int[] individual = new int[8];
        Random random = new Random();

        for (int i = 0; i < 8; i++)
        {
            int a = random.Next(0, 8);
            individual[i] = a;
        }

        int fitScore = UpdateFitnessScore(individual);
        parentFitness[Array.IndexOf(parentFitness, 0)] = fitScore;
        parent[Array.IndexOf(parent, null)] = individual;
    }

    static int UpdateFitnessScore(int[] individual)
    {
        int value = 0;

        for (int i = 0; i < 8; i++)
        {
            for (int j = i + 1; j < 8; j++)
            {
                if (individual[i] != individual[j])
                {
                    int x = j - i;
                    int y = Math.Abs(individual[i] - individual[j]);

                    if (x != y)
                    {
                        value += 1;
                    }
                }
            }
        }

        return value;
    }

    static void InitialPopulation()
    {
        for (int i = 0; i < populationSize; i++)
        {
            InitialIndividual();
        }
    }

    static int Select()
    {
        int totalScore = 0;

        foreach (int fit in parentFitness)
        {
            totalScore += fit;
        }

        Random random = new Random();
        int num = random.Next(0, totalScore);
        int frontScore = 0;

        for (int i = 0; i < populationSize; i++)
        {
            frontScore += parentFitness[i];

            if (frontScore >= num)
            {
                return i;
            }
        }

        return 0;
    }

    static int[] Mutation(int[] changeIndividual)
    {
        Random random = new Random();
        int pos = random.Next(0, 8);
        int change = random.Next(0, 8);
        changeIndividual[pos] = change;

        return changeIndividual;
    }

    static void Hybridization()
    {
        int first = Select();
        int second = Select();
        int[][] selectedParents = new int[2][] { parent[first], parent[second] };
        Random random = new Random();
        int pos1 = random.Next(0, 7);
        int pos2 = random.Next(0, 7);

        if (pos1 > pos2)
        {
            int temp = pos1;
            pos1 = pos2;
            pos2 = temp;
        }

        int rangeLength = pos2 - pos1;
        if (rangeLength >= selectedParents[1].Length) // Check if the range length exceeds the size of the second parent
        {
            pos2 = pos1 + selectedParents[1].Length - 1; // Adjust pos2 to the maximum valid value
            rangeLength = pos2 - pos1;
        }

        int[] tmp = new int[rangeLength + 1];
        for (int i = pos1; i <= pos2; i++)
        {
            tmp[i - pos1] = selectedParents[0][i];
            selectedParents[0][i] = selectedParents[1][i];
            selectedParents[1][i] = tmp[i - pos1];
        }

        double may = random.NextDouble();
        if (may > 0.5)
        {
            selectedParents[0] = Mutation(selectedParents[0]);
        }

        may = random.NextDouble();
        if (may > 0.5)
        {
            selectedParents[1] = Mutation(selectedParents[1]);
        }

        int firstFit = UpdateFitnessScore(selectedParents[0]);
        int secondFit = UpdateFitnessScore(selectedParents[1]);

        children[Array.IndexOf(children, null)] = selectedParents[0];
        children[Array.IndexOf(children, null)] = selectedParents[1];
        childrenFitness[Array.IndexOf(childrenFitness, 0)] = firstFit;
        childrenFitness[Array.IndexOf(childrenFitness, 0)] = secondFit;
    }
}

using System;

class Program
{
    static int populationSize = 8; // Количество особей в популяции
    static int[][] parent = new int[populationSize][]; // Массив родителей (особей)
    static int[][] children = new int[populationSize][]; // Массив потомков (особей)
    static int[] parentFitness = new int[populationSize]; // Массив значений приспособленности для родителей
    static int[] childrenFitness = new int[populationSize]; // Массив значений приспособленности для потомков

    static void Main(string[] args)
    {
        InitialPopulation(); // Создание начальной популяции
        int count = 0; // Счетчик поколений
        double find = double.NaN; // Переменная для хранения индекса найденной особи

        while (true) // Начало эволюционного процесса
        {
            count++; // Увеличение счетчика поколений
            if (count % 1000 == 0) // Каждые 1000 поколений выводится информация о текущем состоянии
            {
                Console.WriteLine($"First {count} Subsets");
            }

            for (int k = 0; k < populationSize / 2; k++) // Создание новых потомков путем скрещивания
            {
                Hybridization();
            }

            for (int k = 0; k < populationSize; k++) // Проверка на достижение оптимального решения
            {
                if (childrenFitness[k] == 28) // Если значение приспособленности равно 28 (оптимальное решение)
                {
                    find = k; // Сохранение индекса найденной особи
                    break;
                }
            }

            if (!double.IsNaN(find)) // Если найдено оптимальное решение, процесс завершается
            {
                break;
            }

            Array.Copy(children, parent, populationSize); // Обновление родителей новыми потомками
            Array.Copy(childrenFitness, parentFitness, populationSize); // Обновление значений приспособленности родителей

            Array.Clear(children, 0, populationSize); // Очистка массива потомков
            Array.Clear(childrenFitness, 0, populationSize); // Очистка массива значений приспособленности потомков
        }

        int[] res = children[(int)find]; // Найденная оптимальная особь
        Console.WriteLine("Result:");
        foreach (int item in res) // Вывод найденной оптимальной особи
        {
            Console.Write($"{item} ");
        }

        Console.WriteLine("\nFind results:");
        int[,] resQueen = new int[8, 8]; // Доска для отображения решения задачи

        for (int t = 0; t < 8; t++) // Заполнение доски на основе найденной оптимальной особи
        {
            resQueen[res[t], t] = 1;
        }

        for (int i = 0; i < 8; i++) // Вывод доски с решением задачи
        {
            for (int j = 0; j < 8; j++)
            {
                Console.Write(resQueen[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    static void InitialIndividual() // Генерация начальной особи
    {
        int[] individual = new int[8]; // Массив для хранения генов особи
        Random random = new Random(); // Генератор случайных чисел

        for (int i = 0; i < 8; i++) // Заполнение генов случайными значениями
        {
            int a = random.Next(0, 8);
            individual[i] = a;
        }

        int fitScore = UpdateFitnessScore(individual); // Вычисление значения приспособленности особи
        parentFitness[Array.IndexOf(parentFitness, 0)] = fitScore; // Сохранение значения приспособленности
        parent[Array.IndexOf(parent, null)] = individual; // Сохранение особи в массив родителей
    }

    static int UpdateFitnessScore(int[] individual) // Вычисление значения приспособленности особи
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

        return value; // Возвращение значения приспособленности
    }

    static void InitialPopulation() // Создание начальной популяции
    {
        for (int i = 0; i < populationSize; i++)
        {
            InitialIndividual(); // Генерация начальных особей
        }
    }

    static int Select() // Выбор особи для скрещивания
    {
        int totalScore = 0;

        foreach (int fit in parentFitness) // Вычисление суммы значений приспособленности всех особей
        {
            totalScore += fit;
        }

        Random random = new Random(); // Генератор случайных чисел
        int num = random.Next(0, totalScore); // Случайное число в диапазоне от 0 до суммы значений приспособленности
        int frontScore = 0;

        for (int i = 0; i < populationSize; i++) // Поиск выбранной особи на основе суммы значений приспособленности
        {
            frontScore += parentFitness[i];

            if (frontScore >= num)
            {
                return i; // Возвращение индекса выбранной особи
            }
        }

        return 0;
    }

    static int[] Mutation(int[] changeIndividual) // Мутация особи
    {
        Random random = new Random(); // Генератор случайных чисел
        int pos = random.Next(0, 8); // Выбор случайной позиции для мутации
        int change = random.Next(0, 8); // Новое случайное значение гена для мутации
        changeIndividual[pos] = change; // Мутация гена

        return changeIndividual; // Возвращение измененной особи
    }

    static void Hybridization() // Скрещивание особей
    {
        int first = Select(); // Выбор первой особи для скрещивания
        int second = Select(); // Выбор второй особи для скрещивания
        int[][] selectedParents = new int[2][] { parent[first], parent[second] }; // Массив выбранных для скрещивания особей
        Random random = new Random(); // Генератор случайных чисел
        int pos1 = random.Next(0, 7); // Выбор случайной позиции начала участка скрещивания
        int pos2 = random.Next(0, 7); // Выбор случайной позиции конца участка скрещивания

        if (pos1 > pos2) // Обеспечение правильного порядка позиций участка скрещивания
        {
            int temp = pos1;
            pos1 = pos2;
            pos2 = temp;
        }

        int rangeLength = pos2 - pos1;
        if (rangeLength >= selectedParents[1].Length) // Проверка, не превышает ли длина участка скрещивания размер второй особи
        {
            pos2 = pos1 + selectedParents[1].Length - 1; // Если да, корректировка конечной позиции участка
            rangeLength = pos2 - pos1;
        }

        int[] tmp = new int[rangeLength + 1]; // Временный массив для обмена генами между родителями
        for (int i = pos1; i <= pos2; i++) // Обмен генами между выбранными особями в участке скрещивания
        {
            tmp[i - pos1] = selectedParents[0][i];
            selectedParents[0][i] = selectedParents[1][i];
            selectedParents[1][i] = tmp[i - pos1];
        }

        double may = random.NextDouble();
        if (may > 0.5) // Вероятность мутации первой особи
        {
            selectedParents[0] = Mutation(selectedParents[0]); // Мутация первой особи
        }

        may = random.NextDouble();
        if (may > 0.5) // Вероятность мутации второй особи
        {
            selectedParents[1] = Mutation(selectedParents[1]); // Мутация второй особи
        }

        int firstFit = UpdateFitnessScore(selectedParents[0]); // Вычисление приспособленности первой потомственной особи
        int secondFit = UpdateFitnessScore(selectedParents[1]); // Вычисление приспособленности второй потомственной особи

        children[Array.IndexOf(children, null)] = selectedParents[0]; // Добавление первой потомственной особи в массив потомков
        children[Array.IndexOf(children, null)] = selectedParents[1]; // Добавление второй потомственной особи в массив потомков
        childrenFitness[Array.IndexOf(childrenFitness, 0)] = firstFit; // Сохранение значения приспособленности первой потомственной особи
        childrenFitness[Array.IndexOf(childrenFitness, 0)] = secondFit; // Сохранение значения приспособленности второй потомственной особи
    }
}

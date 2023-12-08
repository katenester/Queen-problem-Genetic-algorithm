
namespace QueensGenetic
{
    class Boards
    {
        // Расстановка
        public int[] Board{ get; set; }
        //Функция пригодности. Максимальное количество диагональных стролновений - 28. 
        public int Fitness { get; set; }
        //Размер доски 
        public int Size { get; set; }

        /// <summary>
        /// Конструктор досок.
        /// </summary>
        /// <param name="Board">Represents a Board</param>
        public Boards(int[] Board)
        {
            this.Board = Board;
            Fitness = CalculateFitness();
            Size = Board.Length;
        }

        /// <summary>
        /// Определяет пригодность доски.
        /// </summary>
        /// <returns>Пригодность доски</returns>
        public int CalculateFitness()
        {
            int Fitness = 0;
            /* Проверяем каждого ферзя на доске,
             * и если он не атакует другого ферзя, 
             * увеличиваем пригодность доски на единицу.
            */
            for (int i = 0; i < Board.Length; i++)
            {
                for (int j = i+1; j < Board.Length; j++)
                {
                    // Проверяем атаки диагоналей и строк. Если никто никого не бьёт, то функция пригодности++
                    if ((Math.Abs(Board[i]- Board[j]) != Math.Abs(j - i)) && (Board[i] != Board[j])) 
                    {
                        Fitness++;
                    }

                }
            }
            return Fitness;
        }

        /// <summary>
        /// Определяет , есляется ли доска решением 
        /// </summary>
        /// <returns></returns>
        public bool Solved()
        {
            for (int i = 0; i < Board.Length; i++)
            {
                for (int j = i+1; j < Board.Length; j++)
                {
                    // Проверяем атаки диагоналей и строк. Если драка, то функция решение не годное
                    if ((Math.Abs(Board[i] - Board[j]) == Math.Abs(j - i)) || (Board[i] == Board[j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Выполняет мутацию на одной королеве.
        /// </summary>
        public void Mutate()
        {
            Random r = new ();

            int randomPosition = r.Next(0, Size);
            int randomQueenValue = r.Next(1, Size + 1);
            Board[randomPosition] = randomQueenValue;
        }

        /// <summary>
        /// Возвращает доску в виде массива
        /// </summary>
        /// <returns>доска</returns>
        public int[] GetBoard()
        {
            return (int[])Board.Clone();
        }

        /// <summary>
        /// Возвращает строковое представление доски.
        /// </summary>
        /// <returns>Строковое представление доски.</returns>
        public override string ToString()
        {
            string str = string.Empty;

            for (int i = 0; i < Board.Length; i++)
            {
                str += Board[i] + " ";
            }

            return str;
        }

        /// <summary>
        /// Печать решения.
        /// </summary>
        public void PrintBoard()
        {
            string str = String.Empty;

            for (int i = 0; i < Board.Length; i++)
            {
                for (int j = 0; j < Board.Length; j++)
                {
                    if (Board[j] == i + 1)
                    {
                        str += "Q ";
                    }
                    else
                    {
                        str += ". ";
                    }
                }

                str += "\n\n";
            }

            Console.Out.Write(str);
        }
    }
    class GenAlg
    {
        // Rоличество ферзей на шахматной доске.
        const int NUM_QUEENS = 8;
        // Размер популяции.
        const int POP_SIZE = 500;
        // Вероятность мутации (в данном случае, 10%).
        const int MUTATION = 10; 
        // Массив, содержащий особи (доски) в популяции.
        static Boards[] POPULATION = new Boards[POP_SIZE];

        public static void Main()
        {
            //Вызываем метод geneticAlg() для выполнения генетического алгоритма.
            Boards solution = GeneticAlg();
            //Выводит решение на экран.
            Console.Out.WriteLine(solution.ToString() + "\n");
            solution.PrintBoard();
            Console.ReadLine();
        }

        /// <summary>
        /// Реализует одноточечное скрещивание двух родителей (parentX и parentY), чтобы создать потомка.
        /// </summary>
        /// <param name="parentX">Первый родитель</param>
        /// <param name="parentY">Второй родитель</param>
        /// <returns>Ребенок в результате скрещивания.</returns>
        public static Boards Crossover(Boards parentX, Boards parentY)
        {
            Boards child;
            Random r = new();
            //Выбираем случайную точку пересечения и создаём потомка, комбинируя части родительских досок.
            int crossoverPoint = r.Next(1, NUM_QUEENS - 1);

            //Получаем обе части массива от родителей, затем создаём один дочерний элемент.
            int[] firstHalf = parentX.GetBoard().Take(crossoverPoint).ToArray();
            int[] secondHalf = parentY.GetBoard().Skip(crossoverPoint).Take(NUM_QUEENS - crossoverPoint).ToArray();
            int[] childArray = firstHalf.Concat(secondHalf).ToArray();

            child = new Boards(childArray);

            return child;

        }

        /// <summary>
        /// Создает начальную популяцию случайных досок размером POP_SIZE. 
        /// Случайная популяция состоит из растановки ферзей, которые не бьют друг друга по стобцам и строкам.
        /// </summary>
        public static void CreatePopulation()
        {
            int[] initParent = new int[NUM_QUEENS];
            Random r = new();

            // Заполнение популяции случайными первоначальными родителями.
            for (int i = 0; i < POP_SIZE; i++)
            {
                for (int j = 0; j < initParent.Length; j++)
                {
                    initParent[j] = r.Next(1, NUM_QUEENS + 1);
                    while (true)
                    {
                        initParent[j] = r.Next(1, NUM_QUEENS + 1);
                        int k ;
                        // Проверка уникальности(без драк для строк ) (столбцы 100% мирные)
                        for(k = 0; k <= j; k++)
                        {
                            if (initParent[k]== initParent[j])
                            {
                                //Если столкновение выходим из цикла
                                break;
                            }
                        }
                        // Если столкновений нет => выходим из цикла 
                        if (k == j)
                        {
                            break;
                        }
                    }
                }

                POPULATION[i] = new Boards(initParent);
                Console.WriteLine(POPULATION[i] + " ");
            }
        }

        /// <summary>
        ///  Выбирает случайным образом родителя из совокупности по уровню пригодности.
        /// </summary>
        /// <returns>Родитель</returns>
        public static Boards ChooseParent()
        {
            Random r = new();
            // Общая пригодность
            int total = 0;

            // Получаем текущую общую пригодность
            for (int i = 0; i < POPULATION.Length; i++)
            {
                total += POPULATION[i].Fitness;
            }

            int random = r.Next(0, total);

            // Выбираем случайного родителя с более высоким уровнем пригодности
            for (int i = 0; i < POPULATION.Length; i++)
            {
                if (random < POPULATION[i].Fitness)
                {
                    return POPULATION[i];
                }

                random -= POPULATION[i].Fitness;
            }

#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
            return null;
#pragma warning restore CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
        }

        /// <summary>
        /// Выполняет генетический алгоритм
        /// </summary>
        /// <returns>Child which contains a valid solution</returns>
        public static Boards GeneticAlg()
        {
            //Дочерний элемент, содержащий допустимое решение
            Boards child;
            Random r = new();
            // Временная популяция размерностью POP_SIZE
            Boards[] tempPopulation = new Boards[POP_SIZE];
            // Лучшее значение функции пригодности
            int highestFitness = 0;
            // Подсчёт поколений
            int generation = 0;
            // Создаём начальную популяцию 
            CreatePopulation();
            while (true)
            {
                //Счётчик поколений 
                generation++;

                // Создаём новые поколения особей, пока не будет найдено решение.
                for (int i = 0; i < POP_SIZE; i++)
                {
                    // CВыбираем двух родителей и создаём ребенка. Пока выбор рандомный 
                    child = Crossover(ChooseParent(), ChooseParent());

                    // Проверяем, является ли child решением.
                    if (child.Solved())
                    {
                        Console.Out.WriteLine("Fitness: " + child.Fitness + " Generation: " + generation);
                        return child;
                    }

                    // Изменяем мутацию.
                    if (MUTATION > r.Next(0, 100))
                    {
                        child.Mutate();
                    }

                    // Проверяем пригодность ребенка 
                    if (child.Fitness > highestFitness)
                    {
                        highestFitness = child.Fitness ;
                    }

                    tempPopulation[i] = child;
                }

                POPULATION = tempPopulation;
                Console.Out.WriteLine("Fitness: " + highestFitness + " Generation: " + generation);
            }
        }
    }
}

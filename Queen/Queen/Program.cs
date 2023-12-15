
using System.Diagnostics;
using System.Xml.Linq;

namespace QueensGenetic
{
    class Boards
    {
        /// <summary>
        /// Расстановка.
        /// </summary>
        public int[] Board{ get; set; }

        /// <summary>
        /// Функция пригодности. Максимальное количество диагональных столкновений - 28.
        /// </summary>
        public int Fitness { get; set; }

        /// <summary>
        /// Размер доски.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Конструктор досок.
        /// </summary>
        /// <param name="Board">Расстановка фигур на доске.</param>
        public Boards(int[] Board)
        {
            this.Board = Board;
            Fitness = CalculateFitness();
            Size = Board.Length;
        }

        /// <summary>
        /// Определяет пригодность доски.
        /// </summary>
        /// <returns>Пригодность доски.</returns>
        public int CalculateFitness()
        {
            var Fitness = 0;
            /* Проверяем каждого ферзя на доске,
             * и если он не атакует другого ферзя, 
             * увеличиваем пригодность доски на единицу.
            */
            for (var i = 0; i < Board.Length; i++)
            {
                for (var j = i+1; j < Board.Length; j++)
                {
                    // Проверяем атаки диагоналей и строк. Если никто никого не бьёт, то увеличиваем функцию пригодности.
                    if ((Math.Abs(Board[i]- Board[j]) != Math.Abs(j - i)) && (Board[i] != Board[j])) 
                    {
                        Fitness++;
                    }
                }
            }
            return Fitness;
        }

        /// <summary>
        /// Определяет , является ли доска решением.
        /// </summary>
        /// <returns>True - если решение, false - если не является решением (есть столкновения).</returns>
        public bool Solved()
        {
            for (var i = 0; i < Board.Length; i++)
            {
                for (var j = i+1; j < Board.Length; j++)
                {
                    // Проверяем атаки диагоналей и строк. Если столкновения, то решение не подходит.
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
            // Выбираем любую позицию (столбец).
            int randomPosition = r.Next(0, Size);
            // Выбираем любую королеву (расположение на строке).
            int randomQueenValue = r.Next(1, Size + 1);
            Board[randomPosition] = randomQueenValue;
        }

        /// <summary>
        /// Возвращает доску в виде массива.
        /// </summary>
        /// <returns>Копия расстановки доски в виде массива.</returns>
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
            string str = string.Empty;
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
            Console.Write(str);
        }
    }
    class GenAlg
    {
        /// <summary>
        /// Количество ферзей на шахматной доске.
        /// </summary>
        static int NumQueens;

        /// <summary>
        /// Размер популяции.
        /// </summary>
        const int popSize = 500;

        /// <summary>
        /// Вероятность мутации (в данном случае, 10%).
        /// </summary>
        const int mutation = 10;

        /// <summary>
        /// Массив, содержащий особи (доски) в популяции.
        /// </summary>
        static Boards[] population = new Boards[popSize];

        public static void Main()
        {
            //Пользовательский ввод.
            while (true)
            {
                Console.WriteLine("Введите число королев: ");
                if (int.TryParse(Console.ReadLine(),out NumQueens) && (NumQueens>0))
                { 
                   Console.WriteLine("Запуск алгоритма"); 
                    break;
                }
                else
                {
                    Console.WriteLine("Ошибка. Некорректное значение");
                }
            }
            //Время работы программы.
            Stopwatch stopWatch = new();
            stopWatch.Start();
            //Вызываем метод geneticAlg() для выполнения генетического алгоритма.
            Boards solution = GeneticAlg();
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            Console.WriteLine($"Время работы алгоритма : {ts}");
            //Выводит решение на экран.
            Console.WriteLine(solution.ToString() + "\n");
            solution.PrintBoard();
            Console.ReadLine();
        }

        /// <summary>
        /// Реализует одноточечное скрещивание двух родителей (parentX и parentY), чтобы создать потомка.
        /// </summary>
        /// <param name="parentX">Первый родитель.</param>
        /// <param name="parentY">Второй родитель.</param>
        /// <returns>Ребенок в результате скрещивания.</returns>
        public static Boards Crossover(Boards parentX, Boards parentY)
        {
            Boards child;
            Random r = new();
            //Выбираем случайную точку пересечения и создаём потомка, комбинируя части родительских досок.
            int crossoverPoint = r.Next(1, NumQueens - 1);
            //Получаем обе части массива от родителей, затем создаём один дочерний элемент.
            //LINQ: Take() извлекает определенное число элементов(crossoverPoint). 
            //ToArray() берет входную последовательность с элементами типа T и возвращает массив элементов типа Т.
            int[] firstHalf = parentX.GetBoard().Take(crossoverPoint).ToArray();
            //LINQ: Skip() пропускает определенное количество элементов.
            int[] secondHalf = parentY.GetBoard().Skip(crossoverPoint).Take(NumQueens - crossoverPoint).ToArray();
            //LINQ: Concat() соединяет две входные последовательности и выдает одну выходную последовательность
            int[] childArray = firstHalf.Concat(secondHalf).ToArray();
            child = new Boards(childArray);
            return child;
        }

        /// <summary>
        /// Создает начальную популяцию случайных досок размером popSize. 
        /// Случайная популяция состоит из расстановки ферзей, которые не бьют друг друга по столбцам и строкам.
        /// </summary>
        public static void CreatePopulation()
        {
            int[] initParent = new int[NumQueens];
            Random r = new();
            // Заполнение популяции случайными первоначальными родителями.
            for (var i = 0; i < popSize; i++)
            {
                for (var j = 0; j < initParent.Length; j++)
                {
                    initParent[j] = r.Next(1, NumQueens + 1);
                    while (true)
                    {
                        initParent[j] = r.Next(1, NumQueens + 1);
                        int k ;
                        // Проверка уникальности(без столкновений для строк ).
                        for(k = 0; k <= j; k++)
                        {
                            if (initParent[k]== initParent[j])
                            {
                                //Если столкновение => выходим из цикла.
                                break;
                            }
                        }
                        // Если столкновений нет => выходим из цикла.
                        if (k == j)
                        {
                            break;
                        }
                    }
                }
                population[i] = new Boards(initParent);
            }
        }

        /// <summary>
        ///  Выбирает случайным образом родителя из совокупности по уровню пригодности.
        /// </summary>
        /// <returns>Родитель</returns>
        public static Boards ChooseParent()
        {
            Random r = new();
            // Общая пригодность.
            var total = 0;
            // Получаем текущую общую пригодность.
            for (var i = 0; i < population.Length; i++)
            {
                total += population[i].Fitness;
            }
            int random = r.Next(0, total);
            // Выбираем случайного родителя с более высоким уровнем пригодности.
            for (var i = 0; i < population.Length; i++)
            {
                if (random < population[i].Fitness)
                {
                    return population[i];
                }
                random -= population[i].Fitness;
            }
            return null;
        }

        /// <summary>
        /// Выполняет генетический алгоритм.
        /// </summary>
        /// <returns>Дочерний элемент, содержащий допустимое решение.</returns>
        public static Boards GeneticAlg()
        {
            //Дочерний элемент, содержащий допустимое решение.
            Boards child;
            Random r = new();
            // Временная популяция размерностью popSize.
            Boards[] tempPopulation = new Boards[popSize];
            // Лучшее значение функции пригодности. Нужно для проверки пригодности потомка.
            var highestFitness = 0;
            // Подсчёт поколений.
            var generation = 0;
            // Создаём начальную популяцию.
            CreatePopulation();
            while (true)
            {
                //Счётчик поколений .
                generation++;
                // Создаём новые поколения особей, пока не будет найдено решение.
                for (var i = 0; i < popSize; i++)
                {
                    // CВыбираем двух родителей и создаём ребенка. Пока выбор любой.
                    child = Crossover(ChooseParent(), ChooseParent());
                    // Проверяем, является ли child решением.
                    if (child.Solved())
                    {
                        Console.WriteLine("Функция пригодности: " + child.Fitness + " Поколение: " + generation);
                        return child;
                    }
                    // Изменяем мутацию.
                    if (mutation > r.Next(0, 100))
                    {
                        child.Mutate();
                    }
                    // Проверяем пригодность ребенка.
                    if (child.Fitness > highestFitness)
                    {
                        highestFitness = child.Fitness ;
                    }
                    tempPopulation[i] = child;
                }
                population = tempPopulation;
            }
        }
    }
}

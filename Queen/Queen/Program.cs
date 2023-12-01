using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueensGenetic
{
    class Board
    {
        private int[] board;
        private int fitness;
        private int size;

        /// <summary>
        /// Конструктор досок.
        /// </summary>
        /// <param name="board">Represents a board</param>
        public Board(int[] board)
        {
            this.board = board;
            fitness = calculateFitness();
            size = board.Length;
        }

        /// <summary>
        /// Определяет пригодность доски.
        /// </summary>
        /// <returns>Пригодность доски</returns>
        public int calculateFitness()
        {
            int fitness = 0;
            bool result;

            /* Проверяем каждого ферзя на доске,
             * и если он не атакует другого ферзя, 
             * увеличиваем пригодность доски на единицу.
            */
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 1; j < board.Length; j++)
                {
                    result = attacking(board[i], i, board[j], j);

                    if (result == false)
                    {
                        fitness++;
                    }
                }
            }

            return fitness;
        }

        /// <summary>
        /// solved - Determines if a board is solved
        /// </summary>
        /// <returns></returns>
        public bool solved()
        {
            bool result;

            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 1; j < board.Length; j++)
                {
                    result = attacking(board[i], i, board[j], j);

                    if (result == true)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Проверяет, атакуют ли две королевы
        /// </summary>
        /// <param name="queenA">Первая королева для проверки.</param>
        /// <param name="colA">Столбец первой королевы</param>
        /// <param name="queenB">Вторая королева для проверки.</param>
        /// <param name="colB">Столбец второй королевы.</param>
        /// <returns></returns>
        public Boolean attacking(int queenA, int colA, int queenB, int colB)
        {
            //Если королеы в одном столбце
            if (colA == colB)
            {
                return false;
            }

            // Проверяем атаки диагоналей 
            if (Math.Abs(queenB - queenA) == Math.Abs(colB - colA))
            {
                return true;
            }
            else if (queenA == queenB) // Проверяем строки
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Выполняет мутацию на одной королеве.
        /// </summary>
        public void mutate()
        {
            Random r = new Random();

            int randomPosition = r.Next(0, size);
            int randomQueenValue = r.Next(1, size + 1);

            board[randomPosition] = randomQueenValue;
        }

        /// <summary>
        /// Возвращает доску в виде массива
        /// </summary>
        /// <returns>доска</returns>
        public int[] getBoard()
        {
            return (int[])board.Clone();
        }

        /// <summary>
        /// getFitness -Возвращает пригодность доски.
        /// </summary>
        /// <returns>Пригодность доски.</returns>
        public int getFitness()
        {
            return fitness;
        }

        /// <summary>
        /// Возвращает строковое представление доски.
        /// </summary>
        /// <returns>Строковое представление доски.</returns>
        public String toString()
        {
            String str = String.Empty;

            for (int i = 0; i < board.Length; i++)
            {
                str += board[i] + " ";
            }

            return str;
        }

        /// <summary>
        /// Печать решения.
        /// </summary>
        public void printBoard()
        {
            string str = String.Empty;

            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board.Length; j++)
                {
                    if (board[j] == i + 1)
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
        const int MUTATION = 10; // VALUE IS (X/100)
        // Массив, содержащий особи (доски) в популяции.
        static Board[] POPULATION = new Board[POP_SIZE];

        public static void Main()
        {
            //Вызываем метод geneticAlg() для выполнения генетического алгоритма.
            Board solution = geneticAlg();
            //Выводит решение на экран.
            Console.Out.WriteLine(solution.toString() + "\n");
            solution.printBoard();
            Console.ReadLine();
        }

        /// <summary>
        /// Реализует одноточечное скрещивание двух родителей (parentX и parentY), чтобы создать потомка.
        /// </summary>
        /// <param name="parentX">Первый родитель</param>
        /// <param name="parentY">Второй родитель</param>
        /// <returns>Ребенок в результате скрещивания.</returns>
        public static Board crossover(Board parentX, Board parentY)
        {
            Board child;
            Random r = new Random();
            //Выбираем случайную точку пересечения и создаём потомка, комбинируя части родительских досок.
            int crossoverPoint = r.Next(1, NUM_QUEENS - 1);

            //Получаем обе части массива от родителей, затем создаём один дочерний элемент
            int[] firstHalf = parentX.getBoard().Take(crossoverPoint).ToArray();
            int[] secondHalf = parentY.getBoard().Skip(crossoverPoint).Take(NUM_QUEENS - crossoverPoint).ToArray();
            int[] childArray = firstHalf.Concat(secondHalf).ToArray();

            child = new Board(childArray);

            return child;

        }

        /// <summary>
        /// Создает начальную популяцию случайных досок размером POP_SIZE.
        /// </summary>
        public static void createPopulation()
        {
            int[] initParent = new int[NUM_QUEENS];
            Random r = new Random();

            // Заполнение популяции случайными первоначальными родителями.
            for (int i = 0; i < POP_SIZE; i++)
            {
                for (int j = 0; j < initParent.Length; j++)
                {
                    initParent[j] = r.Next(1, NUM_QUEENS + 1);
                }

                POPULATION[i] = new Board((int[])initParent.Clone());
            }

        }

        /// <summary>
        ///  Выбирает случайным образом родителя из совокупности по уровню пригодности.
        /// </summary>
        /// <returns>Родитель</returns>
        public static Board chooseParent()
        {
            Random r = new Random();
            int total = 0;

            // Получаем текущую общую пригодность
            for (int i = 0; i < POPULATION.Length; i++)
            {
                total += POPULATION[i].getFitness();
            }

            int random = r.Next(0, total);

            // Выбираем случайного родителя с более высоким уровнем пригодности
            for (int i = 0; i < POPULATION.Length; i++)
            {
                if (random < POPULATION[i].getFitness())
                {
                    return POPULATION[i];
                }

                random = random - POPULATION[i].getFitness();
            }

            return null;
        }

        /// <summary>
        /// Выполняет генетический алгоритм
        /// </summary>
        /// <returns>Child which contains a valid solution</returns>
        public static Board geneticAlg()
        {
            Board child;
            Random r = new Random();
            Board[] tempPopulation = new Board[POP_SIZE];

            int highestFitness = 0;
            int generation = 0;

            createPopulation();

            while (true)
            {
                generation++;

                // Создаём новые поколения особей, пока не будет найдено решение.
                for (int i = 0; i < POP_SIZE; i++)
                {
                    // CВыбираем двух родителей и создаём ребенка.
                    child = crossover(chooseParent(), chooseParent());

                    // Проверяем, является ли child решением.
                    if (child.solved())
                    {
                        Console.Out.WriteLine("Fitness: " + child.getFitness() + " Generation: " + generation);
                        return child;
                    }

                    // Изменяем мутацию.
                    if (MUTATION > r.Next(0, 100))
                    {
                        child.mutate();
                    }

                    // Проверяем пригодность ребенка 
                    if (child.getFitness() > highestFitness)
                    {
                        highestFitness = child.getFitness();
                    }

                    tempPopulation[i] = child;
                }

                POPULATION = tempPopulation;
                Console.Out.WriteLine("Fitness: " + highestFitness + " Generation: " + generation);
            }
        }
    }
}

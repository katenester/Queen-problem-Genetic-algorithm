using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NQueensGA
{
    class Board
    {
        private int[] board;
        private int fitness;
        private int size;

        /// <summary>
        /// Board Constructor
        /// </summary>
        /// <param name="board">Represents a board</param>
        public Board(int[] board)
        {
            this.board = board;
            fitness = calculateFitness();
            size = board.Length;
        }

        /// <summary>
        /// calculateFitness - Determines the fitness of a board.
        /// </summary>
        /// <returns>Fitness of board</returns>
        public int calculateFitness()
        {
            int fitness = 0;
            bool result;

            /* Check each queen on the board, and if it is not
               attacking another queen, increase the fitness of
               the board by one.
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
        /// attacking - Checks if two queens are attacking
        /// </summary>
        /// <param name="queenA">First queen to check</param>
        /// <param name="colA">Column of first queen</param>
        /// <param name="queenB">Second queen to check</param>
        /// <param name="colB">Column of second queen</param>
        /// <returns></returns>
        public Boolean attacking(int queenA, int colA, int queenB, int colB)
        {
            // If queen is checking itself
            if (colA == colB)
            {
                return false;
            }

            // Check diagonal attacks
            if (Math.Abs(queenB - queenA) == Math.Abs(colB - colA))
            {
                return true;
            }
            else if (queenA == queenB) // Check Row
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// mutate - Performs a mutation on a single queen
        /// </summary>
        public void mutate()
        {
            Random r = new Random();

            int randomPosition = r.Next(0, size);
            int randomQueenValue = r.Next(1, size + 1);

            board[randomPosition] = randomQueenValue;
        }

        /// <summary>
        /// getBoard - Returns the board as an array
        /// </summary>
        /// <returns>board</returns>
        public int[] getBoard()
        {
            return (int[])board.Clone();
        }

        /// <summary>
        /// getFitness - Returns the fitness of board
        /// </summary>
        /// <returns>Fitness of board</returns>
        public int getFitness()
        {
            return fitness;
        }

        /// <summary>
        /// toString - Returns string representation of board
        /// </summary>
        /// <returns>String representing board</returns>
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
        /// printBoard - Prints a visual representation of board
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
        const int NUM_QUEENS = 8;
        const int POP_SIZE = 500;
        const int MUTATION = 10; // VALUE IS (X/100)
        static Board[] POPULATION = new Board[POP_SIZE];

        public static void Main()
        {
            Board solution = geneticAlg();

            Console.Out.WriteLine(solution.toString() + "\n");
            solution.printBoard();
            Console.ReadLine();
        }

        /// <summary>
        /// crossover - Performs a random single crossover two parents
        /// </summary>
        /// <param name="parentX">First Parent</param>
        /// <param name="parentY">Second Parent</param>
        /// <returns>Child as a result of a crossover</returns>
        public static Board crossover(Board parentX, Board parentY)
        {
            Board child;
            Random r = new Random();

            int crossoverPoint = r.Next(1, NUM_QUEENS - 1);

            // Obtain both parts of the array from the parents, then produce a single child
            int[] firstHalf = parentX.getBoard().Take(crossoverPoint).ToArray();
            int[] secondHalf = parentY.getBoard().Skip(crossoverPoint).Take(NUM_QUEENS - crossoverPoint).ToArray();
            int[] childArray = firstHalf.Concat(secondHalf).ToArray();

            child = new Board(childArray);

            return child;

        }

        /// <summary>
        /// createPopulation - Creates the initial population
        /// </summary>
        public static void createPopulation()
        {
            int[] initParent = new int[NUM_QUEENS];
            Random r = new Random();

            // Populating the population with random initial parents
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
        /// chooseParent - Randomly chooses a weighted parent from the
        ///                population, by level of fitness.
        /// </summary>
        /// <returns>Parent</returns>
        public static Board chooseParent()
        {
            Random r = new Random();
            int total = 0;

            // Get current total fitness
            for (int i = 0; i < POPULATION.Length; i++)
            {
                total += POPULATION[i].getFitness();
            }

            int random = r.Next(0, total);

            // Choose random parent, higher fitness has higher chance
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
        /// geneticAlg - Performs the genetic algorithm 
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

                // Begin creation of new generation
                for (int i = 0; i < POP_SIZE; i++)
                {
                    // Choose two parents and create a child
                    child = crossover(chooseParent(), chooseParent());

                    // Check to see if child is a solution
                    if (child.solved())
                    {
                        Console.Out.WriteLine("Fitness: " + child.getFitness() + " Generation: " + generation);
                        return child;
                    }

                    // Mutation change
                    if (MUTATION > r.Next(0, 100))
                    {
                        child.mutate();
                    }

                    // Check childs fitness
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

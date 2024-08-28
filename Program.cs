using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Channels;
using System.Xml;

namespace LifeGame
{
    internal class Program
    {

        static void Main(string[] args)
        {
            int gridSizeX = 30;
            int gridSizeY = 6;
            int nieghbors = 0;

            int dead = 0;
            int alive = 1;
            int generations;
            int succesfulGenerations = 0;

            int runs = 0;
            int deadCells = 0;
            int luckyCells = 0;


            bool debug = false;
            bool valid = false;
            bool pacman = true;


        Prompt:


            string response;
            int[,] grid = new int[gridSizeY, gridSizeX];
            int[,] tempGrid = new int[gridSizeY, gridSizeX];
            int timer = 350;

            if (gridSizeX * gridSizeY > 500) { timer = 100; }
            if (gridSizeX * gridSizeY > 1000) { timer = 0; }

            try { grid[1, 2] = alive; } catch { }
            try { grid[2, 3] = alive; } catch { }
            try { grid[3, 1] = alive; } catch { }// Glider
            try { grid[3, 2] = alive; } catch { };
            try { grid[3, 3] = alive; } catch { };


            while (true)   // Main Loop
            {

                runs = 0;
                Console.Clear();
                Console.WriteLine("Conways Game of Life");


                if (pacman == true) { Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine("Pac-Man Mode Enabled\n"); Console.ResetColor(); }
                if (debug) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("DEBUG MODE"); Console.ResetColor(); }

                Console.Write($"Generation: {succesfulGenerations}      "); Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Lucky Cells: {luckyCells}\n"); Console.ResetColor();
                luckyCells += DisplayGrid(grid, debug);


                Console.Write("\n[GridSize] [Pac-man Mode] [RandomizeGrid] [Add Cell] [Kill Cell] [Debug] [Off] [Exit] \nEnter Number of generations: ");
                response = Console.ReadLine();
                response = response.ToLower();

                if (response == "") { goto Prompt; }
                if (response[0] == 'd') { debug = true; Console.Clear(); goto Prompt; }
                if (response[0] == 'o') { debug = false; pacman = false; Console.Clear(); goto Prompt; }
                if (response[0] == 'p') { debug = false; pacman = true; Console.Clear(); Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine("Wrap Field Mode Enabled"); Console.ResetColor(); goto Prompt; }
                if (response[0] == 'a') { grid = AddCell(grid); Console.Clear(); continue; }
                if (response[0] == 'r') { grid = RandomizeGrid(grid); Console.Clear(); continue; }
                if (response[0] == 'k') { grid = KillCell(grid); Console.Clear(); continue; }
                if (response[0] == 'e') { Environment.Exit(0); }

                if (response[0] == 'g')
                {

                    while (!valid)
                    {
                        Console.Write("Enter new grid size X:");
                        response = Console.ReadLine();
                        try { gridSizeX = Convert.ToInt32(response); } catch { Console.ForegroundColor = ConsoleColor.Red; Console.Clear(); Console.WriteLine("Insert integer!!"); Console.ResetColor(); continue; } finally { valid = true; }
                        Console.Write("\nEnter new grid size Y:");
                        response = Console.ReadLine();
                        try { gridSizeY = Convert.ToInt32(response); } catch { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Insert integer!!"); Console.ResetColor(); } finally { valid = true; Console.Clear(); }

                    }
                    valid = false;
                    goto Prompt;

                }

                try { generations = Convert.ToInt32(response); } catch { goto Prompt; } finally { Console.Clear(); }



                while (runs < generations)  // Generation loop
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Generation: {runs} ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Lucky Cells: {luckyCells} ");
                    Console.ResetColor();
                    deadCells = 0;
                    luckyCells += DisplayGrid(grid, debug);

                    for (int i = 0; i < grid.GetLength(0); i++)                // i = rows
                    {                                                          // j = columns
                        for (int j = 0; j < grid.GetLength(1); j++)
                        {
                            nieghbors = 0;

                            if (pacman) { nieghbors = PacmanMode(grid, i, j); }
                            else { nieghbors = CountNieghbors(grid, i, j); }


                            if (debug == true)
                            {
                                if (j == 0)
                                {
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine($"=========================== Printing Row:{i}  =============================================");
                                    Console.ResetColor();
                                    DisplayGridDebug(grid, i);

                                }

                                Console.WriteLine($"Cell X:{j} Y:{i} has nieghbors: {nieghbors}");
                            }


                            if (grid[i, j] == 0)
                            {
                                if (nieghbors == 0 || nieghbors == 1 || nieghbors == 2)
                                {
                                    tempGrid[i, j] = dead;
                                }
                                else if (nieghbors == 3)
                                {
                                    tempGrid[i, j] = alive;
                                }
                                else if (nieghbors >= 4)
                                {
                                    tempGrid[i, j] = dead;
                                }
                                deadCells++;
                            }

                            else if (grid[i, j] == 1)
                            {
                                if (nieghbors == 2 || nieghbors == 3)
                                {
                                    tempGrid[i, j] = alive;
                                }
                                else
                                {
                                    tempGrid[i, j] = dead;
                                }
                            }
                            if (debug == true)
                            {

                                Console.WriteLine($"Tempgrid X={j} Y={i} is {tempGrid[i, j]}\n");
                            }

                        } // close for loop j

                    } // close for loop i


                    if (debug) { Console.WriteLine("Displaying Temp grid"); DisplayGrid(tempGrid, debug); }

                    Array.Copy(tempGrid, grid, tempGrid.Length);
                    Array.Clear(tempGrid, 0, tempGrid.Length);
                    Thread.Sleep(timer);


                    if (debug) { Console.ReadLine(); }

                    if (deadCells == (gridSizeX * gridSizeY))
                    {
                        Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"All Cells are Dead, They Survived {succesfulGenerations} generations\nPress enter to continue"); Console.ResetColor(); Console.ReadLine(); Console.Clear();
                        runs = generations;
                        succesfulGenerations = 0;
                    }
                    else
                    {
                        Console.Clear();
                        succesfulGenerations++;
                        runs++;
                    }
                    if (runs == generations)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Successfully comleted {succesfulGenerations} generations");
                        if (luckyCells > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Lucky Cells: {luckyCells}");
                        }
                        Console.ResetColor();
                    }

                }
            }
        }

        static int DisplayGrid(int[,] grid, bool debug)
        {

            int count = 0;

            if (!debug)
            {

                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    for (int j = 0; j < grid.GetLength(1); j++)
                    {
                        if (grid[i, j] == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkBlue; Console.Write($"▒▒"); Console.ResetColor();
                        }
                        if ((grid[i, j] == 1))
                        {
                            //Random random = new Random();
                            //ConsoleColor[] colors = (ConsoleColor[])Enum.GetValues(typeof(ConsoleColor));
                            //ConsoleColor randomColor = colors[random.Next(colors.Length)];
                            //Console.ForegroundColor = randomColor; Console.Write("██"); Console.ResetColor();
                            Random random = new Random();
                            int randomInt = random.Next(1, 7);   //  Rolls Die
                            if (randomInt > 5)
                            {
                                randomInt = random.Next(1, 7);
                                if (randomInt > 5)
                                {
                                    count++;
                                    Console.ForegroundColor = ConsoleColor.Green; Console.Write("██"); Console.ResetColor();



                                }
                                else { Console.ForegroundColor = ConsoleColor.Magenta; Console.Write("██"); Console.ResetColor(); }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Magenta; Console.Write("██"); Console.ResetColor();
                            }

                        }

                    }
                    Console.WriteLine();
                }

            }

            if (debug)
            {
                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    for (int j = 0; j < grid.GetLength(1); j++)
                    {
                        if (grid[i, j] == 0)
                        {
                            Console.Write($"0 ");
                        }
                        if ((grid[i, j] == 1))
                        {
                            Console.Write($"x ");
                        }

                    }
                    Console.WriteLine();
                }

            }

            return count;
        }

        static void DisplayGridDebug(int[,] grid, int line)
        {

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {

                    if (i == line) { Console.ForegroundColor = ConsoleColor.Yellow; }



                    if (grid[i, j] == 0)
                    {
                        Console.Write($"0 ");
                    }
                    if ((grid[i, j] == 1))
                    {
                        Console.ForegroundColor = ConsoleColor.Red; Console.Write($"x "); Console.ResetColor();
                    }
                    Console.ResetColor();

                }
                Console.WriteLine();
            }
        }



        static int[,] AddCell(int[,] grid)
        {
            string response;
            int userCoordX = 0; int userCoordY = 0;
            Console.Write("Enter cell X Coord: ");
            response = Console.ReadLine(); try { userCoordX = Convert.ToInt32(response); } catch { Console.WriteLine("Must Be Integer!"); AddCell(grid); }

            Console.Write("Enter cell Y Coord: ");
            response = Console.ReadLine(); try { userCoordY = Convert.ToInt32(response); } catch { Console.WriteLine("Must Be Integer!"); AddCell(grid); } finally { grid[userCoordY, userCoordX] = 1; }

            return grid;

        }

        static int[,] KillCell(int[,] grid)
        {
            string response;
            int userCoordX = 0; int userCoordY = 0;
            Console.Write("Enter cell X Coord: ");
            response = Console.ReadLine(); try { userCoordX = Convert.ToInt32(response); } catch { Console.WriteLine("Must Be Integer!"); KillCell(grid); }

            Console.Write("Enter cell Y Coord: ");
            response = Console.ReadLine(); try { userCoordY = Convert.ToInt32(response); } catch { Console.WriteLine("Must Be Integer!"); KillCell(grid); } finally { grid[userCoordY, userCoordX] = 0; }

            return grid;

        }


        static int[,] RandomizeGrid(int[,] grid)
        {
            Random random = new Random();
            int randomInt = random.Next(1, 7);   //  Rolls Die

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (randomInt >= 5)
                    {
                        grid[i, j] = 1;
                    }
                    else
                    {
                        grid[i, j] = 0;
                    }

                    randomInt = random.Next(1, 7);

                }
            }
            return grid;

        }
        // End RandomGrid 

        // Start CountNieghbors

        static int CountNieghbors(int[,] grid, int i, int j)
        {
            int nieghbors = 0;
            int x = grid.GetLength(1) - 1;    //  -1 for array offset
            int y = grid.GetLength(0) - 1;


            if (i == 0 && j == 0)          // If grid position is  Row 0, Column 0   ( Cell is topLeft of map ╔╔╔)
            {
                //if (grid[i - 1, j - 1] == 1)   { nieghbors++; }  // topleft          ╔╔╔
                //if (grid[i - 1, j - 0] == 1)   { nieghbors++; }  // above           ^^^^
                //if (grid[i - 1, j + 1] == 1)   { nieghbors++; }  // topright         ╗╗╗
                //if (grid[i - 0, j - 1] == 1)   { nieghbors++; }  // left            <---
                //if (grid[i - 0, j + 0] == 1)   { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 0, j + 1] == 1) { nieghbors++; }  // right           --->
                                                               //if (grid[i + 1, j - 1] == 1)   { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[i + 1, j + 0] == 1) { nieghbors++; }  // below            vvvv
                if (grid[i + 1, j + 1] == 1) { nieghbors++; }  // bottom right      ╝╝╝
            }



            if (j < x && j != 0 && i == 0) // If grid position is on Row 0 Column  X-1   ( Cell is Top of map ^^^)

            {
                //if (grid[i - 1, j - 1] == 1)   { nieghbors++; }  // topleft          ╔╔╔
                //if (grid[i - 1, j - 0] == 1)   { nieghbors++; }  // above           ^^^^
                //if (grid[i - 1, j + 1] == 1)   { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, j - 1] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)   { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 0, j + 1] == 1) { nieghbors++; }  // right           --->
                if (grid[i + 1, j - 1] == 1) { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[i + 1, j + 0] == 1) { nieghbors++; }  // below            vvvv
                if (grid[i + 1, j + 1] == 1) { nieghbors++; }  // bottom right      ╝╝╝
            }


            if (j == x && i == 0)  // If grid position is on Row 0 Column X   ( Cell is topRight of map ╗╗╗)

            {
                //Console.WriteLine($"i:{i} j:{j} x:{x} y:{y}");
                //DisplayGrid(grid);
                //if (grid[i - 1, j - 1] == 1)   { nieghbors++; }  // topleft          ╔╔╔
                //if (grid[i - 0, j - 1] == 1)   { nieghbors++; }  // above           ^^^^
                //if (grid[i + 1, j - 1] == 1)   { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, j - 1] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)   { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                                                               //if (grid[i + 1, j - 0] == 1)   { nieghbors++; }  // right           --->
                if (grid[i + 1, j - 1] == 1) { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[i + 1, j + 0] == 1) { nieghbors++; }  // below            vvvv
                                                               //if (grid[i + 1, j + 1] == 1)   { nieghbors++; }  // bottom right      ╝╝╝ 

            }

            if (j == 0 && i != 0 && i < y)  // If grid position is on Row Y-1 Column 0   ( Cell is left Side of map <----)

            {
                //if (grid[i - 1, j - 1] == 1)   { nieghbors++; }  // topleft          ╔╔╔
                if (grid[i - 1, j - 0] == 1) { nieghbors++; }  // above           ^^^^
                if (grid[i - 1, j + 1] == 1) { nieghbors++; }  // topright         ╗╗╗
                                                               //if (grid[i - 0, j - 1] == 1)   { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)   { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 0, j + 1] == 1) { nieghbors++; }  // right           --->
                                                               //if (grid[i + 1, j - 1] == 1)   { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[i + 1, j + 0] == 1) { nieghbors++; }  // below            vvvv
                if (grid[i + 1, j + 1] == 1) { nieghbors++; }  // bottom right      ╝╝╝
            }

            if (j != 0 && j < x && i != 0 && i < y)  // If grid position is on Row Y-1 Column X-1   ( Cell is somewhere in the middle of the map)

            {
                if (grid[i - 1, j - 1] == 1) { nieghbors++; }  // topleft          ╔╔╔
                if (grid[i - 1, j - 0] == 1) { nieghbors++; }  // above           ^^^^
                if (grid[i - 1, j + 1] == 1) { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, j - 1] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1) { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 0, j + 1] == 1) { nieghbors++; }  // right           --->
                if (grid[i + 1, j - 1] == 1) { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[i + 1, j + 0] == 1) { nieghbors++; }  // below            vvvv
                if (grid[i + 1, j + 1] == 1) { nieghbors++; }  // bottom right      ╝╝╝
            }

            if (j == x && i != 0 && i < y)  // If grid position is on Row Y-1 Column X   ( Cell is on Right Side of map ---->)

            {
                if (grid[i - 1, j - 1] == 1) { nieghbors++; }  // topleft          ╔╔╔
                if (grid[i - 1, j - 0] == 1) { nieghbors++; }  // above           ^^^^
                                                               //if (grid[i - 1, j + 1] == 1)   { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, j - 1] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)   { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                                                               //if (grid[i + 0, j + 1] == 1)   { nieghbors++; }  // right           --->
                if (grid[i + 1, j - 1] == 1) { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[i + 1, j + 0] == 1) { nieghbors++; }  // below            vvvv
                                                               //if (grid[i + 1, j + 1] == 1)   { nieghbors++; }  // bottom right      ╝╝╝
            }

            if (j == 0 && i == y)  // If grid position is on Row Y Column 0   ( Cell is on Bottom Left of map ╚╚╚)

            {
                //if (grid[i - 1, j - 1] == 1)   { nieghbors++; }  // topleft          ╔╔╔
                if (grid[i - 1, j - 0] == 1) { nieghbors++; }  // above           ^^^^
                if (grid[i - 1, j + 1] == 1) { nieghbors++; }  // topright         ╗╗╗
                                                               //if (grid[i - 0, j - 1] == 1)   { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)   { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 0, j + 1] == 1) { nieghbors++; }  // right           --->
                                                               //if (grid[i + 1, j - 1] == 1)   { nieghbors++; }  // bottom left      ╚╚╚ 
                                                               //if (grid[i + 1, j + 0] == 1)   { nieghbors++; }  // below            vvvv
                                                               //if (grid[i + 1, j + 1] == 1)   { nieghbors++; }  // bottom right      ╝╝╝
            }

            if (j != 0 && j < x && i == y)  // If grid position is on Row Y Column X-1   ( Cell is on Bottom of map vvvv)

            {
                if (grid[i - 1, j - 1] == 1) { nieghbors++; }  // topleft          ╔╔╔
                if (grid[i - 1, j - 0] == 1) { nieghbors++; }  // above           ^^^^
                if (grid[i - 1, j + 1] == 1) { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, j - 1] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)   { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 0, j + 1] == 1) { nieghbors++; }  // right           --->
                                                               //if (grid[i + 1, j - 1] == 1)   { nieghbors++; }  // bottom left      ╚╚╚ 
                                                               //if (grid[i + 1, j + 0] == 1)   { nieghbors++; }  // below            vvvv
                                                               //if (grid[i + 1, j + 1] == 1)   { nieghbors++; }  // bottom right      ╝╝╝
            }

            if (j == x & i == y)  // If grid position is on Row Y Column X   ( Cell is on bottom Right of map ╝╝╝)

            {
                if (grid[i - 1, j - 1] == 1) { nieghbors++; }  // topleft          ╔╔╔
                if (grid[i - 1, j - 0] == 1) { nieghbors++; }  // above           ^^^^
                                                               //if (grid[i - 1, j + 1] == 1)   { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, j - 1] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)   { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                                                               //if (grid[i + 0, j + 1] == 1)   { nieghbors++; }  // right           --->
                                                               //if (grid[i + 1, j - 1] == 1)   { nieghbors++; }  // bottom left      ╚╚╚ 
                                                               //if (grid[i + 1, j + 0] == 1)   { nieghbors++; }  // below            vvvv
                                                               //if (grid[i + 1, j + 1] == 1)   { nieghbors++; }  // bottom right      ╝╝╝
            }

            return nieghbors;
        }


        //   End of CountNieghbors




        //    Begin PacmanMode


        static int PacmanMode(int[,] grid, int i, int j)
        {
            int nieghbors = 0;
            int x = grid.GetLength(1) - 1;    //  -1 for array offset
            int y = grid.GetLength(0) - 1;    // X and Y are max grid size values for <-Rows(Y)-> <-(columns(x)->


            if (i == 0 && j == 0)          // If grid position is  Row 0, Column 0   ( Cell is topLeft of map ╔╔╔)
            {      //       Y     X)
                if (grid[y - 0, x - 0] == 1) { nieghbors++; }  // topleft (actually bottom right)         ╔╔╔
                if (grid[y - 0, j - 0] == 1) { nieghbors++; }  // above           ^^^^
                if (grid[y - 0, j + 1] == 1) { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, x - 0] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)   { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 0, j + 1] == 1) { nieghbors++; }  // right           --->      // 
                if (grid[i + 1, x - 0] == 1) { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[i + 1, j + 0] == 1) { nieghbors++; }  // below            vvvv
                if (grid[i + 1, j + 1] == 1) { nieghbors++; }  // bottom right      ╝╝╝
            }



            if (j < x && j != 0 && i == 0) // If grid position is on Row 0 Column  X-1   ( Cell is Top of map ^^^)

            {      //       Y     X)
                if (grid[y - 0, j - 1] == 1) { nieghbors++; }  // topleft          ╔╔╔
                if (grid[y - 0, j - 0] == 1) { nieghbors++; }  // above           ^^^^
                if (grid[y - 0, j + 1] == 1) { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, j - 1] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)    { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 0, j + 1] == 1) { nieghbors++; }  // right           --->
                if (grid[i + 1, j - 1] == 1) { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[i + 1, j + 0] == 1) { nieghbors++; }  // below            vvvv
                if (grid[i + 1, j + 1] == 1) { nieghbors++; }  // bottom right      ╝╝╝
            }


            if (j == x && i == 0)  // If grid position is on Row 0 Column X   ( Cell is topRight of map ╗╗╗)

            {      //       Y     X)
                if (grid[y - 0, x - 1] == 1) { nieghbors++; }  // topleft          ╔╔╔
                if (grid[y - 0, x - 1] == 1) { nieghbors++; }  // above           ^^^^
                if (grid[y + 0, i - 0] == 1) { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, j - 1] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)   { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 1, j - x] == 1) { nieghbors++; }  // right           --->
                if (grid[i + 1, j - 1] == 1) { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[i + 1, j + 0] == 1) { nieghbors++; }  // below            vvvv
                if (grid[i + 1, j + 0] == 1) { nieghbors++; }  // bottom right      ╝╝╝ 

            }


            if (j == 0 && i != 0 && i < y)  // If grid position is on Row Y-1 Column 0   ( Cell is left Side of map <----)

            {
                if (grid[i - 1, x - 0] == 1) { nieghbors++; }  // topleft          ╔╔╔
                if (grid[i - 1, j - 0] == 1) { nieghbors++; }  // above           ^^^^
                if (grid[i - 1, j + 1] == 1) { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, x - 0] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)   { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 0, j + 1] == 1) { nieghbors++; }  // right           --->
                if (grid[i + 1, x - 0] == 1) { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[i + 1, j + 0] == 1) { nieghbors++; }  // below            vvvv
                if (grid[i + 1, j + 1] == 1) { nieghbors++; }  // bottom right      ╝╝╝
            }

            if (j != 0 && j < x && i != 0 && i < y)  // If grid position is on Row Y-1 Column X-1   ( Cell is somewhere in the middle of the map)

            {
                if (grid[i - 1, j - 1] == 1) { nieghbors++; }  // topleft          ╔╔╔
                if (grid[i - 1, j - 0] == 1) { nieghbors++; }  // above           ^^^^
                if (grid[i - 1, j + 1] == 1) { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, j - 1] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1) { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 0, j + 1] == 1) { nieghbors++; }  // right           --->
                if (grid[i + 1, j - 1] == 1) { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[i + 1, j + 0] == 1) { nieghbors++; }  // below            vvvv
                if (grid[i + 1, j + 1] == 1) { nieghbors++; }  // bottom right      ╝╝╝
            }

            if (j == x && i != 0 && i < y)  // If grid position is on Row Y-1 Column X   ( Cell is on Right Side of map ---->)

            {
                if (grid[i - 1, j - 1] == 1) { nieghbors++; }  // topleft          ╔╔╔
                if (grid[i - 1, j - 0] == 1) { nieghbors++; }  // above           ^^^^
                if (grid[i - 1, 0 + 0] == 1) { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, j - 1] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)   { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 0, 0 + 0] == 1) { nieghbors++; }  // right           --->
                if (grid[i + 1, j - 1] == 1) { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[i + 1, j + 0] == 1) { nieghbors++; }  // below            vvvv
                if (grid[i + 1, 0 + 0] == 1) { nieghbors++; }  // bottom right      ╝╝╝
            }

            if (j == 0 && i == y)  // If grid position is on Row Y Column 0   ( Cell is on Bottom Left of map ╚╚╚)

            {
                if (grid[i - 1, x - 0] == 1) { nieghbors++; }  // topleft          ╔╔╔
                if (grid[i - 1, j - 0] == 1) { nieghbors++; }  // above           ^^^^
                if (grid[i - 1, j + 1] == 1) { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, x - 0] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)   { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 0, j + 1] == 1) { nieghbors++; }  // right           --->
                if (grid[0 + 0, x - 0] == 1) { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[0 + 0, 0 + 0] == 1) { nieghbors++; }  // below            vvvv
                if (grid[0 + 0, j + 1] == 1) { nieghbors++; }  // bottom right      ╝╝╝
            }

            if (j != 0 && j < x && i == y)  // If grid position is on Row Y Column X-1   ( Cell is on Bottom of map vvvv)

            {
                if (grid[i - 1, j - 1] == 1) { nieghbors++; }  // topleft          ╔╔╔
                if (grid[i - 1, j - 0] == 1) { nieghbors++; }  // above           ^^^^
                if (grid[i - 1, j + 1] == 1) { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, j - 1] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)     { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 0, j + 1] == 1) { nieghbors++; }  // right           --->
                if (grid[0 + 0, j - 1] == 1) { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[0 + 0, j + 0] == 1) { nieghbors++; }  // below            vvvv
                if (grid[0 + 0, j + 1] == 1) { nieghbors++; }  // bottom right      ╝╝╝
            }

            if (j == x & i == y)  // If grid position is on Row Y Column X   ( Cell is on bottom Right of map ╝╝╝)

            {
                if (grid[i - 1, j - 1] == 1) { nieghbors++; }  // topleft          ╔╔╔
                if (grid[i - 1, j - 0] == 1) { nieghbors++; }  // above           ^^^^
                if (grid[i - 1, 0 + 0] == 1) { nieghbors++; }  // topright         ╗╗╗
                if (grid[i - 0, j - 1] == 1) { nieghbors++; }  // left            <---
                                                               //if (grid[i - 0, j + 0] == 1)   { nieghbors++; }  // itsself         [  ]    // i = Rows   j = Columns
                if (grid[i + 0, 0 + 0] == 1) { nieghbors++; }  // right           --->
                if (grid[0 + 0, j - 1] == 1) { nieghbors++; }  // bottom left      ╚╚╚ 
                if (grid[0 + 0, j + 0] == 1) { nieghbors++; }  // below            vvvv
                if (grid[0 + 0, 0 + 0] == 1) { nieghbors++; }  // bottom right      ╝╝╝
            }

            return nieghbors;
        }

    }  // End Class Program
}// End NameSpace


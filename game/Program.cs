using System;
using System.Threading;

class DinoRunGame
{
    private const char Dino = '^';    
    private const char EmptySpace = ' '; 

    private static int dinoX = 5;    
    private static int dinoY = 10;   
    private static bool isJumping = false;
    private static bool isDoubleJumping = false;
    private static int jumpHeight = 5;
    private static int currentJumpHeight = 0;
    private static bool gameOver = false;
    private static DateTime lastDoubleJumpTime = DateTime.MinValue;
    private static DateTime doubleJumpStartTime = DateTime.MinValue;
    private static readonly TimeSpan doubleJumpCooldown = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan maxDoubleJumpDuration = TimeSpan.FromSeconds(3);
    private static bool canDoubleJump = true;
    private static Random random = new Random();
    private static List<Obstacle> obstacles = new List<Obstacle>();
    private static int currentScore = 0;
    private static int highScore = 0;

    private class Obstacle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Type { get; set; }
        public bool MovesVertically { get; set; }
        public int VerticalDirection { get; set; } = 1;
        public bool Passed { get; set; } = false;
    }

    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Console.SetWindowSize(80, 20);
        Console.SetBufferSize(80, 20);
        
        while (true)
        {
            InitializeObstacles();
            gameOver = false;
            currentScore = 0;

            while (!gameOver)
            {
                Update();
                Draw();
                Input();
                Thread.Sleep(100);
            }
        }
    }

    static void InitializeObstacles()
    {
        obstacles.Clear();
        int pattern = random.Next(8);
        switch (pattern)
        {
            case 0:
                obstacles.Add(new Obstacle { X = 50, Y = 10, Type = '#' });
                obstacles.Add(new Obstacle { X = 65, Y = 10, Type = '#' });
                obstacles.Add(new Obstacle { X = 80, Y = 8, Type = '|', MovesVertically = true });
                break;
            case 1:
                obstacles.Add(new Obstacle { X = 40, Y = 10, Type = '#' });
                obstacles.Add(new Obstacle { X = 60, Y = 8, Type = '|', MovesVertically = true });
                obstacles.Add(new Obstacle { X = 80, Y = 8, Type = '|', MovesVertically = true });
                break;
            case 2:
                obstacles.Add(new Obstacle { X = 50, Y = 5, Type = '*' });
                obstacles.Add(new Obstacle { X = 65, Y = 10, Type = '#' });
                obstacles.Add(new Obstacle { X = 80, Y = 5, Type = '*' });
                break;
            case 3:
                obstacles.Add(new Obstacle { X = 40, Y = 7, Type = '|', MovesVertically = true });
                obstacles.Add(new Obstacle { X = 60, Y = 9, Type = '|', MovesVertically = true });
                obstacles.Add(new Obstacle { X = 80, Y = 11, Type = '|', MovesVertically = true });
                break;
            case 4:
                obstacles.Add(new Obstacle { X = 50, Y = 5, Type = '*' });
                obstacles.Add(new Obstacle { X = 60, Y = 10, Type = '#' });
                obstacles.Add(new Obstacle { X = 70, Y = 5, Type = '*' });
                obstacles.Add(new Obstacle { X = 80, Y = 10, Type = '#' });
                break;
            case 5:
                obstacles.Add(new Obstacle { X = 40, Y = 5, Type = '=', MovesVertically = true });
                obstacles.Add(new Obstacle { X = 60, Y = 5, Type = '=', MovesVertically = true });
                obstacles.Add(new Obstacle { X = 80, Y = 5, Type = '=', MovesVertically = true });
                break;
            case 6:
                obstacles.Add(new Obstacle { X = 45, Y = 5, Type = '*' });
                obstacles.Add(new Obstacle { X = 55, Y = 7, Type = '|', MovesVertically = true });
                obstacles.Add(new Obstacle { X = 75, Y = 8, Type = '|', MovesVertically = true });
                break;
            case 7:
                for (int i = 0; i < 5; i++)
                {
                    obstacles.Add(new Obstacle 
                    { 
                        X = 40 + (i * 20), 
                        Y = 7, 
                        Type = '~', 
                        MovesVertically = true,
                        VerticalDirection = i % 2 == 0 ? 1 : -1
                    });
                }
                break;
        }
    }



    static void Update()
    {
        if (isDoubleJumping)
        {
            if ((DateTime.Now - doubleJumpStartTime) >= maxDoubleJumpDuration)
            {
                isDoubleJumping = false;
                isJumping = false;
            }
        }
        
        if (isJumping || isDoubleJumping)
        {
            if (currentJumpHeight < jumpHeight)
            {
                currentJumpHeight++;
                dinoY--;
            }
            else
            {
                if (!isDoubleJumping)
                {
                    isJumping = false;
                }
            }
        }
        else if (currentJumpHeight > 0)
        {
            currentJumpHeight--;
            dinoY++;
        }

        canDoubleJump = (DateTime.Now - lastDoubleJumpTime) >= doubleJumpCooldown;

        foreach (var obstacle in obstacles)
        {
            obstacle.X--;
            
            if (obstacle.MovesVertically)
            {
                obstacle.Y += obstacle.VerticalDirection;
                if (obstacle.Y <= 5 || obstacle.Y >= 15)
                {
                    obstacle.VerticalDirection *= -1;
                }
            }

            if (obstacle.X == dinoX - 1 && !obstacle.Passed)
            {
                obstacle.Passed = true;
                currentScore++;
                if (currentScore > highScore)
                {
                    highScore = currentScore;
                }
            }

            if (obstacle.X < 0)
            {
                obstacle.X = 79;
                obstacle.Passed = false;
                if (obstacle.MovesVertically)
                {
                    obstacle.Y = random.Next(5, 15);
                }
            }

            if (dinoX == obstacle.X && dinoY == obstacle.Y)
            {
                gameOver = true;
                ShowEndGameMenu();
                return;
            }
        }
    }

    static void Draw()
    {
        Console.Clear();

        Console.SetCursorPosition(60, 0);
        Console.Write($"Score: {currentScore} | High Score: {highScore}");

        for (int y = 0; y < 20; y++)
        {
            for (int x = 0; x < 80; x++)
            {
                if (x == dinoX && y == dinoY)
                {
                    Console.Write(Dino);
                }
                else
                {
                    var obstacle = obstacles.FirstOrDefault(o => o.X == x && o.Y == y);
                    if (obstacle != null)
                    {
                        Console.Write(obstacle.Type);
                    }
                    else
                    {
                        Console.Write(EmptySpace);
                    }
                }
            }
            Console.WriteLine();
        }
    }

    static void Input()
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(intercept: true).Key;

            if (key == ConsoleKey.Spacebar)
            {
                if (!isJumping)
                {
                    isJumping = true;
                }
                else if (canDoubleJump && !isDoubleJumping)
                {
                    isDoubleJumping = true;
                    doubleJumpStartTime = DateTime.Now;
                    lastDoubleJumpTime = DateTime.Now;
                }
            }
        }
    }

    static void ShowEndGameMenu()
    {
        Console.Clear();
        Console.SetCursorPosition(30, 8);
        Console.WriteLine("Game Over!");
        Console.SetCursorPosition(25, 9);
        Console.WriteLine($"Final Score: {currentScore}");
        Console.SetCursorPosition(25, 10);
        Console.WriteLine($"High Score: {highScore}");
        Console.SetCursorPosition(25, 12);
        Console.WriteLine("Press 'R' to restart");
        Console.SetCursorPosition(25, 13);
        Console.WriteLine("Press 'Q' to quit");
        
        while (true)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.R)
            {
                ResetGame();
                return;
            }
            else if (key == ConsoleKey.Q)
            {
                Environment.Exit(0);
            }
        }
    }

    static void ResetGame()
    {
        dinoX = 5;
        dinoY = 10;
        isJumping = false;
        isDoubleJumping = false;
        currentJumpHeight = 0;
        lastDoubleJumpTime = DateTime.MinValue;
        doubleJumpStartTime = DateTime.MinValue;
        canDoubleJump = true;
        
    }
}

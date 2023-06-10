using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CGO_ThucHanh1
{
    internal class SnakeGame
    {
        #region parameter
        public Random rand = new Random();
        public ConsoleKeyInfo keypress = new ConsoleKeyInfo();
        int score, headX, headY, fruitX, fruitY, nTail;
        int[] TailX = new int[100];
        int[] TailY = new int[100];
        const int height = 20;
        const int width = 60;
        const int panel = 10;
        bool gameOver, reset, isprinted, horizontal, vertical;
        string dir, pre_dir;
        #endregion

        void ShowBanner()
        {
            Console.SetWindowSize(width, height + panel);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.CursorVisible = false; //tat con tro chuot
            Console.WriteLine("===SNAKE GAME===");
            Console.WriteLine("Press any key to play");
            Console.WriteLine("Tips: - Press P key to PAUSE game");
            Console.WriteLine("      - Press R key to RESET game");
            Console.WriteLine("      - Press Q key to QUIT game");

            keypress = Console.ReadKey(true);
            if (keypress.Key == ConsoleKey.Q) Environment.Exit(0);
        }
        void Setup()
        {
            dir = "RIGHT"; pre_dir = "";    //buoc di dau tien qua phai
            score = 0; nTail = 0;
            gameOver = reset = isprinted = false;
            headX = width / 2; headY = height / 2;  //ko vuot qua width, height (vi tri bat dau)
            randomQua();
        }
        void randomQua()
        {
            fruitX = rand.Next(1, width - 1);
            fruitY = rand.Next(1, height - 1);
        }
        void Update()
        {
            while (!gameOver)
            {
                CheckInput();
                Logic();
                Render();
                if (reset) break;
                //Dung man hinh 30s
                Thread.Sleep(30);
            }
            if (gameOver) Lose();
        }
        void CheckInput()
        {
            while (Console.KeyAvailable)
            {
                //Cho bam phim bat ky
                keypress = Console.ReadKey(true);
                //dir -> pre_dir
                pre_dir = dir; //luu lai huong di truoc do
                switch (keypress.Key)
                {
                    case ConsoleKey.Q: Environment.Exit(0); break;
                    case ConsoleKey.P: dir = "STOP"; break;
                    case ConsoleKey.LeftArrow: dir = "LEFT"; break;
                    case ConsoleKey.RightArrow: dir = "RIGHT"; break;
                    case ConsoleKey.UpArrow: dir = "UP"; break;
                    case ConsoleKey.DownArrow: dir = "DOWN"; break;
                }
            }
        }
        void Logic()
        {
            int preX = TailX[0], preY = TailY[0];
            int tempX, tempY;
            //0 1 2 3 4 => Con ran an them qua  //x 0 1 2 3 4
            if (dir != "STOP")
            {
                TailX[0] = headX; TailY[0] = headY;
                for (int i = 1; i < nTail; i++) 
                {
                    tempX = TailX[i]; tempY = TailY[i];
                    TailX[i] = preX; TailY[i] = preY;
                    preX = tempX; preY = tempY;
                }
            }
            switch (dir)
            {
                case "RIGHT": headX++; break;
                case "LEFT": headX--; break;
                case "DOWN": headY++; break;
                case "UP": headY--; break;
                case "STOP":
                    {
                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine("GAME PAUSE!");
                            Console.WriteLine("- Press P key to PAUSE game");
                            Console.WriteLine("- Press R key to RESET game");
                            Console.WriteLine("- Press Q key to QUIT game");

                            keypress = Console.ReadKey(true);
                            if (keypress.Key == ConsoleKey.Q) Environment.Exit(0);
                            if (keypress.Key == ConsoleKey.R)
                            {
                                reset = true;
                                break; //choi lai game
                            }
                            if (keypress.Key == ConsoleKey.P) break;    //choi tiep game
                        }
                    }
                    dir = pre_dir;  break;
            
            }

            if (headX <= 0 || headX >= width - 1 || headY <= 0 || headY >= height - 1) gameOver = true;
            else gameOver = false;
            //kiem tra an qua
            if (headX == fruitX && headY == fruitY)
            {
                score += 5; nTail++;    //Tinh diem khi an qua
                randomQua();            //random diem qua moi
            }
            if (((dir == "LEFT" && pre_dir != "UP") && (dir == "LEFT" && pre_dir != "DOWN")) || ((dir == "RIGHT" && pre_dir != "UP") && (dir == "RIGHT" && pre_dir != "DOWN"))) horizontal = true;
            else horizontal = false;

            if (((dir == "UP" && pre_dir != "LEFT") && (dir == "UP" && pre_dir != "RIGHT")) || ((dir == "DOWN" && pre_dir != "LEFT") && (dir == "DOWN" && pre_dir != "RIGHT"))) vertical = true;
            else vertical = false;

            //kiem tra cai dau va cham than con ran
            for (int i = 1; i < nTail; i++)
            {
                if (headX == TailX[i] && headY == TailY[i])
                {
                    if (horizontal || vertical) gameOver = false; //quay dau 180 do [bay loi quay dau]
                    else gameOver = true;
                }
                if (fruitX == TailX[i] && fruitY == TailY[i]) randomQua(); //qua trung than con ran cho random lai
            }
        }
        void Render()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == 0 || i == height - 1)  //vien khung tren va duoi
                        Console.Write("#");
                    else if (j == 0 || j == width - 1) //vien khung trai va phai
                        Console.Write("#");
                    else if (j == fruitX && i == fruitY) //hop qua
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("?");
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else if (j == headX && i == headY)  //dau con ran
                        Console.Write("0");
                    else
                    {
                        isprinted = false;
                        for (int k = 0; k < nTail; k++)
                        {
                            if (TailX[k] == j && TailY[k] == i)
                            {
                                Console.Write("o"); //than con ran
                                isprinted = true;
                            }
                        }
                        if (!isprinted) Console.Write("_"); //Vi tri con lai
                    }
                }
                Console.WriteLine();
            }
            //Hien thi panel thong tin diem phia duoi khung vien
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Your score: " + score);
        }
        void Lose()
        {
            Console.WriteLine("YOU DIED!");
            Console.WriteLine("      - Press R key to RESET game");
            Console.WriteLine("      - Press Q key to QUIT game");
            
            while (true)
            {
                keypress = Console.ReadKey(true);
                if (keypress.Key == ConsoleKey.Q) Environment.Exit(0);
                if (keypress.Key == ConsoleKey.R) reset = true; break;
            }
            
        }
        static void Main(string[] args)
        {
            SnakeGame snakegame = new SnakeGame();  //game khong xac dinh diem dung
            snakegame.ShowBanner();
            while (true)
            {
                snakegame.Setup();
                snakegame.Update();
                Console.Clear();    //Xoa man hinh hien thi
            }
        /*   
        //Console.SetCursorPosition(x, y) x la truc ngang, y la truc doc
        Console.SetCursorPosition(10, 5);
        Console.BackgroundColor = ConsoleColor.Green;   //Đặt màu nền
        Console.ForegroundColor = ConsoleColor.Blue;    //Đặt màu chữ
        Console.WriteLine("Hello");

        ConsoleKeyInfo keyInfo = Console.ReadKey();    //lấy phím vừa bấm
        switch(keyInfo.Key)
        {
            case ConsoleKey.A: Console.WriteLine("Ban vua bam phim A"); break;
            case ConsoleKey.UpArrow: Console.WriteLine("Ban vua bam phim mui ten len"); break;
            case ConsoleKey.LeftArrow: Console.WriteLine("Ban vua bam phim mui ten trai"); break;
            case ConsoleKey.NumPad1: Console.WriteLine("Ban vua bam phim 1"); break;
            case ConsoleKey.Enter: Console.WriteLine("Ban vua bam phim Enter"); break;

        }  
        */
        }
    }
}

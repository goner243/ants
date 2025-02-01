using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ants
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        Graphics gr;

        Random rand = new Random();

        int antSize = 1;
        int antsCount = 3000;
        int bordersize = 5;
        int debugLineLengh = 50;
        int signalRadius = 100;
        int maxBases = 2;
        int spawnCount = 2;
        int generation = 0;

        List<Point> foodp = new List<Point>();

        List<Fraction> fractions = new List<Fraction>();

        int pictureH;
        int pictureW;

        Color backgroundColor = Color.Black;


        public Form1()
        {
            InitializeComponent();

            pictureW = pictureBox1.Width;
            pictureH = pictureBox1.Height;

            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gr = Graphics.FromImage(bmp);

            SpawnFood();

            SpawnAnts();

            reDraw();

            timer1.Start();
        }

        public void SpawnFood()
        {
            if (foodp.Count < 4)
            {
                Point food = new Point() { food = true, foodCount = 2000 };

                food.x = rand.Next(0, pictureW - food.radius);
                food.y = rand.Next(0, pictureH - food.radius);

                foodp.Add(food);

                generation++;
            }
        }

        public class Point
        {
            public bool food = false;
            public int x;
            public int y;
            public int foodCount;
            public int radius = 31;
            public int life = 2000;
            public int center => radius / 2 + 1;

            public void Step()
            {
                life--;
            }
        }

        public void SpawnAnts()
        {
            Random rndPosX = new Random();
            Random rndDir = new Random();

            Fraction frac = new Fraction() { id = 0, mainColor = Color.Blue, foodColor = Color.DeepSkyBlue, Brush = Brushes.Blue };

            Point mainB = new Point() { food = false, x = 300, y = 300, };
            frac.lp.Add(mainB);

            for (int i = 0; i < antsCount; i++)
            {
                SpawnAnt(rndPosX, rndDir, frac);
            }

            fractions.Add(frac);

            frac = new Fraction() { id = 1, mainColor = Color.Red, foodColor = Color.MediumVioletRed, Brush = Brushes.Red };

            mainB = new Point() { food = false, x = 900, y = 600, };
            frac.lp.Add(mainB);

            for (int i = 0; i < antsCount; i++)
            {
                SpawnAnt(rndPosX, rndDir, frac);
            }

            fractions.Add(frac);
        }

        public void SpawnAnt(Random x, Random y, Fraction ants, int posx, int posy)
        {
            Ant ant = new Ant();

            ant.fraction = ants.id;

            ant.mainColor = ants.mainColor;

            ant.foodColor = ants.foodColor;

            ant.x = posx;

            ant.y = posy;

            ant.direction = y.Next(-180, 181);

            ant.nextx = (int)(ant.speed * Math.Cos(ant.direction / 57.29) + ant.x + (antSize / 2));
            ant.nexty = (int)(ant.speed * Math.Sin(ant.direction / 57.29) + ant.y + (antSize / 2));

            ant.aDistance = x.Next(600, 1000);
            ant.bDistance = x.Next(600, 1000);

            ant.size = antSize;

            ant.color = Color.Red;

            ant.antGen = generation;

            ant.size = antSize * x.Next(1) + 1;

            ant.speed = y.Next(4 / ant.size * 4, 4 / ant.size * 8);


            ants.ants.Add(ant);
        }

        public void SpawnAnt(Random x, Random y, Fraction ants)
        {
            Ant ant = new Ant();

            ant.fraction = ants.id;

            ant.mainColor = ants.mainColor;

            ant.foodColor = ants.foodColor;

            ant.x = x.Next(0, pictureBox1.Width);

            ant.y = x.Next(0, pictureBox1.Height);

            ant.direction = y.Next(-180, 181);

            ant.nextx = (int)(ant.speed * Math.Cos(ant.direction / 57.29) + ant.x + (antSize / 2));
            ant.nexty = (int)(ant.speed * Math.Sin(ant.direction / 57.29) + ant.y + (antSize / 2));

            ant.aDistance = x.Next(600, 1000);
            ant.bDistance = x.Next(600, 1000);

            ant.size = antSize;

            ant.color = Color.Red;

            ant.antGen = generation;

            ant.size = antSize * x.Next(1) + 1;

            ant.speed = y.Next(4 / ant.size * 4, 4 / ant.size * 8);


            ants.ants.Add(ant);
        }

        public void MoveAnts(Fraction frac)
        {
            Random rnd = new Random();

            frac.ants.ToList().AsParallel().ForAll(ant =>
            {
                if (ant.life > 0)
                {
                    var stopwatch = new Stopwatch();

                    stopwatch.Start();

                    CheckBorder(ant, frac);

                    stopwatch.Stop();

                    Console.WriteLine(stopwatch.Elapsed);

                    ant.x = ant.nextx;
                    ant.y = ant.nexty;

                    ant.nextx = (int)(ant.speed * Math.Cos(ant.direction / 57.29) + ant.x);
                    ant.nexty = (int)(ant.speed * Math.Sin(ant.direction / 57.29) + ant.y);

                    if (rnd.Next(101) > 95)
                    {
                        ant.direction += rnd.Next(-5, 6);
                    }
                }
            });
        }

        static void CheckEnemies(Ant ant, Fraction frac, Ant enemy, Fraction enemies)
        {
            //(ant.nextx - enemy.x) * (ant.nextx - enemy.x) + (ant.nexty - enemy.y) * (ant.nexty - enemy.y) 
            if ((ant.nextx - enemy.x) * (ant.nextx - enemy.x) + (ant.nexty - enemy.y) * (ant.nexty - enemy.y) <= 25) //(enemy.distantion <= 25)
            {
                ant.direction += 180;
                if (ant.direction > 180)
                {
                    ant.direction -= 360;
                }

                if (!ant.food)
                {
                    enemies.antsDead.Add(enemy);

                    ant.food = true;

                    ant.aDistance = 100;

                    ant.nextx = (int)(ant.speed * Math.Cos(ant.direction / 57.29) + ant.x);
                    ant.nexty = (int)(ant.speed * Math.Sin(ant.direction / 57.29) + ant.y);
                }
            }
        }

        public void CheckBorder(Ant ant, Fraction frac)
        {
            int wallInclinationRadians = 1000;

            Fraction enemies = fractions.Find(a => a != frac);

            Parallel.ForEach(enemies.antsFood, enemy =>
            {
                CheckEnemies(ant, frac, enemy, enemies);
            });

            foreach (Point food in foodp.ToList())
            {
                if ((ant.nextx - (food.center + food.x)) * (ant.nextx - (food.center + food.x)) + (ant.nexty - (food.center + food.y)) * (ant.nexty - (food.center + food.y)) <= food.center * food.center)
                {
                    ant.direction += 180;
                    if (ant.direction > 180)
                    {
                        ant.direction -= 360;
                    }

                    ant.nextx = (int)(ant.speed * Math.Cos(ant.direction / 57.29) + ant.x);
                    ant.nexty = (int)(ant.speed * Math.Sin(ant.direction / 57.29) + ant.y);

                    if (!ant.food)
                    {
                        food.foodCount -= ant.foodCount;
                    }


                    ant.food = true;

                    ant.aDistance = 0;
                }
            }

            foreach (Point mainB in frac.lp)
            {

                if ((ant.nextx - (mainB.center + mainB.x)) * (ant.nextx - (mainB.center + mainB.x)) + (ant.nexty - (mainB.center + mainB.y)) * (ant.nexty - (mainB.center + mainB.y)) <= mainB.center * mainB.center)
                {
                    ant.direction += 180;
                    if (ant.direction > 180)
                    {
                        ant.direction -= 360;
                    }

                    ant.nextx = (int)(ant.speed * Math.Cos(ant.direction / 57.29) + ant.x);
                    ant.nexty = (int)(ant.speed * Math.Sin(ant.direction / 57.29) + ant.y);

                    if (ant.food)
                    {
                        if (ant.life < rand.Next(200))
                        {
                            frac.totalFood += ant.foodCount;
                        }
                        else
                        {
                            frac.totalFood++;
                            ant.life += 300;
                        }
                    }

                    ant.food = false;

                    ant.bDistance = 0;
                }

            }


            if (ant.nexty <= bordersize)
            {
                wallInclinationRadians = 0;
            }
            else if (ant.nextx >= pictureW - (bordersize + antSize))
            {
                wallInclinationRadians = 90;
            }
            else if (ant.nexty >= pictureH - (bordersize + antSize))
            {
                wallInclinationRadians = 180;
            }
            else if (ant.nextx <= bordersize)
            {
                wallInclinationRadians = -90;
            }

            if (wallInclinationRadians != 1000)
            {
                if (ant.direction > 180)
                {
                    ant.direction -= 360;
                }

                ant.direction = ant.direction + 2 * (wallInclinationRadians - ant.direction);

                ant.nextx = (int)(ant.speed * Math.Cos(ant.direction / 57.29) + ant.x);
                ant.nexty = (int)(ant.speed * Math.Sin(ant.direction / 57.29) + ant.y);
            }


            ant.SetColor();

        }

        public void DrawAnts(List<Ant> ants)
        {
            foreach (Ant ant in ants)
            {
                gr.DrawEllipse(new Pen(ant.color) { Width = ant.speed / 8 }, ant.x, ant.y, ant.size, ant.size);
                //gr.DrawLine(new Pen(Brushes.Blue), ant.x + (antSize / 2), ant.y + (antSize / 2), (float)(debugLineLengh * Math.Cos(ant.direction / (180 / Math.PI)) + ant.x + (antSize / 2)), (float)(debugLineLengh * Math.Sin(ant.direction / (180 / Math.PI)) + ant.y + (antSize / 2)));
            }
        }

        public class Ant
        {
            public Color color;
            public Color mainColor;
            public Color foodColor;
            public int life = 500;
            public int direction; //1-359
            public bool food = false;
            public int x;
            public int y;
            public int nextx;
            public int nexty;
            public int aDistance;
            public int bDistance;
            public int speed;
            public int size;
            public int antGen = 0;
            public int fraction;
            public int distantion;
            public int foodCount => size * 3;

            public Ant friend;

            public void SetColor()
            {
                if (food)
                {
                    color = foodColor;
                }
                else
                {
                    color = mainColor;
                }
            }

            public void Step()
            {
                aDistance++;
                bDistance++;
                life--;
            }
        }

        public class Fraction
        {
            public int id;

            public List<Ant> ants = new List<Ant>();

            public List<Ant> antsA = new List<Ant>();
            public List<Ant> antsB = new List<Ant>();

            public List<Ant> antsFood = new List<Ant>();
            public List<Ant> antsDead = new List<Ant>();

            public List<Point> lp = new List<Point>();

            public Color mainColor;
            public Color foodColor;

            public Brush Brush;

            public int totalFood = 0;
        }

        public void Scan(List<Ant> ants, List<Ant> antsA, List<Ant> antsB)
        {
            int signalPow = signalRadius * signalRadius;

            ants.AsParallel().ForAll(ant =>
            {
                if (!ant.food)
                {
                    var friend = antsA.Find(a => (ant.x - a.x) * (ant.x - a.x) + (ant.y - a.y) * (ant.y - a.y) <= signalPow);

                    if (ant.friend != friend && friend != null)
                    {
                        if (ant.aDistance > friend.aDistance)
                        {
                            ant.aDistance = friend.aDistance + signalRadius;

                            ant.direction = (int)(Math.Atan2(friend.y - ant.y, friend.x - ant.x) * 57.29);

                            //gr.DrawLine(new Pen(Brushes.Yellow), ant.x, ant.y, friend.x, friend.y);

                            ant.friend = friend;
                        }
                    }
                }
                else
                {
                    var friend = antsB.Find(a => (ant.x - a.x) * (ant.x - a.x) + (ant.y - a.y) * (ant.y - a.y) <= signalPow);

                    if (ant.friend != friend && friend != null)
                    {
                        if (ant.bDistance > friend.bDistance)
                        {
                            ant.bDistance = friend.bDistance + signalRadius;

                            ant.direction = (int)(Math.Atan2(friend.y - ant.y, friend.x - ant.x) * 57.29);

                            //gr.DrawLine(new Pen(Brushes.White), ant.x, ant.y, friend.x, friend.y);

                            ant.friend = friend;
                        }
                    }
                }
            });
        }

        public void SetStep(Fraction frac)
        {
            foreach (Ant ant in frac.ants.ToList())
            {
                if (ant.life <= 0)
                {
                    frac.ants.Remove(ant);

                    if (rand.Next(100) > 95 && frac.lp.Count <= maxBases && frac.totalFood > 500)
                    {
                        SpawnBase(ant, frac);
                    }
                }
                else
                {
                    ant.aDistance++;
                    ant.bDistance++;
                    ant.life--;
                }
            }

            //ants.ForEach(a => a.Step());
        }

        public void SpawnNewAnt(Fraction fract)
        {
            foreach (Point baseM in fract.lp)
            {
                for (int i = 0; i < spawnCount; i++)
                {
                    if (rand.Next(100) > 50 && fract.totalFood >= 10)
                    {
                        Random rndPosX = new Random();
                        Random rndDir = new Random();
                        SpawnAnt(rndPosX, rndDir, fract, baseM.x + rndPosX.Next(50), baseM.y + rndPosX.Next(50));
                        fract.totalFood -= 10;
                    }
                }
            }
        }

        public void SpawnBase(Ant ant, Fraction frac)
        {
            Point mainB = new Point() { food = false, x = ant.x, y = ant.y };
            frac.lp.Add(mainB);
            frac.totalFood -= 250;

            if (frac.totalFood > 10000 && antSize < 3)
            {
                antSize++;
                frac.totalFood -= 7000;
            }
            if (frac.totalFood > 100000 && spawnCount < 4)
            {
                spawnCount++;
                frac.totalFood -= 90000;
            }
        }

        public void DrawBases(Fraction frac)
        {
            foreach (Point p in frac.lp.ToList())
            {
                if (p.life > 0)
                {
                    gr.FillEllipse(frac.Brush, p.x, p.y, p.radius, p.radius);
                    p.life--;
                }
                else
                {
                    frac.lp.Remove(p);
                }
            }
        }

        public void DrawFood()
        {
            if (rand.Next(100) > 98)
            {
                SpawnFood();
            }

            foreach (Point food in foodp.ToList())
            {
                if (food.foodCount > 0)
                {
                    gr.FillEllipse(Brushes.Green, food.x, food.y, food.radius, food.radius);
                }
                else
                {
                    foodp.Remove(food);
                }
            }
        }

        public void SortAnts(Fraction frac)
        {
            foreach (Ant ant in frac.antsDead)
            {
                frac.ants.Remove(ant);
            }
            frac.antsA = new List<Ant>(frac.ants.OrderBy(a => a.aDistance));
            frac.antsB = new List<Ant>(frac.ants.OrderBy(a => a.bDistance));
            frac.antsFood = new List<Ant>(frac.ants.FindAll(a => a.food));
        }

        private void reDraw()
        {
            //gr = Graphics.FromImage(bmp);
            gr.Clear(backgroundColor);

            gr.DrawRectangle(new Pen(Brushes.DarkGray) { Width = bordersize }, 0, 0, pictureBox1.Width, pictureBox1.Height);

            foreach (Fraction frac in fractions)
            {
                SortAnts(frac);
                DrawFood();
                DrawBases(frac);

                MoveAnts(frac);

                SetStep(frac);
                Scan(frac.ants, frac.antsA, frac.antsB);
                DrawAnts(frac.ants);
                SpawnNewAnt(frac);
            }

            pictureBox1.Image = bmp;
            //gr.Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            reDraw();
            //textBox1.Text = ants.Count.ToString();
            //label2.Text = "Total Food " + totalFood.ToString();
            label3.Text = "Total Generation " + generation.ToString();
            //label4.Text = "Oldest Generation " + ants.Select(a => a.antGen).Min(a => a).ToString();
            //CalcKernel();
        }
    }
}

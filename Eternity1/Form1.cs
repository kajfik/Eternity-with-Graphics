using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Eternity1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static int szyrzka = 680, wyszka = 680;
        public static int iloscKostek = 256;
        static int b24p3 = 24 * 24 * 24 * 2;
        static int b24p2 = 24 * 24 * 2;
        static int b24p1 = 24 * 2;
        static int b24p0 = 2;
        public static int szyrzkaPola = Convert.ToInt32(Math.Sqrt(iloscKostek));
        public static int wyszkaPola = szyrzkaPola;
        public static Bitmap bmp = new Bitmap(1, 1);
        public Graphics g;
        static int maxDoTeraz = 0;
        public int numerRzeszeni = 0;
        int iloscDanychKostek = 0; //resetuje se kazdom sekunde
        Int64 iloscDanychKostek2 = 0;
        Int64 secondsElapsed = 0;
        Int64 lastTime = 0;
        public int[,] xypole = new int[szyrzkaPola, szyrzkaPola];
        public static bool[] polozono = new bool[iloscKostek + 1];
        public int[, ,] xypoleSmery = new int[szyrzkaPola, szyrzkaPola, 4];
        public bool hledom3x3 = false;
        public bool rzeszic3x3;
        public int odKielaZkuszac3x3 = 128;
        bool znalezione3x3;
        static int wielkoscPorzadi3x3 = 1;
        int[, ] porzadi3x3 = new int[100, 2];
        static int wielkoscPorzadi = 1;
        Random rnd1 = new Random();
        int[] graph;
        /*
            I get empty tile with colors a north, b east, c south, d west
            returnTile[a, b, c, d, 0] returns numbers of tiles that can go there with ...[1] orientation
            23 is no color there, so empty space there
            0 is the side of the board
        */
        //[a, b, c, d, 0] - numer, ...[1] - obrocyni
        List<int>[] returnTile;

        //[numerkostki, lewo/gora/prawo/dol] = numer obrazku
        public int[,] kostki2 = new int[,] {{7, 6, 6, 7}, {7, 6, 7, 6}, {7, 6, 5, 6}, {7, 6, 5, 7}, {7, 6, 5, 6}, {6, 6, 5, 7}, 
                                           {4, 7, 2, 0}, {7, 6, 6, 7}, {6, 7, 6, 6}, {7, 1, 0, 1}, {6, 1, 0, 2}, {4, 0, 1, 7},
                                           {0, 1, 6, 3}, {3, 4, 0, 0}, {2, 0, 3, 7}, {1, 6, 2, 0}, {0, 3, 6, 3}, {7, 1, 0, 2},
                                           {6, 7, 6, 7}, {1, 0, 3, 7}, {1, 7, 2, 0}, {6, 2, 0, 4}, {2, 7, 3, 0}, {7, 7, 6, 6},
                                           {7, 4, 0, 2}, {4, 0, 0, 4}, {6, 7, 6, 6}, {3, 0, 4, 7}, {5, 5, 6, 6}, {7, 7, 7, 5},
                                           {0, 3, 6, 1}, {7, 7, 6, 7}, {7, 7, 6, 5}, {3, 0, 0, 2}, {6, 7, 7, 7}, {0, 0, 4, 4}};
        //[numerkostki, gora/prawo/dol/lewo]
        public int[,] kostkiPrawe = new int[,] {
	/* 1 - 16 */{ 1, 17, 0, 0 }, { 1, 5, 0, 0 }, { 9, 17, 0, 0 }, { 17, 9, 0, 0 }, { 2, 1, 0, 1 }, { 10, 9, 0, 1 }, { 6, 1, 0, 1 }, { 6, 13, 0, 1 }, { 11, 17, 0, 1 }, { 7, 5, 0, 1 }, { 15, 9, 0, 1 }, { 8, 5, 0, 1 }, { 8, 13, 0, 1 }, { 21, 5, 0, 1 }, { 10, 1, 0, 9 }, { 18, 17, 0, 9 },
	/* 17 - 32 */{ 14, 13, 0, 9 }, { 19, 13, 0, 9 }, { 7, 9, 0, 9 }, { 15, 9, 0, 9 }, { 4, 5, 0, 9 }, { 12, 1, 0, 9 }, { 12, 13, 0, 9 }, { 20, 1, 0, 9 }, { 21, 1, 0, 9 }, { 2, 9, 0, 17 }, { 2, 17, 0, 17 }, { 10, 17, 0, 17 }, { 18, 17, 0, 17 }, { 7, 13, 0, 17 }, { 15, 9, 0, 17 }, { 20, 17, 0, 17 },
	/* 33 - 48 */{ 8, 9, 0, 17 }, { 8, 5, 0, 17 }, { 16, 13, 0, 17 }, { 22, 5, 0, 17 }, { 18, 1, 0, 5 }, { 3, 13, 0, 5 }, { 11, 13, 0, 5 }, { 19, 9, 0, 5 }, { 19, 17, 0, 5 }, { 15, 1, 0, 5 }, { 15, 9, 0, 5 }, { 15, 17, 0, 5 }, { 4, 1, 0, 5 }, { 20, 5, 0, 5 }, { 8, 5, 0, 5 }, { 16, 5, 0, 5 },
	/* 49 - 64 */{ 2, 13, 0, 13 }, { 10, 1, 0, 13 }, { 10, 9, 0, 13 }, { 6, 1, 0, 13 }, { 7, 5, 0, 13 }, { 4, 5, 0, 13 }, { 4, 13, 0, 13 }, { 8, 17, 0, 13 }, { 16, 1, 0, 13 }, { 16, 13, 0, 13 }, { 21, 9, 0, 13 }, { 22, 17, 0, 13 }, { 6, 18, 2, 2 }, { 14, 7, 2, 2 }, { 10, 3, 2, 10 }, { 2, 8, 2, 18 },
	/* 65 - 80 */{ 18, 22, 2, 18 }, { 14, 14, 2, 18 }, { 11, 10, 2, 18 }, { 20, 6, 2, 18 }, { 22, 8, 2, 18 }, { 3, 7, 2, 3 }, { 7, 12, 2, 3 }, { 14, 18, 2, 11 }, { 15, 4, 2, 11 }, { 20, 15, 2, 11 }, { 8, 3, 2, 11 }, { 14, 15, 2, 19 }, { 19, 15, 2, 19 }, { 3, 16, 2, 7 }, { 20, 3, 2, 7 }, { 16, 21, 2, 7 },
	/* 81 - 96 */{ 19, 18, 2, 15 }, { 18, 18, 2, 4 }, { 11, 4, 2, 4 }, { 18, 19, 2, 12 }, { 6, 14, 2, 12 }, { 8, 12, 2, 12 }, { 16, 20, 2, 12 }, { 2, 21, 2, 20 }, { 6, 22, 2, 20 }, { 4, 16, 2, 20 }, { 11, 12, 2, 8 }, { 19, 15, 2, 8 }, { 19, 4, 2, 8 }, { 4, 21, 2, 8 }, { 12, 14, 2, 8 }, { 21, 3, 2, 21 },
	/* 97 - 112 */{ 4, 19, 2, 22 }, { 20, 8, 2, 22 }, { 21, 6, 2, 22 }, { 22, 21, 2, 22 }, { 12, 15, 10, 10 }, { 12, 16, 10, 10 }, { 16, 19, 10, 10 }, { 22, 6, 10, 10 }, { 4, 15, 10, 18 }, { 3, 8, 10, 6 }, { 19, 8, 10, 6 }, { 4, 15, 10, 6 }, { 16, 11, 10, 6 }, { 15, 12, 10, 14 }, { 12, 15, 10, 14 }, { 20, 19, 10, 3 },
	/* 113 - 128 */{ 20, 16, 10, 3 }, { 14, 4, 10, 11 }, { 7, 12, 10, 11 }, { 12, 11, 10, 11 }, { 22, 16, 10, 11 }, { 3, 21, 10, 19 }, { 16, 12, 10, 7 }, { 8, 22, 10, 15 }, { 14, 22, 10, 4 }, { 6, 16, 10, 20 }, { 14, 19, 10, 20 }, { 20, 15, 10, 20 }, { 12, 22, 10, 8 }, { 21, 15, 10, 8 }, { 14, 6, 10, 16 }, { 19, 21, 10, 16 },
	/* 129 - 144 */{ 4, 3, 10, 16 }, { 20, 8, 10, 16 }, { 6, 20, 10, 21 }, { 12, 14, 10, 21 }, { 14, 16, 10, 22 }, { 11, 4, 10, 22 }, { 4, 3, 10, 22 }, { 16, 20, 10, 22 }, { 20, 7, 18, 18 }, { 6, 3, 18, 6 }, { 6, 11, 18, 6 }, { 6, 12, 18, 6 }, { 19, 21, 18, 6 }, { 15, 6, 18, 6 }, { 16, 12, 18, 6 }, { 21, 21, 18, 6 },
	/* 145 - 160 */{ 3, 4, 18, 14 }, { 18, 12, 18, 3 }, { 18, 22, 18, 3 }, { 3, 14, 18, 3 }, { 15, 12, 18, 3 }, { 6, 11, 18, 19 }, { 4, 22, 18, 19 }, { 11, 11, 18, 7 }, { 11, 19, 18, 7 }, { 22, 16, 18, 7 }, { 7, 7, 18, 7 }, { 7, 12, 18, 4 }, { 22, 7, 18, 4 }, { 7, 16, 18, 20 }, { 8, 6, 18, 20 }, { 21, 21, 18, 8 },
	/* 161 - 176 */{ 6, 20, 18, 16 }, { 14, 20, 18, 16 }, { 15, 11, 18, 22 }, { 4, 16, 18, 22 }, { 3, 4, 6, 14 }, { 4, 8, 6, 14 }, { 3, 3, 6, 11 }, { 11, 15, 6, 19 }, { 19, 21, 6, 19 }, { 4, 8, 6, 7 }, { 20, 16, 6, 7 }, { 21, 11, 6, 7 }, { 15, 15, 6, 15 }, { 12, 20, 6, 15 }, { 7, 21, 6, 4 }, { 7, 19, 6, 12 },
	/* 177 - 192 */{ 14, 4, 6, 20 }, { 12, 16, 6, 8 }, { 8, 15, 6, 8 }, { 7, 16, 6, 16 }, { 11, 16, 6, 21 }, { 7, 11, 6, 21 }, { 19, 8, 14, 14 }, { 22, 7, 14, 3 }, { 19, 12, 14, 11 }, { 8, 8, 14, 11 }, { 21, 7, 14, 19, }, { 14, 21, 14, 7 }, { 3, 19, 14, 7 }, { 16, 19, 14, 7 }, { 3, 3, 14, 15 }, { 15, 20, 14, 15 },
	/* 193 - 208 */{ 11, 7, 14, 4 }, { 21, 11, 14, 12 }, { 21, 22, 14, 12 }, { 22, 15, 14, 12 }, { 11, 22, 14, 20 }, { 19, 8, 14, 20 }, { 20, 20, 14, 20 }, { 19, 3, 14, 8 }, { 21, 8, 14, 16 }, { 22, 7, 14, 16 }, { 12, 19, 14, 21 }, { 12, 8, 14, 21 }, { 16, 3, 14, 21 }, { 22, 21, 14, 21 }, { 22, 7, 3, 3 }, { 19, 22, 3, 11 },
	/* 209 - 224 */{ 8, 15, 3, 11 }, { 11, 19, 3, 7 }, { 16, 15, 3, 7 }, { 3, 16, 3, 15 }, { 8, 8, 3, 4 }, { 3, 20, 3, 12 }, { 4, 22, 3, 12 }, { 22, 21, 3, 12 }, { 19, 15, 3, 20 }, { 4, 12, 3, 16 }, { 11, 4, 3, 21 }, { 11, 16, 3, 22 }, { 21, 21, 3, 22 }, { 21, 22, 3, 22 }, { 12, 22, 11, 11 }, { 20, 7, 11, 11 },
	/* 225 - 240 */{ 16, 15, 11, 11 }, { 19, 15, 11, 7 }, { 12, 12, 11, 7 }, { 19, 8, 11, 4 }, { 7, 22, 11, 20 }, { 16, 8, 11, 20 }, { 12, 20, 11, 8 }, { 12, 21, 11, 8 }, { 19, 20, 19, 19 }, { 16, 4, 19, 7 }, { 7, 4, 19, 4 }, { 7, 20, 19, 4 }, { 12, 15, 19, 4 }, { 4, 16, 19, 12 }, { 15, 22, 19, 20 }, { 21, 15, 19, 20 },
	/* 241 - 256 */{ 7, 21, 19, 8 }, { 4, 21, 19, 8 }, { 15, 12, 7, 15 }, { 20, 8, 7, 15 }, { 22, 20, 7, 4 }, { 16, 22, 7, 21 }, { 21, 22, 15, 15 }, { 12, 4, 15, 4 }, { 4, 21, 15, 12 }, { 16, 21, 15, 20 }, { 22, 8, 4, 4 }, { 8, 12, 4, 12 }, { 16, 20, 12, 8 }, { 21, 16, 20, 16 }, { 16, 22, 20, 22 }, { 21, 22, 8, 22 }
};
        public int[,] kostki = new int[256, 7];
        int[] kostkiNowe = new int[256]; //used to remember how are kostki changed
        public int[,] porzadi = new int[iloscKostek, 2];

        Thread mainThread;
        bool stopped = false;

        void zapiszPorzadi()
        {
            char[] numberc = new char[4];
            int[] number = new int[4];
            System.IO.StreamReader file = new System.IO.StreamReader("C:\\Eternity\\porzadi.txt");
            int[] a = new int[szyrzkaPola];
            int rzondek = 0;
            while (rzondek <= wyszkaPola - 1)
            {
                for (int c = 0; c < szyrzkaPola; c++)
                {
                    file.Read(numberc, 0, numberc.Length);
                    number[0] = numberc[0] - 48;
                    number[1] = numberc[1] - 48;
                    number[2] = numberc[2] - 48;
                    number[3] = numberc[3] - 48;
                    if (numberc[2] == ' ')
                    {
                        a[c] = number[3];
                    }
                    else if (numberc[1] == ' ')
                    {
                        a[c] = number[2] * 10 + number[3];
                    }
                    else
                    {
                        a[c] = number[1] * 100 + number[2] * 10 + number[3];
                    }
                }
                file.Read();
                file.Read();
                for (int b = 0; b < szyrzkaPola; b++)
                {
                    if (a[b] != 0)
                    {
                        porzadi[a[b] - 1, 0] = b;
                        porzadi[a[b] - 1, 1] = rzondek;
                        if (a[b] > wielkoscPorzadi)
                        {
                            wielkoscPorzadi = a[b];
                        }
                    }
                }
                rzondek++;
            }
            file.Close();
        }

        public void namalujMrzizke()
        {
            for (int a = 1; a <= szyrzkaPola - 1; a++)
            {
                for (int b = 0; b < szyrzka; b++)
                {
                    bmp.SetPixel(a * szyrzka / szyrzkaPola, b, Color.Black);
                    bmp.SetPixel(b, a * szyrzka / szyrzkaPola, Color.Black);
                }
            }
        }

        public void reset()
        {
            numerRzeszeni = 0;
            for (int i = 0; i < 256; i++)
            {
                kostkiNowe[i] = i;
            }
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    kostki[i, j] = kostkiPrawe[i, j];
                }
            }
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    kostki[i, j + 4] = kostki[i, j];
                }
            }
            for(int a = 0; a < szyrzkaPola; a++)
            {
                for (int b = 0; b < szyrzkaPola; b++)
                {
                    xypole[a, b] = 256;
                }
            }
            for (int a = 0; a < szyrzkaPola; a++)
            {
                for (int b = 0; b < szyrzkaPola; b++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        xypoleSmery[a, b, c] = -1;
                    }
                }
            }
            for (int a = 0; a < iloscKostek + 1; a++)
            {
                polozono[a] = false;
            }
        }

        public void namalujGraph()
        {
            g = Graphics.FromImage(bmp);
            pictureBox1.Image = bmp;
            Brush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(brush);
            Brush brush2 = new SolidBrush(Color.Red);
            Pen pen2 = new Pen(brush2);
            g.DrawLine(pen, 0, wyszka - 2 * 256 - 1, szyrzka, wyszka - 2 * 256 - 1);
            g.DrawLine(pen, 0, wyszka - 1, szyrzka, wyszka - 1);
            g.DrawLine(pen2, 0, wyszka - 2 * (maxDoTeraz + 1) - 1, szyrzka, wyszka - 2 * (maxDoTeraz + 1) - 1);
            for (int i = 0; i < szyrzka - 1; i++)
            {
                g.DrawLine(pen, i, wyszka - 2 * graph[i] - 1, i + 1, wyszka - 2 * graph[i + 1] - 1);
            }
            pictureBox1.Show();
            pictureBox1.Update();
            pictureBox1.Image = bmp;
        }

        public void namalujKostki()
        {
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            namalujMrzizke();
            Font mojFont = new Font(FontFamily.GenericSansSerif, 7, FontStyle.Bold);
            Font mojFont2 = new Font(FontFamily.GenericSansSerif, 5, FontStyle.Regular);
            for (int a = 0; a <= szyrzkaPola - 1; a++)
            {
                for (int b = 0; b <= szyrzkaPola - 1; b++)
                {
                    if (xypole[a, b] >= 0 && polozono[xypole[a, b]])
                    {
                        g.DrawString(Convert.ToString(xypole[a, b] == 256 ? 0 : kostkiNowe[xypole[a, b]] + 1), mojFont, Brushes.Black, new Point(a * szyrzka / szyrzkaPola + szyrzka / szyrzkaPola / 2 - 5 - 7, b * szyrzka / szyrzkaPola + szyrzka / szyrzkaPola / 2 - 5 - 3));
                        g.DrawString(Convert.ToString(xypoleSmery[a, b, 0]), mojFont2, Brushes.Black, new Point(a * szyrzka / szyrzkaPola + szyrzka / szyrzkaPola / 2 - 5, b * szyrzka / szyrzkaPola + szyrzka / szyrzkaPola / 2 - 5 - 15));
                        g.DrawString(Convert.ToString(xypoleSmery[a, b, 1]), mojFont2, Brushes.Black, new Point(a * szyrzka / szyrzkaPola + szyrzka / szyrzkaPola / 2 - 5 + 15, b * szyrzka / szyrzkaPola + szyrzka / szyrzkaPola / 2 - 5));
                        g.DrawString(Convert.ToString(xypoleSmery[a, b, 2]), mojFont2, Brushes.Black, new Point(a * szyrzka / szyrzkaPola + szyrzka / szyrzkaPola / 2 - 5, b * szyrzka / szyrzkaPola + szyrzka / szyrzkaPola / 2 - 5 + 15));
                        g.DrawString(Convert.ToString(xypoleSmery[a, b, 3]), mojFont2, Brushes.Black, new Point(a * szyrzka / szyrzkaPola + szyrzka / szyrzkaPola / 2 - 5 - 17, b * szyrzka / szyrzkaPola + szyrzka / szyrzkaPola / 2 - 5));
                    }
                }
            }
            this.Invoke((MethodInvoker)delegate
                {
                    pictureBox1.Show();
                    pictureBox1.Update();
                    pictureBox1.Image = bmp;
                });
        }

        bool idzieDac3x3(int x, int y, int a, int b, int node)
        {
            znalezione3x3 = false;
            hledom3x3 = true;
            int pozycjaWporzadi3x3 = 0;
            wielkoscPorzadi3x3 = 0;
            xypole[x, y] = a;
            xypoleSmery[x, y, 0] = kostki[a, b];
            xypoleSmery[x, y, 1] = kostki[a, b + 1];
            xypoleSmery[x, y, 2] = kostki[a, b + 2];
            xypoleSmery[x, y, 3] = kostki[a, b + 3];
            polozono[a] = true;
            if (x - 1 >= 0 && xypole[x - 1, y] == 256) { porzadi3x3[pozycjaWporzadi3x3, 0] = x - 1; porzadi3x3[pozycjaWporzadi3x3, 1] = y; pozycjaWporzadi3x3++; wielkoscPorzadi3x3++; }
            if (x - 1 >= 0 && y - 1 >= 0 && xypole[x - 1, y - 1] == 256) { porzadi3x3[pozycjaWporzadi3x3, 0] = x - 1; porzadi3x3[pozycjaWporzadi3x3, 1] = y - 1; pozycjaWporzadi3x3++; wielkoscPorzadi3x3++; }
            if (y - 1 >= 0 && xypole[x, y - 1] == 256) { porzadi3x3[pozycjaWporzadi3x3, 0] = x; porzadi3x3[pozycjaWporzadi3x3, 1] = y - 1; pozycjaWporzadi3x3++; wielkoscPorzadi3x3++; }
            if (y - 1 >= 0 && x + 1 < szyrzkaPola && xypole[x + 1, y - 1] == 256) { porzadi3x3[pozycjaWporzadi3x3, 0] = x + 1; porzadi3x3[pozycjaWporzadi3x3, 1] = y - 1; pozycjaWporzadi3x3++; wielkoscPorzadi3x3++; }
            if (x + 1 < szyrzkaPola && xypole[x + 1, y] == 256) { porzadi3x3[pozycjaWporzadi3x3, 0] = x + 1; porzadi3x3[pozycjaWporzadi3x3, 1] = y; pozycjaWporzadi3x3++; wielkoscPorzadi3x3++; }
            if (x + 1 < szyrzkaPola && y + 1 < szyrzkaPola && xypole[x + 1, y + 1] == 256) { porzadi3x3[pozycjaWporzadi3x3, 0] = x + 1; porzadi3x3[pozycjaWporzadi3x3, 1] = y + 1; pozycjaWporzadi3x3++; wielkoscPorzadi3x3++; }
            if (y + 1 < szyrzkaPola && xypole[x, y + 1] == 256) { porzadi3x3[pozycjaWporzadi3x3, 0] = x; porzadi3x3[pozycjaWporzadi3x3, 1] = y + 1; pozycjaWporzadi3x3++; wielkoscPorzadi3x3++; }
            if (y + 1 < szyrzkaPola && x - 1 >= 0 && xypole[x - 1, y + 1] == 256) { porzadi3x3[pozycjaWporzadi3x3, 0] = x - 1; porzadi3x3[pozycjaWporzadi3x3, 1] = y + 1; pozycjaWporzadi3x3++; wielkoscPorzadi3x3++; }
            if (wielkoscPorzadi3x3 > 0)
            {
                dejKostke3x3(porzadi3x3[0, 0], porzadi3x3[0, 1], 0);
            }
            else { znalezione3x3 = true; }
            xypole[x, y] = 256;
            xypoleSmery[x, y, 0] = -1;
            xypoleSmery[x, y, 1] = -1;
            xypoleSmery[x, y, 2] = -1;
            xypoleSmery[x, y, 3] = -1;
            polozono[a] = false;
            return znalezione3x3;
        }

        void dejKostkeIn3x3(int x, int y, int a, int b, int node3x3)
        {
            xypole[x, y] = a;
            xypoleSmery[x, y, 0] = kostki[a, b];
            xypoleSmery[x, y, 1] = kostki[a, b + 1];
            xypoleSmery[x, y, 2] = kostki[a, b + 2];
            xypoleSmery[x, y, 3] = kostki[a, b + 3];
            polozono[a] = true;
            if (hledom3x3 == true && x == porzadi3x3[wielkoscPorzadi3x3 - 1, 0] && y == porzadi3x3[wielkoscPorzadi3x3 - 1, 1])
            {
                hledom3x3 = false;
                znalezione3x3 = true;
            }
            if (hledom3x3 == true)
            {
                if (node3x3 <= wielkoscPorzadi3x3 - 2)
                {
                    dejKostke3x3(porzadi3x3[node3x3 + 1, 0], porzadi3x3[node3x3 + 1, 1], node3x3 + 1);
                }
            }
            xypole[x, y] = 256;
            xypoleSmery[x, y, 0] = -1;
            xypoleSmery[x, y, 1] = -1;
            xypoleSmery[x, y, 2] = -1;
            xypoleSmery[x, y, 3] = -1;
            polozono[a] = false;
        }

        void dejKostke3x3(int x, int y, int node3x3)
        {
            while (stopped)
            {
                Thread.Sleep(1);
            }
            if (x == 0)
            {
                if (y == 0)
                {
                    int gornoBarwa = 0;
                    int prawoBarwa = polozono[xypole[x + 1, y]] ? xypoleSmery[x + 1, y, 3] : 23;
                    int dolnoBarwa = polozono[xypole[x, y + 1]] ? xypoleSmery[x, y + 1, 0] : 23;
                    int lewoBarwa = 0;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size && hledom3x3; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a < 4)
                        {
                            int b = returnTile[tile + 1][i];
                            dejKostkeIn3x3(x, y, a, b, node3x3);
                        }
                    }
                }
                else if (y >= 1 && y <= wyszkaPola - 2)
                {
                    int gornoBarwa = polozono[xypole[x, y - 1]] ? xypoleSmery[x, y - 1, 2] : 23;
                    int prawoBarwa = polozono[xypole[x + 1, y]] ? xypoleSmery[x + 1, y, 3] : 23;
                    int dolnoBarwa = polozono[xypole[x, y + 1]] ? xypoleSmery[x, y + 1, 0] : 23;
                    int lewoBarwa = 0;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size && hledom3x3; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a >= 4 && a < 60)
                        {
                            int b = returnTile[tile + 1][i];
                            dejKostkeIn3x3(x, y, a, b, node3x3);
                        }
                    }
                }
                else if (y == wyszkaPola - 1)
                {
                    int gornoBarwa = polozono[xypole[x, y - 1]] ? xypoleSmery[x, y - 1, 2] : 23;
                    int prawoBarwa = polozono[xypole[x + 1, y]] ? xypoleSmery[x + 1, y, 3] : 23;
                    int dolnoBarwa = 0;
                    int lewoBarwa = 0;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size && hledom3x3; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a < 4)
                        {
                            int b = returnTile[tile + 1][i];
                            dejKostkeIn3x3(x, y, a, b, node3x3);
                        }
                    }
                }
            }
            else if (x <= szyrzkaPola - 2 && x >= 1)
            {
                if (y == 0)
                {
                    int gornoBarwa = 0;
                    int prawoBarwa = polozono[xypole[x + 1, y]] ? xypoleSmery[x + 1, y, 3] : 23;
                    int dolnoBarwa = polozono[xypole[x, y + 1]] ? xypoleSmery[x, y + 1, 0] : 23;
                    int lewoBarwa = polozono[xypole[x - 1, y]] ? xypoleSmery[x - 1, y, 1] : 23;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size && hledom3x3; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a >= 4 && a < 60)
                        {
                            int b = returnTile[tile + 1][i];
                            dejKostkeIn3x3(x, y, a, b, node3x3);
                        }
                    }
                }
                else if (y >= 1 && y <= wyszkaPola - 2)
                {
                    int gornoBarwa = polozono[xypole[x, y - 1]] ? xypoleSmery[x, y - 1, 2] : 23;
                    int prawoBarwa = polozono[xypole[x + 1, y]] ? xypoleSmery[x + 1, y, 3] : 23;
                    int dolnoBarwa = polozono[xypole[x, y + 1]] ? xypoleSmery[x, y + 1, 0] : 23;
                    int lewoBarwa = polozono[xypole[x - 1, y]] ? xypoleSmery[x - 1, y, 1] : 23;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size && hledom3x3; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a >= 60)
                        {
                            int b = returnTile[tile + 1][i];
                            dejKostkeIn3x3(x, y, a, b, node3x3);
                        }
                    }
                }
                else if (y == wyszkaPola - 1)
                {
                    int gornoBarwa = polozono[xypole[x, y - 1]] ? xypoleSmery[x, y - 1, 2] : 23;
                    int prawoBarwa = polozono[xypole[x + 1, y]] ? xypoleSmery[x + 1, y, 3] : 23;
                    int dolnoBarwa = 0;
                    int lewoBarwa = polozono[xypole[x - 1, y]] ? xypoleSmery[x - 1, y, 1] : 23;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size && hledom3x3; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a >= 4 && a < 60)
                        {
                            int b = returnTile[tile + 1][i];
                            dejKostkeIn3x3(x, y, a, b, node3x3);
                        }
                    }
                }
            }
            else if (x == szyrzkaPola - 1)
            {
                if (y == 0)
                {
                    int gornoBarwa = 0;
                    int prawoBarwa = 0;
                    int dolnoBarwa = polozono[xypole[x, y + 1]] ? xypoleSmery[x, y + 1, 0] : 23;
                    int lewoBarwa = polozono[xypole[x - 1, y]] ? xypoleSmery[x - 1, y, 1] : 23;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size && hledom3x3; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a < 4)
                        {
                            int b = returnTile[tile + 1][i];
                            dejKostkeIn3x3(x, y, a, b, node3x3);
                        }
                    }
                }
                else if (y >= 1 && y <= wyszkaPola - 2)
                {
                    int gornoBarwa = polozono[xypole[x, y - 1]] ? xypoleSmery[x, y - 1, 2] : 23;
                    int prawoBarwa = 0;
                    int dolnoBarwa = polozono[xypole[x, y + 1]] ? xypoleSmery[x, y + 1, 0] : 23;
                    int lewoBarwa = polozono[xypole[x - 1, y]] ? xypoleSmery[x - 1, y, 1] : 23;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size && hledom3x3; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a >= 4 && a < 60)
                        {
                            int b = returnTile[tile + 1][i];
                            dejKostkeIn3x3(x, y, a, b, node3x3);
                        }
                    }
                }
                else if (y == wyszkaPola - 1)
                {
                    int gornoBarwa = polozono[xypole[x, y - 1]] ? xypoleSmery[x, y - 1, 2] : 23;
                    int prawoBarwa = 0;
                    int dolnoBarwa = 0;
                    int lewoBarwa = polozono[xypole[x - 1, y]] ? xypoleSmery[x - 1, y, 1] : 23;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size && hledom3x3; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a < 4)
                        {
                            int b = returnTile[tile + 1][i];
                            dejKostkeIn3x3(x, y, a, b, node3x3);
                        }
                    }
                }
            }
        }

        void dejKostkeIn(int x, int y, int a, int b, int node)
        {
            xypole[x, y] = a;
            xypoleSmery[x, y, 0] = kostki[a, b];
            xypoleSmery[x, y, 1] = kostki[a, b + 1];
            xypoleSmery[x, y, 2] = kostki[a, b + 2];
            xypoleSmery[x, y, 3] = kostki[a, b + 3];
            polozono[a] = true;
            iloscDanychKostek++;
            iloscDanychKostek2++;
            if(lastTime < secondsElapsed)
            {
                lastTime = secondsElapsed;
                this.Invoke((MethodInvoker)delegate
                {
                    label2.Text = "Momentalnie: " + Convert.ToString(node + 1);
                    label2.Update();
                    label3.Text = "Speed: " + iloscDanychKostek + " | " + (int)(iloscDanychKostek2 / (secondsElapsed + 0.01));
                    iloscDanychKostek = 0;
                    label3.Update();
                });
            }
            
            if (node > maxDoTeraz)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    maxDoTeraz = node;
                    label1.Text = "Nejdali: " + Convert.ToString(maxDoTeraz + 1);
                    label1.Update();
                    namalujKostki();
                });
            }
            if (x == porzadi[wielkoscPorzadi - 1, 0] && y == porzadi[wielkoscPorzadi - 1, 1])
            {
                numerRzeszeni++;
            }
            if (node <= wielkoscPorzadi - 2)
            {
                dejKostke(porzadi[node + 1, 0], porzadi[node + 1, 1], node + 1);
            }
            xypole[x, y] = 256;
            xypoleSmery[x, y, 0] = -1;
            xypoleSmery[x, y, 1] = -1;
            xypoleSmery[x, y, 2] = -1;
            xypoleSmery[x, y, 3] = -1;
            polozono[a] = false;
        }

        void dejKostke(int x, int y, int node)
        {
            while (stopped)
            {
                Thread.Sleep(1);
            }
            if (x == 0)
            {
                if (y == 0)
                {
                    int gornoBarwa = 0;
                    int prawoBarwa = polozono[xypole[x + 1, y]] ? xypoleSmery[x + 1, y, 3] : 23;
                    int dolnoBarwa = polozono[xypole[x, y + 1]] ? xypoleSmery[x, y + 1, 0] : 23;
                    int lewoBarwa = 0;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a < 4)
                        {
                            int b = returnTile[tile + 1][i];
                            if (!(rzeszic3x3 && node >= odKielaZkuszac3x3 && !idzieDac3x3(x, y, a, b, node)))
                            {
                                dejKostkeIn(x, y, a, b, node);
                            }
                        }
                    }
                }
                else if (y >= 1 && y <= wyszkaPola - 2)
                {
                    int gornoBarwa = polozono[xypole[x, y - 1]] ? xypoleSmery[x, y - 1, 2] : 23;
                    int prawoBarwa = polozono[xypole[x + 1, y]] ? xypoleSmery[x + 1, y, 3] : 23;
                    int dolnoBarwa = polozono[xypole[x, y + 1]] ? xypoleSmery[x, y + 1, 0] : 23;
                    int lewoBarwa = 0;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a >= 4 && a < 60)
                        {
                            int b = returnTile[tile + 1][i];
                            if (!(rzeszic3x3 && node >= odKielaZkuszac3x3 && !idzieDac3x3(x, y, a, b, node)))
                            {
                                dejKostkeIn(x, y, a, b, node);
                            }
                        }
                    }
                }
                else if (y == wyszkaPola - 1)
                {
                    int gornoBarwa = polozono[xypole[x, y - 1]] ? xypoleSmery[x, y - 1, 2] : 23;
                    int prawoBarwa = polozono[xypole[x + 1, y]] ? xypoleSmery[x + 1, y, 3] : 23;
                    int dolnoBarwa = 0;
                    int lewoBarwa = 0;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a < 4)
                        {
                            int b = returnTile[tile + 1][i];
                            if (!(rzeszic3x3 && node >= odKielaZkuszac3x3 && !idzieDac3x3(x, y, a, b, node)))
                            {
                                dejKostkeIn(x, y, a, b, node);
                            }
                        }
                    }
                }
            }
            else if (x <= szyrzkaPola - 2 && x >= 1)
            {
                if (y == 0)
                {
                    int gornoBarwa = 0;
                    int prawoBarwa = polozono[xypole[x + 1, y]] ? xypoleSmery[x + 1, y, 3] : 23;
                    int dolnoBarwa = polozono[xypole[x, y + 1]] ? xypoleSmery[x, y + 1, 0] : 23;
                    int lewoBarwa = polozono[xypole[x - 1, y]] ? xypoleSmery[x - 1, y, 1] : 23;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a >= 4 && a < 60)
                        {
                            int b = returnTile[tile + 1][i];
                            if (!(rzeszic3x3 && node >= odKielaZkuszac3x3 && !idzieDac3x3(x, y, a, b, node)))
                            {
                                dejKostkeIn(x, y, a, b, node);
                            }
                        }
                    }
                }
                else if (y >= 1 && y <= wyszkaPola - 2)
                {
                    int gornoBarwa = polozono[xypole[x, y - 1]] ? xypoleSmery[x, y - 1, 2] : 23;
                    int prawoBarwa = polozono[xypole[x + 1, y]] ? xypoleSmery[x + 1, y, 3] : 23;
                    int dolnoBarwa = polozono[xypole[x, y + 1]] ? xypoleSmery[x, y + 1, 0] : 23;
                    int lewoBarwa = polozono[xypole[x - 1, y]] ? xypoleSmery[x - 1, y, 1] : 23;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    if (size == 0)
                    {
                        for (int i = 0; i < szyrzka - 1; i++)
                        {
                            graph[i] = graph[i + 1];
                        }
                        graph[szyrzka - 1] = node;
                    }
                    for (int i = 0; i < size; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a >= 60)
                        {
                            int b = returnTile[tile + 1][i];
                            if (!(rzeszic3x3 && node >= odKielaZkuszac3x3 && !idzieDac3x3(x, y, a, b, node)))
                            {
                                dejKostkeIn(x, y, a, b, node);
                            }
                        }
                    }
                }
                else if (y == wyszkaPola - 1)
                {
                    int gornoBarwa = polozono[xypole[x, y - 1]] ? xypoleSmery[x, y - 1, 2] : 23;
                    int prawoBarwa = polozono[xypole[x + 1, y]] ? xypoleSmery[x + 1, y, 3] : 23;
                    int dolnoBarwa = 0;
                    int lewoBarwa = polozono[xypole[x - 1, y]] ? xypoleSmery[x - 1, y, 1] : 23;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a >= 4 && a < 60)
                        {
                            int b = returnTile[tile + 1][i];
                            if (!(rzeszic3x3 && node >= odKielaZkuszac3x3 && !idzieDac3x3(x, y, a, b, node)))
                            {
                                dejKostkeIn(x, y, a, b, node);
                            }
                        }
                    }
                }
            }
            else if (x == szyrzkaPola - 1)
            {
                if (y == 0)
                {
                    int gornoBarwa = 0;
                    int prawoBarwa = 0;
                    int dolnoBarwa = polozono[xypole[x, y + 1]] ? xypoleSmery[x, y + 1, 0] : 23;
                    int lewoBarwa = polozono[xypole[x - 1, y]] ? xypoleSmery[x - 1, y, 1] : 23;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a < 4)
                        {
                            int b = returnTile[tile + 1][i];
                            if (!(rzeszic3x3 && node >= odKielaZkuszac3x3 && !idzieDac3x3(x, y, a, b, node)))
                            {
                                dejKostkeIn(x, y, a, b, node);
                            }
                        }
                    }
                }
                else if (y >= 1 && y <= wyszkaPola - 2)
                {
                    int gornoBarwa = polozono[xypole[x, y - 1]] ? xypoleSmery[x, y - 1, 2] : 23;
                    int prawoBarwa = 0;
                    int dolnoBarwa = polozono[xypole[x, y + 1]] ? xypoleSmery[x, y + 1, 0] : 23;
                    int lewoBarwa = polozono[xypole[x - 1, y]] ? xypoleSmery[x - 1, y, 1] : 23;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a >= 4 && a < 60)
                        {
                            int b = returnTile[tile + 1][i];
                            if (!(rzeszic3x3 && node >= odKielaZkuszac3x3 && !idzieDac3x3(x, y, a, b, node)))
                            {
                                dejKostkeIn(x, y, a, b, node);
                            }
                        }
                    }
                }
                else if (y == wyszkaPola - 1)
                {
                    int gornoBarwa = polozono[xypole[x, y - 1]] ? xypoleSmery[x, y - 1, 2] : 23;
                    int prawoBarwa = 0;
                    int dolnoBarwa = 0;
                    int lewoBarwa = polozono[xypole[x - 1, y]] ? xypoleSmery[x - 1, y, 1] : 23;
                    int tile = b24p3 * gornoBarwa + b24p2 * prawoBarwa + b24p1 * dolnoBarwa + b24p0 * lewoBarwa;
                    int size = returnTile[tile].Count;
                    for (int i = 0; i < size; i++)
                    {
                        int a = returnTile[tile + 0][i];
                        if (!polozono[a] && a < 4)
                        {
                            int b = returnTile[tile + 1][i];
                            if (!(rzeszic3x3 && node >= odKielaZkuszac3x3 && !idzieDac3x3(x, y, a, b, node)))
                            {
                                dejKostkeIn(x, y, a, b, node);
                            }
                        }
                    }
                }
            }
        }

        void changeKostki()
        {
            int zmiana1, zmiana2, zmianao, pomoc;
            for (int i = 0; i < 100; i++)
            {
                zmiana1 = rnd1.Next(4);
                zmiana2 = rnd1.Next(4);
                zmianao = rnd1.Next(4);
                if (!polozono[zmiana1] && !polozono[zmiana2])
                {
                    pomoc = kostkiNowe[zmiana1];
                    kostkiNowe[zmiana1] = kostkiNowe[zmiana2];
                    kostkiNowe[zmiana2] = pomoc;
                    for (int j = zmianao; j < zmianao + 4; j++)
                    {
                        pomoc = kostki[zmiana1, j % 4];
                        kostki[zmiana1, j % 4] = kostki[zmiana2, j % 4];
                        kostki[zmiana2, j % 4] = pomoc;
                    }
                }
            }
            for (int i = 0; i < 10000; i++)
            {
                zmiana1 = rnd1.Next(4, 60);
                zmiana2 = rnd1.Next(4, 60);
                zmianao = rnd1.Next(4);
                if (!polozono[zmiana1] && !polozono[zmiana2])
                {
                    pomoc = kostkiNowe[zmiana1];
                    kostkiNowe[zmiana1] = kostkiNowe[zmiana2];
                    kostkiNowe[zmiana2] = pomoc;
                    for (int j = zmianao; j < zmianao + 4; j++)
                    {
                        pomoc = kostki[zmiana1, j % 4];
                        kostki[zmiana1, j % 4] = kostki[zmiana2, j % 4];
                        kostki[zmiana2, j % 4] = pomoc;
                    }
                }
            }
            for (int i = 0; i < 10000; i++)
            {
                zmiana1 = rnd1.Next(60, 256);
                zmiana2 = rnd1.Next(60, 256);
                zmianao = rnd1.Next(4);
                if (!polozono[zmiana1] && !polozono[zmiana2])
                {
                    pomoc = kostkiNowe[zmiana1];
                    kostkiNowe[zmiana1] = kostkiNowe[zmiana2];
                    kostkiNowe[zmiana2] = pomoc;
                    for (int j = zmianao; j < zmianao + 4; j++)
                    {
                        pomoc = kostki[zmiana1, j % 4];
                        kostki[zmiana1, j % 4] = kostki[zmiana2, j % 4];
                        kostki[zmiana2, j % 4] = pomoc;
                    }
                }
            }
        }

        void zapiszZabraneKostkiZTxt()
        {
            xypole[7, 8] = 139 - 1;
            xypoleSmery[7, 8, 0] = kostki[139 - 1, 2];
            xypoleSmery[7, 8, 1] = kostki[139 - 1, 3];
            xypoleSmery[7, 8, 2] = kostki[139 - 1, 0];
            xypoleSmery[7, 8, 3] = kostki[139 - 1, 1];
            polozono[139 - 1] = true;
            xypole[13, 2] = 255 - 1;
            xypoleSmery[13, 2, 0] = kostki[255 - 1, 1];
            xypoleSmery[13, 2, 1] = kostki[255 - 1, 2];
            xypoleSmery[13, 2, 2] = kostki[255 - 1, 3];
            xypoleSmery[13, 2, 3] = kostki[255 - 1, 0];
            polozono[255 - 1] = true;
            xypole[2, 13] = 181 - 1;
            xypoleSmery[2, 13, 0] = kostki[181 - 1, 1];
            xypoleSmery[2, 13, 1] = kostki[181 - 1, 2];
            xypoleSmery[2, 13, 2] = kostki[181 - 1, 3];
            xypoleSmery[2, 13, 3] = kostki[181 - 1, 0];
            polozono[181 - 1] = true;
            xypole[13, 13] = 249 - 1;
            xypoleSmery[13, 13, 0] = kostki[249 - 1, 0];
            xypoleSmery[13, 13, 1] = kostki[249 - 1, 1];
            xypoleSmery[13, 13, 2] = kostki[249 - 1, 2];
            xypoleSmery[13, 13, 3] = kostki[249 - 1, 3];
            polozono[249 - 1] = true;
            xypole[2, 2] = 208 - 1;
            xypoleSmery[2, 2, 0] = kostki[208 - 1, 1];
            xypoleSmery[2, 2, 1] = kostki[208 - 1, 2];
            xypoleSmery[2, 2, 2] = kostki[208 - 1, 3];
            xypoleSmery[2, 2, 3] = kostki[208 - 1, 0];
            polozono[208 - 1] = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
            reset();
            zapiszPorzadi();
            zapiszZabraneKostkiZTxt();
            changeKostki();
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    kostki[i, j + 4] = kostki[i, j];
                }
            }
            returnTile = new List<int>[24 * 24 * 24 * 24 * 2];
            //initializing returnTile
            for (int i = 0; i < 24 * 24 * 24 * 24 * 2; i++)
            {
                returnTile[i] = new List<int>();
            }
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int s1 = kostki[i, j];
                    int s2 = kostki[i, j + 1];
                    int s3 = kostki[i, j + 2];
                    int s4 = kostki[i, j + 3];
                    //s1,23,23,23
                    returnTile[s1 * b24p3 + 23 * b24p2 + 23 * b24p1 + 23 * b24p0 + 0].Add(i);
                    returnTile[s1 * b24p3 + 23 * b24p2 + 23 * b24p1 + 23 * b24p0 + 1].Add(j);
                    //23,s2,23,23
                    returnTile[23 * b24p3 + s2 * b24p2 + 23 * b24p1 + 23 * b24p0 + 0].Add(i);
                    returnTile[23 * b24p3 + s2 * b24p2 + 23 * b24p1 + 23 * b24p0 + 1].Add(j);
                    //23,23,s3,23
                    returnTile[23 * b24p3 + 23 * b24p2 + s3 * b24p1 + 23 * b24p0 + 0].Add(i);
                    returnTile[23 * b24p3 + 23 * b24p2 + s3 * b24p1 + 23 * b24p0 + 1].Add(j);
                    //23,23,23,s4
                    returnTile[23 * b24p3 + 23 * b24p2 + 23 * b24p1 + s4 * b24p0 + 0].Add(i);
                    returnTile[23 * b24p3 + 23 * b24p2 + 23 * b24p1 + s4 * b24p0 + 1].Add(j);
                    //s1,s2,23,23
                    returnTile[s1 * b24p3 + s2 * b24p2 + 23 * b24p1 + 23 * b24p0 + 0].Add(i);
                    returnTile[s1 * b24p3 + s2 * b24p2 + 23 * b24p1 + 23 * b24p0 + 1].Add(j);
                    //s1,23,s3,23
                    returnTile[s1 * b24p3 + 23 * b24p2 + s3 * b24p1 + 23 * b24p0 + 0].Add(i);
                    returnTile[s1 * b24p3 + 23 * b24p2 + s3 * b24p1 + 23 * b24p0 + 1].Add(j);
                    //s1,23,23,s4
                    returnTile[s1 * b24p3 + 23 * b24p2 + 23 * b24p1 + s4 * b24p0 + 0].Add(i);
                    returnTile[s1 * b24p3 + 23 * b24p2 + 23 * b24p1 + s4 * b24p0 + 1].Add(j);
                    //23,s2,s3,23
                    returnTile[23 * b24p3 + s2 * b24p2 + s3 * b24p1 + 23 * b24p0 + 0].Add(i);
                    returnTile[23 * b24p3 + s2 * b24p2 + s3 * b24p1 + 23 * b24p0 + 1].Add(j);
                    //23,s2,23,s4
                    returnTile[23 * b24p3 + s2 * b24p2 + 23 * b24p1 + s4 * b24p0 + 0].Add(i);
                    returnTile[23 * b24p3 + s2 * b24p2 + 23 * b24p1 + s4 * b24p0 + 1].Add(j);
                    //23,23,s3,s3
                    returnTile[23 * b24p3 + 23 * b24p2 + s3 * b24p1 + s4 * b24p0 + 0].Add(i);
                    returnTile[23 * b24p3 + 23 * b24p2 + s3 * b24p1 + s4 * b24p0 + 1].Add(j);
                    //s1,s2,s3,23
                    returnTile[s1 * b24p3 + s2 * b24p2 + s3 * b24p1 + 23 * b24p0 + 0].Add(i);
                    returnTile[s1 * b24p3 + s2 * b24p2 + s3 * b24p1 + 23 * b24p0 + 1].Add(j);
                    //s1,s2,23,s4
                    returnTile[s1 * b24p3 + s2 * b24p2 + 23 * b24p1 + s4 * b24p0 + 0].Add(i);
                    returnTile[s1 * b24p3 + s2 * b24p2 + 23 * b24p1 + s4 * b24p0 + 1].Add(j);
                    //s1,23,s3,s4
                    returnTile[s1 * b24p3 + 23 * b24p2 + s3 * b24p1 + s4 * b24p0 + 0].Add(i);
                    returnTile[s1 * b24p3 + 23 * b24p2 + s3 * b24p1 + s4 * b24p0 + 1].Add(j);
                    //23,s2,s3,s4
                    returnTile[23 * b24p3 + s2 * b24p2 + s3 * b24p1 + s4 * b24p0 + 0].Add(i);
                    returnTile[23 * b24p3 + s2 * b24p2 + s3 * b24p1 + s4 * b24p0 + 1].Add(j);
                    //s1,s2,s3,s4
                    returnTile[s1 * b24p3 + s2 * b24p2 + s3 * b24p1 + s4 * b24p0 + 0].Add(i);
                    returnTile[s1 * b24p3 + s2 * b24p2 + s3 * b24p1 + s4 * b24p0 + 1].Add(j);
                    //23,23,23,23
                    returnTile[23 * b24p3 + 23 * b24p2 + 23 * b24p1 + 23 * b24p0 + 0].Add(i);
                    returnTile[23 * b24p3 + 23 * b24p2 + 23 * b24p1 + 23 * b24p0 + 1].Add(j);
                }
            }
            odKielaZkuszac3x3 = 0 - 2;
            szyrzka = pictureBox1.Width;
            wyszka = pictureBox1.Height;
            graph = new int[szyrzka];
            for (int i = 0; i < szyrzka; i++)
            {
                graph[i] = 0;
            }
            bmp = new Bitmap(szyrzka, wyszka);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            pictureBox1.Image = bmp;
            mainThread = new Thread(mainFunction);
            mainThread.IsBackground = true;
        }

        public void mainFunction()
        {
            dejKostke(porzadi[0, 0], porzadi[0, 1], 0);
            namalujKostki();
            pictureBox1.Image = bmp;
        }

        //PAUSE
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            stopped = true;
            button2.Enabled = true;
            button1.Enabled = false;
        }

        //START
        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            button3.Enabled = false;
            if (checkBox1.Checked) { rzeszic3x3 = true; }
            else { rzeszic3x3 = false; }
            checkBox1.Enabled = false;
            szyrzka = pictureBox1.Width;
            pictureBox1.Height = szyrzka;
            bmp = new Bitmap(szyrzka, wyszka);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            namalujKostki();
            mainThread.Start();
        }

        //RESUME
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            stopped = false;
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            secondsElapsed++;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            stopped = true;
            button2.Enabled = false;
            button1.Enabled = false;
            label3.Text = "Closing...";
            label3.Update();
            mainThread.Abort();
            Thread.Sleep(50);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Bitmap bmp;

        private Graphics g;

        private List<Point> points = new List<Point>();

        private List<Point> hull = new List<Point> ();

        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);
            pictureBox1.Image = bmp;
        }

        private void draw_point(int x, int y)
        {
            g.DrawEllipse(new Pen(Color.Black, 4), x, y, 2, 2);
            pictureBox1.Image = bmp;
        }

        private int Side(Point p1, Point p2, Point p)
        {
            int val = (p.Y - p1.Y) * (p2.X - p1.X) -
                      (p2.Y - p1.Y) * (p.X - p1.X);

            if (val > 0)
                return 1;
            if (val < 0)
                return -1;
            return 0;
        }

        private int Distance(Point p1, Point p2, Point p)
        {
            return Math.Abs((p.Y - p1.Y) * (p2.X - p1.X) - (p2.Y - p1.Y) * (p.X - p1.X));
        }

        private void QuickHull()
        {

            if (points.Count <= 3)
            {
                foreach (var p in points)
                {
                    hull.Add(p);
                }
                return;
            }

            Point pmin = points
                .Select(p => new { point = p, x = p.X })
                .Aggregate((p1, p2) => p1.x < p2.x ? p1 : p2).point;

            Point pmax = points
                .Select(p => new { point = p, x = p.X })
                .Aggregate((p1, p2) => p1.x > p2.x ? p1 : p2).point;

            hull.Add(pmin);
            hull.Add(pmax);

            List<Point> left = new List<Point>();
            List<Point> right = new List<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];
                if (Side(pmin, pmax, p) == 1)
                    left.Add(p);
                else
                if (Side(pmin, pmax, p) == -1)
                    right.Add(p);
            }
            CreateHull(pmin, pmax, left);
            CreateHull(pmax, pmin, right);
        }

        private void CreateHull(Point a, Point b, List<Point> points)
        {
            int pos = hull.IndexOf(b);

            if (points.Count == 0)
                return;

            if (points.Count == 1)
            {
                Point pp = points[0];
                hull.Insert(pos, pp);
                return;
            }

            int dist = int.MinValue;
            int point = 0;

            for (int i = 0; i < points.Count; i++)
            {
                Point pp = points[i];
                int distance = Distance(a, b, pp);
                if (distance > dist)
                {
                    dist = distance;
                    point = i;
                }
            }

            Point p = points[point];
            hull.Insert(pos, p);

            List<Point> ap = new List<Point>();
            List<Point> pb = new List<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                Point pp = points[i];
                if (Side(a, p, pp) == 1)
                {
                    ap.Add(pp);
                }
            }

            for (int i = 0; i < points.Count; i++)
            {
                Point pp = points[i];
                if (Side(p, b, pp) == 1)
                {
                    pb.Add(pp);
                }
            }
            CreateHull(a, p, ap);
            CreateHull(p, b, pb);
        }

        private void draw_area()
        {
            if (points.Count == 1)
                return;

            if (points.Count == 2)
            {
                g.DrawLine(new Pen(Color.Red, 3), points[0], points[1]);
            }

            if (points.Count == 3)
            {
                g.DrawLine(new Pen(Color.Red, 3), points[0], points[1]);
                g.DrawLine(new Pen(Color.Red, 3), points[0], points[2]);
                g.DrawLine(new Pen(Color.Red, 3), points[1], points[2]);
            }

            if (points.Count > 3)
            {
                hull.Clear();
                bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                g = Graphics.FromImage(bmp);
                pictureBox1.Image = bmp;
                foreach (var p in points)
                {
                    draw_point(p.X, p.Y);
                }
                QuickHull();
                g.DrawPolygon(new Pen(Color.Red, 2), hull.ToArray());
                pictureBox1.Image = bmp;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            draw_point(e.Location.X, e.Location.Y);
            points.Add(e.Location);
            draw_area();
        }
    }
}

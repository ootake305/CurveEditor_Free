using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Windows.Input;


namespace CurveEditor
{
    //中心の線を引くためのポイント
    struct StandardPoint
    {
        public Point startPoint;//開始点
        public  Point endPoint;//終了点
        public Pen pen ;//線の色
    }

    public partial class Form1 : Form
    {
        StandardPoint m_standartpoint;
        const float cpSize = 15;
        Point[] Points;
        Point[] Points2;
        Point[] Points3;
        Point point1 = new Point(50, 300);
        Point point2 = new Point(150, 150);
        Point point3 = new Point(300, 400);
        Point point4 = new Point(450, 150);
        Point point5 = new Point(550, 250);
        Point point6 = new Point(600, 350);
        Point point7 = new Point(750, 450);
        Point point8 = new Point(450, 150);
        Pen pen = new Pen(Color.White, 1);
        Brush brush = new SolidBrush(Color.FromArgb(160, 255, 0, 0));
        int moveIndex = 0;
        bool moveFlag = false;
        GraphicsPath path = new GraphicsPath();
        public Form1()
        {
           
            InitializeComponent();
            Text = "DrawBezier";
            Points = new Point[] { point1, point2 , point3, point4 };
            Points2 = new Point[] { point8, point5, point6, point7 };
            Points3 = new Point[] { point3, point4 };
            InitPointLocationLabel();
            SetStyle(
    ControlStyles.DoubleBuffer |
    ControlStyles.UserPaint |
    ControlStyles.AllPaintingInWmPaint, true);
            　
            StandartPointInit();
        }
        //中心の線を引くためのポイント初期化
        public void StandartPointInit()
        {
            m_standartpoint.startPoint.X = 0;
            m_standartpoint.startPoint.Y = 150;
            m_standartpoint.endPoint.X = 600;
            m_standartpoint.endPoint.Y = 150;
            m_standartpoint.pen = new Pen(Color.FromArgb(100, 200, 200, 200), 1);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < Points.Length; i++)
            {
                if (e.X >= Points[i].X - cpSize / 2 && e.X < Points[i].X + cpSize / 2)
                {
                    if (e.Y >= Points[i].Y - cpSize / 2 && e.Y < Points[i].Y + cpSize / 2)
                    {
                        moveIndex = i;
                        moveFlag = true;
                    }
                }
            }

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            moveFlag = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveFlag)
            {
                if (e.X < 1000)
                {
                    Points[moveIndex].X = e.X;
                }
                else
                {
                    Points[moveIndex].X = 1000;
                }

                Points[moveIndex].Y = e.Y;

              /*  Control cx = Controls[moveIndex + "X"];
                cx.Text = Points[moveIndex].X.ToString();
                Control cy = Controls[moveIndex + "Y"];
                cy.Text = Points[moveIndex].Y.ToString();
                */
                Invalidate();
                Refresh();
            }
          
        }
        private void InitPointLocationLabel()
        {
         /*   for (int i = 0; i < Points.Length; i++)
            {
                Label l = new Label();
                l.Location = new Point(100, 20 * i + 400);
                l.Size = new Size(64, 20);
                l.Text = "Point" + i;
                Controls.Add(l);
                Label l2 = new Label();
                l2.Name = i + "X";
                l2.Text = Points[i].X.ToString();
                l2.Location = new Point(164, 20 * i + 400);
                l2.Size = new Size(64, 20);
                Controls.Add(l2);
                Label l3 = new Label();
                l3.Name = i + "Y";
                l3.Text = Points[i].X.ToString();
                l3.Location = new Point(228, 20 * i + 400);
                l3.Size = new Size(64, 20);
                Controls.Add(l3);

            }*/

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
        //3次ベジェ曲線を結ぶ点描画
        private void PointrPaint(PaintEventArgs e)
        {

        }
        //選択している点の制御点描画
        private void ControlPaint(PaintEventArgs e)
        {

        }
        //3次ベジェ曲線描画
        private void BezierPaint( PaintEventArgs e)
        {

        }
        private void TestPaint(object sender, PaintEventArgs e)
        {
       
            Graphics g = e.Graphics;
          //  g.Clear(Color.FromArgb(20, 230, 230, 230));
            path.Reset();
            //中心の線の描画
            g.DrawLine(m_standartpoint.pen, m_standartpoint.startPoint, m_standartpoint.endPoint);

            //曲線の描画
            path.AddBeziers(Points);
            path.AddBeziers(Points2);

           //点の描画
            for (int i = 0; i < Points.Length; i++)
            {
                e.Graphics.FillRectangle(brush, Points[i].X - cpSize / 2, Points[i].Y - cpSize / 2, cpSize, cpSize);
            }
            for (int i = 0; i < Points2.Length; i++)
            {
                e.Graphics.FillRectangle(brush, Points2[i].X - cpSize / 2, Points2[i].Y - cpSize / 2, cpSize, cpSize);
            }
            e.Graphics.DrawPath(pen, path);
            BezierPaint(e);    //3次ベジェ曲線描画
            ControlPaint(e);   //選択している点の制御点描画
            PointrPaint(e);    //3次ベジェ曲線を結ぶ点描画
        }
    }

}

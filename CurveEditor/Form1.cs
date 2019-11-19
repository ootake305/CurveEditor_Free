﻿using System;
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


    public partial class Form1 : Form
    {
        /// <summary>
        /// 線
        /// </summary>
        CenterLine m_CenterLine;
        TopLine m_TopLine;
        BottomLine m_BottomLine;

        const float cpSize = 8;
        Point[] Points;
        Point[] Points2;
        Point[] Points3;
        Point point1 = new Point(50, 300);
        Point point2 = new Point(150, 150);
        Point point3 = new Point(300, 200);
        Point point4 = new Point(450, 150);
        Point point5 = new Point(550, 250);
        Point point6 = new Point(600, 350);
        Point point7 = new Point(750, 450);
        Point point8 = new Point(450, 150);

        int moveIndex = 0;


        CurvePointControl m_CurvePointControl;
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
            m_CurvePointControl = new CurvePointControl();
        }
        //中心の線を引くためのポイント初期化
        public void StandartPointInit()
        {
            m_CenterLine = new CenterLine();
            m_TopLine = new TopLine();
            m_BottomLine = new BottomLine();
            m_CenterLine.Init();
            m_TopLine.Init();
            m_BottomLine.Init();
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

                    }
                }
            }
            m_CurvePointControl.SearchSelectPoint(e);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            m_CurvePointControl.CancelSelectPoint();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Invalidate();
            Refresh();
            m_CurvePointControl.MovePoint(e);
                  
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
            m_CurvePointControl.SelectPointrPaint(e);
            m_CurvePointControl.PointrPaint(e);
        }
        //選択している点の制御点描画
        private void ControlPaint(PaintEventArgs e)
        {
            m_CurvePointControl.ControlPaint(e);
        }
        //3次ベジェ曲線描画
        private void BezierPaint( PaintEventArgs e)
        {
            m_CurvePointControl.BezierPaint(e);
        }
        /// <summary>
        /// 線描画
        /// </summary>
        /// <param name="g"></param>
        private void LinePaint(Graphics g)
        {
            m_CenterLine.Paint(g);
            m_TopLine.Paint(g);
            m_BottomLine.Paint(g);
        }

        /// <summary>
        /// pictureBox内の描画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestPaint(object sender, PaintEventArgs e)
        {
       
            Graphics g = e.Graphics;
           g.Clear(Color.FromArgb(20, 230, 230, 230));

            //線の描画
            LinePaint(g);

            BezierPaint(e);    //3次ベジェ曲線描画
            ControlPaint(e);   //選択している点の制御点描画
            PointrPaint(e);    //3次ベジェ曲線を結ぶ点描画
        }
    }

}

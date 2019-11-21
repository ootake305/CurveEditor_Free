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


    public partial class Form1 : Form
    {
        /// <summary>
        /// 直線
        /// </summary>
        CenterLine m_CenterLine = new CenterLine();
        TopLine m_TopLine = new TopLine();
        BottomLine m_BottomLine = new BottomLine();
        VerticalLine m_VerticalLeftLine = new VerticalLine(true);
        VerticalLine m_VerticalRightLine = new VerticalLine(false);
        /// <summary>
        /// 曲線
        /// </summary>
        CurvePointControl m_CurvePointControl;

        public Form1()
        {
           
            InitializeComponent();
            Text = "DrawBezier";
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
            m_CenterLine.Init();
            m_TopLine.Init();
            m_BottomLine.Init();
            m_VerticalLeftLine.Init();
            m_VerticalRightLine.Init();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

            m_CurvePointControl.SearchSelectPoint(e);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            m_CurvePointControl.CancelMovePoint();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            m_CurvePointControl.MovePoint(e);
            Invalidate();
            Refresh();//再描画
        }  
        /// <summary>
        /// 3次ベジェ曲線を結ぶ点描画
        /// </summary>
        /// <param name="e"></param>
        private void PointrPaint(PaintEventArgs e)
        {
            m_CurvePointControl.SelectPointrPaint(e);
            m_CurvePointControl.PointrPaint(e);
        }
        /// <summary>
        /// 選択している点の制御点描画
        /// </summary>
        /// <param name="e"></param>
        private void ControlPaint(PaintEventArgs e)
        {
            m_CurvePointControl.ControlPaint(e);
        }
        /// <summary>
        /// 3次ベジェ曲線描画
        /// </summary>
        /// <param name="e"></param>
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
            m_VerticalLeftLine.Paint(g);
            m_VerticalRightLine.Paint(g);
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
        //点追加ボタンクリック
        private void button2_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.AddPoint();
            Refresh();//再描画
        }
    }

}

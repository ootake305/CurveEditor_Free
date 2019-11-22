using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        VerticalEndLine m_VerticalLeftLine = new VerticalEndLine(true);
        VerticalEndLine m_VerticalRightLine = new VerticalEndLine(false);

        VerticalLine[] m_VerticalCenterLines = new VerticalLine[5];
        /// <summary>
        /// 曲線
        /// </summary>
        CurvePointControl m_CurvePointControl = new CurvePointControl();

        int m_MinNum;//最小値
        int m_MaxNum;  //最大値

        const int ScrrenCenterpPosY = 160;  //中央
        Point m_MousePos;
        public Form1()
        {
            InitializeComponent();
            Text = "CurveEditor ver:0.5 α版";
            //ちらつき防止
            SetStyle(
    ControlStyles.DoubleBuffer |
    ControlStyles.UserPaint |
    ControlStyles.AllPaintingInWmPaint, true);
            　
            StandartPointInit();
            KeyPreview = true;//キー入力有効化
            //入力装置の初期化
            numericUpDownInit();
        }
        //中心の線を引くためのポイント初期化
        public void StandartPointInit()
        {
            m_CenterLine.Init();
            m_TopLine.Init();
            m_BottomLine.Init();
            m_VerticalLeftLine.Init();
            m_VerticalRightLine.Init();
            for(int i = 0; i < 5; i++)
            {
                m_VerticalCenterLines[i] = new VerticalLine((i + 1) * 100 );
                m_VerticalCenterLines[i].Init();
            }
         
        }
        public void numericUpDownInit()
        {
            numericUpDown8.ValueChanged += new EventHandler(ChangeFirstStartPoint);
            numericUpDown8.Value = ScrrenCenterpPosY;
            ChangeFirstStartPoint(null,null);
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    m_CurvePointControl.SearchSelectPoint(e);
                    SaveMousePos(e);
                    break;
                case MouseButtons.Middle:
                    break;
                case MouseButtons.Right:
                    break;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    m_CurvePointControl.CancelMovePoint();
                    break;
                case MouseButtons.Middle:
                    break;
                case MouseButtons.Right:
                    break;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            m_CurvePointControl.MovePoint(e);
          //  Invalidate();重くなるのでいらない
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
            for (int i = 0; i < 5; i++)
            {
                m_VerticalCenterLines[i].Paint(g);
            }
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

            m_CurvePointControl.OrganizeControlPoint(); //制御点の整理
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
        //点追加ダブルクリック時呼びだす
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            m_CurvePointControl.AddPoint(m_MousePos);
            Refresh();//再描画
        }
        //点削除ボタンクリック
        private void button1_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.DeletePoint();
            Refresh();//再描画
        }
        //キーを押した際のイベント
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //デリートキー押したら
            if (e.KeyCode == Keys.Delete)
             {
                 m_CurvePointControl.DeletePoint();//点削除
                 Refresh();//再描画
             }
            //エスケープキーを押したら
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    

        public void ChangeFirstStartPoint(object sender, EventArgs e)
        {
            int  num= Convert.ToInt32(numericUpDown8.Value);
            numericUpDown8.Value =  m_CurvePointControl.SetFirstStartPoint(num);
            Refresh();//再描画
        }
        /// <summary>
        ///  picture内のマウス座標を保存
        /// </summary>
        public void SaveMousePos(MouseEventArgs e)
        {    
            Point p = new Point();
            p.X = e.X;
            p.Y = e.Y;
            m_MousePos = p;
        }
    }

}

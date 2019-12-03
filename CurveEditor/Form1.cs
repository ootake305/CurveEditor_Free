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

        const int LineNum = 9;
        VerticalLine[] m_VerticalCenterLines = new VerticalLine[LineNum];
        SideLine[] m_SideLine = new SideLine[LineNum];
        /// <summary>
        /// 曲線
        /// </summary>
        CurvePointControl m_CurvePointControl = new CurvePointControl();

        const int ScrrenCenterpPosY = 162;  //中央
        const int ScrrenTopPosY = 10;      //上端
        const int ScrrenBottomPosY = 510;   //下端
        Point m_MousePos;//一時保存用マウスの座標
        CurvePointControl.BezierPoint bp;//一時保存用
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

            LavelInit();
            pictureBox1.ContextMenuStrip = contextMenuStrip1;
            checkBox1.Checked = true;
        }
        //中心の線を引くためのポイント初期化
        public void StandartPointInit()
        {
            m_CenterLine.Init();
            m_TopLine.Init();
            m_BottomLine.Init();
            m_VerticalLeftLine.Init();
            m_VerticalRightLine.Init();
            for(int i = 0; i < LineNum; i++)
            {
                //縦線
                m_VerticalCenterLines[i] = new VerticalLine((i + 1) * 50  + 10);
                m_VerticalCenterLines[i].Init();
                //横線
                m_SideLine[i] = new SideLine((i + 1) * 50  + 10);
                m_SideLine[i].Init();
            }
         
        }
        //入力項目の初期化
        public void numericUpDownInit()
        {
            //値が変わった時の呼び出されるメソッド設定
            numericUpDown1.ValueChanged += new EventHandler(ChangeSelectPointX);
            numericUpDown2.ValueChanged += new EventHandler(ChangeControlPoint1X);
            numericUpDown3.ValueChanged += new EventHandler(ChangeControlPoint2X);
            numericUpDown4.ValueChanged += new EventHandler(ChangeSelectPointY);
            numericUpDown5.ValueChanged += new EventHandler(ChangeControlPoint1Y);
            numericUpDown6.ValueChanged += new EventHandler(ChangeControlPoint2Y);
            numericUpDown7.ValueChanged += new EventHandler(ChangeEndPoint);
            numericUpDown7.Value = ScrrenTopPosY;
            numericUpDown8.ValueChanged += new EventHandler(ChangeFirstStartPoint);
            numericUpDown8.Value = ScrrenBottomPosY;
            ChangeFirstStartPoint(null,null);
            ChangeEndPoint(null, null);
        }

        /// <summary>
        /// 背景を透過させるための初期化
        /// </summary>
        public void LavelInit()
        {
            pictureBox1.Controls.Add(label10);
            pictureBox1.Controls.Add(label11);
            pictureBox1.Controls.Add(label12);
            pictureBox1.Controls.Add(label13);
            pictureBox1.Controls.Add(label14);
            pictureBox1.Controls.Add(label15);
            pictureBox1.Controls.Add(label16);
            pictureBox1.Controls.Add(label17);
        }
        /// <summary>
        /// 点を動かした際同期を取る
        /// </summary>
        public void numericUpDownSync()
        {
            bp = m_CurvePointControl.GetBezierPoint();
            numericUpDown1.Value = ChageDecimalPosX(bp.startPoint.X);
            numericUpDown2.Value = ChageDecimalPosX(bp.controlPoint1.X);
            numericUpDown3.Value = ChageDecimalPosX(bp.controlPoint2.X);
            numericUpDown4.Value = bp.startPoint.Y;
            numericUpDown5.Value = bp.controlPoint1.Y;
            numericUpDown6.Value = bp.controlPoint2.Y;
            numericUpDown7.Value = m_CurvePointControl.GetEndPointY();
            numericUpDown8.Value = m_CurvePointControl.GetFirstStartPointY();
        }
        /// <summary>
        /// クリックした瞬間の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    SaveMousePos(e);
                    break;
            }
        }
        /// <summary>
        /// クリックを終えた瞬間の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
               
                      m_CurvePointControl.CancelMovePoint();
                    if (!m_CurvePointControl.isSelectPoint())
                    {
                        //入力項目のリセット
                        ResetCurvePointValue();
                    }
                    break;
                case MouseButtons.Middle:
                    break;
                case MouseButtons.Right:
                    break;
            }
        }
        /// <summary>
        /// マウスを動かしてる間の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            m_CurvePointControl.MovePoint(e);
            numericUpDownSync();
          //  Invalidate();重くなるのでいらない
            pictureBox1.Refresh();//再描画
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
            for (int i = 0; i < 9; i++)
            {
                m_VerticalCenterLines[i].Paint(g);
                m_SideLine[i].Paint(g);
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
            if (checkBox1.Checked)
            {
                ControlPaint(e);   //選択している点の制御点描画
                PointrPaint(e);    //3次ベジェ曲線を結ぶ点描画
            }
        }
        //点追加ボタンクリック
        private void button2_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.AddPoint();
            pictureBox1.Refresh();//再描画
        }
        //点追加ダブルクリック時呼びだす
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            m_CurvePointControl.AddPoint(m_MousePos);
            pictureBox1.Refresh();//再描画
        }
        //点削除ボタンクリック
        private void button1_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.DeletePoint();
            pictureBox1.Refresh();//再描画
        }
        //キーを押した際のイベント
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //デリートキー押したら
            if (e.KeyCode == Keys.Delete)
             {
                 m_CurvePointControl.DeletePoint();//点削除
                 pictureBox1.Refresh();//再描画
             }
            //エスケープキーを押したら
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
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
        /// <summary>
        /// 選択点X
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeSelectPointX(object sender, EventArgs e)
        {
            int num3 = ChageNomalPosX(numericUpDown1.Value);
            int num4 = m_CurvePointControl.SetStartPointX(num3);
            numericUpDown1.Value = ChageDecimalPosX(num4); 
             if (!m_CurvePointControl.isMoveSelectPoint()) pictureBox1.Refresh();//再描画
        }
        /// <summary>
        /// 選択点Y
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeSelectPointY(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(numericUpDown4.Value);
            numericUpDown4.Value = m_CurvePointControl.SetStartPointY(num);
            if (!m_CurvePointControl.isMoveSelectPoint()) pictureBox1.Refresh();//再描画
            //最初の開始点と同期を取る
            numericUpDown8.Value = m_CurvePointControl.GetFirstStartPointY();
        }
        /// <summary>
        /// 制御0点1X
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeControlPoint1X(object sender, EventArgs e)
        {
            int num = ChageNomalPosX(numericUpDown2.Value);
            int num2 = m_CurvePointControl.SetControl1PointX(num);
            numericUpDown2.Value =ChageDecimalPosX(num2);
             if (!m_CurvePointControl.isMoveSelectPoint())  pictureBox1.Refresh();//再描画
        }
        /// <summary>
        /// 制御0点1Y
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeControlPoint1Y(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(numericUpDown5.Value);
            numericUpDown5.Value = m_CurvePointControl.SetControl1PointY(num);
            if (!m_CurvePointControl.isMoveSelectPoint())  pictureBox1.Refresh();//再描画
        }
        /// <summary>
        /// 制御0点2X
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeControlPoint2X(object sender, EventArgs e)
        {
            int num = ChageNomalPosX(numericUpDown3.Value);
            int num2 = m_CurvePointControl.SetControl2PointX(num);
            numericUpDown3.Value = ChageDecimalPosX(num2);
            if (!m_CurvePointControl.isMoveSelectPoint())  pictureBox1.Refresh();//再描画
        }
        /// <summary>
        /// 制御0点2Y
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeControlPoint2Y(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(numericUpDown6.Value);
            numericUpDown6.Value = m_CurvePointControl.SetControl2PointY(num);
            if (!m_CurvePointControl.isMoveSelectPoint()) pictureBox1.Refresh();//再描画
        }
        //開始点せってい
        public void ChangeFirstStartPoint(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(numericUpDown8.Value);
            numericUpDown8.Value = m_CurvePointControl.SetFirstStartPoint(num);
            if (!m_CurvePointControl.isMoveSelectPoint()) pictureBox1.Refresh();//再描画
            bp = m_CurvePointControl.GetBezierPoint();
            numericUpDown4.Value = bp.startPoint.Y;
        }
        /// <summary>
        /// 終了点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeEndPoint(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(numericUpDown7.Value);
            numericUpDown7.Value = m_CurvePointControl.SetEndPoint(num);
            if (!m_CurvePointControl.isMoveSelectPoint()) pictureBox1.Refresh();//再描画
            bp = m_CurvePointControl.GetBezierPoint();
        }

        //カーブポイント入力項目のリセット
        public void ResetCurvePointValue()
        {
              numericUpDown1.Value = 0;
              numericUpDown2.Value = 0;
              numericUpDown3.Value = 0;
              numericUpDown4.Value = 0;
              numericUpDown5.Value = 0;
              numericUpDown6.Value = 0;
        }

        public int OriginalPosToLimitPosX()
        {
            return 0;
        }

        public int OriginalPosToLimitPosY()
        {
            return 0;
        }

        public int LimitPosToOriginalPosX()
        {
            return 0;
        }

        public int LimitPosToOriginalPosY()
        {
            return 0;
        }
        //小数から元の座標に変換
        public int ChageNomalPosX(decimal Value)
        {
            decimal num = Value;

            decimal num2 = (num * (decimal)500);
            int num3 = Convert.ToInt32(num2);

            num3 += 10;//10からグラフが始まってるので右に+10
            return num3;
        }
        //元の座標から0～1の間に変換
        public decimal ChageDecimalPosX(int posX)
        {
            posX = Math.Max(0, posX - 10);　//10からグラフが始まってるので右に-10
            return (decimal)posX / (decimal)500;
        }
        //右クリック動作----------------------------------------------------------------
        private void AddPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.AddPoint(m_MousePos);
            pictureBox1.Refresh();//再描画
        }

        private void DeletePointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.DeletePoint();//点削除
            pictureBox1.Refresh();//再描画
        }
        //チェックボックスの値が変化したときに呼ばれる
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Refresh();//再描画
        }
    }
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

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
        PointRender m_PointRender = new PointRender();
        const int ScrrenCenterpPosY = 162;  //中央
        const int ScrrenTopPosY = 10;      //上端
        const int ScrrenBottomPosY = 510;   //下端
        Point m_MousePos;//一時保存用マウスの座標
        CurvePointControl.BezierPoint bp;//一時保存用
        private string editFilePath = "";  //編集中のファイルのパス
        //データの読み書きが早くてカーソルの変更がわからないので待機時間を設ける
        const int WaitTime = 300;
        public Form1()
        {
            InitializeComponent();
            Text = "CurveEditor ver:0.9";
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
            numericUpDown7.Value = 1;
            numericUpDown8.ValueChanged += new EventHandler(ChangeFirstStartPoint);
            numericUpDown8.Value = 0;
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
            pictureBox1.Controls.Add(label18);
            pictureBox1.Controls.Add(label19);
            pictureBox1.Controls.Add(label20);
            pictureBox1.Controls.Add(label21);
        }
        /// <summary>
        /// 点を動かした際同期を取る
        /// </summary>
        public void numericUpDownSync()
        {
            bp = m_CurvePointControl.GetBezierPoint();
            numericUpDown1.Value = CMath.ChageDecimalPosX(bp.startPoint.X);
            numericUpDown2.Value = CMath.ChageDecimalPosX(bp.controlPoint1.X);
            numericUpDown3.Value = CMath.ChageDecimalPosX(bp.controlPoint2.X);
            numericUpDown4.Value = CMath.ChageDecimalPosY(bp.startPoint.Y);
            numericUpDown5.Value = CMath.ChageDecimalPosY(bp.controlPoint1.Y);
            numericUpDown6.Value = CMath.ChageDecimalPosY(bp.controlPoint2.Y);
            numericUpDown7.Value = CMath.ChageDecimalPosY(m_CurvePointControl.GetEndPointY());
            numericUpDown8.Value = CMath.ChageDecimalPosY(m_CurvePointControl.GetFirstStartPointY());
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
                    //変更した値を保存
                    m_CurvePointControl.SaveMemento();
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
            pictureBox1.Refresh();//再描画
            //点を移動してるときはカーソルの見た目を変える
            if (m_CurvePointControl.isMoveSelectPoint())
            {
                this.Cursor = Cursors.SizeAll;
            }
            else
            {
                if (this.Cursor != Cursors.AppStarting) this.Cursor = Cursors.Default;
            }
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
#if DEBUG
            if (checkBox2.Checked)
            {
                m_PointRender.SetList(m_CurvePointControl.GetGraph());
                m_PointRender.Paint(e);
                return;
            }
#endif
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
            m_CurvePointControl.SaveMemento();//変更した値を保存
            m_CurvePointControl.AddPoint();
            pictureBox1.Refresh();//再描画 
           
        }
        //点追加ダブルクリック時呼びだす
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();//変更した値を保存
            m_CurvePointControl.AddPoint(m_MousePos);
            pictureBox1.Refresh();//再描画
        }
        //点削除ボタンクリック
        private void button1_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento(); //変更した値を保存
            m_CurvePointControl.DeletePoint();
            pictureBox1.Refresh();//再描画
        }
        //キーを押した際のイベント
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //デリートキー押したら
            if (e.KeyCode == Keys.Delete)
             {
                m_CurvePointControl.SaveMemento(); //変更した値を保存
                m_CurvePointControl.DeletePoint();//点削除
                 pictureBox1.Refresh();//再描画
            }
            //エスケープキーを押したら
            if (e.KeyCode == Keys.Escape)
            {
                if (isCloseCheck() == DialogResult.OK)
                {
                    // Form1の破棄
                    this.Dispose();
                }
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
        //入力項目の同期-------------------------------------------------------------------------

        /// <summary>
        /// 選択点X
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeSelectPointX(object sender, EventArgs e)
        {
            int num3 = CMath.ChageNomalPosX(numericUpDown1.Value);
            int num4 = m_CurvePointControl.SetStartPointX(num3);
            numericUpDown1.Value = CMath.ChageDecimalPosX(num4); 
             if (!m_CurvePointControl.isMoveSelectPoint()) pictureBox1.Refresh();//再描画
        }
        /// <summary>
        /// 選択点Y
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeSelectPointY(object sender, EventArgs e)
        {
            int num = CMath.ChageNomalPosY(numericUpDown4.Value);
            int num2 = m_CurvePointControl.SetStartPointY(num);
            numericUpDown4.Value = CMath.ChageDecimalPosY(num2);
            if (!m_CurvePointControl.isMoveSelectPoint()) pictureBox1.Refresh();//再描画
            //最初の開始点と同期を取る
            numericUpDown8.Value = CMath.ChageDecimalPosY(m_CurvePointControl.GetFirstStartPointY());
        }
        /// <summary>
        /// 制御点1X
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeControlPoint1X(object sender, EventArgs e)
        {
            int num = CMath.ChageNomalPosX(numericUpDown2.Value);
            int num2 = m_CurvePointControl.SetControl1PointX(num);
            numericUpDown2.Value = CMath.ChageDecimalPosX(num2);
             if (!m_CurvePointControl.isMoveSelectPoint())  pictureBox1.Refresh();//再描画
        }
        /// <summary>
        /// 制御点1Y
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeControlPoint1Y(object sender, EventArgs e)
        {
            int num = CMath.ChageNomalPosY(numericUpDown5.Value);
            int num2 = m_CurvePointControl.SetControl1PointY(num);
            numericUpDown5.Value = CMath.ChageDecimalPosY(num2);
            if (!m_CurvePointControl.isMoveSelectPoint())  pictureBox1.Refresh();//再描画
        }
        /// <summary>
        /// 制御点2X
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeControlPoint2X(object sender, EventArgs e)
        {
            int num = CMath.ChageNomalPosX(numericUpDown3.Value);
            int num2 = m_CurvePointControl.SetControl2PointX(num);
            numericUpDown3.Value = CMath.ChageDecimalPosX(num2);
            if (!m_CurvePointControl.isMoveSelectPoint())  pictureBox1.Refresh();//再描画
        }
        /// <summary>
        /// 制御点2Y
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeControlPoint2Y(object sender, EventArgs e)
        {
            int num = CMath.ChageNomalPosY(numericUpDown6.Value);
            int num2 = m_CurvePointControl.SetControl2PointY(num);
            numericUpDown6.Value = CMath.ChageDecimalPosY(num2);
            if (!m_CurvePointControl.isMoveSelectPoint()) pictureBox1.Refresh();//再描画
        }
        //開始点せってい
        public void ChangeFirstStartPoint(object sender, EventArgs e)
        {
            int num = CMath.ChageNomalPosY(numericUpDown8.Value);
            int num2 = m_CurvePointControl.SetFirstStartPoint(num); ;
            numericUpDown8.Value = CMath.ChageDecimalPosY(num2);
            if (!m_CurvePointControl.isMoveSelectPoint()) pictureBox1.Refresh();//再描画
            bp = m_CurvePointControl.GetBezierPoint();
            numericUpDown4.Value = CMath.ChageDecimalPosY( bp.startPoint.Y);
        }
        /// <summary>
        /// 終了点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeEndPoint(object sender, EventArgs e)
        {
            int num = CMath.ChageNomalPosY(numericUpDown7.Value);
            int num2 = m_CurvePointControl.SetEndPoint(num); ;
            numericUpDown7.Value = CMath.ChageDecimalPosY(num2);
            if (!m_CurvePointControl.isMoveSelectPoint()) pictureBox1.Refresh();//再描画
            bp = m_CurvePointControl.GetBezierPoint();
        }
        //変換----------------------------------------------------------------------

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
        //チェックボックスの値が変化したときに呼ばれる
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Refresh();//再描画
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Refresh();//再描画
        }
        //右クリック動作----------------------------------------------------------------
        private void AddPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento(); //変更した値を保存
            m_CurvePointControl.AddPoint(m_MousePos);
            pictureBox1.Refresh();//再描画       
        }

        private void DeletePointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento(); //変更した値を保存
            m_CurvePointControl.DeletePoint();//点削除
            pictureBox1.Refresh();//再描画       
        }
        /// <summary>
        /// 線を直線にする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StraightLineEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento(); //変更した値を保存
            m_CurvePointControl.StraightLineEdit();
        }
        //  メニューバー--------------------------------------------------------------------------
        private void NewCreate_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("現在あるグラフをリセットしてよろしいでしょうか？",
              "質問",
               MessageBoxButtons.OKCancel,
               MessageBoxIcon.Exclamation,
               MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.OK)
            {
                //保持していたしたデータを破棄
                m_CurvePointControl.ClearSaveMemento();
                //「はい」が選択された時
                m_CurvePointControl.CurveEditorInit();
                m_CurvePointControl.SaveMemento();
                pictureBox1.Refresh();//再描画
                menuSave.Enabled = false;
            }
            else if (result == DialogResult.Cancel)
            {
                //「キャンセル」が選択された時
                Console.WriteLine("「キャンセル」が選択されました");
            }
        }
        /// <summary>
        /// エディタの終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuEnd_Click(object sender, EventArgs e)
        {
            DialogResult result = isCloseCheck();

            //何が選択されたか調べる
            if (result == DialogResult.OK)
            {
                //「はい」が選択された時
                Close();
            }
        }
        /// <summary>
        /// 終了の確認
        /// </summary>
        /// <returns></returns>
        public DialogResult isCloseCheck()
        {
            return MessageBox.Show("アプリケーションを終了してもよろしいでしょうか？",
                  "質問",
                  MessageBoxButtons.OKCancel,
                  MessageBoxIcon.Exclamation,
                  MessageBoxDefaultButton.Button2);
        }
        /// <summary>
        /// 開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuPoen_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.ShowDialog();
        }
        /// <summary>
        /// CSVを開いてグラフを表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.Cursor = Cursors.AppStarting;
            //保持していたしたデータを破棄
            m_CurvePointControl.ClearSaveMemento();
            //データ読み込み
            m_CurvePointControl.LoadGraph(openFileDialog1.FileName);
            m_CurvePointControl.SaveMemento();
            editFilePath = openFileDialog1.FileName;
            menuSave.Enabled = true;     
            pictureBox1.Refresh();//再描画
            Thread.Sleep(WaitTime);
            this.Cursor = Cursors.Default;
        }
        /// <summary>
        /// 名前を付けて保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSaveAdd_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "new.csv";
            saveFileDialog1.ShowDialog();
        }
        /// <summary>
        /// ファイルをCSVに保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.Cursor = Cursors.AppStarting;
            m_CurvePointControl.SaveGraph(saveFileDialog1.FileName);
            editFilePath = saveFileDialog1.FileName;
            menuSave.Enabled = true;
            Thread.Sleep(WaitTime);
            this.Cursor = Cursors.Default;
        }
        /// <summary>
        /// 上書き保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSave_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.AppStarting;
            m_CurvePointControl.SaveGraph(editFilePath);
            Thread.Sleep(WaitTime);
            this.Cursor = Cursors.Default;
        }
        /// <summary>
        /// 戻る
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuUnDo_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.UnDo();
            pictureBox1.Refresh();//再描画
        }
        /// <summary>
        /// 進む
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void MenuReDo_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.ReDo();
            pictureBox1.Refresh();//再描画
        }
        //numericUpDownで値を入寮したときのイベント-------------------------------------------------------------------
        private void numericUpDown1_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown1_DragEnter(object sender, DragEventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown2_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown2_Enter(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown3_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown3_Enter(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown4_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown4_Enter(object sender, EventArgs e)
        {
             m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown5_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown5_Enter(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown6_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown6_Enter(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown7_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown7_Enter(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown8_Click(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }
        private void numericUpDown8_Enter(object sender, EventArgs e)
        {
            m_CurvePointControl.SaveMemento();
        }

  
    }
}

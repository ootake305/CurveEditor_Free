using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CurveEditor
{
    /// <summary>
    /// カーブを表示するための点を制御させるクラス
    /// </summary>
    class CurvePointControl
    {
        //3次ベジェ曲線に必要な点
        struct BezierPoint
        {
            public Point startPoint;     //開始点
            public Point controlPoint1;  //制御点1
            public Point controlPoint2;  //制御点2
            public Point endPoint;       //終了点
        }
        List<BezierPoint> m_list = new List<BezierPoint>();//線を引くための点を格納する場所

        //グラフの範囲を示す座標
        const int ScrrenRightPosX = 600;
        const int ScrrenBottomPosY = 310;
        const int ScrrenLeftPosX = 0;
        const int ScrrenTopPosY =  10;

        int m_SelectPoint = 0;  //左から何番目の点を選択精しているか
        const float m_cpSize = 8; //点のサイズ
        bool m_isMoveStartPoint = false;      //開始点を選択した状態でドラッグできるか
        bool m_isMoveEndPoint = false;       //終了点を選択した状態でドラッグできるか
        bool m_isMoveControl1Point = false;  //終了点を選択した状態でドラッグできるか
        bool m_isMoveControl2Point = false;  //終了点を選択した状態でドラッグできるか
        bool m_isSelectStartPoint = true;    //開始点を選択しているか falseなら終了点を選択
        //線を引くためのパス
        GraphicsPath m_path = new GraphicsPath();//曲線を引くためのパス
        GraphicsPath m_path2 = new GraphicsPath();//直線を引くためのパス
        //いろんな色
        Brush m_PointColor = new SolidBrush(Color.FromArgb(255, 255, 0, 0));       //点の色
        Pen m_PointLineColor = new Pen(Color.FromArgb(200, 245, 245, 245),4);     //強調線の色
        Pen m_CPointLineColor = new Pen(Color.FromArgb(125, 245, 245, 245), 4);   //制御点の強調線の色
        Pen m_pen = new Pen(Color.White, 1.5f);                           //曲線の色
        Pen m_pen2 = new Pen(Color.FromArgb(125, 245, 245, 245), 2.5f);   //直線の色
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public  CurvePointControl()
        {
            //初期の曲線設定
            BezierPoint startBezirPoint = new BezierPoint();
            startBezirPoint.startPoint = new Point(ScrrenLeftPosX, 160);//中央配置
            startBezirPoint.endPoint = new Point(ScrrenRightPosX, ScrrenTopPosY);
            startBezirPoint.controlPoint1 = new Point(20, 150);
            startBezirPoint.controlPoint2 = new Point(40, 130);
            m_list.Add(startBezirPoint);
        }
      
        /// <summary>
        //3次ベジェ曲線を結ぶすべての点描画
        /// <summary>
        public void PointrPaint(PaintEventArgs e)
        {
            var pontcnt = m_list.Count();
            //開始点すべて描画
            for(int i = 0; i < pontcnt; i++)
            {
                e.Graphics.FillEllipse(m_PointColor, m_list[i].startPoint.X - m_cpSize / 2, m_list[i].startPoint.Y - m_cpSize / 2, m_cpSize, m_cpSize);
            }
            int lastpoint = pontcnt - 1;
            //最後の終了点だけ描画
            e.Graphics.FillEllipse(m_PointColor, m_list[lastpoint].endPoint.X - m_cpSize / 2, m_list[lastpoint].endPoint.Y - m_cpSize / 2, m_cpSize, m_cpSize);

        }
        /// <summary>
        //選択している開始点を強調描画
        /// <summary>
        public void SelectPointrPaint(PaintEventArgs e)
        {
            //開始点を選択しているか
            if(m_isSelectStartPoint)
            {
                e.Graphics.DrawEllipse(m_PointLineColor, m_list[m_SelectPoint].startPoint.X - m_cpSize / 2, m_list[m_SelectPoint].startPoint.Y - m_cpSize / 2, m_cpSize, m_cpSize);
                e.Graphics.DrawEllipse(m_CPointLineColor, m_list[m_SelectPoint].controlPoint1.X - m_cpSize / 2, m_list[m_SelectPoint].controlPoint1.Y - m_cpSize / 2, m_cpSize, m_cpSize);
                e.Graphics.DrawEllipse(m_CPointLineColor, m_list[m_SelectPoint].controlPoint2.X - m_cpSize / 2, m_list[m_SelectPoint].controlPoint2.Y - m_cpSize / 2, m_cpSize, m_cpSize);
            }
            else  //終了点を選択している
            {
                int LastNum = m_list.Count() - 1;//最後の点
                e.Graphics.DrawEllipse(m_PointLineColor, m_list[LastNum].endPoint.X - m_cpSize / 2, m_list[LastNum].endPoint.Y - m_cpSize / 2, m_cpSize, m_cpSize);
            }

        }
        /// <summary>
        //選択している点の制御点描画
        /// <summary>
        public void ControlPaint(PaintEventArgs e)
        {
            //開始点を選択してるときのみ制御点の描画をする
            if (!m_isSelectStartPoint) return;
            e.Graphics.FillEllipse(m_PointColor, m_list[m_SelectPoint].controlPoint1.X - m_cpSize / 2, m_list[m_SelectPoint].controlPoint1.Y - m_cpSize / 2, m_cpSize, m_cpSize);
            e.Graphics.FillEllipse(m_PointColor, m_list[m_SelectPoint].controlPoint2.X - m_cpSize / 2, m_list[m_SelectPoint].controlPoint2.Y - m_cpSize / 2, m_cpSize, m_cpSize);
        }
        /// <summary>
        //3次ベジェ曲線描画
        /// <summary>
        public void BezierPaint(PaintEventArgs e)
        {
            m_path.Reset();
            m_path2.Reset();
            //パス追加
            foreach (BezierPoint item in m_list)
            {
                Point[] p = new Point[4];
                p[0] = item.startPoint;
                p[1] = item.controlPoint1;
                p[2] = item.controlPoint2;
                p[3] = item.endPoint;
                m_path.AddBeziers(p);
            }
            m_path2.AddLine(m_list[m_SelectPoint].startPoint, m_list[m_SelectPoint].controlPoint1);
            m_path2.AddLine(m_list[m_SelectPoint].startPoint, m_list[m_SelectPoint].controlPoint2);
            //描画
            e.Graphics.DrawPath(m_pen, m_path);
            //制御点は開始点を選択しているときだけ描画する
            if (!m_isSelectStartPoint) return;
            e.Graphics.DrawPath(m_pen2, m_path2);
        }
        /// <summary>
        /// 点の追加
        /// </summary>
        /// <param name="p"></param>
        public void AddPoint(Point p)
        {
          
        }
        /// <summary>
        /// 点の移動
        /// </summary>
        /// <param name="mouse"></param>
        public void MovePoint(MouseEventArgs mouse)
        {
            //      if (isSelect) return;
            if (m_isMoveStartPoint)
            {
                BezierPoint sp = m_list[m_SelectPoint];//選択した開始点

                if(m_SelectPoint != 0) sp.startPoint.X = mouse.X;
                sp.startPoint.Y = Clamp(mouse.Y, ScrrenTopPosY, ScrrenBottomPosY); 
                m_list[m_SelectPoint] = sp;
            } 
            else if(m_isMoveEndPoint)
            {
                int LastNum = m_list.Count() - 1;//選択した最後の点
                BezierPoint sp = m_list[LastNum];

                sp.endPoint.Y = Clamp(mouse.Y, ScrrenTopPosY, ScrrenBottomPosY); ;
               
                m_list[LastNum] = sp;
            }
          
        }
        /// <summary>
        /// 点を選択してドラック出来る状態か検索する クリックしたときに呼ぶ
        /// </summary>
        ///  /// <param name="mouse"></param>
        public void SearchSelectPoint(MouseEventArgs mouse)
        {
            var pontcnt = m_list.Count();
            //すべての開始点を調べる
            for (int i = 0; i < pontcnt; i++)
            {
                if(isSearchSelectStartPoint(mouse, i))
                {
                    //開始点を選択
                    m_SelectPoint = i;
                    m_isMoveStartPoint = true;
                    m_isMoveEndPoint = false;
                    m_isSelectStartPoint = true;
                    return;
                }
            }
            //終了点を選択
            if (isSearchSelectEndPoint(mouse))
            {
                m_isMoveStartPoint = false;
                m_isMoveEndPoint = true;
                m_isSelectStartPoint = false;
            }

        }
        /// <summary>
        /// 移動可能選択状態を解除　クリックを終えたら呼ぶ
        /// </summary>
        public void CancelMovePoint()
        {
            m_isMoveStartPoint = false;
            m_isMoveEndPoint = false;
        }
       /// <summary>
       /// 開始点がマウスカーソルの下にあるか検索
       /// </summary>
       /// <param name="mouse"></param>
       /// <param name="num"></param>
       /// <returns></returns>
        public bool isSearchSelectStartPoint(MouseEventArgs mouse,int num)
        {
            float SelectPointSize = m_cpSize + 20;//判定は少し大きめに
            if (mouse.X >= m_list[num].startPoint.X - SelectPointSize / 2 && mouse.X < m_list[num].startPoint.X + SelectPointSize / 2)
            {
                if (mouse.Y >= m_list[num].startPoint.Y - SelectPointSize / 2 && mouse.Y < m_list[num].startPoint.Y + SelectPointSize / 2)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 開始点がマウスカーソルの下にあるか検索
        /// </summary>
        /// <param name="mouse"></param>
        /// <returns></returns>
        public bool isSearchSelectEndPoint(MouseEventArgs mouse)
        {
            float SelectPointSize = m_cpSize + 20;//判定は少し大きめに
            int LastNum = m_list.Count() - 1;//最後の点
            if (mouse.X >= m_list[LastNum].endPoint.X - SelectPointSize / 2 && mouse.X < m_list[LastNum].endPoint.X + SelectPointSize / 2)
            {
                if (mouse.Y >= m_list[LastNum].endPoint.Y - SelectPointSize / 2 && mouse.Y < m_list[LastNum].endPoint.Y + SelectPointSize / 2)
                {
                    return true;
                }
            }
            return false;
        }

 
        /// <summary>
        /// /値を特定の範囲に宣言する
        /// </summary>
        /// <param name="x"></param>
        /// <param name="minVal"></param>
        /// <param name="maxVal"></param>
        /// <returns></returns>
        public int  Clamp(int x, int minVal, int maxVal)
        {
            return Math.Min(Math.Max(minVal, x), maxVal);
        }

    }
}

 




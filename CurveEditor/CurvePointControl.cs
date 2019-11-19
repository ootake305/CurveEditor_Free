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
        List<BezierPoint> list = new List<BezierPoint>();//線を引くための点を格納する場所

        int m_SelectPoint = 0;  //左から何番目の点を選択精しているか
        const float cpSize = 8; //点のサイズ
        bool isSelect = false;   //点を選択した状態でドラッグできるか
        //線を引くためのパス
        GraphicsPath path = new GraphicsPath();//曲線を引くためのパス
        GraphicsPath path2 = new GraphicsPath();//直線を引くためのパス
        //いろんな色
        Brush PointColor = new SolidBrush(Color.FromArgb(255, 255, 0, 0));       //点の色
        Pen PointLineColor = new Pen(Color.FromArgb(200, 245, 245, 245),4);     //強調線の色
        Pen CPointLineColor = new Pen(Color.FromArgb(125, 245, 245, 245), 4);   //制御点の強調線の色
        Pen pen = new Pen(Color.White, 1.5f);                           //曲線の色
        Pen pen2 = new Pen(Color.FromArgb(125, 245, 245, 245), 2.5f);   //直線の色
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public  CurvePointControl()
        {
            //初期の曲線設定
            BezierPoint startBezirPoint = new BezierPoint();
            startBezirPoint.startPoint = new Point(0, 160);
            startBezirPoint.endPoint = new Point(600, 10);
            startBezirPoint.controlPoint1 = new Point(20, 150);
            startBezirPoint.controlPoint2 = new Point(40, 130);
            list.Add(startBezirPoint);
        }
      
        /// <summary>
        //3次ベジェ曲線を結ぶすべての点描画
        /// <summary>
        public void PointrPaint(PaintEventArgs e)
        {
            var pontcnt = list.Count();
            //開始点すべて描画
            for(int i = 0; i < pontcnt; i++)
            {
                e.Graphics.FillEllipse(PointColor, list[i].startPoint.X - cpSize / 2, list[i].startPoint.Y - cpSize / 2, cpSize, cpSize);
            }
            int lastpoint = pontcnt - 1;
            //最後の終了点だけ描画
            e.Graphics.FillEllipse(PointColor, list[lastpoint].endPoint.X - cpSize / 2, list[lastpoint].endPoint.Y - cpSize / 2, cpSize, cpSize);

        }
        /// <summary>
        //選択している開始点を強調描画
        /// <summary>
        public void SelectPointrPaint(PaintEventArgs e)
        {
            e.Graphics.DrawEllipse(PointLineColor, list[m_SelectPoint].startPoint.X - cpSize / 2, list[m_SelectPoint].startPoint.Y - cpSize / 2, cpSize, cpSize);
            e.Graphics.DrawEllipse(CPointLineColor, list[m_SelectPoint].controlPoint1.X - cpSize / 2, list[m_SelectPoint].controlPoint1.Y - cpSize / 2, cpSize, cpSize);
            e.Graphics.DrawEllipse(CPointLineColor, list[m_SelectPoint].controlPoint2.X - cpSize / 2, list[m_SelectPoint].controlPoint2.Y - cpSize / 2, cpSize, cpSize);

        }
        /// <summary>
        //選択している点の制御点描画
        /// <summary>
        public void ControlPaint(PaintEventArgs e)
        {
            e.Graphics.FillEllipse(PointColor, list[m_SelectPoint].controlPoint1.X - cpSize / 2, list[m_SelectPoint].controlPoint1.Y - cpSize / 2, cpSize, cpSize);
            e.Graphics.FillEllipse(PointColor, list[m_SelectPoint].controlPoint2.X - cpSize / 2, list[m_SelectPoint].controlPoint2.Y - cpSize / 2, cpSize, cpSize);
        }
        /// <summary>
        //3次ベジェ曲線描画
        /// <summary>
        public void BezierPaint(PaintEventArgs e)
        {
            path.Reset();
            path2.Reset();
            //パス追加
            foreach (BezierPoint item in list)
            {
                Point[] p = new Point[4];
                p[0] = item.startPoint;
                p[1] = item.controlPoint1;
                p[2] = item.controlPoint2;
                p[3] = item.endPoint;
                path.AddBeziers(p);
            }
            path2.AddLine(list[m_SelectPoint].startPoint, list[m_SelectPoint].controlPoint1);
            path2.AddLine(list[m_SelectPoint].startPoint, list[m_SelectPoint].controlPoint2);
            //描画
            e.Graphics.DrawPath(pen, path);
            e.Graphics.DrawPath(pen2, path2);
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
        /// <param name="e"></param>
        public void MovePoint(MouseEventArgs e)
        {
            //      if (isSelect) return;
            if (!isSelect) return;
            BezierPoint sp = list[m_SelectPoint];
            sp.startPoint.X = e.X;
            sp.startPoint.Y = e.Y;
            list[m_SelectPoint] = sp;
        }
        /// <summary>
        /// 点を選択してドラック出来る状態か検索する クリックしたときに呼ぶ
        /// </summary>
        ///  /// <param name="e"></param>
        public void SearchSelectPoint(MouseEventArgs e)
        {
            var pontcnt = list.Count();
            //すべての開始点を調べる
            for (int i = 0; i < pontcnt; i++)
            {
                if(SearchSelectStartPoint(e, i))
                {
                    m_SelectPoint = i;
                    isSelect = true;
                    return;
                }
            }
        }
        /// <summary>
        /// 選択状態を解除　クリックを終えたら呼ぶ
        /// </summary>
        public void CancelSelectPoint()
        {
            isSelect = false;
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="e"></param>
       /// <param name="num"></param>
       /// <returns></returns>
        public bool SearchSelectStartPoint(MouseEventArgs e,int num)
        {
            float SelectPointSize = cpSize + 20;
            if (e.X >= list[num].startPoint.X - SelectPointSize / 2 && e.X < list[num].startPoint.X + SelectPointSize / 2)
            {
                if (e.Y >= list[num].startPoint.Y - SelectPointSize / 2 && e.Y < list[num].startPoint.Y + SelectPointSize / 2)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

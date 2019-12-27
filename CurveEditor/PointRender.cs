using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;
namespace CurveEditor
{
    //他の環境でグラフを読み込めるかをテストするためのクラス
    class PointRender
    {

        //3次ベジェ曲線に必要な点
        public struct CSVPoint
        {
            //配列サイズは2でXとYを保存させる
            //本番の環境はCSVから読み込む
            public decimal[] startPoint;     //開始点
            public decimal[] controlPoint1;  //制御点1
            public decimal[] controlPoint2;  //制御点2
            public decimal[] endPoint;       //終了点
        }
        Brush m_PointColor = new SolidBrush(Color.FromArgb(255, 0, 255, 0));       //点の色
        List<CurvePointControl.BezierPoint> m_list = new List<CurvePointControl.BezierPoint>();//線を引くための点を格納する場所
        List< CSVPoint> m_Csvlist = new List<CSVPoint>();//線を引くための点を格納する場所

        const float m_cpSize = 5; //点のサイズ

        public void Paint(PaintEventArgs e)
        {
            PaintGraph(e);
        }
        //保存されている点を描画
        public void PaintGraph(PaintEventArgs e)
        {
            var pontcnt = m_list.Count();
            //開始点すべて描画
            for (int i = 0; i < pontcnt; i++)
            {
                e.Graphics.FillEllipse(m_PointColor, m_list[i].startPoint.X - m_cpSize / 2, m_list[i].startPoint.Y - m_cpSize / 2, m_cpSize, m_cpSize);
            }
            int lastpoint = pontcnt - 1;
            //最後の終了点だけ描画
            e.Graphics.FillEllipse(m_PointColor, m_list[lastpoint].endPoint.X - m_cpSize / 2, m_list[lastpoint].endPoint.Y - m_cpSize / 2, m_cpSize, m_cpSize);


            for(int i = 0; i< 100; i++)
            {
                int x = CMath.ChageNomalPosX(EvaluateX((decimal)(0.01f * i)));
                int y = CMath.ChageNomalPosY(EvaluateY((decimal)(0.01f * i)));

                e.Graphics.FillEllipse(m_PointColor, x - m_cpSize / 2, y - m_cpSize / 2, m_cpSize, m_cpSize);
            }
            TestPrint();
        }
        bool q = true;
        public void TestPrint()
        {
            if (!q) return;
            for (int i = 0; i < 100; i++)
            {
                //int x = CMath.ChageNomalPosX((decimal)(0.001f * i));
                decimal x = EvaluateX((decimal)(0.01f * i));
                decimal y = EvaluateY((decimal)(0.01f * i));
                Console.Write("x = {0:f3}, y = {1:f3} \n", x, y); // 文字や数値の出力
            }
             q = false;
        }
        public void SetList(List<CurvePointControl.BezierPoint> list)
        {
            m_list = list;
            m_Csvlist.Clear();
            foreach(CurvePointControl.BezierPoint item in m_list)
            {
                CSVPoint cp = new CSVPoint();
                cp.startPoint = new decimal[2];
                cp.controlPoint1 = new decimal[2];
                cp.controlPoint2 = new decimal[2];
                cp.endPoint = new decimal[2];
                cp.startPoint[0] = CMath.ChageDecimalPosX(item.startPoint.X);
                cp.startPoint[1] = CMath.ChageDecimalPosY(item.startPoint.Y);
                cp.controlPoint1[0] = CMath.ChageDecimalPosX(item.controlPoint1.X);
                cp.controlPoint1[1] = CMath.ChageDecimalPosY(item.controlPoint1.Y);
                cp.controlPoint2[0] = CMath.ChageDecimalPosX(item.controlPoint2.X);
                cp.controlPoint2[1] = CMath.ChageDecimalPosY(item.controlPoint2.Y);
                cp.endPoint[0] = CMath.ChageDecimalPosX(item.endPoint.X);
                cp.endPoint[1] = CMath.ChageDecimalPosY(item.endPoint.Y);

                m_Csvlist.Add(cp);
            }
        }

        /// <summary>
        /// グラフのXからYを求める
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public decimal EvaluateY(decimal x)
        {
            int num = 0;
            int pontcnt = m_list.Count();
            //どこのベジェ曲線か検索
            num = SearchBezier(x);

            decimal x1 = m_Csvlist[num].startPoint[0];
            decimal y1 = m_Csvlist[num].startPoint[1];
            decimal x2 = m_Csvlist[num].controlPoint1[0];
            decimal y2 = m_Csvlist[num].controlPoint1[1];
            decimal x3 = m_Csvlist[num].controlPoint2[0];
            decimal y3 = m_Csvlist[num].controlPoint2[1];
            decimal x4 = m_Csvlist[num].endPoint[0];
            decimal y4 = m_Csvlist[num].endPoint[1];

            decimal t = CMath.ChageDecimalT(x1, x4, x);
            var tp = 1 - t;

            var y = (tp * tp * tp * y1) + (y2* 3 * tp * tp * t) + (y3* 3  * tp * t * t) + (t* t* t * y4);

            return y;
        }
        /// <summary>
        /// グラフのxからxを求める
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public decimal EvaluateX(decimal x)
        {
            int num = 0;
            int pontcnt = m_list.Count();
            //どこのベジェ曲線か検索
            num = SearchBezier(x);
          
            decimal x1 = m_Csvlist[num].startPoint[0];
            decimal x2 = m_Csvlist[num].controlPoint1[0];
            decimal x3 = m_Csvlist[num].controlPoint2[0];
            decimal x4 = m_Csvlist[num].endPoint[0];
            decimal t = CMath.ChageDecimalT(x1, x4, x);
            var tp =1 - t;

            var rx = (tp * tp * tp * x1) + (x2 * 3 * tp * tp * t) + (x3 * 3 * tp * t * t) + (t * t * t * x4);

            return rx;
        }
        public int SearchBezier(decimal x)
        {
            int num = 0;
            int pontcnt = m_list.Count();
            //どこのベジェ曲線か検索
            for (int i = 0; i < pontcnt; i++)
            {
                if (m_Csvlist[i].endPoint[0] > x)
                {
                    num = i;
                    return num;
                }
            }
            return 0;
        }
    }


}

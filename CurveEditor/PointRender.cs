using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
namespace CurveEditor
{
    //他の環境でグラフを読み込めるかをテストするためのクラス
    class PointRender
    {
        struct Vec2
        {
            public float x;
            public float y;

            public Vec2(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
        }
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
        
        List<CurvePointControl.BezierPoint> m_list = new List<CurvePointControl.BezierPoint>(); //線を引くための点を格納する場所
        Brush m_PointColor = new SolidBrush(Color.FromArgb(255, 0, 255, 0));                    //点の色
        List<CSVPoint> m_Csvlist = new List<CSVPoint>();    //線を引くための点を格納する場所
        CSVPoint m_SelectPoint = new CSVPoint();            //選択している曲線

        const float m_cpSize = 5; //点のサイズ
        bool m_isOntimeTest = true;//一回だけテスト出力フラグ

        private float Ax;
        private float Ay;

        private float Bx;
        private float By;

        private float Cx;
        private float Cy;

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


            for (int i = 0; i <= 100; i++)
            {
                // int x = CMath.ChageNomalPosX(EvaluateX((decimal)(0.01f * i)));
                int x = CMath.ChageNomalPosX((decimal)(0.01f * i));
                int y = CMath.ChageNomalPosY(EvaluateY((decimal)(0.01f * i)));

                e.Graphics.FillEllipse(m_PointColor, x - m_cpSize / 2, y - m_cpSize / 2, m_cpSize, m_cpSize);
            }
            TestPrint();
        }

        /// <summary>
        /// テストで書き出し後のデータを出力
        /// </summary>
        public void TestPrint()
        {
            if (!m_isOntimeTest) return;
            for (int i = 0; i <= 100; i++)
            {
                //int x = CMath.ChageNomalPosX((decimal)(0.001f * i));
                decimal x = ((decimal)(0.01f * i));
                decimal y = EvaluateY((decimal)(0.01f * i));
                Console.Write("x = {0:f3}, y = {1:f3} \n", x, y); // 文字や数値の出力
            }
            m_isOntimeTest = false;
        }
        public void SetList(List<CurvePointControl.BezierPoint> list)
        {
            m_list = list;
            m_Csvlist.Clear();
            foreach (CurvePointControl.BezierPoint item in m_list)
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

            m_SelectPoint = m_Csvlist[num];

            return (decimal)CalcYfromX((float)x);
            //return y;
        }
        /// <summary>
        /// どこの曲線か検索 0～1
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
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
            // 0～1の間でない値なら端の曲線を返す
            if (x >= 1) return pontcnt - 1;
           if (x <= 0) return  0;
           return 0;
        }
        private void SetConstants3()
        {
            Vec2 b0 = new Vec2((float)m_SelectPoint.startPoint[0], (float)m_SelectPoint.startPoint[1]);
            Vec2 b1 = new Vec2((float)m_SelectPoint.controlPoint1[0], (float)m_SelectPoint.controlPoint1[1]);
            Vec2 b2 = new Vec2((float)m_SelectPoint.controlPoint2[0], (float)m_SelectPoint.controlPoint2[1]);
            Vec2 b3 = new Vec2((float)m_SelectPoint.endPoint[0], (float)m_SelectPoint.endPoint[1]);

            //公式に当てはめる
            /*ベジェ曲線は媒介変数表示で定義される。tを0～1で変化させると、曲線上の点を取得できる。
              (x1,y1)始点
              (x2,y2)制御点
              (x3,y3)制御点
              (x4,y4)終点

              x = x(t) = t^3*x4 + 3*t^2*(1-t)*x3 + 3*t*(1-t)^2*x2 + (1-t)^3*x1
              y = y(t) = t^3*y4 + 3*t^2*(1-t)*y3 + 3*t*(1-t)^2*y2 + (1-t)^3*y1

              tについて降べきの順に整理すると

              x = (x4-3*(x3+x2)-x1)*t^3 + 3*(x3-2*x2+x1)*t^2 + 3*(x2-x1)*t + x1
              y = (y4-3*(y3+y2)-y1)*t^3 + 3*(y3-2*y2+y1)*t^2 + 3*(y2-y1)*t + y1

              t以外の値を求める?
              */
            this.Ax = b3.x - 3f * (b2.x - b1.x) - b0.x;
            this.Bx = 3f * (b2.x - 2 * b1.x + b0.x);
            this.Cx = 3f * (b1.x - b0.x);

            this.Ay = b3.y - 3f * (b2.y - b1.y) - b0.y;
            this.By = 3f * (b2.y - 2 * b1.y + b0.y);
            this.Cy = 3f * (b1.y - b0.y);
        }
        /// <summary>
        /// tからxとyを求める
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
         Vec2 GetPointAtTime(float t)
        {
            SetConstants3();
            //参考プログラムのやつ　こっちのほうが計算が早いかも？
           /* Vec2 p0 = new Vec2((float)m_SelectPoint.startPoint[0], (float)m_SelectPoint.startPoint[1]);
            float t2 = t * t;
            float t3 = t * t * t;
            float x = this.Ax * t3 + this.Bx * t2 + this.Cx * t + p0.x;
            float y = this.Ay * t3 + this.By * t2 + this.Cy * t + p0.y;
            */

            decimal x1 = m_SelectPoint.startPoint[0];
            decimal y1 =m_SelectPoint.startPoint[1];
            decimal x2 =m_SelectPoint.controlPoint1[0];
            decimal y2 =m_SelectPoint.controlPoint1[1];
            decimal x3 =m_SelectPoint.controlPoint2[0];
            decimal y3 =m_SelectPoint.controlPoint2[1];
            decimal x4 =m_SelectPoint.endPoint[0];
            decimal y4 = m_SelectPoint.endPoint[1];

            decimal t1 = (decimal) t;
            decimal t2 = t1 * t1;
            decimal t3 = t1 * t1 * t1;
            decimal tp1 = 1 - t1;
            decimal tp2 = tp1 * tp1;
            decimal tp3 = tp1 * tp1 *tp1;
            //公式に当てはめる
            /*ベジェ曲線は媒介変数表示で定義される。tを0～1で変化させると、曲線上の点を取得できる。
              (x1,y1)始点
              (x2,y2)制御点
              (x3,y3)制御点
              (x4,y4)終点

              x = x(t) = t^3*x4 + 3*t^2*(1-t)*x3 + 3*t*(1-t)^2*x2 + (1-t)^3*x1
              y = y(t) = t^3*y4 + 3*t^2*(1-t)*y3 + 3*t*(1-t)^2*y2 + (1-t)^3*y1

              媒介変数[t]を用いてるのでxとyの両方の解がでる
              */

            decimal tx = (tp3 * x1) + (x2 * 3 * tp2 * t1) + (x3 * 3 * tp1 * t2) + (t3 * x4);
            decimal ty = (tp3 * y1) + (y2 * 3 * tp2 * t1) + (y3 * 3 * tp1* t2) + (t3 * y4);


            return new Vec2((float)tx, (float)ty);
        }
        /// <summary>
        /// あるtについてxの微分値を求める
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private float sampleCurveDerivativeX(float t)
        {
             // //3次ベジェ
             return (3.0f * this.Ax * t + 2.0f * this.Bx) * t + this.Cx;
        }
        /// <summary>
        /// xを与えるとtについての方程式を求めてyを返す
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        /// 参考サイト http://umeru.hatenablog.com/entry/2015/12/03/131844
        public float CalcYfromX(float x)
        {
            float epsilon = 0.001f; // 閾値
            float x2, t0, t1, t2, d2, i;
            for (t2 = x, i = 0; i < 8; i++)
            {
                x2 = GetPointAtTime(t2).x - x;
                if (Math.Abs(x2) < epsilon)
                {
                    return GetPointAtTime(t2).y;
                }
                d2 = sampleCurveDerivativeX(t2);
                if (Math.Abs(d2) < 1e-6f)
                {
                    break;
                }
                t2 = t2 - x2 / d2;
            }
            t0 = 0f;
            t1 = 1f;
            t2 = x;
            //tが0～1の間でないなら
            if (t2 < t0)
            {
                return GetPointAtTime(t0).y;
            }
            //tが0～1の間でないなら
            if (t2 > t1)
            {
                return GetPointAtTime(t1).y;
            }
            while (t0 < t1)
            {
                x2 = GetPointAtTime(t2).x;
                if (Math.Abs(x2 - x) < epsilon)
                {
                    return GetPointAtTime(t2).y;
                }
                if (x > x2)
                {
                    t0 = t2;
                }
                else
                {
                    t1 = t2;
                }
                t2 = (t1 - t0) * 0.5f + t0;
            }

            return GetPointAtTime(t2).y; // 失敗
        }

        /*   /// <summary>
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
       var tp = 1 - t;

       var rx = (tp * tp * tp * x1) + (x2 * 3 * tp * tp * t) + (x3 * 3 * tp * t * t) + (t * t * t * x4);

       return rx;
   }*/
    }
}

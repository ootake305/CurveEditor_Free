using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
namespace CurveEditor
{
    /// <summary>
    /// 直線を描画するクラス
    /// </summary>
    class LineBase
    {
        protected　Point m_startPoint;   //開始点
        protected Point m_endPoint;     //終了点
        protected Pen m_pen;            //線の色

        //グラフの範囲を示す座標
        protected const int ScrrenRightPosX = 600;
        protected const int ScrrenBottomPosY = 310;
        protected const int ScrrenLeftPosX = 0;
        protected const int ScrrenTopPosY = 10;
        protected const int ScrrenCenterpPosY = 160;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LineBase()
        {
            m_startPoint = new Point(0, 0);
            m_endPoint = new Point(0, 0);
            m_pen = new Pen(Color.White, 1);
        }
        /// <summary>
        /// デストラクタ
        /// </summary>
         ~LineBase()
        {
        }
        /// <summary>
        /// 初期化
        /// </summary>
        virtual public void Init()
        {
        }
        /// <summary>
        /// 直線描画
        /// </summary>
        /// <param name="g"></param>
        public void Paint(Graphics g)
        {
            g.DrawLine(m_pen, m_startPoint, m_endPoint);
        }

    }
    /// <summary>
    /// 中心の線
    /// </summary>

    class CenterLine : LineBase
    {
        /// <summary>
        /// 初期化
        /// </summary>
        public override void Init()
        {
            base.Init();
            m_startPoint.X = ScrrenLeftPosX;
            m_startPoint.Y = ScrrenCenterpPosY;
            m_endPoint.X = ScrrenRightPosX;
            m_endPoint.Y = ScrrenCenterpPosY;
            m_pen = new Pen(Color.FromArgb(100, 200, 200, 200), 1);
        }
    }
    /// <summary>
    /// 上の線
    /// </summary>
    class TopLine : LineBase
    {
        /// <summary>
        /// 初期化
        /// </summary>
        public override void Init()
        {
            base.Init();
            m_startPoint.X = ScrrenLeftPosX;
            m_startPoint.Y = ScrrenTopPosY;
            m_endPoint.X = ScrrenRightPosX;
            m_endPoint.Y = ScrrenTopPosY;
            m_pen = new Pen(Color.FromArgb(100, 200, 200, 200), 1);
        }
    }
    /// <summary>
    /// 下の線
    /// </summary>

    class BottomLine : LineBase
    {
        /// <summary>
        /// 初期化
        /// </summary>
        public override void Init()
        {
            base.Init();
            m_startPoint.X = ScrrenLeftPosX;
            m_startPoint.Y = ScrrenBottomPosY;
            m_endPoint.X = ScrrenRightPosX;
            m_endPoint.Y = ScrrenBottomPosY;
            m_pen = new Pen(Color.FromArgb(100, 200, 200, 200), 1);
        }
    }

    //端っこの縦線
     class VerticalEndLine : LineBase
    {
        bool m_isLeft = false;//左端か
        public VerticalEndLine(bool isLeft)
        {
            m_isLeft = isLeft;
        }
        /// <summary>
        /// 初期化
        /// </summary>
        public override void Init()
        {
            base.Init();
          
            //左側の縦線か
            if(m_isLeft)
            {
                m_startPoint.X = ScrrenLeftPosX;
                m_endPoint.X = ScrrenLeftPosX;
            }
            else
            {
                m_startPoint.X = ScrrenRightPosX;
                m_endPoint.X = ScrrenRightPosX;
            }
           
            m_startPoint.Y = ScrrenTopPosY;
            m_endPoint.Y = ScrrenBottomPosY;
            m_pen = new Pen(Color.FromArgb(100, 200, 200, 200), 5.0f);
        }
    }
    class VerticalLine : LineBase
    {
        int m_X;
        public VerticalLine(int x)
        {
            m_X = x;
        }
        /// <summary>
        /// 初期化
        /// </summary>
        public override void Init()
        {
            base.Init();

             m_startPoint.X = m_X;
             m_endPoint.X = m_X;

            m_startPoint.Y = ScrrenTopPosY;
            m_endPoint.Y = ScrrenBottomPosY;
            m_pen = new Pen(Color.FromArgb(50, 200, 200, 200), 1.0f);
        }
    }
}

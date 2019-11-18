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
            m_startPoint.X = 0;
            m_startPoint.Y = 160;
            m_endPoint.X = 600;
            m_endPoint.Y = 160;
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
            m_startPoint.X = 0;
            m_startPoint.Y = 10;
            m_endPoint.X = 600;
            m_endPoint.Y = 10;
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
            m_startPoint.X = 0;
            m_startPoint.Y = 310;
            m_endPoint.X = 600;
            m_endPoint.Y = 310;
            m_pen = new Pen(Color.FromArgb(100, 200, 200, 200), 1);
        }
    }
}

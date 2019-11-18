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
            Point startPoint;       //開始点
            Point controlPoint1;  //制御点1
            Point controlPoint2;  //制御点2
            Point endPoint;       //終了点
        }
        List<BezierPoint> list;
        //3次ベジェ曲線を結ぶ点描画
        public void PointrPaint(PaintEventArgs e)
        {

        }
        //選択している点の制御点描画
        public void ControlPaint(PaintEventArgs e)
        {

        }
        //3次ベジェ曲線描画
        public void BezierPaint(PaintEventArgs e)
        {

        }
    }
}

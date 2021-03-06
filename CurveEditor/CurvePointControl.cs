﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;

namespace CurveEditor
{
    /// <summary>
    /// カーブを表示するための点を制御させるクラス
    /// </summary>
    class CurvePointControl
    {
        //3次ベジェ曲線に必要な点
        public struct BezierPoint
        {
            public Point startPoint;     //開始点
            public Point controlPoint1;  //制御点1
            public Point controlPoint2;  //制御点2
            public Point endPoint;       //終了点
        }
        //選択状態
        enum SelectMode
        {
            SelectStart,//開始点選択
            SelectEnd,  //終了点
            None,       //何も選択していない
        }
        List<BezierPoint> m_list = new List<BezierPoint>();//線を引くための点を格納する場所
        Stack<List<BezierPoint>> m_UnDoStack = new Stack<List<BezierPoint>>();
        Stack<List<BezierPoint>> m_ReDoStack = new Stack<List<BezierPoint>>();
        //グラフの範囲を示す座標
        const int ScrrenRightPosX = 510;    //右端
        const int ScrrenBottomPosY = 510;   //下端
        const int ScrrenLeftPosX = 10;       //左端
        const int ScrrenTopPosY =  10;      //上端
        const int ScrrenCenterpPosY = 260;  //中央
        const int intervalPointPos = 1;     //点と点の感覚
        const int ControlOfSetX = 1;
        int m_SelectPoint = 0;  //左から何番目の点を選択精しているか
        const float m_cpSize = 8; //点のサイズ
        bool m_isMoveStartPoint = false;      //開始点を選択した状態でドラッグできるか
        bool m_isMoveEndPoint = false;       //終了点を選択した状態でドラッグできるか
        bool m_isMoveControl1Point = false;  //制御点1を選択した状態でドラッグできるか
        bool m_isMoveControl2Point = false;  //制御点2を選択した状態でドラッグできるか
        SelectMode m_SelectMode = SelectMode.None;    //点を選択しているか 
        //線を引くためのパス
        GraphicsPath m_path = new GraphicsPath();//曲線を引くためのパス
        GraphicsPath m_path2 = new GraphicsPath();//直線を引くためのパス
        //いろんな色
        Brush m_PointColor = new SolidBrush(Color.FromArgb(255, 0, 255, 0));       //点の色
        Pen m_PointLineColor = new Pen(Color.FromArgb(220, 245, 245, 245),4);     //強調線の色
        Pen m_CPointLineColor = new Pen(Color.FromArgb(100, 245, 245, 245), 4);   //制御点の強調線の色
        Pen m_pen = new Pen(Color.FromArgb(255, 30, 205,30), 1.5f);                           //曲線の色
        Pen m_pen2 = new Pen(Color.FromArgb(105, 245, 245, 245), 2.0f);   //直線の色
        //データ読み書き機能
        CurveEditorStream m_stream = new CurveEditorStream();
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public  CurvePointControl()
        {
            CurveEditorInit();
            SaveMemento_Click();
        }
        /// <summary>
        /// グラフの初期化
        /// </summary>
        public void CurveEditorInit()
        {
            m_SelectMode = SelectMode.None;
            CancelMovePoint();
            m_SelectPoint = 0;
            m_list.Clear();
            //初期の曲線設定
            BezierPoint startBezirPoint = new BezierPoint();
            startBezirPoint.startPoint = new Point(ScrrenLeftPosX, ScrrenBottomPosY);//中央配置
            startBezirPoint.endPoint = new Point(ScrrenRightPosX, ScrrenTopPosY);
            startBezirPoint.controlPoint1 = new Point(ScrrenLeftPosX + 30, ScrrenCenterpPosY + 30);
            startBezirPoint.controlPoint2 = new Point(ScrrenLeftPosX + 30, ScrrenCenterpPosY - 30);
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
            switch (m_SelectMode)
            {
                case SelectMode.SelectStart:  //開始点を選択しているか
                    e.Graphics.DrawEllipse(m_PointLineColor, m_list[m_SelectPoint].startPoint.X - m_cpSize / 2, m_list[m_SelectPoint].startPoint.Y - m_cpSize / 2, m_cpSize, m_cpSize);
                    e.Graphics.DrawEllipse(m_CPointLineColor, m_list[m_SelectPoint].controlPoint1.X - m_cpSize / 2, m_list[m_SelectPoint].controlPoint1.Y - m_cpSize / 2, m_cpSize, m_cpSize);
                    e.Graphics.DrawEllipse(m_CPointLineColor, m_list[m_SelectPoint].controlPoint2.X - m_cpSize / 2, m_list[m_SelectPoint].controlPoint2.Y - m_cpSize / 2, m_cpSize, m_cpSize);
                    break;
                case SelectMode.SelectEnd://終了点を選択している
                    int LastCnt = m_list.Count() - 1;//最後の点
                    e.Graphics.DrawEllipse(m_PointLineColor, m_list[LastCnt].endPoint.X - m_cpSize / 2, m_list[LastCnt].endPoint.Y - m_cpSize / 2, m_cpSize, m_cpSize);
                    break;
                case SelectMode.None:
                    break;
                default:
                    Debug.Assert(false, "選択状態がおかしいよ！");
                    break;
            }
        }
        /// <summary>
        //選択している点の制御点描画
        /// <summary>
        public void ControlPaint(PaintEventArgs e)
        {
            //開始点を選択してるときのみ制御点の描画をする
            if (m_SelectMode != SelectMode.SelectStart) return;
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
                //曲線パス
                Point[] p = new Point[4];
                p[0] = item.startPoint;
                p[1] = item.controlPoint1;
                p[2] = item.controlPoint2;
                p[3] = item.endPoint;
                m_path.AddBeziers(p);
            }
             //描画
            e.Graphics.DrawPath(m_pen, m_path);
            //制御点は開始点を選択しているときだけ描画する
            if (m_SelectMode != SelectMode.SelectStart) return;
            //直線パス
            m_path2.AddLine(m_list[m_SelectPoint].startPoint, m_list[m_SelectPoint].controlPoint1);
            m_path2.AddLine(m_list[m_SelectPoint].startPoint, m_list[m_SelectPoint].controlPoint2);

            e.Graphics.DrawPath(m_pen2, m_path2);
        }
        /// <summary>
        /// マウスが点が何番目の点になるかを求める
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int SearchSelectStartPoint(Point p)
        {
            int ListMax = m_list.Count();
       
            for (int i = 0; i < ListMax; i++)
            {
                if (m_list[i].endPoint.X > p.X) return i;
            }
            return ListMax - 1;
        }
        /// <summary>
        /// 点の追加(ダブルクリックVer)
        /// </summary>
        /// <param name="p"></param>
        public void AddPoint(Point p)
        {
            var SelectNum = SearchSelectStartPoint(p);//何番目の点になるか
            BezierPoint startBezirPoint = new BezierPoint();
            const int ofsetY = 30;

            startBezirPoint.startPoint = new Point(Math.Min(p.X, ScrrenRightPosX - 1), CMath.Clamp(p.Y, ScrrenTopPosY, ScrrenBottomPosY));
            startBezirPoint.endPoint = new Point(m_list[SelectNum].endPoint.X, m_list[SelectNum].endPoint.Y);
            //制御点は追加される開始点の少し右に生成させる
            var cpointX = CMath.Clamp(startBezirPoint.startPoint.X + 10, ScrrenLeftPosX + ControlOfSetX, ScrrenRightPosX - ControlOfSetX);
            startBezirPoint.controlPoint1 = new Point(cpointX, CMath.Clamp( startBezirPoint.startPoint.Y + ofsetY, ScrrenTopPosY, ScrrenBottomPosY));
            startBezirPoint.controlPoint2 = new Point(cpointX, CMath.Clamp(startBezirPoint.startPoint.Y - ofsetY, ScrrenTopPosY, ScrrenBottomPosY));

            m_list.Insert(SelectNum + 1, startBezirPoint);//新しい点追加

            //追加前の最後の終了点を追加した最後の開始点とつなげる ここ大事！
            BezierPoint BezirPoint = new BezierPoint();
            BezirPoint = m_list[SelectNum];
            BezirPoint.endPoint = m_list[SelectNum + 1].startPoint;//繋ぎなおす
            m_list[SelectNum] = BezirPoint;

            //ダブルクリックで選択モードが解除されてるので選択モードに
            m_SelectMode = SelectMode.SelectStart;
            //増やした点を選択状態に
            m_SelectPoint = SelectNum + 1;
        }
        /// <summary>
        /// 点の追加(ボタンver)
        /// </summary>
        public void AddPoint()
        {
            var LastCnt = m_list.Count() - 1;
            BezierPoint startBezirPoint = new BezierPoint();
            const int ofsetX = 30;
            //
            startBezirPoint.startPoint = new Point(Math.Min( m_list[LastCnt].startPoint.X + ofsetX, ScrrenRightPosX - 1), m_list[LastCnt].startPoint.Y);
            startBezirPoint.endPoint = new Point(m_list[LastCnt].endPoint.X, m_list[LastCnt].endPoint.Y);
            //制御点は追加される開始点の少し右に生成させる
            var cpointX = CMath.Clamp(startBezirPoint.startPoint.X + 10, ScrrenLeftPosX, ScrrenRightPosX - 1);
            startBezirPoint.controlPoint1 = new Point(cpointX, m_list[LastCnt].startPoint.Y + 30);
            startBezirPoint.controlPoint2 = new Point(cpointX, m_list[LastCnt].startPoint.Y - 30);
            m_list.Add(startBezirPoint);//新しい点追加

            //追加前の最後の終了点を追加した最後の開始点とつなげる ここ大事！
            BezierPoint BezirPoint = new BezierPoint();
            BezirPoint = m_list[LastCnt];
            BezirPoint.endPoint = m_list[LastCnt + 1].startPoint;//繋ぎなおす
            m_list[LastCnt] = BezirPoint;
            //増やした点を選択状態に
            m_SelectPoint = LastCnt + 1;
        }
        /// <summary>
        /// 選択している点の削除(ボタンver)
        /// </summary>
        public void DeletePoint()
        {
            //削除できないなら
            if(!isDeletePoint())
            {
                DeleteErrorMessage();//エラーメッセージ
                return;
            }
            var LastCnt = m_list.Count() - 1;

            //最後の開始点を選択してるなら
            if (m_SelectPoint == LastCnt)
            {
                var BeforeSelectPoint = m_SelectPoint - 1;  //ひとつ前の終了点の移動
                //一つ前の終了点と最後の終了点を結ぶ
                BezierPoint sp2 = m_list[BeforeSelectPoint];

                sp2.endPoint.X = ScrrenRightPosX;//終了点は一番右端
                sp2.endPoint.Y = m_list[LastCnt].endPoint.Y;
                m_list[BeforeSelectPoint] = sp2;
            }
            else
            {         
                var NextSelectPoint = m_SelectPoint + 1;  //選択している次のポイント
                var BeforeSelectPoint = m_SelectPoint - 1;  //ひとつ前の終了点の移動
                //一つ前の終了点と一つ先の開始点をつなぐ
                BezierPoint sp2 = m_list[BeforeSelectPoint];

                sp2.endPoint = m_list[NextSelectPoint].startPoint;
                m_list[BeforeSelectPoint] = sp2;
            }
            //削除
            m_list.RemoveAt(m_SelectPoint);
            m_SelectPoint--;
            //削除したときは選択モード解除　でないとエラーが出る
            m_SelectMode = SelectMode.None;
        }
        /// <summary>
        /// 点を削除できる状態か
        /// </summary>
        /// <returns></returns>
        bool isDeletePoint()
        {
            //最初の開始点は削除できない
            if (m_SelectPoint == 0) return false;
            //開始点をしてる時のみ削除できる
            if (m_SelectMode != SelectMode.SelectStart) return false;

            return true;
        }
        /// <summary>
        /// 削除出来ない点ならエラーメッセージを出す
        /// </summary>
        public void DeleteErrorMessage()
        {
            //点を選択してないときは削除できない
            if (m_SelectMode == SelectMode.None)
            {
                MessageBox.Show("点を選択していません");
                return;
            }
            //終了点は削除できない
            if (m_SelectMode == SelectMode.SelectEnd)
            {
                MessageBox.Show("最後の点は削除できません。");
                return;
            }
            //最初の開始点は削除できない
            if (m_SelectPoint == 0)
            {
                MessageBox.Show("最初の点は削除できません。");
                return;
            }
        }
        /// <summary>
        /// 点の移動
        /// </summary>
        /// <param name="mouse"></param>
        public void MovePoint(MouseEventArgs mouse)
        {
            if (m_isMoveStartPoint)//選択した開始点
            {
                MoveStartPoint(mouse);
            }
            else if (m_isMoveControl1Point)//選択した制御点1
            {
                MoveControl1Point(mouse);
            }
            else if (m_isMoveControl2Point)//選択した制御点2
            {
                MoveControl2Point(mouse);
            }
            else if(m_isMoveEndPoint)//選択した最後の点
            {
                MoveEndPoint(mouse);
             }  
        }
        /// <summary>
        /// 開始点の移動
        /// </summary>
        /// <param name="mouse"></param>
        public void MoveStartPoint(MouseEventArgs mouse)
        {
            BezierPoint sp = m_list[m_SelectPoint]; //選択している点

            sp.startPoint.Y = CMath.Clamp(mouse.Y, ScrrenTopPosY, ScrrenBottomPosY);
            // 一番最初の開始点だけY軸にしか動かせないように
            if (!isSelectFirstStartPoint())
            {     
                var BeforeSelectPoint = m_SelectPoint - 1;
                //X軸の移動 intervalPointPosを加算減算すること隣の点と同じ座標にならないようにする
                int minpx = m_list[BeforeSelectPoint].startPoint.X + intervalPointPos;
                int maxpx = sp.endPoint.X - intervalPointPos;
                sp.startPoint.X = CMath.Clamp(mouse.X, minpx, maxpx);
                //ひとつ前の終了点の移動
                BezierPoint sp2 = m_list[BeforeSelectPoint];
                sp2.endPoint = sp.startPoint;
                m_list[BeforeSelectPoint] = sp2;
            }
       
            m_list[m_SelectPoint] = sp;
        }   
        /// <summary>
        /// 制御点1の移動
        /// </summary>
        /// <param name="mouse"></param>
        public void MoveControl1Point(MouseEventArgs mouse)
        {
            BezierPoint sp = m_list[m_SelectPoint]; //選択している点
          
            sp.controlPoint1.X = CMath.Clamp(mouse.X, ScrrenLeftPosX + intervalPointPos, ScrrenRightPosX - intervalPointPos);
            sp.controlPoint1.Y = CMath.Clamp(mouse.Y, ScrrenTopPosY, ScrrenBottomPosY);
            //X軸の移動 
            if (!isSelectFirstStartPoint())
            {
                int minpx = m_list[m_SelectPoint - 1].endPoint.X + ControlOfSetX;
                sp.controlPoint1.X = CMath.Clamp(sp.controlPoint1.X, minpx, ScrrenRightPosX - 1);
            }

            //X軸の移動 
            if (!(isSelectLastEndPoint()))
            {
                int maxpx = m_list[m_SelectPoint + 1].startPoint.X - ControlOfSetX;
                sp.controlPoint1.X = CMath.Clamp(sp.controlPoint1.X, ScrrenLeftPosX, maxpx);
            }
            m_list[m_SelectPoint] = sp;
        }
        /// <summary>
        /// 制御点2の移動
        /// </summary>
        /// <param name="mouse"></param>
        public void MoveControl2Point(MouseEventArgs mouse)
        {
            BezierPoint sp = m_list[m_SelectPoint]; //選択している点
            sp.controlPoint2.X = CMath.Clamp(mouse.X, ScrrenLeftPosX + intervalPointPos, ScrrenRightPosX - intervalPointPos);
            sp.controlPoint2.Y = CMath.Clamp(mouse.Y, ScrrenTopPosY, ScrrenBottomPosY);

            //一番最初の開始点以外を選択しているなら 
            if (!isSelectFirstStartPoint())
            {
                int minpx = m_list[m_SelectPoint - 1].endPoint.X + ControlOfSetX;
                sp.controlPoint2.X = CMath.Clamp(sp.controlPoint2.X,minpx, ScrrenRightPosX - 1);
            }

            //一番最後の終了点以外を選択しているなら 
            if (!isSelectLastEndPoint())
            {
                int maxpx = m_list[m_SelectPoint + 1].startPoint.X - ControlOfSetX;
                sp.controlPoint2.X = CMath.Clamp(sp.controlPoint2.X, ScrrenLeftPosX, maxpx);
            }
            m_list[m_SelectPoint] = sp;
        }
        /// <summary>
        /// 最後の終了点の移動
        /// </summary>
        /// <param name="mouse"></param>
        public void MoveEndPoint(MouseEventArgs mouse)
        {
            int LastCnt = m_list.Count() - 1;
            BezierPoint sp = m_list[LastCnt]; //選択している点
            sp.endPoint.Y = CMath.Clamp(mouse.Y, ScrrenTopPosY, ScrrenBottomPosY); ;
            m_list[LastCnt] = sp;
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

                    m_SelectMode = SelectMode.SelectStart;
                    return;
                }
            }
            //制御点1を選択
            if (isSearchSelectControlPoint(mouse))
            {
                m_isMoveControl1Point = true;
                return;
            }
            //制御点2を選択
            if (isSearchSelectContro2Point(mouse))
            {
                m_isMoveControl2Point = true;
                return;
            }
            //終了点を選択
            if (isSearchSelectEndPoint(mouse))
            {
                m_isMoveEndPoint = true;
                m_SelectMode = SelectMode.SelectEnd;
                return;
            }
            //無選択
            m_isMoveStartPoint = false;
            m_isMoveEndPoint = false;
            m_SelectMode = SelectMode.None;
        }
        /// <summary>
        /// すべての制御点の位置を整理させる
        /// </summary>
        public void OrganizeControlPoint()
        {
            int ListMax = m_list.Count();
            int LastNum = m_list.Count() - 1;
            //制御点がいてはいけない場所なら位置を補正する
            for (int i = 0; i < ListMax; i++)
            {
                BezierPoint sp = m_list[i]; //選択している点
                //左端以外なら
                if (i != 0)
                {
                    sp.controlPoint1.X = CMath.Clamp(sp.controlPoint1.X, m_list[i - 1].endPoint.X + ControlOfSetX, ScrrenRightPosX);
                    sp.controlPoint2.X = CMath.Clamp(sp.controlPoint2.X, m_list[i - 1].endPoint.X + ControlOfSetX, ScrrenRightPosX);
                }
                //右端以外なら
                if (i != LastNum)
                {
                    sp.controlPoint1.X = CMath.Clamp(sp.controlPoint1.X, ScrrenLeftPosX, m_list[i + 1].startPoint.X - ControlOfSetX);
                    sp.controlPoint2.X = CMath.Clamp(sp.controlPoint2.X, ScrrenLeftPosX, m_list[i + 1].startPoint.X - ControlOfSetX);
                }
                m_list[i] = sp;
            }
        }
        /// <summary>
        /// 移動可能選択状態を解除　クリックを終えたら呼ぶ
        /// </summary>
        public void CancelMovePoint()
        {
            m_isMoveStartPoint = false;
            m_isMoveEndPoint = false;
            m_isMoveControl1Point = false;
            m_isMoveControl2Point = false;
        }
       /// <summary>
       /// 開始点がマウスカーソルの下にあるか検索
       /// </summary>
       /// <param name="mouse"></param>
       /// <returns></returns>
        public bool isSearchSelectStartPoint(MouseEventArgs mouse,int num)
        {
            float SelectPointSize = m_cpSize + 15;//判定は少し大きめに
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
        /// 制御点1がマウスカーソルの下にあるか検索
        /// </summary>
        /// <param name="mouse"></param>
        /// <returns></returns>
        public bool isSearchSelectControlPoint(MouseEventArgs mouse)
        {
            if (m_SelectMode != SelectMode.SelectStart) return false;
            float SelectPointSize = m_cpSize + 15;//判定は少し大きめに
            if (mouse.X >= m_list[m_SelectPoint].controlPoint1.X - SelectPointSize / 2 && mouse.X < m_list[m_SelectPoint].controlPoint1.X + SelectPointSize / 2)
            {
                if (mouse.Y >= m_list[m_SelectPoint].controlPoint1.Y - SelectPointSize / 2 && mouse.Y < m_list[m_SelectPoint].controlPoint1.Y + SelectPointSize / 2)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 制御点2がマウスカーソルの下にあるか検索
        /// </summary>
        /// <param name="mouse"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool isSearchSelectContro2Point(MouseEventArgs mouse)
        {
            if (m_SelectMode != SelectMode.SelectStart) return false;
            float SelectPointSize = m_cpSize + 15;//判定は少し大きめに
            if (mouse.X >= m_list[m_SelectPoint].controlPoint2.X - SelectPointSize / 2 && mouse.X < m_list[m_SelectPoint].controlPoint2.X + SelectPointSize / 2)
            {
                if (mouse.Y >= m_list[m_SelectPoint].controlPoint2.Y - SelectPointSize / 2 && mouse.Y < m_list[m_SelectPoint].controlPoint2.Y + SelectPointSize / 2)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 終了点がマウスカーソルの下にあるか検索
        /// </summary>
        /// <param name="mouse"></param>
        /// <returns></returns>
        public bool isSearchSelectEndPoint(MouseEventArgs mouse)
        {
            float SelectPointSize = m_cpSize + 15;//判定は少し大きめに
            int LastCnt = m_list.Count() - 1;//最後の点
            if (mouse.X >= m_list[LastCnt].endPoint.X - SelectPointSize / 2 && mouse.X < m_list[LastCnt].endPoint.X + SelectPointSize / 2)
            {
                if (mouse.Y >= m_list[LastCnt].endPoint.Y - SelectPointSize / 2 && mouse.Y < m_list[LastCnt].endPoint.Y + SelectPointSize / 2)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 最初の開始点を選択しているか
        /// </summary>
        /// <returns></returns>
        bool isSelectFirstStartPoint()
        {
            return m_SelectPoint == 0;
        }
        /// <summary>
        /// 最後の終了点を選択しているか
        /// </summary>
        /// <returns></returns>
        bool isSelectLastEndPoint()
        {
            var LastCnt = m_list.Count() - 1;
            return m_SelectPoint == LastCnt;
        }
        /// <summary>
        /// どれかの動いてる点を選択してるか
        /// </summary>
        /// <returns></returns>
        public bool isMoveSelectPoint()
        {
            if (m_isMoveStartPoint) return true;//選択した開始点
            if (m_isMoveControl1Point) return true;//選択した制御点1
            if (m_isMoveControl2Point) return true;//選択した制御点2
            if (m_isMoveEndPoint) return true;//選択した最後の点
            return false;
        }
        /// <summary>
        /// どれかの点を選択してるか
        /// </summary>
        /// <returns></returns>
        public bool isSelectPoint()
        {
            if (m_SelectMode == SelectMode.None) return false;
            return true;
        }
        /// <summary>
        ///    開始点セット
        /// </summary>
        /// <param name="y"></param>
        public int SetStartPointX(int x)
        {
            BezierPoint sp = m_list[m_SelectPoint]; //最初の点

            if (isSelectFirstStartPoint()) return 0;
            if (m_SelectMode == SelectMode.None) return 0;

            var BeforeSelectPoint = m_SelectPoint - 1;
            //X軸の移動 intervalPointPosを加算減算すること隣の点と同じ座標にならないようにする
            int minpx = m_list[BeforeSelectPoint].startPoint.X + intervalPointPos;
            int maxpx = sp.endPoint.X - intervalPointPos;
            sp.startPoint.X = CMath.Clamp(x, minpx, maxpx);
            //ひとつ前の終了点の移動
            BezierPoint sp2 = m_list[BeforeSelectPoint];
            sp2.endPoint = sp.startPoint;
            m_list[BeforeSelectPoint] = sp2;

            m_list[m_SelectPoint] = sp;
            return x;
        }
        /// <summary>
        ///    開始点セット
        /// </summary>
        /// <param name="y"></param>
        public int SetStartPointY(int y)
        {
            if (m_SelectMode == SelectMode.None) return 0;

            BezierPoint sp = m_list[m_SelectPoint]; //最初の点のY

            sp.startPoint.Y = CMath.Clamp(y, ScrrenTopPosY, ScrrenBottomPosY);
            m_list[m_SelectPoint] = sp;

            //最初の開始点を選択してないなら前の終了点も移動させる
            if (!isSelectFirstStartPoint())
            {
                var BeforeSelectPoint = m_SelectPoint - 1;
                BezierPoint sp2 = m_list[BeforeSelectPoint];
                sp2.endPoint = sp.startPoint;
                m_list[BeforeSelectPoint] = sp2;
            }
                return sp.startPoint.Y;
        }
        /// <summary>
        ///    制御点1Xセット
        /// </summary>
        /// <param name="y"></param>
        public int SetControl1PointX(int x)
        {
            if (m_SelectMode == SelectMode.None) return 0;

            BezierPoint sp = m_list[m_SelectPoint]; //選択している点
            sp.controlPoint1.X = CMath.Clamp(x, ScrrenLeftPosX, ScrrenRightPosX);

            //一番最初の開始点以外を選択しているなら  +1 -1することで隣の点と同じ座標にならないようにする
            if (!isSelectFirstStartPoint())
            {
                int minpx = m_list[m_SelectPoint - 1].endPoint.X + ControlOfSetX;
                sp.controlPoint1.X = CMath.Clamp(sp.controlPoint1.X, minpx, ScrrenRightPosX);
            }

            //一番最後の終了点以外を選択しているなら 
            if (!isSelectLastEndPoint())
            {
                int maxpx = m_list[m_SelectPoint + 1].startPoint.X - ControlOfSetX;
                sp.controlPoint1.X = CMath.Clamp(sp.controlPoint1.X, ScrrenLeftPosX, maxpx);
            }
            m_list[m_SelectPoint] = sp;

            return sp.controlPoint1.X;
        }
        /// <summary>
        ///    制御点2Xセット
        /// </summary>
        /// <param name="y"></param>
        public int SetControl2PointX(int x)
        {
            if (m_SelectMode == SelectMode.None) return 0;

            BezierPoint sp = m_list[m_SelectPoint]; //選択している点
            sp.controlPoint2.X = CMath.Clamp(x, ScrrenLeftPosX, ScrrenRightPosX);

            //一番最初の開始点以外を選択しているなら
            if (!isSelectFirstStartPoint())
            {
                int minpx = m_list[m_SelectPoint - 1].endPoint.X + ControlOfSetX;
                sp.controlPoint2.X = CMath.Clamp(sp.controlPoint2.X, minpx, ScrrenRightPosX);
            }

            //一番最後の終了点以外を選択しているなら 
            if (!isSelectLastEndPoint())
            {
                int maxpx = m_list[m_SelectPoint + 1].startPoint.X - ControlOfSetX;
                sp.controlPoint2.X = CMath.Clamp(sp.controlPoint2.X, ScrrenLeftPosX, maxpx);
            }
            m_list[m_SelectPoint] = sp;

            return sp.controlPoint2.X;
        }
        /// <summary>
        ///    制御点1Ｙセット
        /// </summary>
        /// <param name="y"></param>
        public int SetControl1PointY(int y)
        {
            if (m_SelectMode == SelectMode.None) return 0;

            BezierPoint sp = m_list[m_SelectPoint];

            sp.controlPoint1.Y = CMath.Clamp(y, ScrrenTopPosY, ScrrenBottomPosY);
            m_list[m_SelectPoint] = sp;
            return sp.controlPoint1.Y;
        }
        /// <summary>
        ///    制御点2Ｙセット
        /// </summary>
        /// <param name="y"></param>
        public int SetControl2PointY(int y)
        {
            if (m_SelectMode == SelectMode.None) return 0;

            BezierPoint sp = m_list[m_SelectPoint];

            sp.controlPoint2.Y = CMath.Clamp(y, ScrrenTopPosY, ScrrenBottomPosY);
            m_list[m_SelectPoint] = sp;
            return sp.controlPoint2.Y;
        }
        /// <summary>
        /// 最初の開始点セット
        /// </summary>
        /// <param name="y"></param>
        public int SetFirstStartPoint(int y)
        {
            BezierPoint sp = m_list[0]; //最初の点
            sp.startPoint.Y = CMath.Clamp(y, ScrrenTopPosY, ScrrenBottomPosY);
            m_list[0] = sp;
            return sp.startPoint.Y;
        }
        /// <summary>
        /// 最後の終了点セット
        /// </summary>
        /// <param name="y"></param>
         public int  SetEndPoint(int y)
        {
            var LastCnt = m_list.Count() - 1;

            BezierPoint sp = m_list[LastCnt]; //最後の点
            sp.endPoint.Y = CMath.Clamp(y, ScrrenTopPosY, ScrrenBottomPosY);
            m_list[LastCnt] = sp;
            return sp.endPoint.Y;
        }
        /// <summary>
        /// 現在選択している曲線の情報樹徳
        /// </summary>
        /// <returns></returns>
        public BezierPoint GetBezierPoint()
        {
            return m_list[m_SelectPoint];
        }
        /// <summary>
        /// 開始点取得
        /// </summary>
        /// <returns></returns>
        public int GetFirstStartPointY()
        {
            return m_list[0].startPoint.Y;
        }
        /// <summary>
        /// 終了点取得
        /// </summary>
        /// <returns></returns>
        public int GetEndPointY()
        {
            var LastCnt = m_list.Count() - 1;
            return m_list[LastCnt].endPoint.Y;
        }
        /// <summary>
        /// 選択している線を直線にする
        /// </summary>
        public void StraightLineEdit()
        {
            //点を選択してないときは編集できない
            if (m_SelectMode == SelectMode.None)
            {
                MessageBox.Show("点を選択していません");
                return;
            }
            //終了点は編集できない
            if (m_SelectMode == SelectMode.SelectEnd)
            {
                MessageBox.Show("最後の点は編集できません。");
                return;
            }
            //開始点と終了点で引いたベクトルの中間に制御点を置く
            int  posX = m_list[m_SelectPoint].endPoint.X - m_list[m_SelectPoint].startPoint.X ;
            int  posY = m_list[m_SelectPoint].endPoint.Y - m_list[m_SelectPoint].startPoint.Y;

            posX /= 2;
            posY /= 2;

            BezierPoint bp = m_list[m_SelectPoint];

            bp.controlPoint1.X = posX + bp.startPoint.X;
            bp.controlPoint2.X = posX + bp.startPoint.X;
            bp.controlPoint1.Y = posY + bp.startPoint.Y;
            bp.controlPoint2.Y = posY + bp.startPoint.Y;

            m_list[m_SelectPoint] = bp;
        }
        /// <summary>
        /// CSVデータからグラフの点データを読み込む
        /// </summary>
        /// <returns></returns>
        public void LoadGraph(string s)
        {
            CurveEditorInit();
            m_list = m_stream.Load(s);
        }
        /// <summary>
        /// CSVデータにグラフの点データを書き込む
        /// </summary>
        /// <returns></returns>
        public void SaveGraph(string s)
        {
            m_stream.Save(ref m_list,s);
        }
        /// <summary>
        /// 戻る
        /// </summary>
        public void UnDo()
        {
            //無選択状態に
            m_SelectMode = SelectMode.None;
            CancelMovePoint();
            m_SelectPoint = 0;

            if (m_UnDoStack.Count <= 1)
            {
                //すでに初期状態のグラフデータなら進むスタックに余計なデータを入れさせない
                if (isListMatch(ref m_list, m_UnDoStack.Peek())) return;
                //初期状態のグラフデータ入れる
                m_ReDoStack.Push(new List<BezierPoint>(m_list));
                m_list = new List<BezierPoint>(m_UnDoStack.Peek());//初期状態のグラフ

                return;
            }
            m_ReDoStack.Push(new List<BezierPoint>(m_list));
            m_list = m_UnDoStack.Pop();//戻る
        }
        /// <summary>
        /// 進む
        /// </summary>
        public void ReDo()
        {
            //一回も戻っていなければ進まない
           if (m_ReDoStack.Count - 1 <= -1) return;
            //無選択状態に
            m_SelectMode = SelectMode.None;
            CancelMovePoint();
            m_SelectPoint = 0;
            m_UnDoStack.Push(new List<BezierPoint>(m_list));//戻るの方にデータを保存
            m_list = m_ReDoStack.Pop();//進む    
        }
        /// <summary>
        /// 左マウスクリック後のグラフデータをスタックに保存
        /// </summary>
        public void SaveMemento_Click()
        {
            if (m_UnDoStack.Count() != 0)
            {     //余計なデータを保存していたら削除
                if (isListMatch(ref m_list, m_UnDoStack.Peek()))
                {
                    DeleteMemento();
                    return;
                }
            }
            //点選択を解除させたときに余計なデータを保存させない
            if (m_SelectMode == SelectMode.None && m_UnDoStack.Count != 0) return;
   
            m_UnDoStack.Push(new List<BezierPoint>(m_list));
            m_ReDoStack.Clear();
        }
        /// <summary>
        /// 操作した後のグラフデータをスタックに保存
        /// </summary>
        public void SaveMemento()
        {
            if (m_UnDoStack.Count() != 0)
            {
                //余計なデータを保存していたら削除
                if (isListMatch(ref m_list, m_UnDoStack.Peek()))
                {
                    DeleteMemento();
                    return;
                }
            }
            //データを保存
            m_UnDoStack.Push(new List<BezierPoint>(m_list));
            m_ReDoStack.Clear();
        }
        /// <summary>
        /// 余計なデータを保存していたら削除させる
        /// </summary>
        public void DeleteMemento()
        {
            //最初に保存してあるデータは削除させない
            if (m_UnDoStack.Count <= 1) return;        
            m_UnDoStack.Pop();        
        }
        /// <summary>
        ///スタックにたまっているデータを破棄
        /// </summary>
        public void ClearSaveMemento()
        {
            m_UnDoStack.Clear();
            m_ReDoStack.Clear();
        }
        /// <summary>
        /// 今のグラフと前の操作時のグラフが一致しているか
        /// </summary>
        /// <param name="list"></param>
        /// <param name="st"></param>
        /// <returns></returns>
        public bool isListMatch(ref List<BezierPoint> list,  List<BezierPoint> st)
        {
            int listSize = list.Count();
            if (listSize != st.Count()) return false;
            for(int i = 0; i < listSize; i++)
            {
                if (!list[i].Equals(st[i])) return false;
            }
            return true;
        }

        public  List<BezierPoint> GetGraph()
        {
            return m_list;
        }

    }
}

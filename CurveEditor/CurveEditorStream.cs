using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
namespace CurveEditor
{
    //CSVからグラフを読み書きするクラス
    class CurveEditorStream
    {
        /// <summary>
        /// 文字列から座標に変換
        /// </summary>
        /// <param name="values"></param>
        CurvePointControl.BezierPoint ToBezierPoint(string[] values)
        {
            //文字列を数値に変換
            CurvePointControl.BezierPoint bp = new CurvePointControl.BezierPoint();
            bp.startPoint.X = CMath.ChageNomalPosX(decimal.Parse(values[0]));
            bp.startPoint.Y = CMath.ChageNomalPosY(decimal.Parse(values[1]));
            bp.controlPoint1.X = CMath.ChageNomalPosX(decimal.Parse(values[2]));
            bp.controlPoint1.Y = CMath.ChageNomalPosY(decimal.Parse(values[3]));
            bp.controlPoint2.X = CMath.ChageNomalPosX(decimal.Parse(values[4]));
            bp.controlPoint2.Y = CMath.ChageNomalPosY(decimal.Parse(values[5]));
            bp.endPoint.X = CMath.ChageNomalPosX(decimal.Parse(values[6]));
            bp.endPoint.Y = CMath.ChageNomalPosY(decimal.Parse(values[7]));
            return bp;
        }
        /// <summary>
        /// CSVからデータを読み込み
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public List<CurvePointControl.BezierPoint> Load(string s)
        {
            List<CurvePointControl.BezierPoint> list = new List<CurvePointControl.BezierPoint>();
            StreamReader file = new StreamReader(s);

              //末尾まで繰り返す
            while (!file.EndOfStream)
            {
               // CSVファイルの一行を読み込む
                string line = file.ReadLine();
                // 読み込んだ一行をカンマ毎に分けて配列に格納する
                string[] values = line.Split(',');
                CurvePointControl.BezierPoint b = ToBezierPoint(values);
                list.Add(b);
            }
            file.Close();
            return list;
        }
        /// <summary>
        /// CSVにデータを書き込む
        /// </summary>
        /// <param name="List"></param>
        public void Save(ref List<CurvePointControl.BezierPoint> List,string s)
        {
            try
            {
                StreamWriter file = new StreamWriter(s, false, Encoding.UTF8);
                int size = List.Count;
                for(int i = 0; i < size;i++)
                {
                    //座標を0～1の間に変換し文字列化させる
                    string [] name = new string[4];
                    name = ToSting(List[i]);
                    file.Write(name[0]+ "," + name[1] + "," + name[2] + "," + name[3] + "\n");
                }
                file.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);       // エラーメッセージを表示
            }
        }
        /// <summary>
        ///  //座標を0～1の間に変換し文字列化させる
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public string[] ToSting( CurvePointControl.BezierPoint bs)
        {
            //座標を0～1の間に変換し文字列化させる
            string[] name = new string[4];
            name[0] = CMath.ChageDecimalPosX(bs.startPoint.X).ToString()
                + "," + CMath.ChageDecimalPosY(bs.startPoint.Y).ToString();
            name[1] = CMath.ChageDecimalPosX(bs.controlPoint1.X).ToString()
                + "," + CMath.ChageDecimalPosY(bs.controlPoint1.Y).ToString();
            name[2] = CMath.ChageDecimalPosX(bs.controlPoint2.X).ToString()
                + "," + CMath.ChageDecimalPosY(bs.controlPoint2.Y).ToString();
            name[3] = CMath.ChageDecimalPosX(bs.endPoint.X).ToString()
                + "," + CMath.ChageDecimalPosY(bs.endPoint.Y).ToString();

            return name;
        }
    }
}

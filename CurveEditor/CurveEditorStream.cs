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
        /// 文字列数値に変換
        /// </summary>
        /// <param name="values"></param>
        CurvePointControl.BezierPoint ConvertBezierPoint(string[] values)
        {
            //文字列を数値に変換
            CurvePointControl.BezierPoint bp = new CurvePointControl.BezierPoint();
            bp.startPoint.X = int.Parse(values[0]);
            bp.startPoint.Y = int.Parse(values[1]);
            bp.controlPoint1.X = int.Parse(values[2]);
            bp.controlPoint1.Y = int.Parse(values[3]);
            bp.controlPoint2.X = int.Parse(values[4]);
            bp.controlPoint2.Y = int.Parse(values[5]);
            bp.endPoint.X = int.Parse(values[6]);
            bp.endPoint.Y = int.Parse(values[7]);
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
                CurvePointControl.BezierPoint b = ConvertBezierPoint(values);
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
                    //座標を文字列化させる
                    string [] name = new string[4];
                    name[0] = List[i].startPoint.X.ToString() + "," + List[i].startPoint.Y.ToString();
                    name[1] = List[i].controlPoint1.X.ToString() + "," + List[i].controlPoint1.Y.ToString();
                    name[2] = List[i].controlPoint2.X.ToString() + "," + List[i].controlPoint2.Y.ToString();
                    name[3] = List[i].endPoint.X.ToString() + "," + List[i].endPoint.Y.ToString();

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
    }
}

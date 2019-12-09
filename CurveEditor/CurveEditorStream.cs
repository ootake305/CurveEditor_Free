using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
namespace CurveEditor
{
    class CurveEditorStream
    {
        /// <summary>
        /// CSVからデータを読み込み
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public List<CurvePointControl.BezierPoint> Load(string s)
        {
            List<CurvePointControl.BezierPoint> list = new List<CurvePointControl.BezierPoint>();
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

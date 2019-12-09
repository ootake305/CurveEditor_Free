using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurveEditor
{
    //カーブエディタで使う数学関数
    class CMath
    {
        /// <summary>
        /// 小数から元の座標に変換
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static int ChageNomalPosX(decimal Value)
        {
            decimal num = Value;

            decimal num2 = (num * (decimal)500);
            int num3 = Convert.ToInt32(num2);

            num3 += 10;//10からグラフが始まってるので右に+10
            return num3;
        }
        /// <summary>
        /// 元の座標から0～1の間に変換
        /// </summary>
        /// <param name="posX"></param>
        /// <returns></returns>
        public static decimal ChageDecimalPosX(int posX)
        {
            posX = Math.Max(0, posX - 10);　//10からグラフが始まってるので右に-10
            return (decimal)posX / (decimal)500;
        }
        
        /// <summary>
        /// 小数から元の座標に変換
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static int ChageNomalPosY(decimal Value)
        {
            decimal num = Value;

            decimal num2 = num * (decimal)500;
            int num3 = Convert.ToInt32(num2);

            num3 = num3 - 500;
            num3 = num3 * -1;
            num3 += 10;//10からグラフが始まってるので右に+10

            return num3;
        }
        /// <summary>
        /// 元の座標から0～1の間に変換
        /// </summary>
        /// <param name="posY"></param>
        /// <returns></returns>
        public static decimal ChageDecimalPosY(int posY)
        {
            //  posY = Math.Min(500, posY - 10);
            //10からグラフが始まってるので右に-10
            posY = Math.Max(0, posY - 10);
            posY = posY - 500;
            posY = posY * -1;

            return Math.Max(0, (decimal)posY / (decimal)500);
        }
        /// <summary>
        /// 値を制限させる
        /// </summary>
        /// <param name="x"></param>
        /// <param name="minVal"></param>
        /// <param name="maxVal"></param>
        /// <returns></returns>
        public static int Clamp(int x, int minVal, int maxVal)
        {
            return Math.Min(Math.Max(minVal, x), maxVal);
        }
    }
}

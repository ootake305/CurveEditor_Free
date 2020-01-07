using System;

namespace CurveEditor
{
    //カーブエディタで使う数学関数
    class CMath
    {
        static  int GraphSize = 500;
        /// <summary>
        /// 小数から元の座標に変換
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static int ChageNomalPosX(decimal Value)
        {
            decimal num = Value;

            decimal num2 = (num * (decimal)GraphSize);
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
            return (decimal)posX / (decimal)GraphSize;
        }
        /// <summary>
        /// 元の座標から0～1の間に変換(tを求めるための正規化)
        /// </summary>
        /// <param name="startPosX"></param>
        /// <returns></returns>
        public static decimal ChageDecimalT(decimal startPosX, decimal endPosX2, decimal currentPos)
        {
            decimal divisor = endPosX2 - startPosX;//割る数
            decimal dividend = currentPos - startPosX;///割られる数
            return  dividend / divisor;
        }

        /// <summary>
        /// 小数から元の座標に変換
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static int ChageNomalPosY(decimal Value)
        {
            decimal num = Value;

            decimal num2 = num * (decimal)GraphSize;
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
            posY = posY - GraphSize;
            posY = posY * -1;

            return Math.Max(0, (decimal)posY / (decimal)GraphSize);
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

        /*
         3次方程式を解くやつ　結局使い方がわからず
        // real number only
        // swiftlint:disable identifier_name
        // ax^3 + bx^2 + cx + d = 0
        public static double[] solveCubicEquation(double a, double b, double c, double d)
        {
            if (a == 0)
            {
                return solveQuadraticEquation(b, c, d);
            }
            else
            {
                // should be dedoublee `a`
                double A = b / a;
                double B = c / a;
                double C = d / a;
                return solveCubicEquation(A,B,C);
            }
        }


        // MARK: - Private
        // x^3 + Ax^2 + Bx + C = 0
        private static double[] solveCubicEquation(double A, double B, double C)
        {
            if (A == 0)
            {
                return solveCubicEquation( B, C);
            }
            else
            {
                // x = X - A/3 (compdoubleing the cubic. should be dedoublee `Ax^2`)
                double p = B - (Math.Pow(A, 2.0) / 3.0);
                double q = C - ((A * B) / 3.0) + ((2.0 / 27.0) * Math.Pow(A, 3.0));
                double[] roots = solveCubicEquation(p, q);
                // roots
               int num = roots.Length;
                for(int i = 0; i < num; i++)
                {
                    roots[i] = roots[i] - A / 3.0;
                }
                return roots;
             //   return roots.map { $0 - A / 3.0 };
            }
        }

        // x^3 + px + q = 0
        // for avoid considering the complex plane, I choose geometric solution.
        // respect François Viète
        // ref: https://pomax.github.io/bezierinfo/#extremities
        private static double[] solveCubicEquation(double p, double q)
        {
            double p3 = p / 3.0;
            double q2 = q / 2.0;
            double discriminant = Math.Pow(q2, 2.0) + Math.Pow(p3, 3.0); // D: discriminant

            if (discriminant < 0.0)
            {
                // three possible real roots
                double r = Math.Sqrt(Math.Pow(-p3, 3.0));
                double t = -q / (2.0 * r);
                double cosphi = Math.Min(Math.Max(t, -1.0), 1.0);
                double phi = Math.Acos(cosphi);
                double c = 2.0 * cuberoot(r);
                double root1 = c * Math.Cos(phi / 3.0);
                double root2 = c * Math.Cos((phi + 2.0 * Math.PI) / 3.0);
                double root3 = c * Math.Cos((phi + 4.0 * Math.PI) / 3.0);
                return new double[3] { root1, root2 , root3 };
            }
            else if (discriminant == 0.0)
            {
                // three real roots, but two of them are equal
                double u;

                if (q2 < 0.0)
                {
                    u = cuberoot(-q2);
                }
                else
                {
                    u = -cuberoot(q2);
                }
                double root1 = 2.0 * u;
                double root2 = -u;
                return new double[2] { root1, root2 };
            }
            else
            {
                // one real root, two complex roots
                double sd = Math.Sqrt(discriminant);
                double u = cuberoot(sd - q2);
                double v = cuberoot(sd + q2);
                double root1 = u - v;
                return new double[1] { root1 };
            }
        }


        public static double[] solveQuadraticEquation(double a, double b, double c)
        {
            if (a == 0.0)
            {
                // seems linear equation
                double root = solveLinearEquation(a, c);
          
                if(root == double.NaN)
                {
                    return new double[1] { root };
                }
                else
                {
                    return new double[0];
                }

              //  return [root].filter({ !$0.isNaN });
            }

            double discriminant = Math.Pow(b, 2.0) - (4.0 * a * c);// D = b^2 - 4ac
            if (discriminant < 0.0)
            {
                return new double[0];
            }
            else if (discriminant == 0.0)
            {
                double root = -b / (2.0 * a);
                return new double[1] { root };
            }
            else
            {
                double d = Math.Sqrt(discriminant);
                double root1 = (-b + d) / (2.0 * a);
                double root2 = (-b - d) / (2.0 * a);
                return new double[2]{ root1, root2};
            }
        }

        private static double cuberoot(double v)
        {
            double c = 1.0 / 3.0;
            if (v < 0)
            {
                return -Math.Pow(-v, c);
            }
            else
            {
                return Math.Pow(v, c);
            }
        }

        public static double solveLinearEquation(double a, double b)
        {
            if (a == 0.0)
            {
                return double.NaN;
            }
            else
            {
                double root = -b / a;
                return root;
            }
        }*/
    }
}

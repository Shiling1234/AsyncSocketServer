using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinaSy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("input number N:");
            int n = int.Parse(Console.ReadLine());
            int[] a = new int[n];
            int[] m = new int[n];
            int M = 1;
            for (int i = 0; i < n; i++)
            {
                Console.Write("intput a{0}:", i + 1);
                a[i] = int.Parse(Console.ReadLine());
                Console.Write("input m{0}:", i + 1);
                m[i] = int.Parse(Console.ReadLine());
                M *= m[i];
            }
            int x = 0;
            for (int i = 0; i < n; i++)
            {
                x += a[i] * Reverse(M / m[i], m[i]) * M / m[i];
            }
            Console.Write("x={0}", modZ(x, M));
            Console.Read();
        }
        //求逆_M-1=1(mod _m) 既b[i]
        public static int Reverse(int _M, int _m)
        {
            int m = _m;
            int q1 = 0, q2 = 0, q3 = 0, t0 = 0, t1 = 0, t2 = 0;
            int i;
            for (i = 0; _m % _M != 0; i++)
            {
                if (i != 0)
                {
                    q1 = q2;
                    q2 = q3;
                    t0 = t1;
                    t1 = t2;
                }
                q3 = _m / _M;
                int temp = _m % _M;
                _m = _M;
                _M = temp;
                if (i == 0) t2 = 0;
                else if (i == 1) t2 = 1;
                else t2 = modZ(t0 - q1 * t1, m);
            }
            if (i == 1) return modZ(t1 - q2 * t2, m);
            if (i == 0) return 1;
            return modZ(t2 - q3 * (t1 - q2 * t2), m);
        }
        //求余数 保证余数为正整数
        public static int modZ(int _a, int _m)
        {
            while (_a % _m < 0)
            {
                _a += _m;
            }
            return _a % _m;
        }
    }

}

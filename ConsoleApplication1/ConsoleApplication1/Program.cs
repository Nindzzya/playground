using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] x = new string[5];           
            for(int i=0;i<x.Length;i++)
            {
                x[i] = Console.ReadLine();
            }
            var result1= doAscending(x);
            int[] num = new int[x.Length];
            for (int i = 0; i < x.Length; i++)
                num[i] = int.Parse(x[i]);
            var result2 = num.OrderBy(a => a).ToArray();
            var rev = new string(reverseString("Hello"));
        }
        public static int[] doAscending(string[] x)
        {
            int len = x.Length;
            int[] num = new int[len];
            for (int i = 0; i < len; i++)
                num[i] = int.Parse(x[i]);
            for (int j = 0; j < len; j++)
            {
                for (int i = j; i < len; i++)
                {
                  if(num[j] > num[i])
                    {
                        num[j] = num[j] ^ num[i];
                        num[i] = num[j] ^ num[i];
                        num[j] = num[j] ^ num[i];                        
                    }

                }
            }
            return num;
        }
        public static char[] reverseString(string x)
        {
            char[] nor = x.ToArray();
            char[] rev = new char[x.Length];
            for (int i = 0; i < x.Length; i++)
                rev[i] = x[x.Length -1 - i];
            return rev;
        }

        public static bool checkifPalindrome(string x)
        {
            char[] str = x.ToArray();

        }
    }
}

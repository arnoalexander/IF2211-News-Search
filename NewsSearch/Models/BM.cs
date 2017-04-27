using System;

namespace SearchNews.Models
{
    public class BM
    {
        private string input;
        private int[] skipArray;

        public BM()
        {
            input = "";
            skipArray = new int[1000];
        }

        public BM(String temp)
        {
            input = temp;
            skipArray = new int[1000];
            //Init semua dengan -1
            for (int i = 0; i < skipArray.Length; i++)
            {
                skipArray[i] = -1;
            }
            //Isi dengan last occurence character
            for (int i = 0; i < input.Length; i++)
            {
                skipArray[(int)input[i]] = i;
            }
        }

        private bool equals(char c1, char c2)
        {
            int temp = (int)'A' - (int)'a';
            if (c1 >= 'A' && c1 <= 'Z' && c2 >= 'a' && c2 <= 'z')
            {
                return ((int)c1 - (int)c2 == temp);
            }
            else if (c2 >= 'A' && c2 <= 'Z' && c1 >= 'a' && c1 <= 'z')
            {
                return ((int)c2 - (int)c1 == temp);
            }
            else
            {
                return (c2 == c1);
            }
        }

        public int SearchByBM(string T)
        {
            int i = 0;
            while (i <= (T.Length - input.Length))
            {
                int j = input.Length - 1;
                while (j >= 0 && input[j] == T[i + j])
                {
                    j--;
                }
                if (j < 0)
                {
                    System.Diagnostics.Debug.WriteLine("ketemu di idx pertama yaitu: " + i);
                    return i;
                }
                else
                {
                    i += Math.Max(1, j - skipArray[T[i + j]]);
                }
            }
            return -1;
        }

        public string GetString()
        {
            return input;
        }
    }
}

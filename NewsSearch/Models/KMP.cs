public class KMP
{
    private string input;
    private int[] kmpNext;

	public KMP()
	{
        input = "";
        kmpNext = new int[input.Length + 10];
	}

    public KMP(string temp)
    {
        input = temp;
        kmpNext = new int[input.Length + 10];
        for (int i = 0; i <= input.Length + 9; ++i)
        {
            kmpNext[i] = -1;
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
    public void PreKMP()
    {
        int i;
        int j;

        /* ALGORITMA */
        i = 0;
        j = -1;
        kmpNext[0] = -1;
        while (i < input.Length - 1)
        {
            while (j > -1 && !(equals(input[i],input[j])) && j < input.Length)
            {
                j = kmpNext[j];
            }
            ++i;
            ++j;
            if (i < input.Length - 1)
            {
                if (equals(input[i],input[j]))
                {
                    kmpNext[i] = kmpNext[j];
                }
                else
                {
                    kmpNext[i] = j;
                }
            }
            else
            {
                kmpNext[i] = j;
            }
        }

    }

    public int SearchByKMP(string T)
    {
        int i;
        int j;
        int Next;
        kmpNext = new int[input.Length + 1];
        for (i = 0; i <= input.Length; ++i)
        {
            kmpNext[i] = -1;
        }
        PreKMP();
        kmpNext[input.Length] = 0;
        int LengthT = T.Length;
        bool[] found = new bool[LengthT];
        int LengthInput = input.Length;
        for (i = 0; i < LengthT; ++i)
        {
            found[i] = false;
        }
        i = 0;
        while (i < LengthT - LengthInput)
        {
            j = 0;
            while (j < LengthInput && equals(T[i + j],input[j]))
            {
                ++j;
            }
            found[i] = (j >= LengthInput);
            Next = j - kmpNext[j];
            i += Next;
        }

        i = 0;
        bool exit = false;
        while (i < LengthT && !exit)
        {
            exit = found[i];
            ++i;
        }

        int temp = i - 1;
        System.Diagnostics.Debug.WriteLine("ketemu di idx pertama yaitu: " +temp);

        if (exit)
        {
            return i-1;
        }
        else
        {
            return -1;
        }
    }

    public string GetString()
    {
        return input;
    }
}

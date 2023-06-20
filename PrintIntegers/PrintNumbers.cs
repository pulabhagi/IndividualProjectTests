using System;

class PrintNumbers
{
    public static void Main()
    {
        string result = "";
        int s3 = 0, s5 = 0;
        for (int i = 1; i <= 100; i++)
        {
            s3++;
            s5++;
            if (s3 == 3)
            {
                result += "fizz";
                s3 = 0;
            }
            if (s5 == 5)
            {
                result += "buzz";
                s5 = 0;
            }
            if (result.Length == 0)
                Console.WriteLine(i);
            else
                Console.WriteLine(result);
            result = "";
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a number: ");
            if (int.TryParse(Console.ReadLine(), out int result)) {
                if (result > 0) {
                    Console.WriteLine("The value is bigger than zero");
                }
                else if (result < 0) {
                    Console.WriteLine("The value is less than zero");
                }
                else if (result == 0)
                {
                    Console.WriteLine("You have entered zero");
                }
                else
                {
                    Console.WriteLine("Invalid input plese enter an integer");
                }
            }
            int[] arr = {1, 2, 5 , 67, 29, 13};
            Console.WriteLine("average of the {1, 2, 5 , 67, 29, 13}: " + CalculateAverage(arr));
            Console.WriteLine("max of the {1, 2, 5 , 67, 29, 13}: " + findMax(arr));
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
        static double CalculateAverage(int[] arr) {
            int sum = 0;
            foreach(var b in arr)
            {
                sum += b;
            }
            return sum / arr.Length;
        }

        static int findMax(int[] arr)
        {
            int max = 0;
            foreach (var i in arr)
            {
                if (i > max) { 
                max = i;
                }
            }
            return max;
        }
    }
}

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
            if (int.TryParse(Console.ReadLine(), out int result)){
                if (result > 0) {
                    Console.WriteLine("The value is bigger than zero");
                }
                else if (result < 0) {
                    Console.WriteLine("The value is less than zero");
                }
                else if(result == 0)
                {
                    Console.WriteLine("You have entered zero");
                }
                else
                {
                    Console.WriteLine("Invalid input plese enter an integer");
                }
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}

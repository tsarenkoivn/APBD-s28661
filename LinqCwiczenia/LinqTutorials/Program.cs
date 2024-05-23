using System;

namespace LinqTutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = LinqTasks.Task1();
            LinqTasks.ShowTask2Result();

            Console.WriteLine("\nTask 3:");
            Console.WriteLine(LinqTasks.Task3());

            Console.WriteLine("\nTask 4:");
            foreach (var emp in LinqTasks.Task4())
            {
                Console.WriteLine(emp);
            }

            Console.WriteLine("\nTask 5:");
            foreach (var obj in LinqTasks.Task5())
            {
                Console.WriteLine(obj);
            }

            Console.WriteLine("\nTask 6:");
            foreach (var obj in LinqTasks.Task6())
            {
                Console.WriteLine(obj);
            }

            Console.WriteLine("\nTask 7:");
            foreach (var obj in LinqTasks.Task7())
            {
                Console.WriteLine(obj);
            }

            Console.WriteLine("\nTask 8:");
            Console.WriteLine(LinqTasks.Task8());

            Console.WriteLine("\nTask 9:");
            Console.WriteLine(LinqTasks.Task9());

            Console.WriteLine("\nTask 10:");
            foreach (var obj in LinqTasks.Task10())
            {
                Console.WriteLine(obj);
            }

            Console.WriteLine("\nTask 11:");
            foreach (var obj in LinqTasks.Task11())
            {
                Console.WriteLine(obj);
            }

            Console.WriteLine("\nTask 12:");
            foreach (var emp in LinqTasks.Task12())
            {
                Console.WriteLine(emp);
            }

            Console.WriteLine("\nTask 13:");
            int[] arr = { 1, 1, 1, 1, 1, 1, 10, 1, 1, 1, 1 };
            Console.WriteLine(LinqTasks.Task13(arr));

            Console.WriteLine("\nTask 14:");
            foreach (var dept in LinqTasks.Task14())
            {
                Console.WriteLine(dept);
            }
        }
    }
}

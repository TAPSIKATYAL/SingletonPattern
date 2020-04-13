using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monostate
{
    public class CEO
    {
        private static string Name;
        private static int Age;

        public int Age1 { get => Age; set => Age = value; }
        public string Name1 { get => Name; set => Name = value; }
    }
    public class Program
    {
        static void Main(string[] args)
        {
            var ceo = new CEO();
            ceo.Name1 = "Tapsi Katyal";
            ceo.Age1 = 55;
            var ceo2 = new CEO();
            Console.WriteLine(ceo2.Age1 + ceo2.Name1);
            Console.ReadLine();
        }
    }
}
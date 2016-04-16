using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParalelTest {
    class Program {
        static void Main(string[] args) {
            var list = new List<string>() {
                "ace",
                "blink eyes",
                "catter",
                "dozen of fluits",
                "eagle"
            };
            Console.WriteLine("start!");
            Parallel.ForEach(list, (s) => {
                Thread.Sleep(s.Length * 500);
                Console.WriteLine(s);
                list.Remove(s);
            });
            Console.WriteLine("end!");
            Console.ReadLine();
        }
    }
}

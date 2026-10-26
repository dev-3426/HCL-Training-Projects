using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class Product
    {
        public int Id { get; set; }
        private string name;
        public string Name { get { return name; } set }
        name = value; 

           
    }

    public class Prog
    {
        Product pro = new Product();
        Console.WriteLine(Product.Name);

    }
}

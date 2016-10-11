﻿using Goods.BusinessLogic;
using Goods.DbModels;
using Multithreading;
using System;
using DataAccess;

namespace App1
{
    class Program
    {
        static void Main(string[] args)
        {
            const string CategoryPath = @"C:\Users\Слава\Desktop\softserve ac\App1\files\Categories.csv";
            const string ProducerPath = @"C:\Users\Слава\Desktop\softserve ac\App1\files\Producers.csv";
            const string GoodsPath = @"C:\Users\Слава\Desktop\softserve ac\App1\files\Goods.csv";
            const string GoodsXmlPath = @"C:\Users\Слава\Desktop\softserve ac\App1\files\Goods.xml";
            string[] PathesofDir = new string[] { @"C:\Users\Слава\Desktop\softserve ac\App1\files\", @"C:\Users\Слава\Desktop\C#", @"C:\Users\Слава\Desktop\Новая папка" };
            try
            {
                //ParseManager parser = new ParseManager();

                //var patheses = parser.GetPathes(@"C:\Users\Слава\Desktop\Taxi-master", @"*.cs");

                //parser.ManageThreadWork(patheses, 5, "!=");

                GoodsContext goodsc = new GoodsContext();
                ProducerEFManager producermanager = new ProducerEFManager();
                CategoryEFManager categorymanager = new CategoryEFManager();
                var cat1 = categorymanager.Get(1);
                var prod = producermanager.Get(1);
                Good goodef = new Good { Id = 10, Name = "asdasd", Price = 333, Category = cat1, Producer = prod };
                Good goodef2 = new Good { Id = 14, Name = "asdasd", Price = 333, CategoryId = 2, ProducerId = 1 };
                GoodEFManager goodmang = new GoodEFManager();
                //  goodmang.Add(goodef2);
                var tempobj = goodmang.Get(1);






                Console.WriteLine("Done!");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            Console.ReadKey();
        }
    }
}


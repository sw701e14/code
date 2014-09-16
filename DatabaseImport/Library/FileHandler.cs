using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public static class FileHandler
    {
        static string PATH = @"..\..\..\..\TestData\";

        public static string[] LoadFile(string fileName)
        {
            return File.ReadAllLines(PATH + fileName);
        }
    }
}

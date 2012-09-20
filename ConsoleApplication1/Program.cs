using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.IO;

using Nikos.Collections;

namespace ConsoleApplication1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            byte[] a = new byte[] { 1, 0, 1, 0 };

            using (FileStream f = new FileStream(@"D:\output.cod", FileMode.Create))
            {
                f.Write(a,0,a.Length);
            }


            Pair<string, IDictionary<char, string>> cod;
            using (FileStream stream = new FileStream(@"D:\a.txt", FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string txt = reader.ReadToEnd();



                    cod = Nikos.Algorithms.Huffman.Encode(txt);
                }
            }

            //using (FileStream stream = new FileStream(@"D:\output.txt", FileMode.Create))
            //{
            //    using (StreamWriter writer = new StreamWriter(stream))
            //    {
            //        writer.Write(cod.Key);
            //    }
            //}

            using (FileStream stream = new FileStream(@"D:\output.cod", FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, cod.Value);
            }
        }
    }
}

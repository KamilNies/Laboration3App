using System;
using System.IO;
using System.Linq;

namespace Laboration3
{
    class Program
    {
        //Signatures
        static byte[] png = { 137, 80, 78, 71, 13, 10, 26, 10 };
        static byte[] bmp = { 66, 77 };

        static void Main(string[] args)
        {
            string pathScreenDumpPNG = @"..\..\..\Lab3ScreenDump-900x300.png";
            string pathTestBMP = @"..\..\..\Test_400x200.bmp";
            string pathTestGIF = @"..\..\..\Test_400x200.gif";
            string pathTestJPG = @"..\..\..\Test_400x200.jpg";
            string pathTestPNG = @"..\..\..\Test_400x200.png";

            try
            {
                byte[] bytes = GetByteArrayFromPath(pathScreenDumpPNG);
                PrintImageInformation(bytes);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found.");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Directory not found.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Empty path.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
        private static bool IsPNG(byte[] array)
        {
            return array[0..8].SequenceEqual(png);
        }
        private static bool IsBMP(byte[] array)
        {
            return array[0..2].SequenceEqual(bmp);
        }
        private static int ReturnPNGWidth(byte[] array)
        {
            int sum = 0;
            for (int i = 16, exponent = 6; i < 20; i++, exponent -= 2)
            {
                sum += array[i] * (int)Math.Pow(16, exponent);
            }
            return sum;
        }
        private static int ReturnBMPWidth(byte[] array)
        {
            int sum = 0;
            for (int i = 18, exponent = 0; i < 21; i++, exponent += 2)
            {
                sum += array[i] * (int)Math.Pow(16, exponent);
            }
            return sum;
        }
        private static int ReturnPNGHeight(byte[] array)
        {
            int sum = 0;
            for (int i = 20, exponent = 6; i < 24; i++, exponent -= 2)
            {
                sum += array[i] * (int)Math.Pow(16, exponent);
            }
            return sum;
        }
        private static int ReturnBMPHeight(byte[] array)
        {
            int sum = 0;
            for (int i = 22, exponent = 0; i < 25; i++, exponent += 2)
            {
                sum += array[i] * (int)Math.Pow(16, exponent);
            }
            return sum;
        }
        private static void PrintImageInformation(byte[] array)
        {
            if (IsPNG(array))
            {
                Console.WriteLine(new string('-', 70));
                Console.WriteLine("This is a .png image. Resolution: {0}x{1} pixels", ReturnPNGWidth(array), ReturnPNGHeight(array));
                Console.WriteLine(new string('-', 70));
                Console.WriteLine("Chunk type\tSize (bytes)");
                Console.WriteLine(new string('-', 70));
                Chunks.GeneralizedChunkFinder(array);
            }
            else if (IsBMP(array))
            {
                Console.WriteLine(new string('-', 70));
                Console.WriteLine("This is a .bmp image. Resolution: {0}x{1} pixels", ReturnBMPWidth(array), ReturnBMPHeight(array));
                Console.WriteLine(new string('-', 70));
            }
            else
            {
                Console.WriteLine(new string('-', 70));
                Console.WriteLine("This is not a valid .bmp or .png file!");
                Console.WriteLine(new string('-', 70));
            }
        }
        private static byte[] GetByteArrayFromPath(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[fs.Length];
                int numBytesToRead = (int)fs.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = fs.Read(bytes, numBytesRead, numBytesToRead);
                    if (n == 0)
                    {
                        break;
                    }
                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                return bytes;
            }
        }
    }
}

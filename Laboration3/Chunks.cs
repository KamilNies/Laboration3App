using System;
using System.Collections.Generic;
using System.Linq;

namespace Laboration3
{
    class Chunks
    {
        //Chunk type signatures

        private static readonly string[] chunkNames =
        {
            "IHDR", "PLTE", "IDAT", "IEND", "tRNS", "cHRM", "gAMA", "iCCP", "sBIT",
            "sRGB", "tEXt", "zTXt", "iTXt", "bKGD", "hIST", "pHYs", "sPLT", "tIME"
        };

        private static readonly List<byte[]> chunkSignatures = new List<byte[]>()
        {
            new byte[] { 73, 72, 68, 82 }, new byte[] { 80, 76, 84, 69 },
            new byte[] { 73, 68, 65, 84 }, new byte[] { 73, 69, 78, 68 },
            new byte[] { 116, 82, 78, 83 }, new byte[] { 99, 72, 82, 77 },
            new byte[] { 103, 65, 77, 65 }, new byte[] { 105, 67, 67, 80 },
            new byte[] { 115, 66, 73, 84 }, new byte[] { 115, 82, 71, 66 },
            new byte[] { 116, 69, 88, 116 }, new byte[] { 122, 84, 88, 116 },
            new byte[] { 105, 84, 88, 116 }, new byte[] { 98, 75, 71, 68 },
            new byte[] { 104, 73, 83, 84 }, new byte[] { 112, 72, 89, 115 },
            new byte[] { 115, 80, 76, 84 }, new byte[] { 116, 73, 77, 69 },
        };

        //The chunks found in our search are stored below:
        private static List<int> chunkSize = new List<int>();
        private static List<string> chunkType = new List<string>();

        public static void GeneralizedChunkFinder(byte[] array)
        {
            //IHDR always comes first if .png; find it, do neccassary calculations, then break.

            /* Index 0 thoughout 7 are reserved for the .png signature, which is why we start at
            index 8 when searching for the IHDR chunk. */
            int nextChunkIndex = 0;
            for (int i = 8; i <= array.Length - 4; i++)
            {
                if (array.Skip(i).Take(4).SequenceEqual(chunkSignatures[0]))
                {
                    byte[] lengthChunkField = array[(i - 4)..i];
                    chunkType.Add("IHDR");
                    chunkSize.Add(ReturnChunkSize(lengthChunkField));
                    nextChunkIndex = i + chunkSize[chunkSize.Count - 1];
                    break;
                }
            }
            //Recursive method to find and store the remaining chunks.
            SequenceJumper(array, nextChunkIndex);

            //Once all the chunks are found, we simply print the chunk tag and size.
            for (int i = 0; i < chunkType.Count; i++)
            {
                Console.WriteLine("{0}\t\t{1}", chunkType[i], chunkSize[i] - 12);
            }
        }
        public static int ReturnChunkSize(byte[] lengthChunkField)
        {
            /* A simple calculator which uses the length chunk field as input to determine
            the size of the entire chunk in bytes. */
            int sum = 0;
            for (int i = 0, exponent = 6; i < lengthChunkField.Length; i++, exponent -= 2)
            {
                sum += lengthChunkField[i] * (int)Math.Pow(16, exponent);
            }
            sum += 12;
            return sum;
        }
        public static void SequenceJumper(byte[] array, int nextChunkIndex)
        {
            /* The chunk signature for IHDR is found at index 0 of the chunkSignatures list. 
            IHDR only appears once in our array, which is why we don't bother starting
            at index 0 when we do our new search. */
            for (int i = 1; i < chunkSignatures.Count; i++)
            {
                if (array.Skip(nextChunkIndex).Take(4).SequenceEqual(chunkSignatures[i]))
                {
                    byte[] lengthChunkField = array[(nextChunkIndex - 4)..nextChunkIndex];
                    chunkType.Add(chunkNames[i]);
                    chunkSize.Add(ReturnChunkSize(lengthChunkField));
                    nextChunkIndex += chunkSize[chunkSize.Count - 1];
                    break;
                }
            }
            //The remaining recursions are protected by an if-statement to prevent overflow.
            if (chunkType[chunkType.Count - 1] != "IEND")
            {
                SequenceJumper(array, nextChunkIndex);
            }
        }
    }
}

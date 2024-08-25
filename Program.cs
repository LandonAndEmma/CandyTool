using System;
using System.IO;
namespace FileExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(
                    "Drag and drop a file onto this exe to extract embedded SWAR and SBNK files.");
                return;
            }
            foreach (string inputFile in args)
            {
                if (File.Exists(inputFile))
                {
                    ExtractFiles(inputFile);
                }
                else
                {
                    Console.WriteLine($"File not found: {inputFile}");
                }
            }
            Console.WriteLine("Extraction completed successfully.");
        }
        static void ExtractFiles(string inputFile)
        {
            byte[] data = File.ReadAllBytes(inputFile);
            int index = 0;
            while (index < data.Length)
            {
                string fileType = null;
                int fileSize = 0;
                int headerStartIndex = 0;
                if (IsMatch(data, index, "SWAR"))
                {
                    fileType = ".swar";
                    fileSize = BitConverter.ToInt32(data, index + 8);
                    headerStartIndex = index;
                    index += 12;
                }
                else if (IsMatch(data, index, "SBNK"))
                {
                    fileType = ".sbnk";
                    fileSize = BitConverter.ToInt32(data, index + 8);
                    headerStartIndex = index;
                    index += 12;
                }
                else
                {
                    index++;
                    continue;
                }
                if (!Directory.Exists("extracted"))
                {
                    Directory.CreateDirectory("extracted");
                }
                string outputFilename = $"extracted/extracted_{index}_{fileType}";
                using (var outputFile = new FileStream(outputFilename, FileMode.Create,
                                                      FileAccess.Write))
                {
                    outputFile.Write(data, headerStartIndex, fileSize + 8);
                }
                while (index < data.Length && data[index] != 0xDD)
                {
                    index++;
                }

                if (index < data.Length)
                {
                    index++;
                }
            }
        }
        static bool IsMatch(byte[] data, int index, string marker)
        {
            for (int i = 0; i < marker.Length; i++)
            {
                if (data[index + i] != marker[i]) return false;
            }
            return true;
        }
    }
}
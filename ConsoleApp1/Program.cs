using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace icrCS2
{
    class Program
    {
        public static int processedSongs = 0;
        public static int skippedSongs = 0;
        public static float globalAR = 0f;
        /// <summary>
        /// Reads osu map metadata and replaces the approachRate value with "approachRate:10"
        /// Cause i cannot read anything else
        /// </summary>
        /// <param name="file"></param>
        public static void ReadDotOsu(string file)
        {
            try
            {
                var lines = File.ReadAllLines(file);
                var foundApproachRate = false;
                var iteratorIndex = 0;
                var index = 0;
                foreach (string line in lines)
                    if (line.Contains("ApproachRate"))
                    {
                        index = iteratorIndex;
                        foundApproachRate = true;
                        break;
                    }
                    else
                        iteratorIndex++;
                if (foundApproachRate)
                {
                    lines[index] = string.Format("ApproachRate:{0}", globalAR);
                    File.WriteAllLines(file, lines);
                    processedSongs++;
                }
            }
            catch
            {
                skippedSongs++;
            }
        }
        public static void GetSubDirectories(string @root)
        {
            // Get all subdirectories
            string[] subdirectoryEntries = Directory.GetDirectories(@root);
            // Loop through them to see if they have any other subdirectories
            foreach (string subdirectory in subdirectoryEntries)
                LoadSubDirs(subdirectory);
        }
        private static void LoadSubDirs(string dir)
        {
            Console.WriteLine(Path.GetFileName( dir));
            string[] subdirectoryFiles = Directory.GetFiles(dir);
            foreach (string file in subdirectoryFiles)
                if (Path.GetExtension(file) == ".osu")
                    ReadDotOsu(file);
        }

        static bool IsValidAR(string input, out float ar)
        {
            //checks for decimal value
            if (input.Contains("."))
            {
                string[] splitAR = input.Split('.');
                if (splitAR[1].Length > 1)
                {
                    ar = 0f;
                    return false;
                }
            }
            //checks for syntax
            var outAR = 0f;
            var result = float.TryParse(input, out outAR);
            if (!result)
            {
                ar = 0f;
                return result;
            }
            //checks for value
            if (outAR > 10f || outAR < 0f)
            {
                ar = 0f;
                return false;
            }
            ar = outAR;
            return true;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("1. Go to your osu folder with file manager");
            Console.WriteLine("2. Open your songs folder");
            Console.WriteLine("3. Copy the adress from the adress bar and enter it here");
            Console.Write("Please input a path > ");
            string @inputRoot = Console.ReadLine();
            bool result = false;
            while (!result)
            {
                Console.Write("\nPlease input the approachRate value > ");
                float AR = 0f;
                if (IsValidAR(Console.ReadLine(), out AR))
                {
                    globalAR = AR;
                    break;
                }
                Console.WriteLine("\nWrong Input");
            }
            Console.WriteLine("\nWorking...");
            GetSubDirectories(@inputRoot);
            Console.WriteLine(string.Format("\nProcessed {0} songs", processedSongs));
            Console.WriteLine(string.Format("\nSkipped {0} songs", skippedSongs));
            Console.ReadKey();

        }

    }
}

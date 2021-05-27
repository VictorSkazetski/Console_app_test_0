using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Files
    {
        public string[] FilesPath { get; private set; }
        public string DirectoryPath { get; private set; }

        public void GetFilesPath()
        {
            try
            {
                if (!Directory.Exists(DirectoryPath)) throw new Exception("директории не существует");

                FilesPath = Directory.GetFiles(DirectoryPath, "*.txt", SearchOption.AllDirectories);
                if (FilesPath?.Length == 0) throw new Exception("директория не содержит файлов с расширением .txt");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                Environment.Exit(0);
            }

        }

        public void ReadFiles()
        {
            var wordsInFile = new BlockingCollection<Tuple<string,List<string>>>();

            var stage1 = Task.Run(() =>
            {
                try
                {
                    foreach (var filePath in FilesPath)
                    { 

                        using (var reader = new StreamReader(filePath, true))
                        {
                            List<string> allLine = new List<string>();
                            string line;

                            while ((line = reader.ReadLine()) != null)
                            {
                                allLine.AddRange(line.Split(' '));
                            }

                            wordsInFile.Add(new Tuple<string, List<string>>(filePath, allLine));
                        }
                    }
                }
                finally
                {
                    wordsInFile.CompleteAdding();
                }
            });

            var stage2 = Task.Run(() =>
            {

                foreach (var wf in wordsInFile.GetConsumingEnumerable())
                {
                    Console.WriteLine($"Файл: {Path.GetFileName(wf.Item1)}");

                    var frequentWord = wf.Item2
                        .GroupBy(w => w)
                        .Select(group => new {Name = group.Key, Count = group.Count()})
                        .OrderByDescending(c => c.Count)
                        .Take(10)
                        .Select(w => w.Name)
                        .ToList();

                    if (frequentWord.Count != 0)
                    {
                        Console.WriteLine("слова которые встретились 10 и более раз:");

                        foreach (var word in frequentWord)
                        {
                            Console.WriteLine(word);
                        }
                    }
                    else
                    {
                        Console.WriteLine("файл не содержит нужное колчество повторяющих слов");
                    }

                    Console.WriteLine();
                }

                
            });

            
            Task.WaitAll(stage1, stage2);

        }
    

        public Files(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Zhenkai_Project
{
    public static class MyAssistant
    {
        #region Primitive && Non-Primitive Declaration/ - /Initialization
        static Dictionary<string, string> MIMETypesDictionary = new Dictionary<string, string>();
        private static int userOption;
        static string debugPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        static string imagePath = debugPath + @"\images";
        static string textPath = debugPath + @"\text";
        static List<string> regexMatchedFileName = new List<string>();
        static int iCheckerOne;
        static int iCheckerTwo;

        #endregion

        #region Start Method
        public static void Start(string[] args)
        {
            if (Directory.Exists(imagePath) && Directory.Exists(textPath))
            {
                IntroductionMenu();
                List<string> FileNamesList = new List<string>();
                DirectoryInfo imgDi = new DirectoryInfo(imagePath);
                DirectoryInfo textDi = new DirectoryInfo(textPath);
                try
                {
                    Console.WindowHeight = 50;
                    Console.WindowWidth = 200;
                    userOption = Convert.ToInt32(Console.ReadLine());
                    switch (userOption)
                    {
                        case 1:
                            FileNamesList.Clear();
                            PrintFiles(imgDi, FileNamesList);
                            AskDelete(imgDi, args, FileNamesList);
                            break;
                        case 2:
                            FileNamesList.Clear();
                            PrintFiles(textDi, FileNamesList);
                            AskDelete(textDi, args, FileNamesList);
                            break;
                        case 3:
                            FileNamesList.Clear();
                            PrintAllFiles(imgDi, textDi, FileNamesList);
                            AskDeleteDirs(imgDi, textDi, args, FileNamesList);
                            break;
                        case 4:
                            SortingFiles(imgDi, textDi, args);
                            break;
                        case 5:
                            regexMatchedFileName.Clear();
                            PrintAbnormalContent();
                            AskAbnormalDelete(imgDi, textDi,regexMatchedFileName, args);
                            break;
                        default:
                            ReturnHome("Not a Valid Option", args);
                            break;
                    }
                }
                catch (FormatException e)
                {
                    ReturnHome("Invalid Format!", args);
                }
                catch (IndexOutOfRangeException e)
                {
                    ReturnHome("Please have 2 argument",args);
                }
                catch (Exception e)
                {
                    ReturnHome("Error Found! " + e, args);
                }
            }
            else
            {
                Console.WriteLine("Directory(s) not found!");
            }

        }
        #endregion

        #region Helpful Functions

        #region IntroductionMenu Method
        public static void IntroductionMenu()
        {
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            Console.WriteLine("%%%%% Please select an option. %%%%%");
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            Console.WriteLine("1. View all files in Image Folder");
            Console.WriteLine("2. View all files in Text Folder");
            Console.WriteLine("3. View ALL file");
            Console.WriteLine("4. Perform sorting of files to image and text folders");
            Console.WriteLine("5. Perform Check on abnormal content in Text file");
        }
        #endregion

        #region ReturnHome Method
        static void ReturnHome(string msg, string[] args)
        {
            Console.WriteLine(msg);
            Console.ReadKey();
            Console.Clear();
            Program.Main(args);
        }
        #endregion

        #region foreachLoopOneDir Method
        static void foreachLoopOneDir(DirectoryInfo Di, string[] file, List<string> filenamelist, string[] args)
        {

            int firstfile = int.Parse(file[0]);
            int secondfile = int.Parse(file[1]);

            foreach (FileInfo allFi in Di.GetFiles().Where(allFi => (allFi.Attributes & FileAttributes.Hidden) == 0))
            {
                try
                {
                    try
                    {
                        if (allFi.Name == filenamelist[firstfile - 1])
                        {
                            Console.WriteLine(firstfile);
                            Console.WriteLine($"{allFi.FullName} deleted successfully");
                            allFi.Delete();
                        }
                    }
                    catch (Exception)
                    {
                        if (iCheckerOne == 0)
                        {
                            Console.WriteLine(firstfile + " is Not A Valid Choice");
                            iCheckerOne += 1;
                        }
                    }

                    try
                    {
                        if (allFi.Name == filenamelist[secondfile - 1])
                        {
                            Console.WriteLine(secondfile);
                            Console.WriteLine($"{allFi.FullName} deleted successfully");
                            allFi.Delete();
                        }
                    }
                    catch (Exception)
                    {
                        if (iCheckerTwo == 0)
                        {
                            Console.WriteLine(secondfile + " is Not A Valid Choice");
                            iCheckerTwo += 1;
                        }
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    ReturnHome("Option(s) not valid.", args);
                }
                catch (Exception e)
                {
                    ReturnHome(e.ToString(), args);
                }
            }
        }

        #endregion 

        #endregion

        #region Project Option 1 & 2

        #region PrintFiles Method
        public static void PrintFiles(DirectoryInfo Di, List<string> filenamelist)
        {
            int iCount = 0;
            Console.WriteLine("\nNo".PadRight(60) + "Name and Location of files".PadRight(95) + "||".PadRight(15) + "Type of file");
            Console.WriteLine("=".PadRight(59) + "==========================".PadRight(95) + "||".PadRight(11) + "======================");
            foreach (FileInfo allFi in Di.GetFiles().Where(allFi => (allFi.Attributes & FileAttributes.Hidden) == 0))
            {
                filenamelist.Add(allFi.Name);
                iCount++;
                Console.WriteLine(string.Format($@"{iCount,-10}" + $"{allFi.FullName,135}" + $"{Dictionary.GetMimeType(allFi.Extension),35}"));
            }
            Console.WriteLine("\nCompleted Scan!");
            Console.WriteLine("Total Files Count: " + iCount);
        }
        #endregion
        
        #region AskDelete Method
        public static void AskDelete(DirectoryInfo di, string[] args, List<string> filenamelist)
        {
            Console.WriteLine("Do you want to delete files? (Y/N)");
            string deleteFile2 = Console.ReadLine().ToUpper();
            if (deleteFile2 == "Y")
            {
                Console.WriteLine("Select file to delete. To delete file. type file No<space>file No. Example: 1 3");
                DeleteFiles(di,Console.ReadLine().Split(),filenamelist,args);
            }
            else if (deleteFile2 == "N")
            {
                ReturnHome("==============Returning Back To Menu===============", args);
            }
            else
            {
                ReturnHome("Invalid Option!", args);
            }
        }
        #endregion
        
        #region DeleteFiles Method
        public static void DeleteFiles(DirectoryInfo Di, string[] file, List<string> filenamelist, string[] args)
        {
            Console.WriteLine("Do you really want to delete? Y/N");
            string confirmAsk = Console.ReadLine().ToUpper();

            if (confirmAsk == "Y")
            {
                foreachLoopOneDir(Di, file, filenamelist, args);
                iCheckerOne = 0;
                iCheckerTwo = 0;
                ReturnHome(string.Empty, args);
            }
            else
            {
                ReturnHome("===========Returning back to Menu===========", args);
            }

        }
        #endregion

        #endregion

        #region Project Option 3

        #region PrintAllFiles Method
        public static void PrintAllFiles(DirectoryInfo Di1, DirectoryInfo Di2, List<string> filename)
        {
            int iCount = 0;
            Console.WriteLine("\nNo".PadRight(60) + "Name and Location of files".PadRight(95) + "||".PadRight(15) + "Type of file");
            Console.WriteLine("=".PadRight(59) + "==========================".PadRight(95) + "||".PadRight(11) + "======================");
            foreach (FileInfo allFi in Di1.GetFiles().Where(allFi => (allFi.Attributes & FileAttributes.Hidden) == 0))
            {
                filename.Add(allFi.Name);
                iCount++;
                Console.WriteLine(string.Format($@"{iCount,-10}" + $"{allFi.FullName,135}" + $"{Dictionary.GetMimeType(allFi.Extension),35}"));
            }
            foreach (FileInfo allFi in Di2.GetFiles().Where(allFi => (allFi.Attributes & FileAttributes.Hidden) == 0))
            {
                filename.Add(allFi.Name);
                iCount++;
                Console.WriteLine(string.Format($@"{iCount,-10}" + $"{allFi.FullName,135}" + $"{Dictionary.GetMimeType(allFi.Extension),35}"));
            }

            Console.WriteLine("\nCompleted Scan!");
            Console.WriteLine("Total Files Count: " + iCount);
        }
        #endregion

        #region AskDeleteDirs Method
        public static void AskDeleteDirs(DirectoryInfo di1, DirectoryInfo di2, string[] args, List<string> filenamelist)
        {
            Console.WriteLine("Do you want to delete files? (Y/N)");
            string deleteFile = Console.ReadLine().ToUpper();
            if (deleteFile == "Y")
            {
                Console.WriteLine("Select file to delete. To delete file. type file No<space>file No. Example: 1 3");
                DeleteFilesDirs(di1, di2,args,filenamelist,Console.ReadLine().Split());
            }
            else if (deleteFile == "N")
            {
                ReturnHome("===========Returning back to Menu===========", args);
            }
            else
            {
                ReturnHome("Invalid Option", args);
            }
        }
        #endregion
        
        #region DeleteFilesDirs Method
        static void DeleteFilesDirs(DirectoryInfo Di1, DirectoryInfo Di2, string[] args, List<string> filenamelist, string[] file)
        {
            Console.WriteLine("Do you really want to delete? Y/N");
            string confirmAsk = Console.ReadLine().ToUpper();
            if (confirmAsk == "Y")
            {
                foreachLoopOneDir(Di1,file,filenamelist,args);
                foreachLoopOneDir(Di2, file, filenamelist, args);
                iCheckerOne = 0;
                iCheckerTwo = 0;
                ReturnHome(string.Empty, args);
            }
            else
            {
                ReturnHome("===========Returning back to Menu============", args);
            }
        }
        #endregion

        #endregion

        #region Project Option 4

        #region SortingFiles Method
        static void SortingFiles(DirectoryInfo di1, DirectoryInfo di2, string[] args)
        {
            // ===================================== Method 1 ================================================= //

            //Process myProcess = new Process();
            //myProcess.StartInfo.RedirectStandardInput = true;
            //myProcess.StartInfo.RedirectStandardOutput = true;
            //myProcess.StartInfo.RedirectStandardError = true;
            //myProcess.StartInfo.UseShellExecute = false;
            //myProcess.StartInfo.CreateNoWindow = true;
            //myProcess.StartInfo.FileName = "cmd.exe";
            //bool processStarted = myProcess.Start();
            //if (processStarted == true)
            //{
            //    if (Directory.Exists(imagePath) && Directory.Exists(textPath))
            //    {
            //        myProcess.StandardInput.WriteLine($@"move /Y {imagePath}\*.txt {textPath}");

            //        myProcess.StandardInput.WriteLine($@"move /Y {textPath}\*.jpg {imagePath}");

            //        myProcess.StandardInput.WriteLine($@"move /Y {textPath}\*.png {imagePath}");

            //        myProcess.StandardInput.Close();

            //        Console.WriteLine("Files sorted!");

            //        myProcess.Close();
            //        ReturnHome(string.Empty, args);
            //    }
            //    else
            //    {
            //        Console.WriteLine("No Directory Found! Please do not have space in your directory");
            //    }
            //}

            // ===================================== Method 2 ================================================= //

            DirectoryInfo imageDi = new DirectoryInfo(imagePath);
            foreach (FileInfo textFile in imageDi.GetFiles("*.txt"))
            {
                var textdest = Path.Combine(textPath, Path.GetFileName(textFile.Name));
                textFile.MoveTo(textdest);
            }

            DirectoryInfo textDi = new DirectoryInfo(textPath);
            foreach (FileInfo imageFile in textDi.GetFiles("*.jpg"))
            {
                var dest = Path.Combine(imagePath, Path.GetFileName(imageFile.Name));
                imageFile.MoveTo(dest);
            }
            foreach (FileInfo imageFile in textDi.GetFiles("*.png"))
            {
                var dest = Path.Combine(imagePath, Path.GetFileName(imageFile.Name));
                imageFile.MoveTo(dest);
            }
            ReturnHome("Files sorted!", args);
        }
        #endregion

        #endregion

        #region Project Option 5

        #region PrintAbnormalContent Method
        static void PrintAbnormalContent()
        {
            int iCount = 0;

            Console.WriteLine("\nNo".PadRight(60) + "Name and Location of files".PadRight(68) + "||".PadRight(11) + "Abnormal Text detected");
            Console.WriteLine("=".PadRight(59) + "==========================".PadRight(68) + "||".PadRight(11) + "======================\n");
            using (StreamWriter writer = File.CreateText(debugPath + "\\abnormaltext.log"))
            {
                writer.WriteLine("    Name   :     Size       :      Abnormal Text");
                DirectoryInfo textDir = new DirectoryInfo(textPath);
                foreach (FileInfo fi in textDir.GetFiles("*.txt"))
                {
                    StreamReader reader = new StreamReader(fi.FullName);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        Regex validateBit = new Regex(@"(\s|^|\:)[1|3][a-km-zA-HJ-NP-Z0-9]{26,34}(\s|$|\.)");
                        if (validateBit.IsMatch(line))
                        {
                            iCount++;
                            var m = validateBit.Match(line);
                            Console.WriteLine($"{iCount,-10}" + $"{fi.FullName,112} " + $"{m.Groups[0].Value,45}");
                            writer.WriteLine($"{fi.Name} : {fi.Length} Bytes : {m.Groups[0].Value}");
                            regexMatchedFileName.Add(fi.Name);
                        }
                    }
                    reader.Close();
                }
                DirectoryInfo imageDir = new DirectoryInfo(imagePath);
                foreach (FileInfo fi in imageDir.GetFiles("*.txt"))
                {
                    StreamReader reader = new StreamReader(fi.FullName);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        Regex validateBit = new Regex(@"(\s|^|\:)[1|3][a-km-zA-HJ-NP-Z0-9]{26,34}(\s|$|\.)");
                        if (validateBit.IsMatch(line))
                        {
                            iCount++;
                            var m = validateBit.Match(line);
                            Console.WriteLine($"{iCount}".PadRight(10) + $"{fi.FullName}".PadLeft(110) + $"{m.Groups[0].Value}".PadLeft(46));
                            writer.WriteLine($"{fi.Name} : {fi.Length} Bytes : {m.Groups[0].Value}");
                            regexMatchedFileName.Add(fi.Name);
                        }
                    }
                    reader.Close();
                }
                Console.WriteLine("Completed Scan!");
                writer.Close();
            }
        }
        #endregion

        #region AskAbnormalDelete Method
        static void AskAbnormalDelete(DirectoryInfo imagePath,DirectoryInfo textPath, List<string> regexMatchedFileNames, string[] args)
        {
            Console.WriteLine("Do you want to delete file? Y/N");
            string option = Console.ReadLine().ToUpper();
            if (option == "Y")
            {
                Console.WriteLine("Select file to delete. To delete file. type file No<space>file No. Example: 1 3");
                DeleteAbnormalContent(imagePath,textPath,regexMatchedFileNames,Console.ReadLine().Split(),args);
            }
            else if (option == "N")
            {
                ReturnHome("==============Returning Back To Menu===============", args);
            }
            else
            {
                ReturnHome("Invalid Option", args);
            }
        }
        #endregion

        #region DeleteAbnormalContent Method
        static void DeleteAbnormalContent(DirectoryInfo Di1, DirectoryInfo Di2, List<string> filenamelist,string[] files, string[] args)
        {
            Console.WriteLine("Do you really want to delete? Y/N");
            string confirmAsk = Console.ReadLine().ToUpper();
            try
            {
                if (confirmAsk == "Y")
                {
                    foreachLoopOneDir(Di1, files, filenamelist, args);
                    foreachLoopOneDir(Di2, files, filenamelist, args);
                    iCheckerOne = 0;
                    iCheckerTwo = 0;
                    ReturnHome(string.Empty, args);

                }
                else
                {
                    ReturnHome("===============Returning back to Menu===============", args);
                }

            }
            catch (FormatException e)
            {
                ReturnHome("Invalid Format!", args);
            }
            catch (ArgumentOutOfRangeException e)
            {
                ReturnHome("Options are not valid.", args);
            }
            catch (Exception e)
            {
                ReturnHome(e.ToString(), args);
            }
        }
        #endregion

        #endregion
    }
}
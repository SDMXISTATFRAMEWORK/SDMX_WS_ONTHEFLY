using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

namespace FlySetup
{
    class Program
    {
        public const string dirFrw = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\";

        public const string NameRelDir = "Release";

        static void Main(string[] args)
        {
            Console.Title = "On The Fly Setup";
            Console.WriteLine("Wait Setup in work...");
            bool ris = MakeRelease();
            if (ris)
                Console.WriteLine("On The Fly Setup created successfully. Press any Key for Exit");
            Console.Read();


        }

        private static bool MakeRelease()
        {
            try
            {
                string domain = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.LastIndexOf("FlySetup"));
                DirectoryInfo Reldir = new DirectoryInfo(string.Format("{0}{1}", domain, NameRelDir));
                if (Reldir.Exists)
                    Reldir.Delete(true);

                DirectoryInfo dirXcopy = new DirectoryInfo(string.Format("{0}\\TestClient\\xcopy", Reldir));
                Directory.CreateDirectory(string.Format("{0}\\Utils", dirXcopy));
                

                Process p = new Process();
                p.StartInfo = new ProcessStartInfo();
                p.StartInfo.FileName = dirFrw + "MSBuild.exe";
                //BUILD WEB
                p.StartInfo.WorkingDirectory = domain + "OnTheFly";
                p.StartInfo.Arguments = string.Format("OnTheFly.csproj /p:DeployOnBuild=true /p:PublishProfile=\"{0}FlySetup\\OnTheFlySetup.pubxml\"", domain);
                p.Start();
                p.WaitForExit();
                //BUILD TEST
                p.StartInfo.WorkingDirectory = domain + "TestOnTheFlyService";
                p.StartInfo.Arguments = "TestOnTheFlyService.csproj /t:Clean;Rebuild /p:Configuration=Release ";///p:Platform=\"x86\"
                p.Start();
                p.WaitForExit();

                Console.WriteLine("Build Complete.... Moving Forlder and Files");
                Thread.Sleep(500);

                Reldir = new DirectoryInfo(string.Format("{0}{1}", domain, NameRelDir));
                dirXcopy = new DirectoryInfo(string.Format("{0}\\TestClient\\xcopy", Reldir));
                if (Reldir.Exists)
                {
                    if (Directory.Exists(string.Format("{0}\\bin\\sdmxv20", Reldir.FullName)))
                        Directory.Move(string.Format("{0}\\bin\\sdmxv20", Reldir.FullName), string.Format("{0}\\sdmxv20", Reldir.FullName));
                    if (Directory.Exists(string.Format("{0}\\bin\\sdmxv21", Reldir.FullName)))
                        Directory.Move(string.Format("{0}\\bin\\sdmxv21", Reldir.FullName), string.Format("{0}\\sdmxv21", Reldir.FullName));
                    if (Directory.Exists(string.Format("{0}\\bin\\Help", Reldir.FullName)))
                        Directory.Move(string.Format("{0}\\bin\\Help", Reldir.FullName), string.Format("{0}\\Help", Reldir.FullName));
                    if (Directory.Exists(string.Format("{0}\\bin\\doc", Reldir.FullName)))
                        Directory.Move(string.Format("{0}\\bin\\doc", Reldir.FullName), string.Format("{0}\\doc", Reldir.FullName));

                    
                    FileInfo[] Fpdb =new  DirectoryInfo(string.Format("{0}\\bin", Reldir.FullName)).GetFiles("*.pdb");
                    for (int i = Fpdb.Length-1; i >=0; i--)
                        Fpdb[i].Delete();


                    DirectoryInfo dirRelease = new DirectoryInfo(string.Format("{0}TestOnTheFlyService\\bin\\Release\\", domain));
                    File.Copy(string.Format("{0}TestOnTheFlyService.exe", dirRelease), string.Format("{0}\\TestOnTheFlyService.exe", dirXcopy));
                    File.Copy(string.Format("{0}TestOnTheFlyService.exe.config", dirRelease), string.Format("{0}\\TestOnTheFlyService.exe.config", dirXcopy));

                    File.Copy(string.Format("{0}Utils.dll", dirRelease), string.Format("{0}\\Utils.dll", dirXcopy));
                    File.Copy(string.Format("{0}FlyCallWS.dll", dirRelease), string.Format("{0}\\FlyCallWS.dll", dirXcopy));



                    if (Directory.Exists(string.Format("{0}\\bin\\QueriesStructure", Reldir.FullName)))
                        Directory.Move(string.Format("{0}\\bin\\QueriesStructure", Reldir.FullName), string.Format("{0}\\Utils\\QueriesStructure", dirXcopy.FullName));
                    if (Directory.Exists(string.Format("{0}\\bin\\QueriesStructure 21", Reldir.FullName)))
                        Directory.Move(string.Format("{0}\\bin\\QueriesStructure 21", Reldir.FullName), string.Format("{0}\\Utils\\QueriesStructure 21", dirXcopy.FullName));
                    if (Directory.Exists(string.Format("{0}\\bin\\QueryMessage", Reldir.FullName)))
                        Directory.Move(string.Format("{0}\\bin\\QueryMessage", Reldir.FullName), string.Format("{0}\\Utils\\QueryMessage", dirXcopy.FullName));
                    if (Directory.Exists(string.Format("{0}\\bin\\Queries", Reldir.FullName)))
                        Directory.Delete(string.Format("{0}\\bin\\Queries", Reldir.FullName), true);
                    if (Directory.Exists(string.Format("{0}\\bin\\Libs", Reldir.FullName)))
                        Directory.Delete(string.Format("{0}\\bin\\Libs", Reldir.FullName), true);

                    ZipFile.CreateFromDirectory(dirXcopy.ToString(), dirXcopy.ToString() + ".zip");
                    return true;

                }
                else
                    throw new Exception(Reldir.FullName + " Not Exist");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }
}

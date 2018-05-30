using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;

namespace PackageCleanTool
{
    class Program
    {

        static LinkedList<string> appList = new LinkedList<string>();

        static bool isShowAppList = true;


        static void Main(string[] args)
        {


            if (args.Length < 1)
            {
                Usage();
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                switch (arg)
                {

                    case "-?":
                    case "--help":
                        Usage();
                        return;
                    case "-v":
                    case "--version":
                        Version();
                        return;

                    case "-l":
                    case "--list":
                        ListApps();
                        return;

                    case "-c":
                    case "--clean":
                        isShowAppList = false;
                        Clean();
                        break;

                    default:
                        break;
                }
            }
        }


        static void Usage()
        {

            Console.WriteLine(Resource.Usage);

        }


        static void Version()
        {

            string exePath = Assembly.GetEntryAssembly().Location;
            Version ver = AssemblyName.GetAssemblyName(exePath).Version;
            Console.WriteLine("cleanTool v:" + ver);
        }

        static void ListApps()
        {
            string homePath = Environment.GetEnvironmentVariable("UserProfile");
            string configFilePath = homePath + "\\AppData\\Local\\JetBrains\\Toolbox\\.settings.json";

            if (!File.Exists(configFilePath))
            {
                Console.WriteLine("Please make sure JetBrains is installed on your machine!");
                return;
            }

            string text = File.ReadAllText(configFilePath);

            JavaScriptSerializer js = new JavaScriptSerializer();

            dynamic data = js.Deserialize(text, typeof(Setting));


            string installLocation = data.install_location;

            string targetPath = installLocation + "apps";


            string[] subdirectoryEntries = Directory.GetDirectories(targetPath);

            if (isShowAppList)
            {
                Console.WriteLine("Installed applications:");
            }
            foreach (var p in subdirectoryEntries)
            {
                appList.AddLast(p);
                if (isShowAppList)
                {
                    Console.WriteLine(p.Replace(targetPath, string.Empty).Replace("\\", string.Empty));
                }
            }

        }


        static void Clean()
        {
            ListApps();

            foreach (var app in appList)
            {
                string channel = "\\ch-0";
                string[] packages = Directory.GetDirectories(app + channel);
                string[] sortedPackages = packages.OrderBy(x => x).ToArray();

                for (int i = 0; i < sortedPackages.Length - 1; i++)
                {
                    Console.WriteLine("Delete " + sortedPackages[i]);

                    try
                    {
                        Directory.Delete(sortedPackages[i], true);
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e.Message);
                    }

                }

            }

        }
    }
}

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace PhotoMap
{
    class Program
    {
        static void Main(string[] args)
        {
            // если запуск приложения без аргументов (с ярлыка) то запись в реестр добавление контекстного меню
            if (args.Length == 0)
            {
                contextMap();
            }
            // если запуск с аргументами (из контекстного меню) то обработка информации в аргументах и открытие браузера
            else
            {
                checkArgs(args);
            testFileRec(args);
            }
        }

        //запись аргументов в файл на рабочем столе
        private static void testFileRec(string[] args)
        {
            string filePath = @"D:\rabstol\logger.txt";
            string s = String.Join(" ", args);  
            
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                    sw.Write(s);
                foreach (var item in args)
                {
                }
            }
        }

        // добавление в контекстное меню пункта - показать на карте
        static void contextMap()
        {
            const string appName = "PhotoMap";
            string pathApp = string.Format("\"{0}\"", System.Reflection.Assembly.GetExecutingAssembly().Location);
            string contextMenu = @"AllFilesystemObjects\shell\PhotoMap";
            pathApp = pathApp.Substring(1, pathApp.Length - 2);
            string command = @"AllFilesystemObjects\shell\PhotoMap\command";
            using (RegistryKey contextReg = Registry.ClassesRoot.CreateSubKey(contextMenu, true))
            {
                contextReg.SetValue("MUIVerb", "Посмотреть на карте");
                contextReg.SetValue("icon", pathApp);
                contextReg.SetValue
            }
            //using (RegistryKey comReg = Registry.ClassesRoot.CreateSubKey(command, true))
            //{
            //    comReg.SetValue("(По умолчанию)", "\"" + pathApp + "\" %1");
            //}
        }

        // обработка аргументов
        static void checkArgs(string[] coords)
        {

        }
    }
}

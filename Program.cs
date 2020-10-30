using ExifLib;
using Microsoft.Win32;
using System;
using System.IO;

namespace PhotoMap
{
    class Program
    {
        static private string filePath; // получаемый путь к файлу из контекстного меню
        static private string north, east; // северная широта и восточная долгота

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
                geoTags();
            }
        }

        // получение гео тегов из файла
        static void geoTags()
        {
            ExifReader meta = new ExifReader(filePath);

            double[] latitudeComponents;
            double[] longitudeComponents;

            string latitudeRef; // "N" or "S" ("S" will be negative latitude)
            string longitudeRef; // "E" or "W" ("W" will be a negative longitude)

            if (meta.GetTagValue(ExifTags.GPSLatitude, out latitudeComponents)
           && meta.GetTagValue(ExifTags.GPSLongitude, out longitudeComponents)
           && meta.GetTagValue(ExifTags.GPSLatitudeRef, out latitudeRef)
           && meta.GetTagValue(ExifTags.GPSLongitudeRef, out longitudeRef))
            {
                using (StreamWriter fileCoord = new StreamWriter(@"D:\rabstol\coords.txt"))
                {
                    for (int i = 0; i < latitudeComponents.Length; i++)
                    {
                        fileCoord.WriteLine(latitudeComponents[i]); 

                    }
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
            }
            using (RegistryKey comReg = Registry.ClassesRoot.CreateSubKey(command, true))
            {
                comReg.SetValue("", "\"" + pathApp + "\" %1");
            }
        }

        // получение адреса файла
        static void checkArgs(string[] file)
        {
            string logger = @"D:\rabstol\logger.txt";
            filePath = String.Join(" ", file);

            //запись аргументов в файл на рабочем столе - удалить-------------------
            using (StreamWriter sw = new StreamWriter(logger))
            {
                sw.Write(filePath);
            }
        }
    }
}

using ExifLib;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace PhotoMap
{
    class Program
    {
        static private string filePath; // получаемый путь к файлу из контекстного меню
        static private string north, east; // северная широта и восточная долгота, типа - 58°33'57.583
        static private string northDes, eastDes; // широта и долгота десятичные, типа - 58.56599
        static private string linkWithCoord; // ссылка с координатами

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
                filePath = String.Join(" ", args); // получение адреса файла
                geoTags();
                openBrowser();
            }
        }

        // запуск браузера
        private static void openBrowser()
        {
            //System.Diagnostics.Process.Start("http:\\google.com");
            if (linkWithCoord != null)
            {
                System.Diagnostics.Process.Start(linkWithCoord);
            }
        }

        // получение гео тегов из файла
        static void geoTags()
        {
            if (File.Exists(filePath))
            {
                using (ExifReader meta = new ExifReader(filePath))
                {
                    double[] latitudeComponents;
                    double[] longitudeComponents;

                    string latitudeRef; // "N" or "S" ("S" will be negative latitude)
                    string longitudeRef; // "E" or "W" ("W" will be a negative longitude)

                    if (meta.GetTagValue(ExifTags.GPSLatitude, out latitudeComponents)
                   && meta.GetTagValue(ExifTags.GPSLongitude, out longitudeComponents)
                   && meta.GetTagValue(ExifTags.GPSLatitudeRef, out latitudeRef)
                   && meta.GetTagValue(ExifTags.GPSLongitudeRef, out longitudeRef))
                    {
                        north = ConvertDegreeAngle(latitudeComponents[0], latitudeComponents[1], latitudeComponents[2], latitudeRef);
                        east = ConvertDegreeAngle(longitudeComponents[0], longitudeComponents[1], longitudeComponents[2], longitudeRef);
                        northDes = ConvertDegreeAngleDes(latitudeComponents[0], latitudeComponents[1], latitudeComponents[2], latitudeRef);
                        eastDes = ConvertDegreeAngleDes(longitudeComponents[0], longitudeComponents[1], longitudeComponents[2], longitudeRef);
                        north = commaToDot(north);
                        east = commaToDot(east);
                        northDes = commaToDot(northDes);
                        eastDes = commaToDot(eastDes);
                        linkWithCoord = "https://www.google.ru/maps/place/" + north + "+" + east + "/@" + northDes + "," + eastDes;
                        //using (StreamWriter sw = new StreamWriter(@"D:\rabstol\logger.txt"))
                        //{
                        //    sw.Write(filePath + "\n\n" + linkWithCoord);
                        //}
                    }
                }
            }
        }

        // метод изменяющий заятые в строке на точки
        static string commaToDot(string withComma)
        {
            string withDot = Regex.Replace(withComma, ",", ".");
            return withDot;
        }

        // конвертер координат в нужный формат
        static string ConvertDegreeAngle(double degrees, double minutes, double seconds, string latLongRef)
        {
            string sec = seconds.ToString();
            sec = Regex.Replace(sec, ", ", ".");
            string res = degrees + "°" + minutes + "'" + sec + "\"" + latLongRef;
            return res;
        }

        // конвертер координат в еще один формат
        public static string ConvertDegreeAngleDes(double degrees, double minutes, double seconds, string latLongRef)
        {
            double res = ConvertDegreeAngleToDouble(degrees, minutes, seconds);
            if (latLongRef == "S" || latLongRef == "W")
            {
                res *= -1;
            }
            string result = res.ToString();
            return result;
        }

        public static double ConvertDegreeAngleToDouble(double degrees, double minutes, double seconds)
        {
            return degrees + (minutes / 60) + (seconds / 3600);
        }


        // добавление в контекстное меню пункта - показать на карте
        static void contextMap()
        {
            string pathApp = string.Format("\"{0}\"", System.Reflection.Assembly.GetExecutingAssembly().Location);
            string contextMenu = @"SystemFileAssociations\.jpg\shell\PhotoMap";
            pathApp = pathApp.Substring(1, pathApp.Length - 2);
            string command = @"SystemFileAssociations\.jpg\shell\PhotoMap\command";
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
    }
}

using Microsoft.Win32;
using System.Diagnostics;

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
            }
        }

        // добавление в контекстное меню пункта - показать на карте
        static void contextMap()
        {
            const string appName = "PhotoMap";
            const string regKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(regKey, true))
            {
                registryKey.SetValue(appName, string.Format("\"{0}\"", System.Reflection.Assembly.GetExecutingAssembly().Location));
            }

        }

        // обработка аргументов
        static void checkArgs(string[] coords)
        {
            Debug.WriteLine(coords[0]);
        }
    }
}

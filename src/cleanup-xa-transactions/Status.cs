using System;

namespace cleanup_xa_transactions
{
    public static class Status
    {
        private static string _last = "";
        public static void Start(string label)
        {
            Clear();
            _last = label;
            Console.Write($"[ -- ] {label}");
        }

        public static void Ok()
        {
            Console.WriteLine($"\r[ OK ] {_last}");
        }

        public static void Fail()
        {
            Console.WriteLine($"\r[FAIL] {_last}");
        }

        public static void Clear()
        {
            if (_last == "")
            {
                return;
            }
            var wipe = new string(' ', _last.Length);
            Console.Write($"\r{wipe}\r");
        }
    }
}
using System.Net;

namespace DistributedSystem.Common.Validators
{
    public static class InputValidator
    {
        public static IPAddress ValidateIp(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()!;

                if (IPAddress.TryParse(input, out var ip))
                    return ip;

                Console.WriteLine("Invalid IP Address");
            }
        }

        public static int ValidatePort(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()!;

                if (int.TryParse(input, out var port))
                    return port;

                Console.WriteLine("Invalid Port");
            }
        }

        public static string ValidateTopic(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string topic = Console.ReadLine()!;

                if(!string.IsNullOrEmpty(topic))
                    return topic;

                Console.WriteLine("Empty field!");
            }
        }

    }
}

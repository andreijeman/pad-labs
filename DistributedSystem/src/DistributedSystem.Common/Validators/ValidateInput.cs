using System;
using System.Net;

namespace DistributedSystem.Common.Validators;

public static class InputValidator
{
    public static T ValidateInput<T>(string prompt, Func<string, (bool success, T value)> tryParse, string errorMessage)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()!;

            var (success, value) = tryParse(input);

            if (success)
                return value;

            Console.WriteLine(errorMessage);
        }
    }
}

using System;
using System.IO;

namespace geektrust
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var filePath = string.Empty;
            if (args.Length == 0) //if args are empty, asking user the filepath from console
            {
                filePath = Console.ReadLine();
                filePath = filePath.Trim().Replace("\"", ""); //trying to make it a valid string
            }
            else if (args.Length >= 1)
            {
                filePath = args[0]; //in this case double quotes are handled if taken from cmd_args
            }

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var words = line.Split(' ');
                if (PersonExtensions.ValidateInput(words))
                {
                    switch (words[0])
                    {
                        case "ADD_CHILD":
                            Console.WriteLine(PersonExtensions.AddChild(words[1], words[2], PersonExtensions.ParseGender(words[3])));
                            break;
                        case "GET_RELATIONSHIP":
                            Console.WriteLine(PersonExtensions.FindRelationShip(words[1], PersonExtensions.ParseRelation(words[2])));
                            break;
                    }
                } else
                {
                    Console.WriteLine("NONE"); //INVALID OPERATION
                }
            }
        }
    }
}

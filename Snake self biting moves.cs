using System;
using System.Collections.Generic;
using System.Linq;

namespace Solution
{
    class Solution
    {

        static void Main(string[] args)
        {
            var input = Console.ReadLine().Trim();
            var games = 0;
            Int32.TryParse(input, out games);

            List<string> gamesInput = new List<string>();

            while (games > 0)
            {
                var readLine = Console.ReadLine().Trim();
                gamesInput.Add(readLine);
                games--;
            }
            var previousStep = ' ';
            var exit = false;
            foreach (var item in gamesInput) { 
                var readLineAsArray = item.Split(new char[] { ' ' });

                var length = 0;
                Int32.TryParse(readLineAsArray[0], out length);
                var steps = readLineAsArray[1];

                var danger = 0;
                var index = 0;

                foreach (var step in steps)
                {
                    var currentStep = step;
                    if (currentStep == 'E') currentStep = previousStep;

                    if (currentStep == 'L' && currentStep == previousStep)
                    {
                        danger++;
                    }
                    else if (currentStep == 'R' && currentStep == previousStep)
                    {
                        danger++;
                    }
                    else danger = 0;


                    if (danger == 3)
                    {
                        Console.WriteLine(index);
                        exit = true;
                        break;
                    }
                    index++;
                    previousStep = currentStep;
                }
                if (exit) break;
                if(danger != 3) Console.WriteLine("YES");
            }
        }
    }
}

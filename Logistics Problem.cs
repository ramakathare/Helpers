using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerTrail
{
    class Program
    {
        public class ItemModel
        {
            public ItemModel(int _Id, int _Width, int _Weight)
            {
                if (_Id < 0) _Id = 0;
                if (_Width < 0) _Width = 0;
                if (_Weight < 0) _Weight = 0;
                this.Id = _Id;
                this.Weight = _Weight;
                this.Width = _Width;
            }
            public int Id { get; set; }
            public int Width { get; set; }
            public int Weight { get; set; }
        }
        static void Main(string[] args)
        {
            List<ItemModel> items = new List<ItemModel>();
           // Console.WriteLine("Please enter input for each item in <running id> <width> <weight> format. \nPress Esc key after provding input for all items.");

            ReadAllItems(items);

            //Sort items by heighest width, weight and lowest id
            items = items.OrderByDescending(p => p.Width).ThenByDescending(p=>p.Weight).ThenBy(p=>p.Id).ToList();

            List<ItemModel> A = GetCombination(items);
            
            items = FilterItems(items, A);

            List<ItemModel> B = GetCombination(items);

            items = FilterItems(items, B);

            List<ItemModel> C = GetCombination(items);

            Console.Write("A:");
            Console.WriteLine(OutPut(A));
            Console.Write("B:");
            Console.WriteLine(OutPut(B));
            Console.Write("C:");
            Console.WriteLine(OutPut(C));

            Console.ReadKey();

        }

        /// <summary>
        /// Read all lines parse them and add to items
        /// </summary>
        /// <param name="items"></param>
        private static void ReadAllItems(List<ItemModel> items)
        {
            //items.Add(new ItemModel(1,550, 450));
            //items.Add(new ItemModel(2,550, 350));
            //items.Add(new ItemModel(3,550, 250));

            //items.Add(new ItemModel(1, 450, 350));
            //items.Add(new ItemModel(2, 450, 550));
            //items.Add(new ItemModel(3, 800, 200));

            //items.Add(new ItemModel(1, 450, 350));
            //items.Add(new ItemModel(2, 450, 350));
            //items.Add(new ItemModel(3, 300, 200));

            //items.Add(new ItemModel(1, 450, 200));
            //items.Add(new ItemModel(2, 650, 200));
            //items.Add(new ItemModel(3, 900, 300));
            //items.Add(new ItemModel(4, 300, 300));

            //items.Add(new ItemModel(1, 350, 550));
            //items.Add(new ItemModel(2, 200, 300));
            //items.Add(new ItemModel(3, 150, 150));
            //items.Add(new ItemModel(4, 100, 100));
            //items.Add(new ItemModel(5, 200, 300));
            //items.Add(new ItemModel(6, 100, 200));
            //Console.WriteLine("Enter all item inputs in <running id> <width> <weight> format.");
            string input;
            while ((input = Console.ReadLine()) != null)
            {
                try
                {
                   // string input = Console.ReadLine().Trim();

                    if (String.IsNullOrWhiteSpace(input.Trim())) break;

                    var item = ParseInput(input);
                    //items.Add(item);
                    var idExists = items.Any(p => p.Id.Equals(item.Id));
                    if (!idExists) items.Add(item);
                    //else
                    //    Console.WriteLine("Item with same id already exists. Please enter again.");

                    //if (Console.ReadKey().Key == ConsoleKey.Enter) break;
                }
                catch
                {
                    Console.WriteLine("Invalid entry. Please enter again.");
                }
            }
        }


        /// <summary>
        /// Convert input to ItemModel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static ItemModel ParseInput(string input)
        {
            var input_array = input.Split(new char[] { ' ' });
            var item = new ItemModel(
                Int32.Parse(input_array[0]),
                Int32.Parse(input_array[1]),
                Int32.Parse(input_array[2]));
            return item;
        }


        /// <summary>
        /// return comma seprated ids
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static string OutPut(List<ItemModel> items)
        {
            return String.Join(",", items.Select(p => p.Id).ToArray());
        }

        /// <summary>
        /// Get the best combination according to the given requirements
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static List<ItemModel> GetCombination(List<ItemModel> items)
        {
            List<ItemModel> result = new List<ItemModel>();
            GetCombination(items, result, 1100, 1000, items.Count());
            return result;
        }
        /// <summary>
        /// Get combinations
        /// </summary>
        /// <param name="items"></param>
        /// <param name="output"></param>
        /// <param name="currentItemWidth"></param>
        /// <param name="currentItemWeight"></param>
        /// <param name="length"></param>
        private static Tuple<int, int, int> GetCombination(List<ItemModel> items, List<ItemModel> output, int currentItemWidth, int currentItemWeight, int length)
        {
            if (length <= 0 || currentItemWidth <= 0 || currentItemWeight <= 0)
            {
                return new Tuple<int, int, int>(0, 0, 0);
            }
            ItemModel currItem = items[length - 1];
            if (currItem.Width > currentItemWidth || currItem.Weight > currentItemWeight)
            {
                // cannot be included
                List<ItemModel> subChoice = new List<ItemModel>();
                Tuple<int, int, int> optimal = GetCombination(items, subChoice, currentItemWidth, currentItemWeight, length - 1);
                output.AddRange(subChoice);
                return optimal;
            }
            else
            {
                List<ItemModel> includedChoice = new List<ItemModel>();
                List<ItemModel> excludedChoice = new List<ItemModel>();
                Tuple<int, int, int> add = GetCombination(items, includedChoice, currentItemWidth - currItem.Width, currentItemWeight - currItem.Weight, length - 1);

                add = new Tuple<int, int, int>(add.Item1 + currItem.Width, add.Item2 + currItem.Weight, add.Item3 + 1);

                Tuple<int, int, int> minus = GetCombination(items, excludedChoice, currentItemWidth, currentItemWeight, length - 1);
                if (Compare(add, minus) >= 0)
                {
                    output.AddRange(includedChoice);
                    output.Add(currItem);
                    return add;
                }
                else
                {
                    output.AddRange(excludedChoice);
                    return minus;
                }
            }
        }

        /// <summary>
        /// Compare the two tuples based on the recommendation Width, Weight and then Id
        /// </summary>
        /// <param name="include"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public static int Compare(Tuple<int, int, int> include, Tuple<int, int, int> exclude)
        {
            if (include.Item1 != exclude.Item1)
                return include.Item1 - exclude.Item1;
            else if (include.Item2 != exclude.Item2)
            {
                return include.Item2 - exclude.Item2;
            }
            else
            {
                return include.Item3 - exclude.Item3;
            }
        }

        /// <summary>
        /// Remove items that are in items and not in excludeditems
        /// </summary>
        /// <param name="items"></param>
        /// <param name="excluded"></param>
        /// <returns></returns>
        private static List<ItemModel> FilterItems(List<ItemModel> items, List<ItemModel> excluded)
        {
            return items = items.Where(p => !excluded.Any(q => q.Id == p.Id)).ToList();
        }

       

        

    }
}

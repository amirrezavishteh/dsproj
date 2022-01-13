using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DS
{
    /// <summary>Any drug is similar to a node of graph</summary>
    public class Drug
    {
        public double price = 0;
        public string name;
        public List<Drug> interactions_drugs;
        public Drug(string name, double price)
        {
            this.name = name;
            this.price = price;
            this.interactions_drugs = new List<Drug>();
        }
    }
    public class Pharmacy
    {
        Random Rnd = new Random();
        public Pharmacy()
        {}
        public List<Drug> Drugs;


        /// <summary> create a random sign, + or - , for dataset_4</summary> <returns>return a char, (+) or (-) </returns>
        public char createRnd_pos_neg_Effect() => (Rnd.Next() % 2 == 0) ? '+' : '-' ;


        ~Pharmacy()
        {}
    }
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                System.Console.WriteLine("1. start ");
                System.Console.WriteLine("2. Check for drug interactions in a prescription ");
                System.Console.WriteLine("3. Check for drug allergies in a prescription ");
                System.Console.WriteLine("4. Calculate the invoice price of the prescription ");
                System.Console.WriteLine("5. Changing the price of the drugs ");
                System.Console.WriteLine("6. Add or remove from dataset ");
                System.Console.WriteLine("7. Search ");
                string option;
                int option_number;
                while(true)
                {
                    try
                    {
                        System.Console.WriteLine("Choose Option(1-7)");
                        option = System.Console.ReadLine();
                        option_number = int.Parse(option);
                        if(option_number != 1)
                        {
                            throw new Exception("Invalid Input");
                        }
                        break;
                    }
                    catch (System.FormatException)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine("Your input isn't in correct format, please insert number");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    catch (Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        System.Console.WriteLine("For accessing to drugs data insert 1 first of all");
                        Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine("Your input wasn't in correct range, please insert any number in range 1-7");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                switch (option_number)
                {
                    case 1:
                    break;

                    case 2:
                    break;

                    case 3:
                    break;

                    case 4:
                    break;

                    case 5:
                    break;

                    case 6:
                    break;

                    case 7:
                    break;
                }
            }
        }
    }
}

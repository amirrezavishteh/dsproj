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
        public Drug(string name, double price)
        {
            this.name = name;
            this.price = price;
        }
        public override bool Equals(object obj)
        {
            var other = obj as Drug;
            return other.name == this.name;
        }

        public override int GetHashCode()
        {
            int result = 0;
            foreach (char c in this.name)
            {
                result += (10*result + (int)(c));
            }
            return result;
        }
    }

    /// <summary>Not Completed</summary>
    public class Disease
    {
        public List<Drug> Positive_Effect;
        public List<Drug> Negative_Effect;
        public string name;
        public Disease(string name)
        {
            this.name = name;
            this.Positive_Effect = new List<Drug>();
            this.Negative_Effect = new List<Drug>();
        }
    }
    public class Pharmacy
    {
        Random Rnd = new Random();
        public List<Drug> Drugs;
        public List<List<string>> Drugs_Graph;
        public Dictionary<Drug, int> Drugs_ids;
        int last_id = 0;
        public List<Drug> New_Drugs;
        public Pharmacy()
        {
            this.Drugs_Graph = new List<List<string>>();
            this.Drugs_ids = new Dictionary<Drug, int>();
            this.Drugs = new List<Drug>();
            using(StreamReader sr = new StreamReader("./dataset_1.txt"))
            {
                string line = sr.ReadLine();
                while(line != null)
                {
                    var a = line.Split(':').Select(x => x.Trim()).ToArray();
                    Drug d = new Drug(a[0], double.Parse(a[1]));
                    Drugs.Add(d);
                    this.Drugs_ids.Add(d, this.last_id);
                    this.last_id ++;
                    line = sr.ReadLine();
                }
            }
            var column = Enumerable.Range(0, this.Drugs.Count).Select(x => String.Empty).ToList();
            for(int i = 0; i < this.Drugs.Count; i++)
            {
                this.Drugs_Graph.Add(column);
            }
        }

        public void Make_Interactions()
        {
            using(StreamReader sr = new StreamReader("./dataset_3.txt"))
            {
                var line = sr.ReadLine().Split(new char[]{':', '(', ',', ')', ' '}, StringSplitOptions.RemoveEmptyEntries).ToArray();
                int i = Drugs_ids[new Drug(line[0], 0)];
                int j = Drugs_ids[new Drug(line[1], 0)];
                Drugs_Graph[i][j] = line[2];
            }
        }

        public void Create_Drug(string name, double price) 
        {}

        public void Delete_Drug(Drug d)
        {}

        public Drug Read_Drug(string name)
        {
            throw new NotImplementedException();
        }

        public void Create_Disease(string name)
        {}

        public void Delete_Disease(Disease d)
        {}

        public Disease Read_Disease(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary> create a random sign, + or - , for dataset_4</summary> <returns>return a char, (+) or (-) </returns>
        public char createRnd_pos_neg_Effect() => (Rnd.Next() % 2 == 0) ? '+' : '-' ;


        ~Pharmacy()
        {}
    }
    class Program
    {
        static void Main(string[] args)
        {
            Pharmacy p = new Pharmacy();
            p.Make_Interactions();
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

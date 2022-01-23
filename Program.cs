using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace DS
{
    public class Drug
    {
        public double price = 0;
        public string name;
        public Drug(string name, double price)
        {
            this.name = name;
            this.price = price;
        }
    }
    public class Pharmacy : IDisposable
    {
        Stopwatch sw = new Stopwatch();
        string characters = "abcdefghijklmnopqrstuvwxyz";
        public Random Rnd = new Random();
        public static long Encode_HashCode(string value)
        {
            long result = 0;
            for(int i = 0; i < value.Length; i++)
            {
                result += (result * 10 + (int)(value[i]) - 68);
            }
            return result;
        }
        public long Get_Drug_Index(string drug)
        {
            long hashCode = Encode_HashCode(drug) % this.Drugs_Capacity;
            int i = 0;
            while(this.Drugs_With_Their_Interactions_Drug[hashCode].Key==null||this.Drugs_With_Their_Interactions_Drug[hashCode].Key.name != drug)
            {
                if(i > this.Drugs_Capacity)
                    return -1;
                hashCode = (hashCode + 1) % this.Drugs_Capacity;
                i++;
            }
            return hashCode;
        }
        public long Get_Disease_Index(string disease)
        {
            long hashCode = Encode_HashCode(disease) % this.Disease_Capacity;
            int i = 0;
            while(this.Disease_With_Their_Effective_Drugs[hashCode].Key != disease)
            {
                if(i > this.Disease_Capacity)
                    return -1;
                hashCode = (hashCode + 1) % this.Disease_Capacity;
                i++;
            }
            return hashCode;
        }
        public void Put_Drug_Into_Table(Drug drug, bool first_mode = true)
        {
            if(!first_mode)
                for(int i = 0; i<this.Drugs_With_Their_Interactions_Drug.Length; i++)
                    if(this.Drugs_With_Their_Interactions_Drug[i].Key!=null && this.Drugs_With_Their_Interactions_Drug[i].Key.name == drug.name)
                        throw new Exception();
            long hashCode = Encode_HashCode(drug.name) % this.Drugs_Capacity;
            while(this.Drugs_With_Their_Interactions_Drug[hashCode].Key != null)
            {
                hashCode = (hashCode + 1) % this.Drugs_Capacity;
            }
            var kv = new KeyValuePair<Drug, Dictionary<string, string>>(drug, null);
            this.Drugs_With_Their_Interactions_Drug[hashCode] = kv;
        }
        public void Put_Disease_Into_Table(string disease, bool first_mode = true)
        {
            if(!first_mode)
                for(int i = 0; i<this.Disease_With_Their_Effective_Drugs.Length; i++)
                    if(this.Disease_With_Their_Effective_Drugs[i].Key == disease)
                        throw new Exception();
            long hashCode = Encode_HashCode(disease) % this.Disease_Capacity;
            while(this.Disease_With_Their_Effective_Drugs[hashCode].Key != null)
            {
                hashCode = (hashCode + 1) % this.Disease_Capacity;
            }
            var kv = new KeyValuePair<string, Dictionary<string, char>>(disease, null);
            this.Disease_With_Their_Effective_Drugs[hashCode] = kv;
        }
        public char createRnd_pos_neg_Effect() => (Rnd.Next() % 2 == 0) ? '+' : '-' ;
        int Drugs_number = 0;
        int Drugs_Capacity = 100_000;
        int Disease_number = 0;
        int Disease_Capacity = 100_000;
        public KeyValuePair<Drug, Dictionary<string, string>>[] Drugs_With_Their_Interactions_Drug;
        public KeyValuePair<string, Dictionary<string, char>>[] Disease_With_Their_Effective_Drugs;
        public Pharmacy()
        {
            this.Drugs_With_Their_Interactions_Drug = new KeyValuePair<Drug, Dictionary<string, string>>[this.Drugs_Capacity];
            this.Disease_With_Their_Effective_Drugs = new KeyValuePair<string, Dictionary<string, char>>[this.Disease_Capacity];

            #region Read Drug Data from dataset_1
                using (StreamReader sr = new StreamReader("./dataset_1.txt"))
                {
                    string line = sr.ReadLine();
                    while(line != null)
                    {
                        var a = line.Split(':').Select(x => x.Trim()).ToArray();
                        this.Create_Drug(a[0], double.Parse(a[1]));
                        line = sr.ReadLine();
                    }
                }
            #endregion

            #region Read Disease Data from dataset_2
                using (StreamReader sr = new StreamReader("./dataset_2.txt"))
                {
                    string line = sr.ReadLine();
                    while(line != null)
                    {
                        Create_Disease(line);
                        line = sr.ReadLine();
                    }
                }
            #endregion
        }

        public void Create_Drug(string name, double price, bool first_mode = true)
        {
            if(this.Drugs_number >= this.Drugs_Capacity)
            {
                var temp = this.Drugs_With_Their_Interactions_Drug.ToList();
                for(int _ = 0; _ < 10; _++)
                    temp.Add(new KeyValuePair<Drug, Dictionary<string,string>>(null, null));
                this.Drugs_With_Their_Interactions_Drug = temp.ToArray();
                temp = null;
                this.Drugs_Capacity += 10;
            }
            Drug d = new Drug(name, price);
            try
            {
                this.Put_Drug_Into_Table(d, first_mode);
                this.Drugs_number ++;
                if(!first_mode)
                {
                    int random_count = this.Rnd.Next(1, 50);
                    var interactions_drugs = new string[random_count];
                    int idx = 0;
                    List<int> indexes = new List<int>();
                    for(int _ = 0; _ < interactions_drugs.Length; _++)
                    {
                        int i = this.Rnd.Next(this.Drugs_number);
                        while(indexes.Contains(i)||this.Drugs_With_Their_Interactions_Drug[i].Key.name == name)
                            i = this.Rnd.Next(this.Drugs_number);
                        interactions_drugs[idx] = this.Drugs_With_Their_Interactions_Drug[i].Key.name;
                        idx++;
                        indexes.Add(i);
                    }
                    indexes.Clear();
                    idx = 0;
                    random_count = this.Rnd.Next(1, 30);
                    var Effective_diseases = new string[random_count];
                    for(int _ = 0; _ < Effective_diseases.Length; _++)
                    {
                        int i = this.Rnd.Next(this.Disease_number);
                        while(indexes.Contains(i))
                            i = this.Rnd.Next(this.Disease_number);
                        Effective_diseases[idx] = this.Disease_With_Their_Effective_Drugs[i].Key;
                        idx++;
                        indexes.Add(i);
                    }

                    string effect = "Eff_";
                    for(int i = 0; i < interactions_drugs.Length; i++)
                    {
                        for(int _ = 0; _ < 10; _++)
                            effect += this.characters[this.Rnd.Next(26)];
                        this.Make_Interaction(name, interactions_drugs[i], effect);
                        this.Make_Interaction(interactions_drugs[i], name, effect);
                        effect = "Eff_";
                    }

                    for(int i = 0; i<Effective_diseases.Length; i++)
                    {
                        this.Make_Allergie(Effective_diseases[i], name, this.createRnd_pos_neg_Effect());
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.WriteLine($"The {name} added to dataset successfully.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"There was a drug with name {name} in dataset.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        public void Create_Disease(string disease, bool first_mode = true)
        {
            if(this.Disease_number >= this.Disease_Capacity)
            {
                var temp = this.Disease_With_Their_Effective_Drugs.ToList();
                for(int _ = 0; _ < 10; _++)
                    temp.Add(new KeyValuePair<string, Dictionary<string, char>>(null, null));
                this.Disease_With_Their_Effective_Drugs = temp.ToArray();
                temp = null;
                this.Disease_Capacity += 10;
            }
            try
            {
                this.Put_Disease_Into_Table(disease, first_mode);
                this.Disease_number ++;
                if(!first_mode)
                {
                    int random_count = this.Rnd.Next(1, 15);
                    var Effective_drugs = new string[random_count];
                    int idx = 0;
                    List<int> indexes = new List<int>();
                    for(int _ = 0; _ < Effective_drugs.Length; _++)
                    {
                        int i = this.Rnd.Next(this.Drugs_number);
                        while(indexes.Contains(i))
                            i = this.Rnd.Next(this.Drugs_number);
                        Effective_drugs[idx] = this.Drugs_With_Their_Interactions_Drug[i].Key.name;
                        idx++;
                        indexes.Add(i);
                    }

                    for(int i = 0; i<Effective_drugs.Length; i++)
                    {
                        this.Make_Allergie(disease, Effective_drugs[i], this.createRnd_pos_neg_Effect());
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.WriteLine($"The disease {disease} added to dataset successfully.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"There was a disease with name {disease} in dataset.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void Delete_Drug(string drug)
        {
            try
            {
                long hashCode = this.Get_Drug_Index(drug);
                var interactions_drugs = this.Drugs_With_Their_Interactions_Drug[hashCode].Value?.ToArray();
                this.Drugs_With_Their_Interactions_Drug[hashCode] = new KeyValuePair<Drug, Dictionary<string, string>>(null, null);
                this.Drugs_number--;
                for(int i = 0; i<interactions_drugs?.Length; i++)
                {
                    hashCode = Get_Drug_Index(interactions_drugs[i].Key);
                    this.Drugs_With_Their_Interactions_Drug[hashCode].Value.Remove(drug);
                }
                for(int i = 0; i<this.Disease_With_Their_Effective_Drugs.Length; i++)
                {
                    if(this.Disease_With_Their_Effective_Drugs[i].Value!=null && this.Disease_With_Their_Effective_Drugs[i].Value.ContainsKey(drug))
                    {
                        this.Disease_With_Their_Effective_Drugs[i].Value.Remove(drug);
                    }
                }
                Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine($"The drug with name {drug} removed from dataset successfully.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"There is not any drug with name {drug} in dataset.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        public void Delete_Disease(string disease)
        {
            long hashCode = this.Get_Disease_Index(disease);
            try
            {
                this.Disease_With_Their_Effective_Drugs[hashCode] = new KeyValuePair<string, Dictionary<string, char>>(null, null);
                this.Disease_number--;
                Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine($"The disease with name {disease} removed from dataset successfully.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"There is not any disease with name {disease} in dataset.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public KeyValuePair<Drug, Dictionary<string, string>> Read_Drug(string name)
        {
            long hashCode = this.Get_Drug_Index(name);
            try
            {
                return this.Drugs_With_Their_Interactions_Drug[hashCode];
            }
            catch
            {
                return new KeyValuePair<Drug, Dictionary<string, string>>(null, null);
            }
        }
        public KeyValuePair<string, Dictionary<string, char>> Read_Disease(string name)
        {
            long hashCode = Get_Disease_Index(name);
            try
            {
                return this.Disease_With_Their_Effective_Drugs[hashCode];
            }
            catch
            {
                return new KeyValuePair<string, Dictionary<string, char>>(null, null);
            }
        }

        public void Search_Drug(string name)
        {
            var interactions_drugs = this.Read_Drug(name);
            try
            {
                if (interactions_drugs.Key == null) throw new Exception();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                System.Console.WriteLine(" \t Drug \t " + "|" + " \tEffect\t  ");
                System.Console.WriteLine("----------------------------------");
                for(int i = 0; i< interactions_drugs.Value?.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write($" {interactions_drugs.Value.ElementAt(i).Key} ");
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("|");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write($" {interactions_drugs.Value.ElementAt(i).Value} \n");
                    Console.ForegroundColor = ConsoleColor.Black;
                    System.Console.WriteLine("----------------------------------");
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                System.Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++");

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                System.Console.WriteLine("    Disease\t" + "|" + "Allergy");
                System.Console.WriteLine("------------------------");
                for(int i = 0; i<this.Disease_With_Their_Effective_Drugs?.Length; i++)
                {
                    if(this.Disease_With_Their_Effective_Drugs[i].Value!=null && this.Disease_With_Their_Effective_Drugs[i].Value.ContainsKey(name))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write($" {this.Disease_With_Their_Effective_Drugs[i].Key} ");
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write("|");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write($"  {this.Disease_With_Their_Effective_Drugs[i].Value[name]}    \n");
                        Console.ForegroundColor = ConsoleColor.Black;
                        System.Console.WriteLine("------------------------");
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"There is not any drug with name {name} in dataset.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        public void Search_Disease(string name)
        {
            var allergies_drugs = this.Read_Disease(name);
            try
            {
                if(allergies_drugs.Key == null) throw new Exception();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                System.Console.WriteLine(" \tDrug \t " + "|" + " Allergy  ");
                System.Console.WriteLine("----------------------------");
                for(int i = 0; i< allergies_drugs.Value?.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write($" {allergies_drugs.Value.ElementAt(i).Key} ");
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("|");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write($"    {allergies_drugs.Value.ElementAt(i).Value}     \n");
                    Console.ForegroundColor = ConsoleColor.Black;
                    System.Console.WriteLine("----------------------------");
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"There is not any disease with name {name} in dataset.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void Update_Drug_Price(string[] drugs, double inflation)
        {
            for(int i = 0; i<drugs.Length; i++)
            {
                long hashCode = this.Get_Drug_Index(drugs[i]);
                this.Drugs_With_Their_Interactions_Drug[hashCode].Key.price += (this.Drugs_With_Their_Interactions_Drug[hashCode].Key.price * inflation / 100);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("The price of drugs updated successfully.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Make_All_Interactions_Drug()
        {
            string[] line;
            using(StreamReader sr = new StreamReader("./dataset_3.txt"))
            {
                string l = sr.ReadLine();
                while(l != null)
                {
                    line = l.Split(':', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                    var effected_drugs = line[1].Split(new char[]{';', ' '}, StringSplitOptions.RemoveEmptyEntries)
                                .Select(item => {
                                    var a = item.Split(new char[]{'(', ')', ','}, StringSplitOptions.RemoveEmptyEntries);
                                    return (drug:a[0], effect : a[1]);
                                }).ToArray();

                    for(int i = 0; i < effected_drugs.Length; i++)
                    {
                        this.Make_Interaction(line[0], effected_drugs[i].drug, effected_drugs[i].effect);
                    }
                    l = sr.ReadLine();
                }
            }
        }
        public void Make_All_Allergies()
        {
            string[] line;
            (string drug, char effect)[] drugs_with_effect;
            using(StreamReader sr = new StreamReader("./dataset_4.txt"))
            {
                string l = sr.ReadLine();
                while(l != null)
                {
                    line = l.Split(':').Select(item => item.Trim()).ToArray();
                    drugs_with_effect = line[1].Split(';').Select(item => { 
                                    var a = item.Split(new char[]{'(', ')', ',', ' '}, StringSplitOptions.RemoveEmptyEntries);
                                    return (drug : a[0], effect : a[1][0]);
                                }).ToArray();

                    for(int i = 0; i <drugs_with_effect.Length; i++)
                    {
                        Make_Allergie(line[0], drugs_with_effect[i].drug, drugs_with_effect[i].effect);
                    }

                    l = sr.ReadLine();
                }
            }
        }
        public void Make_Interaction(string d1, string d2, string effect)
        {
            long hashCode = this.Get_Drug_Index(d1);
            if(this.Drugs_With_Their_Interactions_Drug[hashCode].Value == null)
            {
                var t = new Dictionary<string, string>();
                this.Drugs_With_Their_Interactions_Drug[hashCode] = new KeyValuePair<Drug, Dictionary<string,string>>(this.Drugs_With_Their_Interactions_Drug[hashCode].Key, t);
            }
            this.Drugs_With_Their_Interactions_Drug[hashCode].Value.Add(d2, effect);
        }
        public void Make_Allergie(string disease, string drug, char allergy)
        {
            long hashCode = this.Get_Disease_Index(disease);
            if(this.Disease_With_Their_Effective_Drugs[hashCode].Value == null)
            {
                var t = new Dictionary<string, char>();
                this.Disease_With_Their_Effective_Drugs[hashCode] = new KeyValuePair<string, Dictionary<string, char>>(this.Disease_With_Their_Effective_Drugs[hashCode].Key, t);
            }
            this.Disease_With_Their_Effective_Drugs[hashCode].Value.Add(drug, allergy);
        }

        public void Check_Interaction(string[] drugs)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            System.Console.WriteLine("    1_Drug\t " + "|" + " \t2_Drug\t  " + " |  " + " \tEffect\t    ");
            System.Console.WriteLine("----------------------------------------------------");
            for(int i=0; i<drugs.Length; i++)
            {
                long hashCode = Get_Drug_Index(drugs[i]);
                var interactions_drugs = this.Drugs_With_Their_Interactions_Drug[hashCode].Value;
                string d = this.Drugs_With_Their_Interactions_Drug[hashCode].Key.name;
                for(int j=i+1; j<drugs.Length && interactions_drugs!=null; j++)
                {
                    hashCode = Get_Drug_Index(drugs[j]);
                    var interact_drug = this.Drugs_With_Their_Interactions_Drug[hashCode].Key;
                    if(interactions_drugs.ContainsKey(interact_drug.name))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write($" {d} ");
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write("| ");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(interact_drug.name);
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(" | ");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(interactions_drugs[interact_drug.name] + " \n");
                        Console.ForegroundColor = ConsoleColor.Black;
                        System.Console.WriteLine("----------------------------------------------------");
                    }
                }
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Check_Allergies(string[] drugs, string[] diseases)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            System.Console.WriteLine("    Disease     " + "|" + " \tDrug\t " + " |" + "   Allergy   ");
            System.Console.WriteLine("------------------------------------------------");
            foreach (string disease in diseases)
            {
                long hashCode = this.Get_Disease_Index(disease);
                var allergies = this.Disease_With_Their_Effective_Drugs[hashCode].Value;
                for(int i = 0; i <drugs.Length && allergies != null; i++)
                {
                    if(allergies.ContainsKey(drugs[i]))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write($" {disease} ");
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write("| ");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(drugs[i]);
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(" | ");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("    " + allergies[drugs[i]] + " \t\n");
                        Console.ForegroundColor = ConsoleColor.Black;
                        System.Console.WriteLine("------------------------------------------------");
                    }
                }
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public double Get_Invoice_Price(string[] drugs)
        {
            double price = 0;
            foreach(var drug in drugs)
            {
                try
                {
                    long hashCode = Get_Drug_Index(drug);
                    price += this.Drugs_With_Their_Interactions_Drug[hashCode].Key.price;
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine($"There is not any drug with name {drug} in dataset.");
                }
            }
            return price;
        }

        public void Dispose()
        {
            #region Write Drugs in dataset_1
            using (StreamWriter sw = new StreamWriter("./dataset_1.txt", false))
            {
                foreach (var drug in this.Drugs_With_Their_Interactions_Drug)
                {
                    if(drug.Key != null)
                        sw.WriteLine($"{drug.Key?.name} : {drug.Key?.price}");
                }
            }
            #endregion

            #region Write Disease in dataset_2
            using(StreamWriter sw = new StreamWriter("./dataset_2.txt", false))
            {
                foreach(var disease in this.Disease_With_Their_Effective_Drugs)
                {
                    if(disease.Key != null)
                        sw.WriteLine(disease.Key);
                }
            }
            #endregion

            #region Write Interactions_Drugs in dataset_3
            using(StreamWriter sw = new StreamWriter("./dataset_3.txt", false))
            {
                foreach(var drug in this.Drugs_With_Their_Interactions_Drug)
                {
                    string result = drug.Key?.name + " :";
                    for(int i = 0; i<drug.Value?.Count; i++)
                    {
                        if(i == drug.Value.Count - 1)
                            result += $" ({drug.Value.ElementAt(i).Key},{drug.Value.ElementAt(i).Value})";
                        else
                            result += $" ({drug.Value.ElementAt(i).Key},{drug.Value.ElementAt(i).Value}) ;";
                    }
                    if(drug.Value != null &&drug.Value.Count > 0)
                    {
                        sw.WriteLine(result);
                    }
                }
            }
            #endregion

            #region Write allergies in dataset_4
            using(StreamWriter sw = new StreamWriter("./dataset_4.txt", false))
            {
                foreach(var disease in this.Disease_With_Their_Effective_Drugs)
                {
                    if(disease.Value != null && disease.Value.Count > 0)
                    {
                        string result = disease.Key + " :";
                        for(int i = 0; i<disease.Value.Count; i++)
                        {
                            if(i == disease.Value.Count - 1)
                                result += $" ({disease.Value.ElementAt(i).Key},{disease.Value.ElementAt(i).Value})";
                            else
                                result += $" ({disease.Value.ElementAt(i).Key},{disease.Value.ElementAt(i).Value}) ;";
                        }
                        sw.WriteLine(result);
                    }
                }
            }
            #endregion

            this.Disease_With_Their_Effective_Drugs = null;
            this.Drugs_With_Their_Interactions_Drug = null;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            bool Continue = true;
            Pharmacy p = new Pharmacy();
            p.Make_All_Interactions_Drug();
            p.Make_All_Allergies();
            Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.WriteLine("1. start ");
            System.Console.WriteLine("2. Check for drug interactions in a prescription ");
            System.Console.WriteLine("3. Check for drug allergies in a prescription ");
            System.Console.WriteLine("4. Calculate the invoice price of the prescription ");
            System.Console.WriteLine("5. Changing the price of the drugs(inflation) ");
            System.Console.WriteLine("6. Add drug in dataset ");
            System.Console.WriteLine("7. remove drug from dataset ");
            System.Console.WriteLine("8. Add disease in dataset ");
            System.Console.WriteLine("9. remove disease from dataset ");
            System.Console.WriteLine("10. Search drug ");
            System.Console.WriteLine("11. Search disease ");
            System.Console.WriteLine("0. to exit program insert q ");
            Console.ForegroundColor = ConsoleColor.White;
            string option;
            int option_number = 0;
            while(option_number != 1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine("For accessing to drugs data insert 1 first of all");
                Console.ForegroundColor = ConsoleColor.White;
                try
                {
                    option = System.Console.ReadLine();
                    option_number = int.Parse(option);
                }
                catch{}
            }
            while (true)
            {
                Stopwatch sw = new Stopwatch();
                while(true)
                {
                    System.Console.WriteLine("Choose Option(1-11)");
                    option = System.Console.ReadLine();
                    try
                    {
                        option_number = int.Parse(option);
                        if(option_number > 11 || option_number < 1) throw new Exception();
                        break;
                    }
                    catch (System.FormatException)
                    {
                        if(option.ToLower() == "q")
                        {
                            Continue = false;
                            break;
                        }
                        Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine("Your input isn't in correct format, please insert number");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    catch (Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine("Your input wasn't in correct range, please insert any number in range 1-10");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                if(!Continue) break;
                try
                {
                    switch (option_number)
                    {
                        case 2:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            System.Console.WriteLine("Insert your drugs name at a single line with space between them.");
                            Console.ForegroundColor = ConsoleColor.White;
                            var drugs = Console.ReadLine().Split();
                            sw.Restart();
                            p.Check_Interaction(drugs);
                            System.Console.WriteLine("time : " + sw.ElapsedMilliseconds + " ms");
                            Console.ForegroundColor = ConsoleColor.White;
                        break;

                        case 3:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            System.Console.WriteLine("Insert your diseases name at first line with space between them.\nInsert your drugs at second line with space between them.");
                            Console.ForegroundColor = ConsoleColor.White;
                            var diseases = Console.ReadLine().Split();
                            drugs = Console.ReadLine().Split();
                            sw.Restart();
                            p.Check_Allergies(drugs, diseases);
                            System.Console.WriteLine("time : " + sw.ElapsedMilliseconds + " ms");
                            Console.ForegroundColor = ConsoleColor.White;
                        break;

                        case 4:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            System.Console.WriteLine("Insert your drugs name at a single line with space between them.");
                            Console.ForegroundColor = ConsoleColor.White;
                            drugs = Console.ReadLine().Split();
                            sw.Restart();
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            System.Console.WriteLine("Total Price : " + p.Get_Invoice_Price(drugs));
                            Console.ForegroundColor = ConsoleColor.White;
                            System.Console.WriteLine("time : " + sw.ElapsedMilliseconds + " ms");
                        break;

                        case 5:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            System.Console.WriteLine("Insert your drugs name at first line with space between them.\nInsert the amount of inflation at second line.");
                            Console.ForegroundColor = ConsoleColor.White;
                            drugs = Console.ReadLine().Split();
                            double inflation = double.Parse(Console.ReadLine());
                            sw.Restart();
                            p.Update_Drug_Price(drugs, inflation);
                            System.Console.WriteLine("time : " + sw.ElapsedMilliseconds + " ms");
                            Console.ForegroundColor = ConsoleColor.White;
                        break;

                        case 6:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            System.Console.WriteLine("Insert your drug name and price base on follow patter.\n<name> <price>");
                            Console.ForegroundColor = ConsoleColor.White;
                            var drug = Console.ReadLine().Split();
                            sw.Restart();
                            p.Create_Drug(drug[0], double.Parse(drug[1]), false);
                            System.Console.WriteLine("time : " + sw.ElapsedMilliseconds + " ms");
                            Console.ForegroundColor = ConsoleColor.White;
                        break;

                        case 7:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            System.Console.WriteLine("Insert your drug name : ");
                            var d = Console.ReadLine();
                            sw.Restart();
                            p.Delete_Drug(d);
                            System.Console.WriteLine("time : " + sw.ElapsedMilliseconds + " ms");
                            Console.ForegroundColor = ConsoleColor.White;
                        break;

                        case 8:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            System.Console.WriteLine("Insert your disease name : ");
                            Console.ForegroundColor = ConsoleColor.White;
                            var disease = Console.ReadLine();
                            sw.Restart();
                            p.Create_Disease(disease, false);
                            System.Console.WriteLine("time : " + sw.ElapsedMilliseconds + " ms");
                            Console.ForegroundColor = ConsoleColor.White;
                        break;

                        case 9:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            System.Console.WriteLine("Insert your disease name : ");
                            Console.ForegroundColor = ConsoleColor.White;
                            disease = Console.ReadLine();
                            sw.Restart();
                            p.Delete_Disease(disease);
                            System.Console.WriteLine("time : " + sw.ElapsedMilliseconds + " ms");
                            Console.ForegroundColor = ConsoleColor.White;
                        break;

                        case 10:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            System.Console.WriteLine("Insert your drug name : ");
                            Console.ForegroundColor = ConsoleColor.White;
                            d = Console.ReadLine();
                            sw.Restart();
                            p.Search_Drug(d);
                            System.Console.WriteLine("time : " + sw.ElapsedMilliseconds + " ms");
                            Console.ForegroundColor = ConsoleColor.White;
                        break;

                        case 11:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            System.Console.WriteLine("Insert your disease name : ");
                            Console.ForegroundColor = ConsoleColor.White;
                            disease = Console.ReadLine();
                            sw.Restart();
                            p.Search_Disease(disease);
                            System.Console.WriteLine("time : " + sw.ElapsedMilliseconds + " ms");
                            Console.ForegroundColor = ConsoleColor.White;
                        break;
                    }
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("Your input is incorrect, please insert the input according to the instructions of each command");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            p.Dispose();
        }
    }
}
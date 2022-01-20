// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;

// namespace DS
// {
//     /// <summary>Any drug is similar to a node of graph</summary>
//     public class Drug
//     {
//         public double price = 0;
//         public string name;
//         public Drug(string name, double price)
//         {
//             this.name = name;
//             this.price = price;
//         }
//         public override bool Equals(object obj)
//         {
//             var other = obj as Drug;
//             return other.name == this.name;
//         }

//         public override int GetHashCode()
//         {
//             int result = 0;
//             foreach (char c in this.name)
//             {
//                 result += (10*result + (int)(c));
//             }
//             return result;
//         }
//     }

//     public class Pharmacy
//     {
//         string characters = "abcdefghijklmnopqrstuvwxyz";
//         Random Rnd = new Random();
//         public List<Drug> Drugs;
//         public List<string> Disease;
//         public List<List<string>> Drugs_Effect_Graph;
//         public Dictionary<string, Dictionary<string, char>> Allergies_Effect_Graph;
//         public Dictionary<string, int> Drugs_ids;
//         int last_id = 0;
//         public List<Drug> New_Drugs;
//         public Pharmacy()
//         {
//             this.Disease = new List<string>();
//             this.Drugs_Effect_Graph = new List<List<string>>();
//             this.Allergies_Effect_Graph = new Dictionary<string, Dictionary<string, char>>();
//             this.Drugs_ids = new Dictionary<string, int>();
//             this.Drugs = new List<Drug>();
//             #region Read Drug Data
//                 using(StreamReader sr = new StreamReader("./dataset_1.txt"))
//                 {
//                     Drug d;
//                     string line = sr.ReadLine();
//                     while(line != null)
//                     {
//                         var a = line.Split(':').Select(x => x.Trim()).ToArray();
//                         d = new Drug(a[0], double.Parse(a[1]));
//                         Drugs.Add(d);
//                         this.Drugs_ids.Add(d.name, this.last_id);
//                         this.last_id ++;
//                         line = sr.ReadLine();
//                     }
//                 }
//                 var column = Enumerable.Range(0, this.Drugs.Count).Select(x => String.Empty).ToList();
//                 for(int i = 0; i < this.Drugs.Count; i++)
//                 {
//                     this.Drugs_Effect_Graph.Add(column);
//                 }
//             #endregion Read Drug Data

//             #region Read Disease Data
//                 using(StreamReader sr = new StreamReader("./dataset_2.txt"))
//                 {
//                     string disease = sr.ReadLine();
//                     while(disease != null)
//                     {
//                         this.Disease.Add(disease);
//                         disease = sr.ReadLine();
//                     }
//                 }
//                 Dictionary<string, char> disease_effect = new Dictionary<string, char>();
//                 for(int i = 0; i<this.Drugs.Count; i++)
//                 {
//                     disease_effect.Add(this.Drugs[i].name, '#');
//                 }
//                 string dis;
//                 for(int i = 0; i < this.Disease.Count; i++)
//                 {
//                     dis = this.Disease[i];
//                     this.Allergies_Effect_Graph.Add(this.Disease[i], disease_effect);
//                 }
//             #endregion Read Disease Data
//         }

//         public void Make_Drug_Interactions()
//         {
//             string[] line;
//             using(StreamReader sr = new StreamReader("./dataset_3.txt"))
//             {
//                 line = sr.ReadLine().Split(new char[]{':', '(', ',', ')', ' '}, StringSplitOptions.RemoveEmptyEntries).ToArray();
//                 int i = Drugs_ids[line[0]];
//                 int j = Drugs_ids[line[0]];
//                 Drugs_Effect_Graph[i][j] = line[2];
//             }
//         }

//         public void Make_Disease_Allergies()
//         {
//             string[] line;
//             Dictionary<string, (string drug, char effect)> drugs_with_effect;
//             using(StreamReader sr = new StreamReader("./dataset_4.txt"))
//             {
//                 line = sr.ReadLine().Split(':').Select(item => item.Trim()).ToArray();
//                 drugs_with_effect = line[1].Split(';').Select(item => { 
//                                 var a = item.Split(new char[]{'(', ')', ','}, StringSplitOptions.RemoveEmptyEntries).Where(x => x!=""&&x !=" ").ToArray();
//                                 return (drug : a[0], effect : a[1][0]);
//                             }).ToDictionary(x => x.drug);
//                 foreach(var item in drugs_with_effect)
//                 {
//                     this.Allergies_Effect_Graph[line[0]][item.Key] = item.Value.effect;
//                 }
//             }
//         }

//         /// <summary>Not Completed (Disease)</summary>
//         public void Create_Drug(string name, double price) 
//         {
//             int random_count = Math.Min(this.Rnd.Next(0, 100), this.Rnd.Next(0, this.Drugs.Count));
//             Drug[] interacted_drugs = new Drug[random_count];
//             int idx = 0;
//             while(idx < random_count)
//             {
//                 var drug_idx = this.Rnd.Next(this.Drugs.Count);
//                 interacted_drugs[idx] = this.Drugs[drug_idx];
//                 idx ++;
//             }
//             Drug d = new Drug(name, price);
//             try
//             {
//                 this.Drugs_ids.Add(name, this.last_id);
//                 this.Drugs.Add(d);
//                 this.New_Drugs.Add(d);
//                 this.last_id ++;
//                 for(int i = 0; i < this.Drugs_Effect_Graph.Count; i++)
//                     this.Drugs_Effect_Graph[i].Add(String.Empty);
//                 string effect = "";
//                 for(int _ = 0; _ < 10; _++)
//                     effect += this.characters[this.Rnd.Next(27)];
//                 for(int u = 0; u < random_count; u++)
//                 {
//                     int i = this.Drugs_ids[name];
//                     int j = this.Drugs_ids[interacted_drugs[u].name];
//                     this.Drugs_Effect_Graph[i][j] = ("Eff_" + effect);
//                 }
//             }
//             catch
//             {
//                 Console.ForegroundColor = ConsoleColor.Red;
//                 System.Console.WriteLine($"This Drug({name}) already existed in dataset.");
//                 Console.ForegroundColor = ConsoleColor.White;
//             }
//         }

//         public void Delete_Drug(Drug d)
//         {
//             int idx = this.Drugs_ids[d.name];
//             for(int i = idx+1; i<this.Drugs_ids.Count; i++)
//             {
//                 var KV = this.Drugs_ids.ElementAt(i);
//                 int v = KV.Value - 1;
//                 this.Drugs_ids[KV.Key] = v;
//             }
//         }

//         public Drug Read_Drug(string name)
//         {
//             throw new NotImplementedException();
//         }

//         public void Create_Disease(string name)
//         {}

//         public void Delete_Disease(string d)
//         {}

//         // public Disease Read_Disease(string name)
//         // {
//         //     throw new NotImplementedException();
//         // }

//         /// <summary> create a random sign, + or - , for dataset_4</summary> <returns>return a char, (+) or (-) </returns>
//         public char createRnd_pos_neg_Effect() => (Rnd.Next() % 2 == 0) ? '+' : '-' ;


//         ~Pharmacy()
//         {}
//     }
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             Pharmacy p = new Pharmacy();
//             p.Make_Drug_Interactions();
//             p.Make_Disease_Allergies();
//             while(true)
//             {
//                 System.Console.WriteLine("1. start ");
//                 System.Console.WriteLine("2. Check for drug interactions in a prescription ");
//                 System.Console.WriteLine("3. Check for drug allergies in a prescription ");
//                 System.Console.WriteLine("4. Calculate the invoice price of the prescription ");
//                 System.Console.WriteLine("5. Changing the price of the drugs ");
//                 System.Console.WriteLine("6. Add or remove from dataset ");
//                 System.Console.WriteLine("7. Search ");
//                 string option;
//                 int option_number;
//                 while(true)
//                 {
//                     try
//                     {
//                         System.Console.WriteLine("Choose Option(1-7)");
//                         option = System.Console.ReadLine();
//                         option_number = int.Parse(option);
//                         if(option_number != 1)
//                         {
//                             throw new Exception("Invalid Input");
//                         }
//                         break;
//                     }
//                     catch (System.FormatException)
//                     {
//                         Console.ForegroundColor = ConsoleColor.Red;
//                         System.Console.WriteLine("Your input isn't in correct format, please insert number");
//                         Console.ForegroundColor = ConsoleColor.White;
//                     }
//                     catch (Exception)
//                     {
//                         Console.ForegroundColor = ConsoleColor.Yellow;
//                         System.Console.WriteLine("For accessing to drugs data insert 1 first of all");
//                         Console.ForegroundColor = ConsoleColor.Red;
//                         System.Console.WriteLine("Your input wasn't in correct range, please insert any number in range 1-7");
//                         Console.ForegroundColor = ConsoleColor.White;
//                     }
//                 }
//                 switch (option_number)
//                 {
//                     case 1:
//                     break;

//                     case 2:
//                     break;

//                     case 3:
//                     break;

//                     case 4:
//                     break;

//                     case 5:
//                     break;

//                     case 6:
//                     break;

//                     case 7:
//                     break;
//                 }
//             }
//         }
//     }
// }

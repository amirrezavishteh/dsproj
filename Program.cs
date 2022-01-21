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
//     }

//     public class Pharmacy
//     {
//         string characters = "abcdefghijklmnopqrstuvwxyz";
//         Random Rnd = new Random();
//         public List<Drug> Drugs;
//         public List<string> Disease;
//         public Dictionary<string, Dictionary<string, string>> Drugs_Effect_Graph;
//         public Dictionary<string, Dictionary<string, char>> Allergies_Effect_Graph;
//         public List<Drug> New_Drugs;
//         public Pharmacy()
//         {
//             this.Disease = new List<string>();
//             this.New_Drugs = new List<Drug>();
//             this.Drugs_Effect_Graph = new Dictionary<string, Dictionary<string, string>>();
//             this.Allergies_Effect_Graph = new Dictionary<string, Dictionary<string, char>>();
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
//                         line = sr.ReadLine();
//                     }
//                 }
//                 Dictionary<string, string> drugs_effect = new Dictionary<string, string>();
//                 for(int i = 0; i< this.Drugs.Count; i++)
//                 {
//                     drugs_effect.Add(this.Drugs[i].name, "");
//                 }
//                 for(int i = 0; i < this.Drugs.Count; i++)
//                 {
//                     this.Drugs_Effect_Graph.Add(this.Drugs[i].name, drugs_effect.ToDictionary(x => x.Key, x => x.Value));
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
//                     this.Allergies_Effect_Graph.Add(this.Disease[i], disease_effect.ToDictionary(x => x.Key, x => x.Value));
//                 }
//             #endregion Read Disease Data
//         }

//         /// <summary>Completed</summary>
//         public void Make_Drug_Interactions()
//         {
//             string[] line;
//             using(StreamReader sr = new StreamReader("./dataset_3.txt"))
//             {
//                 line = sr.ReadLine().Split(':', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
//                 var effected_drugs = line[1].Split(new char[]{';', ' '}, StringSplitOptions.RemoveEmptyEntries)
//                             .Select(item => {
//                                 var a = item.Split(new char[]{'(', ')', ','}, StringSplitOptions.RemoveEmptyEntries);
//                                 return (drug:a[0], effect : a[1]);
//                             }).ToArray();
//                 for(int i = 0; i < effected_drugs.Length; i++)
//                 {
//                     this.Drugs_Effect_Graph[line[0]][effected_drugs[i].drug] = effected_drugs[i].effect;
//                 }
//             }
//         }

//         /// <summary>Completed</summary>
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

//         /// <summary>Completed</summary>
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
//             random_count = Math.Min(this.Rnd.Next(0, 100), this.Rnd.Next(0, this.Disease.Count));
//             string[] allergies_drugs = new string[random_count];
//             idx = 0;
//             while(idx < random_count)
//             {
//                 var drug_idx = this.Rnd.Next(this.Disease.Count);
//                 allergies_drugs[idx] = this.Disease[drug_idx];
//                 idx ++;
//             }
//             Drug d = new Drug(name, price);
//             try
//             {
//                 Dictionary<string, string> initialize_effects = new Dictionary<string, string>();
//                 for(int i = 0; i< this.Drugs.Count; i++)
//                 {
//                     initialize_effects.Add(this.Drugs[i].name, "");
//                 }
//                 this.Drugs_Effect_Graph.Add(name, initialize_effects);
//                 this.Drugs_Effect_Graph[name].Add(name, "");
//                 this.Drugs.Add(d);
//                 this.New_Drugs.Add(d);
//                 string effect = "Eff_";
//                 for(int _ = 0; _ < 10; _++)
//                     effect += this.characters[this.Rnd.Next(26)];
//                 for(int u = 0; u < interacted_drugs.Length; u++)
//                 {
//                     this.Drugs_Effect_Graph[name][interacted_drugs[u].name] = effect;
//                     this.Drugs_Effect_Graph[interacted_drugs[u].name][name] = effect;
//                 }
//                 for(int i = 0; i < allergies_drugs.Length; i++)
//                 {
//                     this.Allergies_Effect_Graph[allergies_drugs[i]][name] = this.createRnd_pos_neg_Effect();
//                 }
//             }
//             catch
//             {
//                 Console.ForegroundColor = ConsoleColor.Red;
//                 System.Console.WriteLine($"This drug({name}) already existed in dataset.");
//                 Console.ForegroundColor = ConsoleColor.White;
//             }
//         }

//         public void Delete_Drug(Drug d)
//         {}

//         public Drug Read_Drug(string name)
//         {
//             throw new NotImplementedException();
//         }

//         public void Create_Disease(string name)
//         {
//             int random_count = Math.Min(this.Rnd.Next(0, 30), this.Rnd.Next(0, this.Drugs.Count));
//             Drug[] efficient_drugs = new Drug[random_count];
//             int idx = 0;
//             while(idx < random_count)
//             {
//                 var drug_idx = this.Rnd.Next(this.Drugs.Count);
//                 efficient_drugs[idx] = this.Drugs[drug_idx];
//                 idx ++;
//             }
//             try
//             {
//                 Dictionary<string, char> drugs_effect = new Dictionary<string, char>();
//                 for(int i = 0; i< this.Drugs.Count; i++)
//                 {
//                     drugs_effect.Add(this.Drugs[i].name, '#');
//                 }
//                 this.Allergies_Effect_Graph.Add(name, drugs_effect);
//                 for(int i = 0; i < efficient_drugs.Length; i++)
//                 {
//                     this.Allergies_Effect_Graph[name][efficient_drugs[i].name] = this.createRnd_pos_neg_Effect();
//                 }
//                 this.Disease.Add(name);
//             }
//             catch
//             {
//                 Console.ForegroundColor = ConsoleColor.Red;
//                 System.Console.WriteLine($"This disease({name}) already existed in dataset.");
//                 Console.ForegroundColor = ConsoleColor.White;
//             }
//         }

//         public void Delete_Disease(string d)
//         {}

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
//             // p.Create_Drug("Dru_dtodslpoig", 850000);
//             // p.Create_Disease("Dis_yueyxfcanr");
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

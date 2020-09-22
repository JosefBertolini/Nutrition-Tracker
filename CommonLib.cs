using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.RightsManagement;
using System.Windows.Input;
using System.Dynamic;
using System.ComponentModel;

namespace Nutrition_Tracker
{

    public static class CommonLib
    {
        public static UnicodeEncoding encoder = new UnicodeEncoding();

        static readonly Regex numeric = new Regex("[^0-9]");
        static readonly Regex alphanumeric = new Regex("[^a-zA-Z0-9]");

        public static bool IsNumeric(string s)
        {
            return numeric.IsMatch(s);
        }

        public static bool IsAplhaNumeric(string s)
        {
            return alphanumeric.IsMatch(s);
        }

        public static string ConcatDoubleString(string s)
        {
            try
            {
                var arr = s.Split('.');
                return arr[0] + "." + arr[1].Substring(0, 2);
            }
            catch (IndexOutOfRangeException _)
            {
                return s;
            }
        }

        public static string[] ArraySlice(string[] source, int start, int length)
        {
            string[] result = new string[length];
            Array.Copy(source, start, result, 0, length);
            return result;
        }


        public static string GetProgressMessageReg(double d)
        {
            if (d >= 1.0)
            {
                return "Congraduations!! YOU DID IT!!";
            }
            else if (d >= .75)
            {
                return "You're almost there PUSH THROUGH!";
            }
            else if (d >= 0.6)
            {
                return "Keep at it! YOU'RE DOING GREAT";
            }
            else if (d >= 0.5)
            {
                return "You're halfway there! You're looking good now! Nothing's gonna get it in the way!";
            }
            else if (d >= 0.3)
            {
                return "You're making real progress! DON'T GIVE UP NOW!";
            }
            else
            {
                return "Still a ways to go, but nothing is impossible";
            }
        }

        public struct MacroFileInfo
        {
            public int kcals;
            public int protein;
            public int carbs;
            public int fats;

            public MacroFileInfo(int k, int p, int c, int f)
            {
                kcals = k;
                protein = p;
                carbs = c;
                fats = f;
            }

            public override string ToString()
            {
                return String.Format("{0}\n{1}\n{2}\n{3}", kcals, protein, carbs, fats);
            }
        }


        public class User
        {
            public enum WEIGHT_MODE
            {
                GAIN,
                LOSE,
                MAINTAIN,
            };

            public static WEIGHT_MODE WEIGHT_MODE_Parse(string s)
            {
                System.Diagnostics.Debug.WriteLine("\n\n\n>" + s + "<\n\n\n");

                switch (s)
                {
                    case "GAIN":
                        return WEIGHT_MODE.GAIN;

                    case "LOSE":
                        return WEIGHT_MODE.LOSE;

                    case "MAINTENANCE":
                        return WEIGHT_MODE.MAINTAIN;

                    default:
                        throw new FormatException("Error in Weight Mode string");
                }


            }


            private MacroFileInfo goal_macros;
            private double start_weight;
            private double current_weight;
            private double target_weight;
            private WEIGHT_MODE weight_mode;
            public readonly string name;
            public readonly string dir_path;

            public int KCalGoal { get => goal_macros.kcals; }
            public int ProteinGoal { get => goal_macros.protein; }
            public int CarbsGoal { get => goal_macros.carbs; }
            public int FatsGoal { get => goal_macros.fats; }
            public double StartWeight { get => start_weight; }
            public double CurrentWeight { get => current_weight; }
            public double TargetWeight { get => target_weight; }
            public WEIGHT_MODE WeightMode { get => weight_mode; }
            
            public User(string _name, string path)
            {
                name = _name;
                dir_path = path;

                string[] user_info = File.ReadAllLines(path + @"\user_info.txt", encoder);
                goal_macros = new MacroFileInfo(
                                        Int32.Parse(user_info[0]),
                                        Int32.Parse(user_info[1]),
                                        Int32.Parse(user_info[2]),
                                        Int32.Parse(user_info[3])
                                        );
                start_weight = double.Parse(user_info[4]);
                current_weight = double.Parse(user_info[5]);
                target_weight = double.Parse(user_info[6]);
                weight_mode = WEIGHT_MODE_Parse(user_info[7]);
            }

            public void UpdateUser(MacroFileInfo macros, double start_weight, double current_weight, double target_weight, string weight_mode)
            {
                goal_macros = macros;
                this.start_weight = start_weight;
                this.current_weight = current_weight;
                this.target_weight = target_weight;
                this.weight_mode = WEIGHT_MODE_Parse(weight_mode);
            }
            
        }

        public static string GetCompletionLevel(double actual, double goal)
        {
            double ratio = (double)actual / goal;

            if (ratio >= 1.0f)
            {
                return "SATISFIED";
            }
            else if (ratio > .50f)
            {
                return "PARTIAL";
            }
            else
            {
                return "FAILED";
            }
        }

        public static string BuildDayFilePath(string path, DateTime day)
        {
            return path + String.Format("\\{0}_{1}_{2}.txt", day.Year, day.Month, day.Day);
        }

        public static (MacroFileInfo, string[]) OpenDayFile(string path)
        {
            try
            {
                string[] lines = File.ReadAllLines(path, encoder);
                int a = Int32.Parse(lines[0]);
                int b = Int32.Parse(lines[1]);
                int c = Int32.Parse(lines[2]);
                int d = Int32.Parse(lines[3]);
                return (new MacroFileInfo(a, b, c, d), ArraySlice(lines, 4, 4));
            }
            catch (FileNotFoundException _)
            {
                string empty = "0\n0\n0\n0\nFAILED\nFAILED\nFAILED\nFAILED";
                var f = File.Create(path);
                f.Close();
                File.AppendAllText(path, empty, encoder);
                return (new MacroFileInfo(0, 0, 0, 0), new string[] { "FAILED", "FAILED", "FAILED", "FAILED" });
            }

        }

        public static void UpdateDayFile(string path, MacroFileInfo macro, string[] completion_levels)
        {
            string content = String.Format("{0}\n{1}\n{2}\n{3}\n{4}", macro.ToString(), completion_levels[0], completion_levels[1], completion_levels[2], completion_levels[3]);
            FileStream fs = File.Open(path, FileMode.Open);
            fs.SetLength(0);
            fs.Write(encoder.GetBytes(content), 0, encoder.GetByteCount(content));
            fs.Flush();
            fs.Close();
        }

        public static void WriteUserInfo(string path, bool creating, MacroFileInfo macros, double start_weight, double current_weight, double target_weight, string weight_mode)
        {
            FileStream fs;

            if (creating)
            {
                fs = File.Create(path + @"\user_info.txt");
            }
            else
            {
                fs = File.Open(path + @"\user_info.txt", FileMode.Open);
            }

            string content = String.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}", 
                                            macros.kcals,
                                            macros.protein,
                                            macros.carbs,
                                            macros.fats,
                                            start_weight,
                                            current_weight,
                                            target_weight,
                                            weight_mode
                                          );

            fs.SetLength(0);
            fs.Write(encoder.GetBytes(content), 0, encoder.GetByteCount(content));
            fs.Flush();
            fs.Close();
        }

    }

    

}

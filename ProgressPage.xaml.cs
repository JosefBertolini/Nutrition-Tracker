using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nutrition_Tracker
{
    using static CommonLib;
     

    public partial class ProgressPage : Page
    {
        Frame main_frame;
        User current_user;
        SolidColorBrush black_fill  = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        SolidColorBrush green_fill  = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
        SolidColorBrush yellow_fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
        SolidColorBrush red_fill    = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        SolidColorBrush gray_fill   = new SolidColorBrush(Color.FromArgb(255, 120, 119, 115));

        List<Canvas> DayGroups;


        private void FillRegWeightBar()
        {
            double weight_diff = current_user.TargetWeight - current_user.StartWeight;
            double weight_change = current_user.CurrentWeight - current_user.StartWeight;
            double ratio = weight_change / weight_diff;
            double shift_distance = ratio > 1 ? 500 : ratio * 500;

            RegWeightProgress.Visibility = Visibility.Visible;

            if ( ((weight_diff > 0.0) && (weight_change > 0.0)) 
                 || ((weight_diff < 0.0) && (weight_change < 0.0)) )
            {
                Thickness temp = CurrentWeightGroup.Margin;
                temp.Left += shift_distance;
                CurrentWeightGroup.Margin = temp;

                WeightRectFill.Width = shift_distance;
                WeightRectFill.Fill = black_fill;
            }

            StartWeightLabel.Content = ConcatDoubleString(current_user.StartWeight.ToString());
            TargetWeightLabel.Content = ConcatDoubleString(current_user.TargetWeight.ToString());
            CurrentWeightLabel.Content = ConcatDoubleString(current_user.CurrentWeight.ToString());
            RegProgressMessage.Text = GetProgressMessageReg(ratio);
        }
        
        private void FillAltWWeightBar()
        {
            AltWeightProgress.Visibility = Visibility.Visible;
            RotateTransform rotate180 = new RotateTransform();
            rotate180.Angle = 180;
            rotate180.CenterY = 10;

            AltCurrentWeightLabel.Content = ConcatDoubleString(current_user.CurrentWeight.ToString());
            AltTargetWeightLabel.Content = ConcatDoubleString(current_user.TargetWeight.ToString());
            
            double weight_change = current_user.CurrentWeight - current_user.TargetWeight;
            double ratio = weight_change / 10.0;
            double abs_ratio = Math.Abs(ratio);
            double shift_distance = abs_ratio > 1 ? 250 : abs_ratio * 250;

            if (ratio != 0.0)
            {
                Thickness temp = AltCurrentWeightGroup.Margin;
                temp.Left += shift_distance * (ratio < 0.0 ? -1 : 1);

                AltWeightRectFill.Width = shift_distance;
                AltWeightRectFill.Fill = black_fill;
                AltCurrentWeightGroup.Margin = temp;
                if (ratio < 0.0)
                    AltWeightRectFill.RenderTransform = rotate180;

                

            }
        }


        private void FillPreviousDays()
        {
            DateTime current_day = DateTime.Today;
            string dir_path = current_user.dir_path + @"\days\";

            foreach(var day_canvas in DayGroups)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("\n\n" + current_day.ToString() + "\n\n");
                    string[] day_file_lines = File.ReadAllLines(BuildDayFilePath(dir_path, current_day), encoder);

                    foreach (var child in day_canvas.Children)
                    {
                        if (child is Ellipse)
                        {
                            Ellipse circle = child as Ellipse;
                            switch (circle.Name.Substring(0, circle.Name.Length - 1))
                            {
                                case "KcalFill":
                                    circle.Fill = GetFillColor(day_file_lines[4]);
                                    break;
                                case "ProteinFill":
                                    circle.Fill = GetFillColor(day_file_lines[5]);
                                    break;
                                case "CarbsFill":
                                    circle.Fill = GetFillColor(day_file_lines[6]);
                                    break;
                                case "FatsFill":
                                    circle.Fill = GetFillColor(day_file_lines[7]);
                                    break;
                                default:
                                    throw new FormatException();
                            }
                        }
                    }
                }
                catch (FileNotFoundException e)
                {
                    foreach (var child in day_canvas.Children)
                    {
                        if (child is Ellipse)
                        {
                            Ellipse circle = child as Ellipse;
                            circle.Fill = gray_fill;
                        }
                    }
                }
                finally
                {
                    current_day = current_day.AddDays(-1);
                }
            }
        }

        private SolidColorBrush GetFillColor(string s)
        {
            switch(s)
            {
                case "SATISFIED":
                    return green_fill;
                case "PARTIAL":
                    return yellow_fill;
                case "FAILED":
                    return red_fill;
                default:
                    throw new FormatException();
            }
        }

        public ProgressPage(Frame f, User u, bool needs_alt = false)
        {
            main_frame = f;
            current_user = u;
            InitializeComponent();

            DayGroups = new List<Canvas>
            {
                Day1Group,
                Day2Group,
                Day3Group,
                Day4Group,
                Day5Group,
                Day6Group,
                Day7Group
            };


            if (needs_alt)
                FillAltWWeightBar();
            else
                FillRegWeightBar();
            
            FillPreviousDays();
        }
    }
}

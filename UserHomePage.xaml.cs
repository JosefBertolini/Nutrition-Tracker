using System;
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

    /// <summary>
    /// Interaction logic for UserHomePage.xaml
    /// </summary>
    public partial class UserHomePage : Page
    {

        Frame content_frame;
        User current_user;
        string current_day_path;
        SolidColorBrush green_fill  = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
        SolidColorBrush yellow_fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
        SolidColorBrush red_fill    = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

        private (SolidColorBrush, double) GetFillAndWidth(int actual, int goal)
        {
            double ratio = (double)actual / goal;
            
            if (ratio > 1.0f)
            {
                return (green_fill, 500);
            }
            else if (ratio > .85f)
            {
                return (green_fill, ratio * 500);
            }
            else if (ratio > .50f)
            {
                return (yellow_fill, ratio * 500);
            }
            else if (ratio < .05f)
            {
                return (red_fill, 25);
            }
            else
            {
                return (red_fill, ratio * 500);
            }
        }

        public UserHomePage(Frame frame_ref, User user)
        {
            content_frame = frame_ref;
            current_user = user;
            current_day_path = CommonLib.BuildDayFilePath(current_user.dir_path + @"\days\", DateTime.Today);
            InitializeComponent();
            WelcomeLabel.Content += user.name + "!";

            MacroFileInfo day_macro;
            string[] completion_levels; 
            (day_macro, completion_levels) = CommonLib.OpenDayFile(current_day_path);

            (KCalRectFill.Fill, KCalRectFill.Width)       = GetFillAndWidth(day_macro.kcals, current_user.KCalGoal);
            (ProteinRectFill.Fill, ProteinRectFill.Width) = GetFillAndWidth(day_macro.protein, current_user.ProteinGoal);
            (CarbsRectFill.Fill, CarbsRectFill.Width)     = GetFillAndWidth(day_macro.carbs, current_user.ProteinGoal);
            (FatsRectFill.Fill, FatsRectFill.Width)       = GetFillAndWidth(day_macro.fats, current_user.FatsGoal);
        }

        private void AddMealButton_Click(object sender, RoutedEventArgs e)
        {
            content_frame.Content = new AddMealPage(content_frame, current_user, current_day_path);
        }
    }
}

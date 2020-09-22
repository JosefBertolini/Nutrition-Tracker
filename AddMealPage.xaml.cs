using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AddMealPage.xaml
    /// </summary>
    public partial class AddMealPage : Page
    {
        Frame content_frame;
        User current_user;
        string day_file_path;


        public AddMealPage(Frame frame, User user, string path_to_day_file)
        {
            content_frame = frame;
            current_user = user;
            day_file_path = path_to_day_file;
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            content_frame.Content = new UserHomePage(content_frame, current_user);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            MacroFileInfo old_macros;
            string[] _;
            (old_macros, _) = CommonLib.OpenDayFile(day_file_path);
            MacroFileInfo new_macros = new MacroFileInfo(
                                                    old_macros.kcals   + Int32.Parse(KCalInput.Text),
                                                    old_macros.protein + Int32.Parse(ProteinInput.Text),
                                                    old_macros.carbs   + Int32.Parse(CarbsInput.Text),
                                                    old_macros.fats    + Int32.Parse(FatsInput.Text)
                                                );

            string[] completion_levels = new string[4] { 
                                                            GetCompletionLevel(new_macros.kcals, current_user.KCalGoal),
                                                            GetCompletionLevel(new_macros.protein, current_user.ProteinGoal),
                                                            GetCompletionLevel(new_macros.carbs, current_user.CarbsGoal),
                                                            GetCompletionLevel(new_macros.fats, current_user.FatsGoal)
                                                       };

            CommonLib.UpdateDayFile(day_file_path, new_macros, completion_levels);

            content_frame.Content = new UserHomePage(content_frame, current_user);
        }

        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsNumeric(e.Text);
        }
    }
}

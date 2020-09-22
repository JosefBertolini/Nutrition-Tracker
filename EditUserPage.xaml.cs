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
    /// Interaction logic for EditUserPage.xaml
    /// </summary>
    public partial class EditUserPage : Page
    {
        Frame main_frame;
        User current_user;

        Dictionary<string, bool> TextBoxHasData = new Dictionary<string, bool>
        {
            { "KCalInput", true },
            { "ProteinInput", true },
            { "CarbsInput", true },
            { "FatsInput", true },
            { "StartWeightWhole", true },
            { "StartWeightDecimal", true },
            { "TargetWeightWhole", true },
            { "TargetWeightDecimal", true }
        };

        private void FillBoxes()
        {
            KCalInput.Text    = current_user.KCalGoal.ToString();
            ProteinInput.Text = current_user.ProteinGoal.ToString();
            CarbsInput.Text   = current_user.CarbsGoal.ToString();
            FatsInput.Text    = current_user.FatsGoal.ToString();

            var start_whole = Math.Round(current_user.StartWeight, MidpointRounding.ToZero);
            var start_decimal = Math.Round(current_user.StartWeight - Math.Truncate(current_user.StartWeight), 2);
            StartWeightWhole.Text   = start_whole.ToString();
            if (start_decimal != 0.0)
                StartWeightDecimal.Text = start_decimal.ToString().Split('.')[1];
            else
                StartWeightDecimal.Text = "0";

            var target_whole = Math.Round(current_user.TargetWeight, MidpointRounding.ToZero);
            var target_decimal = Math.Round(current_user.TargetWeight - Math.Truncate(current_user.TargetWeight), 2);
            TargetWeightWhole.Text   = target_whole.ToString();
            if (target_decimal != 0.0)
                TargetWeightDecimal.Text = target_decimal.ToString().Split('.')[1];
            else
                TargetWeightDecimal.Text = "0";

            var current_whole = Math.Round(current_user.CurrentWeight, MidpointRounding.ToZero);
            var current_decimal = Math.Round(current_user.CurrentWeight - Math.Truncate(current_user.CurrentWeight), 2);
            CurrentWeightWhole.Text   = current_whole.ToString();
            if (current_decimal != 0.0)
                CurrentWeightDecimal.Text = current_decimal.ToString().Split('.')[1];
            else
                CurrentWeightDecimal.Text = "0";

            if (current_user.WeightMode.Equals(User.WEIGHT_MODE.MAINTAIN))
                InMaintenance.IsChecked = true;
        }


        public EditUserPage(Frame f, User u)
        {
            main_frame = f;
            current_user = u;
            InitializeComponent();
            FillBoxes();
        }

        private bool CheckIfAllBoxesHaveData()
        {
            foreach (var bueno in TextBoxHasData)
            {
                if (!bueno.Value)
                    return false;
            }
            return true;
        }

        private void ProcessText(TextBox tb)
        {
            TextBoxHasData[tb.Name] = !tb.Text.Equals(string.Empty);
            UpdateButton.IsEnabled = CheckIfAllBoxesHaveData();
        }


        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsNumeric(e.Text);
        }

        private void UpdateStates(object sender, TextChangedEventArgs e)
        {
            if (!e.Handled)
                ProcessText((TextBox)sender);
            if (InMaintenance.IsChecked.Value)
            {
                if ((sender as TextBox).Name.Equals("StartWeightWhole"))
                {
                    ProcessText(TargetWeightWhole);
                }
                else if ((sender as TextBox).Name.Equals("StartWeightWhole"))
                {
                    ProcessText(TargetWeightDecimal);
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Binding whole_binding = new Binding("Text");
            Binding decimal_binding = new Binding("Text");
            whole_binding.Source = StartWeightWhole;
            decimal_binding.Source = StartWeightDecimal;

            TargetWeightWhole.SetBinding(TextBox.TextProperty, whole_binding);
            TargetWeightDecimal.SetBinding(TextBox.TextProperty, decimal_binding);

            TargetWeightWhole.IsReadOnly = true;
            TargetWeightDecimal.IsReadOnly = true;
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            BindingOperations.ClearBinding(TargetWeightWhole, TextBox.TextProperty);
            BindingOperations.ClearBinding(TargetWeightDecimal, TextBox.TextProperty);

            TargetWeightWhole.IsReadOnly = false;
            TargetWeightDecimal.IsReadOnly = false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            main_frame.Content = new UserHomePage(main_frame, current_user);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            MacroFileInfo goals = new MacroFileInfo(
                                                       int.Parse(KCalInput.Text),
                                                       int.Parse(ProteinInput.Text),
                                                       int.Parse(CarbsInput.Text),
                                                       int.Parse(FatsInput.Text)
                                                   );

            float start_weight = float.Parse(String.Format("{0}.{1}", StartWeightWhole.Text, StartWeightDecimal.Text));
            float target_weight = float.Parse(String.Format("{0}.{1}", TargetWeightWhole.Text, TargetWeightDecimal.Text));
            float current_weight = float.Parse(String.Format("{0}.{1}", CurrentWeightWhole.Text, CurrentWeightDecimal.Text));
            string weight_mode;
            if (InMaintenance.IsChecked.Value)
                weight_mode = "MAINTENANCE";
            else if (target_weight > start_weight)
                weight_mode = "GAIN";
            else
                weight_mode = "LOSE";

            WriteUserInfo(current_user.dir_path, false, goals, start_weight, current_weight, target_weight, weight_mode);
            current_user.UpdateUser(goals, start_weight, current_weight, target_weight, weight_mode);
        }
    }
}

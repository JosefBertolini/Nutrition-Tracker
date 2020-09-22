using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for CreateUserPage.xaml
    /// </summary>
    public partial class CreateUserPage : Page
    {
        private Frame main_frame;
        private string path;

        // Jank AF but it'll do
        Dictionary<string, bool> TextBoxHasData = new Dictionary<string, bool> 
        {
            { "NameInput", false },
            { "KCalInput", false },
            { "ProteinInput", false },
            { "CarbsInput", false },
            { "FatsInput", false },
            { "StartWeightWhole", false },
            { "StartWeightDecimal", false },
            { "TargetWeightWhole", false },
            { "TargetWeightDecimal", false }
        };


        public CreateUserPage(Frame f, string p)
        {
            main_frame = f;
            path = p;
            InitializeComponent();
        }

        private bool CheckIfAllBoxesHaveData()
        {
            foreach(var bueno in TextBoxHasData)
            {
                if (!bueno.Value)
                    return false;
            }
            return true;
        }

        private void ProcessText(TextBox tb)
        {
            TextBoxHasData[tb.Name] = !tb.Text.Equals(string.Empty);
            CreateButton.IsEnabled = CheckIfAllBoxesHaveData();
        }

        private void NameTextPreview(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsAplhaNumeric(e.Text);
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
            main_frame.Content = new LogInPage(main_frame);
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            MacroFileInfo goals = new MacroFileInfo (
                                                        int.Parse(KCalInput.Text), 
                                                        int.Parse(ProteinInput.Text), 
                                                        int.Parse(CarbsInput.Text), 
                                                        int.Parse(FatsInput.Text)
                                                    );

            float start_weight = float.Parse(String.Format("{0}.{1}", StartWeightWhole.Text, StartWeightDecimal.Text));
            float target_weight = float.Parse(String.Format("{0}.{1}", TargetWeightWhole.Text, TargetWeightDecimal.Text));
            string weight_mode;
            if (InMaintenance.IsChecked.Value)
                weight_mode = "MAINTENANCE";
            else if (target_weight > start_weight)
                weight_mode = "GAIN";
            else
                weight_mode = "LOSE";


            var user_dir = Directory.CreateDirectory(path + @"\" + NameInput.Text);
            Directory.CreateDirectory(user_dir.FullName + @"\days\");
            WriteUserInfo(user_dir.FullName, true, goals, start_weight, start_weight, target_weight, weight_mode);

            main_frame.Content = new UserMode(main_frame, new User(NameInput.Text, user_dir.FullName));
        }

    }


}

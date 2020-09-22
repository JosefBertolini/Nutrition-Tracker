using System;
using System.IO;
using System.Linq;
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
    /// Interaction logic for LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {

        private Frame main_frame;

        public LogInPage(Frame frame_ref)
        {
            main_frame = frame_ref;
            InitializeComponent();
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            string users_dir = Directory.GetCurrentDirectory() + @"\users\";

            var users_dict = new Dictionary<string, string>();

            foreach (string user in Directory.EnumerateDirectories(users_dir, "*"))
            {
                users_dict.Add(user.Split(@"\").Last(), user);
            }

            if (users_dict.ContainsKey(UsernameBox.Text))
            {
                main_frame.Content = new UserMode(main_frame, new User(UsernameBox.Text, users_dict[UsernameBox.Text]));
            }
            else
            {
                LogInErrorWindow errorWindow = new LogInErrorWindow();
                errorWindow.Show();
            }

        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            main_frame.Content = new CreateUserPage(main_frame, Directory.GetCurrentDirectory() + @"\users\");
        }
    }
}

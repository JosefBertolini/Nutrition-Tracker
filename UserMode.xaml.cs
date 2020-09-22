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

    public partial class UserMode : Page
    {
        private Frame main_frame;
        User current_user;
        
        public UserMode(Frame frame_ref, User user)
        {
            main_frame = frame_ref;
            current_user = user;
            InitializeComponent();
            UserContentFrame.Content = new UserHomePage(UserContentFrame, user);
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            UserContentFrame.Content = new UserHomePage(UserContentFrame, current_user);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            UserContentFrame.Content = new EditUserPage(UserContentFrame, current_user);
        }

        private void ProgressButton_Click(object sender, RoutedEventArgs e)
        {
            if (current_user.WeightMode.Equals(User.WEIGHT_MODE.MAINTAIN))
                UserContentFrame.Content = new ProgressPage(UserContentFrame, current_user, true);
            else
                UserContentFrame.Content = new ProgressPage(UserContentFrame, current_user);
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            main_frame.Content = new LogInPage(main_frame);
        }
    }
}

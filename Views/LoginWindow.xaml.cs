using System;
using System.Windows;
using AyurvedaClinicManagementSystem.Data;

namespace AyurvedaClinicManagementSystem.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Test DB connection ONLY
                DatabaseHelper db = new DatabaseHelper();

                using (var conn = db.GetConnection())
                {
                    conn.Open();
                }

                MessageBox.Show("Login Successful!");

                // Open dashboard safely
                DashboardWindow dashboard = new DashboardWindow();
                dashboard.Show();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
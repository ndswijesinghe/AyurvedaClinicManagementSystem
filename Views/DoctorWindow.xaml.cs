using AyurvedaClinicManagementSystem.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows;

namespace AyurvedaClinicManagementSystem.Views
{
    public partial class DoctorWindow : Window
    {
        private void txtSearchDoctor_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();

                string query = @"SELECT * FROM Doctor
                         WHERE Name LIKE @search
                         OR Specialization LIKE @search";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearchDoctor.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);

                DoctorGrid.ItemsSource = dt.DefaultView;
            }
        }
        DatabaseHelper db = new DatabaseHelper();
        int selectedDoctorId = -1;

        public DoctorWindow()
        {
            InitializeComponent();
            LoadDoctors();
        }

        // ================= SAVE =================
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO Doctor
                                    (Name, Specialization, PhoneNumber)
                                    VALUES (@n, @s, @p)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@n", txtName.Text);
                    cmd.Parameters.AddWithValue("@s", txtSpecialization.Text);
                    cmd.Parameters.AddWithValue("@p", txtPhone.Text);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Doctor Saved!");
                LoadDoctors();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= LOAD =================
        private void LoadDoctors()
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Doctor", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                DoctorGrid.ItemsSource = dt.DefaultView;
            }
        }

        // ================= DELETE =================
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedDoctorId == -1)
                {
                    MessageBox.Show("Please select a doctor first!");
                    return;
                }

                MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to delete this doctor?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                    return;

                DatabaseHelper db = new DatabaseHelper();

                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    string query = "DELETE FROM Doctor WHERE DoctorID=@id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", selectedDoctorId);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Doctor deleted successfully!");

                LoadDoctors();
                ClearFields();
                selectedDoctorId = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= UPDATE =================
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedDoctorId == -1)
                {
                    MessageBox.Show("Select a doctor first!");
                    return;
                }

                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE Doctor
                                     SET Name=@n,
                                         Specialization=@s,
                                         PhoneNumber=@p
                                     WHERE DoctorID=@id";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@n", txtName.Text);
                    cmd.Parameters.AddWithValue("@s", txtSpecialization.Text);
                    cmd.Parameters.AddWithValue("@p", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@id", selectedDoctorId);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Updated!");
                LoadDoctors();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= SELECT =================
        private void DoctorGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DoctorGrid.SelectedItem == null)
                return;

            DataRowView row = (DataRowView)DoctorGrid.SelectedItem;

            selectedDoctorId = Convert.ToInt32(row["DoctorID"]);
            txtName.Text = row["Name"].ToString();
            txtSpecialization.Text = row["Specialization"].ToString();
            txtPhone.Text = row["PhoneNumber"].ToString();
        }

        // ================= CLEAR =================
        private void ClearFields()
        {
            txtName.Clear();
            txtSpecialization.Clear();
            txtPhone.Clear();
            selectedDoctorId = -1;
        }
    }
}
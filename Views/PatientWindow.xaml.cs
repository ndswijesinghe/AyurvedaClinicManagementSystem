using AyurvedaClinicManagementSystem.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AyurvedaClinicManagementSystem.Views
{
    public partial class PatientWindow : Window
    {
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            DatabaseHelper db = new DatabaseHelper();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string query = @"SELECT * FROM Patient
                         WHERE FirstName LIKE @search
                         OR LastName LIKE @search
                         OR PhoneNumber LIKE @search";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                PatientGrid.ItemsSource = dt.DefaultView;
            }
        }
        private int selectedPatientId = -1;

        public PatientWindow()
        {
            InitializeComponent();
            LoadPatients();
        }

        // ================= SAVE =================
        private void SavePatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DatabaseHelper db = new DatabaseHelper();

                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO Patient 
                    (FirstName, LastName, Gender, PhoneNumber, Address, DateOfBirth)
                    VALUES (@fn, @ln, @g, @p, @a, @dob)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@fn", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@ln", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@g", txtGender.Text);
                    cmd.Parameters.AddWithValue("@p", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@a", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@dob", dpDateOfBirth.SelectedDate ?? DateTime.Now);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Patient Saved!");

                LoadPatients();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= UPDATE =================
        private void UpdatePatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 🔥 FORCE UI TO COMMIT VALUES
                Keyboard.ClearFocus();
                PatientGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                PatientGrid.CommitEdit(DataGridEditingUnit.Row, true);

                // Get selected ID properly
                if (PatientGrid.SelectedItem is DataRowView row)
                {
                    selectedPatientId = Convert.ToInt32(row["PatientID"]);
                }

                if (selectedPatientId == -1)
                {
                    MessageBox.Show("Please select a patient first!");
                    return;
                }

                DatabaseHelper db = new DatabaseHelper();

                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE Patient 
                             SET FirstName=@fn,
                                 LastName=@ln,
                                 Gender=@g,
                                 PhoneNumber=@p,
                                 Address=@a,
                                 DateOfBirth=@dob
                             WHERE PatientID=@id";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@fn", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@ln", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@g", txtGender.Text);
                    cmd.Parameters.AddWithValue("@p", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@a", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@dob", dpDateOfBirth.SelectedDate ?? DateTime.Now);
                    cmd.Parameters.AddWithValue("@id", selectedPatientId);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                        MessageBox.Show("Patient Updated Successfully!");
                    else
                        MessageBox.Show("Update failed!");
                }

                LoadPatients();
                ClearFields();
                selectedPatientId = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= DELETE =================
        private void DeletePatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Check if patient selected
                if (selectedPatientId == -1)
                {
                    MessageBox.Show("Please select a patient first!");
                    return;
                }

                // 2. Confirmation box
                MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to delete this patient?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    return;
                }

                // 3. Database connection
                DatabaseHelper db = new DatabaseHelper();

                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    // 4. Delete query
                    string query = "DELETE FROM Patient WHERE PatientID=@id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", selectedPatientId);

                    cmd.ExecuteNonQuery();
                }

                // 5. Success message
                MessageBox.Show("Patient deleted successfully!");

                // 6. Refresh grid
                LoadPatients();

                // 7. Clear form
                ClearFields();

                // 8. Reset ID
                selectedPatientId = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= LOAD =================
        private void LoadPatients()
        {
            try
            {
                DatabaseHelper db = new DatabaseHelper();

                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT * FROM Patient";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    PatientGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= SELECT ROW =================
        private void PatientGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PatientGrid.SelectedItem == null)
                return;

            DataRowView row = (DataRowView)PatientGrid.SelectedItem;

            selectedPatientId = Convert.ToInt32(row["PatientID"]);

            txtFirstName.Text = row["FirstName"].ToString();
            txtLastName.Text = row["LastName"].ToString();
            txtGender.Text = row["Gender"].ToString();
            txtPhone.Text = row["PhoneNumber"].ToString();
            txtAddress.Text = row["Address"].ToString();

            if (row["DateOfBirth"] != DBNull.Value)
                dpDateOfBirth.SelectedDate = Convert.ToDateTime(row["DateOfBirth"]);
        }

        // ================= ENTER KEY MOVE =================
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                (Keyboard.FocusedElement as UIElement)?.MoveFocus(
                    new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        // ================= CLEAR =================
        private void ClearFields()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtGender.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            dpDateOfBirth.SelectedDate = null;
        }
    }
}
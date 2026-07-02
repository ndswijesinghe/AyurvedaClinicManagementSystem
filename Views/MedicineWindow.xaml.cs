using AyurvedaClinicManagementSystem.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace AyurvedaClinicManagementSystem.Views
{
    public partial class MedicineWindow : Window
    {
        DatabaseHelper db = new DatabaseHelper();
        int selectedMedicineId = -1;

        public MedicineWindow()
        {
            InitializeComponent();
            LoadMedicines();
        }

        // ================= LOAD =================
        private void LoadMedicines()
        {
            try
            {
                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Medicine", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    MedicineGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= SAVE =================
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO Medicine
                                    (MedicineName, Category, Quantity, Price, ExpiryDate)
                                    VALUES (@n, @c, @q, @p, @e)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@n", txtMedicineName.Text);
                    cmd.Parameters.AddWithValue("@c", txtCategory.Text);
                    cmd.Parameters.AddWithValue("@q", txtQuantity.Text);
                    cmd.Parameters.AddWithValue("@p", txtPrice.Text);
                    cmd.Parameters.AddWithValue("@e", dpExpiryDate.SelectedDate ?? DateTime.Now);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Medicine Saved!");
                LoadMedicines();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= DELETE =================
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Check selection
                if (selectedMedicineId == -1)
                {
                    MessageBox.Show("Please select a medicine first!");
                    return;
                }

                // 2. Confirmation
                MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to delete this medicine?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    return;
                }

                // 3. DB connection
                DatabaseHelper db = new DatabaseHelper();

                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    string query = "DELETE FROM Medicine WHERE MedicineID=@id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", selectedMedicineId);

                    cmd.ExecuteNonQuery();
                }

                // 4. Success message
                MessageBox.Show("Medicine deleted successfully!");

                // 5. Refresh grid
                LoadMedicines();

                // 6. Clear form
                ClearFields();

                // 7. Reset selection
                selectedMedicineId = -1;
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
                if (selectedMedicineId == -1)
                {
                    MessageBox.Show("Select a medicine first!");
                    return;
                }

                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE Medicine
                                     SET MedicineName=@n,
                                         Category=@c,
                                         Quantity=@q,
                                         Price=@p,
                                         ExpiryDate=@e
                                     WHERE MedicineID=@id";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@n", txtMedicineName.Text);
                    cmd.Parameters.AddWithValue("@c", txtCategory.Text);
                    cmd.Parameters.AddWithValue("@q", txtQuantity.Text);
                    cmd.Parameters.AddWithValue("@p", txtPrice.Text);
                    cmd.Parameters.AddWithValue("@e", dpExpiryDate.SelectedDate ?? DateTime.Now);
                    cmd.Parameters.AddWithValue("@id", selectedMedicineId);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Updated!");
                LoadMedicines();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= SELECT =================
        private void MedicineGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MedicineGrid.SelectedItem == null)
                return;

            DataRowView row = (DataRowView)MedicineGrid.SelectedItem;

            selectedMedicineId = Convert.ToInt32(row["MedicineID"]);
            txtMedicineName.Text = row["MedicineName"].ToString();
            txtCategory.Text = row["Category"].ToString();
            txtQuantity.Text = row["Quantity"].ToString();
            txtPrice.Text = row["Price"].ToString();
        }

        // ================= SEARCH =================
        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();

                string query = @"SELECT * FROM Medicine
                                 WHERE MedicineName LIKE @s
                                 OR Category LIKE @s";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@s", "%" + txtSearch.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);

                MedicineGrid.ItemsSource = dt.DefaultView;
            }
        }

        // ================= CLEAR =================
        private void ClearFields()
        {
            txtMedicineName.Clear();
            txtCategory.Clear();
            txtQuantity.Clear();
            txtPrice.Clear();
            selectedMedicineId = -1;
        }
    }
}
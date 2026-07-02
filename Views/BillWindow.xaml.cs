using System;
using System.Data;
using System.Windows;
using Microsoft.Data.SqlClient;
using AyurvedaClinicManagementSystem.Data;

namespace AyurvedaClinicManagementSystem.Views
{
    public partial class BillWindow : Window
    {
        
        DatabaseHelper db = new DatabaseHelper();
        private void cmbMedicine_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbMedicine.SelectedItem == null)
                return;

            DataRowView row = (DataRowView)cmbMedicine.SelectedItem;

            txtPrice.Text = row["Price"].ToString();

            CalculateCurrentTotal();
        }
        private void CalculateCurrentTotal()
        {
            if (decimal.TryParse(txtPrice.Text, out decimal price) &&
                int.TryParse(txtQuantity.Text, out int quantity))
            {
                txtTotal.Text = (price * quantity).ToString("0.00");
            }
            else
            {
                txtTotal.Text = "0.00";
            }
        }
        private void txtQuantity_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CalculateCurrentTotal();
        }

        public BillWindow()
        {
            InitializeComponent();
            LoadPatients();
            LoadMedicines();
        }

        // ================= LOAD PATIENTS =================
        private void LoadPatients()
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter("SELECT PatientID, FirstName FROM Patient", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbPatient.ItemsSource = dt.DefaultView;
                cmbPatient.DisplayMemberPath = "FirstName";
                cmbPatient.SelectedValuePath = "PatientID";
            }
        }

        // ================= LOAD MEDICINES =================
        private void LoadMedicines()
        {
            try
            {
                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(
                        "SELECT MedicineID, MedicineName, Price FROM Medicine", conn);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbMedicine.ItemsSource = dt.DefaultView;
                    cmbMedicine.DisplayMemberPath = "MedicineName";
                    cmbMedicine.SelectedValuePath = "MedicineID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= ADD ITEM =================
        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMedicine.SelectedItem == null)
            {
                MessageBox.Show("Select medicine!");
                return;
            }

            DataRowView row = (DataRowView)cmbMedicine.SelectedItem;

            int medicineId = Convert.ToInt32(row["MedicineID"]);
            string medicineName = row["MedicineName"].ToString();
            decimal price = Convert.ToDecimal(row["Price"]);
            int qty = int.Parse(txtQuantity.Text);

            decimal subTotal = price * qty;

            DataTable dt;

            if (BillGrid.ItemsSource == null)
            {
                dt = new DataTable();
                dt.Columns.Add("MedicineID");
                dt.Columns.Add("Medicine");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("Price");
                dt.Columns.Add("SubTotal");
            }
            else
            {
                dt = ((DataView)BillGrid.ItemsSource).ToTable();
            }

            dt.Rows.Add(medicineId, medicineName, qty, price, subTotal);

            BillGrid.ItemsSource = dt.DefaultView;

            txtTotal.Text = CalculateTotal().ToString();
        }

        // ================= SAVE BILL =================
        private void SaveBill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbPatient.SelectedValue == null)
                {
                    MessageBox.Show("Select patient!");
                    return;
                }

                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    // 1. Save Bill
                    string billQuery = "INSERT INTO Bill (PatientID, TotalAmount) VALUES (@p, @t); SELECT SCOPE_IDENTITY();";

                    SqlCommand cmd = new SqlCommand(billQuery, conn);
                    cmd.Parameters.AddWithValue("@p", cmbPatient.SelectedValue);
                    cmd.Parameters.AddWithValue("@t", CalculateTotal());

                    int billId = Convert.ToInt32(cmd.ExecuteScalar());

                    // 2. Save Items
                    foreach (DataRow row in ((DataView)BillGrid.ItemsSource).Table.Rows)
                    {
                        SqlCommand itemCmd = new SqlCommand(
                            "INSERT INTO BillItem (BillID, MedicineID, Quantity, Price) VALUES (@b,@m,@q,@p)", conn);

                        itemCmd.Parameters.AddWithValue("@b", billId);
                        itemCmd.Parameters.AddWithValue("@m", 1); // simple version
                        itemCmd.Parameters.AddWithValue("@q", row["Quantity"]);
                        itemCmd.Parameters.AddWithValue("@p", row["Price"]);

                        itemCmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Bill Saved Successfully!");
                BillGrid.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= CALCULATE TOTAL =================
        private decimal CalculateTotal()
        {
            decimal total = 0;

            if (BillGrid.ItemsSource != null)
            {
                DataTable dt = ((DataView)BillGrid.ItemsSource).ToTable();

                foreach (DataRow row in dt.Rows)
                {
                    total += Convert.ToDecimal(row["Total"]);
                }
            }

            return total;
        }
    }
}
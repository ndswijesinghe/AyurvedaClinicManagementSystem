using System;
using System.Data;
using System.Windows;
using Microsoft.Data.SqlClient;
using AyurvedaClinicManagementSystem.Data;

namespace AyurvedaClinicManagementSystem.Views
{
    public partial class PrescriptionWindow : Window
    {
        DatabaseHelper db = new DatabaseHelper();

        public PrescriptionWindow()
        {
            InitializeComponent();
            LoadPatients();
            LoadDoctors();
            LoadMedicines();
            LoadPrescriptions();
        }

        // ================= LOAD PATIENTS =================
        private void LoadPatients()
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT PatientID, FirstName FROM Patient", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbPatient.ItemsSource = dt.DefaultView;
                cmbPatient.DisplayMemberPath = "FirstName";
                cmbPatient.SelectedValuePath = "PatientID";
            }
        }

        // ================= LOAD DOCTORS =================
        private void LoadDoctors()
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT DoctorID, Name FROM Doctor", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbDoctor.ItemsSource = dt.DefaultView;
                cmbDoctor.DisplayMemberPath = "Name";
                cmbDoctor.SelectedValuePath = "DoctorID";
            }
        }

        // ================= LOAD MEDICINES =================
        private void LoadMedicines()
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT MedicineID, MedicineName FROM Medicine", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbMedicine.ItemsSource = dt.DefaultView;
                cmbMedicine.DisplayMemberPath = "MedicineName";
                cmbMedicine.SelectedValuePath = "MedicineID";
            }
        }

        // ================= SAVE PRESCRIPTION =================
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbPatient.SelectedValue == null ||
                    cmbDoctor.SelectedValue == null ||
                    cmbMedicine.SelectedValue == null)
                {
                    MessageBox.Show("Select Patient, Doctor and Medicine!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDosage.Text))
                {
                    MessageBox.Show("Enter Dosage!");
                    return;
                }

                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO Prescription
                                    (PatientID, DoctorID, MedicineID, Dosage, PrescriptionDate)
                                    VALUES
                                    (@p, @d, @m, @dos, @date)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@p", cmbPatient.SelectedValue);
                    cmd.Parameters.AddWithValue("@d", cmbDoctor.SelectedValue);
                    cmd.Parameters.AddWithValue("@m", cmbMedicine.SelectedValue);
                    cmd.Parameters.AddWithValue("@dos", txtDosage.Text);
                    cmd.Parameters.AddWithValue("@date", dpDate.SelectedDate ?? DateTime.Now);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Prescription saved successfully!");

                LoadPrescriptions();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= LOAD GRID =================
        private void LoadPrescriptions()
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();

                string query = @"SELECT p.PrescriptionID,
                                        pa.FirstName AS Patient,
                                        d.Name AS Doctor,
                                        m.MedicineName,
                                        p.Dosage,
                                        p.PrescriptionDate
                                 FROM Prescription p
                                 JOIN Patient pa ON p.PatientID = pa.PatientID
                                 JOIN Doctor d ON p.DoctorID = d.DoctorID
                                 JOIN Medicine m ON p.MedicineID = m.MedicineID";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                PrescriptionGrid.ItemsSource = dt.DefaultView;
            }
        }

        // ================= DELETE =================
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PrescriptionGrid.SelectedItem == null)
                {
                    MessageBox.Show("Select a record to delete!");
                    return;
                }

                DataRowView row = (DataRowView)PrescriptionGrid.SelectedItem;
                int id = Convert.ToInt32(row["PrescriptionID"]);

                MessageBoxResult result = MessageBox.Show(
                    "Are you sure to delete this prescription?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                    return;

                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(
                        "DELETE FROM Prescription WHERE PrescriptionID=@id", conn);

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Deleted successfully!");

                LoadPrescriptions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= CLEAR FIELDS =================
        private void ClearFields()
        {
            cmbPatient.SelectedIndex = -1;
            cmbDoctor.SelectedIndex = -1;
            cmbMedicine.SelectedIndex = -1;
            txtDosage.Clear();
            dpDate.SelectedDate = null;
        }
    }
}
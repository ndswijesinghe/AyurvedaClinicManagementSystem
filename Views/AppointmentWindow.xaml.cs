using AyurvedaClinicManagementSystem.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows;

namespace AyurvedaClinicManagementSystem.Views
{
    public partial class AppointmentWindow : Window
    {
        private void txtSearchAppointment_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();

                string query = @"SELECT 
                            A.AppointmentID,
                            P.FirstName AS Patient,
                            D.Name AS Doctor,
                            A.AppointmentDate,
                            A.Reason
                         FROM Appointment A
                         JOIN Patient P ON A.PatientID = P.PatientID
                         JOIN Doctor D ON A.DoctorID = D.DoctorID
                         WHERE P.FirstName LIKE @search
                         OR D.Name LIKE @search
                         OR A.Reason LIKE @search";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearchAppointment.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);

                AppointmentGrid.ItemsSource = dt.DefaultView;
            }
        }
        DatabaseHelper db = new DatabaseHelper();

        int selectedAppointmentId = -1;

        public AppointmentWindow()
        {
            InitializeComponent();

            LoadPatients();
            LoadDoctors();
            LoadAppointments();
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

        // ================= SAVE =================
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbPatient.SelectedValue == null || cmbDoctor.SelectedValue == null)
                {
                    MessageBox.Show("Select Patient and Doctor!");
                    return;
                }

                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO Appointment
                                    (PatientID, DoctorID, AppointmentDate, Reason)
                                    VALUES (@p, @d, @date, @r)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@p", Convert.ToInt32(cmbPatient.SelectedValue));
                    cmd.Parameters.AddWithValue("@d", Convert.ToInt32(cmbDoctor.SelectedValue));
                    cmd.Parameters.AddWithValue("@date", dpDate.SelectedDate ?? DateTime.Now);
                    cmd.Parameters.AddWithValue("@r", txtReason.Text);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Appointment Saved!");

                LoadAppointments();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= LOAD APPOINTMENTS =================
        private void LoadAppointments()
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();

                string query = @"SELECT 
                                A.AppointmentID,
                                P.FirstName AS Patient,
                                D.Name AS Doctor,
                                A.AppointmentDate,
                                A.Reason
                                FROM Appointment A
                                JOIN Patient P ON A.PatientID = P.PatientID
                                JOIN Doctor D ON A.DoctorID = D.DoctorID";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                AppointmentGrid.ItemsSource = dt.DefaultView;
            }
        }

        // ================= DELETE =================
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AppointmentGrid.SelectedItem == null)
                {
                    MessageBox.Show("Select appointment first!");
                    return;
                }

                DataRowView row = (DataRowView)AppointmentGrid.SelectedItem;
                int id = Convert.ToInt32(row["AppointmentID"]);

                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(
                        "DELETE FROM Appointment WHERE AppointmentID=@id", conn);

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Deleted!");
                LoadAppointments();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= CLEAR =================
        private void ClearFields()
        {
            cmbPatient.SelectedIndex = -1;
            cmbDoctor.SelectedIndex = -1;
            txtReason.Clear();
            dpDate.SelectedDate = null;
        }

        // ================= GRID SELECT =================
        private void AppointmentGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AppointmentGrid.SelectedItem == null)
                return;

            DataRowView row = (DataRowView)AppointmentGrid.SelectedItem;
            selectedAppointmentId = Convert.ToInt32(row["AppointmentID"]);
        }
    }
}
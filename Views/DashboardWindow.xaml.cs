using System;
using System.Data;
using System.Windows;
using Microsoft.Data.SqlClient;
using AyurvedaClinicManagementSystem.Data;

namespace AyurvedaClinicManagementSystem.Views
{
    public partial class DashboardWindow : Window
    {
        DatabaseHelper db = new DatabaseHelper();

        public DashboardWindow()
        {
            InitializeComponent();
            LoadDashboardData();
        }
        private void OpenBill_Click(object sender, RoutedEventArgs e)
        {
            BillWindow bw = new BillWindow();
            bw.Show();
        }
        private void OpenPrescription_Click(object sender, RoutedEventArgs e)
        {
            PrescriptionWindow pw = new PrescriptionWindow();
            pw.Show();
        }

        // ================= LOAD DASHBOARD COUNTS =================
        private void LoadDashboardData()
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();

                // Patients
                SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM Patient", conn);
                lblPatients.Content = cmd1.ExecuteScalar().ToString();

                // Doctors
                SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM Doctor", conn);
                lblDoctors.Content = cmd2.ExecuteScalar().ToString();

                // Appointments
                SqlCommand cmd3 = new SqlCommand("SELECT COUNT(*) FROM Appointment", conn);
                lblAppointments.Content = cmd3.ExecuteScalar().ToString();
            }
            try
            {
                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    // Patients Count
                    SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM Patient", conn);
                    lblPatients.Content = cmd1.ExecuteScalar().ToString();

                    // Doctors Count
                    SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM Doctor", conn);
                    lblDoctors.Content = cmd2.ExecuteScalar().ToString();

                    // Appointments Count
                    SqlCommand cmd3 = new SqlCommand("SELECT COUNT(*) FROM Appointment", conn);
                    lblAppointments.Content = cmd3.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= OPEN WINDOWS =================
        private void OpenPatient_Click(object sender, RoutedEventArgs e)
        {
            PatientWindow pw = new PatientWindow();
            pw.Show();
        }

        private void OpenDoctor_Click(object sender, RoutedEventArgs e)
        {
            DoctorWindow dw = new DoctorWindow();
            dw.Show();
        }

        private void Appointments_Click(object sender, RoutedEventArgs e)
        {
            AppointmentWindow aw = new AppointmentWindow();
            aw.Show();
        }

        private void OpenMedicine_Click(object sender, RoutedEventArgs e)
        {
            MedicineWindow mw = new MedicineWindow();
            mw.Show();
        }
    }
}
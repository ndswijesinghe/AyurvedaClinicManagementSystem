using System;

namespace AyurvedaClinicManagementSystem.Models
{
    public class Appointment
    {
        public int AppointmentID { get; set; }

        public int PatientID { get; set; }

        public int DoctorID { get; set; }

        public DateTime AppointmentDate { get; set; }
    }
}
using System;
using System.Windows.Forms;

namespace MedicalAppointmentSystem
{
    public partial class MainForm : Form
    {
        private Button btnDoctors;
        private Button btnBook;
        private Button btnManage;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Medical Appointment System - Main";
            this.Width = 500;
            this.Height = 250;
            btnDoctors = new Button() { Text = "View Doctors", Left = 30, Top = 30, Width = 150 };
            btnBook = new Button() { Text = "Book Appointment", Left = 30, Top = 80, Width = 150 };
            btnManage = new Button() { Text = "Manage Appointments", Left = 30, Top = 130, Width = 150 };

            btnDoctors.Click += BtnDoctors_Click;
            btnBook.Click += BtnBook_Click;
            btnManage.Click += BtnManage_Click;

            this.Controls.Add(btnDoctors);
            this.Controls.Add(btnBook);
            this.Controls.Add(btnManage);
        }

        private void BtnDoctors_Click(object? sender, EventArgs e)
        {
            new DoctorListForm().ShowDialog();
        }

        private void BtnBook_Click(object? sender, EventArgs e)
        {
            new AppointmentForm().ShowDialog();
        }

        private void BtnManage_Click(object? sender, EventArgs e)
        {
            new ManageAppointmentsForm().ShowDialog();
        }
    }
}

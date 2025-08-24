using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MedicalAppointmentSystem
{
    public partial class AppointmentForm : Form
    {
        private ComboBox cboDoctor;
        private ComboBox cboPatient;
        private DateTimePicker dtpDate;
        private TextBox txtNotes;
        private Button btnBook;

        public AppointmentForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Book Appointment";
            this.Width = 500;
            this.Height = 350;

            Label lblDoctor = new Label { Left = 20, Top = 20, Text = "Doctor:", Width = 100 };
            Label lblPatient = new Label { Left = 20, Top = 60, Text = "Patient:", Width = 100 };
            Label lblDate = new Label { Left = 20, Top = 100, Text = "Date & Time:", Width = 100 };
            Label lblNotes = new Label { Left = 20, Top = 140, Text = "Notes:", Width = 100 };

            cboDoctor = new ComboBox { Left = 130, Top = 20, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            cboPatient = new ComboBox { Left = 130, Top = 60, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            dtpDate = new DateTimePicker { Left = 130, Top = 100, Width = 300, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm" , ShowUpDown = true};
            txtNotes = new TextBox { Left = 130, Top = 140, Width = 300, Height = 80, Multiline = true };
            btnBook = new Button { Left = 130, Top = 240, Width = 150, Text = "Book" };

            btnBook.Click += BtnBook_Click;
            this.Load += AppointmentForm_Load;

            this.Controls.Add(lblDoctor);
            this.Controls.Add(lblPatient);
            this.Controls.Add(lblDate);
            this.Controls.Add(lblNotes);
            this.Controls.Add(cboDoctor);
            this.Controls.Add(cboPatient);
            this.Controls.Add(dtpDate);
            this.Controls.Add(txtNotes);
            this.Controls.Add(btnBook);
        }

        private void AppointmentForm_Load(object? sender, EventArgs e)
        {
            LoadDoctors();
            LoadPatients();
        }

        private void LoadDoctors()
        {
            try
            {
                using (var con = Db.GetConnection())
                using (var cmd = new SqlCommand("SELECT DoctorID, FullName FROM dbo.Doctors WHERE Availability = 1", con))
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        var dt = new DataTable();
                        dt.Load(reader);
                        cboDoctor.DataSource = dt;
                        cboDoctor.DisplayMember = "FullName";
                        cboDoctor.ValueMember = "DoctorID";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load doctors.\n" + ex.Message);
            }
        }

        private void LoadPatients()
        {
            try
            {
                using (var con = Db.GetConnection())
                using (var cmd = new SqlCommand("SELECT PatientID, FullName FROM dbo.Patients", con))
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        var dt = new DataTable();
                        dt.Load(reader);
                        cboPatient.DataSource = dt;
                        cboPatient.DisplayMember = "FullName";
                        cboPatient.ValueMember = "PatientID";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load patients.\n" + ex.Message);
            }
        }

        private void BtnBook_Click(object? sender, EventArgs e)
        {
            if (cboDoctor.SelectedValue == null || cboPatient.SelectedValue == null)
            {
                MessageBox.Show("Please select a doctor and a patient.");
                return;
            }

            try
            {
                using (var con = Db.GetConnection())
                {
                    con.Open();

                    // Check availability: no overlapping appointment at the same exact minute for the doctor
                    using (var checkCmd = new SqlCommand(@"SELECT COUNT(*) FROM dbo.Appointments 
                                                           WHERE DoctorID=@DoctorID AND CONVERT(varchar(16), AppointmentDate, 120) = CONVERT(varchar(16), @AppointmentDate, 120)", con))
                    {
                        checkCmd.Parameters.AddWithValue("@DoctorID", (int)cboDoctor.SelectedValue);
                        checkCmd.Parameters.AddWithValue("@AppointmentDate", dtpDate.Value);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("The selected doctor already has an appointment at that time.");
                            return;
                        }
                    }

                    using (var cmd = new SqlCommand(@"INSERT INTO dbo.Appointments(DoctorID, PatientID, AppointmentDate, Notes)
                                                      VALUES(@DoctorID, @PatientID, @AppointmentDate, @Notes)", con))
                    {
                        cmd.Parameters.Add("@DoctorID", System.Data.SqlDbType.Int).Value = (int)cboDoctor.SelectedValue;
                        cmd.Parameters.Add("@PatientID", System.Data.SqlDbType.Int).Value = (int)cboPatient.SelectedValue;
                        cmd.Parameters.Add("@AppointmentDate", System.Data.SqlDbType.DateTime).Value = dtpDate.Value;
                        cmd.Parameters.Add("@Notes", System.Data.SqlDbType.VarChar).Value = (object?)txtNotes.Text ?? DBNull.Value;

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                            MessageBox.Show("Appointment booked successfully!");
                        else
                            MessageBox.Show("No rows affected.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to book appointment.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

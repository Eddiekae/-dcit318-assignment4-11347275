using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MedicalAppointmentSystem
{
    public partial class ManageAppointmentsForm : Form
    {
        private DataGridView dgv;
        private Button btnRefresh;
        private Button btnUpdateDate;
        private Button btnDelete;
        private DateTimePicker dtpNewDate;
        private DataSet ds;
        private SqlDataAdapter adapter;

        public ManageAppointmentsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Appointments";
            this.Width = 900;
            this.Height = 500;

            dgv = new DataGridView { Left = 10, Top = 10, Width = 860, Height = 350, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            btnRefresh = new Button { Left = 10, Top = 370, Width = 120, Text = "Refresh" };
            btnUpdateDate = new Button { Left = 140, Top = 370, Width = 160, Text = "Update Date" };
            btnDelete = new Button { Left = 310, Top = 370, Width = 120, Text = "Delete" };
            dtpNewDate = new DateTimePicker { Left = 440, Top = 372, Width = 200, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm", ShowUpDown = true };

            btnRefresh.Click += BtnRefresh_Click;
            btnUpdateDate.Click += BtnUpdateDate_Click;
            btnDelete.Click += BtnDelete_Click;
            this.Load += ManageAppointmentsForm_Load;

            this.Controls.Add(dgv);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnUpdateDate);
            this.Controls.Add(btnDelete);
            this.Controls.Add(dtpNewDate);
        }

        private void ManageAppointmentsForm_Load(object? sender, EventArgs e)
        {
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            try
            {
                using (var con = Db.GetConnection())
                {
                    adapter = new SqlDataAdapter(@"
                        SELECT A.AppointmentID, A.DoctorID, D.FullName AS DoctorName, A.PatientID, P.FullName AS PatientName, 
                               A.AppointmentDate, A.Notes
                        FROM dbo.Appointments A
                        JOIN dbo.Doctors D ON A.DoctorID = D.DoctorID
                        JOIN dbo.Patients P ON A.PatientID = P.PatientID
                        ORDER BY A.AppointmentDate DESC", con);

                    ds = new DataSet();
                    adapter.Fill(ds, "Appointments");
                    dgv.DataSource = ds.Tables["Appointments"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load appointments.\n" + ex.Message);
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e) => LoadAppointments();

        private void BtnUpdateDate_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Select a row first."); return;
            }
            int appointmentId = Convert.ToInt32(dgv.CurrentRow.Cells["AppointmentID"].Value);

            try
            {
                using (var con = Db.GetConnection())
                using (var cmd = new SqlCommand("UPDATE dbo.Appointments SET AppointmentDate=@d WHERE AppointmentID=@id", con))
                {
                    cmd.Parameters.Add("@d", SqlDbType.DateTime).Value = dtpNewDate.Value;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = appointmentId;
                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    MessageBox.Show(rows > 0 ? "Updated." : "No change.");
                    LoadAppointments();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update.\n" + ex.Message);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Select a row first."); return;
            }
            int appointmentId = Convert.ToInt32(dgv.CurrentRow.Cells["AppointmentID"].Value);
            if (MessageBox.Show("Delete this appointment?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            try
            {
                using (var con = Db.GetConnection())
                using (var cmd = new SqlCommand("DELETE FROM dbo.Appointments WHERE AppointmentID=@id", con))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = appointmentId;
                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                    MessageBox.Show(rows > 0 ? "Deleted." : "No change.");
                    LoadAppointments();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete.\n" + ex.Message);
            }
        }
    }
}

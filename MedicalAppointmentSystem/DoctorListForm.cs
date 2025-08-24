using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MedicalAppointmentSystem
{
    public partial class DoctorListForm : Form
    {
        private DataGridView dgv;

        public DoctorListForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Doctors";
            this.Width = 700;
            this.Height = 400;

            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            this.Controls.Add(dgv);
            this.Load += DoctorListForm_Load;
        }

        private void DoctorListForm_Load(object? sender, EventArgs e)
        {
            try
            {
                using (var con = Db.GetConnection())
                using (var cmd = new SqlCommand("SELECT DoctorID, FullName, Specialty, Availability FROM dbo.Doctors", con))
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        var dt = new DataTable();
                        dt.Load(reader);
                        dgv.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load doctors.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

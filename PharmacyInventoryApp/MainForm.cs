using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PharmacyInventoryApp
{
    public partial class MainForm : Form
    {
        private TextBox txtName, txtCategory, txtPrice, txtQuantity, txtSearch;
        private Button btnAdd, btnSearch, btnUpdateStock, btnRecordSale, btnViewAll;
        private DataGridView dgv;
        private ComboBox cboSearch;
        
        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Pharmacy Inventory & Sales";
            this.Width = 1000;
            this.Height = 600;

            Label l1 = new Label { Left = 20, Top = 20, Text = "Name", Width = 80 };
            Label l2 = new Label { Left = 20, Top = 50, Text = "Category", Width = 80 };
            Label l3 = new Label { Left = 20, Top = 80, Text = "Price", Width = 80 };
            Label l4 = new Label { Left = 20, Top = 110, Text = "Quantity", Width = 80 };

            txtName = new TextBox { Left = 110, Top = 20, Width = 200 };
            txtCategory = new TextBox { Left = 110, Top = 50, Width = 200 };
            txtPrice = new TextBox { Left = 110, Top = 80, Width = 200 };
            txtQuantity = new TextBox { Left = 110, Top = 110, Width = 200 };

            btnAdd = new Button { Left = 320, Top = 20, Width = 140, Text = "Add Medicine" };
            btnAdd.Click += BtnAdd_Click;

            // Search
            Label l5 = new Label { Left = 20, Top = 160, Text = "Search", Width = 80 };
            txtSearch = new TextBox { Left = 110, Top = 160, Width = 200 };
            btnSearch = new Button { Left = 320, Top = 160, Width = 140, Text = "Search" };
            btnSearch.Click += BtnSearch_Click;

            // Update stock
            btnUpdateStock = new Button { Left = 320, Top = 50, Width = 140, Text = "Update Stock" };
            btnUpdateStock.Click += BtnUpdateStock_Click;

            // Record sale
            btnRecordSale = new Button { Left = 320, Top = 80, Width = 140, Text = "Record Sale" };
            btnRecordSale.Click += BtnRecordSale_Click;

            // View all
            btnViewAll = new Button { Left = 320, Top = 110, Width = 140, Text = "View All" };
            btnViewAll.Click += BtnViewAll_Click;

            dgv = new DataGridView { Left = 20, Top = 210, Width = 930, Height = 330, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            this.Controls.Add(l1); this.Controls.Add(l2); this.Controls.Add(l3); this.Controls.Add(l4);
            this.Controls.Add(txtName); this.Controls.Add(txtCategory); this.Controls.Add(txtPrice); this.Controls.Add(txtQuantity);
            this.Controls.Add(btnAdd); this.Controls.Add(btnUpdateStock); this.Controls.Add(btnRecordSale); this.Controls.Add(btnViewAll);
            this.Controls.Add(l5); this.Controls.Add(txtSearch); this.Controls.Add(btnSearch);
            this.Controls.Add(dgv);

            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            LoadAllMedicines();
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                using (var con = Db.GetConnection())
                using (var cmd = new SqlCommand("dbo.AddMedicine", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Name", SqlDbType.VarChar, 120).Value = txtName.Text;
                    cmd.Parameters.Add("@Category", SqlDbType.VarChar, 80).Value = txtCategory.Text;
                    cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = decimal.Parse(txtPrice.Text);
                    cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = int.Parse(txtQuantity.Text);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Medicine added.");
                    LoadAllMedicines();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add failed.\n" + ex.Message);
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            try
            {
                using (var con = Db.GetConnection())
                using (var cmd = new SqlCommand("dbo.SearchMedicine", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SearchTerm", SqlDbType.VarChar, 120).Value = txtSearch.Text;
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
                MessageBox.Show("Search failed.\n" + ex.Message);
            }
        }

        private void BtnUpdateStock_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Select a medicine row in the grid to update."); return;
            }
            int medicineId = Convert.ToInt32(dgv.CurrentRow.Cells["MedicineID"].Value);
            if (!int.TryParse(txtQuantity.Text, out int qty))
            {
                MessageBox.Show("Enter a valid Quantity to set."); return;
            }

            try
            {
                using (var con = Db.GetConnection())
                using (var cmd = new SqlCommand("dbo.UpdateStock", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@MedicineID", SqlDbType.Int).Value = medicineId;
                    cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = qty;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Stock updated.");
                    LoadAllMedicines();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed.\n" + ex.Message);
            }
        }

        private void BtnRecordSale_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Select a medicine row in the grid to sell."); return;
            }
            int medicineId = Convert.ToInt32(dgv.CurrentRow.Cells["MedicineID"].Value);
            string qtyStr = Microsoft.VisualBasic.Interaction.InputBox("Quantity sold:", "Record Sale", "1");
            if (!int.TryParse(qtyStr, out int qty) || qty <= 0)
            {
                MessageBox.Show("Invalid quantity."); return;
            }

            try
            {
                using (var con = Db.GetConnection())
                using (var cmd = new SqlCommand("dbo.RecordSale", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@MedicineID", SqlDbType.Int).Value = medicineId;
                    cmd.Parameters.Add("@QuantitySold", SqlDbType.Int).Value = qty;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Sale recorded.");
                    LoadAllMedicines();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sale failed.\n" + ex.Message);
            }
        }

        private void BtnViewAll_Click(object? sender, EventArgs e) => LoadAllMedicines();

        private void LoadAllMedicines()
        {
            try
            {
                using (var con = Db.GetConnection())
                using (var cmd = new SqlCommand("dbo.GetAllMedicines", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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
                MessageBox.Show("Load failed.\n" + ex.Message);
            }
        }
    }
}

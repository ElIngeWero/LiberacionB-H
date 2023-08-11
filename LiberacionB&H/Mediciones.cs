using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LiberacionB_H
{
    public partial class Mediciones : Form
    {
        DataTable dataTable = new DataTable();
        DataGridViewCell celda;

        string partnumber, serialnumber, PN, SN, BN;

        string weldaglet;
        int min, max, med;
        string minimo, maximo, medida, checkbox;
        int i = 0;
        bool chkbox;

        public Mediciones(string partnumber, string serialnumber, string batchnumber)
        {
            InitializeComponent();
            Controls.Add(DGV);
            DGV.CellBeginEdit += DataGridView_CellBeginEdit;
            DGV.CellValidating += DataGridView1_CellValidating;
            PN = partnumber;
            SN = serialnumber;
            BN = batchnumber;
        }

        private void Mediciones_Load_1(object sender, EventArgs e)
        {
           

            dataTable.Columns.Add("WeldAglet", typeof(string));
            dataTable.Columns.Add("Min", typeof(int));
            dataTable.Columns.Add("Max", typeof(int));
            dataTable.Columns.Add("Measure", typeof(int));

            dataTable.Rows.Add("WA01", 35, 38);
            dataTable.Rows.Add("WA02", 35, 38);
            dataTable.Rows.Add("WA03", 20, 25);
            dataTable.Rows.Add("WA04", 20, 25);
            dataTable.Rows.Add("WA05", 40, 45);
            dataTable.Rows.Add("WA06", 40, 45);
            dataTable.Rows.Add("WA07", 28, 33);
            dataTable.Rows.Add("WA08", 28, 33);
            dataTable.Rows.Add("WA09", 35, 40);
            dataTable.Rows.Add("WA10", 35, 40);
            dataTable.Rows.Add("WA11", 38, 43);
            dataTable.Rows.Add("WA12", 38, 43);
            DGV.DataSource = dataTable;

            DGV.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DGV.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DGV.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DGV.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.HeaderText = "Destructiva";
            checkBoxColumn.Name = "checkBoxColumn";
            DGV.Columns.Add(checkBoxColumn);

            celda = DGV.Rows[i].Cells[3];
            celda.DataGridView.CurrentCell = celda;
            celda.Selected = true;
            DGV.BeginEdit(true);

        }


        private void button1_Click(object sender, EventArgs e)
        {
            
            Query insertdata = new Query();
            Query updatebatch = new Query();
            int medi, res;
            string WA;
            DGV.EndEdit();
            (res, medi, WA) = validacion();

            if (res == 5)
            {
                DialogResult result2 = MessageBox.Show("Desea Guardar los datos", "Alerta", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result2 == DialogResult.OK)
                {
                    insertdata.insertDatos(listasWA, listamediciones, listadestructivas, PN, SN, BN);
                    updatebatch.UpdateBatchParts(BN, 0);
                    MessageBox.Show("Datos Guardados con exito\n Batch rechazado", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
            }
            else if (res == 0)
            {
                insertdata.insertDatos(listasWA, listamediciones, listadestructivas, PN, SN, BN);
                updatebatch.UpdateBatchParts(BN, 1);
                MessageBox.Show("Datos Guardados con exito\n Batch Liberado", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }
            
        }

        private void DataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            
            if ((e.RowIndex > (DGV.Rows.Count - 2)) || e.ColumnIndex < 3) 
            {
                e.Cancel = true; 
            }

        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            if (e.ColumnIndex == 3 && e.RowIndex < (DGV.Rows.Count - 1))
            {
                if (!int.TryParse(e.FormattedValue.ToString(), out _))
                {
                    MessageBox.Show("Solo se aceptan valores numericos sin decimales", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true; // Cancela la edición
                }
            }

        }

        List<int> listamediciones = new List<int>();
        List<string> listadestructivas = new List<string>();
        List<string> listasWA = new List<string>();

        private (int, int, string) validacion()
        {
            int res = 0;
            for (int i = 0; i < (DGV.Rows.Count-1); i++)
            {
                weldaglet = DGV.Rows[i].Cells[0].Value.ToString();
                minimo = DGV.Rows[i].Cells[1].Value.ToString();
                min = Convert.ToInt32(minimo);
                maximo = DGV.Rows[i].Cells[2].Value.ToString();
                max = Convert.ToInt32(maximo);
                medida = DGV.Rows[i].Cells[3].Value.ToString();
                try
                {
                    checkbox = DGV.Rows[i].Cells[4].Value.ToString();
                }
                catch (Exception e)
                {
                    checkbox = "False";
                }
                
                chkbox = Convert.ToBoolean(checkbox);

                if (checkbox == "True")
                {
                    checkbox = "1";
                    DGV.Rows[i].Cells[4].Style.BackColor = Color.Green;
                }
                else
                {
                    checkbox = "0";
                    res = 5;
                    DGV.Rows[i].Cells[4].Style.BackColor = Color.Yellow;
                }


                if (medida == "")
                {
                    medida = "0";
                }
                med = Convert.ToInt32(medida);

                if (med < min || med > max)
                {
                    res = 5;
                    DGV.Rows[i].Cells[3].Style.BackColor = Color.Yellow;
                }
                else
                {
                    DGV.Rows[i].Cells[3].Style.BackColor = Color.Green;
                    DGV.Rows[i].Cells[3].Style.ForeColor = Color.White;
                }

                listasWA.Add(weldaglet);
                listamediciones.Add(med);
                listadestructivas.Add(checkbox);
                
            }
            DGV.CurrentCell = DGV.Rows[0].Cells[0];
            return (res, med, weldaglet);

        }

    }
}

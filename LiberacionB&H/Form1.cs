using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiberacionB_H
{
    public partial class Form1 : Form
    {
        private string PN;
        private string SN;
        private string Cavity;
        private string Celda;
        private string BatchNumber, batchdate, CurrentBatch;
        private int Status;
        
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load_1(object sender, EventArgs e)
        {

        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 28)
            {
                Query Consultas = new Query();
                List<string> Lastserials = new List<string>();
                Mediciones form2 = new Mediciones(PN, SN, BatchNumber);
                (PN, SN) = Consultas.GetPNSN(textBox1.Text);
                if (PN != "")
                {
                    Cavity = SN.Substring(18, 2);
                    Celda = SN.Substring(11, 3);
                    label6.Text = Celda;
                    label7.Text = Cavity;

                    if (Celda == "332")
                    {
                        Celda = "32";
                    }

                    (BatchNumber, Status) = Consultas.GetBatchNumber(PN, SN);

                    if (Status == 10 || BatchNumber == " ")
                    {
                        MessageBox.Show($"Pieza no registrada", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        CurrentBatch = Consultas.GetCurrentBatch(Celda, Cavity);
                        label9.Text = BatchNumber;

                        if (BatchNumber == CurrentBatch)
                        {
                            MessageBox.Show($"La pieza corresponde al Batch Activo \n     {BatchNumber}", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {
                            if (Status == 8)
                            {
                                bool partok = false;
                                Lastserials = Consultas.PiezasValidas(BatchNumber);
                                for (int i = 0; i < Lastserials.Count; i++)
                                {
                                    if (SN == Lastserials[i])
                                    {
                                        partok = true;
                                        break;
                                    }
                                    else
                                    {
                                        partok = false; 
                                    }

                                }
                                switch (partok)
                                {
                                    case true:
                                        form2.ShowDialog();
                                        break;
                                    case false:
                                        MessageBox.Show($"Pieza no valida para liberar", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        break;
                                }

                                
                            }
                            else if (Status == 0)
                            {
                                batchdate = Consultas.batchdate(BatchNumber);
                                MessageBox.Show($"Batch rechazado previamente \n     {BatchNumber} \n El dia {batchdate}", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            else if (Status == 1)
                            {
                                batchdate = Consultas.batchdate(BatchNumber);
                                MessageBox.Show($"Batch liberado previamente \n     {BatchNumber} \n El dia {batchdate}", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else if (Status == 5)
                            {
                                batchdate = Consultas.batchdate(BatchNumber);
                                MessageBox.Show($"Batch rechazado previamente, pieza retrabajada \n     {BatchNumber} \n El dia {batchdate}", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Etiqueta Invalida", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                clearform();
            }
        }

        private void clearform()
        {
            textBox1.Text = string.Empty;
            label5.Text = string.Empty;
            label6.Text = string.Empty;
            label7.Text = string.Empty;
            label9.Text = string.Empty;
            textBox1.Focus();
        }


    }

}

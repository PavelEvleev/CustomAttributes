using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomAttributesDataBaseFirst
{
    public partial class Form1 : Form
    {
        private CustomAttributesEntities context = new CustomAttributesEntities();
        public Form1()
        {
            InitializeComponent();
            InitDataGrid();


        }
        private void InitDataGrid()
        {
            dataGridView1.DataSource = context.Persons.Select(p => 
            new { p.ID_Person, p.Name, p.Surename, p.Phone }).ToList();
            int indexPerson = (int)dataGridView1.Rows[0].Cells[0].Value;
            InitTextBoxs(indexPerson);
            InitDataGridView2(indexPerson);
        }
        private void InitTextBoxs(int i)
        {
            Persons tmp = context.Persons.First(p => p.ID_Person == i);
            textBox1.Text = tmp.Name;
            textBox2.Text = tmp.Surename;
            textBox3.Text = tmp.Phone;
        }
        private void InitDataGridView2(int index)
        {
            dataGridView2.DataSource = context.Persons.Where(p => p.ID_Person == index)
                   .Join(context.FullTable, c => c.ID_Person, p => p.ID_Person, (c, p)
                   => new { c.ID_Person, c.Phone, p.ID_Attribute, p.SomeDate })
                   .Join(context.Attributes, c => c.ID_Attribute, p => p.ID_Attribute,
                   (c, p) => new { p.NameAttribute, c.SomeDate }).ToList();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                int i =(int) dataGridView1.Rows[e.RowIndex].Cells[0].Value;
                InitDataGridView2(i);
                InitTextBoxs(i);
            }
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int indexPerson= (int)dataGridView1.Rows[e.RowIndex].Cells[0].Value;
            var listAttributes = context.Attributes.Select(p => p.NameAttribute).ToList();
            Form2 NewAttribute = new Form2(listAttributes);
           
            if (NewAttribute.ShowDialog() == DialogResult.OK)
            {
                if (!AttributeExist(listAttributes, NewAttribute.NameAttribute))
                {
                    Attributes newAtr = new Attributes() { NameAttribute = NewAttribute.NameAttribute };
                    context.Attributes.Add(newAtr);
                    context.SaveChanges();
                }

                int indexAtr=context.Attributes.First(a => a.NameAttribute == NewAttribute.NameAttribute).ID_Attribute;
                FullTable newFullTable = new FullTable() {ID_Person=indexPerson, SomeDate=NewAttribute.SomeData,ID_Attribute=indexAtr };
                context.FullTable.Add(newFullTable);
                context.SaveChanges();
                InitDataGridView2(indexPerson);

            }
        }

        private bool AttributeExist(List<string> listAttributes, string NameAttribute)
        {
            foreach(var item in listAttributes)
            {
                if (item == NameAttribute)
                {
                    return true;
                }
            }
            return false;
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Form2 editAttr = new Form2();
            int rowIndex = e.RowIndex;
            string nameAttr = dataGridView2.Rows[rowIndex].Cells[0].Value.ToString();
            string dataAatr = dataGridView2.Rows[rowIndex].Cells[1].Value.ToString();
            int indAttr = context.Attributes.First(a => a.NameAttribute == nameAttr).ID_Attribute;
            editAttr.textBox1.Text = nameAttr;
            editAttr.textBox2.Text = dataAatr;
            editAttr.textBox1.ReadOnly=true;
            if (editAttr.ShowDialog() == DialogResult.OK)
            {
                if (editAttr.textBox2.Text != dataAatr)
                {
                    int indexPerson = (int)dataGridView1.CurrentRow.Cells[0].Value;
                    context.FullTable.First(x => x.ID_Person == indexPerson && x.SomeDate == dataAatr &&
                    x.ID_Attribute == indAttr).SomeDate = editAttr.textBox2.Text;
                    context.SaveChanges();
                    InitDataGridView2(indexPerson);
                }
            }
        }

        private void Add_button1_Click(object sender, EventArgs e)
        {
            Persons newPerson = new Persons() { Name = "N/A", Phone = "N/A", Surename = "N/A" };
            context.Persons.Add(newPerson);
            context.SaveChanges();
            InitDataGrid();
        }

        private void Click_Submit(object sender, EventArgs e)
        {
            int i=(int)dataGridView1.CurrentRow.Cells[0].Value;
            Persons editingPerson = context.Persons.First(p => p.ID_Person ==i );
            editingPerson.Name = textBox1.Text;
            editingPerson.Surename = textBox2.Text;
            editingPerson.Phone = textBox3.Text;
            context.SaveChanges();
            InitDataGrid();

        }

        private void Click_Delete(object sender, EventArgs e)
        {
            int i = (int)dataGridView1.CurrentRow.Cells[0].Value;
            Persons editingPerson = context.Persons.First(p => p.ID_Person == i);
            context.FullTable.RemoveRange(context.FullTable.Where(x => x.ID_Person == editingPerson.ID_Person).ToList());
            context.Persons.Remove(editingPerson);
            context.SaveChanges();
            InitDataGrid();
            textBox1.Text="";
            textBox2.Text="";
            textBox3.Text="";
        }

        private void Click_Delete_Attribute(object sender, EventArgs e)
        {
            if(dataGridView2.SelectedRows!=null)
            {
                string nameAttribute=dataGridView2.CurrentRow.Cells[0].Value.ToString();
                string data = dataGridView2.CurrentRow.Cells[1].Value.ToString();
                int indAttr = context.Attributes.First(x => x.NameAttribute == nameAttribute).ID_Attribute;
                int indexPerson = (int)dataGridView1.CurrentRow.Cells[0].Value;
                Persons deletAttrPerson = context.Persons.First(p => p.ID_Person == indexPerson);
                context.FullTable.Remove(context.FullTable.First(x=>x.ID_Person==deletAttrPerson.ID_Person&&x.ID_Attribute==indAttr&&x.SomeDate==data));
                deletAttrPerson.FullTable.Remove(context.FullTable.First(x => x.ID_Person == deletAttrPerson.ID_Person && x.ID_Attribute == indAttr && x.SomeDate == data));
                context.SaveChanges();
                InitDataGridView2(indexPerson);
            }
        }
    }
}

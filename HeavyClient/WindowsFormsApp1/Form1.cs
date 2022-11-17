using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.ServiceReference1;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Service1Client serviceClient = new Service1Client();
            JsonOpenRouteService[] jsonRouteService = serviceClient.GetItinerary(float.Parse(textBox1.Text),
                                                                                     float.Parse(textBox2.Text),
                                                                                     float.Parse(textBox3.Text),
                                                                                     float.Parse(textBox4.Text));
            textBox1.ScrollBars = ScrollBars.Both;
            textBox1.WordWrap = false;
            textBox5.Text = jsonRouteService[0].features[0].properties.segments[0].steps[0].instruction;
            for (int i =0; i < jsonRouteService.Length; i++)
            {
                for (int a = 0; a < jsonRouteService[i].features[0].properties.segments[0].steps.Length; a++)
                {
                    textBox5.Text += jsonRouteService[i].features[0].properties.segments[0].steps[a].instruction + Environment.NewLine;
                }
                
            }
        }

        private static async void GenerateExcelSheet()
        {
            Service1Client serviceClient = new Service1Client();
            String[][] Array = serviceClient.GetStats();
            if (Array == null) return;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var file = new FileInfo(@"../../../Excel/statistics.xlsx");

            if (file.Exists)
            {
                file.Delete();
            }

            var package = new ExcelPackage(file);

            var ws = package.Workbook.Worksheets.Add("stations logs");
            var range = ws.Cells.LoadFromArrays(Array);
            range.AutoFitColumns();

            await package.SaveAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GenerateExcelSheet();
        }
    }
}

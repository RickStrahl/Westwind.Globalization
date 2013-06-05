using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms; 

namespace Westwind.Globalization.Designer
{
    public partial class FormLocalization : Form
    {

        public FormLocalization(string ResourceSet,string ConnectionString)
        {
            wwDbResourceConfiguration.Current.ConnectionString = ConnectionString;
            wwDbResourceDataManager Data = new wwDbResourceDataManager();
            InitializeComponent();
        }
        public FormLocalization(string ResourceSet)
        {
            wwDbResourceDataManager Data = new wwDbResourceDataManager();
            InitializeComponent();
        }
        public FormLocalization()
        {
            InitializeComponent();
        }

        private void FormLocalization_Load(object sender, EventArgs e)
        {

        }
    }
}
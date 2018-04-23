using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GridBoundToRuntimeCreatedData {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            gridControl1.DataSource = DataHelper.GetData(10);
            // The grid automatically creates columns for the public fields found in the data source. 
            // Calling the gridView1.PopulateColumns method is not required unless the gridView1.OptionsBehavior.AutoPopulateColumns is disabled
            
            // Create a ComboBox editor that shows available companies in the Company column
            RepositoryItemComboBox riComboBox = new RepositoryItemComboBox();
            riComboBox.Items.AddRange(DataHelper.companies);
            gridControl1.RepositoryItems.Add(riComboBox);
            gridView1.Columns["CompanyName"].ColumnEdit = riComboBox;

            // Specify a different null value text presentation for the Image column
            gridView1.Columns["Image"].RealColumnEdit.NullText = "[load image]";

            //Highlight the RequiredDate cells that match a certain condition.
            GridFormatRule gridFormatRule = new GridFormatRule();
            FormatConditionRuleValue formatConditionRuleValue = new FormatConditionRuleValue();
            gridFormatRule.Column = gridView1.Columns["RequiredDate"];
            formatConditionRuleValue.PredefinedName = "Red Bold Text";
            formatConditionRuleValue.Condition = FormatCondition.Greater;
            formatConditionRuleValue.Value1 = DateTime.Today;
            gridFormatRule.Rule = formatConditionRuleValue;
            gridFormatRule.ApplyToRow = false;
            gridView1.FormatRules.Add(gridFormatRule);

            gridView1.BestFitColumns();
        }

        private void btnClearPayment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            //Change a cell value at the data source level to see the INotifyPropertyChanged interface in action.
            gridView1.CloseEditor();
            Record rec = gridView1.GetFocusedRow() as Record;
            if (rec == null) return;
            rec.Value = 0;
        }

        private void btnSetPayment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            //Change a cell value at the grid level
            gridView1.SetFocusedRowCellValue("Value", 999);
        }
    }

    public class Record : INotifyPropertyChanged {
        public Record() {
        }
        int id;
        public int ID {
            get { return id; }
            set {
                if (id != value) {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }
        
        string text;
        [DisplayName("Company")]
        public string CompanyName {
            get { return text; }
            set {
                if (text != value) {
                    if (string.IsNullOrEmpty(value))
                        throw new Exception();
                    text = value;
                    OnPropertyChanged();
                }
            }
        }
        Nullable<decimal> val;
        [DataType(DataType.Currency)]
        [DisplayName("Payment")]
        public Nullable<decimal> Value {
            get { return val; }
            set {
                if (val != value) {
                    val = value;
                    OnPropertyChanged();
                }
            }
        }
        DateTime dt;
        [DisplayFormat(DataFormatString = "d")]
        public DateTime RequiredDate {
            get { return dt; }
            set {
                if (dt != value) {
                    dt = value;
                    OnPropertyChanged();
                }
            }
        }
        bool state;
        public bool Processed {
            get { return state; }
            set {
                if (state != value) {
                    state = value;
                    OnPropertyChanged();
                }
            }
        }
        Image image;
        public Image Image {
            get { return image; }
            set {
                if (image != value) {
                    image = value;
                    OnPropertyChanged();
                }
            }
        }
        public override string ToString() {
            return string.Format("ID = {0}, Text = {1}", ID, CompanyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DataHelper {

        public static string[] companies = new string[] { "Hanari Carnes", "Que Delícia", "Romero y tomillo", "Mère Paillarde",
            "Comércio Mineiro", "Reggiani Caseifici", "Maison Dewey" };

        public static Image[] images = new Image[] {
            global::GridBoundToRuntimeCreatedData.Properties.Resources.palette_16x16,
            global::GridBoundToRuntimeCreatedData.Properties.Resources.viewonweb_16x16,
            global::GridBoundToRuntimeCreatedData.Properties.Resources.design_16x16,
            global::GridBoundToRuntimeCreatedData.Properties.Resources.piestylepie_16x16,
            global::GridBoundToRuntimeCreatedData.Properties.Resources.alignhorizontaltop2_16x16,
            null
        };

        public static BindingList<Record> GetData(int count) {
            BindingList<Record> records = new BindingList<Record>();
            Random rnd = new Random();
            for (int i = 0; i < count; i++) {
                int n = rnd.Next(10);
                records.Add(new Record() {
                    ID = i + 100,
                    CompanyName = companies[i % companies.Length],
                    RequiredDate = DateTime.Today.AddDays(n - 5),
                    Value = i % 2 == 0 ? (i + 1) * 123 : i * 231,
                    Processed = i % 2 == 0,
                    Image = images[i % images.Length],
                });
            };
            return records;
        }
    }
}

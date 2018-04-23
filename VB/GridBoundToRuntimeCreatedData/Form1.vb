Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraGrid
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms

Namespace GridBoundToRuntimeCreatedData
    Partial Public Class Form1
        Inherits Form

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            gridControl1.DataSource = DataHelper.GetData(10)
            ' The grid automatically creates columns for the public fields found in the data source. 
            ' Calling the gridView1.PopulateColumns method is not required unless the gridView1.OptionsBehavior.AutoPopulateColumns is disabled

            ' Create a ComboBox editor that shows available companies in the Company column
            Dim riComboBox As New RepositoryItemComboBox()
            riComboBox.Items.AddRange(DataHelper.companies)
            gridControl1.RepositoryItems.Add(riComboBox)
            gridView1.Columns("CompanyName").ColumnEdit = riComboBox

            ' Specify a different null value text presentation for the Image column
            gridView1.Columns("Image").RealColumnEdit.NullText = "[load image]"

            'Highlight the RequiredDate cells that match a certain condition.
            Dim gridFormatRule As New GridFormatRule()
            Dim formatConditionRuleValue As New FormatConditionRuleValue()
            gridFormatRule.Column = gridView1.Columns("RequiredDate")
            formatConditionRuleValue.PredefinedName = "Red Bold Text"
            formatConditionRuleValue.Condition = FormatCondition.Greater
            formatConditionRuleValue.Value1 = Date.Today
            gridFormatRule.Rule = formatConditionRuleValue
            gridFormatRule.ApplyToRow = False
            gridView1.FormatRules.Add(gridFormatRule)

            gridView1.BestFitColumns()
        End Sub

        Private Sub btnClearPayment_ItemClick(ByVal sender As Object, ByVal e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnClearPayment.ItemClick
            'Change a cell value at the data source level to see the INotifyPropertyChanged interface in action.
            gridView1.CloseEditor()
            Dim rec As Record = TryCast(gridView1.GetFocusedRow(), Record)
            If rec Is Nothing Then
                Return
            End If
            rec.Value = 0
        End Sub

        Private Sub btnSetPayment_ItemClick(ByVal sender As Object, ByVal e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnSetPayment.ItemClick
            'Change a cell value at the grid level
            gridView1.SetFocusedRowCellValue("Value", 999)
        End Sub
    End Class

    Public Class Record
        Implements INotifyPropertyChanged

        Public Sub New()
        End Sub

        Private id_Renamed As Integer
        Public Property ID() As Integer
            Get
                Return id_Renamed
            End Get
            Set(ByVal value As Integer)
                If id_Renamed <> value Then
                    id_Renamed = value
                    OnPropertyChanged()
                End If
            End Set
        End Property

        Private text As String
        <DisplayName("Company")> _
        Public Property CompanyName() As String
            Get
                Return text
            End Get
            Set(ByVal value As String)
                If text <> value Then
                    If String.IsNullOrEmpty(value) Then
                        Throw New Exception()
                    End If
                    text = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private val? As Decimal
        <DataType(DataType.Currency), DisplayName("Payment")> _
        Public Property Value() As Decimal?
            Get
                Return val
            End Get
            Set(ByVal value? As Decimal)
                If Not val.Equals(value) Then
                    val = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private dt As Date
        <DisplayFormat(DataFormatString := "d")> _
        Public Property RequiredDate() As Date
            Get
                Return dt
            End Get
            Set(ByVal value As Date)
                If dt <> value Then
                    dt = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Private state As Boolean
        Public Property Processed() As Boolean
            Get
                Return state
            End Get
            Set(ByVal value As Boolean)
                If state <> value Then
                    state = value
                    OnPropertyChanged()
                End If
            End Set
        End Property

        Private image_Renamed As Image
        Public Property Image() As Image
            Get
                Return image_Renamed
            End Get
            Set(ByVal value As Image)
                If image_Renamed IsNot value Then
                    image_Renamed = value
                    OnPropertyChanged()
                End If
            End Set
        End Property
        Public Overrides Function ToString() As String
            Return String.Format("ID = {0}, Text = {1}", ID, CompanyName)
        End Function

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = "")
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class

    Public Class DataHelper

        Public Shared companies() As String = { "Hanari Carnes", "Que Delícia", "Romero y tomillo", "Mère Paillarde", "Comércio Mineiro", "Reggiani Caseifici", "Maison Dewey" }

        Public Shared images() As Image = { My.Resources.palette_16x16, My.Resources.viewonweb_16x16, My.Resources.design_16x16, My.Resources.piestylepie_16x16, My.Resources.alignhorizontaltop2_16x16, Nothing }

        Public Shared Function GetData(ByVal count As Integer) As BindingList(Of Record)
            Dim records As New BindingList(Of Record)()
            Dim rnd As New Random()
            For i As Integer = 0 To count - 1
                Dim n As Integer = rnd.Next(10)
                records.Add(New Record() With { _
                    .ID = i + 100, _
                    .CompanyName = companies(i Mod companies.Length), _
                    .RequiredDate = Date.Today.AddDays(n - 5), _
                    .Value = If(i Mod 2 = 0, (i + 1) * 123, i * 231), _
                    .Processed = i Mod 2 = 0, _
                    .Image = images(i Mod images.Length) _
                })
            Next i
            Return records
        End Function
    End Class
End Namespace

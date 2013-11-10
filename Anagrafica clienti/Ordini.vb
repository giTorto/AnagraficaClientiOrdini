

Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data
Imports System.Data.oledb

Public Class Ordini

    Dim Aggiungi As Boolean
    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call ChiudiConn()
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  


        Call ApriConn()
        Primavolta = True
        Call ImpostaDSOrdini()

        grdOrdini.DataSource = BsOrdCliDett
        Call Disabilitasalva()
        Call DisabilitaText()


        Call Abilitanavigatore()
        Call AssociaCampi()
        Call VisualizzaPosizione()

        Call ImpostaComandiOrdini()

        grdOrdini.Columns(0).Visible = False
        grdOrdini.Columns(1).Visible = False
        grdOrdini.Columns(2).HeaderText = "Codice Articolo"
        grdOrdini.Columns(3).HeaderText = "Descrizione"
        grdOrdini.Columns(4).HeaderText = "UM"
        grdOrdini.Columns(5).HeaderText = "Quantità"
        grdOrdini.Columns(6).HeaderText = "Prezzo Unitario"
        grdOrdini.Columns(7).HeaderText = "Sconto 1"
        grdOrdini.Columns(8).HeaderText = "Sconto 2"
        grdOrdini.Columns(9).HeaderText = "Sconto 3"



        Call ImpostaComboGriglia()

    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        BsOrdcliTes.AddNew()

        Aggiungi = True
        Call AbilitaGrid()
        Call AbilitaSalva()
        Call AbilitaText()
        Call DisabilitaNavigatore()

        txtPosizione.Text = "Aggiunta"


    End Sub
    Private Sub btnSalva_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSalva.Click


        Dim NumOrd As Integer
        Dim DataOrd As Date
        Dim DataCons As Date
        Dim i As Integer
        Dim PKOrdTes As Integer
        Dim NumDettagli As Integer
        Dim FineDettagli As Integer
        Dim InizioDettagli As Integer




        NumOrd = CInt(txtNumOrdine.Text)


        If Not IsDate(txtDataOrdine.Text) Or Not IsDate(txtDataConsegna.Text) Then
            MsgBox("Inserire una data valida", MsgBoxStyle.Information, "Data non valida")
        End If

        DataOrd = CDate(txtDataOrdine.Text)
        DataCons = CDate(txtDataConsegna.Text)
        ' ----------------------------------
        BsOrdcliTes.EndEdit()
        Dim cb As New OleDb.OleDbCommandBuilder(DAOrdCliTes)
        DAOrdCliTes.Update(DSOrdini, "OrdCliTes")


        If Aggiungi Then
            Aggiungi = False
            Call LeggiPKOrdTes(NumOrd, PKOrdTes)

            NumDettagli = BsOrdCliDett.Count
            FineDettagli = DSOrdini.Tables("OrdCliDett").Rows.Count
            InizioDettagli = FineDettagli - NumDettagli

            DSOrdini.Tables("OrdCliDett").BeginLoadData()

            For i = InizioDettagli To FineDettagli - 1
                DSOrdini.Tables("OrdCliDett").Rows(i).Item("FkOrdCliTes") = PKOrdTes
            Next

            BsOrdCliDett.EndEdit()
            Dim cbDett As New OleDb.OleDbCommandBuilder(DAOrdCliDett)
            DAOrdCliDett.Update(DSOrdini, "OrdCliDett")

            DSOrdini.Clear()
            Call RiempiDSOrdini()
            BsOrdcliTes.MoveLast()


            '-----
        Else

            BsOrdCliDett.EndEdit()
            Dim cb2 As New OleDb.OleDbCommandBuilder(DAOrdCliDett)
            DAOrdCliDett.Update(DSOrdini, "OrdCliDett")
        End If

        Call Disabilitasalva()
        Call DisabilitaText()

        Call Abilitanavigatore()




    End Sub
    Private Sub btnElimina_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        Dim CodOrdine As Integer

        CodOrdine = CInt(lblCodOrdine.Text)

        cmdEliminaOrdCliTes.Parameters.Add(New OleDb.OleDbParameter("PKOrdCliTes", CodOrdine))
        cmdEliminaOrdCliDett.Parameters.Add(New OleDb.OleDbParameter("FKOrdCliTes", CodOrdine))

        cmdEliminaOrdCliDett.ExecuteNonQuery()
        cmdEliminaOrdCliTes.ExecuteNonQuery()

        cmdEliminaOrdCliTes.Parameters.Clear()
        cmdEliminaOrdCliDett.Parameters.Clear()

        DSOrdini.Clear()
        Call ImpostaDSOrdini()

        Call VisualizzaPosizione()

    End Sub
    Private Sub btnFine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFine.Click
        Me.Close()
    End Sub
    Private Sub btnAnnulla_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        BSOrdCliTes.CancelEdit()

        Call Disabilitasalva()
        Call disabilitatext()

        Call Abilitanavigatore()
        Call VisualizzaPosizione()

    End Sub
    Private Sub btnModifica_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        Aggiungi = False

        Call AbilitaSalva()
        Call AbilitaText()
        Call AbilitaGrid()
    End Sub

    Private Sub AssociaCampi()


        txtNumOrdine.DataBindings.Add("text", BsOrdcliTes, "NumOrd")
        txtDataOrdine.DataBindings.Add("text", BsOrdcliTes, "DataOrdine")
        txtDataConsegna.DataBindings.Add("text", BsOrdcliTes, "DataCons")
        lblCodOrdine.DataBindings.Add("text", BsOrdcliTes, "PKOrdCliTes")


        Call ImpostaComboORD()

        cmbCliente.DataBindings.Add("SelectedValue", BsOrdcliTes, "FKCliente")
        cmbCliente.DataSource = dtCLIORD
        cmbCliente.DisplayMember = "RagioneSociale"
        cmbCliente.ValueMember = "PKCliente"

        cmbPag.DataBindings.Add("SelectedValue", BsOrdcliTes, "FKPagamento")
        cmbPag.DataSource = dtPAGORD
        cmbPag.DisplayMember = "Modalita"
        cmbPag.ValueMember = "PKPagamento"

        grdOrdini.Columns.Remove("CodIva")

    End Sub

    Private Sub ImpostaComboGriglia()



        Call ComboIvaDaDt()


        Dim cmbIvaDet As New DataGridViewComboBoxColumn

        cmbIvaDet.HeaderText = "IVA"
        cmbIvaDet.DataSource = dtIVA
        cmbIvaDet.DisplayMember = "Aliquota"
        cmbIvaDet.ValueMember = "PKIva"
        cmbIvaDet.DataPropertyName = "FkIva"

        grdOrdini.Columns.Add(cmbIvaDet)
    End Sub

    '----------- NAVIGATORE ----------

    Private Sub btnPrimo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrimo.Click
        BsOrdcliTes.Position = 0

        Call VisualizzaPosizione()
    End Sub
    Private Sub btnPrima_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrima.Click
        BsOrdcliTes.Position = BsOrdcliTes.Position - 1

        Call VisualizzaPosizione()
    End Sub
    Private Sub btnDopo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDopo.Click
        BsOrdcliTes.Position = BsOrdcliTes.Position + 1

        Call VisualizzaPosizione()
    End Sub
    Private Sub btnUltimo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUltimo.Click
        BsOrdcliTes.Position = BsOrdcliTes.Count - 1

        Call VisualizzaPosizione()
    End Sub

    Private Sub VisualizzaPosizione()
        Dim totale As Integer
        Dim posizione As Integer
        Dim stringaposizione As String

        totale = BsOrdcliTes.Count
        posizione = BsOrdcliTes.Position + 1
        stringaposizione = CStr(posizione) & " di " & CStr(totale)
        txtPosizione.Text = stringaposizione

    End Sub

    '----------- ABILITAZIONE - DISABILITAZIONE ---------

    Private Sub AbilitaSalva()
        grpConferma.Enabled = True
        grpOper.Enabled = False
        btnFine.Enabled = False
    End Sub
    Private Sub Disabilitasalva()
        grpConferma.Enabled = False
        grpOper.Enabled = True
        btnFine.Enabled = True
    End Sub

    Private Sub AbilitaText()
        grpText.Enabled = True
    End Sub
    Private Sub DisabilitaText()
        grpText.Enabled = False
    End Sub

    Private Sub AbilitaGrid()
        grdOrdini.Enabled = True
    End Sub
   

    Private Sub Abilitanavigatore()
        btnPrimo.Enabled = True
        btnUltimo.Enabled = True
        btnPrima.Enabled = True
        btnDopo.Enabled = True
    End Sub
    Private Sub DisabilitaNavigatore()
        btnPrimo.Enabled = False
        btnUltimo.Enabled = False
        btnPrima.Enabled = False
        btnDopo.Enabled = False
    End Sub

    Private Sub BtnScegli_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnScegli.Click
        Dim TestoQuery As String
        Dim NomeForm As String
        Dim IDTrovato As String
        Dim Pos As Integer
        Dim IDAnnulla As String





        TestoQuery = "SELECT * FROM tblOrdCliTes"
        NomeForm = "Selezione Clienti"
        NasColonna = True
        IDAnnulla = lblCodOrdine.Text


        RicercaDati(Me, TestoQuery, NomeForm, IDTrovato, NasColonna)

        If IDTrovato = "" Then
            IDTrovato = IDAnnulla
        Else
            Pos = BsOrdcliTes.Find("PKOrdCliTes", CInt(IDTrovato))

        End If
        BsOrdcliTes.Position = Pos
        Call VisualizzaPosizione()
    End Sub


    Private Sub grdOrdini_CellDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles grdOrdini.CellDoubleClick
        Dim Riga As Integer
        Dim Colonna As Integer

        Dim TestoQuery As String
        Dim NomeForm As String
        Dim IDTrovato As String



        Colonna = e.ColumnIndex
        Riga = e.RowIndex

        If Colonna = 3 Then


            TestoQuery = "SELECT * FROM tblArticoli"
            NomeForm = "Selezione Articoli"
            NasColonna = False


            Call RicercaDati(Me, TestoQuery, NomeForm, IDTrovato, NasColonna)
        End If

        grdOrdini.Rows(Riga).Cells(Colonna).Value = IDTrovato



    End Sub
End Class

Imports System.Drawing.Printing
Imports System.Globalization
Imports System.Drawing.Drawing2D

Public Class Form1 ' VisitesTD
    Private ReadOnly DT As String
    Public Sub New()
        ' Cet appel est requis par le concepteur.
        InitializeComponent()
        ' Ajoutez une initialisation quelconque après l'appel InitializeComponent().

    End Sub
    Private Sub Form1_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint
        Using gp As New GraphicsPath
        End Using
        e.Graphics.Clear(SystemColors.Window)
        e.Graphics.SmoothingMode = SmoothingMode.None
        Call DrawRoundRect(e.Graphics, Pens.Green, TabControl1.Location.X - 20, TabControl1.Location.Y - 20, TabControl1.Width + 25, TabControl1.Height + 90, 15)
    End Sub
    Public Sub DrawRoundRect(ByVal g As Graphics, ByVal p As Pen, ByVal x As Single, ByVal y As Single, ByVal width As Single, ByVal height As Single, ByVal radius As Single)
        NewMethod(g, p, x, y, width, height, radius)
    End Sub
    Private Shared Sub NewMethod(g As Graphics, p As Pen, x As Single, y As Single, width As Single, height As Single, radius As Single)
        Using gp As New GraphicsPath()
            Using Motif = New HatchBrush(HatchStyle.Percent50, Color.DarkGreen, Color.White)
                gp.StartFigure()
                gp.AddArc(x + width - radius, y, radius * 2, radius * 2, 270, 90)
                gp.AddArc(x + width - radius, y + height - radius, radius * 2, radius * 2, 0, 90)
                gp.AddArc(x, y + height - radius, radius * 2, radius * 2, 90, 90)
                gp.AddArc(x, y, radius * 2, radius * 2, 180, 90)
                gp.CloseFigure()
                ' g.FillPath(Brushes.Black, gp)
                g.FillPath(Motif, gp)
            End Using
            g.DrawPath(p, gp)
        End Using
    End Sub
    Private Sub BtnImprimeCN_Click(sender As Object, e As EventArgs) Handles BtnImprimeCN.Click
        MyDGV = DataGridView3
        NumeroDePage = 1
        Previsualisation()
    End Sub
    Private Sub BtnImpressionKardex_Click(sender As Object, e As EventArgs) Handles BtnImpressionKardex.Click
        MyDGV = DataGridView1
        NumeroDePage = 1
        Previsualisation()
    End Sub
    Private Sub Previsualisation()
        Dim Apercu As PrintPreviewDialog
        Apercu = New PrintPreviewDialog With {
            .Document = PrintDocument1,
            .WindowState = FormWindowState.Maximized
        }
        PrintDocument1.DefaultPageSettings.Landscape = True
        Apercu.Show()
    End Sub
    'Imprimer DGV 1
    'variables pour impression
    Private oStringFormat As StringFormat
    Private nTotalWidth As Short
    Private nRowPos As Short
    Private NewPage As Boolean
    Private nPageNo As Short
    Private NumeroDePage As Short = 1
    Private MyDGV As DataGridView
    Private EnteteImmat As String = "PA28-181 Archer II F-GATD  S/N 28-78-90138  " & " HT cellule: " & HTCellule & " HT Moteur: " & String.Format("{0:F2}", HTMoteur) & "  HT Hélice: " & String.Format("{0:F2}", HTHelice)
    Private UserName As String = "G. ULRIC"
    Private Sub PrintDocument1_BeginPrint(ByVal sender As Object, ByVal e As PrintEventArgs) Handles PrintDocument1.BeginPrint
        oStringFormat = New StringFormat With {
                    .Alignment = StringAlignment.Near,
                    .LineAlignment = StringAlignment.Center,
                    .Trimming = StringTrimming.Character
                }
        nTotalWidth = 0
        For Each oColumn As DataGridViewColumn In MyDGV.Columns
            nTotalWidth += oColumn.Width
        Next
        nPageNo = 1
        NewPage = True
        nRowPos = 0
    End Sub
    Private Sub PrintDocument1_PrintPage(ByVal sender As Object, ByVal e As PrintPageEventArgs) Handles PrintDocument1.PrintPage
        Dim oColumnLefts As New ArrayList
        Dim oColumnWidths As New ArrayList
        Dim oColumnTypes As New ArrayList
        Dim nHeight As Short
        Dim nWidth, i, nRowsPerPage As Short
        Dim nTop As Short = e.MarginBounds.Top
        Dim nLeft As Short = e.MarginBounds.Left
        Dim Motif = New HatchBrush(HatchStyle.LightDownwardDiagonal, Color.Black, Color.White)
        PrintDocument1.DefaultPageSettings.Landscape = True
        If nPageNo = 1 Then
            For Each oColumn As DataGridViewColumn In MyDGV.Columns
                nWidth = CType(Math.Floor(oColumn.Width / nTotalWidth * nTotalWidth * (e.MarginBounds.Width / nTotalWidth)), Short)
                nHeight = e.Graphics.MeasureString(oColumn.HeaderText, oColumn.InheritedStyle.Font, nWidth).Height '+ 12
                oColumnLefts.Add(nLeft)
                oColumnWidths.Add(nWidth)
                oColumnTypes.Add(oColumn.GetType)
                nLeft += nWidth
            Next
        End If
        Do While nRowPos < MyDGV.Rows.Count - 1
            Dim oRow As DataGridViewRow = MyDGV.Rows(nRowPos)
            If nTop + nHeight >= e.MarginBounds.Height + e.MarginBounds.Top Then
                DrawFooter(e, nRowsPerPage)
                NewPage = True
                ' nPageNo += 1
                NumeroDePage += 1
                e.HasMorePages = True
                Exit Sub
            Else
                If NewPage Then
                    ' Draw Entête Immatriculation
                    e.Graphics.DrawString(EnteteImmat, New Font(MyDGV.Font, FontStyle.Bold), Brushes.Blue, e.MarginBounds.Left,
                      e.MarginBounds.Top - e.Graphics.MeasureString(EnteteImmat, New Font(MyDGV.Font, FontStyle.Bold), e.MarginBounds.Width).Height - 13)
                    ' Draw Columns
                    nTop = e.MarginBounds.Top '- 40
                    i = 0
                    For Each oColumn As DataGridViewColumn In MyDGV.Columns
                        e.Graphics.FillRectangle(New SolidBrush(Color.Yellow), New Rectangle(oColumnLefts(i), nTop, oColumnWidths(i), nHeight))
                        e.Graphics.DrawRectangle(Pens.Black, New Rectangle(oColumnLefts(i), nTop, oColumnWidths(i), nHeight))
                        e.Graphics.DrawString(oColumn.HeaderText, oColumn.InheritedStyle.Font, New SolidBrush(oColumn.InheritedStyle.ForeColor), New RectangleF(oColumnLefts(i), nTop, oColumnWidths(i), nHeight), oStringFormat)
                        i += 1
                    Next
                    NewPage = False
                End If
                nTop += nHeight
                i = 0
                'Cellules
                For Each oCell As DataGridViewCell In oRow.Cells
                    If oCell.ColumnIndex > 1 And oCell.Value.ToString = "" Then
                        e.Graphics.FillRectangle(Motif, New Rectangle(oColumnLefts(i), nTop, oColumnWidths(i), nHeight))
                    End If
                    If oColumnTypes(i) Is GetType(DataGridViewTextBoxColumn) OrElse oColumnTypes(i) Is GetType(DataGridViewLinkColumn) Then
                        e.Graphics.DrawString(oCell.Value.ToString, oCell.InheritedStyle.Font, New SolidBrush(oCell.InheritedStyle.ForeColor), New RectangleF(oColumnLefts(i), nTop, oColumnWidths(i), nHeight), oStringFormat)
                    ElseIf oColumnTypes(i) Is GetType(DataGridViewImageColumn) Then
                        Dim oCellSize As Rectangle = New Rectangle(oColumnLefts(i), nTop, oColumnWidths(i), nHeight)
                        Dim oImageSize As Size = CType(oCell.Value, Image).Size
                        e.Graphics.DrawImage(oCell.Value, New Rectangle(oColumnLefts(i) + CType((oCellSize.Width - oImageSize.Width) / 2, Integer),
                        y:=nTop + CType((oCellSize.Height - oImageSize.Height) / 2, Integer), width:=CType(oCell.Value, Image).Width, height:=CType(oCell.Value, Image).Height))
                    End If

                    e.Graphics.DrawRectangle(Pens.Black, New Rectangle(oColumnLefts(i), nTop, oColumnWidths(i), nHeight))
                    i += 1
                Next
            End If
            nRowPos += 1
            nRowsPerPage += 1
        Loop
        ' If nRowsPerPage > 0 Then
        DrawFooter(e, nRowsPerPage)
        '      e.HasMorePages = True
        'Else
        e.HasMorePages = False
        'End If
    End Sub
    Private Sub DrawFooter(ByVal e As PrintPageEventArgs, ByVal RowsPerPage As Integer)
        Dim NbrePageSur As Short = Math.Ceiling(MyDGV.Rows.Count / RowsPerPage)
        ' Dim sPageNo As String = NumeroDePage.ToString + " Of " + Math.Ceiling(MyDGV.Rows.Count / RowsPerPage).ToString
        If nRowPos >= MyDGV.Rows.Count - 1 Then
            NbrePageSur = NumeroDePage
        End If
        Dim sPageNo As String = NumeroDePage.ToString + " Of " + NbrePageSur.ToString 'Math.Ceiling(MyDGV.Rows.Count / RowsPerPage).ToString
        ' Right(Align - UserName)
        e.Graphics.DrawString(UserName, MyDGV.Font, Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width - e.Graphics.MeasureString(sPageNo, MyDGV.Font, e.MarginBounds.Width).Width), e.MarginBounds.Top + e.MarginBounds.Height + 50)
        'Left Align - Date / Time
        e.Graphics.DrawString(Now.ToLongDateString + " " + Now.ToShortTimeString, MyDGV.Font, Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top + e.MarginBounds.Height + 50)
        'Center  -Page No. Info
        e.Graphics.DrawString(sPageNo, MyDGV.Font, Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width - e.Graphics.MeasureString(sPageNo, MyDGV.Font, e.MarginBounds.Width).Width) / 2, e.MarginBounds.Top + e.MarginBounds.Height + 50)
    End Sub
    Private Function SPageNo() As String
        Throw New NotImplementedException()
    End Function
    Private DateVP As Date '= #dd/mm/yyyy#
    Private DossierVP As String
    Private ObsApresVP As String
    Private EntreDateVP As String
    Private VisiteP As String
    Private HeureEntreVP As String
    Private MyCN As String
    Private ReadOnly MonDGV As String
    Private MonDossierDeTravail As String
    Private HeuresVP, HeureEntree, HTCellule, HTMoteur, HMoteurDepuisRG, ResteMoteur, HTHelice, HHeliceDepuisRG, ResteHelice, HeuresAllouees As Decimal
    Private newVisitesTDDossier As String
    Private newVisitesTDDate_VP As Date
    Private newVisitesTDVisite_Programmée As String
    Private newVisitesTDHeures_VP As Decimal
    Private newVisitesTD_Cellule As Decimal
    Private newVisitesTDHT_Moteur As Decimal
    Private newVisitesTDMoteur_Depuis_RG As Decimal
    Private newVisitesTDReste_Moteur As Decimal
    Private newVisitesTDHT_Hélice As Decimal
    Private newVisitesTDHélice_Depuis_RG As Decimal
    Private newVisitesTDReste_Hélice As Decimal
    Private newVisitesTDHeures_Allouees As Decimal
    Private newVisitesTDObservations As String
    Public ReadOnly Property MRow1 As Integer
        Get
            Return MRow2
        End Get
    End Property
    Public ReadOnly Property MRow2 As Integer = 0
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Charger() ' Charge Kardex
        Presentation() 'Présentation Kardex
        Label17.Text = Date.Now.ToShortDateString
        Label16.Text = "S/N: 28-78-90138"
        'TODO: cette ligne de code charge les données dans la table 'TDDatabaseDataSet.VisitesTD'. Vous pouvez la déplacer ou la supprimer selon les besoins.
        VisitesTDTableAdapter.Fill(TDDatabaseDataSet.VisitesTD)
        Try
            LitLesDernieresDonees()
        Catch ex As Exception
            InitialisationDonnesInitiales()
        End Try
    End Sub
    Private Sub VisitesTDBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs) Handles VisitesTDBindingNavigatorSaveItem.Click
        Try
            Validate()
            VisitesTDBindingSource.EndEdit()
            Dim form1 As Form1 = Me
            form1.TableAdapterManager.UpdateAll(form1.TDDatabaseDataSet)
            MsgBox("Sauvegarde réussie.")
        Catch ex As Exception
            MsgBox("Echec de la sauvegarde")
        End Try
        RefreshTDDatabaseDataset()
    End Sub
    ' Private Sub InsertBtn_Click(sender As Object, e As EventArgs) Handles InsertBtn.Click
    Private Sub InitialisationDonnesInitiales()
        Using VisitesTDTableAdapter As New TDDatabaseDataSetTableAdapters.VisitesTDTableAdapter
            Try
                Dim form1 As Form1 = Me
                Me.VisitesTDTableAdapter.Insert("DT18-077", "19/10/2018", "50H", 00.00, 9731.18, 4337.0, 1012.48, 1387.12, 2837.48, 855.06, 1144.54, 50.0, "Renouvellement CEN - ATA 500H")
                '  Me.VisitesTDTableAdapter.Insert("DT19-009", "06/02/2019", "50H", 49.54, 9831.3, 4437.12, 1113.0, 1287.0, 2938.0, 955.18, 1044.42, 50, "")
                HeuresVisite.Text = If(HeuresVisite.Text = "100H", "50H", "100H")
                InsertBtn.Enabled = False
                InsertBtn.Visible = False
            Catch ex1 As Exception
                MessageBox.Show("Echec de l'Insertion.")
            End Try
        End Using
        RefreshTDDatabaseDataset()
        LitLesDernieresDonees()
        HeuresCellule.Text = $"{HTCellule,8:F2}"
        HeuresMoteur.Text = $"{HTMoteur,8:F2}"
        MoteurRG.Text = $"{HMoteurDepuisRG,8:F2}"
        HeuresHelice.Text = $"{HTHelice,8:F2}"
        HeliceRG.Text = $"{HHeliceDepuisRG,8:F2}"
        VisitesTDDataGridView.CurrentCell = VisitesTDDataGridView(0, VisitesTDDataGridView.Rows.Count - 2)
        '    Label2.Text = VisitesTDDataGridView.Rows.Count
    End Sub
    Private Sub VisitesTDDataGridView_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles VisitesTDDataGridView.CellContentClick
        'LblPosition.Text = String.Format("Ligne: {0} Colonne: {1}", VisitesTDDataGridView.CurrentCell.RowIndex, VisitesTDDataGridView.CurrentCell.ColumnIndex) 'VisitesTDBindingSource.Position + 1, VisitesTDBindingSource.Count)
    End Sub
    'Lecture des dernières données varables
    Private Sub LitLesDernieresDonees()
        Dim NLigneActive As Integer = VisitesTDDataGridView.Rows.Count - 2
        If NLigneActive < 0 Then NLigneActive = 0
        ' Pour obtenir la version actuelle d'un enregistrement
        Dim currentCellule = TDDatabaseDataSet.VisitesTD(NLigneActive)(
            columnName:="Cellule",
            version:=DataRowVersion.Current).ToString()
        HTCellule = currentCellule

        Dim currentMoteur = TDDatabaseDataSet.VisitesTD(NLigneActive)(
            columnName:="HT Moteur",
            version:=DataRowVersion.Current).ToString()
        HTMoteur = currentMoteur

        Dim currentMoteurRG = TDDatabaseDataSet.VisitesTD(NLigneActive)(
        columnName:="Moteur Depuis RG", version:=DataRowVersion.Current).ToString()
        HMoteurDepuisRG = currentMoteurRG

        Dim currentRMoteur = TDDatabaseDataSet.VisitesTD(NLigneActive)(
      columnName:="Reste Moteur", version:=DataRowVersion.Current).ToString()
        ResteMoteur = currentRMoteur

        Dim currentHelice = TDDatabaseDataSet.VisitesTD(NLigneActive)(
       columnName:="HT Hélice", version:=DataRowVersion.Current).ToString()
        HTHelice = currentHelice

        Dim currentHeliceRG = TDDatabaseDataSet.VisitesTD(NLigneActive)(
      columnName:="Hélice Depuis RG", version:=DataRowVersion.Current).ToString()
        HHeliceDepuisRG = currentHeliceRG

        Dim currentRHelice = TDDatabaseDataSet.VisitesTD(NLigneActive)(
       columnName:="Reste Hélice", version:=DataRowVersion.Current).ToString()
        ResteHelice = currentRHelice
    End Sub
    Private Sub RefreshTDDatabaseDataset()
        VisitesTDTableAdapter.Fill(TDDatabaseDataSet.VisitesTD)
    End Sub
    ' Routine principale de mise à jour des données
    Public Sub HeuresVisite_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HeuresVisite.Click
        LitLesDernieresDonees()
        EntreDonnes()
    End Sub
    Private Sub EntreDonnes()
SaisieDossterVP:
        DossierVP = InputBox("Dossier de travail: ") ', MsgBoxStyle.OkCancel)
        If DossierVP = "" Then Exit Sub
        'VerificationDoublonDossierVP()
        Dim NumControleLigne As Integer
        For NumControleLigne = 0 To VisitesTDDataGridView.Rows.Count - 2
            Dim currentDossier = TDDatabaseDataSet.VisitesTD(NumControleLigne)(
                columnName:="Dossier",
                version:=DataRowVersion.Current).ToString()
            If currentDossier = UCase(DossierVP) Then
                MsgBox("La référence: " & DossierVP & " est déjà utilisée." & vbCrLf & " Entrez un autre N° de Dossier.")
                GoTo SaisieDossterVP
            End If
        Next
SaisieDateVP:
        Try
            EntreDateVP = InputBox("Date de la VP:  ")
            If EntreDateVP = "" Then Exit Sub
            DateVP = Date.Parse(EntreDateVP)
        Catch evp As Exception
            EntreDateVP = InputBox("Erreur de saisie de la date." & vbCrLf & vbCrLf & "Saisissez selon la forme: jj/mm/aaaaa:", MsgBoxStyle.RetryCancel)
            GoTo SaisieDateVP
        End Try
        DateVP = Date.Parse(EntreDateVP)
        'verification doublon date VP
        For NumControleLigne = 0 To VisitesTDDataGridView.Rows.Count - 2
            Dim currentDateVP = TDDatabaseDataSet.VisitesTD(NumControleLigne)(
    columnName:="Date VP", version:=DataRowVersion.Current).ToString()
            If currentDateVP = DateVP Then
                MsgBox("La référence: " & DateVP & " est déjà utilisée." & vbCrLf & " Entrez une autre Date.")
                GoTo SaisieDateVP
            End If
        Next
SaisieHeureVP:
        Try
            HeureEntreVP = InputBox("Heures de la VP: ", MsgBoxStyle.OkCancel)
            If HeureEntreVP = "" Then Exit Sub
        Catch eE As Exception
            Dim msgBoxResult = MsgBox("Erreur de sasie de l'heure de la VP." & vbCrLf & "Saisir sous la forme: nn,nn")
            GoTo SaisieHeureVP
        End Try
        Try
            HeureEntree = Decimal.Parse(HeureEntreVP, NumberStyles.Float)
        Catch EhVp As Exception
            MsgBox("Erreur de saisie du nombre d'heures de la VP" & vbCrLf & vbCrLf & "Sasir sus la forme: nn,nn")
            GoTo SaisieHeureVP
        End Try
        Dim title = "ALERTE"
        ' Now define a style for the message box. In this example, the message box will have Yes and No buttons, the default will be the No button, and a Critical Message icon will be present.
        Dim style = MsgBoxStyle.YesNo Or MsgBoxStyle.DefaultButton2 'Or MsgBoxStyle.Critical
        Select Case HeureEntree
            Case Is < 47
#Disable Warning BC42025 ' Accès d’un membre partagé, d’un membre de constante, d’un membre enum ou d’un type imbriqué via une instance
                Dim response = MsgBox("La valeur entrée est inférieure à la normale." & vbCrLf & "Vous avez saisi: " & HeureEntree & vbCrLf & vbCrLf & "Cliquez sur OUI pour confirmer la VP" & vbCrLf & vbCrLf & "Sur NON pour une visite exceptionnelle.",
                                      style.YesNoCancel, title)
#Enable Warning BC42025 ' Accès d’un membre partagé, d’un membre de constante, d’un membre enum ou d’un type imbriqué via une instance
                ' Take some action based on the response.
                Select Case response
                    Case MsgBoxResult.No
                        '  MsgBox("YES, continue!!", , title)
                        HeuresAllouees = 50 - HeureEntree
                        HeuresVisite.Text = " - "
                        ObsApresVP = InputBox("Observations: ")
                        CalculsVP()
                        Insertdonne()
                        Dim NVp As String = HeuresVisite.Text
                        ' VisiteP = HeuresVisite.Text
                        HeuresVisite.Text = NVp
                        ObsApresVP = " "
                        Exit Sub
                    Case MsgBoxResult.Yes
                        Dim NLigneActive As Integer = VisitesTDDataGridView.Rows.Count - 2
                        If NLigneActive < 0 Then NLigneActive = 0
                        ' Pour obtenir la version actuelle d'un enregistrement
                        Dim currentHVP = TDDatabaseDataSet.VisitesTD(NLigneActive)(
                            columnName:="Heures VP", version:=DataRowVersion.Current).ToString()
                        HeuresAllouees = currentHVP + HeureEntree
                        If HeuresAllouees > 50 Then
                            HeuresAllouees = 50 - (HeuresAllouees - 50)
                        Else HeuresAllouees = 50
                        End If
                    Case MsgBoxResult.Cancel
                        Exit Sub
                End Select
            Case Is > 50
                Beep()
                Dim response = MsgBox("Vous avez dépassé la tolérance 10% de la VP50." & vbCrLf & "Vouz avez saisi: " & HeureEntree & vbCrLf & vbCrLf & "Voulez-vous confirmer la saisie?", style, title)
                ' Take some action based on the response.
                If response.ToString = "" Then Exit Sub
                Select Case response
                    Case MsgBoxResult.Yes
                        '  MsgBox("YES, continue!!", , title)
                        HeuresAllouees = 50 - (HeureEntree - 50)
                    Case MsgBoxResult.No
                        Exit Select
                    Case Else
                        MsgBox("Sasissez à nouveau.", , title)
                End Select
            Case Else
                HeuresAllouees = 50
        End Select
        CalculsVP()
        Insertdonne()
    End Sub
    Private Sub CalculsVP()
        VisiteP = HeuresVisite.Text
        HeuresVP = HeureEntree
        HTCellule += HeureEntree

        If CInt(Format(Strings.Right(HTCellule, 2), "00")) > 59 Then
            HTCellule += 1
            HTCellule -= 0.6
        End If

        ' VP Moteur
        HTMoteur += HeureEntree
        Dim MinutesEntree As Integer = Format(Strings.Right(HTCellule, 2), "00")
        MinutesEntree = Format(Strings.Right(HTMoteur, 2), "00")
        If CInt(Format(Strings.Right(HTCellule, 2), "00")) > 59 Then
            HTMoteur += 1
            HTMoteur -= 0.6
        End If

        'VP Moteur depuis RG
        HMoteurDepuisRG += HeureEntree
        MinutesEntree = Format(Strings.Right(HMoteurDepuisRG, 2), "00")
        If CInt(Format(Strings.Right(HTCellule, 2), "00")) > 59 Then
            HMoteurDepuisRG -= 0.6
            HMoteurDepuisRG += 1
        End If

        ' VP Reste Moteur
        Dim DiffEntreeMoteur, DifferenceResteMot As Integer
        DiffEntreeMoteur = Format(Strings.Right(ResteMoteur, 2), "00")
        ResteMoteur -= HeureEntree
        DifferenceResteMot = Strings.Right(ResteMoteur, 2)
        If DifferenceResteMot > 59 Then 'Or DiferenceResteMot < 0 Then
            DifferenceResteMot = 60 - (Strings.Right(HeureEntree, 2) - DiffEntreeMoteur)
        End If
        ResteMoteur = $"{Int(ResteMoteur) },{DifferenceResteMot }"

        'VP Hélice
        HTHelice += HeureEntree
        MinutesEntree = Format(Strings.Right(HTHelice, 2), "00")
        If CInt(Format(Strings.Right(HTCellule, 2), "00")) > 59 Then
            HTHelice += 1
            HTHelice -= 0.6
        End If

        ' VP Hélice Depuis RG
        HHeliceDepuisRG += HeureEntree
        MinutesEntree = Format(Strings.Right(HHeliceDepuisRG, 2), "00")
        If CInt(Format(Strings.Right(HTCellule, 2), "00")) > 59 Then
            HHeliceDepuisRG += 1
            HHeliceDepuisRG -= 0.6
        End If

        'VP Reste Hélice
        Dim DifferenceEntreeHelice, DifferenceResteHelice As Integer
        DifferenceEntreeHelice = Strings.Right(ResteHelice, 2) 'Format(Strings.Right(ResteHelice, 2), "00")
        ResteHelice -= HeureEntree
        DifferenceResteHelice = Strings.Right(ResteHelice, 2) 'Format(Strings.Right(ResteHelice, 2), "00")
        If DifferenceResteHelice > 59 Then
            DifferenceResteHelice = 60 - (Strings.Right(HeureEntree, 2) - DifferenceEntreeHelice)
        End If
        ResteHelice = $"{Int(ResteHelice) },{DifferenceResteHelice }"
    End Sub
    ' Insertion des données
    Private Sub Insertdonne()
        newVisitesTDDossier = UCase(DossierVP)
        newVisitesTDDate_VP = FormatDateTime(DateVP.ToShortDateString)
        newVisitesTDVisite_Programmée = VisiteP
        newVisitesTDHeures_VP = HeureEntree
        newVisitesTD_Cellule = HTCellule
        newVisitesTDHT_Moteur = HTMoteur
        newVisitesTDMoteur_Depuis_RG = HMoteurDepuisRG
        newVisitesTDReste_Moteur = ResteMoteur
        newVisitesTDHT_Hélice = HTHelice
        newVisitesTDHélice_Depuis_RG = HHeliceDepuisRG
        newVisitesTDReste_Hélice = ResteHelice
        newVisitesTDHeures_Allouees = HeuresAllouees
        newVisitesTDObservations = ObsApresVP
        Try
            VisitesTDTableAdapter.Insert(newVisitesTDDossier, newVisitesTDDate_VP, newVisitesTDVisite_Programmée, newVisitesTDHeures_VP, newVisitesTD_Cellule, newVisitesTDHT_Moteur, newVisitesTDMoteur_Depuis_RG,
             newVisitesTDReste_Moteur, newVisitesTDHT_Hélice, newVisitesTDHélice_Depuis_RG, newVisitesTDReste_Hélice, newVisitesTDHeures_Allouees, newVisitesTDObservations)
            HeuresCellule.Text = $"{HTCellule,8:F2}"
            HeuresMoteur.Text = $"{HTMoteur,8:F2}"
            MoteurRG.Text = $"{HMoteurDepuisRG,8:F2}"
            HeuresHelice.Text = $"{HTHelice,8:F2}"
            HeliceRG.Text = $"{HHeliceDepuisRG,8:F2}"
        Catch ex2 As Exception
            MessageBox.Show("Echec de l'Insertion.")
        End Try
        RefreshTDDatabaseDataset()
        AnalyseDate()
        MiseAjourListBc()
        HeuresVisite.Text = If(HeuresVisite.Text = "100H", "50H", "100H")
        '    ButePotentiel()
    End Sub
    Private Sub FindErrors()
        If TDDatabaseDataSet.HasErrors Then
            Dim table As DataTable
            For Each table In TDDatabaseDataSet.Tables
                If table.HasErrors Then
                    Dim row As DataRow
                    For Each row In table.Rows
                        If row.HasErrors Then
                            ' Process error here.
                        End If
                    Next
                End If
            Next
        End If
    End Sub
    Private Sub BtnInserLigne_Click(sender As Object, e As EventArgs) Handles BtnInsertLigne.Click
        Dim LigneInsert As Integer = DataGridView1.CurrentCell.RowIndex
        DataGridView1.Rows.Insert(LigneInsert + 1)
    End Sub
    Private Sub BtnSauveKardex_Click(sender As Object, e As EventArgs) Handles BtnSauveKardex.Click
        Enregistre_GridView()
        Presentation()
        BtnCorrectionLigneKardex.Visible = False
        BtnCorrectionLigneKardex.Enabled = False
        BtnInsertLigne.Visible = False
        BtnInsertLigne.Enabled = False
        BtnSauveKardex.Visible = False
        BtnSauveKardex.Enabled = False
        BtnCorrectionKardex.Visible = True
        BtnCorrectionKardex.Enabled = True
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim ListViewItem13 As ListViewItem = New ListViewItem("")
        Dim ListViewItem14 As ListViewItem = New ListViewItem(New String() {"Cellule", "PIPER", "PA 28-181", "28 78 90138", "", "", ""}, -1)
        '   Dim ListViewItem15 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem(New String() {"Moteur", "LYCOMING", "O-360-A4M", "L-306777-36AC", "", "", ""}, -1)
        '  Dim ListViewItem16 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem(New String() {"Hélice", "SENSENICH", "76EMS85-0-82", "100294K", "", "", ""}, -1)
        'DurePeriodicite()
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ButePotentiel()
    End Sub
    Private Sub BtnCorrectionKardex_Click(sender As Object, e As EventArgs) Handles BtnCorrectionKardex.Click
        BtnCorrectionLigneKardex.Visible = True
        BtnCorrectionLigneKardex.Enabled = True
        BtnInsertLigne.Visible = True
        BtnInsertLigne.Enabled = True
        BtnSauveKardex.Visible = True
        BtnSauveKardex.Enabled = True
        BtnCorrectionKardex.Enabled = False
        BtnCorrectionKardex.Visible = False
    End Sub
    Private Sub BtnCorrectionLigneKardex_Click(sender As Object, e As EventArgs) Handles BtnCorrectionLigneKardex.Click
        'Correction ligne
        Dim ColonneCorrection As Integer
        Dim LigneCorrection As Integer = DataGridView1.CurrentCell.RowIndex
        Select Case DataGridView1(0, LigneCorrection).Value
            Case Is <> ""
                For ColonneCorrection = 0 To DataGridView1.ColumnCount - 1
                    DataGridView1.CurrentCell = DataGridView1(ColonneCorrection, LigneCorrection)
                    DataGridView1.CurrentCell.ReadOnly = False
                    DataGridView1.CurrentCell.Style.BackColor = Color.MistyRose
                Next
            Case Else
        End Select
    End Sub
    Private Sub ChargeCN()
        DataGridView3.Rows.Clear()
        'Définition du TextFieldParser
        Using MyReader As New FileIO.TextFieldParser(MyCN & ".txt")
            ' Définition du type de champs et du délimiteur
            MyReader.TextFieldType = FileIO.FieldType.Delimited
            MyReader.SetDelimiters("_")
            ' Lecture données et affichage
            Dim Ligne As String()
            Dim i As Integer
            Dim S(0 To 9) As String
            While Not MyReader.EndOfData
                Try
                    Ligne = MyReader.ReadFields()
                    i = 0
                    Dim champ As String
                    For Each champ In Ligne
                        i += 1
                        S(i) = champ
                    Next
                    DataGridView3.Rows.Add(New String() {S(1), S(2), S(3), S(4), S(5), S(6), S(7), S(8), S(9)})
                Catch ex3 As FileIO.MalformedLineException
                    MsgBox("L'enregistrement " & ex3.Message & "n'est pas valide. Il a été ignoré.")
                End Try
            End While
        End Using
        ProtectionCN()
    End Sub
    Private Sub BtnModifCN_Click(sender As Object, e As EventArgs) Handles BtnModifCN.Click
        '     'Correction cellule
        Dim ColonneCorrectionCN As Integer = DataGridView3.CurrentCell.ColumnIndex
        Dim LigneCorrectionCN As Integer = DataGridView3.CurrentCell.RowIndex
        DataGridView3.CurrentCell = DataGridView3(ColonneCorrectionCN, LigneCorrectionCN)
        DataGridView3.CurrentCell.Style.BackColor = Color.MistyRose
        DataGridView3.CurrentCell.ReadOnly = False
        ' Next
    End Sub
    Private Sub ProtectionCN()
        Dim LigneProtectionCN As Integer = DataGridView3.Rows.Count - 2
        Dim ProCN As Integer
        For ProCN = 0 To LigneProtectionCN
            DataGridView3.Rows(ProCN).ReadOnly = True
        Next
    End Sub
    Private Sub BtnSaveCN_Click(sender As Object, e As EventArgs) Handles BtnSaveCN.Click
        SauvegardeCN()
    End Sub
    ' Sauvegarde les CN
    Public Sub SauvegardeCN()
        Dim sCN As String, LigCN As Integer
        With DataGridView3
            If .Rows.Count > 0 Then
                Using sw As New IO.StreamWriter(MyCN & ".txt")
                    For LigCN = 0 To .Rows.Count - 2
                        If Not .Rows(LigCN).Cells(0).Value Is Nothing Then
                            sCN = .Rows(LigCN).Cells(0).Value & "_" &
                                .Rows(LigCN).Cells(1).Value & "_" &
                                .Rows(LigCN).Cells(2).Value & "_" &
                                .Rows(LigCN).Cells(3).Value & "_" &
                                .Rows(LigCN).Cells(4).Value & "_" &
                                .Rows(LigCN).Cells(5).Value & "_" &
                                .Rows(LigCN).Cells(6).Value & "_" &
                                .Rows(LigCN).Cells(7).Value & "_" &
                                .Rows(LigCN).Cells(8).Value '& "_" & _
                            '.Rows(LigCN).Cells(9).Value '& "_" & _
                            '.Rows(LigCN).Cells(10).Value & "_" & _
                            '.Rows(LigCN).Cells(11).Value
                            sw.WriteLine(sCN)
                        End If
                    Next
                End Using
            End If
        End With
        ProtectionCN()
        MsgBox("Sauvegarde " & MyCN & " terminé.")
    End Sub
    Private Sub ListBox3_DoubleClick(sender As Object, e As EventArgs) Handles ListBox3.DoubleClick
        Dim MonDGV As DataGridView = DataGridView3
        MyCN = ListBox3.SelectedItem
        Label1.Text = "Date: " & DateTimePicker2.Value.ToShortDateString
        'Label12.Text = " F-GATD  PA 28-181 ARCHER II N° 28-78 90138.  Fiche de suivi  " & ListBox3.SelectedItem
        Label11.Text = "HT Cellule: " & HTCellule
        Label13.Text = "Moteur depuis RG: " & String.Format("{0:F2}", HMoteurDepuisRG)
        Label14.Text = "Hélice depuis RG: " & HHeliceDepuisRG
        ChargeCN()
        Dim Ltest As String
        Dim ligRep As Integer
        For ligRep = 0 To DataGridView3.Rows.Count - 2
            DataGridView3.CurrentCell = DataGridView3(7, ligRep)
            Ltest = DataGridView3.CurrentCell.Value.ToString
            If Microsoft.VisualBasic.Left(Ltest, 3) = "OUI" Then
                DataGridView3.CurrentCell.Style.ForeColor = Color.Red
                DataGridView3(8, ligRep).Style.BackColor = Color.Yellow
                DataGridView3(8, ligRep).Style.ForeColor = Color.Red
                DataGridView3(0, ligRep).Style.BackColor = Color.Yellow
                DataGridView3(0, ligRep).Style.ForeColor = Color.Red
            End If
        Next
    End Sub
    ' Gestion KARDEX
    ' Sauvegarde Kardex
    Public Sub Enregistre_GridView() '(ByRef fName As String, ByRef Grid As DataGridView)
        Dim s As String, Lig As Integer
        With DataGridView1
            If .Rows.Count > 0 Then
                Using sw As New IO.StreamWriter("SauveDGV1-CEN.txt")
                    For Lig = 0 To .Rows.Count - 2
                        '  If Not .Rows(Lig).Cells(0).Value Is Nothing Then
                        s = .Rows(Lig).Cells(0).Value & "_" &
                            .Rows(Lig).Cells(1).Value & "_" &
                            .Rows(Lig).Cells(2).Value & "_" &
                            .Rows(Lig).Cells(3).Value & "_" &
                            .Rows(Lig).Cells(4).Value & "_" &
                            .Rows(Lig).Cells(5).Value & "_" &
                            .Rows(Lig).Cells(6).Value & "_" &
                            .Rows(Lig).Cells(7).Value & "_" &
                            .Rows(Lig).Cells(8).Value & "_" &
                            .Rows(Lig).Cells(9).Value & "_" &
                            .Rows(Lig).Cells(10).Value & "_" &
                            .Rows(Lig).Cells(11).Value & "_" &
                        .Rows(Lig).Cells(12).Value '& "_" & _
                        ' .Rows(Lig).Cells(13).Value
                        sw.WriteLine(s)
                        '  End If
                    Next
                End Using
            End If
        End With
        MsgBox("Sauvegarde du Kardex terminé.")
    End Sub
    Private Sub Charger()
        'Définition du TextFieldParser
        Using MyReader As New FileIO.TextFieldParser("SauveDGV1-CEN.txt")
            ' Définition du type de champs et du délimiteur
            MyReader.TextFieldType = FileIO.FieldType.Delimited
            MyReader.SetDelimiters("_")
            ' Lecture données et affichage
            Dim Ligne As String()
            Dim i As Integer
            Dim S(0 To 13) As String
            While Not MyReader.EndOfData
                Try
                    Ligne = MyReader.ReadFields()
                    i = 0
                    Dim champ As String
                    For Each champ In Ligne
                        i += 1
                        S(i) = champ
                    Next
                    DataGridView1.Rows.Add(New String() {S(1), S(2), S(3), S(4), S(5), S(6), S(7), S(8), S(9), S(10), S(11), S(12), S(13)})
                Catch ex4 As FileIO.MalformedLineException
                    MsgBox("L'enregistrement " & ex4.Message & "n'est pas valide. Il a été ignoré.")
                End Try
            End While
        End Using
    End Sub
    Private Sub Presentation()
        Dim LignePres, ColonnePres As Integer
        For LignePres = 0 To DataGridView1.Rows.Count - 2
            Dim row As DataGridViewRow = DataGridView1.Rows(LignePres)
            Select Case row.Cells(0).Value
                Case Is = ""
                    DataGridView1.CurrentCell = DataGridView1(1, LignePres)
                    DataGridView1.CurrentCell.Style.ForeColor = Color.Blue


                    For ColonnePres = 0 To DataGridView1.ColumnCount - 1
                        DataGridView1.CurrentCell = DataGridView1(ColonnePres, LignePres)
                        DataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SunkenHorizontal
                        DataGridView1.CurrentCell.ReadOnly = True
                    Next
                Case Else
                    For ColonnePres = 0 To DataGridView1.ColumnCount - 1
                        DataGridView1.CurrentCell = DataGridView1(ColonnePres, LignePres)
                        Select Case DataGridView1(ColonnePres, LignePres).Value
                            Case Is = ""
                                DataGridView1.CurrentCell.Style.BackColor = Color.LightGray
                            Case Else
                                DataGridView1.CurrentCell.Style.BackColor = Color.White
                        End Select
                        DataGridView1.CurrentCell.ReadOnly = True
                        DataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single
                    Next ColonnePres
            End Select
            DataGridView1.CurrentCell = DataGridView1(0, LignePres)
        Next LignePres
    End Sub
    Private Sub DurePeriodicite()
        Dim LigneSaisieDP As Integer = DataGridView1.CurrentCell.RowIndex
        Dim ColSaisieDP As Integer = DataGridView1.CurrentCell.ColumnIndex
        DataGridView1.CurrentCell = DataGridView1(ColSaisieDP + 2, LigneSaisieDP)
        Dim MonthsValue As Integer
        Dim Dperiodicite As String
        Dperiodicite = DataGridView1.CurrentCell.Value.ToString
        Dim NbMois As Integer = Val(Dperiodicite)
        'Try
        Select Case NbMois
            Case Is = 30
                MonthsValue = 1
            Case Is = 60
                MonthsValue = 2
            Case Is = 90
                MonthsValue = 3
            Case Is = 120
                MonthsValue = 4
            Case Is = 12
                MonthsValue = 12
            Case Is = 24
                MonthsValue = 24
            Case Else
                MonthsValue = 0
        End Select
        DataGridView1.CurrentCell = DataGridView1(ColSaisieDP, LigneSaisieDP)
    End Sub
    Private Sub AnalyseDate()

        Dim CptDate As Integer
        Dim MajATA As String
        ListBoxAta.Visible = True
        ListBoxAta.Items.Clear()
        RichTextBonDeCommande.Clear()
        ' ListView2.Items(1).SubItems.Add("essai")
        Select Case HeuresVisite.Text
            Case = " - "
                RichTextBonDeCommande.SelectedText = "  " & ObsApresVP & vbCrLf & vbCrLf
            Case Else
                RichTextBonDeCommande.SelectedText = "  VP " & HeuresVisite.Text & vbCrLf & vbCrLf
        End Select
        For CptDate = 1 To DataGridView1.Rows.Count - 2
            DataGridView1.CurrentCell = DataGridView1(9, CptDate) 'DataGridView1(DataGridView1.CurrentCell.ColumnIndex, DataGridView1.CurrentCell.RowIndex)
            Select Case DataGridView1.CurrentCell.Value.ToString
                Case Is <> ""
                    Dim MaDate1 As Date = DataGridView1.CurrentCell.Value.ToString
                    Dim Madate2 As Date = DateTimePicker2.Value.ToShortDateString
                    If MaDate1 <= Madate2 Then
                        DataGridView1.CurrentCell.Style.ForeColor = Color.Red
                        DataGridView1.CurrentCell.Style.BackColor = Color.Yellow
                        DataGridView1.CurrentCell = DataGridView1(0, DataGridView1.CurrentCell.RowIndex)
                        MajATA = DataGridView1.CurrentCell.Value.ToString &
                            ": " & DataGridView1(DataGridView1.CurrentCell.ColumnIndex + 1, DataGridView1.CurrentCell.RowIndex).Value.ToString _
                        & ": " & DataGridView1(DataGridView1.CurrentCell.ColumnIndex + 11, DataGridView1.CurrentCell.RowIndex).Value.ToString
                        DataGridView1.CurrentCell.Style.ForeColor = Color.Red
                        '  RichTextBox1.SelectionColor = Color.Red
                        ListBoxAta.Items.Add(" Mettre à jour ATA " & MajATA)
                        '  TextBox2.AppendText("- ATA: " & MajATA & vbNewLine)
                        '  RichTextBox1.SelectionFont = New Font("Times New Roman", 12, FontStyle.Underline)
                        ' RichTextBox1.SelectionColor = Color.Blue
                        '     RichTextBox1.SelectedText = "Travaux supplémentaires" + ControlChars.NewLine
                        '    RichTextBox1.SelectionFont = New Font("Arial", 10, FontStyle.Regular)
                        'RichTextBox1.SelectionColor = Color.Black
                        RichTextBonDeCommande.SelectedText = "- ATA: " & MajATA & vbNewLine
                    End If
            End Select
        Next
    End Sub
    Private Sub BtnQuitter_Click(sender As Object, e As EventArgs) Handles BtnQuitter.Click
        End
    End Sub
    Private Sub ButePotentiel()
        RichTextBoxButeePotentiel.Clear()
        RichTextBoxButeePotentiel.SelectedText = " " & vbCrLf
        RichTextBoxButeePotentiel.SelectedText = " ALERTES BUTEE POTENTIEL" & vbCrLf & vbCrLf
        Dim CptButePot As Integer
        Dim Bute1, Bute2 As Decimal
        Dim MajButePot As String
        '  ListBox1.Visible = True
        ' ListBox1.Items.Clear()
        For CptButePot = 1 To DataGridView1.Rows.Count - 2
            DataGridView1.CurrentCell = DataGridView1(10, CptButePot) 'DataGridView1(DataGridView1.CurrentCell.ColumnIndex, DataGridView1.CurrentCell.RowIndex)
            Select Case DataGridView1.CurrentCell.Value
                Case Is <> ""
                    Bute1 = DataGridView1.CurrentCell.Value
                    Bute2 = HTCellule
                    If Bute1 <= Bute2 Then
                        DataGridView1.CurrentCell.Style.ForeColor = Color.Red
                        DataGridView1.CurrentCell.Style.BackColor = Color.Yellow
                        DataGridView1.CurrentCell = DataGridView1(0, DataGridView1.CurrentCell.RowIndex)
                        MajButePot = DataGridView1.CurrentCell.Value.ToString &
                            ": " & DataGridView1(DataGridView1.CurrentCell.ColumnIndex + 1, DataGridView1.CurrentCell.RowIndex).Value.ToString _
                        & ": " & DataGridView1(DataGridView1.CurrentCell.ColumnIndex + 11, DataGridView1.CurrentCell.RowIndex).Value.ToString
                        DataGridView1.CurrentCell.Style.ForeColor = Color.Red
                        RichTextBoxButeePotentiel.SelectedText = "Mettre à jour Butée Potentiel: " & Bute1 & " de ATA " & MajButePot & vbCrLf
                        '  TextBox2.AppendText("- ATA: " & MajATA & vbNewLine)
                        '  RichTextBox1.SelectionFont = New Font("Times New Roman", 12, FontStyle.Underline)
                        ' RichTextBox1.SelectionColor = Color.Blue
                        '     RichTextBox1.SelectedText = "Travaux supplémentaires" + ControlChars.NewLine
                        '    RichTextBox1.SelectionFont = New Font("Arial", 10, FontStyle.Regular)
                        'RichTextBox1.SelectionColor = Color.Black
                        'RichTextBoxButeePotentiel.SelectedText = "- Mettre à jour butée potentiel: " & MajButePot & vbNewLine
                    End If
            End Select
        Next
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        AnalyseDate()
    End Sub

    Private Sub DateTimePicker1_LostFocus(sender As Object, e As EventArgs) Handles DateTimePicker1.LostFocus
        DateTimePicker1.Visible = False
        DateTimePicker1.Enabled = False
    End Sub
    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        'Sélection date à changer
        DateTimePicker1.Visible = True
        DateTimePicker1.Enabled = True
        '  DateTimePicker1.DropDownAlign = True
    End Sub
    Private Sub DateTimePicker1_closeup(ByVal sender As Object, e As EventArgs) Handles DateTimePicker1.CloseUp
        'ligneDate = DataGridView1.CurrentCell.RowIndex
        'colDate = DataGridView1.CurrentCell.ColumnIndex
        DataGridView1.CurrentCell = DataGridView1(DataGridView1.CurrentCell.ColumnIndex, DataGridView1.CurrentCell.RowIndex)
        DataGridView1.CurrentCell.Value = DateTimePicker1.Value.ToShortDateString
        ' EntreDate()
        '  DateTimePicker1.Enabled = False
        ' DateTimePicker1.Value = Date.Now
    End Sub
    Private Sub MiseAjourListBc()
        ListBox2.Items.RemoveAt(5)
        ListBox2.Items.Add("DATE: " & DateTimePicker2.Value.ToShortDateString)
        With ListView2.Items(1)
            .SubItems(4).Text = Format(HTCellule, "####.0,")
            .SubItems(5).Text = "S/O"
            .SubItems(6).Text = "S/O"
        End With
        With ListView2.Items(2)
            .SubItems(4).Text = String.Format("{0,8:F2}", HTMoteur)
            .SubItems(5).Text = String.Format("{0,8:F2}", HMoteurDepuisRG)
            .SubItems(6).Text = String.Format("{0,8:F2}", ResteMoteur)
        End With
        With ListView2.Items(3)
            .SubItems(4).Text = String.Format("{0,8:F2}", HTHelice)
            .SubItems(5).Text = String.Format("{0,8:F2}", HHeliceDepuisRG)
            .SubItems(6).Text = String.Format("{0,8:F2}", ResteHelice)
        End With
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        DateTimePicker3.Enabled = True
        DateTimePicker3.Visible = True
    End Sub
    Private Sub DateTimePicker3_CloseUp(sender As Object, e As EventArgs) Handles DateTimePicker3.CloseUp
        Label2.Text = DateTimePicker3.Value.ToShortDateString
    End Sub
    Private Sub DateTimePicker3_LostFocus(sender As Object, e As EventArgs) Handles DateTimePicker3.LostFocus
        DateTimePicker3.Enabled = False
        DateTimePicker3.Visible = False
    End Sub
    Private Sub VisitesTDDataGridView_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs) Handles VisitesTDDataGridView.CellMouseDown
        'Dim fileReader As String
        If e.Button = MouseButtons.Right Then  ' Windows.Forms.MouseButtons.Right Then
            'Dim ColonneCorrectionDT As Integer = VisitesTDDataGridView.CurrentCell.ColumnIndex
            Dim LigneCorrectionDT As Integer = VisitesTDDataGridView.CurrentCell.RowIndex
            VisitesTDDataGridView.CurrentCell = VisitesTDDataGridView(0, LigneCorrectionDT)
            MonDossierDeTravail = VisitesTDDataGridView.CurrentCell.Value.ToString
            Try
                Process.Start("C:\Users\ULRIC\Documents\VisitesTD/" & MonDossierDeTravail & ".pdf")
                ' fileReader = My.Computer.FileSystem.ReadAllText("C:\Users\ULRIC\Documents\VisitesTD/" & MonDossierDeTravail & ".pdf") ', System.Text.Encoding.ASCII)
                'MsgBox(fileReader)
                '  Catch ex As FileIO.MalformedLineException
                'Try
                ' My.Computer.FileSystem.OpenTextFileReader("C:\Users\ULRIC\Documents\VisitesTD/" & MonDossierDeTravail & ".pdf")
            Catch ex7 As Exception ' System.IO.IOException
                MsgBox("Le DT ou OE " & MonDossierDeTravail & " n'existe pas.")
                ' Exit Sub
            End Try
            MonDossierDeTravail = " "
        End If
    End Sub
End Class

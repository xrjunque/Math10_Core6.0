Imports System.Globalization
Imports System.IO
Imports System.Windows
Imports Math10_Core3._1.Rational
Imports System.Text.RegularExpressions

Public Class Options
    Dim ctrl As New Controls.ComboBox
    Private Sub Options_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Try
            PopulateGrid()
        Catch ex As Exception

        End Try
    End Sub
    Sub PopulateGrid()
        Try
            'Grid1.ShowGridLines = True

            Dim sText As String = "Culture (&culture)|Rounding (&decNN)|Numeric base (&d,&h,&o,&b)|Angle mode (&rad,&deg,&grad)|Fractions (&fra0,&fra1)|Detail (&detail0,&detail1|Variables length (&var0,&var1)|MathJax"
            Dim vText() As String = Split(sText, "|")
            Dim ctrlHeight As Double = 23.0
            For i = 0 To vText.Length - 1
                Dim rowDef As New Controls.RowDefinition
                rowDef.Height = New GridLength(ctrlHeight + 3, GridUnitType.Star)
                Grid1.RowDefinitions.Add(rowDef)
                Dim lbl As New Controls.Label()
                lbl.Content = vText(i) + ":"
                lbl.Margin = New Thickness(0, 0, 0, 0)
                lbl.Height = ctrlHeight
                lbl.HorizontalAlignment = HorizontalAlignment.Center
                lbl.VerticalAlignment = VerticalAlignment.Bottom
                Grid1.Children.Add(lbl)
                Grid1.Margin = New Thickness(0, 0, 0, 0)
                Controls.Grid.SetColumn(lbl, 0)
                Controls.Grid.SetRow(lbl, i)

                ctrl = New Controls.ComboBox
                Select Case i
                    Case 0
                        ctrl.Height = ctrlHeight
                        ctrl.Name = "CI"
                        populateCultureInfo(ctrl)
                    Case 1
                        Dim vDec(15) As String
                        vDec(0) = "No Rounding (&dec15)"
                        For j As Int32 = 1 To 15
                            If j <> 14 Then
                                vDec(j) = (15 - j).ToString + " decimals (&dec" + (15 - j).ToString + ")"
                            Else
                                vDec(j) = (15 - j).ToString + " decimal (&dec" + (15 - j).ToString + ")"
                            End If
                        Next
                        ctrl.Name = "dec"
                        ctrl.Height = ctrlHeight
                        For j As Int32 = 0 To vDec.Length - 1
                            ctrl.Items.Add(vDec(j))
                        Next
                        If G10.nDec = 15 Then
                            ctrl.SelectedIndex = 0
                        Else
                            ctrl.SelectedIndex = 15 - G10.nDec
                        End If
                    Case 2
                        Dim sBase As String = "Decimal (&d)|Hexadecimal (&h)|Octal (&o)|Binary (&b)"
                        Dim vBase() As String = Split(sBase, "|")
                        ctrl.Name = "base"
                        ctrl.Height = ctrlHeight
                        For j As Int32 = 0 To vBase.Length - 1
                            ctrl.Items.Add(vBase(j))
                        Next
                        Dim index As Int32 = 0
                        Select Case G10.currBase
                            Case Rational.Base.Decimal : index = 0
                            Case Rational.Base.Hexadecimal : index = 1
                            Case Rational.Base.Octal : index = 2
                            Case Rational.Base.Binary : index = 3
                        End Select
                        ctrl.SelectedIndex = index
                    Case 3
                        Dim sAngle As String = "radians (&rad)|degree (&deg)|gradian (&grad)"
                        Dim vAngle() As String = Split(sAngle, "|")
                        ctrl.Name = "angle"
                        ctrl.Height = ctrlHeight
                        For j As Int32 = 0 To vAngle.Length - 1
                            ctrl.Items.Add(vAngle(j))
                        Next
                        Dim index As Int32 = 0
                        Select Case G10.angleMode
                            Case G10.AngleModes.radians : index = 0
                            Case G10.AngleModes.degree : index = 1
                            Case G10.AngleModes.gradian : index = 2
                        End Select
                        ctrl.SelectedIndex = index
                    Case 4
                        Dim s As String = "No (&var0)|Yes (&var1)"
                        Dim vs() As String = Split(s, "|")
                        ctrl.Name = "frac"
                        ctrl.Height = ctrlHeight
                        For j As Int32 = 0 To vs.Length - 1
                            ctrl.Items.Add(vs(j))
                        Next
                        Dim index As Int32 = IIf(G10.frac, 1, 0)
                        ctrl.SelectedIndex = index
                    Case 5
                        Dim s As String = "No (&detail0)|Yes (&detail1)"
                        Dim vs() As String = Split(s, "|")
                        ctrl.Height = ctrlHeight
                        ctrl.Name = "detail"
                        For j As Int32 = 0 To vs.Length - 1
                            ctrl.Items.Add(vs(j))
                        Next
                        Dim index As Int32 = IIf(G10.detail, 1, 0)
                        ctrl.SelectedIndex = index
                    Case 6
                        Dim s As String = "One character (&var0)|One or more chars.& numbers (&var1)"
                        Dim vs() As String = Split(s, "|")
                        ctrl.Name = "var"
                        ctrl.Height = ctrlHeight
                        For j As Int32 = 0 To vs.Length - 1
                            ctrl.Items.Add(vs(j))
                        Next
                        Dim index As Int32 = IIf(G10.var, 1, 0)
                        ctrl.selectedIndex = index
                    Case 7
                        Dim s As String = "No (&mathml0)|Yes (&mathml1)"
                        Dim vs() As String = Split(s, "|")
                        ctrl.Height = ctrlHeight
                        ctrl.Name = "mathml"
                        For j As Int32 = 0 To vs.Length - 1
                            ctrl.Items.Add(vs(j))
                        Next
                        Dim index As Int32 = IIf(G10.detail, 1, 0)
                        ctrl.SelectedIndex = index
                End Select
                ctrl.VerticalAlignment = VerticalAlignment.Bottom
                AddHandler ctrl.SelectionChanged, AddressOf SelectionChanged
                Grid1.Children.Add(ctrl)
                Controls.Grid.SetColumn(ctrl, 1)
                Controls.Grid.SetRow(ctrl, i)
            Next
        Catch ex As Exception

        End Try
    End Sub
    Sub populateCultureInfo(ByRef ctrl As Controls.ComboBox)
        Dim info As New FileStream(IO.Directory.GetCurrentDirectory + "\CultureInfo.txt", FileMode.Open, FileAccess.Read)
        Dim sr As New StreamReader(info)
        Dim vExamples() As String = Split(sr.ReadToEnd(), vbCrLf)
        sr.Close()
        Dim lst As New List(Of String)
        lst.AddRange(vExamples)
        lst.Sort()
        ctrl.ItemsSource = lst
        Dim sCI As String = G10.CI.ToString
        For i = 0 To vExamples.Length - 1
            If InStr(vExamples(i), sCI) Then
                ctrl.SelectedItem = vExamples(i)
                Exit For
            End If
        Next
    End Sub
    Public Sub SelectionChanged(sender As Object, e As Controls.SelectionChangedEventArgs)
        Try
            Dim lst As New List(Of Object)
            lst.AddRange(e.AddedItems)
            If lst.Count Then
                Dim Name As String = e.Source.Name
                Dim value As String = CType(lst(0), String)
                Select Case Name
                    Case "CI" : G10.CI = New CultureInfo(Split(value, vbTab)(1))
                    Case "dec" : G10.nDec = Int32.Parse(Split(value, "&dec")(1).Replace(")", ""))
                    Case "base"
                        If InStr(value, "&d") Then G10.currBase = Base.Decimal
                        If InStr(value, "&h") Then G10.currBase = Base.Hexadecimal
                        If InStr(value, "&o") Then G10.currBase = Base.Octal
                        If InStr(value, "&b") Then G10.currBase = Base.Binary
                    Case "angle"
                        If InStr(value, "&rad") Then G10.angleMode = G10.AngleModes.radians
                        If InStr(value, "&deg") Then G10.angleMode = G10.AngleModes.degree
                        If InStr(value, "&grad") Then G10.angleMode = G10.AngleModes.gradian
                    Case "frac"
                        If InStr(LCase(value), "yes") Then G10.frac = True
                        If InStr(LCase(value), "no") Then G10.frac = False
                    Case "detail"
                        If InStr(LCase(value), "yes") Then G10.detail = True
                        If InStr(LCase(value), "no") Then G10.detail = False
                    Case "var"
                        If InStr(LCase(value), "var1") Then G10.var = True
                        If InStr(LCase(value), "var0") Then G10.var = False
                        G10.Initialize()
                    Case "mathml"
                        If InStr(LCase(value), "yes") Then G10.mathml = True
                        If InStr(LCase(value), "no") Then G10.mathml = False
                End Select
            End If
        Catch ex As Exception

        End Try
    End Sub
End Class

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports mshtml
Imports System.Reflection
Imports System.DirectoryServices.ActiveDirectory
Imports System.Reflection.Emit
Imports System.Buffers
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Forms
Imports System.Windows.Documents
Imports System.Windows.Media
Imports System.Drawing.Font
Imports System.Windows.Input
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Microsoft.VisualBasic
Imports System.Windows.Media.Animation
Imports Microsoft.Web.WebView2.Core
Imports System.Drawing

Class MainWindow

#Region "Dims"
    Dim curFileName As String = ""
    Dim n As Int32 = 1
    Public WithEvents wb As C_WebBrowser
    Dim factor As Single = 1.0
    Dim iCurGo2, lastGo2 As Int32
    Dim vsResultGo2HTML(-1), vsResultGo3(-1) As String
    'Dim infoHelp As System.Windows.Resources.StreamResourceInfo
    Dim filepathCurrOptions As String = Microsoft.VisualBasic.CurDir + "\config.txt"
    Dim us As New Globalization.CultureInfo("en-US")
    Dim sQuery As String
    Dim t1 As Long
    Dim sClr As String = "blue"
    Dim opt As New Options
    Dim sOptions As String = ""

#End Region

#Region "Options"

    Private Sub Open_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        'Me.OpenFile(False)
    End Sub
    Private Sub OpenHTML_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        OpenHTMLFile(False)
        'wb.ZoomWebBrowser(factor)
    End Sub
    Private Sub AppendFile_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        'Me.OpenFile(True)
    End Sub
    Private Sub SaveHTML_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        SaveHTML()
    End Sub
    Private Sub RestoreDefault_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        Try
            setDefaultOptions()
            Dim e1 As String = toStrOptions()
            Dim fs As New FileStream(filepathCurrOptions, FileMode.Create)
            Dim sw As New StreamWriter(fs)
            sw.Write(e1)
            sw.Close()
            fs.Close()
            wb.ZoomWebBrowser(factor)
            updateStatusBar()

        Catch ex As Exception

        End Try
    End Sub
    Private Sub SaveCurrent_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        Try

            Dim e1 As String = toStrOptions()
            Dim fs As New FileStream(filepathCurrOptions, FileMode.Create)
            Dim sw As New StreamWriter(fs)
            sw.Write(e1)
            sw.Close()
            fs.Close()
        Catch ex As Exception
        End Try

    End Sub

    Private Sub GoFn_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
    End Sub

    Function toStrOptions() As String
        Dim e1() As String = Nothing
        Try
            Dim sCulture As String = get_Culture_Full_String()
            e1 = New String() {
                factor.ToString(us),
                CInt(Me.WindowState).ToString,
                Me.Left.ToString,
                Me.Top.ToString,
                Me.Width.ToString,
                Me.Height.ToString,
                rtbQuery.FontFamily.ToString,
                rtbQuery.FontSize.ToString,
                IIf(rtbQuery.FontWeight = FontWeights.Bold, True, False).ToString,
                IIf(rtbQuery.FontStyle = FontStyles.Italic, True, False).ToString,
                sClr,
                sCulture,
                G10.nDec.ToString,
                G10.currBase.ToString,
                G10.angleMode.ToString,
                G10.frac.ToString,
                G10.detail.ToString,
                G10.var.ToString,
                G10.mathml.ToString,
                G10.sImg
                }
        Catch ex As Exception
        End Try
        Return Join(e1, "|")
    End Function
    Function get_Culture_Full_String() As String
        Dim s As String = ""
        Try
            Dim info As New FileStream(IO.Directory.GetCurrentDirectory + "\CultureInfo.txt", FileMode.Open, FileAccess.Read)
            Dim sr As New StreamReader(info)
            Dim vExamples() As String = Split(sr.ReadToEnd(), vbCrLf)
            sr.Close()
            Dim lst As New List(Of String)
            lst.AddRange(vExamples)
            lst.Sort()
            Dim sCI As String = G10.CI.ToString
            For i = 0 To vExamples.Length - 1
                If InStr(vExamples(i), sCI) Then
                    s = vExamples(i)
                    Exit For
                End If
            Next
        Catch ex As Exception

        End Try
        Return s
    End Function
    Sub parseOptions(ByVal txt As String)
        Try
            If txt = "" Then
                Exit Sub
            End If
            Dim e1() As String = Split(txt, "|")
            factor = Single.Parse(e1(0), Globalization.NumberStyles.Float, us)
            Select Case Int32.Parse(e1(1))
                Case Windows.WindowState.Maximized
                    Me.WindowState = Windows.WindowState.Maximized
            End Select
            Me.Left = Int32.Parse(e1(2))
            Me.Top = Int32.Parse(e1(3))
            Me.Width = Int32.Parse(e1(4))
            Me.Height = Int32.Parse(e1(5))
            setFont2(e1(6), Int32.Parse(e1(7)),
                      Boolean.Parse(e1(8)), Boolean.Parse(e1(9)))
            sClr = e1(10)
            parse_G10_options(txt)
        Catch ex As Exception
        End Try
    End Sub
    Sub parse_G10_options(txt As String)
        Try
            Dim e1() As String = Split(txt, "|")
            G10.CI = New Globalization.CultureInfo(Split(e1(11), vbTab)(1))
            G10.nDec = Int32.Parse(e1(12))
            G10.currBase = [Enum].Parse(GetType(Rational.Base), e1(13))
            G10.angleMode = [Enum].Parse(GetType(G10.AngleModes), e1(14))
            G10.frac = Boolean.Parse(e1(15))
            G10.detail = Boolean.Parse(e1(16))
            G10.var = Boolean.Parse(e1(17))
            G10.mathml = Boolean.Parse(e1(18))
            G10.sImg = e1(19)
        Catch ex As Exception

        End Try
    End Sub

    Sub setDefaultOptions()
        Try
            factor = 1.0
            setFont2("Courier New", 14, False, False)
            wb.fontName = "Courier New,Courier, Arial"
            wb.fontSize = "14"
            sClr = "blue"
            factor = 1.0
            Application.Current.MainWindow.Width = 480
            Application.Current.MainWindow.Height = 300
            G10.mathml = False
            wb.ZoomWebBrowser(factor)
            G10.CI = New Globalization.CultureInfo("en-US")
            G10.nDec = 15
            G10.currBase = Rational.Base.Decimal
            G10.angleMode = G10.AngleModes.radians
            G10.frac = False
            G10.detail = False
            G10.var = False
            G10.mathml = False
            G10.sImg = "i"
        Catch ex As Exception
        End Try
    End Sub
    Private Sub ClearAll_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        Try
            wb.Dsp(False, " ", "")
            rtbQuery.Document.Blocks.Clear()
            'eP.vars.Clear()
            lastGo2 = -1
            ReDim Preserve vsResultGo2HTML(lastGo2), vsResultGo3(lastGo2)
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Eng(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        updateStatusBar()
    End Sub
    Private Sub Incr(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        Try
            If factor >= 4 Then
                Exit Sub
            End If
            factor = factor + 0.25
            updateStatusBar()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Decr(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        Try
            If factor <= 0.5 Then
                Exit Sub
            End If
            factor = factor - 15.0 / 100.0
            updateStatusBar()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Img_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        If lblImg.Text = "i" Then
            lblImg.Text = "j"
            G10.sImg = "j"
        Else
            lblImg.Text = "i"
            G10.sImg = "i"
        End If
        G10.sImg = lblImg.Text
        updateStatusBar()
    End Sub
    Private Sub Hexa_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        updateStatusBar()
    End Sub
    Private Sub Fractions_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        updateStatusBar()
    End Sub
    Private Sub Octal_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        updateStatusBar()
    End Sub
    Private Sub Binary_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        updateStatusBar()
    End Sub
    Private Sub Detail(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        updateStatusBar()
    End Sub
    Private Sub Rounding(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        updateStatusBar()
    End Sub
    Private Sub Help(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        Try
            'infoHelp =
            'Application.GetResourceStream(New Uri("/Mates8 User's Guide.htm", UriKind.Relative))
            'WebBrowser1.NavigateToStream(infoHelp.Stream)

        Catch ex As Exception

        End Try
    End Sub
    Private Sub Notes(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)

        Try
            Dim info As New FileStream(IO.Directory.GetCurrentDirectory + "\Notes.txt", FileMode.Open, FileAccess.Read)
            Dim sr As New StreamReader(info)
            webbrowser1.NavigateToString(sr.ReadToEnd())
        Catch ex As Exception

        End Try
    End Sub
    Private Sub About(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        Dim w As New About
        w.Owner = Me
        w.ShowDialog()
    End Sub

#End Region

#Region "Calculate"
    Private Sub CalculateA(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        rtbQuery.IsEnabled = False
        iCurGo2 = vsResultGo3.Length
        Calculate()
    End Sub
    Dim s1() As String
    Private Async Sub Calculate()
        Try
            Dim e1 As String = New TextRange(
                rtbQuery.Document.ContentStart, rtbQuery.Document.ContentEnd).Text
            e1 = Replace(Trim(e1), vbLf, vbCrLf)
            If Regex.Replace(e1, "\s*", "").Length = 0 Then
                wb.Dsp(False, Msg8.num(67), sClr, False)
                Exit Sub
            End If
            If InStr(LCase(e1), "<math") Then
                Try
                    e1 = MathMLToString.convertToString(e1)
                Catch ex As Exception

                End Try
            End If
            sQuery = e1
            lastGo2 = vsResultGo3.Length
            G10.bCancel = False
            tbMsg.Text = "Press 'Esc' to cancel."
            t1 = Now.Ticks
            Dim pMtx As New parseMatrix()
            G10.Initialize()
            ReDim s1(-1)
            Dim getStrings As Task(Of String()) = pMtx.EvaluateAsync(sQuery)

            DoIndependentWork()
            s1 = Await getStrings
            Dim t2 As New TimeSpan(Now.Ticks - t1)
            If t2.TotalSeconds >= 2 Then
                tbMsg.Text = Math.Round(t2.TotalSeconds, 2).ToString(G10.CI) + " seconds"
            Else
                tbMsg.Text = Math.Round(t2.TotalMilliseconds, 0).ToString(G10.CI) + " milliseconds"
            End If
            If G10.currBase <> Rational.Base.Decimal Then
                s1(0) = "Numeric base is " + G10.currBase.ToString + "<br />" + s1(0)
            End If
            If G10.angleMode <> G10.AngleModes.radians Then
                s1(0) = "Angle base is " + G10.angleMode.ToString + "<br />" + s1(0)
            End If
            If s1(1).Length Then
                s1(0) += "</br>Detail:</br>" + s1(1)
            End If
            parse_G10_options(sCurrOptions)
            progBar.BeginAnimation(System.Windows.Controls.ProgressBar.ValueProperty, Nothing)
            ReDim Preserve vsResultGo2HTML(lastGo2), vsResultGo3(lastGo2)
            vsResultGo3(lastGo2) = supressTrailingCRLF(sQuery)
            DspCalc(s1(0))
            GC.Collect(GC.MaxGeneration)
        Catch ex As Exception
        Finally
            rtbQuery.IsEnabled = True
        End Try

    End Sub
    Sub DoIndependentWork()
        Try
            Dim duration As New Duration(TimeSpan.FromSeconds(2))
            Dim doubleanimation As New DoubleAnimation(100.0, duration)
            doubleanimation.RepeatBehavior = RepeatBehavior.Forever
            progBar.BeginAnimation(System.Windows.Controls.ProgressBar.ValueProperty, doubleanimation)
        Catch ex As Exception

        End Try
    End Sub
    Function supressTrailingCRLF(e1 As String) As String
        Do While e1.Length AndAlso e1.Chars(0) = vbCr OrElse e1.Chars(0) = vbLf
            e1 = Mid(e1, 2)
        Loop
        Do While Microsoft.VisualBasic.Right(e1, 2) = vbCrLf
            e1 = Mid(e1, 1, Len(e1) - 2)
        Loop
        Do While Microsoft.VisualBasic.Right(e1, 1) = vbLf OrElse Microsoft.VisualBasic.Right(e1, 1) = vbCr
            e1 = Mid(e1, 1, Len(e1) - 1)
        Loop
        'e1 = Replace(e1, ">|<", "><")
        Return Trim(Replace(e1, vbCr + vbCrLf, vbCrLf))
    End Function
    Sub DspCalc(ByVal sResult As String, Optional ByVal n1 As Int32 = -1)
        Try
            Dim s As String = ""
            If n1 = -1 Then
                n = vsResultGo3.Length
                s += n.ToString + ") Question:<br />"

                If Len(sQuery) Then
                    Dim sQuery1 As String = supressTrailingCRLF(sQuery.Replace(vbCrLf, "</br>"))
                    s += sQuery1.Replace("&", "&#38;") + vbCrLf
                    s += "<br />Answer: <br /><span id='ans'>" + sResult + "</span>"
                End If
                vsResultGo2HTML(lastGo2) = wb.Dsp(True, s, sClr, False, "")
            ElseIf n1 < vsResultGo3.Length Then
                s = "<table class='center' style='color:" + sClr + ";'><tr><td>"
                Dim s1 As String = vsResultGo3(n1).Replace(" ", "").Replace(vbCrLf, "|").Replace("||", "|")
                s += (n1 + 1).ToString + ") Question:<br />" + s1 + "<br/>" + vsResultGo2HTML(n1)
                s += "</td></tr></table>"
                wb.Dsp(False, s, sClr, False, "")
            End If


        Catch ex As Exception
            Dim s As String = ex.ToString
            Dim s1 As String = s
        End Try
    End Sub

#End Region

#Region "Populate"
    Sub populateSpecialChars()
        Try
            G10.getSpecialChars()
            Dim lst As New List(Of String)
            lst.AddRange(G10.vSpecialChars.ToArray)
            lst.Sort()
            SpecialChars.ItemsSource = lst
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Sub populateFnsAndConstants()
        Try
            Dim oc As New List(Of String)
            oc.Add("---Cons./Func.: --")
            oc.Add("---Constants: --")
            oc.Add("e")
            oc.Add("pi")
            oc.Add("---Functions: --")
            Dim vFn() As String = Split(G10.sImFn, "|")
            Array.Sort(vFn)
            oc.AddRange(vFn)
            oc.Add("---Functions: --")
            vFn = G10.vMtxFn
            ReDim Preserve vFn(vFn.Length)
            vFn(vFn.Length - 1) = "derivative"
            Array.Sort(vFn)
            oc.AddRange(vFn)
            Fns.ItemsSource = oc
            Fns.SelectedIndex = 0
            AddHandler Fns.SelectionChanged, AddressOf FnSelectionChanged
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Sub populateExamples()
        Try
            Dim info As New FileStream(IO.Directory.GetCurrentDirectory + "\Examples.txt", FileMode.Open, FileAccess.Read)
            Dim sr As New StreamReader(info)
            Dim vExamples() As String = Split(sr.ReadToEnd(), vbCrLf)
            sr.Close()
            Dim lst As New List(Of String)
            lst.AddRange(vExamples)
            Examples.ItemsSource = lst
            Examples.SelectedIndex = 0

        Catch ex As Exception
            Throw
        End Try
    End Sub
#End Region

#Region "OnLoad & More events"
    Private Sub webbrowser1_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs) Handles webbrowser1.NavigationCompleted
        rtbQuery.Focus()
    End Sub
    Private Sub MainWindow_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        Try
            opt.Close()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub WindowKeyDown(ByVal sender As Object, ByVal e As Input.KeyEventArgs)
        Try
            If e.Key = Key.Enter Then
                If Not (Keyboard.IsKeyDown(Key.LeftCtrl) OrElse Keyboard.IsKeyDown(Key.RightCtrl)) Then
                    CalculateA(Nothing, Nothing)
                    e.Handled = True
                ElseIf sender.name = "rtbQuery" Then
                    rtbQuery.CaretPosition.InsertLineBreak()
                    Dim moveTo As TextPointer = rtbQuery.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward)
                    rtbQuery.CaretPosition = moveTo
                    e.Handled = True
                End If
            ElseIf e.Key = Key.Escape Then
                G10.bCancel = True
            End If
        Catch ex As Exception

        End Try
    End Sub
    Dim sCurrOptions As String
    Private Sub MainWindow_Initialized(sender As Object, e As System.EventArgs) Handles Me.Initialized
        Try
            webbrowser1.EnsureCoreWebView2Async()
            wb = New C_WebBrowser(webbrowser1)
            If File.Exists(filepathCurrOptions) Then
                Dim fs As New FileStream(filepathCurrOptions, FileMode.Open)
                Dim sr As New StreamReader(fs)
                sCurrOptions = sr.ReadToEnd
                parseOptions(sCurrOptions)
                sr.Close()
                fs.Close()
            End If
            updateStatusBar()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            Me.Title = Msg8.NameAndVersion
            'populateCultureInfo()
            populateSpecialChars()
            populateFnsAndConstants()
            populateExamples()
            AccessKeyManager.Register("F", Fns)
            AccessKeyManager.AddAccessKeyPressedHandler(Fns, AddressOf AccessKeyPressedEventHandler)
            AccessKeyManager.Register("E", Examples)
            AccessKeyManager.AddAccessKeyPressedHandler(Examples, AddressOf ExamplesKeyPressedEventHandler)
            rtbQuery.Focus()
            wb.ZoomWebBrowser(factor)
        Catch ex As Exception

        End Try
    End Sub
    Private Sub AccessKeyPressedEventHandler(sender As Object, e As AccessKeyPressedEventArgs)
        Fns.IsDropDownOpen = True
    End Sub
    Private Sub ExamplesKeyPressedEventHandler(sender As Object, e As AccessKeyPressedEventArgs)
        Examples.IsDropDownOpen = True
    End Sub
    Private Sub rtbQueryToolTip(sender As Object, e As ToolTipEventArgs)
        Try
            Dim e1 As String = New TextRange(
                   rtbQuery.Document.ContentStart,
                   rtbQuery.Document.ContentEnd).Text
            If Len(Trim(e1)) = 0 OrElse e1 = vbCrLf Then
                If sender.name = "rtbQuery" Then
                    Tooltip1.Text = "Press Enter to calculate;" _
                    + vbCrLf + "'Ctrl+Enter' for a bnew line."
                    'Tooltip1.Visibility = Windows.Visibility.Visible
                    rtbTooltip1.Visibility = Windows.Visibility.Visible
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub rtbMouseWheel(sender As Object, e As System.Windows.Input.MouseWheelEventArgs) Handles rtbQuery.PreviewMouseWheel
        Try
            If Keyboard.Modifiers <> ModifierKeys.Control Then
                Return
            End If
            e.Handled = True
            If e.Delta > 0 Then
                rtbQuery.FontSize += 1
            Else
                rtbQuery.FontSize -= 1
            End If
            wb.head = ""
        Catch ex As Exception

        End Try
    End Sub
    Private Sub record_edit(sender As Object, e As RoutedEventArgs)
        Dim w As New Record(Me, Me.vsResultGo2HTML, vsResultGo3)
        w.Owner = Me
        w.ShowDialog()
    End Sub
    Public Sub setsTbQrysTbvar(sResult2HTML, sResult3)
        vsResultGo2HTML = sResult2HTML
        vsResultGo3 = sResult3
        iCurGo2 = vsResultGo3.Length - 1
    End Sub

#End Region

#Region "selectionChanged"
    Private Sub Examples_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles Examples.SelectionChanged
        Try
            If Examples.SelectedIndex < 0 Then Exit Sub
            Dim s As String = Examples.SelectedItem
            s = s.Replace("@", "|@").Replace("|", vbCrLf)
            showExample(s)
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Examples_PreviewKeyDown(sender As System.Object, e As System.Windows.Input.KeyEventArgs)
        Try
            showExample(e.OriginalSource.content)
        Catch ex As Exception
        End Try
    End Sub
    Sub showExample(exampleStr As String)
        Try
            Dim e1 As String = exampleStr
            e1 = Trim(e1)
            If Len(e1) = 0 Then
                Exit Sub
            End If
            If Microsoft.VisualBasic.Left(e1, 2) = "--" OrElse
            Microsoft.VisualBasic.Left(e1, 2) = "==" Then
                Exit Sub
            End If
            sOptions = toStrOptions()
            G10.currBase = Rational.Base.Decimal
            G10.angleMode = G10.AngleModes.radians
            G10.var = False
            G10.frac = False
            G10.nDec = 15
            G10.mathml = False
            G10.detail = False
            rtbQuery.Document.Blocks.Clear()
            rtbQuery.Document.Blocks.Add(New Paragraph(New Run(e1)))
            n -= 1
            CalculateA(Nothing, Nothing)

            updateStatusBar()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub SpecialChars_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles SpecialChars.SelectionChanged
        If sender.selectedindex > -1 Then
            Dim s As String = CType(sender.selecteditem, String)
            rtbQuery.CaretPosition.InsertTextInRun(s)
        End If
    End Sub
    Public Sub FnSelectionChanged(sender As Object, e As Controls.SelectionChangedEventArgs)
        Try
            If sender.selectedIndex < 1 Then Exit Sub
            Dim s As String = CType(sender.selecteditem, String)
            If Microsoft.VisualBasic.Left(s, 2) = "--" Then Exit Sub
            rtbQuery.CaretPosition.InsertTextInRun(s)
        Catch ex As Exception

        End Try
    End Sub

#End Region

#Region "File menu"

    Sub OpenHTMLFile(bAppend As Boolean)
        Dim fs As FileStream = Nothing
        Try

            Dim OpenFileDialog1 As New Microsoft.Win32.OpenFileDialog
            OpenFileDialog1.Filter = "HTML files (.htm)|*.htm;*.html|All files (*.*)|*.*"
            OpenFileDialog1.FileName = curFileName
            Dim bOk As Boolean = OpenFileDialog1.ShowDialog
            If bOk <> True Then
                Exit Sub
            End If
            Dim r As Forms.DialogResult
            If vsResultGo3.Length Then
                r = System.Windows.Forms.MessageBox.Show("Append file data to current data?", "Append file",
                    Forms.MessageBoxButtons.YesNoCancel, Forms.MessageBoxIcon.Question, Forms.MessageBoxDefaultButton.Button2)
                If r = MessageBoxResult.Cancel Then
                    Exit Sub
                End If
            End If
            fs = New FileStream(OpenFileDialog1.FileName, FileMode.Open)
            Dim s As String
            Using sr As New StreamReader(fs, True)
                s = sr.ReadToEnd
            End Using

            Try
                Dim doc As New mshtml.HTMLDocument
                Dim htmlDocument As mshtml.IHTMLDocument2 = CType(doc, mshtml.IHTMLDocument2)
                htmlDocument.write(s)
                htmlDocument.close()


                'Dim vTbl() As String = Regex.Split(s, "<TABLE>")
                Dim iv As Int32 = 1
                lastGo2 = IIf(r = MessageBoxResult.No, 0, vsResultGo3.Length)
                Dim sTbl As String = doc.body.innerHTML
                Dim vTbl() As String = Regex.Split(sTbl, "\d+\) Question:<BR>")
                For iv = 1 To vTbl.Length - 1
                    ReDim Preserve vsResultGo2HTML(lastGo2), vsResultGo3(lastGo2)
                    Dim posEndQuestion As Int32 = InStr(vTbl(iv), "Answer")
                    Dim sQuestion = Microsoft.VisualBasic.Left(vTbl(iv), posEndQuestion - 1)
                    vsResultGo3(lastGo2) = sQuestion.Replace("<BR>", vbCrLf).Replace("|", vbCrLf).Replace("&amp;", "&")
                    vsResultGo2HTML(lastGo2) = Mid(vTbl(iv), posEndQuestion)
                    '' HTML answer:
                    'Dim mc As MatchCollection = Regex.Matches(vTbl(iv), "(@(?<row>\d+)@(?<col>\d+)@>(?<ans>[^@>]+))+(-->)")
                    'Dim ans As String = "<!--"
                    'For iTbl As Int32 = 0 To mc(0).Groups("row").Captures.Count - 1
                    '    Dim row As Int32 = Int32.Parse(mc(0).Groups("row").Captures(iTbl).Value)
                    '    Dim col As Int32 = Int32.Parse(mc(0).Groups("col").Captures(iTbl).Value)
                    '    ans += mc(0).Groups("ans").Captures(iTbl).Value
                    '    If iTbl < mc(0).Groups("row").Captures.Count - 1 Then
                    '        ans += "<>"
                    '    End If
                    'Next
                    'vsResultGo2HTML(lastGo2) = ans.Replace("<!--", "").Replace("-->", "").Replace("|", "<br/>")
                    lastGo2 += 1
                Next
            Catch ex As Exception

            End Try
            iCurGo2 = 0
        Catch ex As Exception
            Dim s1 As String = ex.ToString
            Dim s2 As String = s1
        Finally
        End Try
    End Sub
    Private Sub SaveHTML()
        Dim fs As FileStream = Nothing
        Try
            Dim SaveFileDialog1 As New Microsoft.Win32.SaveFileDialog
            SaveFileDialog1.Filter = "HTML files (.htm)|*.htm;*.html|All files (*.*)|*.*"
            SaveFileDialog1.FileName = curFileName
            Dim r As Boolean = SaveFileDialog1.ShowDialog()
            If r <> True Then
                Exit Sub
            End If
            Dim path As String = SaveFileDialog1.FileName
            fs = New FileStream(path, FileMode.Create)
            Using sw As New StreamWriter(fs)
                Dim i As Int32
                Dim s As String = ""
                s += wb.head
                s += "<body><div id=""div1"">"
                For i = 0 To vsResultGo3.Length - 1
                    If Microsoft.VisualBasic.Left(vsResultGo2HTML(i), 7) = "Answer:" Then
                        s += (i + 1).ToString + ") Question:</br>" + vbCrLf + vsResultGo3(i)
                        s += vsResultGo2HTML(i)
                    Else
                        Dim doc As New mshtml.HTMLDocument
                        Dim htmlDocument As mshtml.IHTMLDocument2 = CType(doc, mshtml.IHTMLDocument2)
                        htmlDocument.write(vsResultGo2HTML(i))
                        htmlDocument.close()
                        Dim s1 As String = doc.body.innerHTML
                        s1 = Regex.Replace(s1, "\d+\) Question", (i + 1).ToString + ") Question")
                        If InStr(s1, ") Question") = 0 Then
                            Dim posDiv As Int32 = InStr(s1, "<DIV")
                            Dim posFinDiv As Int32 = InStr(posDiv, s1, ">")
                            s1 = s1.Insert(posFinDiv, (i + 1).ToString + ") Question:</br>" + vbCrLf)
                        End If
                        s += s1 ' Replace(s1, ">|<", "><") + vbCrLf
                    End If
                Next
                s += "</div>"
                s += "<div id=""last""></div>" + vbCrLf
                s += "</body></html>" + vbCrLf
                sw.Write(s)
            End Using

        Catch ex As Exception
        End Try
    End Sub
    Function Replace_BR_and_Others(s1 As String) As String
        Try
            s1 = Replace(s1, vbCrLf + vbCrLf, vbCrLf)
            s1 = Replace(Replace(s1, "<br />", "|"), vbCrLf, "|")
            s1 = Replace(s1, "<BR><BR>", "")
            s1 = Replace(s1, vbTab, ";")
            s1 = Replace(s1, ",", ";")
            s1 = Replace(s1, "base64;", "base64,")
        Catch ex As Exception

        End Try
        Return s1
    End Function
#End Region

#Region "statusBar"
    Sub updateStatusBar()
        'lblHexa.TextDecorations = IIf(chkHexa.IsChecked, Nothing, TextDecorations.Strikethrough)
        'lblOctal.TextDecorations = IIf(chkOctal.IsChecked, Nothing, TextDecorations.Strikethrough)
        'lblBinary.TextDecorations = IIf(chkBinary.IsChecked, Nothing, TextDecorations.Strikethrough)
        'lblRound.TextDecorations = IIf(chkRound.IsChecked, Nothing, TextDecorations.Strikethrough)
        'lblFractions.TextDecorations = IIf(chkFractions.IsChecked, Nothing, TextDecorations.Strikethrough)
        'lblEng.TextDecorations = IIf(chkEng.IsChecked, Nothing, TextDecorations.Strikethrough)
        lblImg.Text = IIf(G10.sImg = "i", "i", "j")
        Try
            Dim scale As New ScaleTransform(factor, factor)
            rtbQuery.LayoutTransform = scale
            wb.head = ""
            'wb.ZoomWebBrowser(factor)
            'lblPje.Text = Math.Round(factor * 100).ToString + "%"
        Catch ex As Exception

        End Try
    End Sub
    Private Sub StBar(sender As Object, e As RoutedEventArgs)
        Try
            Dim nom As String = Mid(CStr(sender.name), 4)
            Select Case LCase(nom)
                Case "lt" : gotoPrevious()
                Case "gt" : gotoNext()
                Case "clear"
                    Try
                        wb.Dsp(False, " ", "")
                        rtbQuery.Document.Blocks.Clear()
                    Catch ex As Exception

                    End Try
               'case "img" -->see stBarImg_MouseLeftButtonDown()
                Case "lblpje"

            End Select
            updateStatusBar()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub stBarImg_MouseLeftButtonDown(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) _
        Handles stBarImg.MouseLeftButtonDown, stBarImg.MouseRightButtonDown
        If G10.sImg = "i" Then
            G10.sImg = "j"
        Else
            G10.sImg = "i"
        End If
        lblImg.Text = G10.sImg
        updateStatusBar()
    End Sub
#End Region

#Region "GoTo"

    Sub dspGo2()
        Try
            If iCurGo2 = -1 Then Exit Sub
            lastGo2 = iCurGo2
            Dim s As String = Me.vsResultGo3(iCurGo2)
            's = supressTrailingCRLF(s)
            rtbQuery.Document.Blocks.Clear()
            rtbQuery.Document.Blocks.Add(New Paragraph(New Run(s)))
            DspCalc(Me.vsResultGo2HTML(iCurGo2).Replace("|", "<br />"), iCurGo2)
            updateStatusBar()
        Catch ex As Exception

        End Try
    End Sub

#End Region

#Region "ContextMenu"
    'Sub fontVars()
    '    setfont()
    'End Sub
    Sub font()
        Try
            Dim f As New System.Drawing.Font("Courier New", 14)
            Dim fD As New System.Windows.Forms.FontDialog
            fD.Font = f
            Dim result As Boolean = fD.ShowDialog()
            If result <> True Then
                Exit Sub
            End If
            Dim dst As System.Windows.Controls.RichTextBox

            dst = rtbQuery
            dst.FontFamily = New Media.FontFamily(fD.Font.Name)
            dst.FontSize = fD.Font.Size * 96.0 / 72.0
            dst.FontWeight = IIf(fD.Font.Bold, FontWeights.Bold, FontWeights.Regular)
            dst.FontStyle = IIf(fD.Font.Italic, FontStyles.Italic, FontStyles.Normal)

            wb.fontName = fD.Font.Name
            wb.fontSize = fD.Font.Size
            wb.fontBold = fD.Font.Bold
            wb.fontItalic = fD.Font.Italic
            'Dim tdc As New TextDecorationCollection()
            'If fD.Font.Underline Then tdc.Add(TextDecorations.Underline)
            'If f.Strikeout Then tdc.Add(TextDecorations.Strikethrough)
            'Me.TextDecorations = tdc
            setFont2(fD.Font.Name, fD.Font.Size, fD.Font.Bold, fD.Font.Italic)
            Calculate()
        Catch ex As Exception

        End Try
    End Sub
    Sub setFont2(family As String, fontSize As Int32, bold As Boolean, italic As Boolean)
        Try
            With rtbQuery
                .FontFamily = New Media.FontFamily(family)
                .FontSize = fontSize
                .FontWeight = IIf(bold, FontWeights.Bold, FontWeights.Regular)
                .FontStyle = IIf(italic, FontStyles.Italic, FontStyles.Normal)
            End With
            wb.fontName = family
            wb.fontSize = fontSize
            wb.fontBold = bold
            wb.fontItalic = italic
            wb.head = ""
            If iCurGo2 < vsResultGo2HTML.Length Then
                DspCalc("", Me.iCurGo2)
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Color(sender As Object, e As RoutedEventArgs)
        Dim c As New System.Windows.Forms.ColorDialog
        Try
            Dim r As System.Windows.Forms.DialogResult = c.ShowDialog
            If r = Forms.DialogResult.OK Then
                Dim clr As System.Drawing.Color = c.Color
                Dim hex As String = Convert.ToString(clr.ToArgb, 16)
                Do While Len(hex) < 6
                    hex = "0" + hex
                Loop
                sClr = "#" + Right(hex, 6)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Sub ClearQuery()
        rtbQuery.Document.Blocks.Clear()
    End Sub
    Sub ClearAndPasteQuery()
        rtbQuery.Document.Blocks.Clear()
        rtbQuery.Paste()
    End Sub
    Sub copyQuery()
        rtbQuery.Copy()
    End Sub
    Sub pasteQuery()
        rtbQuery.Paste()
    End Sub
    Sub PasteText()
        rtbQuery.Selection.Text = CStr(System.Windows.Forms.Clipboard.GetData("Text"))
    End Sub


    Sub gotoNext()
        Try
            ' Go to next calc.
            If Me.iCurGo2 < vsResultGo3.Length - 1 Then
                Me.iCurGo2 += 1
                dspGo2()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub MenuItem_Click(sender As Object, e As RoutedEventArgs) Handles Options.Click
        Try
            opt.Show()
        Catch ex As Exception

        End Try
    End Sub

    Sub gotoPrevious()
        ' Go to previous calc.
        If Me.iCurGo2 > 0 Then
            Me.iCurGo2 -= 1
            dspGo2()
            'CalculateA(Nothing, Nothing)
        Else
            Me.iCurGo2 = 0
        End If
    End Sub

    Sub populateLBQuery()
        Try
            lbGoTo.Items.Clear()
            For i As Int32 = 0 To vsResultGo3.Length - 1
                Dim item As New ListBoxItem
                Dim s As String = Replace(vsResultGo3(i), "|", vbCrLf)
                If Len(s) Then
                    item.Content = (i + 1).ToString + ") Question:" + s
                    lbGoTo.Items.Add(item)
                End If
            Next
        Catch ex As Exception

        End Try
    End Sub
    Private Sub lbGoTo_SelectionChanged(sender As Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles lbGoTo.SelectionChanged
        Try
            Me.iCurGo2 = lbGoTo.SelectedIndex
            dspGo2()
        Catch ex As Exception

        End Try
    End Sub


#End Region

End Class

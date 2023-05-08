Imports System.Text.RegularExpressions
Imports System.Text
Imports mshtml
Imports Microsoft.Web.WebView2.Core

Public Class C_WebBrowser
    Public WithEvents webbrowser1 As Microsoft.Web.WebView2.Wpf.WebView2
    Public fontName As String = "Courier New,Courier, Arial"
    Public fontSize As String = "14"
    Public fontBold As Boolean = False
    Public fontItalic As Boolean = False
    Public css As String
    Public head As String = ""
    Public factor As Double = 1.0

    Public Sub New(ByVal webview2 As Microsoft.Web.WebView2.Wpf.WebView2)
        Me.webbrowser1 = webview2
    End Sub

    Public Sub Clear()
        webbrowser1.NavigateToString("")
    End Sub
    Function Dsp(bIncludeInTable As Boolean, ByVal e1 As String, ByVal clr As String,
            Optional ByVal sangrado As Boolean = False, Optional Image As String = "") As String
        Dim e2 As String = ""
        Try
            If head = "" Then
                e2 = "<!doctype html>" + vbCrLf
                e2 += "<html><head id=""hd""><meta charset=""UTF-8"">" + vbCrLf
                e2 += "<meta name=""viewport"" content=""width=device-width"">" + vbCrLf
                css = ""
                css = "<style>" + vbCrLf
                css += "BODY {" + vbCrLf
                css += "font-family: " + fontName + ";" + vbCrLf
                css += "font-size: " + fontSize.ToString(New Globalization.CultureInfo("en-US")) + "px;" + vbCrLf
                css += "font-weight: " + IIf(fontBold, "bold", "normal") + ";" + vbCrLf
                css += "font-style: " + IIf(fontItalic, "italic", "normal") + ";" + vbCrLf
                css += "text-align: left;" + vbCrLf
                css += "zoom: 100%;" + vbCrLf
                css += "}" + vbCrLf
                css += ".center {"
                'css += "border: 1px solid;"
                css += "text-align: left;"
                css += "margin-left: auto;"
                css += "margin-right: auto;"
                css += "border-spacing: 5px;"
                css += "border-collapse: separate;"
                css += "padding:5px;"
                css += "}"
                css += ".center td {text-align:center;}"
                css += "td.mytd {background-color:#f3f3f3;}"
                css += "</style>" + vbCrLf
                e2 += css

                Dim sMW As String = ""
                sMW += "<script type="" text/javascript"" language=""javascript"">" + vbCrLf
                'If Image <> "" Then
                '    sMW += " var image = New Image();" + vbCrLf
                '    sMW += " image.src='data:image/png;base64," + Image + "'" + vbCrLf
                'End If
                sMW += "</script>" + vbCrLf
                sMW += "<script src=""https://polyfill.io/v3/polyfill.min.js?features=es6""></script>" + vbCrLf
                sMW += "<script id=""MathJax-script"" async src=""https://cdn.jsdelivr.net/npm/mathjax@3/es5/tex-mml-chtml.js""></script>" + vbCrLf
                e2 += sMW
                e2 += "</head>" + vbCrLf
                head = e2
            Else
            End If
            e2 = head
            e2 += "<body onload=""document.getElementById('ans').scrollIntoView();"" id=""bd"">" + vbCrLf
            If bIncludeInTable Then
                e2 += "<table class='center'><tr><td wrap>" + vbCrLf
                If clr <> "" Then
                    e2 += "<div id=""div1"" class='center' style='color:" + clr + ";'>" + vbCrLf
                Else
                    e2 += "<div id=""div1"" class='center'>" + vbCrLf
                End If
                Dim e3 As String = ""
                If sangrado Then e3 = "&nbsp;&nbsp;&nbsp;"
                e1 = Regex.Replace(e1, "(?<href>(http:|https:)([^\s]*))", "<a href='${href}'>${href}</a>")
                Dim e5 As String = e3 + e1
                e2 += e5
                e2 += "</div></td></tr></table>" + vbCrLf
                If Image <> "" Then
                    e2 += "<div><img src='data:image/png;base64," + Image + "'/></div>"
                End If
                e2 += "<div id=""last""></div><br/></td></tr></table>" + vbCrLf
            Else
                e2 += e1
            End If
            e2 += "</body></html>" + vbCrLf
            webbrowser1.NavigateToString(e2)
            e2 = Regex.Replace(e2, "\d+\s*\)\s*Question:<br />", "")
        Catch ex As Exception
        End Try
        Return e2
    End Function
    Sub ZoomWebBrowser(ByVal factor As Single)
        Try
            webbrowser1.ZoomFactor = factor
            'Dim style = webbrowser1.Document.body.style
            'Dim str As String = (100 * factor).ToString(New Globalization.CultureInfo("en-US"))
            'style.zoom = str
            Me.factor = factor
        Catch ex As Exception
        End Try
    End Sub
    Private Sub webbrowser1_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs) Handles webbrowser1.NavigationCompleted
    End Sub


End Class

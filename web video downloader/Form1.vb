Imports System.Web
Imports mshtml
Imports HtmlAgilityPack
Imports System.Net

Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        TextBox1.Text = ""
        TextBox2.Text = ""
        For Each itm As ListViewItem In ListView1.Items
            itm.Remove()
        Next

        Dim link = TextBox3.Text
        Dim request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(link)
        Dim response As System.Net.HttpWebResponse = request.GetResponse()
        Dim sr As System.IO.StreamReader = New System.IO.StreamReader(response.GetResponseStream())
        Dim sourcecode As String = sr.ReadToEnd()

        If ComboBox1.Text.ToLower = "youtube" Then
            parse_youtube_Html(sourcecode)
        ElseIf ComboBox1.Text.ToLower = "veoh" Then
            parse_veoh_html(sourcecode)
        ElseIf ComboBox1.Text.ToLower = "dailymotion" Then
            parse_dailymotion_html(sourcecode)
        ElseIf ComboBox1.Text.ToLower = "metacafe" Then
            parse_metacafe_html(sourcecode)
        End If
    End Sub

    Private Sub TextBox3_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox3.TextChanged
        If TextBox3.Text.ToLower.Contains("/watch?v") And TextBox3.Text.ToLower.Contains(".youtube.") Then
            ComboBox1.Text = "Youtube"
        ElseIf TextBox3.Text.ToLower.Contains(".veoh.") Then
            ComboBox1.Text = "Veoh"
        ElseIf TextBox3.Text.ToLower.Contains(".dailymotion.") Then
            ComboBox1.Text = "Dailymotion"
        ElseIf TextBox3.Text.ToLower.Contains(".metacafe.") Then
            ComboBox1.Text = "Metacafe"
        Else
            ComboBox1.Text = "Not supported"
        End If
    End Sub


    Function parse_metacafe_html(ByVal htmltoparse As String)
        Dim htmldoc As New HtmlAgilityPack.HtmlDocument
        htmldoc.LoadHtml(htmltoparse)
        Dim links As String = ""
        Dim my_title = htmldoc.DocumentNode.SelectSingleNode("//head/title").InnerText().Replace("- Video", "")
        For Each c In IO.Path.GetInvalidFileNameChars
            my_title = my_title.Replace(c, "")
        Next

        Dim allnodes As HtmlNodeCollection = htmldoc.DocumentNode.SelectNodes("//script")
        For Each nde As HtmlNode In allnodes
            If nde.InnerText.Contains("swfobject.registerObject") Then
                links = extract_metacafe_links(HttpUtility.UrlDecode(nde.InnerText), my_title)
                Return links
                Exit Function
            End If
        Next
        Return "nothing"
    End Function
    Function extract_metacafe_links(ByVal jstext As String, ByVal title As String)
        jstext = jstext.Replace("http:\/\/", vbCrLf & "http:\/\/")
        Dim links = ""
        TextBox2.Text = jstext
        For Each line As String In TextBox2.Lines
            If ((line.Contains("http:\/\/") And line.Contains(".mp4"))) Then
                links += line & vbCrLf
            End If
        Next
        links = links.Replace("\/", "/")
        links = links.Replace("""", vbCrLf)
        TextBox2.Text = links
        links = ""
        For Each line As String In TextBox2.Lines
            If ((line.Contains("http://") And line.Contains(".mp4"))) Then
                links += line & vbCrLf
            End If
        Next
        TextBox2.Text = links
        Dim str(2) As String
        For Each line As String In TextBox2.Lines
            str(0) = title
            str(1) = ".mp4"
            str(2) = line
            If str(2).Contains("http://") = True Then
                Dim listitem As ListViewItem = New ListViewItem(str)
                ListView1.Items.Add(listitem)
            End If
        Next
        Return "nothing"
    End Function


#Region "dailymotion"
    Function parse_dailymotion_html(ByVal htmltoparse As String)
        Dim htmldoc As New HtmlAgilityPack.HtmlDocument
        htmldoc.LoadHtml(htmltoparse)
        Dim links As String = ""
        Dim my_title = htmldoc.DocumentNode.SelectSingleNode("//head/title").InnerText().Replace("- Video Dailymotion", "")
        For Each c In IO.Path.GetInvalidFileNameChars
            my_title = my_title.Replace(c, "")
        Next

        Dim allnodes As HtmlNodeCollection = htmldoc.DocumentNode.SelectNodes("//script")
        For Each nde As HtmlNode In allnodes
            If nde.InnerText.Contains("var videoId") Then
                links = extract_dailymotion_links(HttpUtility.UrlDecode(nde.InnerText), my_title)
                Return links
                Exit Function
            End If
        Next
        Return "nothing"
    End Function
    Function extract_dailymotion_links(ByVal jstext As String, ByVal title As String)
        jstext = jstext.Replace("http:\/\/", vbCrLf & "http:\/\/")
        Dim links = ""

        TextBox2.Text = jstext
        For Each line As String In TextBox2.Lines
            If ((line.Contains("http:\/\/") And line.Contains(".mp4"))) Then
                links += line & vbCrLf
            End If
        Next
        links = links.Replace("\/", "/")
        links = links.Replace("""", vbCrLf)
        TextBox2.Text = links
        links = ""
        For Each line As String In TextBox2.Lines
            If ((line.Contains("http://") And line.Contains(".mp4"))) Then
                links += line & vbCrLf
            End If
        Next
        TextBox2.Text = links
        Dim str(2) As String
        For Each line As String In TextBox2.Lines
            str(0) = title
            str(1) = ".mp4"
            str(2) = line
            If str(2).Contains("http://") = True Then
                Dim listitem As ListViewItem = New ListViewItem(str)
                ListView1.Items.Add(listitem)
            End If
        Next
        Return "nothing"
    End Function
#End Region

#Region "for veoh"
    Function parse_veoh_html(ByVal htmltoparse As String)
        Dim htmldoc As New HtmlAgilityPack.HtmlDocument
        htmldoc.LoadHtml(htmlToParse)
        Dim links As String = ""
        Dim my_title = htmldoc.DocumentNode.SelectSingleNode("//head/title").InnerText().Replace("Watch Videos Online |", "")
        For Each c In IO.Path.GetInvalidFileNameChars
            my_title = my_title.Replace(c, "")
        Next

        Dim allnodes As HtmlNodeCollection = htmldoc.DocumentNode.SelectNodes("//script")
        For Each nde As HtmlNode In allnodes
            If nde.InnerText.Contains("api.baseURL") And nde.InnerText.Contains("api.apiKey") Then
                links = extract_veoh_links(HttpUtility.UrlDecode(nde.InnerText), my_title)
                Return links
                Exit Function
            End If
        Next
        Return "nothing"
    End Function

    Function extract_veoh_links(ByVal jstext As String, ByVal title As String)
        jstext = jstext.Replace("http:\/\/", vbCrLf & "http:\/\/")
        Dim links = ""

        TextBox2.Text = jstext
        For Each line As String In TextBox2.Lines
            If ((line.Contains("http:\/\/") And line.Contains(".mp4")) And Not (line.Contains("fullHashPath") _
                                                                                Or line.Contains("embedCode"))) Then
                links += line & vbCrLf
            End If
        Next
        links = links.Replace("\/", "/")
        links = links.Replace("""", vbCrLf)
        TextBox2.Text = links
        links = ""
        For Each line As String In TextBox2.Lines
            If ((line.Contains("http://") And line.Contains(".mp4"))) Then
                links += line & vbCrLf
            End If
        Next
        TextBox2.Text = links
        Dim str(2) As String
        For Each line As String In TextBox2.Lines
            str(0) = title
            str(1) = ".mp4"
            str(2) = line
            If str(2).Contains("http://") = True Then
                Dim listitem As ListViewItem = New ListViewItem(str)
                ListView1.Items.Add(listitem)
            End If
        Next
        Return "nothing"
    End Function
#End Region

#Region "for you tube"
    Function parse_youtube_Html(ByVal htmlToParse) As String
        Dim htmldoc As New HtmlAgilityPack.HtmlDocument
        htmldoc.LoadHtml(htmlToParse)
        Dim links As String = ""
        Dim my_title = htmldoc.DocumentNode.SelectSingleNode("//head/title").InnerText().Replace("-", "")
        For Each c In IO.Path.GetInvalidFileNameChars
            my_title = my_title.Replace(c, "")
        Next

        Dim allnodes As HtmlNodeCollection = htmldoc.DocumentNode.SelectNodes("//script")
        For Each nde As HtmlNode In allnodes
            If nde.InnerText.Contains("ytplayer") Then
                links = extract_youtube_links(HttpUtility.UrlDecode(nde.InnerText), my_title.Replace("youtube", ""))
                Return links
                Exit Function
            End If
        Next
        Return "nothing"
    End Function
    Function extract_youtube_links(ByVal jstext As String, ByVal title As String)
        jstext = jstext.Replace("url=", vbCrLf & "url=")
        jstext = jstext.Replace("\u0", vbCrLf & "\u0")
        Dim links = ""
        TextBox2.Text = jstext
        For Each line As String In TextBox2.Lines
            If (line.Contains("url=") And line.Contains("/videoplayback?")) Then
                links += (line.Replace("url=", "")) & vbCrLf
            End If
        Next

        TextBox2.Text = links
        Dim str(2) As String
        Dim itag As String = ""
        For Each line As String In TextBox2.Lines
            TextBox1.Text = line.Replace("?", vbCrLf)
            TextBox1.Text = TextBox1.Text.Replace("&", vbCrLf)
            TextBox1.Text = TextBox1.Text.Replace("%", vbCrLf)
            TextBox1.Text = TextBox1.Text.Replace(",", vbCrLf)
            For Each line2 As String In TextBox1.Lines
                If line2.Contains("itag=") Then
                    itag = line2.Replace("itag=", "")
                    Exit For
                End If
            Next
            str(0) = title
            str(1) = youtube_itag(itag)
            str(2) = line & "&title=" & title
            If str(2).Contains("/videoplayback?") = True Then
                Dim listitem As ListViewItem = New ListViewItem(str)
                ListView1.Items.Add(listitem)
            End If
        Next
        Return links
    End Function
#End Region

#Region "youtube itags"
    Function youtube_itag(ByVal itag As String)
        Dim des As String = "format id " & itag
        If itag.Contains("5") Then
            des = "Flv (400 x 240)"
        ElseIf itag.Contains("6") Then
            des = "Flv (450 x 270)"
        ElseIf itag.Contains("13") Then
            des = "3gp (Mobile phones, iPod friendly)"
        ElseIf itag.Contains("17") Then
            des = "3gp (176 x 144)"
        ElseIf itag.Contains("18") Then
            des = "mp4 (Medium Quality [360p])"
        ElseIf itag.Contains("22") Then
            des = "mp4 (HD High Quality [720p])"
        ElseIf itag.Contains("34") Then
            des = "Flv (360p)"
        ElseIf itag.Contains("35") Then
            des = "Flv [480p])"
        ElseIf itag.Contains("36") Then
            des = "3gp (240p)"
        ElseIf itag.Contains("37") Then
            des = "mp4 (HD High Quality [1080p])"
        ElseIf itag.Contains("38") Then
            des = "mp4 (HD High Quality [3072p])"
        ElseIf itag.Contains("43") Then
            des = "Webm (Medium Quality [360p])"
        ElseIf itag.Contains("44") Then
            des = "Webm (480p)"
        ElseIf itag.Contains("45") Then
            des = "mp4 (Webm [720p])"
        ElseIf itag.Contains("46") Then
            des = "Webm (1080p)"
        ElseIf itag.Contains("82") Then
            des = "mp4(640 x 360)"
        ElseIf itag.Contains("83") Then
            des = "mp4 (854 x 480)"
        ElseIf itag.Contains("84") Then
            des = "mp4 (1280 x 720)"
        ElseIf itag.Contains("85") Then
            des = "mp4(1920 x 1080p)"
        ElseIf itag.Contains("100") Then
            des = "Webm (640 x 360)"
        ElseIf itag.Contains("101") Then
            des = "Webm (854 x 480)"
        ElseIf itag.Contains("102") Then
            des = "Webm (1280 x 720)"
        ElseIf itag.Contains("92") Then
            des = "mp4(320 x 240)"
        ElseIf itag.Contains("93") Then
            des = "mp4 (640 x 360)"
        ElseIf itag.Contains("94") Then
            des = "mp4 (854 x 480)"
        ElseIf itag.Contains("95") Then
            des = "mp4 (1280 x 720)"
        ElseIf itag.Contains("96") Then
            des = "mp4 (1920 x 1080)"
        ElseIf itag.Contains("132") Then
            des = "mp4 (320 x 240)"
        ElseIf itag.Contains("151") Then
            des = "mp4 (* x 72)"
            'dash mp4 video
        ElseIf itag.Contains("133") Then
            des = "mp4(video only [320 x 240])"
        ElseIf itag.Contains("134") Then
            des = "mp4(video only [640 x 360])"
        ElseIf itag.Contains("135") Then
            des = "mp4(video only [854 x 480])"
        ElseIf itag.Contains("136") Then
            des = "mp4(video only [1280 x 720])"
        ElseIf itag.Contains("137") Then
            des = "mp4(video only [1920 x 1080])"
        ElseIf itag.Contains("138") Then
            des = "mp4(video only [* x 2160 ])"
        ElseIf itag.Contains("160") Then
            des = "mp4(video only [176 x 144])"
        ElseIf itag.Contains("264") Then
            des = "mp4(video only [176 x 1440])"
        ElseIf itag.Contains("298") Then
            des = "mp4(video only [1280 x 720])"
        ElseIf itag.Contains("299") Then
            des = "mp4(video only [1920 x 1080])"
        ElseIf itag.Contains("266") Then
            des = "mp4(video only [* x 2160])"
            'dash mp4 audio
        ElseIf itag.Contains("139") Then
            des = "m4a(audio only)"
        ElseIf itag.Contains("140") Then
            des = "m4a(audio only)"
        ElseIf itag.Contains("141") Then
            des = "m4a(audio only)"
            'dash webm video
        ElseIf itag.Contains("167") Then
            des = "Webm(video only [640 x 360])"
        ElseIf itag.Contains("168") Then
            des = "Webm(video only [854 x 480])"
        ElseIf itag.Contains("169") Then
            des = "Webm(video only [1280 x 720])"
        ElseIf itag.Contains("170") Then
            des = "Webm(video only [1920 x 1080])"
        ElseIf itag.Contains("218") Then
            des = "Webm(video only [854 x 480])"
        ElseIf itag.Contains("219") Then
            des = "Webm(video only [854 x 480])"
        ElseIf itag.Contains("220") Then '-------->recheck 220?
            des = "Webm(video only [* x 144])"
        ElseIf itag.Contains("242") Then
            des = "Webm(video only [320 x 240])"
        ElseIf itag.Contains("243") Then
            des = "Webm(video only [640 x 360])"
        ElseIf itag.Contains("244") Then
            des = "Webm(video only [854 x 480])"
        ElseIf itag.Contains("245") Then
            des = "Webm(video only [854 x 480])"
        ElseIf itag.Contains("246") Then
            des = "Webm(video only [854 x 480])"
        ElseIf itag.Contains("247") Then
            des = "Webm(video only [1280 x 720])"
        ElseIf itag.Contains("248") Then
            des = "Webm(video only [1920 x 1080])"
        ElseIf itag.Contains("271") Then
            des = "Webm(video only [176 x 1440])"
        ElseIf itag.Contains("272") Then
            des = "Webm(video only [* x 2160])"
        ElseIf itag.Contains("302") Then
            des = "Webm(video only [* x 2160])"
        ElseIf itag.Contains("303") Then
            des = "Webm(video only [1920 x 1080])"
        ElseIf itag.Contains("308") Then
            des = "Webm(video only [176 x 1440])"
        ElseIf itag.Contains("313") Then
            des = "Webm(video only [* x 2160])"
        ElseIf itag.Contains("315") Then
            des = "Webm(video only [* x 2160])"
            'dash webm audio
        ElseIf itag.Contains("171") Then
            des = "Webm(audio only)"
        ElseIf itag.Contains("172") Then
            des = "Webm(audio only)"
        End If
        Return des
    End Function
#End Region

#Region "download"  'download
    Dim whereToSave As String 'Where the program save the file
    Delegate Sub ChangeTextsSafe(ByVal length As Long, ByVal position As Integer, ByVal percent As Integer, ByVal speed As Double)
    Delegate Sub DownloadCompleteSafe(ByVal cancelled As Boolean)

    Public Sub DownloadComplete(ByVal cancelled As Boolean)
        Button2.Enabled = True
        Button3.Enabled = False
        If cancelled Then
            Me.Label6.Text = "Cancelled"
            MessageBox.Show("Download aborted", "Aborted", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Me.Label6.Text = "Successfully downloaded"
            MessageBox.Show("Successfully downloaded!", "Downloaded", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        Me.ProgressBar1.Value = 0
        Me.Label2.Text = "Downloading: "
        Me.Label3.Text = "Save to: "
        Me.Label4.Text = "File size: "
        Me.Label5.Text = "Download speed: "
        Me.Label6.Text = ""
        Me.Label7.Text = ""
        Me.Button1.Enabled = True
    End Sub

    Public Sub ChangeTexts(ByVal length As Long, ByVal position As Integer, ByVal percent As Integer, ByVal speed As Double)
        Me.Label4.Text = "File Size: " & Math.Round((length / 1024), 2) & " KB"
        ' Me.Label2.Text = "Downloading: " & Me.txtFileName.Text
        Me.Label6.Text = "Downloaded " & Math.Round((position / 1024), 2) & " KB of " & Math.Round((length / 1024), 2) & "KB (" & Me.ProgressBar1.Value & "%)"
        If speed = -1 Then
            Me.Label5.Text = "Speed: calculating..."
        Else
            Me.Label5.Text = "Speed: " & Math.Round((speed / 1024), 2) & " KB/s"
        End If
        Me.ProgressBar1.Value = percent
    End Sub

    Private Sub btnDownload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim dl_file = ""
        Dim title = ""
        Dim ext = "All|*.*"
        For Each item As ListViewItem In ListView1.SelectedItems
            dl_file = (item.SubItems(2).Text)
            title = item.SubItems(0).Text
            Label7.Text = dl_file
            If item.SubItems(1).Text.ToLower.Contains("webm") Then
                ext = "webm|*.webm"
            ElseIf item.SubItems(1).Text.ToLower.Contains("flv") Then
                ext = "flv|*.flv"
            ElseIf item.SubItems(1).Text.ToLower.Contains("mp4") Then
                ext = "mp4|*.mp4"
            ElseIf item.SubItems(1).Text.ToLower.Contains("3gp") Then
                ext = "3gp|*.3gp"
            End If
        Next

        If dl_file <> "" AndAlso (dl_file.StartsWith("http://") Or dl_file.StartsWith("https://")) Then
            Me.SaveFileDialog1.FileName = title
            Me.SaveFileDialog1.Filter = ext
            If Me.SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                Me.whereToSave = Me.SaveFileDialog1.FileName
                Me.Label3.Text = "Save to: " & Me.whereToSave
                Me.Button2.Enabled = False
                Me.Button3.Enabled = True
                Me.Button1.Enabled = False
                Me.BackgroundWorker1.RunWorkerAsync() 'Start download
            End If
        Else
            MessageBox.Show("Please insert valid URL for download", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        'Creating the request and getting the response
        Dim theResponse As HttpWebResponse
        Dim theRequest As HttpWebRequest
        Try 'Checks if the file exist
            theRequest = WebRequest.Create(Label7.Text)
            theResponse = theRequest.GetResponse
        Catch ex As Exception
            MessageBox.Show("An error occurred while downloading file. Possibe causes:" & ControlChars.CrLf & _
                            "1) File doesn't exist" & ControlChars.CrLf & _
                            "2) Remote server error" & ControlChars.CrLf & _
                            "Try clicking the 'Find links' button again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Dim cancelDelegate As New DownloadCompleteSafe(AddressOf DownloadComplete)
            Me.Invoke(cancelDelegate, True)
            Exit Sub
        End Try
        Dim length As Long = theResponse.ContentLength 'Size of the response (in bytes)

        Dim safedelegate As New ChangeTextsSafe(AddressOf ChangeTexts)
        Me.Invoke(safedelegate, length, 0, 0, 0) 'Invoke the TreadsafeDelegate
        Dim writeStream As New IO.FileStream(Me.whereToSave, IO.FileMode.Create)
        'Replacement for Stream.Position (webResponse stream doesn't support seek)
        Dim nRead As Integer
        'To calculate the download speed
        Dim speedtimer As New Stopwatch
        Dim currentspeed As Double = -1
        Dim readings As Integer = 0
        Do
            If BackgroundWorker1.CancellationPending Then 'If user abort download
                Exit Do
            End If
            speedtimer.Start()
            Dim readBytes(4095) As Byte
            Dim bytesread As Integer = theResponse.GetResponseStream.Read(readBytes, 0, 4096)
            nRead += bytesread
            Dim percent As Short = (nRead * 100) / length
            Me.Invoke(safedelegate, length, nRead, percent, currentspeed)
            If bytesread = 0 Then Exit Do
            writeStream.Write(readBytes, 0, bytesread)
            speedtimer.Stop()
            readings += 1
            If readings >= 5 Then 'For increase precision, the speed it's calculated only every five cicles
                currentspeed = 20480 / (speedtimer.ElapsedMilliseconds / 1000)
                speedtimer.Reset()
                readings = 0
            End If
        Loop
        'Close the streams
        theResponse.GetResponseStream.Close()
        writeStream.Close()
        If Me.BackgroundWorker1.CancellationPending Then
            IO.File.Delete(Me.whereToSave)
            Dim cancelDelegate As New DownloadCompleteSafe(AddressOf DownloadComplete)
            Me.Invoke(cancelDelegate, True)
            Exit Sub
        End If
        Dim completeDelegate As New DownloadCompleteSafe(AddressOf DownloadComplete)
        Me.Invoke(completeDelegate, False)
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Me.BackgroundWorker1.CancelAsync() 'Send cancel request
    End Sub
#End Region
    
    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        MsgBox("Created by Pratheesh Russell", 0, "About:")
    End Sub
End Class
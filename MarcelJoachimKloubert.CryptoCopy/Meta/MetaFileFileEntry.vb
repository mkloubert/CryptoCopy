'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO

''' <summary>
''' A file entry for a meta file.
''' </summary>
Public NotInheritable Class MetaFileFileEntry

#Region "Fields (15)"

    Private _creationTime As Date?
    Private _crypter As ICrypter
    Private ReadOnly _FILE As MetaFile
    Private _lastAccessTime As Date?
    Private _lastWriteTime As Date?
    Private _name As String
    Private _realFile As FileInfo
    Private ReadOnly _XML As XElement
    ''' <summary>
    ''' The name of the attribute for the creation time.
    ''' </summary>
    Public Const CREATION_TIME_ATTRIB_NAME As String = "creationTime"
    ''' <summary>
    ''' The attribute name for a file name.
    ''' </summary>
    Public Const FILE_ATTRIB_NAME As String = "file"
    ''' <summary>
    ''' The attribute name for iteration count.
    ''' </summary>
    Public Const ITERATIONS_ATTRIB_NAME As String = "iterations"
    ''' <summary>
    ''' The name of the attribute for the last access time.
    ''' </summary>
    Public Const LAST_ACCESS_TIME_ATTRIB_NAME As String = "lastAccessTime"
    ''' <summary>
    ''' The name of the attribute for the last write time.
    ''' </summary>
    Public Const LAST_WRITE_TIME_ATTRIB_NAME As String = "lastWriteTime"
    ''' <summary>
    ''' The attribute name for a password.
    ''' </summary>
    Public Const PASSWORD_ATTRIB_NAME As String = "pwd"
    ''' <summary>
    ''' The attribute name for a salt.
    ''' </summary>
    Public Const SALT_ATTRIB_NAME As String = "salt"

#End Region

#Region "Constructors (1)"

    ''' <summary>
    ''' Initializes a new instance of the <see cref="MetaFileFileEntry" /> class.
    ''' </summary>
    ''' <param name="file">The underlying meta file.</param>
    ''' <param name="xml">The underlying meta data.</param>
    Sub New(file As MetaFile, xml As XElement)
        Me._FILE = file
        Me._XML = xml

        Me.Init()
    End Sub

#End Region

#Region "Properties (8)"

    ''' <summary>
    ''' Gets the creation time (UTC).
    ''' </summary>
    Public ReadOnly Property CreationTime As Date?
        Get
            Return Me._creationTime
        End Get
    End Property

    ''' <summary>
    ''' Gets the underlying crypter.
    ''' </summary>
    Public ReadOnly Property Crypter As ICrypter
        Get
            Return Me._crypter
        End Get
    End Property

    ''' <summary>
    ''' Gets the underlying meta file.
    ''' </summary>
    Public ReadOnly Property File As MetaFile
        Get
            Return Me._FILE
        End Get
    End Property

    ''' <summary>
    ''' Gets the last access time (UTC).
    ''' </summary>
    Public ReadOnly Property LastAccessTime As Date?
        Get
            Return Me._lastAccessTime
        End Get
    End Property
    ''' <summary>
    ''' Gets the last write time (UTC).
    ''' </summary>
    Public ReadOnly Property LastWriteTime As Date?
        Get
            Return Me._lastWriteTime
        End Get
    End Property

    ''' <summary>
    ''' Gets the name of the file.
    ''' </summary>
    Public ReadOnly Property Name As String
        Get
            Return Me._name
        End Get
    End Property

    ''' <summary>
    ''' Gets the real local file.
    ''' </summary>
    Public ReadOnly Property RealFile As FileInfo
        Get
            Return Me._realFile
        End Get
    End Property

    ''' <summary>
    ''' Gets the underlying meta data.
    ''' </summary>
    Public ReadOnly Property Xml As XElement
        Get
            Return Me._XML
        End Get
    End Property

#End Region

#Region "Methods (2)"

    ''' <summary>
    ''' Decrypts the file to a specific directory.
    ''' </summary>
    ''' <param name="dirPath">The path to the target directory.</param>
    ''' <returns>The decrypted file.</returns>
    Public Function DecryptTo(dirPath As String) As FileInfo
        Dim dir As DirectoryInfo = New DirectoryInfo(dirPath)
        If Not dir.Exists Then
            dir.Create()
        End If

        Dim result = New FileInfo(Path.Combine(dir.FullName, _
                                               Me.Name))
        If result.Exists Then
            result.Delete()
        End If

        Using srcStream As FileStream = Me.RealFile.OpenRead()
            Using destStream As FileStream = New FileStream(result.FullName, FileMode.CreateNew, FileAccess.ReadWrite)
                Me.Crypter.Decrypt(srcStream, destStream)
            End Using
        End Using

        '' creation time
        If Me.CreationTime.HasValue Then
            result.CreationTimeUtc = Me.CreationTime.Value
        End If

        '' last write time
        If Me.LastWriteTime.HasValue Then
            result.LastWriteTimeUtc = Me.LastWriteTime.Value
        End If

        result.Refresh()

        '' last write time
        If Me.LastAccessTime.HasValue Then
            result.LastAccessTimeUtc = Me.LastAccessTime.Value
        End If

        Return result
    End Function

    Private Sub Init()
        '' name
        If Not String.IsNullOrWhiteSpace(Me.Xml.Value) Then
            Me._name = Me.Xml.Value.Trim()
        End If

        '' real file
        Dim fileAttrib As XAttribute = Me.Xml.Attribute(FILE_ATTRIB_NAME)
        If Not fileAttrib Is Nothing Then
            Dim fileName As String = fileAttrib.Value

            If Not String.IsNullOrWhiteSpace(fileName) Then
                Me._realFile = New FileInfo(Path.Combine(Me.File.File.Directory.FullName, _
                                                         fileName.Trim()))
            End If
        End If

        '' creation time
        Dim creationTimeTimeAttrib As XAttribute = Me.Xml.Attribute(CREATION_TIME_ATTRIB_NAME)
        If Not creationTimeTimeAttrib Is Nothing Then
            If Not String.IsNullOrWhiteSpace(creationTimeTimeAttrib.Value) Then
                Dim ticks As Long
                If Long.TryParse(creationTimeTimeAttrib.Value.Trim(), ticks) Then
                    Try
                        Me._creationTime = New Date(ticks, DateTimeKind.Utc)
                    Catch ex As Exception
                        '' ignore here
                    End Try
                End If
            End If
        End If

        '' last write time
        Dim lastWriteTimeAttrib As XAttribute = Me.Xml.Attribute(LAST_WRITE_TIME_ATTRIB_NAME)
        If Not lastWriteTimeAttrib Is Nothing Then
            If Not String.IsNullOrWhiteSpace(lastWriteTimeAttrib.Value) Then
                Dim ticks As Long
                If Long.TryParse(lastWriteTimeAttrib.Value.Trim(), ticks) Then
                    Try
                        Me._lastWriteTime = New Date(ticks, DateTimeKind.Utc)
                    Catch ex As Exception
                        '' ignore here
                    End Try
                End If
            End If
        End If

        '' last access time
        Dim lastAccessTimeAttrib As XAttribute = Me.Xml.Attribute(LAST_ACCESS_TIME_ATTRIB_NAME)
        If Not lastAccessTimeAttrib Is Nothing Then
            If Not String.IsNullOrWhiteSpace(lastAccessTimeAttrib.Value) Then
                Dim ticks As Long
                If Long.TryParse(lastAccessTimeAttrib.Value.Trim(), ticks) Then
                    Try
                        Me._lastAccessTime = New Date(ticks, DateTimeKind.Utc)
                    Catch ex As Exception
                        '' ignore here
                    End Try
                End If
            End If
        End If

        '' crypter
        Dim pwdAttrib As XAttribute = Me.Xml.Attribute(PASSWORD_ATTRIB_NAME)
        If Not pwdAttrib Is Nothing Then
            Dim saltAttrib As XAttribute = Me.Xml.Attribute(SALT_ATTRIB_NAME)

            If Not saltAttrib Is Nothing Then
                Dim iterationsAttrib As XAttribute = Me.Xml.Attribute(ITERATIONS_ATTRIB_NAME)

                If Not iterationsAttrib Is Nothing Then
                    Me._crypter = New RijndaelCrypter(Convert.FromBase64String(pwdAttrib.Value.Trim()), _
                                                      Convert.FromBase64String(saltAttrib.Value.Trim()), _
                                                      Integer.Parse(iterationsAttrib.Value.Trim()))
                End If
            End If
        End If
    End Sub

#End Region

End Class
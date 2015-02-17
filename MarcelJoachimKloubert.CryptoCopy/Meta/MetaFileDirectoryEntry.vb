'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO

''' <summary>
''' A directory entry for a meta file.
''' </summary>
Public NotInheritable Class MetaFileDirectoryEntry

#Region "Fields (11)"

    Private _creationTime As Date?
    Private ReadOnly _FILE As MetaFile
    Private _lastAccessTime As Date?
    Private _lastWriteTime As Date?
    Private _name As String
    Private _realDir As DirectoryInfo
    Private ReadOnly _XML As XElement
    ''' <summary>
    ''' The name of the attribute for the creation time.
    ''' </summary>
    Public Const CREATION_TIME_ATTRIB_NAME As String = "creationTime"
    ''' <summary>
    ''' The attribute name for a directory name.
    ''' </summary>
    Public Const DIR_ATTRIB_NAME As String = "dir"
    ''' <summary>
    ''' The name of the attribute for the last access time.
    ''' </summary>
    Public Const LAST_ACCESS_TIME_ATTRIB_NAME As String = "lastAccessTime"
    ''' <summary>
    ''' The name of the attribute for the last write time.
    ''' </summary>
    Public Const LAST_WRITE_TIME_ATTRIB_NAME As String = "lastWriteTime"

#End Region

#Region "Constructors (1)"

    ''' <summary>
    ''' Initializes a new instance of the <see cref="MetaFileDirectoryEntry" /> class.
    ''' </summary>
    ''' <param name="file">The underlying meta file.</param>
    ''' <param name="xml">The underlying meta data.</param>
    Sub New(file As MetaFile, xml As XElement)
        Me._FILE = file
        Me._XML = xml

        Me.Init()
    End Sub

#End Region

#Region "Properties (7)"

    ''' <summary>
    ''' Gets the creation time (UTC).
    ''' </summary>
    Public ReadOnly Property CreationTime As Date?
        Get
            Return Me._creationTime
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
    ''' Gets the real local directory.
    ''' </summary>
    Public ReadOnly Property RealDirectory As DirectoryInfo
        Get
            Return Me._realDir
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
    ''' Decrypts the directory to a specific directory.
    ''' </summary>
    ''' <param name="dirPath">The path to the target directory.</param>
    ''' <returns>The decrypted directory.</returns>
    Public Function DecryptTo(dirPath As String) As DirectoryInfo
        Dim dir As DirectoryInfo = New DirectoryInfo(dirPath)
        If Not dir.Exists Then
            dir.Create()
        End If

        Dim result As DirectoryInfo = New DirectoryInfo(Path.Combine(dir.FullName, Me.Name))
        If Not result.Exists Then
            result.Create()
        End If

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

        '' real dir
        Dim dirAttrib As XAttribute = Me.Xml.Attribute(DIR_ATTRIB_NAME)
        If Not dirAttrib Is Nothing Then
            Dim dirName As String = dirAttrib.Value

            If Not String.IsNullOrWhiteSpace(dirName) Then
                Me._realDir = New DirectoryInfo(Path.Combine(Me.File.File.Directory.FullName, _
                                                             dirName.Trim()))
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
    End Sub

#End Region

End Class
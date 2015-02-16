'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO

''' <summary>
''' A directory entry for a meta file.
''' </summary>
Public NotInheritable Class MetaFileDirectoryEntry

#Region "Fields (5)"

    Private ReadOnly _FILE As MetaFile
    Private _name As String
    Private _realDir As DirectoryInfo
    Private ReadOnly _XML As XElement
    ''' <summary>
    ''' The attribute name for a directory name.
    ''' </summary>
    Public Const DIR_ATTRIB_NAME As String = "dir"

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

#Region "Properties (4)"

    ''' <summary>
    ''' Gets the underlying meta file.
    ''' </summary>
    Public ReadOnly Property File As MetaFile
        Get
            Return Me._FILE
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

#Region "Methods (1)"

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
    End Sub

#End Region

End Class
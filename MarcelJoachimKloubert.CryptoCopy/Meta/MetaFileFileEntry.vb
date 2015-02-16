'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO

''' <summary>
''' A file entry for a meta file.
''' </summary>
Public NotInheritable Class MetaFileFileEntry

#Region "Fields (9)"

    Private _crypter As ICrypter
    Private ReadOnly _FILE As MetaFile
    Private _name As String
    Private _realFile As FileInfo
    Private ReadOnly _XML As XElement
    ''' <summary>
    ''' The attribute name for a file name.
    ''' </summary>
    Public Const FILE_ATTRIB_NAME As String = "file"
    ''' <summary>
    ''' The attribute name for iteration count.
    ''' </summary>
    Public Const ITERATIONS_ATTRIB_NAME As String = "iterations"
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

#Region "Properties (5)"

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

#Region "Methods (1)"

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
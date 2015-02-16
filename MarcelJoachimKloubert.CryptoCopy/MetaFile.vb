'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO

''' <summary>
''' Handles a meta file.
''' </summary>
Public NotInheritable Class MetaFile

#Region "Fields (4)"

    Private ReadOnly _CRYPTER As ICrypter
    Private ReadOnly _FILE As FileInfo
    ''' <summary>
    ''' The name of the root element of the meta file.
    ''' </summary>
    Public Const ROOT_ELEMENT_NAME = "dir"
    Private _xml As XElement

#End Region

#Region "Constructors (1)"

    ''' <summary>
    ''' Initializes a new instance of the <see cref="MetaFile" /> class.
    ''' </summary>
    ''' <param name="path">The path of the real file.</param>
    ''' <param name="crypter">The crypter to use.</param>
    Sub New(path As String, crypter As ICrypter)
        Me._CRYPTER = crypter
        Me._FILE = New FileInfo(path)

        Me.Init()
    End Sub

#End Region

#Region "Properties (3)"

    ''' <summary>
    ''' Gets the underlying crypter.
    ''' </summary>
    Public ReadOnly Property Crypter As ICrypter
        Get
            Return Me._CRYPTER
        End Get
    End Property

    ''' <summary>
    ''' Gets the underlying file.
    ''' </summary>
    Public ReadOnly Property File As FileInfo
        Get
            Return Me._FILE
        End Get
    End Property

    ''' <summary>
    ''' Gets the underlying meta data.
    ''' </summary>
    Public ReadOnly Property Xml As XElement
        Get
            Return Me._xml
        End Get
    End Property

#End Region

#Region "Methods (4)"

    Private Sub Init()
        Dim xml As XElement = Nothing

        If Me.File.Exists Then
            Try
                Using cryptedStream As FileStream = Me.File.OpenRead()
                    Using uncryptedStream As MemoryStream = New MemoryStream()
                        Me.Crypter.Decrypt(cryptedStream, uncryptedStream)

                        uncryptedStream.Position = 0
                        xml = XDocument.Load(uncryptedStream).Root
                    End Using
                End Using
            Catch ex As Exception
                xml = Nothing
            End Try
        End If

        If xml Is Nothing Then
            xml = New XElement(ROOT_ELEMENT_NAME)
        End If

        xml.Name = ROOT_ELEMENT_NAME

        Me._xml = xml
    End Sub

    ''' <summary>
    ''' Saves the underlying meta data.
    ''' </summary>
    Public Sub Save()
        Me.File.Refresh()
        If Me.File.Exists Then
            Me.File.Delete()
        End If

        Using uncryptedStream As MemoryStream = New MemoryStream()
            Me.Xml.Save(uncryptedStream)

            uncryptedStream.Position = 0
            Using cryptedStream As FileStream = New FileStream(Me.File.FullName, FileMode.CreateNew, FileAccess.ReadWrite)
                Me.Crypter.Encrypt(uncryptedStream, cryptedStream)
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' Updates the underlying meta data.
    ''' </summary>
    Public Sub Update()
        '' TODO
    End Sub

    ''' <summary>
    ''' Updates and saves the underlying meta data.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateAndSave()
        Me.Update()
        Me.Save()
    End Sub

#End Region

End Class
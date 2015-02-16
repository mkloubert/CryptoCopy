'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO
Imports System.Xml.XPath
Imports System.Security.Cryptography

''' <summary>
''' Handles a meta file.
''' </summary>
Public NotInheritable Class MetaFile

#Region "Fields (10)"

    Private ReadOnly _CRYPTER As ICrypter
    Private ReadOnly _DIRECTORIES As List(Of MetaFileDirectoryEntry)
    Private ReadOnly _FILE As FileInfo
    Private ReadOnly _FILES As List(Of MetaFileFileEntry)
    Private _xml As XElement
    ''' <summary>
    ''' The element name for a directory name.
    ''' </summary>
    Public Const DIR_ELEMENT_NAME As String = "dir"
    ''' <summary>
    ''' The element name for a directory container.
    ''' </summary>
    Public Const DIR_CONTAINER_ELEMENT_NAME As String = "dirs"
    ''' <summary>
    ''' The name of the element for file entries.
    ''' </summary>
    Public Const FILE_CONTAINER_ELEMENT_NAME = "files"
    ''' <summary>
    ''' The name of the element for a file entry.
    ''' </summary>
    Public Const FILE_ELEMENT_NAME = "file"
    ''' <summary>
    ''' The default name of a meta file.
    ''' </summary>
    Public Const FILENAME As String = "0.bin"
    ''' <summary>
    ''' The name of the root element of the meta file.
    ''' </summary>
    Public Const ROOT_ELEMENT_NAME = "dir"

#End Region

#Region "Constructors (1)"

    ''' <summary>
    ''' Initializes a new instance of the <see cref="MetaFile" /> class.
    ''' </summary>
    ''' <param name="dir">The path where the real file is / should be stored.</param>
    ''' <param name="crypter">The crypter to use.</param>
    Sub New(dir As String, crypter As ICrypter)
        Me._CRYPTER = crypter
        Me._FILE = New FileInfo(Path.Combine(dir, FILENAME))

        Me._DIRECTORIES = New List(Of MetaFileDirectoryEntry)()
        Me._FILES = New List(Of MetaFileFileEntry)()

        Me.Init()
    End Sub

#End Region

#Region "Properties (5)"

    ''' <summary>
    ''' Gets the underlying crypter.
    ''' </summary>
    Public ReadOnly Property Crypter As ICrypter
        Get
            Return Me._CRYPTER
        End Get
    End Property

    ''' <summary>
    ''' Gets the list of directories.
    ''' </summary>
    Public ReadOnly Property Directories As List(Of MetaFileDirectoryEntry)
        Get
            Return Me._DIRECTORIES
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

    Public ReadOnly Property Files As List(Of MetaFileFileEntry)
        Get
            Return Me._FILES
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

#Region "Methods (8)"

    ''' <summary>
    ''' Encrypts a directory (NOT its subdirectories!).
    ''' </summary>
    ''' <param name="dirPath">The path of the directory to entry.</param>
    ''' <returns>The new entry.</returns>
    Public Function EncryptDirectory(dirPath As String) As MetaFileDirectoryEntry
        Dim dir As DirectoryInfo = New DirectoryInfo(dirPath)

        Dim destDir = New DirectoryInfo(Me.FindNextDir())
        destDir.Create()

        Dim fileEntriesToDelete As List(Of MetaFileFileEntry) = New List(Of MetaFileFileEntry)()
        Dim metaFile As MetaFile = Nothing
        Dim xml As XElement = Nothing
        Try
            metaFile = New MetaFile(destDir.FullName, Me.Crypter)

            For Each file As FileInfo In dir.GetFiles()
                Dim newFileEntry As MetaFileFileEntry = metaFile.EncryptFile(file.FullName)

                fileEntriesToDelete.Add(newFileEntry)
            Next

            '' //dir/dirs
            Dim dirsElement As XElement = Me.Xml.Element(DIR_CONTAINER_ELEMENT_NAME)
            If dirsElement Is Nothing Then
                dirsElement = New XElement(DIR_CONTAINER_ELEMENT_NAME)

                Me.Xml.Add(dirsElement)
            End If

            xml = New XElement(DIR_ELEMENT_NAME)
            dirsElement.Add(xml)

            '' set data
            xml.SetAttributeValue(MetaFileDirectoryEntry.DIR_ATTRIB_NAME, destDir.Name)
            xml.Value = dir.Name

            Dim newEntry As MetaFileDirectoryEntry = New MetaFileDirectoryEntry(metaFile, xml)

            Me.Directories.Add(newEntry)
            Return newEntry
        Catch ex As Exception
            '' remove XML element
            If Not xml Is Nothing Then
                xml.Remove()
            End If

            '' delete created files
            '' before rethro exception
            For Each file As MetaFileFileEntry In fileEntriesToDelete
                Try
                    file.RealFile.Refresh()
                    If file.RealFile.Exists Then
                        file.RealFile.Delete()
                    End If
                Catch ex2 As Exception
                    '' ignore errors here
                End Try
            Next

            Try
                '' delete meta file
                If Not metaFile Is Nothing Then
                    metaFile.File.Refresh()
                    If metaFile.File.Exists Then
                        metaFile.File.Delete()
                    End If
                End If
            Catch ex2 As Exception
                '' ignore where
            End Try

            '' delete destination directory
            destDir.Refresh()
            If destDir.Exists Then
                destDir.Delete()
            End If

            '' rethrow exception
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Encrypts and adds a file.
    ''' </summary>
    ''' <param name="filePath">The path of the file to encrypt.</param>
    ''' <returns>The new entry.</returns>
    Public Function EncryptFile(filePath As String) As MetaFileFileEntry
        Dim rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider()
        Dim rand As Random = New Random()

        Dim file As FileInfo = New FileInfo(filePath)

        Using srcStream As FileStream = file.OpenRead()
            Dim destFile = New FileInfo(Me.FindNextFile())

            Dim xml As XElement = Nothing
            Try
                Using destStream As FileStream = New FileStream(destFile.FullName, FileMode.CreateNew, FileAccess.ReadWrite)
                    Me.Crypter.Encrypt(srcStream, destStream)
                End Using

                '' //dir/files
                Dim filesElement As XElement = Me.Xml.Element(FILE_CONTAINER_ELEMENT_NAME)
                If filesElement Is Nothing Then
                    filesElement = New XElement(FILE_CONTAINER_ELEMENT_NAME)

                    Me.Xml.Add(filesElement)
                End If

                '' password
                Dim pwd As Byte() = New Byte(63) {}
                rng.GetBytes(pwd)

                '' salt
                Dim salt As Byte() = New Byte(15) {}
                rng.GetBytes(salt)

                '' iterations
                Dim iterations As Integer = rand.Next(1000, 2001)

                xml = New XElement(FILE_ELEMENT_NAME)
                filesElement.Add(xml)

                '' set data
                xml.SetAttributeValue(MetaFileFileEntry.FILE_ATTRIB_NAME, destFile.Name)
                xml.SetAttributeValue(MetaFileFileEntry.ITERATIONS_ATTRIB_NAME, iterations)
                xml.SetAttributeValue(MetaFileFileEntry.PASSWORD_ATTRIB_NAME, Convert.ToBase64String(pwd))
                xml.SetAttributeValue(MetaFileFileEntry.SALT_ATTRIB_NAME, Convert.ToBase64String(salt))
                xml.Value = file.Name

                Dim newEntry As MetaFileFileEntry = New MetaFileFileEntry(Me, xml)

                Me.Files.Add(newEntry)
                Return newEntry
            Catch ex As Exception
                If Not xml Is Nothing Then
                    xml.Remove()
                End If

                destFile.Refresh()
                If destFile.Exists Then
                    destFile.Delete()
                End If

                Throw
            End Try
        End Using
    End Function

    Private Function FindNextDir()
        Dim result As String = Nothing

        For i As ULong = ULong.MinValue To ULong.MaxValue
            Dim dirName As String = i.ToString()
            Dim dir As DirectoryInfo = New DirectoryInfo(Path.Combine(Me.File.Directory.FullName, _
                                                                      dirName))

            If Not dir.Exists Then
                result = dir.FullName
                Exit For
            End If
        Next

        Return result
    End Function

    Private Function FindNextFile() As String
        Dim result As String = Nothing

        For i As ULong = ULong.MinValue To ULong.MaxValue
            Dim fileName As String = String.Format("{0}.bin", i)
            Dim file As FileInfo = New FileInfo(Path.Combine(Me.File.Directory.FullName, _
                                                             fileName))

            If Not file.Exists Then
                result = file.FullName
                Exit For
            End If
        Next

        Return result
    End Function

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

        '' sub directories
        For Each dirElement As XElement In xml.XPathSelectElements(String.Format("{0}/{1}", _
                                                                                 DIR_CONTAINER_ELEMENT_NAME, _
                                                                                 DIR_ELEMENT_NAME))
            Try
                Dim newEntry As MetaFileDirectoryEntry = New MetaFileDirectoryEntry(Me, dirElement)

                If newEntry.RealDirectory.Exists Then
                    Me.Directories.Add(newEntry)
                End If
            Catch ex As Exception
                '' ignore errors here
            End Try
        Next

        '' files
        For Each fileElement As XElement In xml.XPathSelectElements(String.Format("{0}/{1}", _
                                                                                  FILE_CONTAINER_ELEMENT_NAME, _
                                                                                  FILE_ELEMENT_NAME))
            Try
                Dim newEntry As MetaFileFileEntry = New MetaFileFileEntry(Me, fileElement)

                If newEntry.RealFile.Exists Then
                    If Not newEntry.Crypter Is Nothing Then
                        Me.Files.Add(newEntry)
                    End If
                End If
            Catch ex As Exception
                '' ignore errors here
            End Try
        Next

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
'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.IO
Imports System.Security.Cryptography

''' <summary>
''' A Rijndael crypter
''' </summary>
Public NotInheritable Class RijndaelCrypter
    Inherits CrypterBase

#Region "Feilds (3)"

    Private _iterations As Integer
    Private _pwd As Byte()
    Private _salt As Byte()

#End Region

#Region "Constructors (2)"

    ''' <summary>
    ''' Initializes a new instance of the <see cref="RijndaelCrypter" /> class.
    ''' </summary>
    ''' <param name="pwd">The password.</param>
    ''' <param name="salt">The salt.</param>
    ''' <param name="iterations">The iterations.</param>
    Sub New(pwd As Byte(), salt As Byte(), iterations As Integer)
        Me._iterations = iterations
        Me._pwd = pwd
        Me._salt = salt
    End Sub

#End Region

#Region "Methods (2)"

    Private Function CreateCryptoStream(baseStream As Stream, mode As CryptoStreamMode) As CryptoStream
        Dim transform As ICryptoTransform = Nothing

        Using alg As Rijndael = Rijndael.Create()
            Dim db As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(Me._pwd, Me._salt, Me._iterations)
            alg.Key = db.GetBytes(32)
            alg.IV = db.GetBytes(16)

            Select Case mode
                Case CryptoStreamMode.Read
                    transform = alg.CreateDecryptor()
                    Exit Select

                Case CryptoStreamMode.Write
                    transform = alg.CreateEncryptor()
                    Exit Select
            End Select
        End Using

        Return New CryptoStream(baseStream, transform, mode)
    End Function

    ''' <inheriteddoc />
    Public Overrides Sub Decrypt(src As Stream, dest As Stream)
        Dim cryptoStream = Me.CreateCryptoStream(src, CryptoStreamMode.Read)

        cryptoStream.CopyTo(dest)
    End Sub

    ''' <inheriteddoc />
    Public Overrides Sub Encrypt(src As Stream, dest As Stream)
        Dim cryptoStream = Me.CreateCryptoStream(dest, CryptoStreamMode.Write)

        src.CopyTo(cryptoStream)
        cryptoStream.FlushFinalBlock()
    End Sub

#End Region

End Class
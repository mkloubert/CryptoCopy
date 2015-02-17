'' LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt
''
'' s. https://github.com/mkloubert/CryptoCopy

Imports System.Runtime.CompilerServices

Namespace Global.MarcelJoachimKloubert.CryptoCopy.Extensions
    ''' <summary>
    ''' Extension methods
    ''' </summary>
    Module CryptoCopyExtensionMethods

#Region "Methods (2)"

        ''' <summary>
        ''' Shuffles the items inside a generic list.
        ''' </summary>
        ''' <typeparam name="T">Type of the items.</typeparam>
        ''' <param name="list">The list.</param>
        <Extension()>
        Public Sub Shuffle(Of T)(list As IList(Of T))
            Shuffle(list, New Random())
        End Sub

        ''' <summary>
        ''' Shuffles the items inside a generic list.
        ''' </summary>
        ''' <typeparam name="T">Type of the items.</typeparam>
        ''' <param name="list">The list.</param>
        ''' <param name="rand">The randomizer to use.</param>
        <Extension()>
        Public Sub Shuffle(Of T)(list As IList(Of T), rand As Random)
            For i As Integer = 0 To (list.Count - 1)
                Dim newIndex As Integer = rand.Next(0, list.Count)

                Dim temp As T = list(newIndex)
                list(newIndex) = list(i)
                list(i) = temp
            Next
        End Sub

#End Region

    End Module
End Namespace
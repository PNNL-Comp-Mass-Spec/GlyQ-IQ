Option Strict On

' This class can be used to open a Fasta file and return each protein present
'
' -------------------------------------------------------------------------------
' Written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA)
' Program started in January 2004
'
' E-mail: matthew.monroe@pnl.gov or matt@alchemistmatt.com
' Website: http://ncrr.pnl.gov/ or http://www.sysbio.org/resources/staff/
' -------------------------------------------------------------------------------
' 
' Licensed under the Apache License, Version 2.0; you may not use this file except
' in compliance with the License.  You may obtain a copy of the License at 
' http://www.apache.org/licenses/LICENSE-2.0
'
' Notice: This computer software was prepared by Battelle Memorial Institute, 
' hereinafter the Contractor, under Contract No. DE-AC05-76RL0 1830 with the 
' Department of Energy (DOE).  All rights in the computer software are reserved 
' by DOE on behalf of the United States Government and the Contractor as 
' provided in the Contract.  NEITHER THE GOVERNMENT NOR THE CONTRACTOR MAKES ANY 
' WARRANTY, EXPRESS OR IMPLIED, OR ASSUMES ANY LIABILITY FOR THE USE OF THIS 
' SOFTWARE.  This notice including this sentence must appear on any copies of 
' this computer software.

'
' Last modified November 20, 2004

Public Class FastaFileReader
    Inherits ProteinFileReaderBaseClass

    Public Sub New()
        InitializeLocalVariables()
    End Sub


#Region "Constants and Enums"
    Private Const PROTEIN_LINE_START_CHAR As Char = ">"c            ' Each protein description line in the Fasta file should start with a > symbol
    Private Const PROTEIN_LINE_ACCESSION_TERMINATOR As Char = " "c
#End Region

#Region "Classwide Variables"
    Private mProteinLineStartChar As Char
    Private mProteinLineAccessionEndChar As Char

    Private mNextEntry As udtProteinEntryType

#End Region

#Region "Interface Functions"

    Public Overloads ReadOnly Property HeaderLine() As String
        Get
            Return HeaderLine(False)
        End Get
    End Property

    Public Overloads ReadOnly Property HeaderLine(ByVal blnIncludeStartChar As Boolean) As String
        Get
            Try
                If Not blnIncludeStartChar AndAlso mCurrentEntry.HeaderLine.StartsWith(mProteinLineStartChar) Then
                    ' Remove the > character from the start of the line
                    Return mCurrentEntry.HeaderLine.TrimStart(mProteinLineStartChar).Trim
                Else
                    Return mCurrentEntry.HeaderLine
                End If
            Catch ex As Exception
                Return mCurrentEntry.HeaderLine
            End Try
        End Get
    End Property

    Public Property ProteinLineStartChar() As Char
        Get
            Return mProteinLineStartChar
        End Get
        Set(ByVal Value As Char)
            If Not Value = Nothing Then
                mProteinLineStartChar = Value
            End If
        End Set
    End Property

    Public Property ProteinLineAccessionEndChar() As Char
        Get
            Return mProteinLineAccessionEndChar
        End Get
        Set(ByVal Value As Char)
            If Not Value = Nothing Then
                mProteinLineAccessionEndChar = Value
            End If
        End Set
    End Property

#End Region

    Private Function ExtractDescriptionFromHeader(ByVal strHeaderLine As String) As String
        Dim intCharLoc As Integer
        Dim strDescription As String = strHeaderLine

        Try
            If strHeaderLine.StartsWith(mProteinLineStartChar) Then
                ' Remove the > character from the start of the line
                strHeaderLine = strHeaderLine.TrimStart(mProteinLineStartChar).Trim
            End If

            intCharLoc = strHeaderLine.IndexOf(mProteinLineAccessionEndChar)
            If intCharLoc > 0 Then
                strDescription = strHeaderLine.Substring(intCharLoc + 1).Trim
            Else
                strDescription = strHeaderLine
            End If
        Catch ex As Exception
            ' Ignore any errors
        End Try

        Return strDescription
    End Function

    Private Function ExtractAccessionNameFromHeader(ByVal strHeaderLine As String) As String
        ' Note: strHeaderLine should not start with the > character; it should have already been removed when the file was read
        ' Look for mProteinLineAccessionEndChar in strHeaderLine

        Dim intCharLoc As Integer
        Dim strAccessionName As String = strHeaderLine

        Try
            If strHeaderLine.StartsWith(mProteinLineStartChar) Then
                ' Remove the > character from the start of the line
                strHeaderLine = strHeaderLine.TrimStart(mProteinLineStartChar).Trim
            End If

            intCharLoc = strHeaderLine.IndexOf(mProteinLineAccessionEndChar)
            If intCharLoc > 0 Then
                strAccessionName = strHeaderLine.Substring(0, intCharLoc).Trim
            Else
                strAccessionName = strHeaderLine
            End If
        Catch ex As Exception
            ' Ignore any errors
        End Try

        Return strAccessionName

    End Function

    Private Sub InitializeLocalVariables()
        mProteinLineStartChar = PROTEIN_LINE_START_CHAR
        mProteinLineAccessionEndChar = PROTEIN_LINE_ACCESSION_TERMINATOR

        With mNextEntry
            .HeaderLine = String.Empty
            .Name = String.Empty
            .Description = String.Empty
            .Sequence = String.Empty
        End With

    End Sub

    Public Overrides Function ReadNextProteinEntry() As Boolean
        ' Reads the next entry in a Fasta file
        ' Returns true if an entry is found, otherwise, returns false

        Dim strLineIn As String
        Dim blnProteinEntryFound As Boolean

        With mCurrentEntry
            .HeaderLine = String.Empty
            .Name = String.Empty
            .Description = String.Empty
            .Sequence = String.Empty
        End With

        blnProteinEntryFound = False
        If Not mProteinFileInputStream Is Nothing Then

            Try
                Do While Not blnProteinEntryFound And mProteinFileInputStream.Peek() >= 0
                    If Not mNextEntry.HeaderLine Is Nothing AndAlso mNextEntry.HeaderLine.Length > 0 Then
                        strLineIn = mNextEntry.HeaderLine
                    Else
                        strLineIn = mProteinFileInputStream.ReadLine
                        If Not strLineIn Is Nothing AndAlso strLineIn.Trim.Length > 0 Then
                            mFileBytesRead += strLineIn.Length + 2
                            mFileLinesRead += 1
                        End If
                    End If


                    If Not strLineIn Is Nothing AndAlso strLineIn.Trim.Length > 0 Then
                        strLineIn = strLineIn.Trim

                        ' See if strLineIn starts with the protein header start character
                        If strLineIn.StartsWith(mProteinLineStartChar) Then
                            With mCurrentEntry
                                .HeaderLine = strLineIn
                                .Name = ExtractAccessionNameFromHeader(strLineIn)
                                .Description = ExtractDescriptionFromHeader(strLineIn)
                                .Sequence = String.Empty
                            End With
                            blnProteinEntryFound = True

                            ' Now continue reading until the next protein header start character is found
                            Do While mProteinFileInputStream.Peek() >= 0
                                strLineIn = mProteinFileInputStream.ReadLine

                                If Not strLineIn Is Nothing AndAlso strLineIn.Trim.Length > 0 Then
                                    mFileBytesRead += strLineIn.Length + 2
                                    mFileLinesRead += 1

                                    strLineIn = strLineIn.Trim

                                    If strLineIn.StartsWith(mProteinLineStartChar) Then
                                        ' Found the next protein entry
                                        ' Store in mNextEntry and jump out of the loop
                                        mNextEntry.HeaderLine = strLineIn
                                        Exit Do
                                    Else
                                        mCurrentEntry.Sequence &= strLineIn
                                    End If
                                End If
                            Loop

                        End If
                    End If
                Loop

            Catch ex As Exception
                ' Error reading the input file
                ' Ignore any errors
                blnProteinEntryFound = False
            End Try
        End If

        Return blnProteinEntryFound

    End Function

End Class

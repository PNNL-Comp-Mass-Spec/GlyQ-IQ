Option Strict On

' This class can be used to open a Fasta file or delimited text file 
'  with protein names and sequences and return each protein present
'
' -------------------------------------------------------------------------------
' Written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA)
' Program started in October 2004
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

Public MustInherit Class ProteinFileReaderBaseClass

    Public Sub New()
        mClassVersionDate = "July 12, 2005"
        InitializeLocalVariables()
    End Sub

#Region "Structures"
    ' Note that this udt is used both for Proteins and for Peptides
    ' Only Peptides from delimited text files use the Mass and NET fields
    Protected Structure udtProteinEntryType
        Public HeaderLine As String                 ' For fasta files, the header line, including the protein header start character; for Delimited files, the entire line
        Public Name As String                       ' Aka the accession name of the protein
        Public Description As String
        Public Sequence As String
        Public UniqueID As Integer                  ' For delimited text files listing peptides, the UniqueID of the peptide sequence
        Public Mass As Double
        Public NET As Single
        Public NETStDev As Single
        Public DiscriminantScore As Single
    End Structure

#End Region

#Region "Classwide Variables"
    Protected mCurrentEntry As udtProteinEntryType

    Protected mProteinFileInputStream As System.IO.StreamReader

    Private mFileOpen As Boolean
    Protected mFileBytesRead As Long
    Protected mFileLinesRead As Integer

    Private mClassVersionDate As String
#End Region

#Region "Interface Functions"

    Public ReadOnly Property EntryUniqueID() As Integer
        Get
            Return mCurrentEntry.UniqueID
        End Get
    End Property

    Public ReadOnly Property LinesRead() As Integer
        Get
            Return mFileLinesRead
        End Get
    End Property

    Public ReadOnly Property PeptideDiscriminantScore() As Single
        Get
            Return mCurrentEntry.DiscriminantScore
        End Get
    End Property

    Public ReadOnly Property PeptideMass() As Double
        Get
            Return mCurrentEntry.Mass
        End Get
    End Property

    Public ReadOnly Property PeptideNET() As Single
        Get
            Return mCurrentEntry.NET
        End Get
    End Property

    Public ReadOnly Property PeptideNETStDev() As Single
        Get
            Return mCurrentEntry.NETStDev
        End Get
    End Property

    Public ReadOnly Property ProteinName() As String
        Get
            Return mCurrentEntry.Name
        End Get
    End Property

    Public ReadOnly Property ProteinDescription() As String
        Get
            Return mCurrentEntry.Description
        End Get
    End Property

    Public ReadOnly Property ProteinSequence() As String
        Get
            Return mCurrentEntry.Sequence
        End Get
    End Property

    Public Overridable ReadOnly Property HeaderLine() As String
        Get
            Try
                Return mCurrentEntry.HeaderLine
            Catch ex As Exception
                Return mCurrentEntry.HeaderLine
            End Try
        End Get
    End Property

#End Region

    Public Function CloseFile() As Boolean

        Dim blnSuccess As Boolean

        Try
            If Not mProteinFileInputStream Is Nothing Then
                mProteinFileInputStream.Close()
            End If
            mFileOpen = False
            mFileBytesRead = 0
            mFileLinesRead = 0
            blnSuccess = True
        Catch ex As Exception
            blnSuccess = False
        End Try

        Return blnSuccess

    End Function

    Protected Sub EraseProteinEntry(ByRef udtProteinEntry As udtProteinEntryType)
        With udtProteinEntry
            .HeaderLine = String.Empty
            .Name = String.Empty
            .Description = String.Empty
            .Sequence = String.Empty
            .UniqueID = 0
            .Mass = 0
            .NET = 0
            .NETStDev = 0
            .DiscriminantScore = 0
        End With
    End Sub

    Private Sub InitializeLocalVariables()

        EraseProteinEntry(mCurrentEntry)

    End Sub

    Public Overridable Function OpenFile(ByVal strInputFilePath As String) As Boolean
        ' Returns true if the file is successfully opened

        Dim blnSuccess As Boolean
        Dim ioInStream As System.IO.FileStream

        Try
            If CloseFile() Then
                ioInStream = New System.IO.FileStream(strInputFilePath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
                mProteinFileInputStream = New System.IO.StreamReader(ioInStream)
                mFileOpen = True
                mFileBytesRead = 0
                mFileLinesRead = 0
                blnSuccess = True
            End If
        Catch ex As Exception
            blnSuccess = False
        End Try

        Return blnSuccess

    End Function

    Public Function PercentFileProcessed() As Single

        If mFileOpen Then

            If mProteinFileInputStream.BaseStream.Length > 0 Then
                Return CType(Math.Round(mFileBytesRead / mProteinFileInputStream.BaseStream.Length * 100, 2), Single)
            Else
                Return 0
            End If

        Else
            Return 0
        End If
    End Function

    Public MustOverride Function ReadNextProteinEntry() As Boolean

    Protected Overrides Sub Finalize()
        If mFileOpen Then CloseFile()
        MyBase.Finalize()
    End Sub

End Class

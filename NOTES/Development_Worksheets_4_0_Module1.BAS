Attribute VB_Name = "Module1"
Option Explicit
'   ============================================================================
'   Module Name:        Module1
'
'   Module Purpose:     This module organizes two functions that wrap around my
'                       robust CSV string parsing object, along with a handful
'                       of utility functions that I devised to help with testing
'                       the library interface.
'
'   Module Author:      David A. Gray
'
'   ----------------------------------------------------------------------------
'   Module Revision HIstory
'   ----------------------------------------------------------------------------
'
'   Date       By  Comment
'   ---------- --- -------------------------------------------------------------
'   2016/10/16 DAG Module completed.
'   ============================================================================

    Const ARRAY_DFLT_LBOUND As Long = 0
    Const ARRAY_INVALID_SUBSCRIPT As Long = -1
    Const FIRST_ITEM As Long = 1
    Const LASTDLLERROR_NONE As Integer = 0
    Const LENGTH_OF_EMPTY_STRING As Integer = 0
    Const MIN_VARTYPE As Integer = 0
    Const NO_SUBSTRINGS As Long = 0
    Const RNG_ORIGIN_INDEX As Long = 1

    '   ------------------------------------------------------------------------
    '   The first call to function DisplayVariantSubtype initializes these three
    '   module level variables, saving a few CPU cycles on subsequent calls.
    '   ------------------------------------------------------------------------

    Dim m_intMinVarType, m_intMaxVarType As Integer
    Dim m_lngSubtypeCol As Long
Public Sub AnyCSVDemo1()

    '   ----------------------------------------------------------------------------------------------------
    '   C:\Users\DAVE\Documents\Visual Studio 2013\Projects\WizardWrx_Libs\AnyCSV\NOTES\CSV_Test_Strings.TXT
    '   ----------------------------------------------------------------------------------------------------

    Const CSV_TEST_STRINGS_FILENAME As String = "CSV_Test_Strings.TXT"

    On Error GoTo AnyCSVDemo1_Err

    Dim wbThis As Workbook: Set wbThis = ActiveWorkbook
    Dim strWBAbsoluteFileName As String: strWBAbsoluteFileName = wbThis.FullName
    Dim strWBAbsoluteDirName As String: strWBAbsoluteDirName = GetFolder(strWBAbsoluteFileName)
    Dim strTestStringFileName As String: strTestStringFileName = AddBackslash(strWBAbsoluteDirName) & CSV_TEST_STRINGS_FILENAME

    Debug.Print "strWBAbsoluteFileName    = " & strWBAbsoluteFileName
    Debug.Print "strWBAbsoluteDirName     = " & strWBAbsoluteDirName
    Debug.Print "strTestStringFileName    = " & strTestStringFileName

    Dim objMyFSO As Scripting.FileSystemObject: Set objMyFSO = New Scripting.FileSystemObject

    If objMyFSO.FileExists(strTestStringFileName) Then
        Dim objInputStream As TextStream: Set objInputStream = objMyFSO.OpenTextFile(strTestStringFileName, _
                                                                                     ForReading, _
                                                                                     False)
        Dim lngCounter As Long

        Dim objCSVParser As WizardWrx_AnyCSV.CSVParseEngine
        Set objCSVParser = New WizardWrx_AnyCSV.CSVParseEngine

        Debug.Print "objCSVParser Properties (Ante):" & vbLf

        Dim enmGuardDisp As GuardDisposition: enmGuardDisp = objCSVParser.GuardCharDisposition
        Dim enmLockState As LockState: enmLockState = objCSVParser.SettingsLocked

        Debug.Print "    DelimiterGuard       = " & Chr(CLng(objCSVParser.DelimiterGuard)) & " (" & objCSVParser.DelimiterGuard & " decimal)"
        Debug.Print "    FieldDelimiter       = " & Chr(CLng(objCSVParser.FieldDelimiter)) & " (" & objCSVParser.FieldDelimiter & " decimal)"
        Debug.Print "    GuardCharDisposition = " & enmGuardDisp & " (" & CInt(objCSVParser.GuardCharDisposition) & " decimal)"
        Debug.Print "    SettingsLocked       = " & enmLockState & " (" & CInt(objCSVParser.SettingsLocked) & " decimal)"

        Debug.Print vbLf & "End of objCSVParser Properties Enumeration" & vbLf

        Do  ' Loop Until objInputStream.AtEndOfStream
            Dim strInputString As String: strInputString = objInputStream.ReadLine
            Dim varSubstrings As Variant: varSubstrings = objCSVParser.Parse(strInputString)
            Dim lngNSubStrings As Long: lngNSubStrings = UBound(varSubstrings)
            lngCounter = lngCounter + 1

            If lngCounter = FIRST_ITEM Then
                Debug.Print "Test String " & lngCounter & " = " & strInputString & vbTab & "Substring Count = " & OrdinalFromSubscript(lngNSubStrings) & ", as follows:" & vbLf
            Else
                Debug.Print vbLf & "Test String " & lngCounter & " = " & strInputString & vbTab & "Substring Count = " & OrdinalFromSubscript(lngNSubStrings) & ", as follows:" & vbLf
            End If  ' If lngCounter = FIRST_ITEM Then

            Dim lngCurrSubStr As Long

            For lngCurrSubStr = ARRAY_DFLT_LBOUND To lngNSubStrings
                Debug.Print "            # " & OrdinalFromSubscript(lngCurrSubStr) & ": " & varSubstrings(lngCurrSubStr)
            Next    ' For lngCurrSubStr = ARRAY_DFLT_LBOUND To lngNSubStrings
        Loop Until objInputStream.AtEndOfStream

        objInputStream.Close

        MsgBox lngCounter & " records read from input file " & strTestStringFileName & ".", _
               vbExclamation, _
               strWBAbsoluteFileName
    Else
        MsgBox "Input file " & strTestStringFileName & " cannot be found.", _
               vbExclamation, _
               strWBAbsoluteFileName
    End If  ' If objMyFSO.FileExists(strTestStringFileName) Then

AnyCSVDemo1_End:

    Exit Sub

AnyCSVDemo1_Err:

    If Err.LastDllError = LASTDLLERROR_NONE Then
        MsgBox "Custom Subroutine AnyCSVDemo1 encountered an error." & vbLf & vbLf _
                    & "Error 0x" & Hex(Err.Number) & " (" & Err.Number & " decimal): " & Err.Description & vbLf _
                    & "Error Source: " & Err.Source, _
               vbInformation Or vbApplicationModal, _
               ActiveWorkbook.FullName
    Else
        MsgBox "Custom Subroutine AnyCSVDemo1 encountered an error." & vbLf & vbLf _
                    & "Error 0x" & Hex(Err.Number) & " (" & Err.Number & " decimal): " & Err.Description & vbLf _
                    & "Last DLL Error = " & Hex(Err.LastDllError) & "(" & Err.LastDllError & " decimal)" & vbLf _
                    & "Error Source: " & Err.Source, _
               vbInformation Or vbApplicationModal, _
               ActiveWorkbook.FullName
    End If  ' If Err.LastDllError = LASTDLLERROR_NONE Then

    Err.Clear
    Resume AnyCSVDemo1_End

End Sub ' AnyCSVDemo1


Public Function ParseAnyCSVExtractSubstring( _
    ByRef pstrString As String, _
    ByVal plngSubstringNumber As Long, _
    Optional ByVal penmDelimiterChar As DelimiterChar = DelimiterChar.DelimiterChar_Comma, _
    Optional ByVal penmGuardChar As GuardChar = GuardChar.GuardChar_DoubleQuote, _
    Optional ByVal penmGuardDisposition As GuardDisposition = GuardDisposition.GuardDisposition_Strip, _
    Optional ByVal penmTrimWhiteSpace As TrimWhiteSpace = TrimWhiteSpace.TrimWhiteSpace_Leave _
        ) As String

    '   ------------------------------------------------------------------------
    '   Function Name:  ParseAnyCSVExtractSubstring
    '
    '   Purpose:        Count substrings in a delimited string.
    '
    '   Arguments:      pstrString              = String, one of two required
    '                                             arguments, the delimited string
    '                   plngSubstringNumber     = Ordinal of desired subsstring
    '                                             to split and count
    '                   penmDelimiterChar       = DelimiterChar, defaults to
    '                                             DelimiterChar.DelimiterChar_Comma
    '                   penmGuardChar           = GuardChar, defaults to
    '                                             GuardChar.GuardChar_DoubleQuote
    '                   penmGuardDisposition    = GuardDisposition, defaults to
    '                                             GuardDisposition.GuardDisposition_Strip
    '                   penmTrimWhiteSpace      = TrimWhiteSpace, defaults to
    '                                             TrimWhiteSpace.TrimWhiteSpace_Leave
    '
    '   Returns:        Unless argument pstrString is the empty string or
    '                   plngSubstringNumber is greater than the number of
    '                   substrings contained in pstrString, the return value is
    '                   the substring that occupies the specified ordinal
    '                   position in the string.
    '   ------------------------------------------------------------------------

    If Len(pstrString) > LENGTH_OF_EMPTY_STRING Then
        On Error GoTo ParseAnyCSVExtractSubstring_Err
        Dim objCSVParser As WizardWrx_AnyCSV.CSVParseEngine
        Set objCSVParser = StartEngine(penmDelimiterChar, _
                                       penmGuardChar, _
                                       penmGuardDisposition, _
                                       penmTrimWhiteSpace)
        Dim varSubstrings As Variant: varSubstrings = objCSVParser.Parse(pstrString)
        Dim lngNSubStrings As Long: lngNSubStrings = OrdinalFromSubscript(UBound(varSubstrings))
        Dim lngDesiredSubstr As Long: lngDesiredSubstr = SubscriptFromOrdinal(plngSubstringNumber)

        If lngDesiredSubstr < lngNSubStrings Then
            ParseAnyCSVExtractSubstring = varSubstrings(lngDesiredSubstr)
        Else
            ParseAnyCSVExtractSubstring = vbNullString
        End If  ' If lngDesiredSubstr > lngNSubStrings Then
    Else
        ParseAnyCSVExtractSubstring = vbNullString
    End If  ' If Len(pstrString) > LENGTH_OF_EMPTY_STRING Then

ParseAnyCSVExtractSubstring_End:

    Exit Function

ParseAnyCSVExtractSubstring_Err:

    If Err.LastDllError = LASTDLLERROR_NONE Then
        MsgBox "Custom VBA function ParseAnyCSVExtractSubstring encountered an error." & vbLf & vbLf _
                    & "Error 0x" & Hex(Err.Number) & " (" & Err.Number & " decimal): " & Err.Description & vbLf _
                    & "Error Source: " & Err.Source, _
               vbInformation Or vbApplicationModal, _
               ActiveWorkbook.FullName
    Else
        MsgBox "Custom VBA function ParseAnyCSVExtractSubstring encountered an error." & vbLf & vbLf _
                    & "Error 0x" & Hex(Err.Number) & " (" & Err.Number & " decimal): " & Err.Description & vbLf _
                    & "Last DLL Error = " & Hex(Err.LastDllError) & "(" & Err.LastDllError & " decimal)" & vbLf _
                    & "Error Source: " & Err.Source, _
               vbInformation Or vbApplicationModal, _
               ActiveWorkbook.FullName
    End If  ' If Err.LastDllError = LASTDLLERROR_NONE Then

    Err.Clear
    Resume ParseAnyCSVExtractSubstring_End

End Function    ' ParseAnyCSVExtractSubstring


Public Function ParseAnyCSVGetSubstringCount( _
    ByRef pstrString As String, _
    Optional ByVal penmDelimiterChar As DelimiterChar = DelimiterChar.DelimiterChar_Comma, _
    Optional ByVal penmGuardChar As GuardChar = GuardChar.GuardChar_DoubleQuote, _
    Optional ByVal penmGuardDisposition As GuardDisposition = GuardDisposition.GuardDisposition_Strip, _
    Optional ByVal penmTrimWhiteSpace As TrimWhiteSpace = TrimWhiteSpace.TrimWhiteSpace_Leave _
        ) As Long

    '   ------------------------------------------------------------------------
    '   Function Name:  ParseAnyCSVGetSubstringCount
    '
    '   Purpose:        Count substrings in a delimited string.
    '
    '   Arguments:      pstrString              = String, the only required
    '                                             argument, the delimited string
    '                                             to split and count
    '                   penmDelimiterChar       = DelimiterChar, defaults to
    '                                             DelimiterChar.DelimiterChar_Comma
    '                   penmGuardChar           = GuardChar, defaults to
    '                                             GuardChar.GuardChar_DoubleQuote
    '                   penmGuardDisposition    = GuardDisposition, defaults to
    '                                             GuardDisposition.GuardDisposition_Strip
    '                   penmTrimWhiteSpace      = TrimWhiteSpace, defaults to
    '                                             TrimWhiteSpace.TrimWhiteSpace_Leave
    '
    '   Returns:        Long integer count of substrings in string pstrString,
    '                   unless pstrString is the empty string, in which case the
    '                   return value is zero.
    '   ------------------------------------------------------------------------

    If Len(pstrString) > LENGTH_OF_EMPTY_STRING Then
        On Error GoTo ParseAnyCSVGetSubstringCount_Err
        Dim objCSVParser As WizardWrx_AnyCSV.CSVParseEngine
        Set objCSVParser = StartEngine(penmDelimiterChar, _
                                       penmGuardChar, _
                                       penmGuardDisposition, _
                                       penmTrimWhiteSpace)
        Dim varSubstrings As Variant: varSubstrings = objCSVParser.Parse(pstrString)
        ParseAnyCSVGetSubstringCount = OrdinalFromSubscript(UBound(varSubstrings))
    Else
        ParseAnyCSVGetSubstringCount = NO_SUBSTRINGS
    End If  ' If Len(pstrString) > LENGTH_OF_EMPTY_STRING Then

ParseAnyCSVGetSubstringCount_End:

    Exit Function

ParseAnyCSVGetSubstringCount_Err:

    If Err.LastDllError = LASTDLLERROR_NONE Then
        MsgBox "Custom VBA function ParseAnyCSVGetSubstringCount encountered an error." & vbLf & vbLf _
                    & "Error 0x" & Hex(Err.Number) & " (" & Err.Number & " decimal): " & Err.Description & vbLf _
                    & "Error Source: " & Err.Source, _
               vbInformation Or vbApplicationModal, _
               ActiveWorkbook.FullName
    Else
        MsgBox "Custom VBA function ParseAnyCSVGetSubstringCount encountered an error." & vbLf & vbLf _
                    & "Error 0x" & Hex(Err.Number) & " (" & Err.Number & " decimal): " & Err.Description & vbLf _
                    & "Last DLL Error = " & Hex(Err.LastDllError) & "(" & Err.LastDllError & " decimal)" & vbLf _
                    & "Error Source: " & Err.Source, _
               vbInformation Or vbApplicationModal, _
               ActiveWorkbook.FullName
    End If  ' If Err.LastDllError = LASTDLLERROR_NONE Then

    Err.Clear
    Resume ParseAnyCSVGetSubstringCount_End

End Function    ' ParseAnyCSVGetSubstringCount


Public Function DisplayVariantSubtype(ByVal pintVariantSubType As Integer) As String

    '   ------------------------------------------------------------------------
    '   Function Name:  DisplayVariantSubtype
    '
    '   Purpose:        Return a string representation of the components of a
    '                   specified Variant Type (the value returned by calling
    '                   the intrinsic VarType function with a variable).
    '
    '   Arguments:      pintVariantSubType  = Complete variant type to evaluate
    '
    '   Returns:        String containing one or more variant subtype names,
    '                   separated by comma and space if there are two or more,
    '                   otherwise, a string that begins with "Custom Function
    '                   DisplayVariantSubtype encountered an error," followed by
    '                   a detailed error message, is returned.
    '   ------------------------------------------------------------------------

    Const DLM_FOR_DSP As String = ", "
    Const LOOKUP_EXACT_MATCH As Boolean = False
    Const LOOKUP_INDEX_COLNBR As Integer = 1
    Const MATCH_EXACTLY As Integer = 0

    Const VSTBL_DTLS_RNG_NM As String = "VariantSubtypeConstants_ByDecValue_Details"
    Const VSTBL_LBLS_RNG_NM As String = "VariantSubtypeConstants_ByDecValue_Labels"
    Const VSTBL_CNST_NM As String = "VBA Constant Name"
    Const VSTP_OUT_OF_RANGE = "ERROR: Variant subtype value must be greater than zero."

    On Error GoTo DisplayVariantSubtype_Err

    Dim rngVarTypeInfo As Range
    Set rngVarTypeInfo = ActiveWorkbook.Names(VSTBL_DTLS_RNG_NM).RefersToRange
    Dim lngCurrRow, lngLastRow As Long

    lngLastRow = rngVarTypeInfo.Rows.Count

    '   ------------------------------------------------------------------------
    '   Initialize module variables m_intMaxVarType and m_intMinVarType on the
    '   first call.
    '   ------------------------------------------------------------------------

    If m_intMaxVarType = MIN_VARTYPE Then
        m_intMinVarType = MIN_VARTYPE
        m_lngSubtypeCol = Application.Match(VSTBL_CNST_NM, _
                                            ActiveWorkbook.Names(VSTBL_LBLS_RNG_NM).RefersToRange, _
                                            MATCH_EXACTLY)

        For lngCurrRow = RNG_ORIGIN_INDEX To lngLastRow
            m_intMaxVarType = m_intMaxVarType + rngVarTypeInfo.Cells(lngCurrRow, RNG_ORIGIN_INDEX)
        Next    ' For lngCurrRow = RNG_ORIGIN_INDEX To lngLastRow
    End If  ' If m_intMaxVarType = MIN_VARTYPE Then

    '   ------------------------------------------------------------------------
    '   If function argument pintVariantSubType passes the smell test, construct
    '   a delimited string of variant subtype literals. Otherwise, return an
    '   error message.
    '   ------------------------------------------------------------------------

    If pintVariantSubType >= m_intMinVarType And pintVariantSubType <= m_intMaxVarType Then
        Dim strSubtypeList As String: strSubtypeList = vbNullString

        For lngCurrRow = RNG_ORIGIN_INDEX To lngLastRow
            Dim intCurrVarType As Integer: intCurrVarType = rngVarTypeInfo.Cells(lngCurrRow, RNG_ORIGIN_INDEX)

            If IsSubType(pintVariantSubType, intCurrVarType) Then
                Dim strCurrSubType As String: strCurrSubType = CStr(Application.VLookup(intCurrVarType, _
                                                                                        rngVarTypeInfo, _
                                                                                        m_lngSubtypeCol, _
                                                                                        LOOKUP_EXACT_MATCH))

                If Len(strSubtypeList) = LENGTH_OF_EMPTY_STRING Then
                    strSubtypeList = strCurrSubType
                Else
                    strSubtypeList = strSubtypeList _
                                     & DLM_FOR_DSP _
                                     & strCurrSubType
                End If  ' If Len(strSubtypeList) = LENGTH_OF_EMPTY_STRING Then
            End If  ' If ((pintVariantSubType = MIN_VARTYPE) And (intCurrVarType = MIN_VARTYPE)) Or ((pintVariantSubType > MIN_VARTYPE) And (pintVariantSubType And intCurrVarType = intCurrVarType)) Then
        Next    ' For lngCurrRow = RNG_ORIGIN_INDEX To lngLastRow

        DisplayVariantSubtype = strSubtypeList
    Else
        DisplayVariantSubtype = VSTP_OUT_OF_RANGE
    End If  ' If pintVariantSubType >= m_intMinVarType And pintVariantSubType <= m_intMaxVarType Then

DisplayVariantSubtype_End:

    Exit Function

DisplayVariantSubtype_Err:

    If Err.LastDllError = LASTDLLERROR_NONE Then
        DisplayVariantSubtype = "Custom Function DisplayVariantSubtype encountered an error." & vbLf & vbLf _
                                 & "Error 0x" & Hex(Err.Number) _
                                 & " (" & Err.Number & " decimal): " _
                                 & Err.Description
    Else
        DisplayVariantSubtype = "Custom Function DisplayVariantSubtype encountered an error." & vbLf & vbLf _
                                & "Error 0x" & Hex(Err.Number) _
                                & " (" & Err.Number & " decimal): " _
                                & Err.Description & vbLf _
                                & "Last DLL Error = " _
                                & Hex(Err.LastDllError) _
                                & "(" & Err.LastDllError & " decimal)"
    End If  ' If Err.LastDllError = LASTDLLERROR_NONE Then

    Err.Clear
    Resume DisplayVariantSubtype_End

End Function    ' DisplayVariantSubtype


Public Function OrdinalFromSubscript(ByVal plngSubscript As Long) As Long
    '   ------------------------------------------------------------------------
    '   Function Name:  OrdinalFromSubscript
    '
    '   Purpose:        Return the ordinal value that corresponds to an array
    '                   subscript.
    '
    '   Arguments:      plngSubscript       = Subscript to evaluate
    '
    '   Returns:        The return value is the ordinal that corresponds to the
    '                   specified subscript, UNLESS input plngSubscript is less
    '                   than zero, in which case, the return value is -1.
    '   ------------------------------------------------------------------------

    Const ARRAY_SUBSCR_TO_ORD As Long = 1

    If plngSubscript < ARRAY_DFLT_LBOUND Then
        OrdinalFromSubscript = ARRAY_INVALID_SUBSCRIPT
    Else
        OrdinalFromSubscript = plngSubscript + ARRAY_SUBSCR_TO_ORD
    End If  ' If plngSubscript < ARRAY_DFLT_LBOUND Then

End Function    ' OrdinalFromSubscript


Private Function IsSubType(pintVariantSubType, pintCurrVarType) As Boolean
    '   ------------------------------------------------------------------------
    '   Function Name:  IsSubType
    '
    '   Purpose:        Determine whether to add the current variant subtype to
    '                   the list of subtypes mapped into a Variant Type.
    '
    '   Arguments:      pintVariantSubType  = Complete variant type to evaluate
    '                   pintCurrVarType     = Variant subtype to test
    '
    '   Returns:        True if pintVariantSubType contains pintCurrVarType or
    '                   pintVariantSubType is equal to MIN_VARTYPE (zero), else
    '                   False
    '   ------------------------------------------------------------------------

    If pintVariantSubType = MIN_VARTYPE Then
        IsSubType = True        ' Degenerate case 1 of 2
    Else
        If pintCurrVarType = MIN_VARTYPE Then
            IsSubType = False   ' Degenerate case 2 of 2
        Else
            IsSubType = (pintVariantSubType And pintCurrVarType) = pintCurrVarType
        End If  ' If pintCurrVarType = MIN_VARTYPE Then
    End If  ' If pintVariantSubType = MIN_VARTYPE Then

End Function    ' IsSubType


Public Function SubscriptFromOrdinal(ByVal plngOrdinal As Long) As Long
    '   ------------------------------------------------------------------------
    '   Function Name:  SubscriptFromOrdinal
    '
    '   Purpose:        Return the subscript value that corresponds to an array
    '                   ordinal.
    '
    '   Arguments:      plngOrdinal       = Ordinal to evaluate
    '
    '   Returns:        The return value is the subscript that corresponds to
    '                   the specified ordinal, UNLESS input plngOrdinal is less
    '                   than one, in which case, the return value is -1.
    '   ------------------------------------------------------------------------

    Const ARRAY_SUBSCR_TO_ORD As Long = 1

    If plngOrdinal < (ARRAY_DFLT_LBOUND + ARRAY_SUBSCR_TO_ORD) Then
        SubscriptFromOrdinal = ARRAY_INVALID_SUBSCRIPT
    Else
        SubscriptFromOrdinal = plngOrdinal - ARRAY_SUBSCR_TO_ORD
    End If  ' If plngOrdinal < ARRAY_DFLT_LBOUND Then

End Function    ' SubscriptFromOrdinal


Private Function StartEngine(Optional ByVal penmDelimiterChar As DelimiterChar = DelimiterChar.DelimiterChar_Comma, _
                             Optional ByVal penmGuardChar As GuardChar = GuardChar.GuardChar_DoubleQuote, _
                             Optional ByVal penmGuardDisposition As GuardDisposition = GuardDisposition.GuardDisposition_Strip, _
                             Optional ByVal penmTrimWhiteSpace As TrimWhiteSpace = TrimWhiteSpace.TrimWhiteSpace_Leave _
                                ) As WizardWrx_AnyCSV.CSVParseEngine

    Dim robjCSVParser As WizardWrx_AnyCSV.CSVParseEngine
    Set robjCSVParser = New WizardWrx_AnyCSV.CSVParseEngine

    If penmDelimiterChar <> DelimiterChar_Comma Then
        robjCSVParser.DelimiterGuard = penmDelimiterChar
    End If  ' If penmDelimiterChar <> DelimiterChar_Comma Then

    If penmGuardChar <> GuardChar_DoubleQuote Then
        robjCSVParser.FieldDelimiter = penmGuardChar
    End If  ' If penmGuardChar <> GuardChar_DoubleQuote Then

    If penmGuardDisposition <> GuardDisposition.GuardDisposition_Strip Then
        robjCSVParser.GuardCharDisposition = penmGuardDisposition
    End If  ' If penmGuardDisposition <> GuardDisposition.GuardDisposition_Strip Then

    If penmTrimWhiteSpace <> TrimWhiteSpace.TrimWhiteSpace_Leave Then
        robjCSVParser.WhiteSpaceDisposition = penmTrimWhiteSpace
    End If  ' If penmTrimWhiteSpace <> TrimWhiteSpace.TrimWhiteSpace_Leave Then

    Set StartEngine = robjCSVParser

End Function    ' StartEngine

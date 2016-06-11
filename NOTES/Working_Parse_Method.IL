.method public hidebysig static string[] 
        Parse(string pstrAnyCSV,
              char pchrDelimiter,
              char pchrProtector,
              valuetype WizardWrx.AnyCSV.Parser/GuardDisposition penmGuardDisposition,
              valuetype WizardWrx.AnyCSV.Parser/TrimWhiteSpace penmTrimWhiteSpace) cil managed
{
  // Code size       262 (0x106)
  .maxstack  5
  .locals init ([0] class [mscorlib]System.Collections.Generic.List`1<string> lstSubstrings,
           [1] bool fInProgress,
           [2] bool fProtectDelimiters,
           [3] int32 intTotalChars,
           [4] class [mscorlib]System.Text.StringBuilder sbCurrentField,
           [5] int32 intCurrChar)
  IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<string>::.ctor()
  IL_0005:  stloc.0
  IL_0006:  ldarg.0
  IL_0007:  call       bool [mscorlib]System.String::IsNullOrEmpty(string)
  IL_000c:  brfalse.s  IL_001e
  IL_000e:  ldloc.0
  IL_000f:  ldsfld     string [mscorlib]System.String::Empty
  IL_0014:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
  IL_0019:  br         IL_00ff
  IL_001e:  ldc.i4.0
  IL_001f:  stloc.1
  IL_0020:  ldc.i4.0
  IL_0021:  stloc.2
  IL_0022:  ldarg.0
  IL_0023:  callvirt   instance int32 [mscorlib]System.String::get_Length()
  IL_0028:  stloc.3
  IL_0029:  ldloc.3
  IL_002a:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor(int32)
  IL_002f:  stloc.s    sbCurrentField
  IL_0031:  ldc.i4.0
  IL_0032:  stloc.s    intCurrChar
  IL_0034:  br         IL_00ca
  IL_0039:  ldarg.0
  IL_003a:  ldloc.s    intCurrChar
  IL_003c:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
  IL_0041:  ldarg.1
  IL_0042:  bne.un.s   IL_0090
  IL_0044:  ldloc.1
  IL_0045:  brfalse.s  IL_0083
  IL_0047:  ldloc.2
  IL_0048:  brfalse.s  IL_005c
  IL_004a:  ldloc.s    sbCurrentField
  IL_004c:  ldarg.0
  IL_004d:  ldloc.s    intCurrChar
  IL_004f:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
  IL_0054:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(char)
  IL_0059:  pop
  IL_005a:  br.s       IL_00c4
  IL_005c:  ldloc.0
  IL_005d:  ldloc.s    sbCurrentField
  IL_005f:  ldarg.2
  IL_0060:  ldarg.3
  IL_0061:  ldarg.s    penmTrimWhiteSpace
  IL_0063:  call       string WizardWrx.AnyCSV.Parser::TransformAsDirectoed(class [mscorlib]System.Text.StringBuilder,
                                                                            char,
                                                                            valuetype WizardWrx.AnyCSV.Parser/GuardDisposition,
                                                                            valuetype WizardWrx.AnyCSV.Parser/TrimWhiteSpace)
  IL_0068:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
  IL_006d:  ldloc.s    sbCurrentField
  IL_006f:  ldc.i4.0
  IL_0070:  ldloc.s    sbCurrentField
  IL_0072:  callvirt   instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
  IL_0077:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Remove(int32,
                                                                                                                      int32)
  IL_007c:  pop
  IL_007d:  ldc.i4.0
  IL_007e:  stloc.1
  IL_007f:  ldc.i4.0
  IL_0080:  stloc.2
  IL_0081:  br.s       IL_00c4
  IL_0083:  ldloc.0
  IL_0084:  ldsfld     string [mscorlib]System.String::Empty
  IL_0089:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
  IL_008e:  br.s       IL_00c4
  IL_0090:  ldarg.0
  IL_0091:  ldloc.s    intCurrChar
  IL_0093:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
  IL_0098:  ldarg.2
  IL_0099:  bne.un.s   IL_00b2
  IL_009b:  ldloc.s    sbCurrentField
  IL_009d:  ldarg.0
  IL_009e:  ldloc.s    intCurrChar
  IL_00a0:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
  IL_00a5:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(char)
  IL_00aa:  pop
  IL_00ab:  ldloc.2
  IL_00ac:  ldc.i4.0
  IL_00ad:  ceq
  IL_00af:  stloc.2
  IL_00b0:  br.s       IL_00c4
  IL_00b2:  ldc.i4.1
  IL_00b3:  stloc.1
  IL_00b4:  ldloc.s    sbCurrentField
  IL_00b6:  ldarg.0
  IL_00b7:  ldloc.s    intCurrChar
  IL_00b9:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
  IL_00be:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(char)
  IL_00c3:  pop
  IL_00c4:  ldloc.s    intCurrChar
  IL_00c6:  ldc.i4.1
  IL_00c7:  add
  IL_00c8:  stloc.s    intCurrChar
  IL_00ca:  ldloc.s    intCurrChar
  IL_00cc:  ldloc.3
  IL_00cd:  blt        IL_0039
  IL_00d2:  ldloc.1
  IL_00d3:  brfalse.s  IL_00e8
  IL_00d5:  ldloc.0
  IL_00d6:  ldloc.s    sbCurrentField
  IL_00d8:  ldarg.2
  IL_00d9:  ldarg.3
  IL_00da:  ldarg.s    penmTrimWhiteSpace
  IL_00dc:  call       string WizardWrx.AnyCSV.Parser::TransformAsDirectoed(class [mscorlib]System.Text.StringBuilder,
                                                                            char,
                                                                            valuetype WizardWrx.AnyCSV.Parser/GuardDisposition,
                                                                            valuetype WizardWrx.AnyCSV.Parser/TrimWhiteSpace)
  IL_00e1:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
  IL_00e6:  br.s       IL_00ff
  IL_00e8:  ldarg.0
  IL_00e9:  ldloc.3
  IL_00ea:  ldc.i4.1
  IL_00eb:  sub
  IL_00ec:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
  IL_00f1:  ldarg.1
  IL_00f2:  bne.un.s   IL_00ff
  IL_00f4:  ldloc.0
  IL_00f5:  ldsfld     string [mscorlib]System.String::Empty
  IL_00fa:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
  IL_00ff:  ldloc.0
  IL_0100:  callvirt   instance !0[] class [mscorlib]System.Collections.Generic.List`1<string>::ToArray()
  IL_0105:  ret
} // end of method Parser::Parse


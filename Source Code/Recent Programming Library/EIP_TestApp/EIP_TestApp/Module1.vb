Module Module1

    Sub Main()
        'Dim eipHandler As New LIB_EIP
        Dim sfmHandler As New LIB_Deprag.SFM_Module
        Dim sfmResults As New LIB_Deprag.SFM_Module.SFM_DepragResults

        sfmHandler.SFM_RunProgram(LIB_Deprag.SFM_Module.SFM_Command.Program, 1)
        sfmResults = sfmHandler.SFM_ReadInputs()
        sfmResults = sfmHandler.SFM_ReadInputs()
        sfmHandler.SFM_RunProgram(LIB_Deprag.SFM_Module.SFM_Command.Program, 0)

        'eipHandler.TestFunction()
    End Sub

End Module

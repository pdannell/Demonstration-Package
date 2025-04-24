'Module that handles and updates updating and redrawing all text use within the program.
Module Func_Language

#Region "Global Text Variable Definitions"
    'MainUI Text
    Public TXT_UI_Start, TXT_UI_Stop As String

    'Robot Jokes Text
    Public TXT_Robot_Hello, TXT_Robot_Bye, TXT_Robot_Anger,
        TXT_Robot_Bench, TXT_Robot_Impatient, TXT_Robot_Showoff,
        TXT_Robot_Doctor, TXT_Robot_Reps As String

    Public TXT_WI_RemovePart, TXT_WI_LoadPart, TXT_WI_PressStart,
        TXT_WI_Wait, TXT_WI_FollowHMI, TXT_WI_Unlock,
        TXT_WI_AddTop, TXT_WI_Pass, TXT_WI_Fail As String
#End Region

    'Public sub that when called sets all text variables to the requested language.
    Public Sub SetLanguage()

        Select Case REY_PressMate.Settings.Settings_User.Default.Language
            Case "English"  'Set all  variables to English.

                'WI Text
                TXT_WI_RemovePart = "Remove part!"
                TXT_WI_LoadPart = "Load Part Into Machine"
                TXT_WI_PressStart = "Press Start Button."
                TXT_WI_Wait = "Wait!"
                TXT_WI_FollowHMI = "Follow steps for the HMI."
                TXT_WI_Unlock = "Press unlock button."
                TXT_WI_AddTop = "Add topcover."
                TXT_WI_Pass = "Part Passed: Happy Part!"
                TXT_WI_Fail = "Part Failed: Sad Part!"

                'Robot Quips
                TXT_Robot_Hello = "Hola!"
                TXT_Robot_Bye = "Adios!"
                TXT_Robot_Anger = "#&!"
                TXT_Robot_Bench = "I live to press!"
                TXT_Robot_Impatient = "What's taking so long?"
                TXT_Robot_Showoff = "Over 9000N, one day..."
                TXT_Robot_Doctor = "A press a day keeps the doctor away!"
                TXT_Robot_Reps = "I need to get my presses in..."

                'UI Text
                TXT_UI_Start = "Start"
                TXT_UI_Stop = "Stop"
                Form_PressMate.TSM_File.Text = "File"
                Form_PressMate.TSM_File_Exit.Text = "Exit"
                Form_PressMate.TSM_Options.Text = "Options"
                Form_PressMate.TSM_Options_Settings.Text = "Settings"
                Form_PressMate.TSM_Options_Login.Text = "Login"
                Form_PressMate.TSM_Help.Text = "Help"
                Form_PressMate.TSM_Help_Contact.Text = "Contact"
                Form_PressMate.TSM_Language.Text = "Language"
                Form_PressMate.Btn_StartStop.Text = "Start"
                Form_PressMate.Lbl_UI_AvgCycletime.Text = "Average Cycletime:"
                Form_PressMate.Lbl_UI_LastCycletime.Text = "Last Cycletime:"
                Form_PressMate.Lbl_UI_PartsPressed.Text = "Parts Built:"
                Form_PressMate.Lbl_UI_Status.Text = "Status"

            Case "Spanish"  'Set all variables to Spanish

                'Texto WI
                TXT_WI_RemovePart = "¡Eliminar parte!"
                TXT_WI_LoadPart = "Insertar parte!"
                TXT_WI_PressStart = "Presione el botón Inicio."
                TXT_WI_Wait = "¡Espera!"
                TXT_WI_FollowHMI = "Siga los pasos para la HMI."
                TXT_WI_Unlock = "Presione Desbloquear."
                TXT_WI_AddTop = "Añade cobertura."
                TXT_WI_Pass = "¡Parte feliz!"
                TXT_WI_Fail = "¡Parte triste!"

                'Bromas de robots
                TXT_Robot_Hello = "¡Hola!"
                TXT_Robot_Bye = "¡Adiós!"
                TXT_Robot_Anger = "#&!"
                TXT_Robot_Bench = "¡Vivo para presionar!"
                TXT_Robot_Impatient = "¿Por qué estás tardando tanto?"
                TXT_Robot_Showoff = "Más de 9000N, un día..."
                TXT_Robot_Doctor = "El médico recomienda una prensa al día."
                TXT_Robot_Reps = "Necesito colocar mis prensas..."

                'Texto de la interfaz de usuario
                TXT_UI_Start = "Iniciar"
                TXT_UI_Stop = "Detener"
                Form_PressMate.TSM_File.Text = "Archivo"
                Form_PressMate.TSM_File_Exit.Text = "Salir"
                Form_PressMate.TSM_Options.Text = "Opciones"
                Form_PressMate.TSM_Options_Settings.Text = "Configuración"
                Form_PressMate.TSM_Options_Login.Text = "Iniciar sesión"
                Form_PressMate.TSM_Help.Text = "Ayuda"
                Form_PressMate.TSM_Help_Contact.Text = "Contacto"
                Form_PressMate.TSM_Language.Text = "Idioma"
                Form_PressMate.Btn_StartStop.Text = "Iniciar"
                Form_PressMate.Lbl_UI_AvgCycletime.Text = "Tiempo promedio:"
                Form_PressMate.Lbl_UI_LastCycletime.Text = "Ultima vez:"
                Form_PressMate.Lbl_UI_PartsPressed.Text = "Piezas construidas:"
                Form_PressMate.Lbl_UI_Status.Text = "Estado"

        End Select

    End Sub

End Module

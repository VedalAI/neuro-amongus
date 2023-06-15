#if !Defined(UNICODE)
  #error A unicode version of Inno Setup is required to compile this script
#endif

[Setup]
AllowNetworkDrive=no
AllowUNCPath=no
AlwaysShowDirOnReadyPage=yes
AppendDefaultDirName=no
AppId={{378A73B1-CA1C-4994-BC89-05C52CCAD152}}
AppName=Neuro Among Us
AppPublisher=VedalAI
AppPublisherURL=https://github.com/VedalAI/neuro-amongus
AppVerName=Neuro Among Us
AppVersion=1.1.0
CloseApplications=False
Compression=lzma2/max
DefaultDirName=.
DirExistsWarning=no
DisableDirPage=no
DisableProgramGroupPage=yes
DisableWelcomePage=no
EnableDirDoesntExistWarning=no
OutputBaseFilename=NeuroAmongUs_Setup
OutputDir=bin
PrivilegesRequired=admin
SetupIconFile=Icon.ico
SolidCompression=yes
UsePreviousAppDir=no
UsePreviousLanguage=no
UsePreviousSetupType=False
UsePreviousTasks=False
UninstallDisplayIcon=Icon.ico
WizardImageFile=BigImage.bmp
WizardSmallImageFile=SmallImage.bmp

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\Neuro\bin\ReleaseDataCollection\Neuro.dll"; DestDir: "{app}\BepInEx\plugins"; Components: Neuro; Flags: IgnoreVersion;
Source: "Dependencies\BepInEx\*"; DestDir: "{app}"; Components: BepInEx; Flags: ConfirmOverwrite RecurseSubdirs IgnoreVersion UninsNeverUninstall;
Source: "Dependencies\Reactor.dll"; DestDir: "{app}\BepInEx\plugins"; Components: Reactor; Flags: ConfirmOverwrite IgnoreVersion UninsNeverUninstall;

Source: "Carbon.vsf"; Flags: DontCopy
Source: "InstallerExtensions.dll"; Flags: DontCopy
Source: "VclStylesinno.dll"; Flags: DontCopy

[Messages]
BeveledLabel=Neuro-sama
WizardSelectDir=Select install location
SelectDirLabel3=Please select the install folder of Among Us.
SelectDirBrowseLabel=To continue, click Next. If you would like to select a different folder, click Browse.%nIf you have Among Us on Steam, you can also use the button on the bottom left to automatically set the install path.

[Types]
Name: "Full"; Description: "Install everything (I have no idea what I'm doing)";
Name: "Custom"; Description: "I know what I'm doing"; Flags: IsCustom;

[Components]
Name: "Neuro"; Description: "Neuro"; Flags: fixed; Types: Full Custom;
Name: "BepInEx"; Description: "BepInEx"; Types: Full;
Name: "Reactor"; Description: "Reactor"; Types: Full;

[Code]
function PathsEqual(pathone, pathtwo: WideString): Boolean; external 'PathsEqual@files:InstallerExtensions.dll stdcall setuponly delayload';

function IsAmongUs(): Boolean;
begin
  if (FileExists(ExpandConstant('{app}')+ '\Among Us.exe')) then
  begin
    Result := true
    Exit
  end
  else
  begin
    Result := false
    Exit
  end
end;

function GetSteamDir(): String;
var
I : Integer;
P : Integer;
steamInstallPath : String;
configFile : String;
fileLines: TArrayOfString;
begin
  steamInstallPath := ''
  RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\WOW6432Node\Valve\Steam', 'InstallPath', steamInstallPath)
  if (FileExists(steamInstallPath + '\steamapps\common\Among Us\Among Us.exe')) then
  begin
    Result := steamInstallPath + '\steamapps\common\Among Us'
    Exit
  end
  else
  begin
    configFile := steamInstallPath + '\config\config.vdf' 
    if FileExists(configFile) then
    begin
      if LoadStringsFromFile(configFile, FileLines) then 
      begin
        for I := 0 to GetArrayLength(FileLines) - 1 do
        begin
          P := Pos('BaseInstallFolder_', FileLines[I])
          if P > 0 then
          begin
            steamInstallPath := Copy(FileLines[I], P + 23, Length(FileLines[i]) - P - 23)
            if (FileExists(steamInstallPath + '\steamapps\common\Among Us\Among Us.exe')) then
            begin
              Result := steamInstallPath + '\steamapps\common\Among Us'
              Exit
            end
          end
        end
      end
    end
  end;
  Result := 'none'
  Exit
end;

var ACLabel: TLabel;
var SteamButton: TNewRadioButton;

procedure SteamButtonOnClick(Sender: TObject);
begin
  WizardForm.DirEdit.Text := GetSteamDir()
  SteamButton.Checked := true
end;

var DirEditOnChangePrev: TNotifyEvent;

procedure DirEditOnChange(Sender: TObject);
var
  S: String;
begin
  if Pos('among us', LowerCase(WizardForm.DirEdit.Text)) <> 0 then
  begin
    if PathsEqual(WizardForm.DirEdit.Text, GetSteamDir()) then
    begin
      SteamButton.Checked := true;
    end
    else
    begin
      SteamButton.Checked := false;
    end
  end
  else
  begin
    SteamButton.Checked := false;
  end;                            
  
  if (Pos('://', WizardForm.DirEdit.Text) <> 0) or (Pos(':\\', WizardForm.DirEdit.Text) <> 0) then
  begin
    S := WizardForm.DirEdit.Text;
    StringChangeEx(S, '://', ':/', true);
    StringChangeEx(S, ':\\', ':\', true);
    WizardForm.DirEdit.Text := S;
  end
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  if CurPageID = wpSelectDir then
  begin
    if not IsAmongUs() then
    begin
      MsgBox('The folder you have chosen is not a valid Among Us installation directory. Please select a valid path.', mbError, MB_OK);
      Result := False;
    end;
  end;
end;

procedure LoadVCLStyle(VClStyleFile: String); external 'LoadVCLStyleW@files:VclStylesInno.dll stdcall';
procedure UnLoadVCLStyles; external 'UnLoadVCLStyles@files:VclStylesInno.dll stdcall';

function InitializeSetup(): Boolean;
begin
  ExtractTemporaryFile('Carbon.vsf');
  LoadVCLStyle(ExpandConstant('{tmp}\Carbon.vsf'));
  Result := true
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpSelectDir then
  begin
    WizardForm.DirEdit.Text := ''
    if GetSteamDir() = 'none' then
    begin
      SteamButton.Enabled := false
    end;
    
    if SteamButton.Enabled then
    begin
      WizardForm.DirEdit.Text := GetSteamDir()
      SteamButton.Checked := true
    end
  end;
  SteamButton.Visible := CurPageID = wpSelectDir
  ACLabel.Visible := CurPageID = wpSelectDir
end;

procedure InitializeWizard();
begin
  ACLabel := TLabel.Create(WizardForm)
  with ACLabel do
  begin
    Parent := WizardForm
    Caption := 'Get Among Us path from:'
    Left := WizardForm.SelectDirLabel.Left / 3
    Top := WizardForm.BackButton.Top - WizardForm.BackButton.Top / 90
  end;

  SteamButton := TNewRadioButton.Create(WizardForm)
  with SteamButton do
  begin
    Parent := WizardForm
    Caption := 'Steam'
    OnClick := @SteamButtonOnClick
    Left := WizardForm.SelectDirLabel.Left + WizardForm.SelectDirLabel.Left / 30
    Top := WizardForm.BackButton.Top + 10
    Height := WizardForm.BackButton.Height
  end;
  //  Left := SubnauticaButton.Left * 3
  //  Top := WizardForm.BackButton.Top + 10
  //  Height := WizardForm.BackButton.Height

  DirEditOnChangePrev := WizardForm.DirEdit.OnChange
  WizardForm.DirEdit.OnChange := @DirEditOnChange
end;

procedure DeinitializeSetup();
begin
  UnLoadVCLStyles;
end;

var
  CheckListBox: TNewCheckListBox;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  Form: TSetupForm;
  OKButton: TNewButton;
  InfoText: TNewStaticText;
  ComponentFiles, ComponentDirs: array of string;
  MyUninstallProgressForm: TSetupForm;
  ProgressBar: TNewProgressBar;
  I: Integer;
begin
  if (CurUninstallStep = usUninstall) then
  begin
    Form := CreateCustomForm;
    try
      Form.ClientWidth := ScaleX(350);
      Form.ClientHeight := ScaleY(250);
      Form.Caption := 'Uninstall Neuro Among Us';
      Form.Position := poScreenCenter;

      InfoText := TNewStaticText.Create(Form);
      InfoText.Parent := Form;
      InfoText.Left := ScaleX(10);
      InfoText.Top := ScaleY(10);
      InfoText.Width := Form.ClientWidth - 2*InfoText.Left;
      InfoText.Caption := 'Choose components to uninstall:';

      CheckListBox := TNewCheckListBox.Create(Form);
      CheckListBox.Parent := Form;
      CheckListBox.Left := InfoText.Left;
      CheckListBox.Top := InfoText.Top + InfoText.Height + ScaleY(10);
      CheckListBox.Width := InfoText.Width;
      CheckListBox.Height := Form.ClientHeight - ScaleY(43 + CheckListBox.Top + InfoText.Height);

      CheckListBox.AddCheckBox('Neuro', '', 0, True, False, True, False, nil);
      CheckListBox.AddCheckBox('BepInEx', '', 0, True, True, True, True, nil);
      CheckListBox.AddCheckBox('Reactor', '', 0, True, True, True, True, nil);

      OKButton := TNewButton.Create(Form);
      OKButton.Parent := Form;
      OKButton.Width := ScaleX(75);
      OKButton.Height := ScaleY(23);
      OKButton.Left := Form.ClientWidth - ScaleX(85);
      OKButton.Top := Form.ClientHeight - ScaleY(33);
      OKButton.Caption := 'OK';
      OKButton.ModalResult := mrOk;
      OKButton.Default := True;

      Form.ActiveControl := CheckListBox;

      if Form.ShowModal = mrOk then
      begin

        if CheckListBox.Checked[1] then
        begin
          SetArrayLength(ComponentDirs, 2);
          ComponentDirs[0] := ExpandConstant('{app}\BepInEx\core');
          ComponentDirs[1] := ExpandConstant('{app}\dotnet');

          SetArrayLength(ComponentFiles, 3);
          ComponentFiles[0] := ExpandConstant('{app}\.doorstop_version');
          ComponentFiles[1] := ExpandConstant('{app}\doorstop_config.ini');
          ComponentFiles[2] := ExpandConstant('{app}\winhttp.dll');
        end;
        
        if CheckListBox.Checked[2] then
        begin
          SetArrayLength(ComponentFiles, Length(ComponentFiles) + 1);
          ComponentFiles[Length(ComponentFiles) - 1] := ExpandConstant('{app}\BepInEx\plugins\Reactor.dll');
        end;

        MyUninstallProgressForm := CreateCustomForm();
        MyUninstallProgressForm.ClientWidth := 300;
        MyUninstallProgressForm.ClientHeight := 100;
        MyUninstallProgressForm.Caption := 'Uninstalling components...';
        MyUninstallProgressForm.Position := poScreenCenter;

        ProgressBar := TNewProgressBar.Create(MyUninstallProgressForm);
        ProgressBar.Parent := MyUninstallProgressForm;
        ProgressBar.Left := 20;
        ProgressBar.Top := 20;
        ProgressBar.Width := MyUninstallProgressForm.ClientWidth - 2 * ProgressBar.Left;
        ProgressBar.Max := Length(ComponentFiles);

        MyUninstallProgressForm.Show();

        for I := 0 to Length(ComponentDirs) - 1 do
        begin
          if DelTree(ComponentDirs[I], True, True, True) then
          begin
            ProgressBar.Position := ProgressBar.Position + 1;
          end;
        end;

        for I := 0 to Length(ComponentFiles) - 1 do
        begin
          if DeleteFile(ComponentFiles[I]) then
          begin
            ProgressBar.Position := ProgressBar.Position + 1;
          end;
        end;

        MyUninstallProgressForm.Close();
        MyUninstallProgressForm.Free();

      end;
    finally
      Form.Free;
    end;
  end;
end;
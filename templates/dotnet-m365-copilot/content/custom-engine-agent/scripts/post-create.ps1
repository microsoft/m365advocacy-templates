# This script is executed after the project has been created.

# Configure solution to use multiple startup projects
$files = Get-ChildItem -Path . -Filter *.sln
$slnPath = $files[0].FullName
$slnName = [System.IO.Path]::GetFileNameWithoutExtension($slnPath)
$dte = New-Object -ComObject VisualStudio.DTE.17.0
$dte.Solution.Open($slnPath)
Write-Host "Current startup projects"
$dte.Solution.SolutionBuild.StartupProjects
$csprojName = "$($slnName)\$($slnName).csproj"
$csprojName
Write-Host "Adding project to startup projects"
$dte.Solution.SolutionBuild.StartupProjects += $csprojName
Write-Host "Updated startup projects"
$dte.Solution.SolutionBuild.StartupProjects
$dte.Solution.SaveAs($slnPath)
$dte.Quit()
[System.Runtime.Interopservices.Marshal]::ReleaseComObject($dte) | Out-Null

# Rename _git* file to .git*
# Rename-Item -Path _gitignore -NewName .gitignore
# Rename-Item -Path ./M365Agent/_gitignore -NewName .gitignore
# Rename-Item -Path _gitattributes -NewName .gitattributes

# Rename user files
# Rename-Item -Path ./M365Agent/env/.env.local.user -NewName .env.local.user
# Rename-Item -Path ./M365Agent/env/.env.dev.user -NewName .env.dev.user

# Cleanup scripts folder
Remove-Item -Path scripts -Recurse -Force
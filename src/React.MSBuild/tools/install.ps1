function Resolve-ProjectName {
    param(
        [parameter(ValueFromPipelineByPropertyName = $true)]
        [string[]]$ProjectName
    )
    
    if($ProjectName) {
        $projects = Get-Project $ProjectName
    }
    else {
        # All projects by default
        $projects = Get-Project
    }
    
    $projects
}

function Get-MSBuildProject {
    param(
        [parameter(ValueFromPipelineByPropertyName = $true)]
        [string[]]$ProjectName
    )
    Process {
        (Resolve-ProjectName $ProjectName) | % {
            $path = $_.FullName
            @([Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($path))[0]
        }
    }
}

$project = Get-Project
$buildProject = Get-MSBuildProject
$target = $buildProject.Xml.AddTarget("TransformJsx")
$target.AfterTargets = "Build"
$task = $target.AddTask("Exec")
$task.SetParameter("Command", '"$(msbuildtoolspath)\msbuild.exe" $(ProjectDirectory)TransformJsx.proj /p:OutputPath=$(OutputPath) /nr:false')
$project.Save()
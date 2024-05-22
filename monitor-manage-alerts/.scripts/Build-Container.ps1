$ModuleName = "monitor-manage-alerts"
$ModuleTagsLocal = "${ModuleName}:local"
Invoke-Expression "docker build --rm -f .\.docker\Dockerfile -t $ModuleTagsLocal ." -ErrorAction Stop
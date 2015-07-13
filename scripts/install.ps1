$configFile = "StormReplayUploader.exe.config"

# Load configuration
$config = [xml](Get-Content $configFile)
 
Write-Host "Fill replayDirectory attribute if it is empty"
$node = $config.SelectSingleNode("//uploaderConfiguration").replayDirectory
Write-Host "Current value: '$node'"
if ($node -eq $null -or $node -eq "")
{
	# Determine path to replay files
	$myDocuments = [Environment]::GetFolderPath("MyDocuments")
	Write-Host "Path1: $myDocuments"
	$hotsDir = Join-Path $myDocuments "Heroes of the Storm"
	Write-Host "Path2: $hotsDir"
	$replayDir = Join-Path $hotsDir "Accounts"
	Write-Host "Path3: $replayDir"

	Write-Host "Setting Heroes of the Storm replay directory to $replayDir" 
    $node.replayDirectory = $replayDir.ToString()
}
 
# Save the result
$config.Save($configFile)

# Install service and start it
.\StormReplayUploader.exe install --localsystem --autostart
.\StormReplayUploader.exe start 

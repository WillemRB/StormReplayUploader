$configFile = "StormReplayUploader.exe.config"

# Load configuration
$config = [xml](Get-Content $configFile)
 
# Update attribute with path if it is empty
$node = $config.SelectSingleNode("//uploaderConfiguration")
if ($node.replayDirectory -eq "")
{
	# Determine path to replay files
	$myDocuments = [Environment]::GetFolderPath("MyDocuments")
	$hotsFolder = Join-Path $myDocuments "Heroes of the Storm"
	$replayFolder = Join-Path $hotsFolder "Accounts"

    $node.replayDirectory = $replayFolder.ToString()
}
 
# Save the result
$config.Save($configFile)

# Install service and start it
.\StormReplayUploader.exe install --localsystem --autostart
.\StormReplayUploader.exe start 

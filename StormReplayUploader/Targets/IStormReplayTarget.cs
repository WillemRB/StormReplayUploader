namespace StormReplayUploader.Targets
{
    public interface IStormReplayTarget
    {
        void Notify(string path);
    }
}

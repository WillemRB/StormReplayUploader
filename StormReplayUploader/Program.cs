using Topshelf;

namespace StormReplayUploader
{
    public class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ReplayWatcher>(s =>
                {
                    s.ConstructUsing(() => new ReplayWatcher());
                    s.WhenStarted(watcher => watcher.Start());
                    s.WhenStopped(watcher => watcher.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("StormReplayUploader");
                x.SetDisplayName("Heroes of the Storm Replay Uploader");
                x.SetDescription("Uploads your Heroes of the Storm replay files to different places.");

                x.StartAutomatically();
            });
        }
    }
}

using System.Threading.Channels;

namespace MySaleForceBackup;

public class ChannelConsumer : BackgroundService
{
  private readonly Channel<string> _channel;
  public ChannelConsumer(Channel<string> channel)
  {
    _channel = channel;
  }
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!_channel.Reader.Completion.IsCompleted && await _channel.Reader.WaitToReadAsync())
    {
      if (_channel.Reader.TryRead(out var msg))
      {
        Console.WriteLine(msg);
      }
    }
  }
}
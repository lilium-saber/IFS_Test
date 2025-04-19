using System;
using System.Threading;
using AvaloniaApp.OpenGLUse;

namespace AvaloniaApp.Ults;

public class OpenGlThread
{
    private Thread? _thread;
    private OpenGlNoWindow? _openGlNoWindow = new();
    private bool _isRunning = false;
    
    public void StartOpenGlThread()
    {
        if (_isRunning)
            return;

        _isRunning = true;
        _openGlNoWindow ??= new OpenGlNoWindow();
        _thread = new(() =>
        {
            try
            {
                _openGlNoWindow?.StartOpenGlBackground();
            }
            catch (Exception err)
            {
                Console.WriteLine("OpenGlThread start have err" + err.Message);
                throw;
            }
        })
        {
            IsBackground = true
        };
        _thread.Start();
    }
    
    public void StopOpenGlThread()
    {
        if (_thread == null || !_isRunning)
            return;

        _isRunning = false;
        _openGlNoWindow?.StopOpenGlBackground();
        if(_thread is { IsAlive: true })
        {
            _thread.Join();
        }
        _thread = null;
        _openGlNoWindow = null;
    }
}
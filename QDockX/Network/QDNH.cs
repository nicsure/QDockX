using NAudio.Wave.SampleProviders;
using QDockX.Context;
using QDockX.Debug;
using QDockX.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Network
{
    public static class QDNH
    {
        private static TcpClient serial = null, audio = null;
        private static NetworkStream serialStream = null, audioStream = null;
        private static Task audioWriteTask = null, serialWriteTask = null;
        private static bool audioReady = false, serialReady = false;
        private static Color err = Colors.Red;
        private static readonly object sync = new();

        public static void Init()
        {
            Data.Instance.LED2.Value = Colors.Black;
            Task.Run(Loop);
        }

        // red = Cannot connect to host
        // yellow = autenticator error (wrong password?)
        // blue = The host did not respond as expected (not a QDNH server?)
        // orange = connecting to host
        // magenta = authenticating (usually too quick to notice)
        // cyan = connection to host was lost (only visible for about a second before reconnection is attempted)
        // green = connected okay
        // pink = no serial connection
        // brown = no audio connection

        public static async Task Loop()
        {
            Data.Instance.Host.PropertyChanged += NetworkParams_Changed;
            Data.Instance.Port.PropertyChanged += NetworkParams_Changed;
            MessageHub.Message += MessageHub_Message;
            while (true) 
            {
                using (serial) using (audio) using (serialStream) using (audioStream)
                {
                    serial = new();
                    audio = new();
                    bool audioAuth = false, serialAuth = false;
                    Data.Instance.LED2.Value = Colors.Orange;
                    err = Colors.Red;
                    try
                    {
                        using Task serialConnect = serial.ConnectAsync(Data.Instance.Host.Value, Data.Instance.Port.Value + 1);
                        using Task audioConnect = audio.ConnectAsync(Data.Instance.Host.Value, Data.Instance.Port.Value);
                        await serialConnect;
                        await audioConnect;
                        Data.Instance.LED2.Value = Colors.Magenta;
                        serialStream = serial.GetStream();
                        audioStream = audio.GetStream();
                        err = Colors.Yellow;
                        serialAuth = Authenticate(serialStream, Data.Instance.Password.Value);
                        audioAuth = Authenticate(audioStream, Data.Instance.Password.Value);
                    }
                    catch (Exception ex) { DebugLog.Exception(ex); }
                    if (serialAuth && audioAuth)
                    {
                        using Task serialPump = Pump(serialStream, Msg._serialin, false);
                        using Task audioPump = Pump(audioStream, Msg._audioin, true);                        
                        await serialPump;
                        await audioPump;
                    }
                    else
                        err = Colors.Blue;
                    Close(err);
                    serial = null;
                    audio = null;
                    serialStream = null;
                    audioStream = null;
                }
                await Watchdog.Delay(1000);
            }
        }

        private static void NetworkParams_Changed(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Close();
        }

        private static bool Authenticate(NetworkStream stream, string password)
        {
            byte[] salt = new byte[32];
            for (int i = 0; i < salt.Length; i++)
            {
                int byt;
                try { byt = stream.ReadByte(); }
                catch(Exception ex) { DebugLog.Exception(ex); byt = -1; }
                if (byt < 0) return false;
                salt[i] = (byte)byt;
            }
            using SHA256 sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(password).Concat(salt).ToArray());
            try { stream.Write(hash); stream.Flush(); }
            catch (Exception ex) { DebugLog.Exception(ex); return false; }
            return true;
        }

        private static Task WriteToStream(NetworkStream ns, byte[] buffer, int length)
        {
            return Task.Run(() => 
            {
                try
                {
                    ns?.Write(buffer, 0, length);
                    return;
                }
                catch(Exception ex)
                {
                    DebugLog.Exception(ex); 
                }
                Close();
            });
        }


        private static void MessageHub_Message(object sender, MessageEventArgs e)
        {
            switch(e.Message)
            {
                case var n when n == Msg._audioout:
                    if(audioReady)
                    {
                        if (audioWriteTask?.IsCompleted ?? true)
                        {
                            using (audioWriteTask)
                            {
                                var (buffer, length) = ((byte[] buffer, int length))e.Parameter;
                                audioWriteTask = WriteToStream(audioStream, buffer, length);
                            }
                        }
                    }
                    break;
                case var n when n == Msg._serialout:
                    if(serialReady)
                    {
                        if (serialWriteTask?.Wait(5000) ?? true)
                        {
                            using (serialWriteTask)
                            {
                                var (buffer, length) = ((byte[] buffer, int length))e.Parameter;
                                serialWriteTask = WriteToStream(serialStream, buffer, length);
                            }
                        }
                        else
                        {                           
                            Close();
                            if (!serialWriteTask.Wait(1000))
                                serialWriteTask = null;
                            else
                                serialWriteTask.Dispose();
                            break;
                        }
                    }
                    break;
            }
        }

        public static void Close(Color col)
        {
            Data.Instance.LED2.Value = col;
            Close();
        }

        public static void Close()
        {
            audioReady = false;
            serialReady = false;
            try { (_ = serialStream)?.Close(); } catch { }
            try { (_ = audioStream)?.Close(); } catch { }
            try { (_ = serial)?.Close(); } catch { }
            try { (_ = audio)?.Close(); } catch { }
        }
        
        private static async Task Pump(NetworkStream stream, string message, bool audio)
        {
            byte[] b = new byte[4096];
            int br;            
            try
            {
                br = await stream.ReadAsync(b);
                err = Colors.Cyan;
                lock (sync)
                {
                    if (audio) audioReady = true; else serialReady = true;
                    Data.Instance.LED2.Value =
                        audioReady && serialReady ? Colors.Green :
                        audioReady && !serialReady ? Colors.Pink : Colors.Brown;
                }
            }
            catch (Exception ex)
            {
                DebugLog.Exception(ex);
                br = -1;
                if (audio) audioReady = false; else serialReady = false;
            }
            if (br > 0)
            {
                MessageHub.Send(message, (b, br));
                while (true)
                {
                    try
                    {
                        br = await stream.ReadAsync(b);
                    }
                    catch (Exception ex)
                    {
                        DebugLog.Exception(ex);
                        br = -1;
                    }
                    if (br <= 0) break;
                    MessageHub.Send(message, (b, br));
                }
            }
            Close();
        }



    }
}

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
        private static bool ready = false;

        public static void Init()
        {
            Task.Run(Loop);
        }

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
                    try
                    {
                        using Task serialConnect = serial.ConnectAsync(Data.Instance.Host.Value, Data.Instance.Port.Value + 1);
                        using Task audioConnect = audio.ConnectAsync(Data.Instance.Host.Value, Data.Instance.Port.Value);
                        await serialConnect;
                        await audioConnect;
                        serialStream = serial.GetStream();
                        audioStream = audio.GetStream();
                        serialAuth = Authenticate(serialStream, Data.Instance.Password.Value);
                        audioAuth = Authenticate(audioStream, Data.Instance.Password.Value);
                    }
                    catch (Exception ex) { DebugLog.Exception(ex); }
                    if (serialAuth && audioAuth)
                    {
                        using Task serialPump = Pump(serialStream, "SerialIn");
                        using Task audioPump = Pump(audioStream, "AudioIn");
                        ready = true;
                        await serialPump;
                        await audioPump;
                    }
                    Close();
                    serial = null;
                    audio = null;
                    serialStream = null;
                    audioStream = null;
                }
                await (Watchdog.Watch = Task.Delay(1000));
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
            try { stream.Write(hash); }
            catch (Exception ex) { DebugLog.Exception(ex); return false; }
            return true;
        }

        private static void MessageHub_Message(object sender, MessageEventArgs e)
        {
            switch(e.Message)
            {
                case "AudioOut":
                    if(ready)
                    {
                        if (audioWriteTask?.IsCompleted ?? true)
                        {
                            using (audioWriteTask)
                            {
                                if (audioWriteTask?.Exception != null)
                                {
                                    DebugLog.Exception(audioWriteTask.Exception);
                                    Close();
                                    break;
                                }
                                var (buffer, length) = ((byte[] buffer, int length))e.Parameter;
                                audioWriteTask = (_ = audioStream)?.WriteAsync(buffer, 0, length);
                            }
                        }
                    }
                    break;
                case "SerialOut":
                    if(ready)
                    {
                        serialWriteTask?.Wait(5000);
                        if (!(serialWriteTask?.IsCompleted ?? true))
                        {
                            Close();
                            break;
                        }
                        using (serialWriteTask)
                        {
                            if (serialWriteTask?.Exception != null)
                            {
                                DebugLog.Exception(serialWriteTask.Exception);
                                Close();
                                break;
                            }
                            var (buffer, length) = ((byte[] buffer, int length))e.Parameter;
                            serialWriteTask = (_ = serialStream)?.WriteAsync(buffer, 0, length);
                        }
                    }
                    break;
            }
        }

        public static void Close()
        {
            ready = false;
            try { (_ = serialStream)?.Close(); } catch { }
            try { (_ = audioStream)?.Close(); } catch { }
            try { (_ = serial)?.Close(); } catch { }
            try { (_ = audio)?.Close(); } catch { }
        }

        private static async Task Pump(NetworkStream stream, string message)
        {
            while(true)
            {
                byte[] b = new byte[4096];
                int br;
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
            Close();
        }



    }
}

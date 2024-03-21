using QDockX.Context;
using QDockX.UI;
using QDockX.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QDockX.Radio
{
    public static class Serial
    {
        private static readonly byte[] xor_array = { 0x16, 0x6c, 0x14, 0xe6, 0x2e, 0x91, 0x0d, 0x40, 0x21, 0x35, 0xd5, 0x40, 0x13, 0x03, 0xe9, 0x80 };
        private enum Stage { Idle, CD, LenLSB, LenMSB, Data, CrcLSB, CrcMSB, DC, BA, UiType, UiVal1, UiVal2, UiVal3, UiDataLen, UiData }
        private static Stage stage = Stage.Idle;
        private static int pLen, pCnt;
        private static byte[] pdata = Array.Empty<byte>(), uiData;
        private static int uiType, uiVal1, uiVal2, uiVal3, uiDataLen, uiDataCnt;

        public static void Init()
        {
            MessageHub.Message += MessageHub_Message;
        }

        private static void MessageHub_Message(object sender, MessageEventArgs e)
        {
            switch(e.Message)
            {
                case "KeyPress":
                    if(e.Parameter is int keyNum)
                    {
                        SendCommand(Packet.KeyPress, (ushort)keyNum);
                    }
                    break;
                case "SerialIn":
                    var (buffer, length) = ((byte[] buffer, int length))e.Parameter;
                    for (int i = 0; i < length; i++)
                        ByteIn(buffer[i]);
                    break;
            }
        }

        private static void ByteIn(byte b)
        {
            switch (stage)
            {
                case Stage.Idle:
                    if (b == 0xAB)
                        stage = Stage.CD;
                    else
                    if (b == 0xB5)
                        stage = Stage.UiType;
                    break;
                case Stage.CD:
                    stage = (b == 0xcd ? Stage.LenLSB : Stage.Idle);
                    break;
                case Stage.LenLSB:
                    pLen = b;
                    stage = Stage.LenMSB;
                    break;
                case Stage.LenMSB:
                    pCnt = 0;
                    pLen |= b << 8;
                    pdata = new byte[pLen];
                    stage = Stage.Data;
                    break;
                case Stage.Data:
                    pdata[pCnt] = Crypt(b, pCnt++);
                    if (pCnt >= pLen)
                        stage = Stage.CrcLSB;
                    break;
                case Stage.CrcLSB:
                    stage = Stage.CrcMSB;
                    break;
                case Stage.CrcMSB:
                    stage = Stage.DC;
                    break;
                case Stage.DC:
                    stage = (b == 0xdc ? Stage.BA : Stage.Idle);
                    break;
                case Stage.BA:
                    stage = Stage.Idle;
                    if (b == 0xba)
                        ParsePacket(pdata);
                    break;
                case Stage.UiType:
                    uiType = b;
                    stage = Stage.UiVal1;
                    break;
                case Stage.UiVal1:
                    uiVal1 = b;
                    stage = Stage.UiVal2;
                    break;
                case Stage.UiVal2:
                    uiVal2 = b;
                    stage = Stage.UiVal3;
                    break;
                case Stage.UiVal3:
                    uiVal3 = b;
                    stage = Stage.UiDataLen;
                    break;
                case Stage.UiDataLen:
                    uiDataLen = b;
                    uiDataCnt = 0;
                    uiData = new byte[uiType == 6 ? 0 : uiDataLen];
                    if (uiData.Length == 0)
                    {
                        stage = Stage.Idle;
                        UiPacket();
                    }
                    else
                        stage = Stage.UiData;
                    break;
                case Stage.UiData:
                    uiData[uiDataCnt++] = b;
                    if (uiDataCnt >= uiDataLen)
                    {
                        stage = Stage.Idle;
                        UiPacket();
                    }
                    break;
            }
        }

        private static void DrawText(int x, int line, double height, string text, bool bold = false, bool stretch = false)
        {
            MessageHub.Send("LcdText", (x, line, height, text, bold));
        }

        private static void ClearLines(int start, int end)
        {
            MessageHub.Send("LcdClear", (start, end));
        }

        private static void DrawSignal(int v1, int v2)
        {
            MessageHub.Send("LcdSignal", v1 + v2);
        }

        private static void UiPacket()
        {
            switch (uiType)
            {
                case 0:
                    while (uiVal1 > 128) { uiVal2++; uiVal1 -= 128; }
                    DrawText(uiVal1, uiVal2 + 1, 1.5, Encoding.ASCII.GetString(uiData));
                    break;
                case 1:
                    while (uiVal1 > 128) { uiVal2++; uiVal1 -= 128; }
                    DrawText(uiVal1, uiVal2 + 1, uiVal3 / 6.0, Encoding.ASCII.GetString(uiData), false, false);
                    break;
                case 2:
                    while (uiVal1 > 128) { uiVal2++; uiVal1 -= 128; }
                    DrawText(uiVal1, uiVal2 + 1, uiVal3 / 6.0, Encoding.ASCII.GetString(uiData), true, true);
                    break;
                case 3:
                    while (uiVal1 > 128) { uiVal2++; uiVal1 -= 128; }
                    DrawText(uiVal1, uiVal2 + 1, 2, Encoding.ASCII.GetString(uiData), false, true);
                    break;
                case 5:
                    ClearLines(uiVal1, uiVal2);
                    break;
                case 6:
                    string ps;
                    switch (uiVal1 & 7)
                    {
                        case 1:
                            ps = "T  ";
                            Status.TX = true;
                            Data.Instance.LED.Value = Colors.Red;
                            break;
                        case 2:
                            ps = "R  ";
                            Status.RX = true;
                            Data.Instance.LED.Value = Colors.LimeGreen;
                            break;
                        case 4:
                            ps = "PS ";
                            Status.SQ = true;
                            Data.Instance.LED.Value = Colors.Black;
                            break;
                        default:
                            ps = "   ";
                            if (!Status.SQ)
                                DrawSignal(0, 0);
                            Status.SQ = true;
                            Data.Instance.LED.Value = Colors.DarkBlue;
                            break;
                    }
                    ps += (uiVal1 & 8) != 0 ? "NOA " : "    ";
                    ps += (uiVal1 & 16) != 0 ? "DTMF " : "     ";
                    ps += (uiVal1 & 32) != 0 ? "FM " : "   ";
                    ps += uiVal3 != 0 ? ((char)uiVal3).ToString()+" " : "  ";
                    ps += (uiVal1 & 64) != 0 ? "🢀 " : "  ";

                    if ((uiVal1 & 128) != 0) ps += "DW ";
                    else if ((uiVal2 & 1) != 0) ps += ">< ";
                    else if ((uiVal2 & 2) != 0) ps += "XB ";
                    else ps += "   ";

                    ps += (uiVal2 & 4) != 0 ? "VOX " : "    ";

                    bool f = false;
                    if ((uiVal2 & 8) != 0) ps += "🔒 ";
                    else if ((uiVal2 & 16) != 0) { ps += "F "; f = true; }
                    else ps += "  ";
                    QButton.SetFunctionLabels(f);

                    ps += (uiVal2 & 32) != 0 ? "⚡ 🔋" : "  🔋";

                    float bat = uiDataLen * 0.04f;
                    if (bat > 8.4f) bat = 8.4f;
                    ps += $"{bat:F2}V {(uiDataLen / 2.1f):F0}%";

                    DrawText(0, 0, 0.5, ps, false, false);

                    break;
                case 7:
                    DrawText(0, uiVal1, 1, uiVal2 == 0 ? "▻" : "➤", false, true);
                    break;
                case 8:
                    DrawSignal(uiVal1, uiVal2);
                    break;
            }
        }

        private static void ParsePacket(byte[] data)
        {

        }

        private static byte Crypt(int byt, int xori) => (byte)(byt ^ xor_array[xori & 15]);
        public static int Crc16(int byt, int crc)
        {
            crc ^= byt << 8;
            for (int i = 0; i < 8; i++)
            {
                crc <<= 1;
                if (crc > 0xffff)
                {
                    crc ^= 0x1021;
                    crc &= 0xffff;
                }
            }
            return crc;
        }

        public static void SendCommand(ushort cmd, params object[] args)
        {
            var data = new byte[256];
            data[0] = 0xAB;
            data[1] = 0xCD;
            data[4] = cmd.Byte(0);
            data[5] = cmd.Byte(1);
            int ind = 8;
            foreach (object val in args)
            {
                if (val is uint[] ia)
                {
                    foreach (uint u in ia)
                    {
                        Array.Copy(BitConverter.GetBytes(u), 0, data, ind, 4);
                        ind += 4;
                    }
                }
                else
                if (val is byte[] ba)
                {
                    foreach (byte byt in ba)
                        data[ind++] = byt;
                }
                else
                if (val is byte b)
                    data[ind++] = b;
                else if (val is ushort s1)
                {
                    data[ind++] = s1.Byte(0);
                    data[ind++] = s1.Byte(1);
                }
                else if (val is short s2)
                {
                    data[ind++] = s2.Byte(0);
                    data[ind++] = s2.Byte(1);
                }
                else if (val is uint i1)
                {
                    data[ind++] = i1.Byte(0);
                    data[ind++] = i1.Byte(1);
                    data[ind++] = i1.Byte(2);
                    data[ind++] = i1.Byte(3);
                }
                else if (val is int i2)
                {
                    data[ind++] = i2.Byte(0);
                    data[ind++] = i2.Byte(1);
                    data[ind++] = i2.Byte(2);
                    data[ind++] = i2.Byte(3);
                }
            }
            int prmLen = ind - 8;
            data[6] = prmLen.Byte(0);
            data[7] = prmLen.Byte(1);
            int crc = 0, xor = 0;
            for (int i = 4; i < ind; i++)
            {
                crc = Crc16(data[i], crc);
                data[i] = Crypt(data[i], xor++);
            }
            data[ind++] = Crypt(crc.Byte(0), xor++);
            data[ind++] = Crypt(crc.Byte(1), xor);
            data[ind++] = 0xDC;
            data[ind++] = 0xBA;
            ind -= 8;
            data[2] = ind.Byte(0);
            data[3] = ind.Byte(1);
            MessageHub.Send("SerialOut", (data, ind + 8));
        }


        public static byte Byte(this ushort s, int byteIndex) => (byte)((s >> (byteIndex * 8)) & 0xff);
        public static byte Byte(this short s, int byteIndex) => (byte)((s >> (byteIndex * 8)) & 0xff);
        public static byte Byte(this int s, int byteIndex) => (byte)((s >> (byteIndex * 8)) & 0xff);
        public static byte Byte(this uint s, int byteIndex) => (byte)((s >> (byteIndex * 8)) & 0xff);
    }
}

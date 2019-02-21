using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MyCRC
{
    public partial class Form1 : Form
    {
        //List<byte> OriginArray = new List<byte>();

        public Form1()
        {
            InitializeComponent();
        }
        //[DllImport("DLL_01.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        //private static extern int sum(int a, int b);

        private void button1_Click(object sender, EventArgs e)
        {
            string[] OriginArray = textBox1.Text.Trim().Split(' ');

            byte crcValue = 0xff;
            for (int i = 0; i < OriginArray.Length; i++)
            {
                crcValue = CalcHeaderCRC(Convert.ToByte(OriginArray[i], 16), crcValue);
            }

            //crcValue = CRC8_2(0x81, OriginArray);
            
            textBox2.Text = Convert.ToString(crcValue, 16).ToUpper();
            textBox3.Text = Convert.ToString((0xff - crcValue), 16).ToUpper();
        }
        /* Accumulate "dataValue" into the CRC in crcValue.
        / Return value is updated CRC
        /
        / Assumes that "unsigned char" is equivalent to one octet.
        / Assumes that "unsigned int" is 16 bits.
        / The ^ operator means exclusive OR.
        */
        private byte CalcHeaderCRC(byte dataValue, byte crcValue)
        {
            int crc;
            crc = crcValue ^ dataValue; /* XOR C7..C0 with D7..D0 */
                                        /* Exclusive OR the terms in the table (top down) */
            crc = crc ^ (crc << 1) ^ (crc << 2) ^ (crc << 3) ^ (crc << 4) ^ (crc << 5) ^ (crc << 6) ^ (crc << 7);
            /* Combine bits shifted out left hand end */
            return Convert.ToByte(((crc & 0xfe) ^ ((crc >> 8) & 1)) & 0xff);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] OriginArray = textBox4.Text.Trim().Split(' ');

            ushort crcValue = 0xffff;
            for (int i = 0; i < OriginArray.Length; i++)
            {
                crcValue = CalcDataCRC(Convert.ToByte(OriginArray[i], 16), crcValue);
            }

            //crcValue = CRC8_2(0x81, OriginArray);

            textBox5.Text = Convert.ToString(crcValue, 16).ToUpper();
            string tmp = Convert.ToString((0xffff - crcValue), 16).ToUpper();
            textBox6.Text = tmp.Substring(2, 2) + " " + tmp.Substring(0, 2);

        }
        /* Accumulate "dataValue" into the CRC in crcValue.
        / Return value is updated CRC
        /
        / Assumes that "unsigned char" is equivalent to one octet.
        / Assumes that "unsigned int" is 16 bits.
        / The ^ operator means exclusive OR.
        */
        private ushort CalcDataCRC(byte dataValue, ushort crcValue)
        {
            int crcLow;
            crcLow = (crcValue & 0xff) ^ dataValue; /* XOR C7..C0 with D7..D0 */
                                                    /* Exclusive OR the terms in the table (top down) */
            return Convert.ToUInt16(((crcValue >> 8) ^ (crcLow << 8) ^ (crcLow << 3) ^ (crcLow << 12) ^ (crcLow >> 4) ^ (crcLow & 0x0f) ^ ((crcLow & 0x0f) << 7)) & 0xffff);
        }


        #region otherCRCAlgorithm
        public static byte CRC8_2(byte crcPoly, string[] buffer)
        {
            byte crc = 0xff;
            for (int j = 0; j < buffer.Length; j++)
            {
                crc ^= Convert.ToByte(buffer[j], 16);
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x01) != 0)
                    {
                        crc >>= 1;
                        crc ^= crcPoly;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return crc;
        }
        #endregion
    }
}

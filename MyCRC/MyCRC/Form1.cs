﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyCRC
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        public static string BAC = "BACnet";
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string tempS = textBox_Header.Text.Replace(" ", "").Replace("	", "").Replace(",", "").Replace("\r\n", "").Trim();
                if(tempS == "" || tempS.Length % 2 != 0)
                {
                    MessageBox.Show("Please check the header!\r\nThe length is not correct!");
                    return;
                }
                int length = tempS.Length / 2;//get lenngth
                string[] OriginArray = new string[length];
                for (int i = 0; i < length; i++)
                {
                    OriginArray[i] = tempS.Substring(i * 2, 2);
                }

                byte crcValue = 0xff;
                for (int i = 0; i < OriginArray.Length; i++)
                {
                    crcValue = CalcHeaderCRC(Convert.ToByte(OriginArray[i], 16), crcValue);
                }

                //crcValue = CRC8_2(0x81, OriginArray);

                textBox2.Text = Convert.ToString(crcValue, 16);//.ToUpper();
                textBox3.Text = Convert.ToString((0xff - crcValue), 16);//.ToUpper();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            try
            {
                string tempS = textBox_Data.Text.Replace(" ", "").Replace("	", "").Replace(",", "").Replace("\r\n", "").Trim();
                if (tempS == "" || tempS.Length % 2 != 0)
                {
                    MessageBox.Show("Please check the data!\r\nThe length is not correct!");
                    return;
                }
                int length = tempS.Length / 2;//get lenngth
                string[] OriginArray = new string[length];
                for (int i = 0; i < length; i++)
                {
                    OriginArray[i] = tempS.Substring(i * 2, 2);
                }

                ushort crcValue = 0xffff;
                for (int i = 0; i < OriginArray.Length; i++)
                {
                    crcValue = CalcDataCRC(Convert.ToByte(OriginArray[i], 16), crcValue);
                }

                //crcValue = CRC8_2(0x81, OriginArray);

                textBox5.Text = Convert.ToString(crcValue, 16);//.ToUpper();
                string tmp = Convert.ToString((0xffff - crcValue), 16);//.ToUpper();
                textBox6.Text = tmp.Substring(2, 2) + "" + tmp.Substring(0, 2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
        // input:
        //        crcPoly is the low octet of the hex of the crc polynomial
        //        eg. G(x) = x^8 + x^7 + 1
        //                                 => (0b) 1 1000 0001 = 0x181 => crcPoly = 0x81
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

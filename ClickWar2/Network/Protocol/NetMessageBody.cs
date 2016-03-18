using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ClickWar2.Utility;

namespace ClickWar2.Network.Protocol
{
    public class NetMessageBody
    {
        public NetMessageBody()
        {

        }

        public NetMessageBody(string data)
        {
            this.Data = new StringBuilder(data);
        }

        public NetMessageBody(byte[] data)
        {
            try
            {
                this.CheckSum = BitConverter.ToInt32(data, 0);
                this.Data = new StringBuilder(Encoding.Unicode.GetString(data, sizeof(int), data.Length - sizeof(int)));
            }
            catch (Exception)
            {
                this.CheckSum = -1;
            }
        }

        public NetMessageBody(byte[] encryptedData, byte[] key)
        {
            try
            {
                byte[] data = Security.Decode(encryptedData, key);

                this.CheckSum = BitConverter.ToInt32(data, 0);
                this.Data = new StringBuilder(Encoding.Unicode.GetString(data, sizeof(int), data.Length - sizeof(int)));
            }
            catch (Exception)
            {
                this.CheckSum = -1;
            }
        }

        //#####################################################################################

        protected int CheckSum
        { get; set; }

        public StringBuilder Data
        { get; set; } = new StringBuilder();

        //#####################################################################################

        public NetMessageBody Clone()
        {
            NetMessageBody clone = new NetMessageBody();
            clone.CheckSum = this.CheckSum;
            clone.Data = new StringBuilder(this.Data.ToString());


            return clone;
        }

        //#####################################################################################

        protected int CalculateCheckSum()
        {
            int checkSum = 0;
            checkSum += Encoding.Unicode.GetByteCount(this.Data.ToString());


            return checkSum;
        }

        //#####################################################################################

        public bool IsValid
        {
            get
            {
                return (this.CheckSum == this.CalculateCheckSum());
            }
        }

        public int ByteSize
        { get { return (sizeof(int) + Encoding.Unicode.GetByteCount(this.Data.ToString())); } }

        public byte[] GetBytes()
        {
            this.CheckSum = this.CalculateCheckSum();


            byte[] result = new byte[this.ByteSize];

            var checkSumBytes = BitConverter.GetBytes(this.CheckSum);
            Array.Copy(checkSumBytes, 0, result, 0, checkSumBytes.Length);

            var dataBytes = Encoding.Unicode.GetBytes(this.Data.ToString());
            Array.Copy(dataBytes, 0, result, checkSumBytes.Length, dataBytes.Length);


            return result;
        }
    }
}

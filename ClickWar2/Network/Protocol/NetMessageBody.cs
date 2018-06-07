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

        public NetMessageBody(byte[] data)
        {
            try
            {
                this.CheckSum = BitConverter.ToInt32(data, 0);
                this.Data = data.Skip(sizeof(int)).ToList();
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
                this.Data = data.Skip(sizeof(int)).ToList();
            }
            catch (Exception)
            {
                this.CheckSum = -1;
            }
        }

        //#####################################################################################

        protected int CheckSum
        { get; set; }

        public List<byte> Data
        { get; set; } = new List<byte>();

        //#####################################################################################

        public NetMessageBody Clone()
        {
            NetMessageBody clone = new NetMessageBody();
            clone.CheckSum = this.CheckSum;
            clone.Data.AddRange(this.Data);


            return clone;
        }

        //#####################################################################################

        protected int CalculateCheckSum()
        {
            int checkSum = 0;
            checkSum += this.Data.Count;


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
        { get { return (sizeof(int) + this.Data.Count); } }

        public byte[] GetBytes()
        {
            this.CheckSum = this.CalculateCheckSum();


            byte[] result = new byte[this.ByteSize];

            var checkSumBytes = BitConverter.GetBytes(this.CheckSum);
            Array.Copy(checkSumBytes, 0, result, 0, checkSumBytes.Length);
            
            Array.Copy(this.Data.ToArray(), 0, result, checkSumBytes.Length, this.Data.Count);


            return result;
        }
    }
}

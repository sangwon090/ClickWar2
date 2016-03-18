using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Network.Protocol
{
    public class NetMessageHeader
    {
        public NetMessageHeader()
        {

        }

        public NetMessageHeader(int messageNumber)
        {
            this.MessageNumber = messageNumber;
        }

        public NetMessageHeader(byte[] data)
        {
            if (data.Length >= ByteSize)
            {
                try
                {
                    this.CheckSum = BitConverter.ToInt32(data, 0);
                    this.MessageNumber = BitConverter.ToInt32(data, sizeof(int));
                    this.SequenceNumber = BitConverter.ToInt32(data, sizeof(int) * 2);
                    this.BodySize = BitConverter.ToInt32(data, sizeof(int) * 3);
                }
                catch (Exception)
                {
                    this.CheckSum = -1;
                }
            }
            else
            {
                this.CheckSum = -1;
            }
        }

        //#####################################################################################

        protected int CheckSum
        { get; set; }
        
        public int MessageNumber
        { get; set; }

        public int SequenceNumber
        { get; set; }

        public int BodySize
        { get; set; }

        //#####################################################################################

        public NetMessageHeader Clone()
        {
            NetMessageHeader clone = new NetMessageHeader();
            clone.CheckSum = this.CheckSum;
            clone.MessageNumber = this.MessageNumber;
            clone.SequenceNumber = this.SequenceNumber;
            clone.BodySize = this.BodySize;


            return clone;
        }

        //#####################################################################################

        protected int CalculateCheckSum()
        {
            int checkSum = 0;
            checkSum += this.MessageNumber;
            checkSum += this.SequenceNumber;
            checkSum += this.BodySize;


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

        public const int ByteSize = sizeof(int) * 4;

        public byte[] GetBytes()
        {
            this.CheckSum = this.CalculateCheckSum();


            byte[] result = new byte[NetMessageHeader.ByteSize];

            int destIndex = 0;

            var checkSumBytes = BitConverter.GetBytes(this.CheckSum);
            Array.Copy(checkSumBytes, 0, result, destIndex,
                checkSumBytes.Length);
            destIndex += checkSumBytes.Length;

            var msgNumBytes = BitConverter.GetBytes(this.MessageNumber);
            Array.Copy(msgNumBytes, 0, result, destIndex,
                msgNumBytes.Length);
            destIndex += msgNumBytes.Length;

            var seqNumBytes = BitConverter.GetBytes(this.SequenceNumber);
            Array.Copy(seqNumBytes, 0, result, destIndex,
                seqNumBytes.Length);
            destIndex += seqNumBytes.Length;

            var bodySizeBytes = BitConverter.GetBytes(this.BodySize);
            Array.Copy(bodySizeBytes, 0, result, destIndex,
                bodySizeBytes.Length);
            //destIndex += bodySizeBytes.Length;


            return result;
        }
    }
}

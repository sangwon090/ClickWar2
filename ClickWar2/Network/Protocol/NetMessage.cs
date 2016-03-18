using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickWar2.Utility;

namespace ClickWar2.Network.Protocol
{
    public class NetMessage
    {
        public static readonly NetMessage TestMessage;

        static NetMessage()
        {
            IO.NetMessageStream maker = new IO.NetMessageStream();
            maker.WriteData("TEST MESSAGE");
            TestMessage = maker.CreateMessage(0);
        }

        //#####################################################################################

        public NetMessage()
        {
            this.Header = new NetMessageHeader();
            this.Body = new NetMessageBody();
        }

        public NetMessage(NetMessageHeader header, NetMessageBody body)
        {
            this.Header = header;
            this.Body = body;
        }

        public NetMessage(byte[] data)
        {
            try
            {
                this.Header = new NetMessageHeader(data);

                byte[] bodyBytes = new byte[data.Length - NetMessageHeader.ByteSize];
                data.CopyTo(bodyBytes, NetMessageHeader.ByteSize);
                this.Body = new NetMessageBody(data);
            }
            catch (Exception)
            {
                this.Header = null;
                this.Body = null;
            }
        }

        public NetMessage(byte[] encryptedData, byte[] key)
        {
            try
            {
                this.Header = new NetMessageHeader(encryptedData);

                byte[] encryptedBodyBytes = new byte[encryptedData.Length - NetMessageHeader.ByteSize];
                encryptedData.CopyTo(encryptedBodyBytes, NetMessageHeader.ByteSize);
                byte[] bodyBytes = Security.Decode(encryptedBodyBytes, key);
                this.Body = new NetMessageBody(bodyBytes);
            }
            catch (Exception)
            {
                this.Header = null;
                this.Body = null;
            }
        }

        //#####################################################################################

        public int RetryCount
        { get; set; } = 0;

        //#####################################################################################

        public NetMessageHeader Header
        { get; set; }

        public NetMessageBody Body
        { get; set; }

        //#####################################################################################

        public NetMessage Clone()
        {
            NetMessage clone = new NetMessage();
            clone.RetryCount = this.RetryCount;
            clone.Header = this.Header.Clone();
            clone.Body = this.Body.Clone();


            return clone;
        }

        //#####################################################################################

        public bool IsValid
        {
            get
            {
                return (this.Header != null && this.Body != null
                    && this.Header.IsValid
                    && this.Body.IsValid);
            }
        }

        public int ByteSize
        { get { return NetMessageHeader.ByteSize + this.Body.ByteSize; } }

        public byte[] GetBytes()
        {
            // 바디의 크기를 헤더에 설정
            this.Header.BodySize = this.Body.ByteSize;

            // 헤더와 바디의 바이트배열을 합쳐서 반환
            var header = this.Header.GetBytes();
            var body = this.Body.GetBytes();

            int byteSize = header.Length + body.Length;

            if (byteSize > 0)
            {
                byte[] result = new byte[byteSize];
                Array.Copy(header, 0, result, 0, header.Length);
                Array.Copy(body, 0, result, header.Length, body.Length);

                return result;
            }
            else
            {
                return null;
            }
        }

        public byte[] GetEncryptedBytes(byte[] key)
        {
            // 바디의 바이트배열을 얻고 암호화
            var body = this.Body.GetBytes();
            var encryptedBody = Security.Encode(body, key);

            // 암호화된 바디의 크기를 헤더에 설정
            this.Header.BodySize = encryptedBody.Length;

            // 헤더의 바이트배열을 얻음
            var header = this.Header.GetBytes();

            // 헤더와 암호화된 바디 데이터를 합쳐 반환
            int byteSize = header.Length + encryptedBody.Length;

            if (byteSize > 0)
            {
                byte[] result = new byte[byteSize];
                Array.Copy(header, 0, result, 0, header.Length);
                Array.Copy(encryptedBody, 0, result, header.Length, encryptedBody.Length);

                return result;
            }
            else
            {
                return null;
            }
        }
    }
}

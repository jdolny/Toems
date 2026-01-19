using System.Security.Cryptography;

namespace Toems_ServiceCore.Infrastructure
{
    //http://www.alexandre-gomes.com/?p=144
    public class ServiceFileHash
    {
        protected byte[] hash;
        protected bool cancel = false;
        protected int bufferSize = 4096;
        public delegate void FileHashingProgressHandler(object sender, FileHashingProgressArgs e);
        public event FileHashingProgressHandler FileHashingProgress;
        
        public byte[] ComputeHash(Stream stream, HashAlgorithm hashAlgorithm )
        {
            cancel = false;
            hash = null;
            int _bufferSize = bufferSize; // this makes it impossible to change the buffer size while computing  

            byte[] readAheadBuffer, buffer;
            int readAheadBytesRead, bytesRead;
            long size, totalBytesRead = 0;

            size = stream.Length;
            readAheadBuffer = new byte[_bufferSize];
            readAheadBytesRead = stream.Read(readAheadBuffer, 0, readAheadBuffer.Length);

            totalBytesRead += readAheadBytesRead;

            do
            {
                bytesRead = readAheadBytesRead;
                buffer = readAheadBuffer;

                readAheadBuffer = new byte[_bufferSize];
                readAheadBytesRead = stream.Read(readAheadBuffer, 0, readAheadBuffer.Length);

                totalBytesRead += readAheadBytesRead;

                if (readAheadBytesRead == 0)
                    hashAlgorithm.TransformFinalBlock(buffer, 0, bytesRead);
                else
                    hashAlgorithm.TransformBlock(buffer, 0, bytesRead, buffer, 0);

                FileHashingProgress(this, new FileHashingProgressArgs(totalBytesRead, size));
            } while (readAheadBytesRead != 0 && !cancel);

            if (cancel)
                return hash = null;

            return hash = hashAlgorithm.Hash;
        }

        public int BufferSize
        {
            get
            { return bufferSize; }
            set
            { bufferSize = value; }
        }

        public byte[] Hash
        {
            get
            { return hash; }
        }

        public void Cancel()
        {
            cancel = true;
        }

        public override string ToString()
        {
            string hex = "";
            foreach (byte b in Hash)
                hex += b.ToString("x2");

            return hex;
        }
    }

    public class FileHashingProgressArgs
    {
        public long ProcessedSize { get; set; }
        public long TotalSize { get; set; }
        public int Percent { get; set; }

        public FileHashingProgressArgs(long totalBytesRead, long size)
        {
            ProcessedSize = totalBytesRead;
            TotalSize = size;
            if (size > 0 && totalBytesRead > 0)
            {
                var dec = (double) totalBytesRead / (double) size;
                var tmp = (dec * 100).ToString("##");
                int percentTemp;
                if (!int.TryParse(tmp, out percentTemp))
                    Percent = -1;
                else
                {
                    Percent = percentTemp;
                }
            }
            else
            {
                Percent = -1;
            }
        }
    }
}

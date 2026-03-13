namespace Toems_Common.Dto
{
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
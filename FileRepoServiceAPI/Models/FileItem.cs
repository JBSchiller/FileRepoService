namespace FileRepoServiceApi.Models
{
    public class FileItem
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public byte[] Contents { get; set; } = new byte[0]; 
        public float Version { get; set; } = float.MinValue;
        public string FileType { get; set; } = string.Empty;
        public long Size { get; set; } = long.MinValue;
        public DateTime LastModified { get; set; }
        public string Checksum { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
     
    }
}
    
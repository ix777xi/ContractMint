namespace Document_Blockchain.IInternalServices
{
    public interface IUserServices
    {
        public long UserID { get; }

        public string Email { get; }

        public string Name { get; }

        public int GroupID { get; }
    }
}

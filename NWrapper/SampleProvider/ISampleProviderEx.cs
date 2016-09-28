namespace NWrapper
{
    public interface ISampleProviderEx : NAudio.Wave.ISampleProvider
    {
        bool Enabled { get; set; }
    }
}
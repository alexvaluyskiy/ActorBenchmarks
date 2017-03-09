namespace SpawnBenchmark
{
    internal class SpawnRequest
    {
        public SpawnRequest(long div, long num, long size)
        {
            Div = div;
            Num = num;
            Size = size;
        }

        public long Div { get; }

        public long Num { get; }

        public long Size { get; }
    }
}

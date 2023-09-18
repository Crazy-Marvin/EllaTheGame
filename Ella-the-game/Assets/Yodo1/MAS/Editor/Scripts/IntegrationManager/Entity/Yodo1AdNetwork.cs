using System;
namespace Yodo1.MAS
{
    [Serializable]
    public class Yodo1AdNetwork
    {
        public Yodo1AdNetwork()
        {
        }

        public string name;
        public string version;
        public float size;
        public string repoUrl;
        public int[] supported;
        public string dependency;
        public string admobAdapterDependency;
        public string admanagerAdapterDependency;
        public string applovinAdapterDependency;
        public string ironsourceAdapterDependency;

    }
}

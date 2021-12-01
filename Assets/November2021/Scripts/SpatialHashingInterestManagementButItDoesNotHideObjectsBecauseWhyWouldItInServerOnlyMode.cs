using Mirror;

namespace JamesFrowen.NetworkBenchmark.November2021
{
    public class SpatialHashingInterestManagementButItDoesNotHideObjectsBecauseWhyWouldItInServerOnlyMode : SpatialHashingInterestManagement
    {
        public override void SetHostVisibility(NetworkIdentity identity, bool visible)
        {
            // no :)
            return;
        }
    }
}

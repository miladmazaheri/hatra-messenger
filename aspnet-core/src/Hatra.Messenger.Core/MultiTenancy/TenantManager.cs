using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Hatra.Messenger.Authorization.Users;
using Hatra.Messenger.Editions;

namespace Hatra.Messenger.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public TenantManager(
            IRepository<Tenant> tenantRepository, 
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository, 
            EditionManager editionManager,
            IAbpZeroFeatureValueStore featureValueStore) 
            : base(
                tenantRepository, 
                tenantFeatureRepository, 
                editionManager,
                featureValueStore)
        {
        }
    }
}

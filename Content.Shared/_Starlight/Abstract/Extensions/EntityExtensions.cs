using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.Abstract.Extensions;
public static class EntityExtensions
{
    public static bool TryGetEntProtoId(this EntityPrototype? entityPrototype, out EntProtoId protoId)
    {
        if (entityPrototype == null)
        {
            protoId = default;
            return false;
        }
        
        protoId = new EntProtoId(entityPrototype.ID);
        
        return true;
    }

    public static bool IsInitialized(this IEntityManager entityManager)
    {
        //This is the only way to check if entitySystemManager is initialized... Why isn't this a boolean property... FML
        try
        {
            var systemDeps = entityManager.EntitySysManager.DependencyCollection;
        }
        catch (InvalidOperationException e)
        {
            return false;
        }
        return true;
    }
}

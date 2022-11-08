namespace JinGine.Infra.Serialization;

public class StrategySerializer : BaseSerializer, ISerializer
{
    private readonly IStrategy _strategy;
    
    public StrategySerializer(IStrategy strategy) => _strategy = strategy;

    public override object Deserialize() => _strategy.Deserialize();

    public override void Serialize(object data) => _strategy.Serialize(data);
}
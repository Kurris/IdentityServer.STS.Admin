namespace IdentityServer.STS.Admin.Models;

public class SelectItem<TId, TText>
{
    public SelectItem(TId id, TText text)
    {
        Id = id;
        Text = text;
    }

    public TId Id { get; }

    public TText Text { get; }
}
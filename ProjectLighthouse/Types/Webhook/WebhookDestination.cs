namespace LBPUnion.ProjectLighthouse.Types.Webhook;

/// <summary>
/// The destination of the webhook post.
/// </summary>
public enum WebhookDestination : byte
{
    /// <summary>
    /// A channel intended for public viewing; where new levels and photos are sent.
    /// </summary>
    Public,

    /// <summary>
    /// A channel intended for moderators; where grief reports are sent.
    /// </summary>
    Moderation,

    /// <summary>
    /// A channel intended for public viewing; specifically for announcing user registrations
    /// </summary>
    Registration,
}
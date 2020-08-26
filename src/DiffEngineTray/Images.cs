using System.Drawing;
using Resourcer;

public static class Images
{
    static Images()
    {
        using var activeStream = Resource.AsStream("icon_active.ico");
        Active = new Icon(activeStream);
        using var defaultStream = Resource.AsStream("icon_default.ico");
        Default = new Icon(defaultStream);
        using var exitStream = Resource.AsStream("image_exit.png");
        Exit = Image.FromStream(exitStream);
        using var deleteStream = Resource.AsStream("image_delete.png");
        Delete = Image.FromStream(deleteStream);
        using var acceptAllStream = Resource.AsStream("image_acceptAll.png");
        AcceptAll = Image.FromStream(acceptAllStream);
        using var acceptStream = Resource.AsStream("image_accept.png");
        Accept = Image.FromStream(acceptStream);
        using var clearStream = Resource.AsStream("image_clear.png");
        Clear = Image.FromStream(clearStream);
    }

    public static Image Clear { get; }
    public static Image Accept { get; }
    public static Image AcceptAll { get; }
    public static Image Delete { get; }
    public static Image Exit { get; }
    public static Icon Active { get; }
    public static Icon Default { get; }
}
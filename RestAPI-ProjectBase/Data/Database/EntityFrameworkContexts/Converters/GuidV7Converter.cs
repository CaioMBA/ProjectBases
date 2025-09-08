using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Data.Database.EntityFrameworkContexts.Converters
{
    public class GuidV7Converter : ValueConverter<Guid, Guid>
    {
        public GuidV7Converter()
            : base(guid => guid == Guid.Empty ? Guid.CreateVersion7() : guid,
                   guid => guid)
        { }
    }

    public class NullableGuidV7Converter : ValueConverter<Guid?, Guid?>
    {
        public NullableGuidV7Converter()
            : base(
                guid => (guid.HasValue && guid.Value == Guid.Empty) ? Guid.CreateVersion7() : guid,

                guid => guid)
        { }
    }
}

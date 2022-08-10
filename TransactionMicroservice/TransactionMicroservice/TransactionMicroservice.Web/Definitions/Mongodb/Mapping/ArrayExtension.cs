using Google.Protobuf.Collections;

namespace TransactionMicroservice.Definitions.Mongodb.Mapping;

public static class ArrayExtension
{
    public static RepeatedField<T> ToRepeatedField<T>(this T[] array)
    {
        var tmp = new RepeatedField<T>();
        tmp.AddRange(array);
        return tmp;
    }
}
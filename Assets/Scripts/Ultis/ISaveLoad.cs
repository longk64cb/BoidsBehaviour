

public interface ISaveLoad<out T>
{
    T Load();

    void Save();
}

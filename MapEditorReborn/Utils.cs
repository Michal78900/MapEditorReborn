namespace MapEditorReborn;

public static class Utils
{
    /// <summary>
    /// Окрашивает текст в указанный цвет
    /// </summary>
    /// <param name="text">Текст.</param>
    /// <param name="color">Цвет.</param>
    /// <returns>Текст в указанном цвете</returns>
    public static string ToColor(this string text, string color)
    {
        return $"<color={color}>{text}</color>";
    }
}
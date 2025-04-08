namespace TaskManagement.Common
{
    public static class ColorHelper
    {
        public static string GetRandomColor()
        {
            var colors = new List<string>
            {
                "#FF5733", "#33FF57", "#3357FF", "#FF33F6", "#F6FF33", "#F633FF"
            };
            Random random = new Random();
            int index = random.Next(colors.Count);
            return colors[index];
        }
    }
}

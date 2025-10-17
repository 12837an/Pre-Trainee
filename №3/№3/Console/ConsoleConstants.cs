namespace TaskManager.Cons
{
    public static class ConsoleConstants
    {
        public const string MENU_SHOW_TASKS = "1";
        public const string MENU_ADD_TASK = "2";
        public const string MENU_CHANGE_STATUS = "3";
        public const string MENU_REMOVE_TASK = "4";
        public const string MENU_EXIT = "5";

        public static class Confirmation
        {
            public static readonly string[] YesValues = { "y", "yes" };
            public static readonly string[] YesValuesRu = { "д", "да" };
            public static readonly string[] NoValues = { "n", "no" };
            public static readonly string[] NoValuesRu = { "н", "нет" };
            public static readonly string[] AllYesValues = YesValues.Concat(YesValuesRu).ToArray();
            public static readonly string[] AllNoValues = NoValues.Concat(NoValuesRu).ToArray();
            public const string PromptMessage = "Задача выполнена? (y/n/д/н): ";
            public const string ErrorMessage = "Ошибка. Необходимо ввести y/n (д/н)\n";
        }
    }
}
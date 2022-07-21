namespace WpfDataGrid.PaintPreparationProcess
{
    /// <summary>
    /// индексы источников изменений в расчете рецепта
    /// </summary>
    internal static class IndexSourceChangesList
    {
        const int beforeTestIS = 10001;
        const int afterTestIS = 10002;
        const int testIS = 10003;
        const int tareIS = 10004;
        const int previousIterationBruttoIS = 10005;

        const int paintIS = 20000;
        const int thinnerIS = 30000;

        /// <summary>
        /// индекс для "смесь + тара до теста" 10001
        /// </summary>
        public static int BeforeTestIS { get { return beforeTestIS; } }
        /// <summary>
        /// индекс для "смесь + тара после теста" 10002
        /// </summary>
        public static int AfterTestIS { get { return afterTestIS; } }
        /// <summary>
        /// индекс для "тест" 10003
        /// </summary>
        public static int TestIS { get { return testIS; } }
        /// <summary>
        /// индекс для "тара" 10004
        /// </summary>
        public static int TareIC { get { return tareIS; } }
        /// <summary>
        /// индекс для "смесь + тара после теста предыдущей итерации" 10005
        /// </summary>
        public static int PreviousIterationBruttoIS { get { return previousIterationBruttoIS; } }


        /// <summary>
        /// индекс для миксов 20000
        /// </summary>
        public static int PaintIS { get { return paintIS; } }
        /// <summary>
        /// индекс для разбавителя 30000
        /// </summary>
        public static int ThinnerIS { get { return thinnerIS; } }
    }
}

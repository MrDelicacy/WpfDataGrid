using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using WpfDataGrid.SQLiteOperation;


namespace WpfDataGrid.PaintPreparationProcess
{
    /// <summary>
    /// модель вкладки расчета рецепта,
    /// содержит информацию о компонентах краски и методы работы сними, использует две коллекции компонентов: 
    /// для расчета пропорционального соотношения компонентов  и хранения текущей пропорции.
    /// </summary>
    internal partial class MixingComponentsTab : MixingComponentsTabBase
    {
        /// <summary>
        /// список компонентов краски, используемая для расчета рецепта
        /// </summary>
        public ObservableCollection<UsedInCalculationPaintComponent> Components { get; set; }


        private PaintRecipe paintRecipe;
        /// <summary>
        /// список компонентов краски, используемая в рецепте
        /// </summary>
        public PaintRecipe PaintRecipe { get { return paintRecipe; } }


        /// <summary>
        /// список компонентов краски, потраченных на тест в предыдущей итерации
        /// </summary>
        public List<WastePaintComponent> WastePaintComponents { get; set; }


        /// <summary>
        /// разбавитель в расчете рецепта
        /// </summary>
        private UsedInCalculationPaintComponent thinner;


        public delegate void AddWorkProcessActionHandler(WorkProcessAction processAction);
        /// <summary>
        /// возникает при изменении данных на влкладке расчета рецепта
        /// </summary>
        public event AddWorkProcessActionHandler AddWorkProcessActionNotify;
        public delegate void RemoveWorkProcessActionHandler(int indexSource);
        /// <summary>
        /// возникает при изменении удалении компонента из расчета рецепта
        /// </summary>
        public event RemoveWorkProcessActionHandler RemoveWorkProcessActionNotify;

        

        public MixingComponentsTab():base()
        {
            Components = new ObservableCollection<UsedInCalculationPaintComponent>();

            Components.CollectionChanged += this.OnCollectionChanged;

            paintRecipe = new PaintRecipe();
            thinner = new UsedInCalculationPaintComponent("Thinner");
            thinner.AddAmountNotify += ChangeAddAmountComponent;
            CanDeleteRow = true;

            stepBackLimit = 6;//это минимальное количество источников изменений в расчете рецепта
        }

        /// <summary>
        /// вызывается при добавлении компонента краски в список "Components"
        /// </summary>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (UsedInCalculationPaintComponent newItem in e.NewItems)
                    {
                        newItem.AddAmountNotify += ChangeAddAmountComponent;
                        paintRecipe.RecipeComponents.Add(new UsedInRecipePaintComponent(newItem.ComponentName));
                        AddWorkProcessActionNotify.Invoke(GetProcessActionInfo(newItem));
                    }
                    stepBackLimit++;
                }
            }
            if (e.OldItems != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove && iterationId==0)
                {
                    
                    foreach (UsedInCalculationPaintComponent oldItem in e.OldItems)
                    {
                        oldItem.AddAmountNotify -= ChangeAddAmountComponent;
                        int indexSource = IndexSourceChangesList.PaintIS + e.OldStartingIndex;
                        RemoveWorkProcessActionNotify.Invoke(indexSource);
                        paintRecipe.RecipeComponents.RemoveAt(e.OldStartingIndex);
                    }
                    stepBackLimit--;
                }
                
            }
        }

        /// <summary>
        /// показывает компонент краски "разбавитель" в расчете рецепта краски
        /// </summary>
        public UsedInCalculationPaintComponent Thinner
        {
            get { return thinner; }
            set { thinner = value; }
        }


        /// <summary>
        /// изменяет добавляемое количество в компонент, используемый в расчете рецепта
        /// и пересчитывает пропорцию
        /// </summary>
        /// <param name="sender">компонент</param>
        /// <param name="addAmount">добавленное количество</param>
        private void ChangeAddAmountComponent(object sender, float addAmount)
        {

            UsedInCalculationPaintComponent paintComponent = sender as UsedInCalculationPaintComponent;
            if (paintComponent.ComponentName == "Thinner")
            {
                if (Components.Count > 0)
                {
                    thinner.AbsoluteAmount = addAmount + PaintRecipe.Thinner.AbsoluteAmount;
                    CalculateProportion.Calculate(Components, Thinner,out totalPaintComponentWeight);  
                }
            }
            else
            {
                paintComponent.AbsoluteAmount = addAmount + PaintRecipe.RecipeComponents[Components.IndexOf(paintComponent)].AbsoluteAmount;
                CalculateProportion.Calculate(Components, Thinner, out totalPaintComponentWeight);
            }
            OnPropertyChanged("TotalPaintComponentWeight");
            TotalPaintComponentWeightWithOutThinner = totalPaintComponentWeight - Thinner.AbsoluteAmount;
            AddWorkProcessActionNotify.Invoke(GetProcessActionInfo(paintComponent));
        }


        /// <summary>
        /// устанавливает данные в рецепт краски, обнуляет данные в расчете рецепта
        /// </summary>
         public void SetRecipePaintComponents()
        {
            IterationId++;
            CanDeleteRow = false;
            if (AfterTest != 0)
            PreviousIterationBrutto = AfterTest;
            BeforeTest = 0;
            AfterTest = 0;

            for (int i = 0; i < PaintRecipe.RecipeComponents.Count; i++)
            {
                PaintRecipe.RecipeComponents[i].AbsoluteAmount = Components[i].AbsoluteAmount;
                Components[i].AbsoluteAmount = 0;
                Components[i].AddAmount = 0;
            }
            PaintRecipe.Thinner.AbsoluteAmount = Thinner.AbsoluteAmount;
            Thinner.AbsoluteAmount = 0;
            Thinner.AddAmount = 0;
        }


        /// <summary>
        ///  устанавливает список пораченных на тест компонентов и пересчитывает абсолютное количество каждого компонента краски
        /// </summary>
        public void SetWastePaintComponents()
        {
            WastePaintComponents = TestCostCalculation.CalculateWasteComponents(Components, Thinner, Test);
        }


        /// <summary>
        /// устанавливает значения для рецепта и расчета рецепта из предыдущей итерации
        /// </summary>
        /// <param name="index">источник изменений</param>
        /// <param name="addAmount">добавленное количество</param>
        /// <param name="absolute">абсолютное количество</param>
        /// <param name="test">затраты на тест</param>
        public void SetPreviousRecipePaitComponents(int index, float addAmount,float absolute, float test)
        {
            if (index==30000)
            {
                PaintRecipe.Thinner.AbsoluteAmount = test + absolute - addAmount;
                Thinner.AddAmount = addAmount;
            }
            else
            {
                PaintRecipe.RecipeComponents[index].AbsoluteAmount = test + absolute - addAmount;
                Components[index].AddAmount = addAmount;
            }
        }
    }


    internal partial class MixingComponentsTab
    {
        private float beforeTest;
        /// <summary>
        /// показывает вес готовой смеси и тары до теста
        /// </summary>
        public float BeforeTest
        {
            get { return beforeTest; }
            set
            {
                beforeTest = value;
                AddWorkProcessActionNotify.Invoke(GetProcessActionInfo(beforeTest, IndexSourceChangesList.BeforeTestIS));
                OnPropertyChanged("BeforeTest");
            }
        }
        private float afterTest;
        /// <summary>
        /// показывает вес готовой смеси и тары после теста
        /// </summary>
        public float AfterTest
        {
            get { return afterTest; }
            set
            { 
                afterTest = value;
                AddWorkProcessActionNotify.Invoke(GetProcessActionInfo(afterTest, IndexSourceChangesList.AfterTestIS));
                SetTest();
                OnPropertyChanged("AfterTest");
            }
        }
        private float test;
        /// <summary>
        /// показывает затраты на тест
        /// </summary>
        public float Test
        {
            get { return test; }
            set 
            {
                test = value;
                AddWorkProcessActionNotify.Invoke(GetProcessActionInfo(test, IndexSourceChangesList.TestIS));
                OnPropertyChanged("Test");
            }
        }
        private float tare;
        /// <summary>
        /// показывает вес тары
        /// </summary>
        public float Tare
        {
            get { return tare; }
            set 
            { 
                tare = value;
                AddWorkProcessActionNotify.Invoke(GetProcessActionInfo(tare, IndexSourceChangesList.TareIC));
                OnPropertyChanged("Tare");
            }
        }
        private float previousIterationBrutto;
        /// <summary>
        /// показывает вес тары и смеси предыдущей итерации
        /// </summary>
        public float PreviousIterationBrutto
        {
            get { return previousIterationBrutto; }
            set 
            {
                previousIterationBrutto = value;
                AddWorkProcessActionNotify.Invoke(GetProcessActionInfo(previousIterationBrutto, IndexSourceChangesList.PreviousIterationBruttoIS));
                OnPropertyChanged("PreviousIterationBrutto");
            }
        }
        /// <summary>
        /// считает сколько краски затраченно на тест
        /// </summary>
        private void SetTest()
        {
            if ((BeforeTest - AfterTest) < 0)
            {
                MessageBox.Show("Затраты на тест не могут быть отрицательными!");
                Test = 0; 
            }
            else
            Test= BeforeTest - AfterTest;
        }


        private float totalPaintComponentWeight;
        /// <summary>
        /// показывает вес компонентов краски
        /// </summary>
        public float TotalPaintComponentWeight
        {
            get { return totalPaintComponentWeight; }
            set
            {
                totalPaintComponentWeight = value;
                OnPropertyChanged("TotalPaintComponentWeight");
            }
        }


        private float totalPaintComponentWeightWithOutThinner;
        /// <summary>
        /// показывает вес компонентов краски без разбавителя
        /// </summary>
        public float TotalPaintComponentWeightWithOutThinner
        {
            get { return totalPaintComponentWeightWithOutThinner; }
            set
            {
                totalPaintComponentWeightWithOutThinner = value;
                OnPropertyChanged("TotalPaintComponentWeightWithOutThinner");
            }
        }
        /// <summary>
        /// возвращает WorkProcessBook с заполнеными данными
        /// </summary>
        private WorkProcessAction GetProcessActionInfo(float amount,int index)
        {
            WorkProcessAction processAction = new WorkProcessAction();
            processAction.CustomerOrderId = this.OrderDetail.CustomerOrderId;
            processAction.IterationId = this.IterationId;
            processAction.Amount = amount;
            processAction.IndexSourceChanges = index;
            return processAction;
        }
        /// <summary>
        /// возвращает WorkProcessBook с заполнеными данными
        /// </summary>
        private WorkProcessAction GetProcessActionInfo(UsedInCalculationPaintComponent comp)
        {
            WorkProcessAction processAction = new WorkProcessAction();
            processAction.CustomerOrderId = this.OrderDetail.CustomerOrderId;
            processAction.IterationId = this.IterationId;

            processAction.Amount = comp.AddAmount;
            if (comp.ComponentName != "Thinner")
                processAction.IndexSourceChanges = IndexSourceChangesList.PaintIS + Components.IndexOf(comp);
            else processAction.IndexSourceChanges = IndexSourceChangesList.ThinnerIS;

            return processAction;
        }

        /// <summary>
        /// показывает количество записей в WorkProcessActions, при котором кнопка "шаг назад" не активна
        /// </summary>
        public int stepBackLimit;

        private bool canDeleteRow;
        /// <summary>
        /// показывает можно ли удалять строку
        /// </summary>
        public bool CanDeleteRow
        {
            get { return canDeleteRow; }
            set
            {
                canDeleteRow = value;
                OnPropertyChanged("CanDeleteRow");
            }
        }
    }
}

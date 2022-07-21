using System.Collections.Generic;
using System.Collections.ObjectModel;
using WpfDataGrid.PaintPreparationProcess;
using WpfDataGrid.SQLiteOperation;
using System.Data.Entity;
using System.Linq;
using WpfDataGrid.SecondaryWindows;
using WpfDataGrid.SecondaryWindows.CreateOrder;
using System.Windows;
using System;
using System.Data;

namespace WpfDataGrid.ViewModel
{
    internal class AppViewModel:RightPanelElementsViewModel
    {
        private MixingComponentsTab selectedMixingComponentsTab;
        private DataTable iterationsDetailTable;
        private DataTable testCostTable;
        private RelayCommand createOrderCommand;
        private RelayCommand createOrderBasedOnCurrentCommand;
        private RelayCommand openOrderCommand;
        private RelayCommand closeOrderCommand;
        private RelayCommand completeOrderCommand;
        private RelayCommand closeWindowCommand;
        private RelayCommand iteration;
        private RelayCommand cancelIteration;
        private RelayCommand undoLastActionCommand;
        private RelayCommand redoLastActionCommand;
        private CustomerOrderInfo selectedOrderInfoRow;
        private RelayCommand previewRecipeCommand;
        private RelayCommand onScaleCommand;
        private RelayCommand useProportionCommand;
        private RelayCommand removeOrderCommand;
        private string orderId;
        private string customerName;
        private string manufacturer;
        private string colorGroup;
        private string colorCode;
        private DateTime orderDate;
        private RelayCommand fastSearchCommand;
        /// <summary>
        /// id выбранного заказа
        /// </summary>
        private int selectedOrderId;

        /// <summary>
        /// количество записей в таблице последних заказов
        /// </summary>
        private int orderInfosRowsCount;

        /// <summary>
        /// контекст данных для PrismDB
        /// </summary>
        private ConnectToPrismDB db;


        /// <summary>
        ///  список вкладок главной рабочей области
        /// </summary>
        public ObservableCollection<MixingComponentsTab> MixingComponentsTabs { get; set; }

        /// <summary>
        /// список заказов
        /// </summary>
        private IEnumerable<CustomerOrder> customerOrders;


        /// <summary>
        /// подробности заказов
        /// </summary>
        private IEnumerable<OrderDetail> orderDetails;


        /// <summary>
        /// хранит контрольные суммы, для отслеживания изменений в расчете рецепта
        /// </summary>
        private Dictionary<int, float> ControlSumms;

        public AppViewModel()
        {
            MixingComponentsTabs = new ObservableCollection<MixingComponentsTab>();
            db = new ConnectToPrismDB();
            db.CustomerOrders.Load();
            db.OrderDetails.Load();
            customerOrders = db.CustomerOrders.Local.ToBindingList();
            orderDetails = db.OrderDetails.Local.ToBindingList();
            writeProcessAction = true;
            ControlSumms = new Dictionary<int, float>();
            orderInfosRowsCount = 20;
            SetCustomerOrderInfo(orderInfosRowsCount);
        }


        /// <summary>
        /// выбранная вкладка расчета рецепта
        /// </summary>
        public MixingComponentsTab SelectedMixingComponentsTab
        {
            get { return selectedMixingComponentsTab; }
            set
            {
                selectedMixingComponentsTab = value;

                if (selectedMixingComponentsTab != null)
                {
                    selectedOrderId = selectedMixingComponentsTab.OrderDetail.CustomerOrderId;
                    
                    if (selectedMixingComponentsTab.IterationId > 0) 
                    {
                        IterationsDetailTable = IterationsDetail.GetIterationDetailTableAsync(db, selectedOrderId).Result;
                        TestCostTable = IterationsTestCost.GetTestCostTableAsync(db, selectedOrderId).Result;
                    }                    
                }
                OnPropertyChanged("SelectedMixingComponentsTab");
            }
        }

        /// <summary>
        /// таблица пропорций компонентов на каждой итерации
        /// </summary>
        public DataTable IterationsDetailTable
        {
            get { return iterationsDetailTable; }
            set
            {
                iterationsDetailTable = value;
                OnPropertyChanged("IterationsDetailTable");
            }
        }
        /// <summary>
        /// таблица добавленных и затраченных на тест компонентов каждой итерации
        /// </summary>
        public DataTable TestCostTable
        {
            get { return testCostTable; }
            set
            {
                testCostTable = value;
                OnPropertyChanged("TestCostTable");
            }
        }


        #region создание и открытие заказа
        /******************************************************************/
        /// <summary>
        /// устанавливает контрольную сумму для текущего расчета рецепта
        /// </summary>
        private float GetControlSumm()
        {
            var result = GetCurrentWorkProcessActions();
            float summ = 0;
            foreach (WorkProcessAction w in result)
                summ += w.Amount;
            return summ;
        }


        /// <summary>
        /// получает из базы данных набор строк WorkProcessAction для текущего расчета рецепта,
        /// используется для получения контрольной суммы
        /// </summary>
        private IEnumerable<WorkProcessAction> GetCurrentWorkProcessActions()
        {
            var workProcessActions = from wpa in db.WorkProcessActions
                                     where wpa.CustomerOrderId == selectedOrderId
                                     select wpa;
            var result = workProcessActions.ToList();
            return result;
        }
        
        /// <summary>
        /// создает модель вкладки расчета рецепта
        /// </summary>
        private MixingComponentsTab CreateOrder(CreateOrderViewModel viewModel)
        {
            CustomerOrder order = new CustomerOrder();
            order.CustomerId = viewModel.SelectedCustomer.Id;
            order.OrderDate = System.DateTime.Now.Date;
            order.Status = "inwork";
            db.CustomerOrders.Add(order);
            db.SaveChanges();

            OrderDetail detail = new OrderDetail();
            detail.Id = orderDetails.Count()!=0 ? orderDetails.Last().Id + 1 : 1;
            detail.CustomerOrderId = customerOrders.Last().Id;
            detail.Manufacturer = viewModel.SelectedManufacturer.ManufacturerName;
            detail.ColorCode = viewModel.ColorCode;
            detail.ColorGroup = viewModel.ColorGroup;
            detail.ColorName = viewModel.ColorName;
            detail.Comment = viewModel.Comment;
            db.OrderDetails.Add(detail);
            db.SaveChanges();

            MixingComponentsTab m = new MixingComponentsTab();
            m.AddWorkProcessActionNotify += AddWorkProcessAction;
            m.RemoveWorkProcessActionNotify += RemoveWorkProcessAction;
            m.OrderDetail = detail;
            m.CustomerName = viewModel.SelectedCustomer.CustomerName;
            m.BeforeTest = 0;
            m.AfterTest = 0;
            m.Tare = viewModel.Tare;
            m.PreviousIterationBrutto = viewModel.Tare;

            return m;
        }

        /// <summary>
        /// команда создания заказа
        /// </summary>
        public RelayCommand CreateOrderCommand
        {
            get
            {
                return createOrderCommand ??
                    (createOrderCommand = new RelayCommand(obj =>
                    {
                        CreateOrderViewModel viewModel = new CreateOrderViewModel();
                        CreateOrderWindow createOrder = new CreateOrderWindow(viewModel);
                        if (createOrder.ShowDialog() == true)
                        {
                            MixingComponentsTab tab = CreateOrder(viewModel);

                            MixingComponentsTabs.Add(tab);
                            SelectedMixingComponentsTab = tab;
                            ControlSumms.Add(selectedOrderId, GetControlSumm());
                            SetCustomerOrderInfo(orderInfosRowsCount);
                        }
                    }));
            }
        }


        /// <summary>
        /// команда создания заказа на основе текущего
        /// </summary>
        public RelayCommand CreateOrderBasedOnCurrentCommand
        {
            get
            {
                return createOrderBasedOnCurrentCommand ??
                    (createOrderBasedOnCurrentCommand = new RelayCommand(obj =>
                    {
                        CreateOrderViewModel viewModel = new CreateOrderViewModel();

                        viewModel.SetOrderDetail(SelectedMixingComponentsTab.OrderDetail);

                        CreateOrderWindow createOrder = new CreateOrderWindow(viewModel);
                        if (createOrder.ShowDialog() == true)
                        {
                            MixingComponentsTab tab = CreateOrder(viewModel);

                           if (SelectedMixingComponentsTab != null)
                            {
                                UseProportionWindow useProportion = new UseProportionWindow(SelectedMixingComponentsTab.PaintRecipe,true);
                                if (useProportion.ShowDialog() == true)
                                {
                                    for (int i = 0; i < useProportion.newRecipe.RecipeComponents.Count; i++) 
                                    {
                                        UsedInCalculationPaintComponent component = new UsedInCalculationPaintComponent();
                                        component.ComponentName = useProportion.newRecipe.RecipeComponents[i].ComponentName;
                                        tab.Components.Add(component);
                                        tab.Components[i].AddAmount = useProportion.newRecipe.RecipeComponents[i].AbsoluteAmount;
                                    }
                                    tab.Thinner.AddAmount = 0;//инициализируем нулем, чтобы можно было сделать шаг назад
                                    tab.Thinner.AddAmount = useProportion.newRecipe.Thinner.AbsoluteAmount;//устанавливаем значение
                                }
                            }
                            MixingComponentsTabs.Add(tab);
                            SelectedMixingComponentsTab = tab;

                            ControlSumms.Add(selectedOrderId, GetControlSumm());
                            SetCustomerOrderInfo(orderInfosRowsCount);
                        }
                    }, (obj) => MixingComponentsTabs.Count() > 0));
            }
        }

        private void OpenOrderBySelectedOrderInfoRow()
        {
            MixingComponentsTab m1 = new MixingComponentsTab();
            m1.AddWorkProcessActionNotify += AddWorkProcessAction;

            m1.OrderDetail = SelectedOrderInfoRow.OrderDetail;
            m1.CustomerName = SelectedOrderInfoRow.CustomerName;


            var weightInfo = from twi in db.TestWeightInfoes
                             where twi.CustomerOrderId == SelectedOrderInfoRow.OrderDetail.CustomerOrderId
                             select twi;
            if (weightInfo.Count() > 0)
            {
                TestWeightInfo testWeight = weightInfo.ToList().Last();

                m1.IterationId = testWeight.IterationId;

                var iterationRows = from it in db.OrderIterationRows
                                    where it.CustomerOrderId == SelectedOrderInfoRow.OrderDetail.CustomerOrderId
                                    && it.IterationId == testWeight.IterationId
                                    select it;

                foreach (OrderIterationRow itRow in iterationRows)
                {
                    if (itRow.IndexSourceChanges == IndexSourceChangesList.ThinnerIS)
                        m1.Thinner.AbsoluteAmount = itRow.AbsoluteAmount;
                    else
                    {
                        UsedInCalculationPaintComponent component = new UsedInCalculationPaintComponent();
                        component.ComponentName = itRow.ComponentName;
                        component.AbsoluteAmount = itRow.AbsoluteAmount;
                        m1.Components.Add(component);
                    }
                }

                m1.SetRecipePaintComponents();

                m1.Tare = testWeight.Tare;
                m1.PreviousIterationBrutto = testWeight.PreviousIterationBrutto;
            }

            MixingComponentsTabs.Add(m1);
            SelectedMixingComponentsTab = m1;
            ControlSumms.Add(selectedOrderId, GetControlSumm());

            CancelIterationActivity = true;
        }

        /// <summary>
        /// команда открытия заказа
        /// </summary>
        public RelayCommand OpenOrderCommand
        {
            get
            {
                return openOrderCommand ??
                    (openOrderCommand = new RelayCommand(obj =>
                    {
                        if (selectedOrderInfoRow != null)
                        {
                            if (ControlSumms.ContainsKey(SelectedOrderInfoRow.OrderDetail.CustomerOrderId))
                                MessageBox.Show("Этот заказ уже открыт.", "Предупреждение!");
                            else
                            {
                                OpenOrderBySelectedOrderInfoRow();
                            }
                        }
                    }));
            }
        }
        #endregion


        #region закрытие и завершение заказа
        /******************************************************************/
        /// <summary>
        /// закрывает вкладку расчета рецепта
        /// </summary>
        private void CloseOrder()
        {
            if (CheckingChanges())
            {
                AddIterationInfo();
                SelectedMixingComponentsTab.AddWorkProcessActionNotify -= AddWorkProcessAction;
                ClearWorkProcessBooks();
                MixingComponentsTabs.Remove(SelectedMixingComponentsTab);
            }
            else
            {
                if (SelectedMixingComponentsTab.Components.Count == 0) 
                {
                    RemoveEmptyOrder(selectedOrderId);
                }
                else
                {
                    AddIterationInfo();
                }
                SelectedMixingComponentsTab.AddWorkProcessActionNotify -= AddWorkProcessAction;
                ClearWorkProcessBooks();
                MixingComponentsTabs.Remove(SelectedMixingComponentsTab);
            }
            if (MixingComponentsTabs.Count > 0)
                SelectedMixingComponentsTab = MixingComponentsTabs.Last();
            else
                StepBackActivity = false;
        }


        /// <summary>
        /// удаляет пустой заказ
        /// </summary>
        private void RemoveEmptyOrder(int orderId)
        {
            var detail = db.OrderDetails.Where(o => o.CustomerOrderId == orderId);
            if (detail != null)
            {
                db.OrderDetails.RemoveRange(detail);
                var order = db.CustomerOrders.Where(o => o.Id == orderId);
                db.CustomerOrders.RemoveRange(order);
                db.SaveChanges();
            }
        }


        /// <summary>
        /// проверяет наличие изменений в расчете рецепта
        /// </summary>
        /// <returns>true, если изменения были</returns>
        private bool CheckingChanges()
        {
            bool changes = false;

            var result = GetCurrentWorkProcessActions();
            float summ = 0;
            foreach (WorkProcessAction w in result)
                summ += w.Amount;

            if (summ != ControlSumms[selectedOrderId])
                changes = true;

            return changes;
        }

        /// <summary>
        /// получает из базы данных набор строк UndoWorkProcessAction для текущего расчета рецепта
        /// </summary>
        private IEnumerable<UndoWorkProcessAction> GetCurrentUndoWorkProcessActions()
        {
            var undoActions = from wpa in db.UndoWorkProcessActions
                              where wpa.CustomerOrderId == selectedOrderId
                              select wpa;
            var result = undoActions.ToList();
            return result;
        }


        /// <summary>
        /// удаляет из базы данных изменения в расчете рецепта для текущего заказа
        /// </summary>
        private void ClearWorkProcessBooks()
        {

            db.WorkProcessActions.RemoveRange(GetCurrentWorkProcessActions());
            db.UndoWorkProcessActions.RemoveRange(GetCurrentUndoWorkProcessActions());
            db.SaveChanges();

            ControlSumms.Remove(selectedOrderId);
        }


        /// <summary>
        /// команда закрытия заказа
        /// </summary>
        public RelayCommand CloseOrderCommand
        {
            get
            {
                return closeOrderCommand ??
                    (closeOrderCommand = new RelayCommand(obj =>
                    {
                        if (SelectedMixingComponentsTab != null)
                        {
                            CloseOrder();
                            SetCustomerOrderInfo(orderInfosRowsCount);
                        }
                    }, (obj) => MixingComponentsTabs.Count() > 0));
            }
        }


        /// <summary>
        /// команда завершения заказа
        /// </summary>
        public RelayCommand CompleteOrderCommand
        {
            get
            {
                return completeOrderCommand ??
                    (completeOrderCommand = new RelayCommand(obj =>
                    {
                        if (SelectedMixingComponentsTab != null)
                        {

                            CompletedWork completedWork = new CompletedWork() { CustomerOrderId = selectedOrderId };
                            completedWork.Id = db.CompletedWorks.Count() != 0 ? db.CompletedWorks.ToList().Last().Id + 1 : 1;
                            CompleteOrderWindow window = new CompleteOrderWindow(completedWork);
                            if (window.ShowDialog() == true)
                            {
                                try
                                {
                                    CustomerOrder order = db.CustomerOrders.Find(selectedOrderId);
                                    if (order.Status == "inwork")
                                    {
                                        order.Status = "complete";
                                        db.CompletedWorks.Add(window.CompletedWork);
                                    }
                                    else
                                    {
                                        CompletedWork cw = db.CompletedWorks.Where(c => c.CustomerOrderId == selectedOrderId).FirstOrDefault();
                                        cw.TypeWork = completedWork.TypeWork;
                                        cw.SpectroTinting = completedWork.SpectroTinting;
                                        cw.CarTinting = completedWork.CarTinting;
                                        cw.Tinting = completedWork.Tinting;
                                        cw.SpectroPreparation = completedWork.SpectroPreparation;
                                        cw.RecipeRaiting = completedWork.RecipeRaiting;
                                    }
                                    //db.SaveChanges();
                                    CloseOrder();
                                    SetCustomerOrderInfo(orderInfosRowsCount);
                                }
                                catch(Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }

                        }
                    }, (obj) => MixingComponentsTabs.Count() > 0));
            }
        }


        /// <summary>
        /// команда закрытия приложения
        /// </summary>
        public RelayCommand CloseWindowCommand
        {
            get
            {
                return closeWindowCommand ??
                    (closeWindowCommand = new RelayCommand(obj =>
                    {
                        while (MixingComponentsTabs.Count > 0)
                        {
                            CloseOrder();
                        }
                        if (ButtonScaleConnectionState)
                        {
                            scaleConnect.StopConnection();
                            scaleConnect.ScaleStringNotify -= SetScaleString;
                            ScaleConnectionButtonText = "подключить";
                            ScaleLable = "весы отключены";
                        }
                    }));
            }
        }
        #endregion


        #region итерации
        /******************************************************************/
        /// <summary>
        /// добавляет данные о таре и затратах на тест в бд
        /// </summary>
        private void AddTestWeightInfo()
        {
            TestWeightInfo testWeight = new TestWeightInfo();
            testWeight.CustomerOrderId = selectedOrderId;
            testWeight.IterationId = SelectedMixingComponentsTab.IterationId;
            testWeight.Tare = SelectedMixingComponentsTab.Tare;
            testWeight.PreviousIterationBrutto = SelectedMixingComponentsTab.PreviousIterationBrutto;
            testWeight.TestBefore = SelectedMixingComponentsTab.BeforeTest;
            testWeight.TestAfter = SelectedMixingComponentsTab.AfterTest;
            testWeight.Test = SelectedMixingComponentsTab.Test;
            db.TestWeightInfoes.Add(testWeight);
            db.SaveChanges();
        }


        /// <summary>
        /// добавляет информацию о текущей итерации в бд
        /// </summary>
        private void AddIterationInfo()
        {
            SelectedMixingComponentsTab.SetWastePaintComponents();

            AddTestWeightInfo();

            for (int i = 0; i < SelectedMixingComponentsTab.Components.Count; i++)
            {
                OrderIterationRow iterationRow = new OrderIterationRow();
                iterationRow.CustomerOrderId = selectedOrderId;
                iterationRow.IterationId = SelectedMixingComponentsTab.IterationId;
                iterationRow.ComponentName = SelectedMixingComponentsTab.Components[i].ComponentName;
                iterationRow.AddAmount = SelectedMixingComponentsTab.Components[i].AddAmount;
                iterationRow.AbsoluteAmount = SelectedMixingComponentsTab.Components[i].AbsoluteAmount;
                iterationRow.IndexSourceChanges = i;

                iterationRow.Test = SelectedMixingComponentsTab.WastePaintComponents[i].ComponentCost;
                db.OrderIterationRows.Add(iterationRow);
            }
            if (SelectedMixingComponentsTab.Thinner.PercentageAmount != 0)
            {
                OrderIterationRow iterationRow = new OrderIterationRow();
                iterationRow.CustomerOrderId = selectedOrderId;
                iterationRow.IterationId = SelectedMixingComponentsTab.IterationId;
                iterationRow.ComponentName = SelectedMixingComponentsTab.Thinner.ComponentName;
                iterationRow.AddAmount = SelectedMixingComponentsTab.Thinner.AddAmount;
                iterationRow.AbsoluteAmount = SelectedMixingComponentsTab.Thinner.AbsoluteAmount;
                iterationRow.IndexSourceChanges = IndexSourceChangesList.ThinnerIS;
                iterationRow.Test = SelectedMixingComponentsTab.WastePaintComponents[SelectedMixingComponentsTab.Components.Count].ComponentCost;
                db.OrderIterationRows.Add(iterationRow);
            }
            SelectedMixingComponentsTab.SetRecipePaintComponents();

            db.SaveChanges();
        }


        /// <summary>
        /// команда завершения текущей итерации
        /// </summary>
        public RelayCommand Iteration
        {
            get
            {
                return iteration ??
                    (iteration = new RelayCommand(obj =>
                    {
                        AddIterationInfo();
                        StepBackActivity = false;
                        StepForwardActivity = false;
                        CancelIterationActivity = true;
                        ControlSumms[selectedOrderId] = GetControlSumm();
                        IterationsDetailTable = IterationsDetail.GetIterationDetailTableAsync(db, selectedOrderId).Result;
                        TestCostTable = IterationsTestCost.GetTestCostTableAsync(db, selectedOrderId).Result;
                    }, (obj) => MixingComponentsTabs.Count() > 0));
            }
        }


        /// <summary>
        /// команда завершения текущей итерации
        /// </summary>
        public RelayCommand CancelIteration
        {
            get
            {
                return cancelIteration ??
                    (cancelIteration = new RelayCommand(obj =>
                    {
                        var itRows = (from it in db.OrderIterationRows
                                                 where it.CustomerOrderId == selectedOrderId
                                                 && it.IterationId == SelectedMixingComponentsTab.IterationId-1
                                                 select it).ToList();

                        var testWeightinfo = db.TestWeightInfoes.ToList().Last();

                        var workProcessActions = from pa in db.WorkProcessActions
                                                 where pa.CustomerOrderId == selectedOrderId
                                                 && pa.IterationId == SelectedMixingComponentsTab.IterationId
                                                 select pa;

                        var undoWorkProcessActions = from upa in db.UndoWorkProcessActions
                                                 where upa.CustomerOrderId == selectedOrderId
                                                 && upa.IterationId == SelectedMixingComponentsTab.IterationId
                                                 select upa;

                        writeProcessAction = false;

                        foreach (OrderIterationRow it in itRows)
                            SelectedMixingComponentsTab.SetPreviousRecipePaitComponents(it.IndexSourceChanges, it.AddAmount, it.AbsoluteAmount, it.Test);

                        SelectedMixingComponentsTab.Tare = testWeightinfo.Tare;
                        SelectedMixingComponentsTab.BeforeTest = testWeightinfo.TestBefore;
                        SelectedMixingComponentsTab.AfterTest = testWeightinfo.TestAfter;
                        SelectedMixingComponentsTab.PreviousIterationBrutto = testWeightinfo.PreviousIterationBrutto;

                        db.WorkProcessActions.RemoveRange(workProcessActions);
                        if (undoWorkProcessActions != null)
                            db.UndoWorkProcessActions.RemoveRange(undoWorkProcessActions);
                        db.TestWeightInfoes.Remove(testWeightinfo);
                        db.OrderIterationRows.RemoveRange(itRows);
                        db.SaveChanges();

                        writeProcessAction = true;
                        SelectedMixingComponentsTab.IterationId--;

                        StepBackActivity = false;
                        StepForwardActivity = false;
                        if (SelectedMixingComponentsTab.IterationId == 0)
                            CancelIterationActivity = false;

                        ControlSumms[selectedOrderId] = GetControlSumm();
                    }, (obj) => MixingComponentsTabs.Count() > 0));
            }
        }
        #endregion


        #region отмена и возврат последнего действия
        /******************************************************************/
        /// <summary>
        /// возвращает запись о последнем действии
        /// </summary>
        private WorkProcessAction GetLastAction()
        {
            var workProcessActions = (from pa in db.WorkProcessActions
                                      where pa.CustomerOrderId == selectedOrderId
                                      && pa.IterationId == SelectedMixingComponentsTab.IterationId
                                      select pa).ToList();
            return workProcessActions.Last();
        }


        /// <summary>
        /// возвращает запись о последнем действии по индексу источника действия
        /// </summary>
        private WorkProcessAction GetLastAction(int sourseActionIndex)
        {
                var workProcessActions = (from pa in db.WorkProcessActions
                                          where pa.CustomerOrderId == selectedOrderId
                                          && pa.IterationId == SelectedMixingComponentsTab.IterationId && pa.IndexSourceChanges == sourseActionIndex
                                          select pa).ToList();
                return workProcessActions.Last();
        }


        /// <summary>
        /// перемещает запись о последнем действии в таблицу отмененых действий
        /// </summary>
        private int TransferLastAction()
        {
            var upa = GetLastAction();

            UndoWorkProcessAction undo = new UndoWorkProcessAction();
            undo.CustomerOrderId = upa.CustomerOrderId;
            undo.IterationId = upa.IterationId;
            undo.IndexSourceChanges = upa.IndexSourceChanges;
            undo.Amount = upa.Amount;

            db.UndoWorkProcessActions.Add(undo);
            db.WorkProcessActions.Remove(upa);

            db.SaveChanges();
            return undo.IndexSourceChanges;
        }


        /// <summary>
        /// устанавливает в расчет рецепта значение из предпоследего действия
        /// </summary>
        private void SetWorkProcessActionAmount(WorkProcessAction action)
        {
            int controlTypeIndex = action.IndexSourceChanges / 10000;//определяем тип источника изменений
            int controlPointerIndex = action.IndexSourceChanges % 10000;//определяем позицию источника, для компонентов краски
            if (controlTypeIndex == 1)
            {
                switch (action.IndexSourceChanges)
                {
                    case 1001:
                        SelectedMixingComponentsTab.BeforeTest = action.Amount;
                        break;
                    case 1002:
                        SelectedMixingComponentsTab.AfterTest = action.Amount;
                        break;
                    case 1003:
                        SelectedMixingComponentsTab.Test = action.Amount;
                        break;
                    case 1004:
                        SelectedMixingComponentsTab.Tare = action.Amount;
                        break;
                    case 1005:
                        SelectedMixingComponentsTab.PreviousIterationBrutto = action.Amount;
                        break;
                }
            }
            if (controlTypeIndex == 2)
                SelectedMixingComponentsTab.Components[controlPointerIndex].AddAmount = action.Amount;

            if (controlTypeIndex == 3)
                SelectedMixingComponentsTab.Thinner.AddAmount = action.Amount;
        }


        /// <summary>
        /// возвращает запись о последнем отмененном действии
        /// </summary>
        private WorkProcessAction GetRedoWorkProcessAction()
        {
            var undoWorkProcessAction = (from pa in db.UndoWorkProcessActions
                                         where pa.CustomerOrderId == selectedOrderId
                                         && pa.IterationId == SelectedMixingComponentsTab.IterationId
                                         select pa).ToList().Last();

            WorkProcessAction action = new WorkProcessAction();
            action.CustomerOrderId = undoWorkProcessAction.CustomerOrderId;
            action.IterationId = undoWorkProcessAction.IterationId;
            action.IndexSourceChanges = undoWorkProcessAction.IndexSourceChanges;
            action.Amount = undoWorkProcessAction.Amount;

            db.UndoWorkProcessActions.Remove(undoWorkProcessAction);
            db.SaveChanges();

            return action;
        }


        /// <summary>
        /// показывает, возможна ли запись последнего действия в бд
        /// </summary>
        private bool writeProcessAction;


        /// <summary>
        /// добавляет в бд запись об из менениях на вкладке расчета рецепта,
        /// управляет активностью кнопок "шаг назад", "шаг вперед"
        /// </summary>
        private void AddWorkProcessAction(WorkProcessAction processAction)
        {
            if (writeProcessAction)
            {
                db.WorkProcessActions.Add(processAction);
                db.SaveChanges();
            }

            if (SelectedMixingComponentsTab != null)
            {
                if (SelectedMixingComponentsTab.stepBackLimit == 6 ||
                    db.WorkProcessActions.Where(m => m.IterationId == SelectedMixingComponentsTab.IterationId).Count() == SelectedMixingComponentsTab.stepBackLimit)
                    StepBackActivity = false;
                else
                    StepBackActivity = true;

                if (db.UndoWorkProcessActions.Count() > 0)
                    StepForwardActivity = true;
                else
                    StepForwardActivity = false;
            }
            
        }

        /// <summary>
        /// удаляет записи о компоненте краски
        /// </summary>
        /// <param name="indexSource">индекс компонента краски</param>
        private void RemoveWorkProcessAction(int indexSource)
        {
            var res = db.WorkProcessActions.Where(w => w.IndexSourceChanges == indexSource).ToList();
            db.WorkProcessActions.RemoveRange(res);
            db.SaveChanges();
        }

        /// <summary>
        /// команда отмены последнего действия
        /// </summary>;
        public RelayCommand UndoLastActionCommand
        {
            get
            {
                return undoLastActionCommand ??
                    (undoLastActionCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            writeProcessAction = false;
                            int index = TransferLastAction();
                            SetWorkProcessActionAmount(GetLastAction(index));
                            writeProcessAction = true;
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }));
            }
        }


        /// <summary>
        /// команда возврата последнего действия
        /// </summary>
        public RelayCommand RedoLastActionCommand
        {
            get
            {
                return redoLastActionCommand ??
                    (redoLastActionCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            SetWorkProcessActionAmount(GetRedoWorkProcessAction());
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }));
            }
        }
        #endregion


        #region информация о последних заказах
        /******************************************************************/
        /// <summary>
        /// список последних заказов
        /// </summary>
        public ObservableCollection<CustomerOrderInfo> CustomerOrderInfos { get; set; }



        /// <summary>
        /// выбранная строка в таблице последних заказов 
        /// </summary>
        public CustomerOrderInfo SelectedOrderInfoRow
        {
            get { return selectedOrderInfoRow; }
            set
            {
                selectedOrderInfoRow = value;
                OnPropertyChanged("SelectedOrderInfoRow");
            }
        }

        ///// <summary>
        ///// возвращает список с информацией о заказах клиентов
        ///// </summary>
        ///// <param name="pageSize">количество записей в списке</param>
        //private List<CustomerOrderInfo> CustomerOrderInfosToList(int pageSize)
        //{
        //    var result = (from order in db.CustomerOrders.OrderByDescending(i => i.Id).Take(pageSize)
        //                  join customer in db.Customers on order.CustomerId equals customer.Id
        //                  join detail in db.OrderDetails on order.Id equals detail.CustomerOrderId
        //                  select new CustomerOrderInfo
        //                  {
        //                      OrderDetail = detail,
        //                      CustomerName = customer.CustomerName,
        //                      CustomerId = order.CustomerId,
        //                      OrderDate = order.OrderDate,
        //                      RelatedOrderId = order.RelatedOrderId,
        //                      Status = order.Status
        //                  }).ToList();
        //    return result;
        //}


        /// <summary>
        /// устанавливает информацию о заказах в таблицу последних заказов, по умолчанию в параметр передается orderInfosRowsCount
        /// </summary>
        /// <param name="rowsCount">количество строк</param>
        private void SetCustomerOrderInfo(int rowsCount)
        {
            if (CustomerOrderInfos != null)
                CustomerOrderInfos.Clear();
            else
                CustomerOrderInfos = new ObservableCollection<CustomerOrderInfo>();

            foreach (CustomerOrderInfo info in CustomerOrderInfoSetter.FastSearchQuery(OrderId, CustomerName, Manufacturer, ColorGroup, ColorCode, rowsCount))
                CustomerOrderInfos.Add(info);

        }

        /// <summary>
        /// команда прсмотра рецепта
        /// </summary>
        public RelayCommand PreviewRecipeCommand
        {
            get
            {
                return previewRecipeCommand ??
                    (previewRecipeCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            var lastIteration = db.OrderIterationRows.Where(o => o.CustomerOrderId == SelectedOrderInfoRow.OrderDetail.CustomerOrderId).ToList().Last();
                            var iterationRows = from it in db.OrderIterationRows
                                                where it.CustomerOrderId == SelectedOrderInfoRow.OrderDetail.CustomerOrderId
                                                && it.IterationId == lastIteration.IterationId
                                                select it;
                            PaintRecipe recipe = new PaintRecipe();
                            foreach (OrderIterationRow itRow in iterationRows)
                            {
                                UsedInRecipePaintComponent component = new UsedInRecipePaintComponent(itRow.ComponentName);
                                component.AbsoluteAmount = itRow.AbsoluteAmount;
                                if (itRow.ComponentName == "Thinner")
                                    recipe.Thinner = component;
                                else recipe.RecipeComponents.Add(component);
                            }
                            UseProportionWindow useProportion = new UseProportionWindow(recipe, true);
                            if (useProportion.ShowDialog() == true)
                            {
                                CreateOrderViewModel viewModel = new CreateOrderViewModel();
                                viewModel.SetOrderDetail(SelectedOrderInfoRow.OrderDetail);
                                CreateOrderWindow createOrder = new CreateOrderWindow(viewModel);
                                if (createOrder.ShowDialog() == true)
                                {
                                    MixingComponentsTab tab = CreateOrder(viewModel);
                                    MixingComponentsTabs.Add(tab);
                                    SelectedMixingComponentsTab = tab;

                                    for (int i = 0; i < useProportion.newRecipe.RecipeComponents.Count; i++)
                                    {
                                        UsedInCalculationPaintComponent component = new UsedInCalculationPaintComponent();
                                        component.ComponentName = useProportion.newRecipe.RecipeComponents[i].ComponentName;
                                        SelectedMixingComponentsTab.Components.Add(component);
                                        SelectedMixingComponentsTab.Components[i].AddAmount = useProportion.newRecipe.RecipeComponents[i].AbsoluteAmount;
                                    }
                                    SelectedMixingComponentsTab.Thinner.AddAmount = 0;//инициализируем нулем, чтобы можно было сделать шаг назад
                                    SelectedMixingComponentsTab.Thinner.AddAmount = useProportion.newRecipe.Thinner.AbsoluteAmount;//устанавливаем значение


                                    ControlSumms.Add(selectedOrderId, GetControlSumm());
                                    SetCustomerOrderInfo(orderInfosRowsCount);
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }));
            }
        }
        #endregion


        #region правая панель
        /******************************************************************/
        /// <summary>
        /// команда установки веса тары и смеси до теста
        /// </summary>
        public RelayCommand OnScaleCommand
        {
            get
            {
                return onScaleCommand ??
                    (onScaleCommand = new RelayCommand(obj =>
                    {
                        if (SelectedMixingComponentsTab != null)
                        {
                            SelectedMixingComponentsTab.BeforeTest = SelectedMixingComponentsTab.TotalPaintComponentWeight + SelectedMixingComponentsTab.PreviousIterationBrutto;
                        }
                    }, (obj) => MixingComponentsTabs.Count() > 0));
            }
        }


        /// <summary>
        /// команда использования пропорции
        /// </summary>
        public RelayCommand UseProportionCommand
        {
            get
            {
                return useProportionCommand ??
                    (useProportionCommand = new RelayCommand(obj =>
                    {
                        if (SelectedMixingComponentsTab != null)
                        {
                            UseProportionWindow useProportion = new UseProportionWindow(SelectedMixingComponentsTab.PaintRecipe,false);
                            if(useProportion.ShowDialog()==true)
                            {
                                for (int i = 0; i < SelectedMixingComponentsTab.Components.Count; i++)
                                    SelectedMixingComponentsTab.Components[i].AddAmount = useProportion.newRecipe.RecipeComponents[i].AbsoluteAmount;

                                SelectedMixingComponentsTab.Thinner.AddAmount = useProportion.newRecipe.Thinner.AbsoluteAmount;
                            }
                        }
                    }, (obj) => MixingComponentsTabs.Count() > 0));
            }
        }


        /// <summary>
        /// команда удаления заказа
        /// </summary>
        public RelayCommand RemoveOrderCommand
        {
            get
            {
                return removeOrderCommand ??
                    (removeOrderCommand = new RelayCommand(obj =>
                    {
                        if (SelectedMixingComponentsTab != null)
                        {
                            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить заказ?", "Внимание!", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                var itRows = db.OrderIterationRows.Where(i => i.CustomerOrderId==selectedOrderId);
                                if (itRows != null)
                                {
                                    db.OrderIterationRows.RemoveRange(itRows);

                                    RemoveEmptyOrder(selectedOrderId);
                                    SelectedMixingComponentsTab.AddWorkProcessActionNotify -= AddWorkProcessAction;
                                    MixingComponentsTabs.Remove(SelectedMixingComponentsTab);
                                }
                                if (MixingComponentsTabs.Count > 0)
                                    SelectedMixingComponentsTab = MixingComponentsTabs.Last();
                                else
                                    StepBackActivity = false;
                            SetCustomerOrderInfo(orderInfosRowsCount);
                            }       
                        }
                    }, (obj) => MixingComponentsTabs.Count() > 0));
            }
        }
        #endregion

        #region быстрый поиск
        /******************************************************************/

        public string OrderId
        {
            get { return orderId; }
            set
            {
                orderId = value;
                OnPropertyChanged("OrderId");
            }
        }
        public string CustomerName
        {
            get { return customerName; }
            set
            {
                customerName = value;
                OnPropertyChanged("CustomerName");
            }
        }
        public string Manufacturer
        {
            get { return manufacturer; }
            set
            {
                manufacturer = value;
                OnPropertyChanged("Manufacturer");
            }
        }
        public string ColorGroup
        {
            get { return colorGroup; }
            set
            {
                colorGroup = value;
                OnPropertyChanged("ColorGroup");
            }
        }
        public string ColorCode
        {
            get { return colorCode; }
            set
            {
                colorCode = value;
                OnPropertyChanged("ColorCode");
            }
        }
        public DateTime OrderDate
        {
            get { return orderDate; }
            set
            {
                orderDate = value;
                OnPropertyChanged("OrderDate");
            }
        }


        /// <summary>
        /// команда быстрого поиска
        /// </summary>
        public RelayCommand FastSearchCommand
        {
            get
            {
                return fastSearchCommand ??
                    (fastSearchCommand = new RelayCommand(obj =>
                    {
                            SetCustomerOrderInfo(orderInfosRowsCount);
                    }));
            }
        }
        #endregion

        private RelayCommand openRecipeCatalogCommand;
        /// <summary>
        /// команда быстрого поиска
        /// </summary>
        public RelayCommand OpenRecipeCatalogCommand
        {
            get
            {
                return openRecipeCatalogCommand ??
                    (openRecipeCatalogCommand = new RelayCommand(obj =>
                    {
                        RecipeCatalogWindow showRecipeCatalog = new RecipeCatalogWindow();
                        if (showRecipeCatalog.ShowDialog() == true)
                        {
                            CreateOrderViewModel viewModel = new CreateOrderViewModel();


                            CreateOrderWindow createOrder = new CreateOrderWindow(viewModel);
                            if (createOrder.ShowDialog() == true)
                            {
                                MixingComponentsTab tab = CreateOrder(viewModel);
                                MixingComponentsTabs.Add(tab);
                                SelectedMixingComponentsTab = tab;

                                for (int i = 0; i < showRecipeCatalog.recipe.RecipeComponents.Count; i++)
                                {
                                    UsedInCalculationPaintComponent component = new UsedInCalculationPaintComponent();
                                    component.ComponentName = showRecipeCatalog.newRecipe.RecipeComponents[i].ComponentName;
                                    SelectedMixingComponentsTab.Components.Add(component);
                                    SelectedMixingComponentsTab.Components[i].AddAmount = showRecipeCatalog.newRecipe.RecipeComponents[i].AbsoluteAmount;
                                }
                                SelectedMixingComponentsTab.Thinner.AddAmount = 0;//инициализируем нулем, чтобы можно было сделать шаг назад
                                SelectedMixingComponentsTab.Thinner.AddAmount = showRecipeCatalog.newRecipe.Thinner.AbsoluteAmount;//устанавливаем значение

                                ControlSumms.Add(selectedOrderId, GetControlSumm());
                                SetCustomerOrderInfo(orderInfosRowsCount);
                            }
                        }
                    }));
            }
        }

        private RelayCommand checkWorkProcessAction;
        /// <summary>
        /// команда проверки данных в WorkProcessAction, используется для восстановления данных в случае некорректного завершения программы
        /// </summary>
        public RelayCommand CheckWorkProcessAction
        {
            get
            {
                return checkWorkProcessAction ??
                    (checkWorkProcessAction = new RelayCommand(obj =>
                    {
                        //получаем id незавершенных заказов
                        var ordersId = (from pa in db.WorkProcessActions
                                                  select pa.CustomerOrderId).Distinct().ToList();
                        if (ordersId.Count>0)
                        {
                            MessageBoxResult result = MessageBox.Show("Восстановить данные?", "Последний сеанс завершенн некорректно", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                try
                                {
                                    foreach(int id in ordersId)
                                    {
                                        SelectedOrderInfoRow = CustomerOrderInfos.Where(o => o.OrderDetail.CustomerOrderId == id).FirstOrDefault();
                                        writeProcessAction = false;
                                        OpenOrderBySelectedOrderInfoRow();
                                        var sourseIndexes= (from pa in db.WorkProcessActions.Where(o=>o.CustomerOrderId==id)
                                                                           select pa.IndexSourceChanges).Distinct().ToList();
                                        
                                        for(int i = 0; i < sourseIndexes.Count-6; i++)
                                        {
                                            if (i == SelectedMixingComponentsTab.stepBackLimit || i > SelectedMixingComponentsTab.stepBackLimit)
                                            {
                                                UsedInCalculationPaintComponent component = new UsedInCalculationPaintComponent();
                                                SelectedMixingComponentsTab.Components.Add(component);
                                            }
                                            var processActions = (from pa in db.WorkProcessActions.Where(o => o.CustomerOrderId == id)
                                                                  orderby pa.Id descending
                                                                  select pa).ToList();
                                            foreach(WorkProcessAction w in processActions)
                                            {
                                                if (w.IndexSourceChanges == sourseIndexes[i])
                                                {
                                                    SetWorkProcessActionAmount(w);
                                                    break;
                                                }
                                            }     
                                        }
                                        writeProcessAction = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                        }
                    }));
            }
        }
        private RelayCommand openEditCustomerWindow;
        /// <summary>
        /// команда открытия окна редактирования данных клиента
        /// </summary>
        public RelayCommand OpenEditCustomerWindow
        {
            get
            {
                return openEditCustomerWindow ??
                    (openEditCustomerWindow = new RelayCommand(obj =>
                    {
                        EditCustomerWindow window = new EditCustomerWindow();
                        window.Show();
                    }));
            }
        }
    }
}

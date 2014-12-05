using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.Forms;
using Penates.Models.ViewModels.Transactions;
using Penates.Models.ViewModels.Transactions.ProductsReceptions;
using Penates.Services;
using Penates.Services.ABMs;
using Penates.Services.DC;
using Penates.Services.Transactions;
using Penates.Utils.Enums;
using Penates.Utils.JSON.TableObjects;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Penates.Utils;
using Penates.Models.ViewModels.DC;
using Penates.Services.Notifications;
using Penates.Interfaces.Repositories;
using Penates.Repositories.ABMs;

namespace Penates.Controllers.Transactions.ProductsReceptions
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
    public class ProductsReceptionsController : Controller
    {
        private readonly IDistributionCenterService _dcService = new DistributionCenterService();
        private readonly IReceptionService _receptionService = new ReceptionService();
        private readonly ITemporaryDepositService _tempDepositService = new TemporaryDepositService();
        private readonly IProductService _productService = new ProductService();
        private readonly IContainerService _containerService = new ContainerService();

        private static long NO_CONTAINER_ID = 0;
        private static string NO_CONTAINER_DESCRIPTION = "No Container";
        private static string NO_CONTAINER_CODE = "TEMP";


        public ProductsReceptionsController(IDistributionCenterService dcService, IReceptionService receptionService, ITemporaryDepositService tempDepositService)
        {
            _dcService = dcService;
            _receptionService = receptionService;
            _tempDepositService = tempDepositService;
        }


        public ProductsReceptionsController()
        {

        }

        public ActionResult Index()
        {
            ViewBag.Action = Url.Action("VerifyOrder", "ProductsReceptions");
            ProductsReceptionModel model = new ProductsReceptionModel();
            return View("~/Views/Transactions/ProductsReceptions/ProductsReceptions.cshtml", model);
        }


        public ActionResult VerifyOrder(ProductsReceptionModel model)
        {
            if (model.SupplierID.HasValue && !String.IsNullOrEmpty(model.OrderID))
            {                
                IOrderService ordersServic = new OrderService();                
                OrderViewModel order = ordersServic.getData(model.OrderID.ToString(), (long) model.SupplierID);
                if (order != null)
                {
                    VerifyOrderModel verifyOrderModel = new VerifyOrderModel();
                    verifyOrderModel.OrderID = model.OrderID;
                    verifyOrderModel.SupplierID = model.SupplierID;
                    verifyOrderModel.SupplierName = order.SupplierName;
                    verifyOrderModel.OrderDate = order.OrderDate;
                    return View("~/Views/Transactions/ProductsReceptions/VerifyOrder.cshtml", verifyOrderModel);
                }
                else
                {
                    model.Error = true;
                    model.Message = Resources.Errors.OrderNotFound; 
                    return View("~/Views/Transactions/ProductsReceptions/ProductsReceptions.cshtml", model);
                }
            }
            return View("~/Views/Transactions/ProductsReceptions/ProductsReceptions.cshtml", model);
        }


        public string CorrectValueInTable(string id, string value )
        {
            //update recieved amount of boxes in the table 
            IOrderItemsService orderItemService  = new OrderItemsService();
            IOrderService orderService = new OrderService();
            string[] parameters = id.Split('_'); 
            if ( parameters.Length == 3){
                string IDSupplierOrder = parameters[0];
                long IDSupplier = Int64.Parse(parameters[1]);
                long IDProduct =  Int64.Parse(parameters[2]);
                orderService.updateProductsReceivedQuantityInOrder(IDSupplierOrder, IDSupplier, IDProduct, Int32.Parse(value));                          
            }
            return value;
        }   


        [ValidateAntiForgeryToken]
        public ActionResult ProcessOrder(string OrderID, string SupplierID, string SupplierName, DateTime OrderDate)
        {
            if (!_receptionService.existReceptionForOrder(OrderID, Int64.Parse(SupplierID)))
            {
                FinishReceptionModel finishReceptionModel = new FinishReceptionModel();
                finishReceptionModel.OrderID = Int64.Parse(OrderID);
                finishReceptionModel.SupplierID = Int64.Parse(SupplierID);
                finishReceptionModel.SupplierName = SupplierName;
                finishReceptionModel.OrderDate = OrderDate;
                finishReceptionModel.Error = false;
                return View("~/Views/Transactions/ProductsReceptions/FinishReception.cshtml", finishReceptionModel);
            }
            else
            {
                VerifyOrderModel verifyOrderModel = new VerifyOrderModel();
                verifyOrderModel.OrderID = OrderID;
                verifyOrderModel.SupplierID = Int64.Parse(SupplierID);
                IOrderService ordersServic = new OrderService();
                OrderViewModel order = ordersServic.getData(OrderID, Int64.Parse(SupplierID));
                verifyOrderModel.SupplierName = order.SupplierName;
                verifyOrderModel.OrderDate = order.OrderDate;
                verifyOrderModel.Error = true;
                verifyOrderModel.Message = Resources.Errors.OrderAlreadyProcessed;
                return View("~/Views/Transactions/ProductsReceptions/VerifyOrder.cshtml", verifyOrderModel);
            }
        }

        public ActionResult VerifyDistributionCenterProductCategories(long DistributionCenterCode, string COT, string OrderID, string SupplierID, bool IsPurchase, bool LeaveInTemporaryDeposit)
        {                                    
            IOrderService orderService = new OrderService();
            IQueryable<SupplierOrderItem> orderItems = orderService.getOrderItemsList(OrderID, Int64.Parse(SupplierID));           
            IDistributionCenterService dcService = new DistributionCenterService();
            if (dcService.isInternal(DistributionCenterCode))
            {
                if (!LeaveInTemporaryDeposit) {
                    ICollection<Rack> distributionCenterRacks = dcService.getAllRacks(DistributionCenterCode);
                    ICollection<string> missingCategories = verifyProductTypesCanBeStoredInDistributionCenter(orderItems, distributionCenterRacks.ToArray());
                    if (missingCategories.Count == 0) {
                        SelectContainerModel model = new SelectContainerModel();
                        model.ProductBoxes = orderItems.ToList();
                        model.OrderID = OrderID;
                        model.SupplierID = Int64.Parse(SupplierID);
                        model.DistributionCenterID = DistributionCenterCode;
                        model.COT = COT;
                        model.IsPurchase = IsPurchase;
                        model.LeaveInTemporaryDeposit = LeaveInTemporaryDeposit;
                        return View("~/Views/Transactions/ProductsReceptions/SelectContainer.cshtml", model);
                    } else {
                        FinishReceptionModel model = new FinishReceptionModel();
                        model.Error = true;
                        model.Message = Resources.Errors.MisssingCategoriesError + ": ";
                        foreach (String category in missingCategories) {
                            model.Message += category + ", ";
                        }
                        return View("~/Views/Transactions/ProductsReceptions/FinishReception.cshtml", model);
                    }
                }else {
                    SelectContainerModel model = new SelectContainerModel();
                    model.ProductBoxes = orderItems.ToList();
                    model.OrderID = OrderID;
                    model.SupplierID = Int64.Parse(SupplierID);
                    model.DistributionCenterID = DistributionCenterCode;
                    model.COT = COT;
                    model.IsPurchase = IsPurchase;
                    model.LeaveInTemporaryDeposit = LeaveInTemporaryDeposit;
                    return View("~/Views/Transactions/ProductsReceptions/SelectTempContainer.cshtml", model);
                }
            }
            else
            {                            
                SelectContainerModel model = new SelectContainerModel();
                model.ProductBoxes = orderItems.ToList();
                model.OrderID = OrderID;
                model.SupplierID = Int64.Parse(SupplierID);
                model.DistributionCenterID = DistributionCenterCode;
                model.COT = COT;
                model.IsPurchase = IsPurchase;
                model.LeaveInTemporaryDeposit = LeaveInTemporaryDeposit;
                return View("~/Views/Transactions/ProductsReceptions/SelectPack.cshtml", model);
            }
        }


        private ICollection<string> verifyProductTypesCanBeStoredInDistributionCenter(IQueryable<SupplierOrderItem> ProductItems, Rack[] DistributionCenterRacks)
        {
            ICollection<string> result = new List<string>();
            ICollection<ProductCategory> allDistributionCenterCategories = this.getAllDistributionCenterCategories(DistributionCenterRacks);
            foreach (SupplierOrderItem productItem in ProductItems)
            {
                if (allDistributionCenterCategories.Where(x => x.ProductCategoriesID == productItem.ProvidedBy.Product.IDProductCategory).Count() == 0)
                {
                    result.Add(productItem.ProvidedBy.Product.ProductCategory.Description);
                }     
            }   
            return result; 
        }

        private ICollection<ProductCategory> getAllDistributionCenterCategories(Rack[] DistributionCenterRacks)
        {
            ICollection<ProductCategory> allDistributionCenterCategories = new List<ProductCategory>();
            ICategoryService productCategoryService = new CategoryService();
            foreach (Rack rack in DistributionCenterRacks)
            {
                foreach (ProductCategory rackCategory in rack.ProductCategories)
                {
                    productCategoryService.getAllChildren(rackCategory.ProductCategoriesID, allDistributionCenterCategories);
                }
            }
            return allDistributionCenterCategories;
        }
     
        public ActionResult RequestContainers(List<long> Containers, List<SupplierOrderItem> ProductBoxes, List<long> PackID, long SupplierID, string OrderID, string COT, long DistributionCenterID, bool IsPurchase)
        {
            if (Containers.Contains(0))
            {
                SelectContainerModel model = new SelectContainerModel();
                model.Containers = Containers;
                model.ProductBoxes = ProductBoxes;
                IProductRepository productRepository = new ProductRepository(); 

                foreach (SupplierOrderItem item in ProductBoxes) {
                    Product product = productRepository.getData(item.ProvidedBy.IDProduct);
                    if (product != null)
                    {
                        item.ProvidedBy.Product = product; 
                    }
                }

                model.PackID = PackID;
                model.SupplierID = SupplierID;
                model.OrderID = OrderID;
                model.DistributionCenterID = DistributionCenterID;
                model.COT = COT;
                model.IsPurchase = IsPurchase;
                model.Error = true;                
                model.Message = Resources.Errors.AllProductsMustHaveAContainer;
                return View("~/Views/Transactions/ProductsReceptions/SelectContainer.cshtml", model);
            } 

            RequestedContainers requestedContainers = getNeededContainersList(Containers, ProductBoxes, SupplierID, OrderID);
            if (requestedContainers.Error == ""){              
                if (createReception(DistributionCenterID,OrderID, SupplierID,COT,IsPurchase)){
                    SortedDictionary<long, List<long>> boxesPerPorductID = createBoxes(ProductBoxes, PackID, DistributionCenterID, OrderID, SupplierID);
                    ContainersAndLocationAssignationModel model = assignBoxesToContainers(requestedContainers.containersPerProduct, boxesPerPorductID, SupplierID, OrderID, DistributionCenterID);
                    TempData["Assignation"] = model;
                    return RedirectToAction("Assignations", "ProductsReceptions");
                }else{
                    SelectContainerModel model = new SelectContainerModel();
                    model.Error = true;
                    model.Message = Resources.Errors.ReceptionDBError; 	                    
                    return View("~/Views/Transactions/ProductsReceptions/SelectContainer.cshtml", model);
                }                 
            }else{
                SelectContainerModel model = new SelectContainerModel() { 
                    Error = true,
                    Message = requestedContainers.Error,
                    Containers = Containers,
                    COT = COT,
                    DistributionCenterID = DistributionCenterID,
                    IsPurchase = IsPurchase,
                    OrderID = OrderID,
                    PackID = PackID,
                    SupplierID = SupplierID
                };
                IProductRepository productRepo = new ProductRepository();
                foreach (SupplierOrderItem item in ProductBoxes) {
                    Product p = productRepo.getData(item.ProvidedBy.IDProduct);
                    item.ProvidedBy.Product = p;
                }
                model.ProductBoxes = ProductBoxes;

                List<string> ContainerTypeName = new List<string>();
                IContainerTypeService containerTypeService = new ContainerTypeService();
                foreach(long cont in Containers){
                    ContainerTypeName.Add(containerTypeService.getContainerTypeName(cont));
                }
                model.ContainersNames = ContainerTypeName;

                List<string> PackNamesList = new List<string>();
                IPackService packService = new PackService();
                foreach (long packID in PackID) {
                    PackViewModel mod = packService.getData(packID);
                    if (mod == null) {
                        PackNamesList.Add("");
                    } else {
                        PackNamesList.Add(mod.SerialNumber);
                    }
                }
                model.PackSerialNumber = PackNamesList;
                return View("~/Views/Transactions/ProductsReceptions/SelectContainer.cshtml", model);
            }     
        }


        private ProductAssignation createTransitoryBoxesForProduct(SupplierOrderItem Product, long PackID, long DistributionCenterID, string OrderID, long SupplierID, TempDepositLocation Location)
        {
            List<SupplierOrderItem> productBoxes = new List<SupplierOrderItem>();
            productBoxes.Add(Product); 
            List<long> packsIDs = new List<long>(); 
            packsIDs.Add(PackID);
            SortedDictionary<long,List<long>> boxesPerProduct = createBoxes(productBoxes, packsIDs, DistributionCenterID, OrderID, SupplierID);

            Location tempLocation = new Location();
            tempLocation.TempDeposit = true;
            tempLocation.DepositDescription = Location.DepositDescription;
            tempLocation.DepositID = Location.DepositID;           

            ProvidedByViewModel boxData = _productService.getProvidedByData(Product.ProvidedBy.IDProduct, SupplierID);        
        
            ContainerViewModel NoContainer = new ContainerViewModel()
            {
                ContainerTypeID = NO_CONTAINER_ID,
                Code = NO_CONTAINER_CODE + DateTime.Now.ToString(),
                ContainerTypeName = NO_CONTAINER_DESCRIPTION,
                DepositID = Location.DepositID,
                DepositName = Location.DepositDescription,
                Size = (decimal) boxData.Size * boxesPerProduct[Product.ProvidedBy.IDProduct].Count,
                TemporaryDepositID = Location.DepositID,
                TemporaryDepositName = Location.DepositDescription,
                UsedPercentage = 100,               
            };

            long containerID = _containerService.Save(NoContainer);

            IBoxService boxService = new BoxService();
            foreach (long boxId in boxesPerProduct[Product.ProvidedBy.IDProduct])
            {
                boxService.setContainerID(boxId, containerID);
            }

            ProductAssignation transitoryBoxes = new ProductAssignation() { 
                BoxesID = boxesPerProduct[Product.ProvidedBy.IDProduct],
                BoxesQuantity = boxesPerProduct[Product.ProvidedBy.IDProduct].Count,                 
                ProductName = _productService.getProductData(Product.ProvidedBy.IDProduct).Name,
                ProductID = Product.ProvidedBy.IDProduct,
                ContainerLocation = tempLocation, 
                ContainerTypeDescription = NO_CONTAINER_DESCRIPTION,
                ContainerID = containerID,
                IsTransitory = true
            };

            return transitoryBoxes;  
        }


        public ActionResult requestContainersTempDeposit(List<SupplierOrderItem> ProductBoxes, List<bool> Transitory, List<long> Containers, List<long> PackID, long SupplierID, string OrderID, string COT, long DistributionCenterID, bool IsPurchase, bool LeaveInTemporaryDeposit)
        {
            RequestedContainers requestedContainers = getNeededContainersList(Containers, ProductBoxes, SupplierID, OrderID);
            if (requestedContainers.Error == "")
            {
                decimal spaceNeeded = calculateNeededSpaceForOderderInM3(ProductBoxes, OrderID, SupplierID);
                TempDepositLocation orderLocation = _dcService.findTemporaryDepositWithSpace(DistributionCenterID, spaceNeeded);
                if (orderLocation != null)
                {
                    if (createReception(DistributionCenterID, OrderID, SupplierID, COT, IsPurchase)) 
                    {                   
                        ContainersAndLocationAssignationModel model = new ContainersAndLocationAssignationModel();
                        List<long> transitoryProductsIDs = new List<long>();
                        for (int i = 0; i < ProductBoxes.Count; i++)
                        {
                            if (Transitory[i])
                            {
                                ProductAssignation transitoryBoxes = createTransitoryBoxesForProduct(ProductBoxes[i], PackID[i], DistributionCenterID, OrderID, SupplierID, orderLocation);
                                model.productsAssignations.Add(transitoryBoxes);
                                transitoryProductsIDs.Add(ProductBoxes[i].ProvidedBy.IDProduct);
                            }
                        }
                        
                        if (transitoryProductsIDs.Count > 0)
                        {
                            foreach (long id in transitoryProductsIDs)
                            {
                                int i = 0;
                                bool continueSearching = true;
                                while(i<ProductBoxes.Count && continueSearching)
                                {
                                    if (ProductBoxes[i].ProvidedBy.IDProduct.Equals(id))
                                    {
                                        ProductBoxes.RemoveAt(i);
                                        Containers.RemoveAt(i);
                                        PackID.RemoveAt(i);
                                        continueSearching = false; 
                                    }
                                    i++;
                                }
                            }
                        }

                        SortedDictionary<long, List<long>> boxesPerPorductID = createBoxes(ProductBoxes, PackID, DistributionCenterID, OrderID, SupplierID);       
                        List<ProductAssignation> productAssignations = assignBoxesToContainersInTempDeposit(requestedContainers.containersPerProduct, boxesPerPorductID, SupplierID, OrderID, DistributionCenterID, orderLocation);
                        model.productsAssignations.AddRange(productAssignations); 

                        TempData["Assignation"] = model;
                        return RedirectToAction("Assignations", "ProductsReceptions");                        
                    }else{
                        SelectContainerModel model = new SelectContainerModel();
                        model.Error = true;
                        model.Message = Resources.Errors.ReceptionDBError; 	                    
                        return View("~/Views/Transactions/ProductsReceptions/SelectTempContainer.cshtml", model);
                    }                                         
                }else{
                    SelectContainerModel model = new SelectContainerModel();
                    model.Error = true;
                    model.Message = Resources.Errors.NoLocationError;
                    return View("~/Views/Transactions/ProductsReceptions/SelectTempContainer.cshtml", model);
                }                    
            }else{
                SelectContainerModel model = new SelectContainerModel() { 
                Error = true,
                Message = requestedContainers.Error,
                Containers = Containers,
                COT = COT,
                DistributionCenterID = DistributionCenterID,
                IsPurchase = IsPurchase,
                OrderID = OrderID,
                PackID = PackID,
                SupplierID = SupplierID
                };
                IProductRepository productRepo = new ProductRepository();
                foreach (SupplierOrderItem item in ProductBoxes) {
                    Product p = productRepo.getData(item.ProvidedBy.IDProduct);
                    item.ProvidedBy.Product = p;
                }
                model.ProductBoxes = ProductBoxes;

                List<string> ContainerTypeName = new List<string>();
                IContainerTypeService containerTypeService = new ContainerTypeService();
                foreach(long cont in Containers){
                    ContainerTypeName.Add(containerTypeService.getContainerTypeName(cont));
                }
                model.ContainersNames = ContainerTypeName;

                List<string> PackNamesList = new List<string>();
                IPackService packService = new PackService();
                foreach (long packID in PackID) {
                    PackViewModel mod = packService.getData(packID);
                    if (mod == null) {
                        PackNamesList.Add("");
                    } else {
                        PackNamesList.Add(mod.SerialNumber);
                    }
                }
                model.PackSerialNumber = PackNamesList;
                return View("~/Views/Transactions/ProductsReceptions/SelectTempContainer.cshtml", model);
            }     
        }        


        public ActionResult Assignations()
        {
            ContainersAndLocationAssignationModel model = (ContainersAndLocationAssignationModel)TempData.Peek("Assignation");
            return View("~/Views/Transactions/ProductsReceptions/ContainersAndLocationAssignations.cshtml", model);
        }

        public ActionResult sendBoxes(List<SupplierOrderItem> ProductBoxes, List<long> PackID, long SupplierID,
            string OrderID, string COT, long DistributionCenterID, bool IsPurchase, bool LeaveInTemporaryDeposit)
        {            
            decimal neededSpace = calculateNeededSpaceForOderderInM3(ProductBoxes, OrderID, SupplierID);           
            ExternalDistributionCenterViewModel externalDC = _dcService.getExternalData(DistributionCenterID);
            if (externalDC.HasMaxCapacity) {
                if (externalDC.UsableSpace >= neededSpace) {
                    createReception(DistributionCenterID, OrderID, SupplierID, COT, IsPurchase);
                    ExternalLocationAssignationModel model = new ExternalLocationAssignationModel();
                    SortedDictionary<string, List<long>> boxesIdsPerProductName = createBoxesForExternalOrTemp(ProductBoxes, PackID, DistributionCenterID, OrderID, SupplierID);
                    model.DistributionCenterDestiny = externalDC.Address;
                    model.boxesIdsPerProductName = boxesIdsPerProductName;
                    _dcService.setExternalDCUsableUsedSpace(DistributionCenterID, _dcService.getExternalDCUsableUsedSpace(DistributionCenterID) + neededSpace);
                    return View("~/Views/Transactions/ProductsReceptions/ExternalLocationAssignation.cshtml", model);
                } else {
                    FinishReceptionModel model = new FinishReceptionModel();
                    model.Error = true;
                    model.Message = Resources.Errors.NotEnoughSpace;
                    return View("~/Views/Transactions/ProductsReceptions/FinishReception.cshtml", model);
                }
            } else {
                createReception(DistributionCenterID, OrderID, SupplierID, COT, IsPurchase);
                ExternalLocationAssignationModel model = new ExternalLocationAssignationModel();
                SortedDictionary<string, List<long>> boxesIdsPerProductName = createBoxesForExternalOrTemp(ProductBoxes, PackID, DistributionCenterID, OrderID, SupplierID);
                model.DistributionCenterDestiny = externalDC.Address;
                model.boxesIdsPerProductName = boxesIdsPerProductName;
                _dcService.setExternalDCUsableUsedSpace(DistributionCenterID, _dcService.getExternalDCUsableUsedSpace(DistributionCenterID) + neededSpace);
                return View("~/Views/Transactions/ProductsReceptions/ExternalLocationAssignation.cshtml", model);
            }          
        }
      
        public ActionResult tomparalDepositAssignation()
        {
            TempLocationAssignationModel model = (TempLocationAssignationModel)TempData.Peek("Assignation");
            return View("~/Views/Transactions/ProductsReceptions/TempDepositAssignation.cshtml", model);
        }


        private SortedDictionary<string, List<long>> createBoxesForExternalOrTemp(List<SupplierOrderItem> ProductBoxes, List<long> PackID, long DistributionCenterID, string OrderID, long SupplierID )
        {
            SortedDictionary<long, List<long>> boxes = createBoxes(ProductBoxes, PackID, DistributionCenterID, OrderID, SupplierID);
            IProductService productService = new ProductService();
            SortedDictionary<string, List<long>> boxesIdsPerProductName = new SortedDictionary<string, List<long>>();
            foreach (KeyValuePair<long, List<long>> productBoxes in boxes)
            {
                ProductViewModel product = productService.getProductData(productBoxes.Key);
                boxesIdsPerProductName.Add(product.Name, productBoxes.Value);
            }
            return boxesIdsPerProductName;
        }

        private decimal calculateNeededSpaceForOderderInM3(List<SupplierOrderItem> ProductList, string OrderID, long SupplierID)
        {
            IOrderService orderService = new OrderService(); 
            IProductService productService = new ProductService();            
            decimal result = 0;
            if (ProductList != null)
            {
                foreach (SupplierOrderItem orderItem in ProductList)
                {
                    decimal recievedBoxes = orderService.getReceivedQuantity(OrderID, SupplierID, orderItem.ProvidedBy.IDProduct.ToString());
                    ProvidedByViewModel boxDetails = productService.getProvidedByData(orderItem.ProvidedBy.IDProduct, SupplierID);
                    result += result + (recievedBoxes * (decimal)boxDetails.Size);
                }
            }
            return SizeUtils.fromCm3ToM3(result);
        }


        private ContainersAndLocationAssignationModel assignBoxesToContainers(SortedDictionary<long, string> containersNeededPerProductMap, SortedDictionary<long, List<long>> boxesPerPorductID, long SupplierID, string OrderID, long DistributionCenterID)
        {
            ContainersAndLocationAssignationModel model = new ContainersAndLocationAssignationModel();
            IDistributionCenterService distributionCenterService = new DistributionCenterService();
            IProductService productService = new ProductService();
            IContainerTypeService containerTypeService = new ContainerTypeService();
            IContainerService containerService = new ContainerService();
            
            foreach (KeyValuePair<long, string> containersNeededPerProduct in containersNeededPerProductMap)
            {
                //requestedContainers.containersPerProduct <long:ProductID, string: ContaynerTypeID:cant>  
                string[] value = containersNeededPerProduct.Value.Split(':');                
                decimal containerBoxesCapacity = getContainerBoxesCapacity(Int64.Parse(value[0]), containersNeededPerProduct.Key, SupplierID);
                if (value.Count() == 2)
                {
                    ProductViewModel product = productService.getProductData(containersNeededPerProduct.Key);
                    for (int i = 0; i < Int32.Parse(value[1]); i++)
                    {
                        ProductAssignation productAssignation = new ProductAssignation();

                        decimal containerWithBoxesHeightNeeded = getHeightNeeded(Int64.Parse(value[0]), containersNeededPerProduct.Key, SupplierID);
                        decimal volumeFullContainer =  getVolumeForContainerWithBoxes(Int64.Parse(value[0]), containersNeededPerProduct.Key,SupplierID,containerBoxesCapacity);
                        Location location = distributionCenterService.findLocationForContainer(DistributionCenterID, product.Category, containerTypeService.getContainerArea(Int64.Parse(value[0])), volumeFullContainer, containerWithBoxesHeightNeeded);
                        if (location != null)
                        {
                            productAssignation.ProductID = product.ProductID;
                            productAssignation.ProductName = product.Name;
                            productAssignation.ContainerLocation = location;

                            Container container = containerService.getEmptyContainer(Int64.Parse(value[0]));
                            productAssignation.ContainerID = container.ContainerID;
                            productAssignation.ContainerTypeDescription = container.ContainerType.Description;

                            bool thereIsMoreSpaceInTheContainer = true;
                            List<long> boxesListIds;
                            List<long> containerBoxes = new List<long>();
                            boxesPerPorductID.TryGetValue(product.ProductID, out boxesListIds);
                            while (thereIsMoreSpaceInTheContainer && boxesListIds.Count > 0)
                            {
                                containerBoxes.Add(boxesListIds.FirstOrDefault());
                                boxesListIds.Remove(boxesListIds.FirstOrDefault());                                
                                if (containerBoxes.Count == containerBoxesCapacity)
                                {
                                    thereIsMoreSpaceInTheContainer = false;
                                }
                            }
                            productAssignation.BoxesID = containerBoxes;
                            productAssignation.BoxesQuantity = containerBoxes.Count;

                            ProvidedByViewModel boxData = productService.getProvidedByData(product.ProductID, SupplierID);
                            decimal usedSpace = boxData.Depth * boxData.Width * boxData.Height * containerBoxes.Count;
                            updateUsedSpace(location, container.ContainerID, usedSpace);
                            boxesPerPorductID[product.ProductID] = boxesListIds;
                        }
                        else
                        {
                            deleteBoxes(boxesPerPorductID);
                            model.Error = true;
                            model.Message = Resources.Errors.LocationMissing;
                            return model; 
                        }
                        updateContainerId(productAssignation.BoxesID, productAssignation.ContainerID); 
                        model.productsAssignations.Add(productAssignation);
                    }
                    List<long> remaningBoxes;
                    if (boxesPerPorductID.TryGetValue(product.ProductID, out remaningBoxes))
                    {
                        if (remaningBoxes.Count != 0)
                        {
                            //This should never happend                             
                            model.Error = true;
                            model.Message = Resources.Errors.NotEnoughContainersForAllBoxes;
                            return model;
                        }
                    }
                }
                else
                {
                    deleteBoxes(boxesPerPorductID);
                    model.Error = true;
                    model.Message = Resources.Errors.RequestedContainersError;
                    return model; 
                }
            }
            return model;     
        }

        private List<ProductAssignation> assignBoxesToContainersInTempDeposit(SortedDictionary<long, string> containersNeededPerProductMap, SortedDictionary<long, List<long>> boxesPerPorductID, long SupplierID, string OrderID, long DistributionCenterID, TempDepositLocation tempLocation)
        {
            List<ProductAssignation> assignations = new List<ProductAssignation>();
            IDistributionCenterService distributionCenterService = new DistributionCenterService();
            IProductService productService = new ProductService();
            IContainerTypeService containerTypeService = new ContainerTypeService();
            IContainerService containerService = new ContainerService();

            foreach (KeyValuePair<long, string> containersNeededPerProduct in containersNeededPerProductMap)
            {
                //requestedContainers.containersPerProduct <long:ProductID, string: ContaynerTypeID:cant>  
                string[] value = containersNeededPerProduct.Value.Split(':');
                decimal containerBoxesCapacity = getContainerBoxesCapacity(Int64.Parse(value[0]), containersNeededPerProduct.Key, SupplierID);
                if (value.Count() == 2)
                {
                    ProductViewModel product = productService.getProductData(containersNeededPerProduct.Key);
                    for (int i = 0; i < Int32.Parse(value[1]); i++)
                    {
                        ProductAssignation productAssignation = new ProductAssignation();

                        decimal containerWithBoxesHeightNeeded = getHeightNeeded(Int64.Parse(value[0]), containersNeededPerProduct.Key, SupplierID);
                        decimal volumeFullContainer = getVolumeForContainerWithBoxes(Int64.Parse(value[0]), containersNeededPerProduct.Key, SupplierID, containerBoxesCapacity);
                      
                        productAssignation.ProductID = product.ProductID;
                        productAssignation.ProductName = product.Name;

                        Location location = new Location()
                        {
                            TempDeposit = true,
                            DepositDescription = tempLocation.DepositDescription,
                            DepositID = tempLocation.DepositID
                        };
                        productAssignation.ContainerLocation = location;

                        Container container = containerService.getEmptyContainer(Int64.Parse(value[0]));
                        productAssignation.ContainerID = container.ContainerID;
                        productAssignation.ContainerTypeDescription = container.ContainerType.Description;

                        bool thereIsMoreSpaceInTheContainer = true;
                        List<long> boxesListIds;
                        List<long> containerBoxes = new List<long>();
                        boxesPerPorductID.TryGetValue(product.ProductID, out boxesListIds);
                        while (thereIsMoreSpaceInTheContainer && boxesListIds.Count > 0)
                        {
                            containerBoxes.Add(boxesListIds.FirstOrDefault());
                            boxesListIds.Remove(boxesListIds.FirstOrDefault());
                            if (containerBoxes.Count == containerBoxesCapacity)
                            {
                                thereIsMoreSpaceInTheContainer = false;
                            }
                        }
                        productAssignation.BoxesID = containerBoxes;
                        productAssignation.BoxesQuantity = containerBoxes.Count;

                        ProvidedByViewModel boxData = productService.getProvidedByData(product.ProductID, SupplierID);
                        decimal usedSpace = boxData.Depth * boxData.Width * boxData.Height * containerBoxes.Count;
                        updateTempDepositUsedSpace(DistributionCenterID, location, container.ContainerID, usedSpace);
                        boxesPerPorductID[product.ProductID] = boxesListIds;
                      
                        updateContainerId(productAssignation.BoxesID, productAssignation.ContainerID);
                        assignations.Add(productAssignation);
                    }

                    List<long> remaningBoxes;
                    if (boxesPerPorductID.TryGetValue(product.ProductID, out remaningBoxes))
                    {
                        if (remaningBoxes.Count != 0)
                        {
                            //This should never happend                                                     
                            return null;
                        }
                    }
                }
                else
                {                   
                    return null;
                }
            }
            return assignations;
        }

        private void updateTempDepositUsedSpace(long DistributionCenterID, Location Location, long ContainerID, decimal UsedSpaceInCm)
        {
            IContainerService containerService = new ContainerService();
            containerService.setContainerUsedSpace(ContainerID, UsedSpaceInCm);
         
            _dcService.updateTempDepositUsedSpace(DistributionCenterID, Location.DepositID, UsedSpaceInCm);
        }

        private void updateUsedSpace(Location Location, long ContainerID, decimal UsedSpaceInCm)
        {
            IContainerService containerService = new ContainerService();
            containerService.setContainerUsedSpace(ContainerID, UsedSpaceInCm);
            IRackService rackService = new RackService();
            rackService.updateUsedSpace(Location.ShelfSubdivisionID, UsedSpaceInCm);           
        }

        /// <summary>
        /// Returns the Hight needed in Meters so that it can be used to find space in a Rack. 
        /// It takes the higher value betweeen the container and the box 
        /// </summary>
        /// <param name="ContainerTypeID">Container Type</param>
        /// <param name="ProductID">Product ID</param>
        /// <param name="SupplierID">Supplier ID</param>
        /// <returns>Needed Height</returns>
        private decimal getHeightNeeded(long ContainerTypeID, long ProductID, long SupplierID)
        {
            IContainerTypeService containerTypeService = new ContainerTypeService();
            IProductService productService = new ProductService();
            ProvidedByViewModel box = productService.getProvidedByData(ProductID, SupplierID);
            decimal containerHeight = containerTypeService.getContainerHeight(ContainerTypeID);
            if (box.Height > containerHeight)
            {
                return box.Height / 100;
            }
            return containerHeight / 100;
        }


        private decimal getVolumeForContainerWithBoxes(long ContainerTypeID, long ProductID, long SupplierID, decimal ContainerBoxesCapacity)
        {
            IContainerTypeService containerTypeService = new ContainerTypeService();
            IProductService productService = new ProductService();
            ProvidedByViewModel box = productService.getProvidedByData(ProductID, SupplierID);
            decimal containerWithBoxesVolumeNeeded = ContainerBoxesCapacity * ((decimal) box.Size) + (containerTypeService.getContainerSize(ContainerTypeID));
            return SizeUtils.fromCm3ToM3(containerWithBoxesVolumeNeeded);
        }


        private void updateContainerId(List<long> Boxes, long ContainerID)
        {
            BoxService boxService = new BoxService();
            foreach (long box in Boxes)
            {
                boxService.setContainerID(box, ContainerID);
            }
        }

        private bool deleteBoxes(SortedDictionary<long, List<long>> boxesPerPorductID)
        {
            BoxService boxService = new BoxService();
            bool result = true;
            foreach (KeyValuePair<long, List<long>> boxesIdsList in boxesPerPorductID)
            {
                foreach (long boxID in boxesIdsList.Value)
                {
                    if (!boxService.Delete(boxID))
                    {
                        return result = false; 
                    }
                }
            }
            return result;
        }
        
      
        private SortedDictionary<long, List<long>> createBoxes(List<SupplierOrderItem> ProductBoxes, List<long> PackID, long DistributionCenterID, string OrderID, long SupplierID)
        {
            SortedDictionary<long, List<long>> boxesIdsPerProductId = new SortedDictionary<long, List<long>>();
            if (ProductBoxes != null)
            {
                IOrderService orderService = new OrderService();
                IProductService productService = new ProductService();

                IStatusService statusService = new StatusService();
                IPackService packService = new PackService();
                BoxService boxServicce = new BoxService();

                List<long> boxesIds = new List<long>();
                int index = 0;
                while (index < ProductBoxes.Count)
                {
                    ProductViewModel productDetails = productService.getProductData(ProductBoxes[index].ProvidedBy.IDProduct);
                    ProvidedByViewModel providedBy = productService.getProvidedByData(productDetails.ProductID, SupplierID);

                    decimal amountOfBoxes = orderService.getReceivedQuantity(OrderID.ToString(), SupplierID, ProductBoxes[index].ProvidedBy.IDProduct.ToString());
                    for (int i = 1; i <= amountOfBoxes; i++)
                    {
                        BoxViewModel box = new BoxViewModel();
                        box.AdquisitionDate = DateTime.Now;
                        box.DistributionCenterID = DistributionCenterID;
                        box.BuyerCost = providedBy.PurchasePrice;
                        box.Quantity = providedBy.ItemsPerBox;
                        box.ProductName = productDetails.Description;
                        box.ProductID = productDetails.ProductID;
                        box.Size = (decimal) providedBy.Size;
                        box.Height = providedBy.Height;
                        box.Width = providedBy.Width;
                        box.Depth = providedBy.Depth;
                        box.IsWaste = false;
                        box.Reserved = false;
                        box.Reevaluate = false;

                        PackViewModel pack = packService.getData(PackID[index]);
                        if (pack != null)
                        {
                            box.PackSerialCode = pack.SerialNumber;
                            box.PackID = pack.PackID;
                        }

                        ItemsState itemState = statusService.getDefaultState();
                        box.StatusDescription = itemState.Description;
                        box.StatusID = itemState.ItemStateID;

                        box.ContainerID = 0;

                        long boxID = boxServicce.Add(box);
                        boxesIds.Add(boxID);
                    }
                    boxesIdsPerProductId.Add(ProductBoxes[index].ProvidedBy.IDProduct, boxesIds);
                    boxesIds = new List<long>();
                    index++;
                }
            }
            return boxesIdsPerProductId;
        }

        private decimal getContainerBoxesCapacity(long ContainerID, long ProductID, long SupplierID)
        {
            IContainerTypeService containerService = new ContainerTypeService();
            IProductService productService = new ProductService();
            ProvidedByViewModel boxData = productService.getProvidedByData(ProductID, SupplierID);
            decimal boxArea = boxData.Depth * boxData.Width;
            decimal containerArea = containerService.getContainerArea(ContainerID);
          
            return Math.Floor(containerArea / boxArea);
        }

        private bool createReception(long DistributionCenterID, string IDSupplierOrder, long SupplierID, string COT, bool IsPurchased)
        {
            ReceptionModel reception = new ReceptionModel();
            reception.date = DateTime.Now;
            reception.COT = COT;
            reception.IDDistribucionCenter = DistributionCenterID;
            reception.IDSupplier = SupplierID;
            reception.IDSupplierOrder = IDSupplierOrder;
            reception.IsPurchase = IsPurchased;
            decimal receptionID = _receptionService.save(reception);
            if (receptionID > 0)
            {           
                Notification notification = new Notification();
                notification.Date = reception.date;
                notification.IDDistributionCenter = DistributionCenterID;               
                IOrderService orderService = new OrderService();
                OrderViewModel order = orderService.getData(IDSupplierOrder.ToString(),SupplierID,true);                
                notification.Message = String.Format(Resources.Messages.ReceptionNotification, order.OldOrderID, order.SupplierName);
                notification.IDNotificationType = 4;                 
                NotificationsService notificationService = new NotificationsService();
                notificationService.save(notification);
                return true;
            }
            return false; 
        }

        private RequestedContainers getNeededContainersList(List<long> Containers, List<SupplierOrderItem> ProductBoxes, long SupplierID, string OrderID)
        {
            IContainerTypeService containerService = new ContainerTypeService();
            IProductService productService = new ProductService();
            IOrderService orderService = new OrderService();

            RequestedContainers requestedContainers = new RequestedContainers();
            String Error = "";

            for (int i = 0; i < ProductBoxes.Count; i++)
            {
                if (!Containers[i].Equals(0)) //Requested container type != NO Container, if container type it's 0 needs to be created later 
                {
                    ProvidedByViewModel boxData = productService.getProvidedByData(ProductBoxes[i].ProvidedBy.IDProduct, SupplierID);
                    decimal boxArea = boxData.Depth * boxData.Width;
                    decimal containerArea = containerService.getContainerArea(Containers[i]);

                    if (boxArea <= containerArea)
                    {
                        decimal boxesPerContainer = Math.Floor(containerArea / boxArea);
                        decimal containersQuantity = Math.Ceiling(orderService.getReceivedQuantity(OrderID, SupplierID, ProductBoxes[i].ProvidedBy.IDProduct.ToString()) / boxesPerContainer);
                        decimal aux;
                        if (requestedContainers.requestedContainersMap.TryGetValue(Containers[i], out aux))
                        {
                            requestedContainers.requestedContainersMap[Containers[i]] = aux + containersQuantity;
                        }
                        else
                        {
                            requestedContainers.requestedContainersMap.Add(Containers[i], containersQuantity);
                        }
                        requestedContainers.containersPerProduct.Add(ProductBoxes[i].ProvidedBy.IDProduct, Containers[i].ToString() + ":" + containersQuantity.ToString());
                    }
                    else
                    {
                        ProductViewModel product = productService.getProductData(ProductBoxes[i].ProvidedBy.IDProduct);
                        Error += Resources.Errors.ContainerTooSmall + "[" + containerService.getContainerTypeName(Containers[i]) + "] - " + Resources.Errors.Box + " [" + product.Description + "] ";
                    }
                }               
            }
            foreach (KeyValuePair<long, decimal> containerTypeEntry in requestedContainers.requestedContainersMap)
            {
                if (!containerService.areThereEnoughEmptyContainers(containerTypeEntry.Key, containerTypeEntry.Value))
                {
                    Error += Resources.Errors.NotEnoughContainers + ": \"" + containerService.getContainerTypeName(containerTypeEntry.Key) + "\"  "+ Resources.Errors.Requested + ":" + containerTypeEntry.Value.ToString() + " ";
                }
            }
            requestedContainers.Error = Error;          
            return requestedContainers;
        }


        
        public JsonResult SupplierAutocomplete(string term)
        {
            try
            {
                ISupplierService service = new SupplierService(); 
                var items = service.getAutocomplete(term, null);
                var json = service.toJsonAutocomplete(items);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (DatabaseException ex)
            {
                Response.StatusCode = 504;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            }
            catch (Exception e)
            {
                Response.StatusCode = 504;
                Response.StatusDescription = Resources.Errors.DatabaseError;
                Response.Write(e.Message);
                return null;
            }
        }

        public JsonResult ContainerTypeAutocomplete(string term)
        {
            try
            {
                IContainerTypeService service = new ContainerTypeService();
                var items = service.getAutocomplete(term);
                var json = service.toJsonAutocomplete(items);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (DatabaseException ex)
            {
                Response.StatusCode = 504;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            }
            catch (Exception e)
            {
                Response.StatusCode = 504;
                Response.StatusDescription = Resources.Errors.DatabaseError;
                Response.Write(e.Message);
                return null;
            }
        }


        public JsonResult OrderAutocomplete(string term, long? SupplierID)
        {
            try
            {
                ISupplierService service = new SupplierService();
                var items = service.getOrdersAutocomplete(term, SupplierID);
                var json = service.toJsonAutocomplete(items);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (DatabaseException ex)
            {
                Response.StatusCode = 504;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            }
            catch (Exception e)
            {
                Response.StatusCode = 504;
                Response.StatusDescription = Resources.Errors.DatabaseError;
                Response.Write(e.Message);
                return null;
            }
        }

        public ActionResult ProductLoad(jQueryDataTableParamModel param, string OrderID, long? SupplierID)
        {
            try
            {
                IOrderService os = new OrderService();
                ICollection<SupplierOrderItem> query = os.getItems(OrderID, SupplierID);
                IOrderItemsService ois = new OrderItemsService();
                List<OrderItemsTableJson> table = ois.toJsonArrayVerificationOrder(query);
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = table
                },
                            JsonRequestBehavior.AllowGet);
            }
            catch (DatabaseException ex)
            {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "OrderFormsController", "ProductLoad");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
            catch (Exception e)
            {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "OrderFormsController", "ProductLoad");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }
    }
}
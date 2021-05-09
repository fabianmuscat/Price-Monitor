using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PriceMonitor.ProductManager;

namespace PriceMonitor.EntityFramework
{
    public class DatabaseManager
    {
        private readonly List<object> _removedObjects;

        public DatabaseManager()
        {
            _removedObjects = new List<object>();
        }

        /// <summary>
        /// Search Handling Methods
        /// </summary>
        public Search GetSearch(string searchTerm)
        {
            using (PriceMonitorEntities db = new PriceMonitorEntities())
                return db.Searches.FirstOrDefault(s => s.SearchTerm.Equals(searchTerm));
        }

        public void AddSearchTerm(string searchTerm)
        {
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                int searches = db.Searches.Count(search => search.SearchTerm.Equals(searchTerm));
                if (searches == 0)
                {
                    Search newSearch = new Search();
                    newSearch.SearchTerm = searchTerm;
                    newSearch.Date = DateTime.Now;
                    db.Searches.Add(newSearch);

                    db.SaveChanges();
                }
            }
        }


        /// <summary>
        /// Websites Handling Methods
        /// </summary>
        private Website GetWebsite(string website, string url)
        {
            Uri uri = new Uri(url);
            string host = uri.Host;

            using (PriceMonitorEntities db = new PriceMonitorEntities())
                return db.Websites.FirstOrDefault(w => w.WebsiteName.ToLower().Equals(website.ToLower()) && w.URL.Equals(host));
        }

        public void AddWebsite(string website, string url)
        {
            string host = new Uri(url).Host;
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                Website web = db.Websites.FirstOrDefault(w => w.WebsiteName.Equals(website) && w.URL.Contains(host));
                if (web == null)
                {
                    Website websiteToAdd = new Website();
                    websiteToAdd.WebsiteName = website;
                    websiteToAdd.URL = host;

                    db.Websites.Add(websiteToAdd);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Activity Handling Methods
        /// </summary>
        public static List<string> GetLogs(User user)
        {
            List<string> userLogs = new List<string>();
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                Account account = GetAccount(user.Username);

                var logs = (
                    from log in db.Logs
                    join acc in db.Accounts
                        on log.AccountID equals acc.AccountID
                    join act in db.Activities
                        on log.ActivityID equals act.ActivityID
                    join sear in db.Searches
                        on log.SearchID equals sear.SearchID into sj
                    from sear in sj.DefaultIfEmpty() // left outer join (will show all logs even if the search id is null)
                    where acc.AccountID == account.AccountID && (log.Time >= DbFunctions.AddMinutes(DateTime.Now, -60))
                    orderby log.Time descending
                    select new
                    {
                        Activity = act.Type,
                        log.Time,
                        sear.SearchTerm,
                    });

                string activity = GetActivityType(ActivityType.Search);
                foreach (var log in logs.ToList())
                {
                    if (log.Activity.ToLower().Equals(activity.ToLower())) userLogs.Add($"{log.Activity} for '{log.SearchTerm}' @ {log.Time:HH:mm:ss tt}");
                    else userLogs.Add($"{log.Activity} @ {log.Time:HH:mm:ss tt}");
                }
            }

            return userLogs;
        }

        private static string GetActivityType(ActivityType type)
        {
            string activity = string.Empty;
            switch (type)
            {
                case ActivityType.Login: activity = "logged in"; break;
                case ActivityType.Logout: activity = "logged out"; break;
                case ActivityType.Search: activity = "searched"; break;
                case ActivityType.MonitoredProduct: activity = "monitored product"; break;
                case ActivityType.RemovedMonitoredProduct: activity = "removed monitored product"; break;
            }

            return activity;
        }

        public static void Log(ActivityType type, User user, string searchTerm = null)
        {
            string activity = GetActivityType(type);
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                Log log = new Log();
                Account account = GetAccount(user.Username);
                Activity activityType = db.Activities.FirstOrDefault(a => a.Type.ToLower().Equals(activity));
                Search search = db.Searches.FirstOrDefault(s => s.SearchTerm.Equals(searchTerm));

                log.AccountID = account.AccountID;
                log.ActivityID = activityType.ActivityID;
                log.Time = DateTime.Now;
                if (searchTerm != null)
                    log.SearchID = search.SearchID;

                db.Logs.Add(log);
                db.SaveChanges();
            }
        }
        
        /// <summary>
        /// Categories Handling Methods
        /// </summary>
        public List<string> GetCategories(string website)
        {
            List<string> strCategories = new List<string>();
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                Website websiteToCompare = db.Websites.First(web => web.WebsiteName.ToLower().Equals(website.ToLower()));
                List<Category> categories = db.Categories.Where(c => c.WebsiteID == websiteToCompare.WebsiteID).ToList();
                categories.ForEach(cat => strCategories.Add(cat.CategoryName));
            }

            return strCategories.Count == 0 ? null : strCategories;
        }

        private Category GetCategory(string category, int? websiteId = null)
        {
            Category returnCategory = new Category();
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                if (websiteId != null)
                    returnCategory = db.Categories.FirstOrDefault(cat =>
                        (cat.WebsiteID == websiteId) &&
                        (cat.CategoryName.ToLower().Equals(category.ToLower())));
                else
                    returnCategory = db.Categories.FirstOrDefault(cat => (cat.CategoryName.ToLower().Equals(category.ToLower())));
            }

            return returnCategory;
        }

        public void AddCategory(string category)
        {
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                int count = db.Categories.Count(c => c.CategoryName.ToLower().Equals(category.ToLower()));
                if (count == 0)
                {
                    Category newCategory = new Category();
                    newCategory.CategoryName = category;
                    db.Categories.Add(newCategory);
                    db.SaveChanges();
                }
            }
        }

        public void AddCategory(Dictionary<string, string> categories)
        {
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                foreach (KeyValuePair<string, string> valuePair in categories)
                {
                    string categoryName = valuePair.Key;
                    string website = valuePair.Value;

                    Website websiteToAdd = db.Websites.First(w => w.WebsiteName.ToLower().Equals(website.ToLower()));

                    Category newCategory = new Category();
                    newCategory.CategoryName = categoryName;
                    newCategory.WebsiteID = websiteToAdd.WebsiteID;
                    db.Categories.Add(newCategory);
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Products Handling Methods
        /// </summary>
        public List<Item> Search(string website, string toSearch, string category)
        {
            List<Item> productsToReturn = new List<Item>();
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                List<ProductsObtainedView> allProducts = db.ProductsObtainedViews.ToList();
                if (allProducts.Count != 0)
                {
                    allProducts.ForEach(product =>
                    {
                        // adding a product if parameters match the product's details in the database
                        if (product.SearchTerm.ToLower().Equals(toSearch.ToLower()) && product.Website.ToLower().Equals(website.ToLower()) && product.Category.Equals(category))
                        {
                            Item item = new Item(product.Product_Title, product.Subtotal.ToString(),
                                product.Shipping.ToString(), product.URL, product.Website, product.Category);
                            productsToReturn.Add(item);
                        }
                    });
                }
                else return null;
            }

            if (productsToReturn.Count == 0) productsToReturn = null;
            return productsToReturn;
        }
        
        private Product GetProduct(string title, string url)
        {
            using (PriceMonitorEntities db = new PriceMonitorEntities())
                return db.Products.FirstOrDefault(pro => pro.ProductTitle.Equals(title) && pro.URL.Equals(url));
        }

        private Product GetProduct(string url)
        {
            using (PriceMonitorEntities db = new PriceMonitorEntities())
                return db.Products.FirstOrDefault(pro => pro.URL.Equals(url));
        }

        public void AddProducts(List<Item> products, string website, string searchTerm, bool websiteNull = false)
        {
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {

                bool exists = false;
                void Add(bool isWebsiteNull)
                {
                    foreach (var retrievedProduct in products)
                    {
                        if (db.Products.ToList().Count != 0)
                            exists = db.Products.Any(pro => retrievedProduct.Url.Equals(pro.URL));

                        if (!exists)
                        {
                            Product productToAdd = new Product();
                            Website websiteToAdd = GetWebsite(website, retrievedProduct.Url);
                            Category category = isWebsiteNull
                                ? GetCategory(retrievedProduct.Category)
                                : GetCategory(retrievedProduct.Category, websiteToAdd.WebsiteID);

                            Search search = GetSearch(searchTerm);

                            if (search != null && category != null)
                            {
                                productToAdd.ProductTitle = retrievedProduct.Title;
                                productToAdd.Price = retrievedProduct.Price;
                                productToAdd.Shipping = retrievedProduct.Shipping;
                                productToAdd.URL = retrievedProduct.Url;
                                productToAdd.CategoryID = category.CategoryID;
                                productToAdd.WebsiteID = websiteToAdd.WebsiteID;
                                productToAdd.SearchID = search.SearchID;
                                db.Products.Add(productToAdd);
                            }
                        }
                    }
                    db.SaveChanges();

                    if (_removedObjects.Count > 0)
                    {
                        _removedObjects.ForEach(monProd =>
                        {
                            dynamic monProdDynamic = monProd; //dynamic is used to tell the compiler that this variable's data type can change as in this case, it is an anonymous object
                            object title = monProdDynamic.title;
                            Product pr = db.Products.FirstOrDefault(pro => pro.ProductTitle.Equals((string) title));

                            if (pr != null)
                            {
                                object accountId = monProdDynamic.acId;
                                object websiteId = monProdDynamic.webId;

                                MonitorProduct toAdd = new MonitorProduct();
                                toAdd.ProductID = pr.ProductID;
                                toAdd.AccountID = (int)accountId;
                                toAdd.WebsiteID = (int)websiteId;
                                toAdd.DateMonitored = DateTime.Now;
                                db.MonitorProducts.Add(toAdd);
                            }
                        });
                    }
                }

                Add(websiteNull);
                db.SaveChanges();
            }
        }

        public void RemoveProductsWithSearch(string searchTerm)
        {
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                Search toRemove = db.Searches.FirstOrDefault(search => search.SearchTerm.ToLower().Equals(searchTerm));
                if (toRemove != null)
                {
                    List<Product> productsToRemove = db.Products.Where(product => product.SearchID == toRemove.SearchID).ToList();
                    productsToRemove.ForEach(product =>
                    {
                        RemoveMonitoredProductsWithUrl(product.URL);
                        db.Products.Remove(product);
                    });
                    db.Searches.Remove(toRemove);
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Products Monitored Handling Methods
        /// </summary>
        public void AddMonitoredProduct(Item item, User user)
        {
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                Product product = GetProduct(item.Title, item.Url);
                Website website = GetWebsite(item.Website, item.Url);
                Account account = GetAccount(user.Username);

                if (!db.MonitorProducts.Any(mon => mon.ProductID == product.ProductID && mon.WebsiteID == website.WebsiteID && mon.AccountID == account.AccountID))
                {
                    MonitorProduct monitorProduct = new MonitorProduct();
                    monitorProduct.ProductID = product.ProductID;
                    monitorProduct.WebsiteID = website.WebsiteID;
                    monitorProduct.AccountID = account.AccountID;
                    monitorProduct.DateMonitored = DateTime.Now;
                
                    db.MonitorProducts.Add(monitorProduct);
                    db.SaveChanges();
                }
            }
        }

        public List<UserItem> GetMonitoredProducts(string username)
        {
            List<UserItem> toReturn = new List<UserItem>();
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                List<MonitoredProductsView> monitoredProducts = db.MonitoredProductsViews.Where(us => us.Account.Equals(username)).ToList();
                monitoredProducts.ForEach(mon =>
                {
                    toReturn.Add(new UserItem(mon.Product, mon.Subtotal, mon.Shipping, mon.Obtained_From, mon.Link, mon.Date_Monitored));
                });
            }

            return toReturn;
        }
       
        private void RemoveMonitoredProductsWithUrl(string url)
        {
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                Product product = GetProduct(url);
                List<MonitorProduct> monProds = new List<MonitorProduct>();
                if (product != null)
                {
                    monProds = db.MonitorProducts.Where(prod => prod.ProductID == product.ProductID).ToList();
                    if (monProds.Count > 0)
                    {
                        monProds.ForEach(monitoredProduct =>
                        {
                            string title = monitoredProduct.Product.ProductTitle;
                            int acId = (int) monitoredProduct.AccountID;
                            int webId = (int) monitoredProduct.WebsiteID;
                            _removedObjects.Add(new { title, acId, webId });
                            db.MonitorProducts.Remove(monitoredProduct);
                        });
                    }
                }

                db.SaveChanges();
            }
        }

        public void RemoveMonitoredProduct(UserItem item, User user)
        {
            using (PriceMonitorEntities db = new PriceMonitorEntities())
            {
                Account account = GetAccount(user.Username);
                Product product = GetProduct(item.Title, item.Url);

                MonitorProduct toRemove = db.MonitorProducts.FirstOrDefault(pro => pro.AccountID == account.AccountID && pro.ProductID == product.ProductID);

                db.MonitorProducts.Remove(toRemove);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Account Handling Method
        /// </summary>
        private static Account GetAccount(string username)
        {
            using (PriceMonitorEntities db = new PriceMonitorEntities())
                return db.Accounts.FirstOrDefault(ac => ac.Username.Equals(username));
        }
    }
}
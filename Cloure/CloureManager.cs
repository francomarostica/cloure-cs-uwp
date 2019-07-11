using Cloure.Core;
using Cloure.Modules.users;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Data.Json;
using Windows.Networking.Sockets;
using Windows.Services.Store;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cloure
{
    public static class CloureManager
    {
        public static CloureClient cloureClient = new CloureClient();

        private static string decimal_separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        private static object parameter { get; set; }
        private static ModuleInfo ModuleInfo { get; set; }
        private static Frame Frame;
        private static TextBlock textBlock;
        private static Grid grdLoader;
        private static int vencimientoDias = 0;
        private static string appToken = "";
        private static string userToken = "";
        private static JsonArray modulesGroupsArr;
        private static User loguedUser = null;
        private static string accountType = "";
        private static string primaryDomain = "";
        public static string currency_iso = "";
        public static string currency_symbol = "";
        public static string thousand_separator = "";
        private static string company_type = "";
        private static int show_payment_advice = 14;
        //private static readonly int BuildVersion = 36;
        public static bool account_data_saved = false;
        public static string lang = "en";
        private static string country = "us";

        //store products
        private static StoreContext context = null;
        private static JsonObject selectedProductSubscription = null;
        private static StoreProduct subscriptionStoreProduct;

        public static PackageVersion GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return version;
        }

        public static string GetAppVersionString()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        public static void SetAppToken(string AppToken)
        {
            appToken = AppToken;
        }

        public static void SetUserToken(string UserToken)
        {
            userToken = UserToken;
        }

        /// <summary>
        /// Navigate to page
        /// </summary>
        /// <param name="Page">The destination page</param>
        public static void Navigate(Type Page)
        {
            parameter = null;
            Frame.Navigate(Page);
        }

        public static string GetSystemDecimalSeparator()
        {
            return decimal_separator;
        }

        public static async void ShowDialog(string text)
        {
            var dialog = new MessageDialog(text);
            await dialog.ShowAsync();
        }

        public static void sendToast(string title, string stringContent)
        {
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            Windows.Data.Xml.Dom.XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(stringContent));
            Windows.Data.Xml.Dom.IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            Windows.Data.Xml.Dom.XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTime.Now.AddSeconds(4);
            ToastNotifier.Show(toast);
        }

        public static void ShowLoader()
        {
            grdLoader.Visibility = Visibility.Visible;
        }

        public static async Task<JsonObject> getLocales(string module)
        {
            JsonObject api_response = new JsonObject();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("topic", "get_locales"));
                cparams.Add(new CloureParam("module_name", module));
                string res = await ExecuteAsync(cparams);

                api_response = JsonObject.Parse(res);
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }

            return api_response;
        }

        public static void ShowLoader(string text)
        {
            grdLoader.Visibility = Visibility.Visible;
            foreach(UIElement uIElement in grdLoader.Children)
            {
                if(uIElement.GetType() == typeof(TextBlock))
                {
                    TextBlock tb = (TextBlock)uIElement;
                    if (tb.Name == "txtLoaderText") tb.Text = text;
                }
            }
        }

        public static void HideLoader()
        {
            grdLoader.Visibility = Visibility.Collapsed;
        }

        public static void NumberInput(object sender)
        {
            TextBox textBox = (TextBox)sender;
            if (decimal_separator == ",") textBox.Text = textBox.Text.Replace(".", ",");
            textBox.SelectionStart = textBox.Text.Length;
        }

        public static void SelectAddress(object sender)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null) tb.SelectAll();
        }

        public static double ParseNumber(JsonValue jsonValue)
        {
            double res = 0;

            if (jsonValue.ValueType == JsonValueType.Number)
            {
                res = jsonValue.GetNumber();
            }
            else if(jsonValue.ValueType == JsonValueType.String)
            {
                string number_str = jsonValue.GetString();
                if (decimal_separator == "") decimal_separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                if (decimal_separator == ",")
                {
                    //in API double always is with point
                    number_str = number_str.Replace(".", ",");
                    double.TryParse(number_str, out res);
                }
                else if(decimal_separator == ".")
                {
                    number_str = number_str.Replace(",", ".");
                    double.TryParse(number_str, out res);
                }
            }

            return res;
        }

        public static bool ParseBoolObject(object val)
        {
            bool res = false;

            if (val != null)
            {
                if (val.GetType() == typeof(string))
                {
                    string strval = (string)val;
                    strval = strval.ToLower();
                    if (strval == "true" || strval == "on" || strval == "1" || strval=="yes" || strval == "allow")
                    {
                        res = true;
                    }
                    else
                    {
                        res = false;
                    }
                }
                else if (val.GetType() == typeof(JsonValue))
                {
                    JsonValue jsonValue = (JsonValue)val;
                    if (jsonValue.ValueType == JsonValueType.Number)
                    {
                        if (jsonValue.GetNumber() == 1)
                            res = true;
                        else
                            res = false;
                    }
                }
            }

            return res;
        }

        public static int ParseInt(JsonValue jsonValue)
        {
            int res = 0;

            if (jsonValue.ValueType == JsonValueType.Number)
            {
                res = (int)jsonValue.GetNumber();
            }
            else if (jsonValue.ValueType == JsonValueType.String)
            {
                string number_str = jsonValue.GetString();
                int.TryParse(number_str, out res);
            }

            return res;
        }

        public static DateTime ParseDate(JsonValue jsonValue)
        {
            DateTime res = new DateTime();

            if (jsonValue.ValueType == JsonValueType.String)
            {
                string value = jsonValue.GetString();
                res = DateTime.Parse(value, CultureInfo.InvariantCulture);
            }

            return res;
        }

        public static async Task LoadAccountInfo()
        {
            List<CloureParam> cloureParams = new List<CloureParam>();
            cloureParams.Add(new CloureParam("topic", "get_account_info"));

            try
            {
                string api_response = await ExecuteAsync(cloureParams);
                JsonObject api_response_obj = JsonObject.Parse(api_response);
                currency_symbol = api_response_obj.GetNamedString("currency_symbol");
                currency_iso = api_response_obj.GetNamedString("currency_iso");
                decimal_separator = api_response_obj.GetNamedString("decimal_separator");
                thousand_separator = api_response_obj.GetNamedString("thousand_separator");
                show_payment_advice = ParseInt(api_response_obj.GetNamedValue("show_advice_days"));
                company_type = api_response_obj.GetNamedString("business_type_id");
                country = api_response_obj.GetNamedString("country_id");
                vencimientoDias = ParseInt(api_response_obj.GetNamedValue("vencimiento_dias"));
                
                //CloureManager.ShowDialog(vencimientoDias.ToString());

            }
            catch (Exception e)
            {
                var dialog = new MessageDialog(e.Message);
                await dialog.ShowAsync();
            }
        }

        public static void Navigate(Type Page, object Parameter)
        {
            parameter = null;
            Frame.Navigate(Page, Parameter);
        }

        public static int getVencimientoDias()
        {
            return vencimientoDias;
        }

        public static void GoBack()
        {
            parameter = null;
            Frame.GoBack();
        }

        public static void GoBack(object Parameter)
        {
            parameter = Parameter;
            Frame.GoBack();
        }

        public static object GetParameter()
        {
            return parameter;
        }

        public static ModuleInfo GetModuleInfo()
        {
            return ModuleInfo;
        }

        public static void SetModuleInfo(ModuleInfo moduleInfo)
        {
            ModuleInfo = moduleInfo;
        }

        public static void SetFrame(Frame frame)
        {
            Frame = frame;
        }

        public static void SetLoaderUI(Grid grid)
        {
            grdLoader = grid;
        }

        public static void SetTitleControl(TextBlock textBlock)
        {
            CloureManager.textBlock = textBlock;
        }

        public static void SetTitle(string Title)
        {
            textBlock.Text = Title;
        }

        public static string getAccountType()
        {
            return cloureClient.SubscriptionType;
            //return accountType;
        }

        public static int getPaymentAdviceDays()
        {
            return show_payment_advice;
        }

        public static string getCompanyType()
        {
            return company_type;
        }

        public static User getLoguedUser()
        {
            return loguedUser;
        }

        public static JsonArray getModulesGroups()
        {
            return modulesGroupsArr;
        }

        public static string getPrimaryDomain()
        {
            return cloureClient.PrimaryDomain;
        }
        
        public static async Task<string> ExecuteAsync(List<CloureParam> cparams)
        {
            string Response = "";

            //string url_str = "https://cloure.com/api/v1/";
            string url_str = "http://cloure.test/api/v1/";
            MultipartFormDataContent dataContent = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url_str);
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));

            if (appToken != null)
            {
                if (appToken.Length == 32)
                    cparams.Add(new CloureParam("app_token", appToken));
            }
            if (userToken != null)
            {
                if (userToken.Length == 32) cparams.Add(new CloureParam("user_token", userToken));
            }

            cparams.Add(new CloureParam("language", lang));
            cparams.Add(new CloureParam("referer", "uwp"));
            cparams.Add(new CloureParam("referer_version", cloureClient.BuildVersion));

            try
            {
                foreach (CloureParam param in cparams)
                {
                    if (param != null)
                    {
                        if (param.name == "module") url_str += param.value + "/";
                        if (param.name == "topic") url_str += param.value;

                        if (param.value.GetType() == typeof(string))
                        {
                            HttpContent httpContent = new StringContent((string)param.value);
                            dataContent.Add(httpContent, param.name);
                        }
                        else if (param.value.GetType() == typeof(double))
                        {
                            HttpContent httpContent = new StringContent(((double)param.value).ToString("F2", CultureInfo.InvariantCulture));
                            dataContent.Add(httpContent, param.name);
                        }
                        else if (param.value.GetType() == typeof(int))
                        {
                            HttpContent httpContent = new StringContent(((int)param.value).ToString());
                            dataContent.Add(httpContent, param.name);
                        }
                        else if (param.value.GetType() == typeof(CloureImage))
                        {
                            string option = "api_image_object";
                            CloureImage cloureImage = (CloureImage)param.value;
                            HttpContent httpContent = new StringContent("");

                            if (option == "byte")
                            {
                                httpContent = new ByteArrayContent(cloureImage.GetBytes());
                                dataContent.Add(httpContent, param.name, "image");
                            }
                            else if (option == "base64")
                            {
                                string base64String = Convert.ToBase64String(cloureImage.GetBytes());
                                httpContent = new StringContent(base64String);
                                dataContent.Add(httpContent, param.name);
                            }
                            else if (option == "api_image_object")
                            {
                                httpContent = new StringContent(cloureImage.ToJSONString());
                                dataContent.Add(httpContent, param.name);
                            }
                        }
                        else if (param.value.GetType() == typeof(List<CloureImage>))
                        {
                            List<CloureImage> cloureImages = (List<CloureImage>)param.value;
                            string str_content = "[";
                            foreach (CloureImage cloureImage in cloureImages)
                            {
                                str_content += cloureImage.ToJSONString() + ",";
                            }
                            str_content = str_content.TrimEnd(',');
                            str_content += "]";
                            HttpContent httpContent = new StringContent(str_content);
                            dataContent.Add(httpContent, param.name);
                        }
                        else if (param.value.GetType() == typeof(bool))
                        {
                            bool value = (bool)param.value;
                            if (value)
                            {
                                HttpContent httpContent = new StringContent("1");
                                dataContent.Add(httpContent, param.name);
                            }
                            else
                            {
                                HttpContent httpContent = new StringContent("0");
                                dataContent.Add(httpContent, param.name);
                            }

                        }
                    }
                }

                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(url_str, dataContent);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Response = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                Response = e.Message.ToString();
                var dialog = new MessageDialog(Response);
                await dialog.ShowAsync();
            }

            return Response;
        }


        public static async Task<bool> registerAccount(
            string name, 
            string lastName,
            string eMail,
            string password,
            string repeatPassword,
            string companyName,
            string companyTypeId,
            string desiredCloureURL,
            string countryId
            )
        {
            bool result = false;

            List<CloureParam> cloureParams = new List<CloureParam>();
            cloureParams.Add(new CloureParam("topic", "register_account"));
            cloureParams.Add(new CloureParam("name", name));
            cloureParams.Add(new CloureParam("last_name", lastName));
            cloureParams.Add(new CloureParam("email", eMail));
            cloureParams.Add(new CloureParam("password", password));
            cloureParams.Add(new CloureParam("repeat_password", repeatPassword));
            cloureParams.Add(new CloureParam("company_name", companyName));
            cloureParams.Add(new CloureParam("company_type", companyTypeId));
            cloureParams.Add(new CloureParam("cloure_url", desiredCloureURL));
            cloureParams.Add(new CloureParam("country_id", countryId));

            string api_response = await ExecuteAsync(cloureParams);
            JsonObject api_response_obj = JsonObject.Parse(api_response);
            string ApiError = api_response_obj.GetNamedString("Error");

            if (ApiError == "")
            {
                JsonObject ApiResponse = api_response_obj.GetNamedObject("Response");
                appToken = ApiResponse.GetNamedString("app_token");
                userToken = ApiResponse.GetNamedString("user_token");
                accountType = ApiResponse.GetNamedString("account_type");
                modulesGroupsArr = ApiResponse.GetNamedArray("modules_groups");
                loguedUser = await Users.GetUser(userToken);
                primaryDomain = ApiResponse.GetNamedString("primary_domain");

                result = true;
            }
            else
            {
                var dialog = new MessageDialog(ApiError);
                await dialog.ShowAsync();
            }

            return result;
        }

        public static async Task<bool> RequestPurchase(JsonObject priceObject)
        {
            bool response = false;

            selectedProductSubscription = priceObject;
            string storeProductId = priceObject.GetNamedString("ms_product_id");

            System.Diagnostics.Debug.WriteLine("MS-Store product ID requested:" + storeProductId);
            try
            {
                if (context == null)
                {
                    context = StoreContext.GetDefault();
                }

                bool userOwnsSubscription = await CheckIfUserHasSubscriptionAsync(storeProductId);
                if (userOwnsSubscription)
                {
                    // Unlock all the subscription add-on features here.
                    
                    return true;
                }

                StoreProductQueryResult result = await context.GetAssociatedStoreProductsAsync(new string[] { "Durable" });

                if (result.ExtendedError != null)
                {
                    System.Diagnostics.Debug.WriteLine("Something went wrong while getting the add-ons. ExtendedError:" + result.ExtendedError);
                    return false;
                }

                // Look for the product that represents the subscription.
                foreach (var item in result.Products)
                {
                    System.Diagnostics.Debug.WriteLine(item.Value);
                    StoreProduct product = item.Value;
                    if (product.StoreId == storeProductId) subscriptionStoreProduct = product;
                }

                if (subscriptionStoreProduct == null)
                {
                    System.Diagnostics.Debug.WriteLine("The subscription was not found.");
                }
                else
                {
                    response = await PromptUserToPurchaseAsync();
                }

                return response;
            }
            catch (Exception ex)
            {
                ShowDialog("An error occured: " + ex.Message);
            }

            return response;
        }

        private static async Task<bool> CheckIfUserHasSubscriptionAsync(string msProductId)
        {
            if (context == null) context = StoreContext.GetDefault();

            StoreAppLicense appLicense = await context.GetAppLicenseAsync();

            // Check if the customer has the rights to the subscription.
            foreach (var addOnLicense in appLicense.AddOnLicenses)
            {
                StoreLicense license = addOnLicense.Value;
                if (license.SkuStoreId.StartsWith(msProductId))
                {
                    if (license.IsActive)
                    {
                        // The expiration date is available in the license.ExpirationDate property.
                        accountType = "starter";
                        updateCloureAccount(license.ExpirationDate);
                        return true;
                    }
                }
            }

            // The customer does not have a license to the subscription.
            return false;
        }

        private static async Task<bool> PromptUserToPurchaseAsync()
        {
            bool response = false;

            // Request a purchase of the subscription product. If a trial is available it will be offered 
            // to the customer. Otherwise, the non-trial SKU will be offered.
            StorePurchaseResult result = await subscriptionStoreProduct.RequestPurchaseAsync();

            // Capture the error message for the operation, if any.
            string extendedError = string.Empty;
            if (result.ExtendedError != null)
            {
                extendedError = result.ExtendedError.Message;
            }

            switch (result.Status)
            {
                case StorePurchaseStatus.Succeeded:
                    accountType = "starter";
                    DateTimeOffset vencimiento = new DateTimeOffset();
                    updateCloureAccount(vencimiento);
                    response = true;
                    break;

                case StorePurchaseStatus.NotPurchased:
                    System.Diagnostics.Debug.WriteLine("The purchase did not complete. " +
                        "The customer may have cancelled the purchase. ExtendedError: " + extendedError);
                    break;

                case StorePurchaseStatus.ServerError:
                case StorePurchaseStatus.NetworkError:
                    System.Diagnostics.Debug.WriteLine("The purchase was unsuccessful due to a server or network error. " +
                        "ExtendedError: " + extendedError);
                    break;

                case StorePurchaseStatus.AlreadyPurchased:
                    System.Diagnostics.Debug.WriteLine("The customer already owns this subscription." +
                            "ExtendedError: " + extendedError);
                    break;
            }

            return response;
        }

        private static async void updateCloureAccount(DateTimeOffset vencimiento)
        {
            try
            {
                DateTime venc = vencimiento.DateTime;

                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("topic", "update_subscription_ms"));
                cparams.Add(new CloureParam("apptoken", appToken));
                cparams.Add(new CloureParam("product_id", accountType));
                cparams.Add(new CloureParam("period_id", ""));
                cparams.Add(new CloureParam("vencimiento", venc.ToString("yyyy-MM-dd")));
                cparams.Add(new CloureParam("store_in_app", ""));

                string res = await ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {

                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
        }

    }
}

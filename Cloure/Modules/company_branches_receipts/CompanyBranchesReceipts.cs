using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.company_branches_receipts
{
    public static class CompanyBranchesReceipts
    {
        public static async Task<List<CompanyBranchReceipt>> GetList()
        {
            List<CompanyBranchReceipt> receipts = new List<CompanyBranchReceipt>();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "company_branches_receipts"));
                cparams.Add(new CloureParam("topic", "listar"));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject api_response = api_result.GetNamedObject("Response");
                    JsonArray registers = api_response.GetNamedArray("Registros");

                    foreach (JsonValue jsonValue in registers)
                    {
                        JsonObject register = jsonValue.GetObject();
                        CompanyBranchReceipt receipt = new CompanyBranchReceipt();
                        receipt.Id = (int)register.GetNamedNumber("Id");
                        receipt.Name = register.GetNamedString("Nombre");
                        receipts.Add(receipt);
                    }
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

            return receipts;
        }
    }
}
